# Chess World — Phase 1 Design Spec

**Date:** 2026-04-21
**Status:** Approved for implementation planning
**Scope:** Phase 1 of 4 (Core)

---

## 1. Project overview

Chess World is a 2D mobile chess game with a zombie-versus-humans theme. The player always controls white (humans) against a computer opponent controlling black (zombies). The full product targets iOS and Android and features three game modes (Survive, Escape, Eliminate), puzzles, a survival mode, XP/level progression, 3 lives with rewind, and a heavily animated black-and-white aesthetic inspired by the look of Unity and Blender renders.

The full product is too large for a single design. Work is decomposed into four phases; each phase gets its own spec, implementation plan, and build cycle:

| Phase | Contents |
|---|---|
| **1 — Core** *(this spec)* | Unity project skeleton, chess rules engine, one AI, board + piece rendering, Eliminate mode, visual theme foundation |
| 2 — Polish & animations | Zombie/human attack animations, loading screen, sound, UI shell |
| 3 — More modes | Survive, Escape, Puzzles, Survival mode, difficulty curve, 3 lives + rewind |
| 4 — Accounts & progression | Email login, XP/levels, treasure chests, cloud save, iOS + Android build pipelines |

## 2. Phase 1 goals

A playable, testable skeleton that proves out the engine, architecture, and visual foundation end-to-end before investing in polish or additional modes.

**In scope:**
- Unity 2D project at `/Users/dakotabrown/Projects/chess-world` using the Universal Render Pipeline (URP).
- Chess rules engine with all six piece types, including castling, en passant, and pawn promotion.
- Eliminate-mode win/loss rules: king is a capturable piece; a side loses when it has no pieces remaining.
- Minimax AI with alpha-beta pruning at depth 3 and material-only evaluation.
- Single playable scene: board rendering, piece placement, tap-to-select + tap-to-move with legal-move highlighting, full turn flow, capture handling.
- Placeholder-friendly sprite pipeline: code consumes Blender-rendered sprite sheets via a `SpriteCatalog` ScriptableObject; art can be dropped in without code changes.
- Minimal "feels alive" animation: piece slide on move, fade-out on capture, glow on the selected square.
- Android emulator slash command (`/chess-android`) that builds and launches the game.

**Out of scope (deferred to later phases):**
- Authentication, XP, levels, treasure chests.
- Survive, Escape, Puzzles, Survival modes.
- 3 lives, rewind.
- Animated loading screen, full zombie/human attack animations, sound design.
- iOS build pipeline, cloud save.

## 3. Architecture

One Unity project, two assembly definitions. Enforces a one-way dependency: the Unity layer may reference the Core engine, never the reverse.

```
chess-world/
├── Assets/
│   ├── Core/               ChessWorld.Core.asmdef (plain C#, no UnityEngine)
│   ├── Game/               ChessWorld.Game.asmdef (references Core + Unity)
│   ├── Editor/             BuildScripts for CLI Android build
│   ├── Art/                Blender sprite sheets drop here
│   ├── Scenes/             Main.unity
│   └── Tests/
│       ├── EditMode/       Core unit tests (no Unity runtime)
│       └── PlayMode/       Game-layer integration tests
├── Packages/
├── ProjectSettings/
└── docs/
```

This separation is the single most important architectural choice. The rules engine is notoriously easy to get *almost* right and have subtle bugs; keeping it Unity-free means every edge case can be pinned down with `dotnet test` in seconds without booting the editor.

### 3.1 `ChessWorld.Core` (plain C#)

**Domain types — immutable where possible:**

| Type | Shape |
|---|---|
| `PieceType` | enum: King, Queen, Rook, Bishop, Knight, Pawn |
| `Side` | enum: White, Black |
| `Square` | readonly struct `(File: 0..7, Rank: 0..7)` |
| `Piece` | readonly struct `(Side, PieceType, HasMoved)` |
| `Move` | readonly struct `(From, To, MoveKind, PromotionTo?)` where `MoveKind` = Normal \| Capture \| Castle \| EnPassant \| Promotion |
| `Board` | 64-element `Piece?[]` + side-to-move + en-passant target + castling rights; `Board.Apply(Move) → Board` returns a new instance |
| `GameResult` | enum: InProgress, WhiteWins, BlackWins, Draw |

**Modules (one static class per file):**

