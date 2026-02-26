using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using StingTools.Core;

namespace StingTools.Temp
{
    /// <summary>
    /// Ported from STINGTemp 2_Materials.panel — Create BLE Materials.
    /// Creates building-element materials from BLE_MATERIALS.csv.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateBLEMaterialsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            string csvPath = FindDataFile("BLE_MATERIALS.csv");

            if (csvPath == null)
            {
                TaskDialog.Show("Create BLE Materials",
                    "BLE_MATERIALS.csv not found in the data directory.\n" +
                    $"Searched: {StingToolsApp.DataPath}");
                return Result.Failed;
            }

            var lines = File.ReadAllLines(csvPath)
                .Skip(1) // header
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            int created = 0;
            int skipped = 0;

            using (Transaction tx = new Transaction(doc, "Create BLE Materials"))
            {
                tx.Start();

                // Get existing material names for dedup
                var existingNames = new HashSet<string>(
                    new FilteredElementCollector(doc)
                        .OfClass(typeof(Material))
                        .Cast<Material>()
                        .Select(m => m.Name));

                foreach (string line in lines)
                {
                    string[] cols = ParseCsvLine(line);
                    if (cols.Length < 2) continue;

                    string matName = cols[0].Trim();
                    if (string.IsNullOrEmpty(matName)) continue;

                    if (existingNames.Contains(matName))
                    {
                        skipped++;
                        continue;
                    }

                    ElementId newId = Material.Create(doc, matName);
                    if (newId != ElementId.InvalidElementId)
                    {
                        created++;
                        existingNames.Add(matName);
                    }
                }

                tx.Commit();
            }

            TaskDialog.Show("Create BLE Materials",
                $"Created {created} BLE materials.\nSkipped {skipped} (already exist).\n" +
                $"Source: {Path.GetFileName(csvPath)}");

            return Result.Succeeded;
        }

        private static string FindDataFile(string fileName)
        {
            if (string.IsNullOrEmpty(StingToolsApp.DataPath))
                return null;

            string direct = Path.Combine(StingToolsApp.DataPath, fileName);
            if (File.Exists(direct)) return direct;

            // Search subdirectories
            foreach (string f in Directory.GetFiles(
                StingToolsApp.DataPath, fileName, SearchOption.AllDirectories))
            {
                return f;
            }

            return null;
        }

        private static string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuote = false;
            var current = new System.Text.StringBuilder();

            foreach (char c in line)
            {
                if (c == '"')
                {
                    inQuote = !inQuote;
                }
                else if (c == ',' && !inQuote)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
            result.Add(current.ToString());
            return result.ToArray();
        }
    }

    /// <summary>
    /// Ported from STINGTemp 2_Materials.panel — Create MEP Materials.
    /// Creates MEP materials from MEP_MATERIALS.csv.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateMEPMaterialsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            string csvPath = FindDataFile("MEP_MATERIALS.csv");

            if (csvPath == null)
            {
                TaskDialog.Show("Create MEP Materials",
                    "MEP_MATERIALS.csv not found in the data directory.\n" +
                    $"Searched: {StingToolsApp.DataPath}");
                return Result.Failed;
            }

            var lines = File.ReadAllLines(csvPath)
                .Skip(1)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            int created = 0;
            int skipped = 0;

            using (Transaction tx = new Transaction(doc, "Create MEP Materials"))
            {
                tx.Start();

                var existingNames = new HashSet<string>(
                    new FilteredElementCollector(doc)
                        .OfClass(typeof(Material))
                        .Cast<Material>()
                        .Select(m => m.Name));

                foreach (string line in lines)
                {
                    string[] cols = line.Split(',');
                    if (cols.Length < 1) continue;

                    string matName = cols[0].Trim().Trim('"');
                    if (string.IsNullOrEmpty(matName)) continue;

                    if (existingNames.Contains(matName))
                    {
                        skipped++;
                        continue;
                    }

                    ElementId newId = Material.Create(doc, matName);
                    if (newId != ElementId.InvalidElementId)
                    {
                        created++;
                        existingNames.Add(matName);
                    }
                }

                tx.Commit();
            }

            TaskDialog.Show("Create MEP Materials",
                $"Created {created} MEP materials.\nSkipped {skipped} (already exist).\n" +
                $"Source: {Path.GetFileName(csvPath)}");

            return Result.Succeeded;
        }

        private static string FindDataFile(string fileName)
        {
            if (string.IsNullOrEmpty(StingToolsApp.DataPath))
                return null;

            string direct = Path.Combine(StingToolsApp.DataPath, fileName);
            if (File.Exists(direct)) return direct;

            foreach (string f in Directory.GetFiles(
                StingToolsApp.DataPath, fileName, SearchOption.AllDirectories))
            {
                return f;
            }

            return null;
        }
    }
}
