# Muffle — Agent Workflow Guide

## Overview

This document defines how an AI coding agent should approach building, maintaining, and extending the Muffle project. The agent works through tasks defined in `SPEC.md`, follows project conventions from `.github/copilot-instructions.md`, and validates every change before marking it complete.

> **Canonical references** (read before every session):
>
> | Document | Location | Purpose |
> |----------|----------|---------|
> | Product Specification | `SPEC.md` | Feature requirements, data model, architecture |
> | Copilot Instructions | `.github/copilot-instructions.md` | Build commands, project structure, time limits, known limitations |
> | App Settings | `Muffle/appsettings.json` | Connection strings |

---

## 1. Agent Operating Principles

### 1.1 Task-Driven Development

- Read tasks from `SPEC.md` feature tables (IDs like `SRV-1`, `CHAT-3`, etc.).
- Complete tasks **sequentially within each feature area** — respect implicit dependencies (e.g., `DB-1` before `DAT-1`).
- After completing a task, note what was done and any follow-up needed.
- Document blockers with the specific requirement ID and reason.

### 1.2 Data-Layer-First Rule

**Always start with `Muffle.Data`.**

The copilot instructions state: *"Always work with the data project first to validate core functionality before attempting full MAUI builds."*

Order of operations for any feature:

1. **Model** → `Muffle.Data/Models/`
2. **Database schema** → `SqliteDbService.cs` and `SqlServerDbService.cs`
3. **Service / query** → `Muffle.Data/Services/`
4. **ViewModel** → `Muffle/ViewModels/`
5. **View (XAML + code-behind)** → `Muffle/Views/`
6. **Validation build** → `dotnet build Muffle.Data/Muffle.Data.csproj` then full solution

### 1.3 Minimal, Correct Changes

- Make the **smallest diff** that satisfies the requirement.
- Follow existing naming conventions, formatting, and patterns already in the codebase.
- Do **not** add interfaces, abstractions, or layers unless they are required for external dependencies or testing.
- Do **not** modify auto-generated files (`*.g.cs`, `*.AssemblyInfo.cs`).

---

## 2. Before Starting Any Task

### 2.1 Required Checks

| # | Check | How |
|---|-------|-----|
| 1 | **Read the spec** | Open `SPEC.md` and locate the requirement ID you are implementing. |
| 2 | **Read copilot instructions** | Review `.github/copilot-instructions.md` for build commands and constraints. |
| 3 | **Check dependencies** | Verify that prerequisite requirement IDs are complete (e.g., `DB-3` before `DAT-1`). |
| 4 | **Review existing code** | Read the files you plan to modify — understand current state before editing. |
| 5 | **Plan the approach** | Outline which files change and in what order before writing any code. |

### 2.2 Context Gathering Strategy

Use tools in this priority order:

1. **Direct file reads** when you know the exact path (from the solution structure in SPEC.md §4.1).
2. **Symbol search** (`find_symbol`) when tracing how a type or method is used across the solution.
3. **Semantic search** (`code_search`) when looking for a concept or behavior.
4. **Project/solution enumeration** (`get_projects_in_solution`, `get_files_in_project`) when exploring unfamiliar areas.

**Anti-pattern:** Do not guess file paths. The solution has exactly two projects — `Muffle/Muffle.csproj` and `Muffle.Data/Muffle.Data.csproj`.

---

## 3. Implementation Workflow

### 3.1 Standard Task Flow

```
┌─────────────────────────┐
│ 1. Read requirement     │
│    from SPEC.md         │
├─────────────────────────┤
│ 2. Read target files    │
│    (get_file)           │
├─────────────────────────┤
│ 3. Implement changes    │
│    (replace/create)     │
├─────────────────────────┤
│ 4. Validate — data      │
│    layer build first    │
├─────────────────────────┤
│ 5. Validate — full      │
│    solution build       │
├─────────────────────────┤
│ 6. Run related tests    │
│    (if they exist)      │
├─────────────────────────┤
│ 7. Mark task complete   │
└─────────────────────────┘
```

### 3.2 File Change Rules

