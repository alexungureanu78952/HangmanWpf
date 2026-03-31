# Implementation Progress Tracker

**Last Updated**: March 31, 2025  
**Current Status**: Ready for Phase 1  
**Overall Progress**: 0/7 phases complete

---

## Quick Status Summary

| Phase | Name | Status | Start Date | End Date | Notes |
|-------|------|--------|-----------|----------|-------|
| 1 | Foundation | ⬜ Not Started | — | — | Project setup, models, utilities |
| 2 | Data Layer | ⬜ Not Started | — | — | Services (User, Word, Stats, Persistence) |
| 3 | Theme System | ⬜ Not Started | — | — | Dynamic color loading & switching |
| 4 | Game Logic | ⬜ Not Started | — | — | GameService engine (guessing, timer, winning) |
| 5 | ViewModels | ⬜ Not Started | — | — | State management + Commands |
| 6 | Views (XAML) | ⬜ Not Started | — | — | UI layouts (pure binding) |
| 7 | Integration | ⬜ Not Started | — | — | Navigation, keyboard, end-to-end tests |

---

## Phase 1: Foundation

**Objective**: Create project structure, models, and utility classes  
**Status**: ⬜ Not Started  
**Expected Duration**: 2-3 hours  
**Deliverables**:
- [ ] .NET Core WPF project created
- [ ] NuGet packages installed (Newtonsoft.Json, MVVM Toolkit, others as needed)
- [ ] `Models/` folder with 5 model classes
- [ ] `Utilities/` folder with RelayCommand, AsyncRelayCommand, PathHelpers
- [ ] `ViewModels/ViewModelBase.cs` implemented
- [ ] Project compiles without errors
- [ ] Models serialize/deserialize JSON correctly

**Key Files to Create**:
- `Models/User.cs` — User model with GUID, username, image path
- `Models/GameSession.cs` — Current game state (word, guesses, level, timer, etc.)
- `Models/SavedGame.cs` — Extends GameSession with save metadata
- `Models/Statistics.cs` — User statistics per category
- `Models/ThemeDefinition.cs` — Theme colors definition
- `Utilities/RelayCommand.cs` — Non-generic ICommand implementation
- `Utilities/AsyncRelayCommand.cs` — Async-capable ICommand
- `Utilities/PathHelpers.cs` — Relative path resolution
- `ViewModels/ViewModelBase.cs` — INotifyPropertyChanged base class

**Verification Checklist**:
- [ ] `dotnet run` starts without exceptions
- [ ] Create a User object → serialize to JSON → deserialize back → matches original
- [ ] RelayCommand.Execute() calls action, CanExecute() evaluates predicate
- [ ] PathHelpers.GetRelativePath("test.json") returns valid path
- [ ] ViewModelBase.SetProperty() fires PropertyChanged event

**Dependencies**: None (foundation phase)  
**Blocks**: Phase 2 (needs models), Phase 5 (needs ViewModelBase, RelayCommand)

---

## Phase 2: Data Layer

**Objective**: Implement all services for persistence and game data  
**Status**: ⬜ Not Started  
**Expected Duration**: 4-5 hours  
**Depends On**: Phase 1 ✓

**Deliverables**:
- [ ] `IUserService` interface + `UserService` implementation
- [ ] `IWordService` interface + `WordService` implementation
- [ ] `IStatisticsService` interface + `StatisticsService` implementation
- [ ] `IGamePersistenceService` interface + `GamePersistenceService` implementation
- [ ] Theme JSON files created (DarkPurpleTheme.json, DarkRedTheme.json)
- [ ] Words JSON files created (AllCategories.json per category)
- [ ] File I/O operations use async/await
- [ ] Relative paths validated across all services

**Key Files to Create**:
- `Services/IUserService.cs` & `UserService.cs` — CRUD users.json
- `Services/IWordService.cs` & `WordService.cs` — Load words by category
- `Services/IStatisticsService.cs` & `StatisticsService.cs` — Track game stats
- `Services/IGamePersistenceService.cs` & `GamePersistenceService.cs` — Save/load game snapshots
- `Resources/Themes/DarkPurpleTheme.json` — Purple theme colors
- `Resources/Themes/DarkRedTheme.json` — Red theme colors
- `Resources/Words/AllCategories.json` — Word lists per category

**Verification Checklist**:
- [ ] Create new user → saved to users.json
- [ ] Load users → matches saved data
- [ ] Delete user → removed from users.json + stats deleted + save folder deleted
- [ ] LoadWords("Movies") → returns list of movie words
- [ ] SaveGame(gameSession) → creates JSON file in SavedGames/{UserId}/
- [ ] LoadGame(userId, gameId) → restores exact GameSession state
- [ ] Stats persist after game completion
- [ ] All paths verified as relative (no C:\Users\...)