- `MoveGenerator.cs` — `IEnumerable<Move> LegalMoves(Board, Side)`. Split per piece (`Moves.Pawn.cs`, `Moves.Knight.cs`, …) to keep each file well under 500 lines.
- `Rules.cs` — `Board Apply(Board, Move)`, `GameResult Evaluate(Board)` for Eliminate.
- `Evaluation.cs` — material score: K = effectively ∞, Q = 9, R = 5, B = 3, N = 3, P = 1.
- `MinimaxAi.cs` — alpha-beta search, configurable depth (default 3), returns best `Move`.
- `GameEngine.cs` — façade consumed by the Unity layer:
  - Methods: `StartNewGame()`, `TryPlayerMove(from, to)`, `RequestAiMove()`.
  - Events: `MovePlayed(Move, Board)`, `GameEnded(GameResult)`.
  - Raises standard C# `event Action<T>`, not `UnityEvent`.

### 3.2 `ChessWorld.Game` (Unity layer)

**Single scene `Main.unity`:**

```
Main
├── Camera (orthographic, fit-to-board, safe-area aware)
├── BoardRoot
│   ├── BoardBackground   (zombie-themed rectangle sprite)
│   ├── Squares           (8×8 grid of quads for highlights)
│   └── Pieces            (dynamically spawned PieceView GameObjects)
├── UICanvas
│   ├── TurnIndicator
│   ├── CapturedTray      (white captures left, black captures right)
│   └── GameOverPanel     (hidden until end)
└── GameRoot              (hosts GameController)
```

**MonoBehaviours (each < 500 lines, target < 200):**

| Script | Responsibility |
|---|---|
| `GameController.cs` | Owns a `GameEngine`, subscribes to its events, drives turn flow. The only script that touches `Core` directly. |
| `BoardView.cs` | Spawns squares, converts world-position ↔ `Square`, handles selection + legal-move highlights. |
| `PieceView.cs` | One per piece. Renders sprite; exposes `AnimateMoveTo(Vector3)`, `AnimateCapture()`. |
| `InputHandler.cs` | Raycasts taps to squares, emits `SquareTapped(Square)`. Stateless. |
| `SelectionController.cs` | Tap state machine: no-selection → selected → move-attempted. Queries `GameEngine` for legal moves, drives highlights, submits approved moves. |
| `SpriteCatalog.cs` | `ScriptableObject` mapping `(Side, PieceType) → Sprite / AnimationClip`. Blender art drops in with no code change. |
| `CameraFit.cs` | One-time orthographic sizing + Android safe-area handling. |

**Turn flow:**
1. `SelectionController` emits a legal `Move` → `GameController.TryPlayerMove(move)`.
2. `GameEngine` validates, applies, fires `MovePlayed` → `BoardView` animates the slide + any capture.
3. When the animation finishes, `GameController` calls `GameEngine.RequestAiMove()` on a background thread (`Task.Run`) so the UI doesn't stall during minimax.
4. AI returns a `Move` → marshalled back to the main thread via a `UnityMainThreadDispatcher` helper → animate → resume player input.
5. After each move, `GameEngine.Result` is checked; if terminal, `GameOverPanel` is shown.

**Input edge cases handled in Phase 1:**
- Tap your own piece while one is selected → reselect.
- Tap the same piece twice → deselect.
- Tap an illegal square → deselect, no move.
- Tap during AI turn → ignored.

### 3.3 Animation (Phase 1, intentionally minimal)

- **Piece slide:** hand-rolled coroutine tweens `transform.position` over ~0.25s with easing (no external tween dependency).
- **Capture:** the captured `PieceView` fades alpha to 0 over ~0.3s, then is destroyed. Phase 2 replaces this with full zombie/human attack sequences.
- **Highlight:** selected square gently glows (sine-wave alpha on a child sprite).

## 4. Data flow

```
Input → SelectionController → GameController → GameEngine (Core)
                                                     │
                                                     ▼
                                      MoveGenerator, Rules, MinimaxAi
                                                     │
                                                     ▼
                              MovePlayed / GameEnded events (Core)
                                                     │
                                                     ▼
                             GameController → BoardView / PieceView / UI
```

All game state lives in `Core`'s immutable `Board`. The Unity layer never mutates it — it only renders what the engine reports.

## 5. Error handling

Deliberately thin for Phase 1 — fail loudly, do not swallow.

- `Core` throws `InvalidMoveException` on illegal moves; `GameController` is the only caller that catches, and it simply logs + ignores.
- Missing sprite in `SpriteCatalog` → `Debug.LogError` + magenta placeholder square. No silent fallback.
- AI hitting an unexpected state throws — bug surface, don't hide.
- `BuildScripts` exits with non-zero code on failure so the slash command surfaces errors.

No retry logic and no user-facing error UI in Phase 1. Real error handling lands with the backend in Phase 4.

## 6. Testing strategy

