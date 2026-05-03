using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ChessWorld.Core;
using ChessWorld.Game.UI;

namespace ChessWorld.Game
{
    public sealed class GameController : MonoBehaviour
    {
        public SpriteCatalog Catalog;
        public BoardView Board;
        public BackgroundView Background;
        public InputHandler Input;
        public SelectionController Selection;
        public TurnIndicator TurnIndicator;
        public CapturedTray WhiteCaptures;  // zombies captured BY white
        public CapturedTray BlackCaptures;  // humans captured BY black
        public GameOverPanel GameOverPanel;
        public Transform PiecesRoot;

        public int AiDepth = 3;

        private GameEngine _engine;
        private readonly Dictionary<Square, PieceView> _views = new Dictionary<Square, PieceView>();
        private bool _aiPending;

        private void Start()
        {
            _engine = new GameEngine(AiDepth);
            _engine.MovePlayed += OnMovePlayed;
            _engine.GameEnded += OnGameEnded;

            Selection.Engine = _engine;
            Selection.Board = Board;
            Input.SquareTapped += Selection.OnSquareTapped;
            Selection.MoveRequested += OnPlayerMoveRequested;

            GameOverPanel.OnRestart = RestartGame;

            StartNewGame();
        }

        private void StartNewGame()
        {
            GameOverPanel.Hide();
            WhiteCaptures.Clear();
            BlackCaptures.Clear();
            ClearAllViews();

            _engine.StartNewGame(BuildInitialPosition());
            SpawnViewsFromBoard();
            TurnIndicator?.SetTurn(_engine.Board.SideToMove);
        }

        // Chess World Phase 1 setup: black's only pieces are pawns (rank 7),
        // white's only pieces are the two rooks (a1, h1). Eliminate mode rules
        // mean the game ends when one side has zero pieces remaining.
        // Fully qualified ChessWorld.Core.Board to avoid collision with the BoardView
        // field also named "Board" on this MonoBehaviour.
        private static ChessWorld.Core.Board BuildInitialPosition()
        {
            var pieces = new Piece[64];
            for (int file = 0; file < 8; file++)
                pieces[new Square(file, 6).Index] = new Piece(Side.Black, PieceType.Pawn);
            pieces[new Square(0, 0).Index] = new Piece(Side.White, PieceType.Rook);
            pieces[new Square(7, 0).Index] = new Piece(Side.White, PieceType.Rook);
            return ChessWorld.Core.Board.Empty()
                .With(pieces, Side.White, CastlingRights.None, null, 0, 1);
        }

        private void RestartGame() => StartNewGame();

        private void OnPlayerMoveRequested(Move m)
        {
            // MovePlayed fires synchronously inside ApplyMove and drives animations.
            _engine.ApplyMove(m);
        }

        private void OnMovePlayed(Move move, Board newBoard)
        {
            StartCoroutine(AnimateAndMaybeRunAi(move, newBoard));
        }

        private IEnumerator AnimateAndMaybeRunAi(Move move, Board newBoard)
        {
            Input.Enabled = false;
            yield return StartCoroutine(AnimateMove(move, newBoard));
            TurnIndicator?.SetTurn(_engine.Board.SideToMove);

            if (_engine.Result != GameResult.InProgress) yield break;

            if (_engine.Board.SideToMove == Side.Black)
            {
                _aiPending = true;
                // Hold "Zombies thinking…" visible briefly so the user sees the turn change
                // before the AI's animated response starts.
                yield return new WaitForSeconds(0.45f);
                _ = Task.Run(() =>
                {
                    var aiMove = _engine.ComputeAiMove();
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        _aiPending = false;
                        if (aiMove.HasValue) _engine.ApplyMove(aiMove.Value);
                    });
                });
            }
            else
            {
                Input.Enabled = true;
            }
        }

        private IEnumerator AnimateMove(Move move, Board newBoard)
        {
            // Handle the capture victim first (normal, en passant).
            var victimSq = VictimSquareOf(move);
            if (victimSq.HasValue && _views.TryGetValue(victimSq.Value, out var victim))
            {
                AppendCaptureForVictim(victim);
                _views.Remove(victimSq.Value);
                yield return StartCoroutine(victim.AnimateCaptureAndDestroy());
            }

            if (!_views.TryGetValue(move.From, out var mover))
            {
                Debug.LogWarning($"No view at {move.From} for {move}");
                yield break;
            }

            _views.Remove(move.From);
            yield return StartCoroutine(mover.AnimateMoveTo(Board.WorldOf(move.To), move.To));

            // Promotion: swap sprite to promoted piece
            if (move.Kind == MoveKind.Promotion || move.Kind == MoveKind.PromotionCapture)
            {
                mover.Configure(mover.Side, move.PromotionTo, move.To,
                    Catalog.GetPiece(mover.Side, move.PromotionTo));
            }
            _views[move.To] = mover;

            // Castling: slide the rook to its post-castle square
            if (move.Kind == MoveKind.CastleKingSide)
                yield return MoveViewNoEvent(new Square(7, move.From.Rank),
                                             new Square(5, move.From.Rank));
            else if (move.Kind == MoveKind.CastleQueenSide)
                yield return MoveViewNoEvent(new Square(0, move.From.Rank),
                                             new Square(3, move.From.Rank));
        }

        private IEnumerator MoveViewNoEvent(Square from, Square to)
        {
            if (!_views.TryGetValue(from, out var v)) yield break;
            _views.Remove(from);
            yield return StartCoroutine(v.AnimateMoveTo(Board.WorldOf(to), to));
            _views[to] = v;
        }

        private Square? VictimSquareOf(Move m)
        {
            if (m.Kind == MoveKind.Capture || m.Kind == MoveKind.PromotionCapture) return m.To;
            if (m.Kind == MoveKind.EnPassant)
            {
                int r = m.From.Rank;  // capturing side's pawn row
                return new Square(m.To.File, r);
            }
            return null;
        }

        private void AppendCaptureForVictim(PieceView v)
        {
            var piece = new Piece(v.Side, v.Type);
            if (piece.Side == Side.Black) WhiteCaptures?.AppendCapture(piece);
            else BlackCaptures?.AppendCapture(piece);
        }

        private void OnGameEnded(GameResult result)
        {
            Input.Enabled = false;
            GameOverPanel.Show(result);
        }

        private void SpawnViewsFromBoard()
        {
            for (int i = 0; i < 64; i++)
            {
                var sq = Square.FromIndex(i);
                var p = _engine.Board[sq];
                if (p.IsNone) continue;
                SpawnPieceView(sq, p);
            }
        }

        private void SpawnPieceView(Square sq, Piece p)
        {
            var go = new GameObject($"Pc_{sq.ToAlgebraic()}_{p.Side}_{p.Type}");
            go.transform.SetParent(PiecesRoot, false);
            go.transform.localPosition = Board.WorldOf(sq);
            var view = go.AddComponent<PieceView>();
            view.Configure(p.Side, p.Type, sq, Catalog.GetPiece(p.Side, p.Type));
            _views[sq] = view;
        }

        private void ClearAllViews()
        {
            foreach (var v in _views.Values) if (v != null) Destroy(v.gameObject);
            _views.Clear();
        }
    }
}
