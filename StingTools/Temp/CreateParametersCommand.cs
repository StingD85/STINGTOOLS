using System;
using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using StingTools.Core;

namespace StingTools.Temp
{
    /// <summary>
    /// Ported from STINGTemp 1_Setup.panel — Create Parameters.
    /// Binds shared parameters from MR_PARAMETERS.txt to the active project.
    /// Delegates to the same logic as Tags.LoadSharedParamsCommand.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateParametersCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            // Delegate to the Tags panel LoadSharedParams implementation
            var cmd = new Tags.LoadSharedParamsCommand();
            return cmd.Execute(commandData, ref message, elements);
        }
    }
}
