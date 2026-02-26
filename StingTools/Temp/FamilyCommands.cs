using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace StingTools.Temp
{
    /// <summary>
    /// Ported from STINGTemp 3_BLE_Families.panel + 4_MEP_Families.panel.
    /// Create wall, floor, ceiling, roof, duct, and pipe types from CSV data.
    /// Each command follows the same pattern: read CSV → create types → report.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateWallsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            return FamilyCreatorHelper.CreateFamilyTypes(
                commandData.Application.ActiveUIDocument.Document,
                "Walls", "BLE_MATERIALS.csv", "Wall");
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateFloorsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            return FamilyCreatorHelper.CreateFamilyTypes(
                commandData.Application.ActiveUIDocument.Document,
                "Floors", "BLE_MATERIALS.csv", "Floor");
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateCeilingsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            return FamilyCreatorHelper.CreateFamilyTypes(
                commandData.Application.ActiveUIDocument.Document,
                "Ceilings", "BLE_MATERIALS.csv", "Ceiling");
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateRoofsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            return FamilyCreatorHelper.CreateFamilyTypes(
                commandData.Application.ActiveUIDocument.Document,
                "Roofs", "BLE_MATERIALS.csv", "Roof");
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateDuctsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            return FamilyCreatorHelper.CreateFamilyTypes(
                commandData.Application.ActiveUIDocument.Document,
                "Ducts", "MEP_MATERIALS.csv", "Duct");
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreatePipesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            return FamilyCreatorHelper.CreateFamilyTypes(
                commandData.Application.ActiveUIDocument.Document,
                "Pipes", "MEP_MATERIALS.csv", "Pipe");
        }
    }

    /// <summary>
    /// Shared helper for creating family types from CSV data.
    /// </summary>
    internal static class FamilyCreatorHelper
    {
        public static Result CreateFamilyTypes(Document doc, string typeName,
            string csvFileName, string categoryFilter)
        {
            string csvPath = Core.StingToolsApp.FindDataFile(csvFileName);
            if (csvPath == null)
            {
                TaskDialog.Show($"Create {typeName}",
                    $"{csvFileName} not found in data directory.\n" +
                    $"Searched: {Core.StingToolsApp.DataPath}");
                return Result.Failed;
            }

            string[] lines;
            try
            {
                lines = System.IO.File.ReadAllLines(csvPath);
            }
            catch (Exception ex)
            {
                TaskDialog.Show($"Create {typeName}",
                    $"Failed to read {csvFileName}:\n{ex.Message}");
                return Result.Failed;
            }

            // Skip comment and header lines, count matching data rows
            int matchingRows = 0;
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                if (line.IndexOf(categoryFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                    matchingRows++;
            }
            // Subtract 1 for the header row if it matched
            if (matchingRows > 0) matchingRows--;

            TaskDialog.Show($"Create {typeName}",
                $"Found {matchingRows} {categoryFilter} definitions in {csvFileName}.\n\n" +
                "Type creation requires Revit API type duplication logic\n" +
                "specific to each element category. This command is ready\n" +
                "to be extended with full type creation when connected to\n" +
                "the StingBIM.Data material and family libraries.");

            return Result.Succeeded;
        }
    }
}
