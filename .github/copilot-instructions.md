# Hangman WPF Game - Copilot Instructions

## Overview
This is a **Hangman Word Guessing Game** built in **C# WPF .NET Core** following strict **MVVM architecture** with dynamic theming, JSON-based persistence, and comprehensive game state management.

**Target Completion**: Week of April 20-24, 2025 (3 weeks)  
**Scoring Rubric**: 9 points total (see Scoring section)

---

## Architecture Principles (STRICT)

### 1. MVVM Separation of Concerns
- **View Layer (XAML)**: Pure binding only, ZERO code-behind logic
  - All behavior → bound to ViewModel commands/properties
  - All colors → DynamicResource (never hardcoded)
  - All interaction → ICommand binding (never events)
  
- **ViewModel Layer**: Business logic + state management
  - Implements `INotifyPropertyChanged` via `ViewModelBase`
  - Exposes `ICommand` properties for each user action
  - No direct UI manipulation (no MessageBox, no window.Close())
  - Stateful: tracks game state, UI state, navigation state
  
- **Service Layer**: Business rules + I/O operations
  - `IUserService` — User persistence (CRUD JSON)
  - `IGameService` — Game engine (guessing logic, timer, win/loss)
  - `IGamePersistenceService` — Save/load game snapshots
  - `IStatisticsService` — Track and persist game statistics
  - `IThemeService` — Apply themes dynamically at runtime
  - `IWordService` — Load words by category
  
- **Model Layer**: Data structures (all JSON-serializable)
  - `User` — {UserId: GUID, Username: string, ImagePath: relative, CreatedDate}
  - `GameSession` — {Word, GuessedLetters, Level, TimeRemaining, Category, WrongCount}
  - `SavedGame` — Extends GameSession, adds SavedDate, UserId
  - `Statistics` — {UserId, CategoryStats: [{Category, GamesPlayed, GamesWon}]}
  - `ThemeDefinition` — {Name: string, Colors: Dictionary<string, string>}

### 2. No Hardcoded Colors in XAML
Every color must be:
- Defined in JSON theme files (`DarkPurpleTheme.json`, `DarkRedTheme.json`)
- Loaded into `Application.Current.Resources` by `ThemeService` at startup
- Referenced via `{DynamicResource ColorKey}` in XAML
- Changeable at runtime without recompilation

Example:
```xml
<!-- CORRECT -->
<Window Background="{DynamicResource PrimaryBackground}" 
        Foreground="{DynamicResource Foreground}">

<!-- WRONG - never do this -->
<Window Background="#1a1a2e" Foreground="#eaeaea">
```

### 3. Commands (Minimum 7 Required)
Implement these commands using `RelayCommand` or `AsyncRelayCommand`:

1. **GuessLetterCommand** (GameWindowViewModel) — Execute letter guess
2. **StartNewGameCommand** (GameWindowViewModel) — Reset word engine
3. **SaveGameCommand** (GameWindowViewModel) — Serialize session to JSON
4. **LoadGameCommand** (GameWindowViewModel) — Restore saved session
5. **CreateUserCommand** (NewUserWindowViewModel) — Add user to users.json
6. **DeleteUserCommand** (LoginWindowViewModel) — Remove user + all associated data
7. **ChangeThemeCommand** (AppViewModel/MainViewModel) — Apply theme at runtime

Extras (for robustness):
- SelectUserCommand
- BrowseImageCommand
- OpenGameCommand (File menu)
- NewGameCommand (File menu)
- CancelCommand (window close/return)

### 4. Data Persistence Strategy
- **Format**: JSON (human-readable, no database)
- **File Locations** (all relative paths):
  - `Resources/Data/users.json` — User list with avatar paths
  - `Resources/Data/statistics.json` — Game stats per user/category
  - `Resources/Words/AllCategories.json` — Word lists per category
  - `Resources/Themes/{ThemeName}Theme.json` — Color definitions
  - `Resources/Data/SavedGames/{UserId}/{GameId}.json` — Saved game sessions

