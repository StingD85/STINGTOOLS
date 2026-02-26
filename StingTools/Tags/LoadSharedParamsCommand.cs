using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using StingTools.Core;

namespace StingTools.Tags
{
    /// <summary>
    /// Ported from shared_params.py LoadSharedParams logic.
    /// Bind shared parameters (universal + discipline-specific) to project categories.
    /// Pass 1: 17 universal ASS_MNG parameters → all 53 categories.
    /// Pass 2: discipline-specific tag containers → correct category subsets.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class LoadSharedParamsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Autodesk.Revit.ApplicationServices.Application app =
                commandData.Application.Application;

            // Check for shared parameter file
            string spFile = app.SharedParametersFilename;
            if (string.IsNullOrEmpty(spFile) || !File.Exists(spFile))
            {
                TaskDialog.Show("Load Shared Params",
                    "No shared parameter file is set in Revit.\n\n" +
                    "Go to Manage → Shared Parameters and set the file path to " +
                    "MR_PARAMETERS.txt first.");
                return Result.Failed;
            }

            DefinitionFile defFile = app.OpenSharedParameterFile();
            if (defFile == null)
            {
                TaskDialog.Show("Load Shared Params",
                    "Could not open shared parameter file:\n" + spFile);
                return Result.Failed;
            }

            int pass1Bound = 0;
            int pass1Skipped = 0;
            int pass2Bound = 0;
            int pass2Skipped = 0;

            using (Transaction tx = new Transaction(doc, "STING Load Shared Params"))
            {
                tx.Start();

                // Pass 1: Universal parameters → all categories
                CategorySet allCats = BuildCategorySet(doc, SharedParamGuids.AllCategories);

                foreach (string paramName in SharedParamGuids.UniversalParams)
                {
                    ExternalDefinition extDef = FindDefinition(defFile, paramName);
                    if (extDef == null)
                    {
                        pass1Skipped++;
                        continue;
                    }

                    InstanceBinding binding = app.Create.NewInstanceBinding(allCats);
                    bool result = doc.ParameterBindings.Insert(
                        extDef, binding, BuiltInParameterGroup.INVALID);
                    if (result)
                        pass1Bound++;
                    else
                        pass1Skipped++;
                }

                // Pass 2: Discipline-specific parameters
                // Use the known discipline params from SharedParamGuids
                foreach (var kvp in SharedParamGuids.ParamGuids)
                {
                    if (SharedParamGuids.UniversalParams.Contains(kvp.Key))
                        continue;

                    ExternalDefinition extDef = FindDefinition(defFile, kvp.Key);
                    if (extDef == null)
                    {
                        pass2Skipped++;
                        continue;
                    }

                    // Bind to all categories (simplified — full impl would use
                    // DisciplineParams mapping from shared_params.py)
                    CategorySet cats = BuildCategorySet(doc, SharedParamGuids.AllCategories);
                    InstanceBinding binding = app.Create.NewInstanceBinding(cats);
                    bool result = doc.ParameterBindings.Insert(
                        extDef, binding, BuiltInParameterGroup.INVALID);
                    if (result)
                        pass2Bound++;
                    else
                        pass2Skipped++;
                }

                tx.Commit();
            }

            TaskDialog.Show("Load Shared Params",
                $"Shared parameter binding complete.\n\n" +
                $"Pass 1 (Universal):   {pass1Bound} bound, {pass1Skipped} skipped\n" +
                $"Pass 2 (Discipline):  {pass2Bound} bound, {pass2Skipped} skipped\n\n" +
                $"Source: {spFile}");

            return Result.Succeeded;
        }

        private static ExternalDefinition FindDefinition(DefinitionFile defFile, string name)
        {
            foreach (DefinitionGroup group in defFile.Groups)
            {
                foreach (Definition def in group.Definitions)
                {
                    if (def.Name == name && def is ExternalDefinition extDef)
                        return extDef;
                }
            }
            return null;
        }

        private static CategorySet BuildCategorySet(Document doc, string[] ostNames)
        {
            CategorySet catSet = new CategorySet();
            Categories cats = doc.Settings.Categories;

            foreach (string ostName in ostNames)
            {
                // Convert OST_ name to BuiltInCategory enum
                if (Enum.TryParse<BuiltInCategory>(ostName, out var bic))
                {
                    Category cat = cats.get_Item(bic);
                    if (cat != null && cat.AllowsBoundParameters)
                        catSet.Insert(cat);
                }
            }

            return catSet;
        }
    }
}
