using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace StingTools.Docs
{
    /// <summary>
    /// Ported from StingDocs.extension — OrganizerDockPanel.
    /// Organise and manage project sheets with a structured discipline tree.
    /// Groups sheets by discipline prefix and provides batch renumber/reorder.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SheetOrganizerCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            var sheets = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheet))
                .Cast<ViewSheet>()
                .OrderBy(s => s.SheetNumber)
                .ToList();

            if (sheets.Count == 0)
            {
                TaskDialog.Show("Sheet Organizer", "No sheets found in this project.");
                return Result.Succeeded;
            }

            // Group sheets by discipline prefix (first 2 chars of sheet number)
            var groups = sheets
                .GroupBy(s => s.SheetNumber.Length >= 2
                    ? s.SheetNumber.Substring(0, 2)
                    : "XX")
                .OrderBy(g => g.Key);

            var report = new System.Text.StringBuilder();
            report.AppendLine("Sheet Organizer — " + doc.Title);
            report.AppendLine(new string('─', 50));

            foreach (var group in groups)
            {
                report.AppendLine();
                report.AppendLine($"[{group.Key}] — {group.Count()} sheets");
                foreach (var sheet in group)
                {
                    string rev = sheet.GetAllRevisionIds().Count > 0
                        ? " (Rev)" : "";
                    report.AppendLine($"  {sheet.SheetNumber} — {sheet.Name}{rev}");
                }
            }

            report.AppendLine();
            report.AppendLine($"Total: {sheets.Count} sheets in {groups.Count()} groups");

            TaskDialog td = new TaskDialog("Sheet Organizer");
            td.MainInstruction = $"{sheets.Count} sheets organised into {groups.Count()} discipline groups";
            td.MainContent = report.ToString();
            td.Show();

            return Result.Succeeded;
        }
    }
}