- **Relative Paths**: Critical requirement
  - Use `AppDomain.CurrentDomain.BaseDirectory` as root
  - Never use absolute paths (C:\Users\...) — breaks on other machines
  - Verify with `PathHelpers.GetRelativePath()` helper

### 5. Game Logic Rules
- **Lives**: 6 incorrect guesses per word
- **Timer**: 30 seconds per word, countdown displayed
- **Win Condition**: 3 consecutive words guessed within 30s → Game Won
- **Lose Condition**: 6 wrong guesses OR timer expires → Round lost (reset level counter)
- **Level Tracking**: 
  - Numbered 0-3 (display on UI: "Level: 1/3")
  - Resets to 0 if player loses a word
  - Persists across game saves/loads
- **Category Selection**: Choose at login/game start
  - "All Categories" = union of all word lists
  - Changing category resets level counter to 0
- **Hangman Display**: ASCII art (not images)
  - 6-character progression (0-6 wrong guesses)
  - String constants (not loaded from files)

### 6. Window Structure & Navigation

**Login Window** (startup)
- User list (ListBox, selectable)
- Selected user image (Image control)
- Buttons: New User, Delete User, Play
  - Delete/Play disabled until user selected
  - Delete → confirm dialog → cascade delete (stats + saves)
- Cancel → app closes

**New User Window** (modal from Login)
- Username input (single word only)
- Image browser (OpenFileDialog)
  - Accept: .jpg, .png, .gif
  - Store relative path in users.json
- Buttons: Browse, Create, Cancel
- Validation: username not empty, image selected

**Game Window** (main gameplay)
- Menu bar:
  - File: New Game, Open Game, Save Game, Cancel
  - Categories: All categories, Cars, Movies, Rivers, Mountains, States
  - Help: About
- Game layout:
  - Top-left: User name + avatar image
  - Left side: ASCII hangman art (updates per wrong guess)
  - Center: Word display (e.g., "_ _ _ _ D")
  - Right side: Letter button grid (A-Z, click/keyboard disables)
  - Bottom: Timer (MM:SS countdown), Level (X/3), Wrong count
  - Status bar: Feedback ("Correct!", "Wrong!", "Game Won!", "Game Lost!")

**Statistics Window** (modal from Game/Login)
- Table: UserId | Category | Games Played | Games Won
- All users, all categories, sortable
- Button: Close

**Save/Load Dialog** (modal from Game menu)
- Save section: Input game name, Save button
- Load section: List of user's saved games
  - Columns: Name, Timestamp, Level, Progress
  - Buttons: Load, Delete, Cancel

**About Window** (modal from Help menu)
- Student info: Name, Group number, Specialization
- OK button

### 7. Keyboard Support
- **Letter Keys (A-Z)**: Trigger GuessLetterCommand directly
  - Disabled if GuessLetter already called in this round
  - Or if game is won/lost
- **Tab/Shift+Tab**: Navigate between buttons (standard WPF behavior)
- Menu shortcuts: NOT required (keep UI simple)

### 8. Cascading Deletion
When user is deleted:
1. Remove from `users.json`
2. Delete image file reference (or log if file missing)
3. Remove all stats entries in `statistics.json`
4. Delete entire user's save folder: `Resources/Data/SavedGames/{UserId}/`
5. Confirm deletion with user

---

## Project Structure

