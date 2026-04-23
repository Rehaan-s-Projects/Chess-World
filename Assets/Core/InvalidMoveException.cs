using System;

namespace ChessWorld.Core
{
    public class InvalidMoveException : Exception
    {
        public Move AttemptedMove { get; }

        public InvalidMoveException(Move move, string reason)
            : base($"Invalid move {move}: {reason}")
        {
            AttemptedMove = move;
        }
    }
}