**Dependencies**: Phase 1 ✓  
**Blocks**: Phase 4 (GameService needs WordService), Phase 5 (ViewModels inject services)

---

## Phase 3: Theme System

**Objective**: Implement dynamic theme loading & switching (zero hardcoded colors)  
**Status**: ⬜ Not Started  
**Expected Duration**: 2-3 hours  
**Depends On**: Phase 1 ✓, Phase 2 ✓ (theme JSON files)

**Deliverables**:
- [ ] `IThemeService` interface + `ThemeService` implementation
- [ ] Theme JSON files created with color definitions
- [ ] ApplyTheme() loads JSON → populates Application.Resources
- [ ] DynamicResource binding works in XAML
- [ ] Theme switching at runtime (no recompilation)
- [ ] All color keys centralized in JSON

**Key Files to Create**:
- `Services/IThemeService.cs` & `ThemeService.cs` — Apply themes dynamically
- `Resources/Themes/DarkPurpleTheme.json` — Color palette (Purple theme)
- `Resources/Themes/DarkRedTheme.json` — Color palette (Red theme)

**Theme JSON Structure** (per theme file):
```json
{
  "name": "DarkPurple",
  "colors": {
    "PrimaryBackground": "#1a1a2e",
    "SecondaryBackground": "#16213e",
    "Foreground": "#eaeaea",
    "Accent": "#9d4edd",
    "AccentDark": "#7209b7",
    "ButtonHover": "#3a0ca3",
    "Border": "#5a189a"
  }
}
```

**Verification Checklist**:
- [ ] App.xaml.cs calls ThemeService.ApplyTheme("DarkPurple") on startup
- [ ] Application.Resources contains SolidColorBrush for each color key
- [ ] XAML uses `{DynamicResource PrimaryBackground}` (never hardcoded colors)
- [ ] Switch theme at runtime → all windows update colors immediately
- [ ] No recompilation needed after theme change

**Dependencies**: Phase 1 ✓, Phase 2 ✓  
**Blocks**: Phase 6 (XAML binds to theme colors)

---

## Phase 4: Game Logic

**Objective**: Implement game engine (word guessing, timer, state machine)  
**Status**: ⬜ Not Started  
**Expected Duration**: 3-4 hours  
**Depends On**: Phase 1 ✓, Phase 2 ✓

**Deliverables**:
- [ ] `IGameService` interface + `GameService` implementation
- [ ] GuessLetter() → updates word state, checks win/loss
- [ ] Timer (30 seconds per word) with countdown
- [ ] Win/loss detection: 6 wrongs = lose, timeout = lose, 3 consecutive wins = game won
- [ ] Level tracking (0-3): resets on loss, persists on save
- [ ] ASCII hangman art (6-character progression)
- [ ] Unit tests for game logic

**Key Files to Create**:
- `Services/IGameService.cs` & `GameService.cs` — Game engine
- ASCII art constants (in GameService or separate Hangman.cs)

**Game State Machine**:
```
NotStarted → (StartGame) → InProgress
InProgress → (AllLettersGuessed or TimeoutOr6Wrongs) → WordLost/Won
WordLost/Won → (ResetCounter/IncrementLevel) → InProgress OR GameWon
GameWon → (Cancel) → NotStarted
```

**Verification Checklist**:
- [ ] Correct guess → word display updates with letter in correct positions
- [ ] Wrong guess → IncorrectCount increments, hangman art updates
- [ ] 6 wrong guesses → IsGameLost() returns true
- [ ] Timer countdown works, displays remaining time
- [ ] Timeout (30s) → IsGameLost() returns true
- [ ] Win 3 consecutive words → IsGameWon() returns true (entire game won)
- [ ] Level counter resets on word loss
- [ ] SavedGame deserializes → GameService restores exact state

**Dependencies**: Phase 1 ✓, Phase 2 ✓  
**Blocks**: Phase 5 (ViewModels use GameService)

---

## Phase 5: ViewModels & Commands

**Objective**: Implement all ViewModels with ICommand properties (business logic)  
**Status**: ⬜ Not Started  
**Expected Duration**: 4-5 hours  
**Depends On**: Phase 1 ✓, Phase 2 ✓, Phase 4 ✓