```
HangmanWpf/
├── .github/
│   ├── copilot-instructions.md (this file)
│   └── plan.md (progress tracker - updated after each phase)
├── App.xaml (.cs)
├── Models/
│   ├── User.cs
│   ├── GameSession.cs
│   ├── SavedGame.cs
│   ├── Statistics.cs
│   └── ThemeDefinition.cs
├── Services/
│   ├── IUserService.cs & UserService.cs
│   ├── IGameService.cs & GameService.cs
│   ├── IGamePersistenceService.cs & GamePersistenceService.cs
│   ├── IStatisticsService.cs & StatisticsService.cs
│   ├── IThemeService.cs & ThemeService.cs
│   ├── IWordService.cs & WordService.cs
├── ViewModels/
│   ├── ViewModelBase.cs
│   ├── LoginWindowViewModel.cs
│   ├── GameWindowViewModel.cs
│   ├── NewUserWindowViewModel.cs
│   ├── StatisticsWindowViewModel.cs
│   ├── SaveLoadDialogViewModel.cs
│   └── AboutWindowViewModel.cs
├── Views/
│   ├── LoginWindow.xaml (.cs - binding only)
│   ├── GameWindow.xaml (.cs - binding only)
│   ├── NewUserWindow.xaml (.cs - binding only)
│   ├── StatisticsWindow.xaml (.cs - binding only)
│   ├── SaveLoadDialog.xaml (.cs - binding only)
│   ├── AboutWindow.xaml (.cs - binding only)
│   ├── Converters/
│   │   ├── BoolToVisibilityConverter.cs
│   │   ├── GuessedLetterHighlightConverter.cs
│   │   └── TimeSpanToStringConverter.cs
├── Resources/
│   ├── Themes/
│   │   ├── DarkPurpleTheme.json
│   │   └── DarkRedTheme.json
│   ├── Words/
│   │   └── AllCategories.json
│   ├── Data/ (generated at runtime)
│   │   ├── users.json
│   │   ├── statistics.json
│   │   └── SavedGames/
├── Utilities/
│   ├── RelayCommand.cs
│   ├── AsyncRelayCommand.cs
│   └── PathHelpers.cs
└── HangmanWpf.csproj

```

---

## Scoring Rubric (9 Points Total)

| Feature | Points | Status |
|---------|--------|--------|
| Login/Sign up | 1 | ⬜ Not Started |
| Game implementation (4-6 lives, timer, word display) | 2 | ⬜ Not Started |
| Game reset (new word, maintain level counter) | 1 | ⬜ Not Started |
| Timer (30s countdown, timeout logic) | 1 | ⬜ Not Started |
| Save game (serialize session) | 1 | ⬜ Not Started |
| Load game (restore & continue) | 1 | ⬜ Not Started |
| Statistics (per-user, per-category breakdown) | 1 | ⬜ Not Started |
| Delete user (cascade delete all data) | 1 | ⬜ Not Started |
| Bonus (theme + architecture + code quality) | 1 | ⬜ Not Started |
| **Total** | **9** | |

---

## Implementation Phases (7 Total)

**See `.github/plan.md` for detailed progress tracking. Update after each phase completes.**

1. **Phase 1: Foundation** — Project setup, models, utilities
2. **Phase 2: Data Layer** — Services (User, Word, Stats, Persistence)
3. **Phase 3: Theme System** — Dynamic color loading
4. **Phase 4: Game Logic** — GameService engine
5. **Phase 5: ViewModels** — State + commands
6. **Phase 6: Views (XAML)** — UI layouts (binding only)
7. **Phase 7: Integration** — Window navigation, keyboard input, end-to-end testing

---

## Code Quality Standards

### Do's ✅
- Use explicit dependency injection (constructor params)
- Use `INotifyPropertyChanged` with property setters
- Use `DynamicResource` for all colors/theme values
- Use async/await for file I/O (via `AsyncRelayCommand`)
- Use relative paths (validate with `PathHelpers`)
- Use meaningful variable/method names
- Use XML documentation comments on public APIs
- Use LINQ for collections (no foreach when LINQ available)
- Validate user input (username not empty, image path exists)
- Log errors (use Debug.WriteLine or ILogger if available)