| Rule | Details |
|------|---------|
| **Always read before editing** | Use `get_file` to load current content before calling `replace_string_in_file`. |
| **Unique match context** | Include 3–5 lines above and below the change point to ensure a unique match. |
| **Group by file** | When making multiple edits to one file, send all replacements together. |
| **Never show code blocks** | Use `replace_string_in_file` or `create_file` directly — do not paste code into chat. |
| **Dual-database parity** | Any schema or seed-data change must be applied to **both** `SqliteDbService.cs` and `SqlServerDbService.cs`. |

### 3.3 New Feature Checklist

When adding a brand-new feature (new model, new view, etc.):

- [ ] Add model class in `Muffle.Data/Models/`
- [ ] Add or update table creation SQL in `SqliteDbService.InitializeDatabase()` **and** `SqlServerDbService.InitializeDatabase()`
- [ ] Add table drop SQL in `SqliteDbService.DisposeDatabase()` **and** `SqlServerDbService.DisposeDatabase()`
- [ ] Add seed data if applicable
- [ ] Add service methods in `Muffle.Data/Services/`
- [ ] Build `Muffle.Data` project: `dotnet build Muffle.Data/Muffle.Data.csproj`
- [ ] Add ViewModel in `Muffle/ViewModels/`
- [ ] Add View (XAML + code-behind) in `Muffle/Views/`
- [ ] Wire up navigation / content switching in `MuffleMain.xaml.cs`
- [ ] Build full solution (Windows/macOS only)

---

## 4. Build & Validation

### 4.1 Build Commands

| Scope | Command | Time | Notes |
|-------|---------|------|-------|
| Data layer only | `dotnet build Muffle.Data/Muffle.Data.csproj` | ~30 s | Always succeeds on any OS with .NET 10 SDK |
| Full solution | `dotnet build` | 2–5 min | **Requires MAUI workloads.** NEVER cancel — set timeout ≥ 10 min. |
| Package restore | `dotnet restore` | 20–60 s | Run if build fails on missing packages |
| Clean + rebuild | `dotnet clean && dotnet build` | 3–6 min | NEVER cancel. |

### 4.2 Validation Sequence

After every change set:

1. **`dotnet build Muffle.Data/Muffle.Data.csproj`** — must pass.
2. **`get_errors`** on modified files — check for compiler diagnostics.
3. **Full solution build** (if MAUI workloads are available) — must pass.
4. **Run tests** if any exist for the changed area (`run_tests`).

### 4.3 Build Failure Triage

| Error | Cause | Fix |
|-------|-------|-----|
| `NETSDK1147: workloads must be installed` | Missing MAUI workloads | Install with `dotnet workload install maui-android maui-ios maui-maccatalyst maui-windows` or fall back to data-layer-only build. |
| Missing type or namespace | File not saved, wrong namespace, missing `using` | Check namespace matches folder; add `using` directive. |
| Dapper mapping failure | Column name mismatch between SQL and model property | Align SQL column names with C# property names. |
| Connection string not found | `ConfigurationLoader` cannot locate `appsettings.json` | Verify `appsettings.json` is present and the directory-walk logic in `ConfigurationLoader.cs` resolves correctly. |

---

## 5. Code Conventions

### 5.1 Project Conventions (inferred from codebase)

| Area | Convention |
|------|-----------|
| **Nullable** | Enabled (`<Nullable>enable</Nullable>`) — use nullable annotations. |
| **Implicit usings** | Enabled — do not add redundant `using System;` etc. |
| **Namespace style** | Block-scoped (`namespace Muffle.Data.Models { ... }`). Some newer files use file-scoped. Match the file being edited. |
| **MVVM base class** | ViewModels extend `BindableObject` (not a third-party MVVM library). |
| **Commands** | Use `Command` / `Command<T>` from `System.Windows.Input`. |
| **Collections** | Use `ObservableCollection<T>` for bindable lists; `List<T>` only when the UI does not need change notification. |
| **Database access** | Dapper with raw SQL via `IDbConnection`. No EF Core queries (EF Core package is present but unused). |
| **Static services** | `UsersService`, `SQLiteDbService`, `SqlServerDbService` use static methods and static connection strings. |
| **Connection creation** | `SQLiteDbService.CreateConnection()` / `SqlServerDbService.CreateConnection()` — always wrap in `using`. |
| **View construction** | ContentViews receive their data (model or ViewModel) via constructor parameter, then set `BindingContext`. |

