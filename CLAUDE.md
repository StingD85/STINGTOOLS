# CLAUDE.md — AI Assistant Guide for STINGTOOLS

## Repository Overview

**StingTools** is an Autodesk Revit plugin (addin) built with the Revit API using .NET/C#. The repository is owned by `beckykyomugisha`.

This file provides guidance for AI assistants (Claude Code, etc.) working in this repository.

## Technology Stack

- **Platform**: Autodesk Revit (BIM software)
- **Language**: C# / .NET
- **Plugin type**: Revit External Application/Command (`.addin` manifest)
- **Deployment helper**: `extract_plugin.sh` (Bash script for extracting/deploying the plugin)

## Directory Structure

```
STINGTOOLS/
├── CLAUDE.md              # AI assistant guide (this file)
├── StingTools.addin        # Revit addin manifest (XML) — tells Revit how to load the plugin
├── extract_plugin.sh       # Shell script for plugin extraction/deployment
└── StingTools/             # Main plugin source code (C# project)
```

### Key Files

- **`StingTools.addin`** — XML manifest file that Revit reads to discover and load the plugin. Contains the assembly path, class name, GUID (`AddInId`), and vendor info. Installed to Revit's addin directory (e.g., `C:\ProgramData\Autodesk\Revit\Addins\<year>\`).
- **`extract_plugin.sh`** — Shell script for extracting or deploying the plugin build artifacts.
- **`StingTools/`** — The main C# project directory containing the plugin source code, likely including a `.csproj` file and classes implementing `IExternalApplication` or `IExternalCommand`.

## Development Workflow

### Building

- Build the project using Visual Studio or `dotnet build` (if SDK-style `.csproj`)
- The build output is a `.dll` assembly referenced by `StingTools.addin`
- To test locally, copy the `.addin` manifest and built `.dll` to the Revit addins folder

### Revit Addin Deployment

1. Build the `StingTools` project to produce the plugin DLL
2. Copy `StingTools.addin` to the Revit addins directory:
   - Per-machine: `C:\ProgramData\Autodesk\Revit\Addins\<year>\`
   - Per-user: `%APPDATA%\Autodesk\Revit\Addins\<year>\`
3. Copy the built DLL to the path specified in the `<Assembly>` tag of the `.addin` file
4. Restart Revit to load the plugin

### Branching

- The default branch is `main`
- Feature branches: `feature/<description>` or `claude/<session-id>`
- Always create feature branches from the latest `main`

### Commits

- Write clear, concise commit messages in imperative mood (e.g., "Add wall selection filter", not "Added wall selection filter")
- Keep commits focused — one logical change per commit
- Do not commit secrets, credentials, `.env` files, or API keys
- Do not commit build output (`bin/`, `obj/`, `.dll` files) unless intentional

### Pull Requests

- PRs should have a descriptive title and summary
- Include a test plan when applicable

## Conventions for AI Assistants

### General Rules

1. **Read before editing** — Always read a file before modifying it
2. **Prefer edits over rewrites** — Use targeted edits instead of rewriting entire files
3. **Don't over-engineer** — Keep changes minimal and focused on what was requested
4. **No unnecessary files** — Don't create documentation, config, or helper files unless explicitly asked
5. **Security first** — Never commit secrets; validate user input at system boundaries

### C# / Revit API Style

- Follow existing naming conventions in the codebase (PascalCase for public members, camelCase for locals)
- Use Revit API best practices: always wrap DB operations in `Transaction` blocks
- Dispose of Revit API objects properly
- Handle `OperationCanceledException` for user-cancelled operations
- Use `TaskDialog` for user-facing messages within Revit (not `MessageBox`)

### Testing

- Revit plugins are difficult to unit test in isolation — prefer integration testing within Revit
- Validate changes by loading the plugin in Revit and testing the affected commands
- Do not mark a task as complete if the code does not compile

### Git Safety

- Never force-push without explicit permission
- Never run destructive git commands (`reset --hard`, `clean -f`, `branch -D`) without confirmation
- Always commit to the correct branch — verify before pushing

## Dependencies

- **Revit API assemblies**: `RevitAPI.dll`, `RevitAPIUI.dll` (referenced from the Revit installation directory, not distributed in the repo)
- **Target framework**: .NET Framework 4.8 (Revit 2021–2024) or .NET 8 (Revit 2025+), depending on the target Revit version
- Additional NuGet packages as specified in the `.csproj` file