### Don'ts ❌
- Never hardcode colors in XAML
- Never add logic to code-behind (Views)
- Never use absolute paths
- Never use `MessageBox` (use dialog ViewModels instead)
- Never directly modify UI elements from Services
- Never make Services depend on Views/ViewModels
- Never serialize internal IDs (e.g., don't expose GameSession.Id in JSON)
- Never forget to handle exceptions (file I/O, JSON parsing)
- Never duplicate theme colors (centralize in JSON)
- Never leave Debug.WriteLine in production (but keep clean logging)

### Best Practices
- Use *static factory methods* for complex object creation
- Use *value objects* (immutable records) for simple Models
- Use *guards* (null checks) at service entry points
- Use *dependency injection* for testability
- Use *readonly fields* for dependencies
- Keep Services small and focused (Single Responsibility)
- Keep ViewModels thin (delegate logic to Services)
- Keep Views dumb (delegate logic to ViewModels)

---

## Testing Strategy

### Unit Tests (Optional but Recommended)
- GameService.GuessLetter() → correct word state
- GameService.IsGameWon() → 3 consecutive levels
- UserService.CreateUser() → user in JSON
- StatisticsService.UpdateStats() → category tracked
- PathHelpers.GetRelativePath() → portable paths

### Integration Tests
- Create user → play game → save game → load game → continue → win
- Delete user → verify cascade delete (files gone)
- Theme switch → all windows update colors

### Manual Acceptance Tests
See `.github/plan.md` — "Verification" section for each phase

---

## Common Pitfalls to Avoid

1. **Hardcoded Colors**: XAML with `Background="#FF000000"` instead of `DynamicResource`
   - ❌ Bad: Cannot change theme at runtime
   - ✅ Good: `Background="{DynamicResource PrimaryBackground}"`

2. **Logic in Code-Behind**: Button_Click methods with game logic
   - ❌ Bad: Tests fail, hard to maintain
   - ✅ Good: Button Command binding to ViewModel method

3. **Absolute Paths**: `C:\Users\student\AppData\...`
   - ❌ Bad: Breaks on different machine
   - ✅ Good: `PathHelpers.GetRelativePath("Resources/Data/users.json")`

4. **Synchronous File I/O**: `File.ReadAllText()` blocks UI thread
   - ❌ Bad: UI freezes during file ops
   - ✅ Good: `await File.ReadAllTextAsync()` in AsyncRelayCommand

5. **Cascading Delete Bugs**: Forgetting to delete one of:
   - User from users.json
   - Stats from statistics.json
   - Saved games folder
   - Image file reference
   - ❌ Bad: Orphaned data
   - ✅ Good: Delete all 5 in transaction-like order

6. **Timer Not Stopping**: Countdown continues after game ends
   - ❌ Bad: Timer fires after window closes
   - ✅ Good: Stop timer in CancelCommand, SaveCommand, etc.

7. **Theme Not Applied on Startup**: Only dark blue hardcoded
   - ❌ Bad: Theme file ignored
   - ✅ Good: ApplyTheme() called in App.xaml.cs constructor

---

## Resources & Documentation

- **MVVM Pattern**: https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm
- **WPF Data Binding**: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/data-binding-overview
- **Relative Paths in .NET**: AppDomain.CurrentDomain.BaseDirectory
- **JSON Serialization**: Newtonsoft.Json (Json.NET)
- **Async Patterns**: Task, async/await, CancellationToken

---

## Key Dates

- **March 31, 2025**: Planning & clarifications ✅
- **Week of April 7-11**: Phases 1-3 (Foundation + Data + Theme)
- **Week of April 14-18**: Phases 4-5 (Logic + ViewModels)
- **Week of April 21-24**: Phase 6-7 (Views + Integration) + Submission
  - *Deadline*: Friday, April 25 by EOD (submit via lab platform)

---

## Contact & Questions

If implementation feels unclear:
1. Check plan.md for current phase status
2. Review the "Common Pitfalls" section
3. Verify against the Architecture Principles
4. Run unit tests to validate assumptions
5. Ask for clarification with specific code snippet

**Every phase ends with a Verification checklist — do not proceed until ✅**

---

*Last Updated: March 31, 2025*
*Status: Plan Approved, Ready for Phase 1*
