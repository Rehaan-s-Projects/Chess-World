# Chess World

2D mobile chess game with a zombie-vs-humans theme. Player plays white (humans); computer plays black (zombies).

**Status:** Phase 1 (Core) — see `docs/superpowers/specs/2026-04-21-chess-world-phase-1-design.md`.

## Quick start

1. Install Unity LTS + Android Build Support via Unity Hub.
2. Set env vars: `UNITY_PATH`, `ANDROID_HOME`, `JAVA_HOME`.
3. Open this folder in Unity Hub → Add → select `chess-world/`.
4. In Unity, open `Assets/Scenes/Main.unity` and press Play.

## Running Core tests (fast, no Unity needed)

```bash
dotnet test tests/Core/ChessWorld.Core.Tests.csproj
```

## Building and running on Android emulator

In Claude Code, type `/chess-android`.

## Credits

Background art: Rehaan Rashid (DeviantArt: `RehaanRashid`).