**Deliverables**:
- [ ] `LoginWindowViewModel.cs` — User selection + Create/Delete commands
- [ ] `GameWindowViewModel.cs` — Game state + Guess/Save/Load commands
- [ ] `NewUserWindowViewModel.cs` — User creation + Image browse command
- [ ] `StatisticsWindowViewModel.cs` — Stats display
- [ ] `SaveLoadDialogViewModel.cs` — Save/load games
- [ ] `AboutWindowViewModel.cs` — Static info display
- [ ] Minimum 7 commands implemented (GuessLetter, Save, Load, Delete, Create, etc.)
- [ ] All ViewModels inject services via constructor
- [ ] No business logic duplicated between VMs

**Key Commands** (Minimum 7):
1. `GuessLetterCommand` — Execute letter guess, update UI
2. `StartNewGameCommand` — Initialize new word, reset UI
3. `SaveGameCommand` — Serialize current session
4. `LoadGameCommand` — Restore saved session
5. `CreateUserCommand` — Add user to persistence
6. `DeleteUserCommand` — Cascade delete user data
7. `ChangeThemeCommand` — Apply theme at runtime

**Verification Checklist**:
- [ ] Each ViewModel has constructor dependency injection (no `new Service()`)
- [ ] All commands execute without null reference exceptions
- [ ] Command.CanExecute() correctly enables/disables button
- [ ] PropertyChanged events fire when properties update
- [ ] Game state persists when saved/loaded
- [ ] User deletion cascades: stats deleted, saves deleted, user removed

**Dependencies**: Phase 1 ✓, Phase 2 ✓, Phase 4 ✓  
**Blocks**: Phase 6 (XAML binds to VM properties/commands), Phase 7 (integration)

---

## Phase 6: Views (XAML)

**Objective**: Create ui layouts with pure binding (no code-behind logic)  
**Status**: ⬜ Not Started  
**Expected Duration**: 4-5 hours  
**Depends On**: Phase 3 ✓, Phase 5 ✓

**Deliverables**:
- [ ] `LoginWindow.xaml` — User list + buttons (Delete/Play disabled until selected)
- [ ] `GameWindow.xaml` — Game UI with menu, hangman, word display, letter buttons, timer
- [ ] `NewUserWindow.xaml` — Username input + image browser
- [ ] `StatisticsWindow.xaml` — Stats table
- [ ] `SaveLoadDialog.xaml` — Save/load/delete game options
- [ ] `AboutWindow.xaml` — Student info
- [ ] All colors use `DynamicResource` (never hardcoded)
- [ ] All interaction via Command binding (never event handlers)
- [ ] All text uses DataContexts (never hardcoded text)

**XAML Rules**:
- ✅ Correct: `<Button Command="{Binding GuessLetterCommand}" CommandParameter="A" />`
- ❌ Wrong: `<Button Click="Button_Click" />`
- ✅ Correct: `<TextBlock Foreground="{DynamicResource Foreground}" />`
- ❌ Wrong: `<TextBlock Foreground="White" />`
- ✅ Correct: `<Label Content="{Binding Username}" />`
- ❌ Wrong: `<Label>Hardcoded Username</Label>`

**Verification Checklist**:
- [ ] App launches → LoginWindow appears
- [ ] Select user → Play/Delete buttons enable
- [ ] Click Play → GameWindow opens with selected user info
- [ ] Game window displays timer, hangman, word, letter buttons
- [ ] Letter buttons click-enabled → disabled after clicked
- [ ] Theme changes → all windows update colors (no recompilation)
- [ ] Close button → returns to previous window
- [ ] Save/load dialogs display correctly
- [ ] Statistics window shows user stats

**Dependencies**: Phase 3 ✓, Phase 5 ✓  
**Blocks**: Phase 7 (integration testing)

---

## Phase 7: Integration & Finalization

**Objective**: Wire everything together, test end-to-end, handle edge cases  
**Status**: ⬜ Not Started  
**Expected Duration**: 3-4 hours  
**Depends On**: Phases 1-6 ✓

**Deliverables**:
- [ ] Window navigation working (Login ↔ Game ↔ Stats)
- [ ] Letter keyboard input (A-Z) triggers GuessLetter
- [ ] File menu actions (New Game, Open Game, Save Game, Cancel)
- [ ] Categories menu populates correctly
- [ ] Cascading user deletion verified (5-part cleanup)
- [ ] Save game → switch users → cannot load other user's save
- [ ] Save game → load game → continue playing → win/lose states captured
- [ ] Statistics update on game completion (only after 3-level win or loss)
- [ ] Timeout logic works (30s per word)
- [ ] ASCII hangman art displays correctly (6 stages)
- [ ] Theme switching works at runtime (all windows update)
- [ ] No crashes on edge cases (empty strings, missing files, etc.)

**Key Integration Scenarios**:

