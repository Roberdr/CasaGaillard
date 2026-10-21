# Project boundaries

This repository contains two separate web applications with overlapping feature names:

- `CasaGaillard.Core/` is the active .NET application. Implement current application features here by default.
- The root `Areas/`, `Controllers/`, `Models/`, and `Views/` belong to the older `CasaGaillard.csproj` application. Edit them only when the user explicitly asks for the legacy application or both applications.

Open `CasaGaillard.Core.slnf` for current development or `CasaGaillard.Legacy.slnf` for legacy maintenance. Keep changes within the selected application's project unless a shared repository-level change is needed.
