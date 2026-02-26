# CLAUDE.md — AI Assistant Guide for STINGTOOLS

## Repository Overview

**STINGTOOLS** is a new repository owned by `beckykyomugisha`. This project is in its initial setup phase — no source code has been committed yet.

This file provides guidance for AI assistants (Claude Code, etc.) working in this repository.

## Project Status

- **Current state**: Empty repository — initial setup required
- **Remote**: `origin` → `github.com/beckykyomugisha/STINGTOOLS`

## Development Workflow

### Branching

- The default branch is `main`
- Feature branches should follow the naming convention: `feature/<description>` or `claude/<session-id>`
- Always create feature branches from the latest `main`

### Commits

- Write clear, concise commit messages in imperative mood (e.g., "Add user authentication", not "Added user authentication")
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
5. **Security first** — Never commit secrets; validate user input at system boundaries; avoid OWASP top 10 vulnerabilities

### Code Style

- Follow the conventions already established in the codebase (once code exists)
- Use consistent formatting with whatever linter/formatter the project adopts
- Prefer clarity over cleverness

### Testing

- Run existing tests before and after making changes
- Add tests for new functionality when a test framework is in place
- Do not mark a task as complete if tests are failing

### Git Safety

- Never force-push without explicit permission
- Never run destructive git commands (`reset --hard`, `clean -f`, `branch -D`) without confirmation
- Always commit to the correct branch — verify before pushing

## Directory Structure

_To be updated as the project grows._

```
STINGTOOLS/
├── CLAUDE.md          # This file — AI assistant guide
└── ...                # Project source code (TBD)
```

## Build & Run

_To be updated once the project's language, framework, and tooling are established._

## Dependencies

_To be updated once a package manager and dependency file are added (e.g., `package.json`, `requirements.txt`, `go.mod`)._