### 5.2 Naming Conventions

| Element | Style | Example |
|---------|-------|---------|
| Class | PascalCase | `FriendDetailsContentViewModel` |
| Public property | PascalCase | `ChatMessages` |
| Private field | `_camelCase` | `_signalingService` |
| Method | PascalCase | `SendMessageAsync` |
| Async method | `…Async` suffix | `StartVoiceCallAsync` |
| Event handler | `Subject_Event` | `ServerButton_OnClicked` |
| XAML element name | PascalCase with `x:Name` | `MainContentFrame` |
| SQL table | PascalCase plural | `Servers`, `ServerOwners` |

### 5.3 Error Handling

- Use `try/catch` around database seed operations (seeds may fail on re-run — this is expected).
- Log errors to `Console.WriteLine` (no structured logging set up yet).
- Do **not** swallow exceptions silently in new code — at minimum log the message.

---

## 6. Feature Area Reference

### 6.1 Requirement Dependency Map

The following shows implicit ordering between requirement IDs:

```
DB-1 ──► DB-3 ──► DAT-1 ──► SRV-1 ──► SRV-2
                  DAT-2 ──► FRI-3 ──► FRI-4
                  DAT-3 ──► SRV-3 ──► SRV-4
                  DAT-4 ──► (user context)

CFG-1 ──► DB-1

CHAT-1 ──► CHAT-2 ──► CHAT-3
IMG-2  ──► IMG-3  ──► IMG-4

RTC-3 ──► RTC-1 ──► RTC-2   (WebRTC — blocked / WIP)
```

### 6.2 Current Feature Status

| Feature Area | IDs | Status |
|--------------|-----|--------|
| Three-panel layout | UI-1 … UI-4 | ✅ Implemented |
| Server management | SRV-1 … SRV-6 | ✅ Implemented |
| Friends & DMs | FRI-1 … FRI-4 | ✅ Implemented |
| Friends & DMs | FRI-5 (add friend) | ⚠️ Placeholder only |
| Real-time chat | CHAT-1 … CHAT-6 | ✅ Implemented (requires WS server) |
| Image messaging | IMG-1 … IMG-7 | ✅ Implemented (placeholder picker) |
| Voice / video calls | RTC-1 … RTC-4 | 🚧 WIP — `WebRTCManager` commented out |
| Database init | DB-1 … DB-4 | ✅ Implemented |
| Data access | DAT-1 … DAT-4 | ✅ Implemented |
| Configuration | CFG-1 … CFG-3 | ✅ Implemented |

### 6.3 Known Gaps & TODOs

| Area | Gap | Location |
|------|-----|----------|
| Add Friend | Clicking "+" shows a placeholder label — no real add-friend flow | `MuffleMain.xaml.cs` → `FriendsTopBarUIView_FriendAddButtonClicked` |
| Image Picker | Returns a hardcoded path — needs real `FilePicker` integration | `ImagePickerService.cs` → `PickImageAsync` |
| WebRTC | Entire `WebRTCManager.cs` is commented out | `Muffle.Data/Services/WebRTCManager.cs` |
| User identity | `UsersService.GetUser()` returns an empty `User` — no login | `UsersService.cs` |
| DI | Services are instantiated manually — DI container is not wired up | `MauiProgram.cs` (builder.Services unused) |
| Server top bar | `ServerTopBarUIView` is constructed but content is set to `null` | `MuffleMain.xaml.cs` → `UpdateSharedUIFrame` |
| Search | "Find or start a discussion" `Entry` has no backing logic | `MuffleMain.xaml` |

---

## 7. Working with Specific Layers

### 7.1 Adding a New Database Table

