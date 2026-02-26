using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using StingTools.Core;

namespace StingTools.Tags
{
    /// <summary>
    /// Validate existing tags for completeness and correct token counts.
    /// Reports incomplete, malformed, or missing tags by category.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    public class ValidateTagsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            var collector = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType();

            var knownCategories = new HashSet<string>(TagConfig.DiscMap.Keys);
            int total = 0;
            int valid = 0;
            int incomplete = 0;
            int missing = 0;
            var issuesByCategory = new Dictionary<string, int>();

            foreach (Element el in collector)
            {
                string catName = ParameterHelpers.GetCategoryName(el);
                if (string.IsNullOrEmpty(catName) || !knownCategories.Contains(catName))
                    continue;

                total++;
                string tag = ParameterHelpers.GetString(el, "ASS_TAG_1_TXT");

                if (string.IsNullOrEmpty(tag))
                {
                    missing++;
                    IncrementCategory(issuesByCategory, catName);
                }
                else if (TagConfig.TagIsComplete(tag))
                {
                    valid++;
                }
                else
                {
                    incomplete++;
                    IncrementCategory(issuesByCategory, catName);
                }
            }

            var report = new StringBuilder();
            report.AppendLine("Tag Validation Report");
            report.AppendLine(new string('─', 45));
            report.AppendLine($"Total taggable elements:  {total}");
            report.AppendLine($"Valid (8 tokens):         {valid}");
            report.AppendLine($"Incomplete:               {incomplete}");
            report.AppendLine($"Missing:                  {missing}");

            if (issuesByCategory.Count > 0)
            {
                report.AppendLine();
                report.AppendLine("Issues by category:");
                foreach (var kvp in issuesByCategory.OrderByDescending(x => x.Value))
                {
                    report.AppendLine($"  {kvp.Key,-25} {kvp.Value} issues");
                }
            }

            double pct = total > 0 ? (valid * 100.0 / total) : 0;
            report.AppendLine();
            report.AppendLine($"Compliance: {pct:F1}%");

            TaskDialog td = new TaskDialog("Validate Tags");
            td.MainInstruction = $"Tag compliance: {pct:F1}% ({valid}/{total})";
            td.MainContent = report.ToString();
            td.Show();

            return Result.Succeeded;
        }

        private static void IncrementCategory(Dictionary<string, int> dict, string key)
        {
            if (dict.ContainsKey(key))
                dict[key]++;
            else
                dict[key] = 1;
        }
    }
}
