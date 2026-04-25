# Chess World — Phase 1 Smoke Test

Prereqs: Android emulator running, slash command configured, project opens without console errors.

## Steps

1. In Claude Code, type `/chess-android`. Wait for "Game is running on the emulator."
2. Observe: board renders with black/white squares, 32 pieces in starting formation, Rehaan's background (or fallback gradient) behind the board, "Your turn" label visible.
3. Tap a white pawn on the second rank. Legal-move dots appear on rank 3 and rank 4.
4. Tap rank 4. The pawn slides smoothly to the destination. "Zombies thinking…" appears.
5. Wait up to ~2 seconds. A black piece responds by moving.
6. Move a white piece to capture a black piece. The black piece fades and disappears; its icon appears in the "White captures" tray.
7. Let the AI capture one of your pieces. Confirm it fades and appears in the "Black captures" tray.
8. Continue until one side has no pieces. The GameOverPanel appears with the correct headline and the "Background art by Rehaan Rashid" credit line.
9. Tap Restart. Board resets; captures clear; a new game begins.

## Known acceptable looseness in Phase 1

- No sounds.
- No zombie/human attack animations — only slide + fade.
- Placeholder piece art (simple silhouettes). Blender-rendered art lands in Phase 2.
- AI takes ~0.5–2s per move at depth 3 (acceptable).

## If any step fails

Capture the adb logcat output and file a bug in `docs/bugs.md`. Do NOT declare Phase 1 done.
