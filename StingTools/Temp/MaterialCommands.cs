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
            string csvPath = StingToolsApp.FindDataFile("BLE_MATERIALS.csv");

            if (csvPath == null)
            {
                TaskDialog.Show("Create BLE Materials",
                    "BLE_MATERIALS.csv not found in the data directory.\n" +
                    $"Searched: {StingToolsApp.DataPath}");
                return Result.Failed;
            }

            // Skip comment line (row 0: "# v2.2 ...") and header row
            var lines = File.ReadAllLines(csvPath)
                .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("#"))
                .Skip(1) // skip header
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
                    // BLE_MATERIALS columns: SOURCE_SHEET(0), MAT_DISCIPLINE(1),
                    // MAT_ISO_19650_ID(2), MAT_CODE(3), MAT_ELEMENT_TYPE(4),
                    // MAT_CATEGORY(5), MAT_NAME(6), ...
                    string[] cols = StingToolsApp.ParseCsvLine(line);
                    if (cols.Length < 7) continue;

                    string matName = cols[6].Trim();
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
            string csvPath = StingToolsApp.FindDataFile("MEP_MATERIALS.csv");

            if (csvPath == null)
            {
                TaskDialog.Show("Create MEP Materials",
                    "MEP_MATERIALS.csv not found in the data directory.\n" +
                    $"Searched: {StingToolsApp.DataPath}");
                return Result.Failed;
            }

            // Skip comment line (row 0: "# v2.2 ...") and header row
            var lines = File.ReadAllLines(csvPath)
                .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("#"))
                .Skip(1) // skip header
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
                    // MEP_MATERIALS columns: same layout as BLE — MAT_NAME is at index 6
                    string[] cols = StingToolsApp.ParseCsvLine(line);
                    if (cols.Length < 7) continue;

                    string matName = cols[6].Trim();
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
    }
}