1. **Full Game Loop**
   - Start app → Login window appears
   - Create user "TestPlayer" with custom image
   - Click Play → Game window opens, Category menu, select "Movies"
   - Click File → New Game → Game starts with random movie word
   - Guess correct letters → word updates
   - Guess 3 wrong → hangman updates, wrong count shows "3"
   - Continue guessing → win word (level = 1/3)
   - Game continues with new word (level = 2/3)
   - Win 2nd word (level = 3/3)
   - Guess 3rd word completely → "CongraGame Won!" message
   - Statistics updated: TestPlayer | Movies | 1 played | 1 won
   - Click Cancel → back to Login

2. **Save/Load Game**
   - Mid-game, File → Save Game → enter "save1"
   - Switch to different user → play different game
   - Switch back to TestPlayer → File → Open Game → select "save1"
   - Game state restored exactly: same word, same wrong count, same level, same timer
   - Continue game from saved point

3. **Cascading Delete**
   - Delete TestPlayer → confirm dialog
   - Verify user removed from users.json
   - Verify stats removed from statistics.json
   - Verify save folder deleted: SavedGames/TestPlayer/
   - Verify all references cleaned up

4. **Theme Switching**
   - In-game, change theme from Dark-Purple to Dark-Red
   - All windows update colors immediately (continue playing)
   - No recompilation needed
   - Colors persist if app restarts (set in App.xaml.cs)

5. **Timeout Logic**
   - Start game, do nothing for 30+ seconds
   - Timer reaches 0:00 → Game Lost notification
   - Statistics updated (game counted as lost)
   - New word offered or return to menu

6. **Keyboard Input**
   - Focus on game window (any control)
   - Press 'A' → GuessLetterCommand triggered → "A" guessed
   - Press 'B' → "B" guessed (if not already guessed)
   - Press 'B' again → no effect (already guessed)
   - Tab → focus moves to buttons (standard WPF behavior)

**Verification Checklist**:
- [ ] All 6 scenarios above work without errors
- [ ] No crashes on:
  - [ ] Empty username input
  - [ ] Missing image file (user deleted externally)
  - [ ] Corrupted JSON file (graceful error handling)
  - [ ] Game window still open when user deleted from another window
  - [ ] Rapid consecutive guesses
- [ ] Statistics file properly formatted after multiple games
- [ ] ASCII hangman displays all 6 states clearly
- [ ] Timer countdown accurate (±100ms tolerance acceptable)
- [ ] No memory leaks (windows dispose properly, timers stop)

**Final Checklist**:
- [ ] Code follows naming conventions (PascalCase public, camelCase private)
- [ ] All public APIs have XML doc comments
- [ ] No TODO/FIXME comments left in code
- [ ] RelativeCheck: No absolute paths found in code (`Find: "C:\\"` → 0 results)
- [ ] ColorCheck: No hardcoded colors in XAML (`Find: "#[0-9A-F]{6}"` in .xaml → 0 results)
- [ ] CodeBehindCheck: ViewModels properly injected (no `new ViewModel()`)
- [ ] AppCompiles: `dotnet build` succeeds with 0 warnings
- [ ] AppRuns: `dotnet run` starts without exceptions

**Dependencies**: All Phases 1-6 ✓

---

## Scoring Verification

| Feature | Points | Test Scenario | Status |
|---------|--------|---------------|--------|
| **Login/Sign up** | 1 | Create user → saved to JSON | ⬜ |
| **Game implementation** | 2 | Guess letter → word updates; wrong guess → hangman updates | ⬜ |
| **Game reset** | 1 | New word → level counter shows correct value | ⬜ |
| **Timer** | 1 | Countdown 30s → game lost on timeout | ⬜ |
| **Save game** | 1 | File → Save Game → JSON created | ⬜ |
| **Load game** | 1 | File → Open Game → state restored exactly | ⬜ |
| **Statistics** | 1 | Win/lose game → stats file updated | ⬜ |
| **Delete user** | 1 | Delete user → user + stats + saves all gone | ⬜ |
| **Bonus** (theme + MVVM + code) | 1 | Switch themes, verify MVVM structure, review code quality | ⬜ |
| **Total** | **9** | | ⬜ |

---

## Known Issues & Workarounds

(Update as issues discovered)

---

## Notes

- Update this file after completing each phase
- Mark deliverables with ✅ as they're completed
- Note any blockers or challenges in "Known Issues"
- Keep track of time spent per phase for future reference
- Verify against `.github/copilot-instructions.md` before marking phase complete

---

*Last Updated*: March 31, 2025 (Created)  
*Next Review*: After Phase 1 completion
