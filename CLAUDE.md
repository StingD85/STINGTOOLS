# CLAUDE.md — AI Assistant Guide for STINGTOOLS

## Repository Overview

**StingTools** is a suite of **pyRevit extensions** for Autodesk Revit — a collection of Python-based BIM automation tools for tagging, documentation, templates, and intelligent Revit workflows. The repository is owned by `beckykyomugisha`.

This file provides guidance for AI assistants (Claude Code, etc.) working in this repository.

## Technology Stack

- **Platform**: Autodesk Revit (BIM software)
- **Framework**: pyRevit (IronPython/CPython scripting framework for Revit)
- **Language**: Python (IronPython 2.7 for Revit API access, some CPython 3 support)
- **Data formats**: CSV and JSON files for configuration data (materials, parameters, schedules)
- **Deployment**: `extract_plugin.sh` (Bash) + `StingTools.addin` (XML manifest)

## Directory Structure

```
STINGTOOLS/
├── CLAUDE.md                       # AI assistant guide (this file)
├── StingTools.addin                # Revit addin manifest (XML)
├── extract_plugin.sh               # Plugin extraction/deployment script
│
├── StingTools/                     # Shared data and configuration
│   ├── Data/                       # CSV/JSON data files
│   │   ├── Materials/              # Material definitions (CSVs)
│   │   ├── Parameters/             # Parameter definitions
│   │   ├── Schedules/              # Schedule templates and configs
│   │   ├── SharedParameterBindings/# Shared parameter binding data
│   │   └── ...
│   └── PYREVIT_SCRIPT_MANIFEST     # pyRevit manifest
│
├── STINGTags.extension/            # Extension: Tagging tools
│   └── STING Tags.tab/            # Tab in Revit ribbon
│       └── *.panel/               # Panels containing pushbutton commands
│           └── *.pushbutton/      # Individual commands
│               ├── script.py      # Command implementation
│               └── bundle.yaml    # Command metadata
│
├── STINGTemp.extension/            # Extension: Template tools
│   └── STING Temp.tab/
│       └── *.panel/
│           └── *.pushbutton/
│
└── STINGDocs.extension/            # Extension: Documentation tools
    └── STING Docs.tab/
        └── *.panel/
            └── *.pushbutton/
```

### pyRevit Extension Structure

Each extension follows the pyRevit bundle hierarchy:
- **`.extension`** — Top-level extension registered with pyRevit
- **`.tab`** — Creates a tab in the Revit ribbon UI
- **`.panel`** — Groups related commands within a tab
- **`.pushbutton`** — An individual clickable command
  - `script.py` — The Python script executed when the button is clicked
  - `bundle.yaml` — Metadata (title, tooltip, icon, etc.)

### Shared Libraries

Located under the extension `lib/` directories, these modules provide reusable functionality:

| Library | Purpose |
|---------|---------|
| `DimensionTools` | Dimension placement and management |
| `IntelligenceCore` | AI-powered features (`ai_engine.py`) |
| `LegendTools` | Legend creation and formatting |
| `MeasurementTools` | Measurement utilities |
| `RevisionTools` | Revision tracking and management |
| `ScheduleTools` | Schedule creation and manipulation |
| `SheetTools` | Sheet management and organization |
| `TextNoteTools` | Text note creation and formatting |
| `TitleBlockTools` | Title block management |
| `ViewportTools` | Viewport placement and configuration |

### Data Files (`StingTools/Data/`)

CSV and JSON files providing configuration data:
- **Materials/** — Material property definitions
- **Parameters/** — Revit parameter definitions and mappings
- **Schedules/** — Schedule templates and column configurations
- **SharedParameterBindings/** — Shared parameter binding definitions

## Development Workflow

### Adding a New Command

1. Navigate to the appropriate extension (e.g., `STINGDocs.extension/`)
2. Find or create the correct `.tab/` → `.panel/` hierarchy
3. Create a new `.pushbutton/` folder named `CommandName.pushbutton`
4. Add `script.py` with the command logic
5. Add `bundle.yaml` with metadata (title, tooltip, author)

### Modifying an Existing Command

1. Locate the command's `.pushbutton/` folder
2. Read and understand the existing `script.py` before editing
3. Test changes by reloading pyRevit in Revit (no Revit restart needed)

### Deployment

- **Development**: Clone the repo into pyRevit's extensions directory, or register the path in pyRevit settings
- **`extract_plugin.sh`**: Use this script for packaging/deploying the plugin
- **`StingTools.addin`**: XML manifest for Revit addin registration

### Branching

- The default branch is `main`
- Feature branches: `feature/<description>` or `claude/<session-id>`
- Always create feature branches from the latest `main`

### Commits

- Write clear, concise commit messages in imperative mood (e.g., "Add wall tag automation", not "Added wall tag automation")
- Keep commits focused — one logical change per commit
- Do not commit secrets, credentials, `.env` files, or API keys

### Pull Requests

- PRs should have a descriptive title and summary
- Include a test plan when applicable

## Conventions for AI Assistants

### General Rules

1. **Read before editing** — Always read a file before modifying it
2. **Prefer edits over rewrites** — Use targeted edits instead of rewriting entire files
3. **Don't over-engineer** — Keep changes minimal and focused on what was requested
4. **No unnecessary files** — Don't create documentation, config, or helper files unless explicitly asked
5. **Security first** — Never commit secrets; protect any API keys used by IntelligenceCore

### Python / pyRevit Style

- Follow existing naming conventions in the codebase
- Use `snake_case` for functions and variables, `PascalCase` for classes
- Import Revit API namespaces via: `from Autodesk.Revit.DB import *` (IronPython style)
- Access the current document via `__revit__.ActiveUIDocument.Document`
- Always wrap Revit DB modifications in a `Transaction`:
  ```python
  t = Transaction(doc, "Description")
  t.Start()
  # ... modifications ...
  t.Commit()
  ```
- Use `pyrevit.forms` for UI dialogs (not Windows Forms directly)
- Use `pyrevit.script` for output and logging
- Keep `script.py` files focused — put reusable logic in library modules

### Data File Conventions

- CSV files use standard comma-separated format
- JSON files should be well-formatted with consistent indentation
- When modifying data files, preserve the existing structure and column order
- Data files are read at runtime — changes take effect on next command execution

### Testing

- pyRevit scripts run inside Revit — test by reloading pyRevit (`Alt+Click` the pyRevit reload button for a full reload)
- Validate changes in Revit with a test project before committing
- Ensure scripts handle missing/null elements gracefully (Revit models vary widely)
- Do not mark a task as complete if the code has syntax errors

### Git Safety

- Never force-push without explicit permission
- Never run destructive git commands (`reset --hard`, `clean -f`, `branch -D`) without confirmation
- Always commit to the correct branch — verify before pushing

## Dependencies

- **pyRevit**: Must be installed on the user's machine (provides the IronPython runtime and Revit API bridge)
- **Revit API**: `RevitAPI.dll`, `RevitAPIUI.dll` (accessed through pyRevit, not directly referenced)
- **Python packages**: Any additional packages used by `IntelligenceCore` / `ai_engine.py` (check imports)
- **Data files**: CSV/JSON files in `StingTools/Data/` are required at runtime
