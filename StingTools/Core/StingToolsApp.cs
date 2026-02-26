using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace StingTools.Core
{
    /// <summary>
    /// Main Revit external application. Creates a single "STING Tools" ribbon
    /// tab with three panels: Docs, Tags, Temp — porting the three PyRevit
    /// extensions (StingDocs, STINGTags, STINGTemp) into one compiled plugin.
    /// </summary>
    public class StingToolsApp : IExternalApplication
    {
        public static string AssemblyPath { get; private set; }
        public static string DataPath { get; private set; }

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                AssemblyPath = Assembly.GetExecutingAssembly().Location;
                DataPath = Path.Combine(
                    Path.GetDirectoryName(AssemblyPath) ?? string.Empty,
                    "data");

                const string tabName = "STING Tools";
                application.CreateRibbonTab(tabName);

                BuildDocsPanel(application, tabName);
                BuildTagsPanel(application, tabName);
                BuildTempPanel(application, tabName);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("STING Tools",
                    "Failed to initialise STING Tools:\n" + ex.Message);
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        // ── Docs Panel ──────────────────────────────────────────────────
        private void BuildDocsPanel(UIControlledApplication app, string tab)
        {
            var panel = app.CreateRibbonPanel(tab, "Docs");
            string asmPath = AssemblyPath;

            AddButton(panel, "btnSheetOrganizer",
                "Sheet\nOrganizer",
                asmPath,
                typeof(Docs.SheetOrganizerCommand).FullName,
                "Organise and manage project sheets with drag-and-drop tree view");

            AddButton(panel, "btnViewOrganizer",
                "View\nOrganizer",
                asmPath,
                typeof(Docs.ViewOrganizerCommand).FullName,
                "Organise views by discipline, type, and level");

            AddButton(panel, "btnSheetIndex",
                "Sheet\nIndex",
                asmPath,
                typeof(Docs.SheetIndexCommand).FullName,
                "Generate a sheet index schedule with revision tracking");

            AddButton(panel, "btnTransmittal",
                "Document\nTransmittal",
                asmPath,
                typeof(Docs.TransmittalCommand).FullName,
                "Create ISO 19650-compliant document transmittal records");
        }

        // ── Tags Panel ──────────────────────────────────────────────────
        private void BuildTagsPanel(UIControlledApplication app, string tab)
        {
            var panel = app.CreateRibbonPanel(tab, "Tags");
            string asmPath = AssemblyPath;

            AddButton(panel, "btnAutoTag",
                "Auto\nTag",
                asmPath,
                typeof(Tags.AutoTagCommand).FullName,
                "Automatically apply ISO 19650 asset tags to all elements in the active view");

            AddButton(panel, "btnBatchTag",
                "Batch\nTag",
                asmPath,
                typeof(Tags.BatchTagCommand).FullName,
                "Batch-apply tags to all taggable elements in the project model");

            AddButton(panel, "btnTagConfig",
                "Tag\nConfig",
                asmPath,
                typeof(Tags.TagConfigCommand).FullName,
                "Configure discipline, system, product, and function code lookup tables");

            AddButton(panel, "btnLoadParams",
                "Load\nParams",
                asmPath,
                typeof(Tags.LoadSharedParamsCommand).FullName,
                "Bind shared parameters (universal + discipline-specific) to project categories");

            AddButton(panel, "btnValidateTags",
                "Validate\nTags",
                asmPath,
                typeof(Tags.ValidateTagsCommand).FullName,
                "Validate existing tags for completeness and correct token counts");
        }

        // ── Temp Panel ──────────────────────────────────────────────────
        private void BuildTempPanel(UIControlledApplication app, string tab)
        {
            var panel = app.CreateRibbonPanel(tab, "Temp");
            string asmPath = AssemblyPath;

            // Setup group
            var setupGroup = panel.AddItem(
                new PulldownButtonData("grpSetup", "Setup")) as PulldownButton;
            if (setupGroup != null)
            {
                setupGroup.LongDescription =
                    "Project setup: parameters, data verification";
                AddPulldownItem(setupGroup, "btnCreateParams",
                    "Create Parameters",
                    asmPath,
                    typeof(Temp.CreateParametersCommand).FullName,
                    "Bind shared parameters to the active project");
                AddPulldownItem(setupGroup, "btnCheckData",
                    "Check Data Files",
                    asmPath,
                    typeof(Temp.CheckDataCommand).FullName,
                    "Verify data files and show file inventory with SHA hashes");
            }

            // Materials group
            var matGroup = panel.AddItem(
                new PulldownButtonData("grpMaterials", "Materials")) as PulldownButton;
            if (matGroup != null)
            {
                matGroup.LongDescription =
                    "Create and manage Revit materials from CSV data";
                AddPulldownItem(matGroup, "btnCreateBLE",
                    "Create BLE Materials",
                    asmPath,
                    typeof(Temp.CreateBLEMaterialsCommand).FullName,
                    "Create building-element materials from BLE_MATERIALS.csv (815 materials)");
                AddPulldownItem(matGroup, "btnCreateMEP",
                    "Create MEP Materials",
                    asmPath,
                    typeof(Temp.CreateMEPMaterialsCommand).FullName,
                    "Create MEP materials from MEP_MATERIALS.csv (464 materials)");
            }

            // Families group
            var famGroup = panel.AddItem(
                new PulldownButtonData("grpFamilies", "Families")) as PulldownButton;
            if (famGroup != null)
            {
                famGroup.LongDescription =
                    "Create wall, ceiling, floor, roof, and MEP family types";
                AddPulldownItem(famGroup, "btnCreateWalls",
                    "Create Walls",
                    asmPath,
                    typeof(Temp.CreateWallsCommand).FullName,
                    "Create wall types from BLE_MATERIALS.csv");
                AddPulldownItem(famGroup, "btnCreateFloors",
                    "Create Floors",
                    asmPath,
                    typeof(Temp.CreateFloorsCommand).FullName,
                    "Create floor types from BLE_MATERIALS.csv");
                AddPulldownItem(famGroup, "btnCreateCeilings",
                    "Create Ceilings",
                    asmPath,
                    typeof(Temp.CreateCeilingsCommand).FullName,
                    "Create ceiling types from BLE_MATERIALS.csv");
                AddPulldownItem(famGroup, "btnCreateRoofs",
                    "Create Roofs",
                    asmPath,
                    typeof(Temp.CreateRoofsCommand).FullName,
                    "Create roof types from BLE_MATERIALS.csv");
                AddPulldownItem(famGroup, "btnCreateDucts",
                    "Create Ducts",
                    asmPath,
                    typeof(Temp.CreateDuctsCommand).FullName,
                    "Create duct types from MEP_MATERIALS.csv");
                AddPulldownItem(famGroup, "btnCreatePipes",
                    "Create Pipes",
                    asmPath,
                    typeof(Temp.CreatePipesCommand).FullName,
                    "Create pipe types from MEP_MATERIALS.csv");
            }

            // Schedules group
            var schGroup = panel.AddItem(
                new PulldownButtonData("grpSchedules", "Schedules")) as PulldownButton;
            if (schGroup != null)
            {
                schGroup.LongDescription =
                    "Schedule creation, auto-populate, and CSV export";
                AddPulldownItem(schGroup, "btnBatchSchedules",
                    "Batch Create Schedules",
                    asmPath,
                    typeof(Temp.BatchSchedulesCommand).FullName,
                    "Multi-discipline schedule creation (168 definitions)");
                AddPulldownItem(schGroup, "btnAutoPopulate",
                    "Auto-Populate",
                    asmPath,
                    typeof(Temp.AutoPopulateCommand).FullName,
                    "Apply field remaps across categories (42 remaps)");
                AddPulldownItem(schGroup, "btnExportCSV",
                    "Export to CSV",
                    asmPath,
                    typeof(Temp.ExportCSVCommand).FullName,
                    "Export schedule data to CSV files");
            }

            // Templates group
            var tplGroup = panel.AddItem(
                new PulldownButtonData("grpTemplates", "Templates")) as PulldownButton;
            if (tplGroup != null)
            {
                tplGroup.LongDescription =
                    "View templates, filters, line styles, worksets, and project phases";
                AddPulldownItem(tplGroup, "btnCreateFilters",
                    "Create Filters",
                    asmPath,
                    typeof(Temp.CreateFiltersCommand).FullName,
                    "Create view filters from template definitions");
                AddPulldownItem(tplGroup, "btnCreateWorksets",
                    "Create Worksets",
                    asmPath,
                    typeof(Temp.CreateWorksetsCommand).FullName,
                    "Create worksets (46 definitions)");
                AddPulldownItem(tplGroup, "btnViewTemplates",
                    "View Templates",
                    asmPath,
                    typeof(Temp.ViewTemplatesCommand).FullName,
                    "Create and apply view templates");
            }
        }

        // ── Data file utilities ───────────────────────────────────────

        /// <summary>Find a data file by name, searching DataPath and subdirectories.</summary>
        public static string FindDataFile(string fileName)
        {
            if (string.IsNullOrEmpty(DataPath))
                return null;

            string direct = Path.Combine(DataPath, fileName);
            if (File.Exists(direct)) return direct;

            foreach (string f in Directory.GetFiles(
                DataPath, fileName, SearchOption.AllDirectories))
            {
                return f;
            }
            return null;
        }

        /// <summary>Parse a CSV line respecting quoted fields.</summary>
        public static string[] ParseCsvLine(string line)
        {
            var result = new System.Collections.Generic.List<string>();
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

        // ── Ribbon helpers ───────────────────────────────────────────────

        private static void AddButton(RibbonPanel panel, string name,
            string text, string asmPath, string className,
            string tooltip)
        {
            var data = new PushButtonData(name, text, asmPath, className);
            data.ToolTip = tooltip;
            panel.AddItem(data);
        }

        private static void AddPulldownItem(PulldownButton pulldown,
            string name, string text, string asmPath, string className,
            string tooltip)
        {
            var data = new PushButtonData(name, text, asmPath, className);
            data.ToolTip = tooltip;
            pulldown.AddPushButton(data);
        }
    }
}
