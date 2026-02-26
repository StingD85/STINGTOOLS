using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace StingTools.Docs
{
    /// <summary>
    /// Organise views by discipline, type, and level. Reports unplaced views
    /// and suggests cleanup actions.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    public class ViewOrganizerCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            var views = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .Where(v => !v.IsTemplate && v.CanBePrinted)
                .ToList();

            var byType = views
                .GroupBy(v => v.ViewType)
                .OrderBy(g => g.Key.ToString());

            int placed = views.Count(v =>
            {
                var sheets = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewSheet))
                    .Cast<ViewSheet>()
                    .Where(s => s.GetAllPlacedViews().Contains(v.Id));
                return sheets.Any();
            });

            var report = new System.Text.StringBuilder();
            report.AppendLine("View Organizer — " + doc.Title);
            report.AppendLine(new string('─', 50));

            foreach (var group in byType)
            {
                report.AppendLine($"\n{group.Key} — {group.Count()} views");
                foreach (var v in group.OrderBy(x => x.Name).Take(20))
                {
                    report.AppendLine($"  {v.Name}");
                }
                if (group.Count() > 20)
                    report.AppendLine($"  ... and {group.Count() - 20} more");
            }

            report.AppendLine($"\nTotal: {views.Count} views ({placed} on sheets, {views.Count - placed} unplaced)");

            TaskDialog td = new TaskDialog("View Organizer");
            td.MainInstruction = $"{views.Count} views found";
            td.MainContent = report.ToString();
            td.Show();

            return Result.Succeeded;
        }
    }
}
