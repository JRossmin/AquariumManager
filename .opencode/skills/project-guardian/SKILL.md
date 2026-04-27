\---

name: project-guardian

description: Skill to understand, maintain, and improve a software project by applying best practices, SOLID principles, design patterns, and continuously updating the project context documentation.

\---



\## Skill goal



This skill guides the agent to:

\- Understand the project’s architecture and conventions.

\- Keep an up-to-date high-level view in the `project\_Context.md` file.

\- Propose and apply changes that follow software engineering best practices (SOLID, design patterns, clean code, and clean architecture).

\- Avoid changes that degrade code quality or break the existing architecture.



\## Project stack and context



The project is built with:

\- Backend: .NET 8 (C#), web API style (adjust if using Minimal API, MVC, etc.).

\- Frontend: React (adjust if using Next.js, Vite, etc.).

\- Database: \[SQL Server / PostgreSQL / other].

\- Typical folder organization (adjust to your real repo):

&#x20; - `src/` or `backend/`: backend code.

&#x20; - `frontend/` or `client/`: frontend code.

&#x20; - `tests/`: automated test projects.

&#x20; - `docs/`: additional documentation, if present.



When analyzing or modifying the project, always respect this separation of responsibilities and do not mix layers without a clear reason.



\## Project context file



There is a global context file called `project\_Context.md` that acts as living documentation for the project.  

This file may live outside the repo root, in a path configured in `opencode.json` via `external\_directory`.



Rules for working with `project\_Context.md`:



1\. \*\*Constant use\*\*: Before making large changes or giving high-level recommendations, read the current content of `project\_Context.md` to understand:

&#x20;  - Project overview.

&#x20;  - Main modules.

&#x20;  - Relevant design decisions.

&#x20;  - TODOs or next steps already documented.



2\. \*\*Mandatory updates\*\*:

&#x20;  - When significant changes are made to architecture, modules, endpoints, use cases, or folder structure, update `project\_Context.md`.

&#x20;  - Keep a clear structure in the file, for example:

&#x20;    - `# Overview`

&#x20;    - `## Architecture`

&#x20;    - `## Modules`

&#x20;    - `## Endpoints / APIs`

&#x20;    - `## Frontend`

&#x20;    - `## Design decisions`

&#x20;    - `## Next steps / TODOs`



3\. \*\*Format and style\*\*:

&#x20;  - Use clean, consistent Markdown.

&#x20;  - Avoid pasting large blocks of code into `project\_Context.md`; instead, describe components and their purpose.

&#x20;  - When summarizing changes, include:

&#x20;    - What was changed.

&#x20;    - Why it was changed.

&#x20;    - Which files were affected.



\## Code quality principles



When proposing or modifying code, follow these principles:



1\. \*\*SOLID\*\*:

&#x20;  - Single Responsibility: Each class, method, or component should have a single reason to change.

&#x20;  - Open/Closed: Prefer extending behavior over modifying stable code when possible.

&#x20;  - Liskov Substitution: Implementations should be substitutable for their abstractions without breaking expectations.

&#x20;  - Interface Segregation: Prefer small, focused interfaces instead of large, “fat” ones.

&#x20;  - Dependency Inversion: Depend on abstractions (interfaces), not concrete implementations.



2\. \*\*Clean Code\*\*:

&#x20;  - Use descriptive names for classes, methods, variables, and components.

&#x20;  - Keep functions small and focused on doing one thing well.

&#x20;  - Avoid code duplication; extract reusable functions or methods when it makes sense.

&#x20;  - Use clear error and exception handling.



3\. \*\*Design patterns and architecture\*\*:

&#x20;  - Apply patterns only when they simplify the design or improve extensibility/readability (e.g., Repository, Factory, Strategy, CQRS, Mediator).

&#x20;  - Respect the project’s established architecture (for example: Domain, Application, Infrastructure, API layers).

&#x20;  - Do not introduce new dependencies or complex patterns without a clear and documented reason.



4\. \*\*Testing and maintainability\*\*:

&#x20;  - When reasonable, propose unit or integration tests for new features or significant refactors.

&#x20;  - Avoid tight coupling that makes testing difficult.

&#x20;  - If the project already has a testing convention, follow it.



\## Rules for interacting with the code



When working in the repository:



1\. Before proposing changes:

&#x20;  - Use reading and search tools (e.g., glob/grep) to understand how the code is organized.

&#x20;  - Identify the main entry points (endpoints, events, UI components) and core business logic before modifying anything.



2\. Change proposals:

&#x20;  - Always explain the intention of the change before showing code.

&#x20;  - Point out which files and sections will be changed.

&#x20;  - Clarify how the change respects or improves the best practices mentioned above.



3\. Applying changes:

&#x20;  - When editing files, make small, well-scoped changes.

&#x20;  - Avoid huge refactors in a single step; if a change is large, propose doing it in multiple steps.

&#x20;  - After important changes, update `project\_Context.md` so it reflects the new state of the system.



\## How to use this skill



When the user invokes this skill (or when the system loads it automatically):



1\. Read or recall the content of `project\_Context.md`.

2\. Analyze the current structure of the project (backend, frontend, tests).

3\. When answering:

&#x20;  - Prioritize solutions that preserve or improve the existing architecture.

&#x20;  - Explicitly mention if a 