1. Open `Muffle.Data/Services/SqliteDbService.cs`.
2. Add a `CREATE TABLE IF NOT EXISTS` statement inside `InitializeDatabase()`.
3. Add a `DROP TABLE IF EXISTS` statement inside `DisposeDatabase()` (**before** tables that reference it via FK, or after tables that depend on it — mind FK ordering).
4. Add seed `INSERT` statements if the table needs default data.
5. Repeat steps 1–4 in `Muffle.Data/Services/SqlServerDbService.cs` using T-SQL syntax (`IF NOT EXISTS (SELECT * FROM sysobjects …)`, `NVARCHAR`, `IDENTITY`, etc.).
6. Create a corresponding model class in `Muffle.Data/Models/`.
7. Build: `dotnet build Muffle.Data/Muffle.Data.csproj`.

### 7.2 Adding a New Service Method

1. Add the method to the appropriate service class in `Muffle.Data/Services/`.
2. Follow the existing pattern: `public static` method, create connection with `using var connection = SQLiteDbService.CreateConnection();`, open it, run Dapper query.
3. Return strongly-typed results mapped from the model class.
4. Build: `dotnet build Muffle.Data/Muffle.Data.csproj`.

### 7.3 Adding a New View

1. Create a XAML file in `Muffle/Views/` (or a subfolder for component views).
2. Create the matching `.xaml.cs` code-behind file.
3. Accept the model or ViewModel via the constructor and set `BindingContext`.
4. Wire the view into `MuffleMain.xaml.cs` by adding a new branch in `UpdateMainContentFrame()` and/or `UpdateSharedUIFrame()`.
5. Build: full solution build.

### 7.4 Adding a New ViewModel

1. Create a class in `Muffle/ViewModels/` extending `BindableObject`.
2. Expose `ObservableCollection<T>` for list data and `ICommand` for actions.
3. Call `OnPropertyChanged()` from property setters.
4. Instantiate the ViewModel in the View's constructor (manual DI pattern used throughout).

---

## 8. WebSocket / Signaling Work

All real-time messaging flows through `ISignalingService` → `SignalingService`:

| Operation | Method | Notes |
|-----------|--------|-------|
| Connect | `ConnectAsync(Uri)` | Currently hardcoded to `ws://localhost:8080` |
| Send text | `SendMessageAsync(string)` | Raw string |
| Send structured | `SendMessageWrapperAsync(MessageWrapper)` | JSON-serialized DTO |
| Receive | `ReceiveMessageAsync()` | Returns raw string; caller deserializes |
| Receive structured | `ReceiveMessageWrapperAsync()` | Deserializes to `MessageWrapper` |

**Important:** A WebSocket server at `ws://localhost:8080` must be running for chat to work. The app does **not** include the server — it is a separate process.

---

## 9. Testing Strategy

### 9.1 Current State

- No formal test project exists in the solution.
- `Muffle.Data/Tests/ImageMessageDemo.cs` is a manual demo, not a test-runner test.

### 9.2 Recommended Approach for Adding Tests

1. Create a new project: `Muffle.Data.Tests` targeting `net10.0` with xUnit.
2. Reference `Muffle.Data`.
3. Test service methods against an in-memory SQLite database (`:memory:` connection string).
4. Mirror class names: `UsersService` → `UsersServiceTests`.
5. Name tests by behavior: `GetUsersServers_WhenDatabaseSeeded_ReturnsThreeServers`.
6. Follow Arrange-Act-Assert. One assertion per test.

### 9.3 Validation Without Tests

When no tests exist for the area being modified:

1. Build successfully.
2. Review `get_errors` output for the modified files.
3. Manually trace the call chain from the entry point (`MauiProgram.cs` → ViewModel → Service → DB).

---

## 10. Session Checklist

Use this checklist at the start and end of every agent session:

### Start of Session
- [ ] Read `SPEC.md` for current requirements
- [ ] Read `.github/copilot-instructions.md` for build/env constraints
- [ ] Identify which requirement IDs to work on
- [ ] Review current state of target files

### End of Session
- [ ] All modified files compile (`dotnet build Muffle.Data/Muffle.Data.csproj` at minimum)
- [ ] No regressions — existing features still work (full build if possible)
- [ ] Any new requirement IDs completed are noted
- [ ] Blockers and open questions are documented
- [ ] No uncommitted partial implementations left in a broken state
