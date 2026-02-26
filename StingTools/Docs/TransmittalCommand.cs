using System;
using System.Linq;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace StingTools.Docs
{
    /// <summary>
    /// Create ISO 19650-compliant document transmittal records.
    /// Lists all sheets with their revision status for transmittal documentation.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    public class TransmittalCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            var sheets = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheet))
                .Cast<ViewSheet>()
                .OrderBy(s => s.SheetNumber)
                .ToList();

            if (sheets.Count == 0)
            {
                TaskDialog.Show("Document Transmittal", "No sheets found.");
                return Result.Succeeded;
            }

            string projectName = doc.ProjectInformation?.Name ?? "Unnamed";
            string projectNumber = doc.ProjectInformation?.Number ?? "000";

            var report = new StringBuilder();
            report.AppendLine("═══════════════════════════════════════════════");
            report.AppendLine("  DOCUMENT TRANSMITTAL — ISO 19650");
            report.AppendLine("═══════════════════════════════════════════════");
            report.AppendLine($"  Project: {projectName}");
            report.AppendLine($"  Number:  {projectNumber}");
            report.AppendLine($"  Date:    {DateTime.Now:yyyy-MM-dd}");
            report.AppendLine($"  Sheets:  {sheets.Count}");
            report.AppendLine("───────────────────────────────────────────────");

            foreach (var sheet in sheets)
            {
                string revId = sheet.GetCurrentRevision() != ElementId.InvalidElementId
                    ? "Rev" : "---";
                report.AppendLine(
                    $"  {sheet.SheetNumber,-12} {sheet.Name,-30} {revId}");
            }

            report.AppendLine("───────────────────────────────────────────────");
            report.AppendLine($"  Total: {sheets.Count} documents");

            TaskDialog td = new TaskDialog("Document Transmittal");
            td.MainInstruction = "ISO 19650 Document Transmittal";
            td.MainContent = report.ToString();
            td.Show();

            return Result.Succeeded;
        }
    }
}