**6.1 Core unit tests (`Assets/Tests/EditMode/`)** — run via `dotnet test` in seconds without Unity.
- `MoveGeneratorTests` — one test per piece type: starting moves, blocked moves, captures, edge-of-board. Parameterized with `[TestCase]`.
- `SpecialMovesTests` — castling (both sides, blocked), en passant window, promotion.
- `PerftTests` — depth-3 perft from the standard starting position plus two published tactical positions. Counts must match published values. The single most valuable test in the suite.
- `RulesTests` — Eliminate win/loss detection (one side has zero pieces).
- `MinimaxTests` — one-move obvious-capture setups; AI must find the best capture. Sanity checks, not strength benchmarks.

**6.2 Game-layer play-mode tests (`Assets/Tests/PlayMode/`)** — run in Unity, slower.
- `SelectionControllerTests` — tap state machine against a stub `GameEngine`.
- `BoardViewTests` — square ↔ world coordinate round-trip; highlight on/off.
- Animations are not tested.

**6.3 Manual smoke test** — documented in `docs/smoke-test.md`: run slash command, play 5 moves, force a capture, let the AI capture you, verify the game-over panel. ~2 minutes. Required before any commit is considered done.

**Test-first discipline for Phase 1:** move-generator + perft tests must exist and pass **before** `MinimaxAi` is written. The AI's correctness depends entirely on the move generator; flipping the order hides rules bugs inside AI bugs.

## 7. Android simulator slash command

User-level command at `~/.claude/commands/chess-android.md`. When invoked, executes:

1. `adb devices` — if no emulator is running, start an AVD in the background. AVD name is read from the `CHESS_WORLD_AVD` environment variable; if unset, the command lists available AVDs (`emulator -list-avds`) and uses the first one, failing with a helpful message if none exist.
2. `Unity -batchmode -quit -projectPath /Users/dakotabrown/Projects/chess-world -executeMethod BuildScripts.BuildAndroid -logFile -` — build APK.
3. `adb install -r build/android/ChessWorld.apk` — install.
4. `adb shell am start -n com.chessworld.app/com.unity3d.player.UnityPlayerActivity` — launch.
5. `adb logcat -s Unity ChessWorld:*` — tail logs.

`Assets/Editor/BuildScripts.cs` (~50 lines) wraps `BuildPipeline.BuildPlayer` so the command line can invoke it.

**Android package identifier:** `com.chessworld.app` for Phase 1. This is a placeholder — if the real App Store / Play Store account requires a different bundle identifier, it gets changed in Phase 4 alongside the release pipeline. Changing it is a single `ProjectSettings` edit.

**Prerequisites (command fails loudly with a helpful message if missing):**
- Unity Hub + Unity LTS with Android Build Support module.
- Android SDK + at least one AVD configured.
- `JAVA_HOME`, `ANDROID_HOME`, and `UNITY_PATH` environment variables set.
- Optional: `CHESS_WORLD_AVD` to pin a specific emulator.

## 8. File-length discipline

Every source file in this project must stay under 500 lines. The architecture is designed to make this easy rather than a constant pruning chore:

- `MoveGenerator` is split per piece type across multiple partial-class files.
- MonoBehaviours each own one responsibility and communicate by event.
- Shared helpers (coordinate math, color palette, constants) live in small focused files.

If a file approaches 400 lines during implementation, that's the signal to split before it grows further.

## 9. Key design decisions (with rationale)

| Decision | Chosen | Rationale |
|---|---|---|
| Engine | Unity 2D + URP | Mobile-ready, excellent animation tools, iOS + Android targets, URP gives the Unity/Blender look without going 3D. |
| Architecture | Core/Game split via `.asmdef` | Makes rules engine testable without Unity; enforces one-way dependency. |
| Piece art source | Blender-rendered sprite sheets | Matches the visual-inspiration goal; pipeline tolerates delayed art via `SpriteCatalog`. |
| Interaction | Tap-to-select, tap-to-move | Mobile-standard, no piece-under-finger occlusion. |
| Eliminate rules | King is capturable; loss = zero pieces | Fits the zombie-hunting theme, simpler than checkmate logic. |
| AI | Minimax + alpha-beta, depth 3, material eval | Plays a decent beginner on-device, ~200–400 lines, trivially extensible for later difficulty levels. |

## 10. Acceptance criteria for Phase 1

Phase 1 is complete when all of the following hold:

- [ ] `dotnet test` on Core passes, including perft at depth 3 from the starting position.
- [ ] Play-mode tests pass in Unity.
- [ ] `/chess-android` builds, installs, and launches the game on a running Android emulator.
- [ ] A full game of Eliminate can be played on the emulator: select pieces, see legal-move highlights, make moves, see AI respond, capture pieces, end the game (either side wins).
- [ ] No source file exceeds 500 lines.
- [ ] Smoke test in `docs/smoke-test.md` passes.
