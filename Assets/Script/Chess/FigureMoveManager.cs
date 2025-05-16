using System.Collections.Generic;
using UnityEngine;
using ChessSharp;
using VoiceChess.FigureParameters;
using ChessSharp.Pieces;
using ChessSharp.SquareData;
using System;
using System.Linq;
using VoiceChess.BoardCellsParameters;
using VoiceChess.Example.FigureMoves;

namespace VoiceChess.MoveFigureManager
{
    public class FigureMoveManager : MonoBehaviour
    {
        public FigureParams[] Figures;

        [HideInInspector]
        public GameBoard Board;
        [HideInInspector]
        public bool IsCastlingMove = false;
        [HideInInspector]
        public string RookTargetPosition;

        private void Awake()
        {
            Board = new GameBoard();
        }

        public string UpdateGameState()
        {
            string currentGameState;
            GameState currentState = Board.GameState;
            currentGameState = currentState.ToString();
            return currentGameState;
        }

        public bool IsMoveAvailable(string? figureName, string? currentPosition, string newPosition, string? pawnPromotion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newPosition))
                {
                    Debug.LogError("Error: New position is empty or null.");
                    return false;
                }

                foreach (var figure in Figures)
                {
                    bool matchByName = !string.IsNullOrWhiteSpace(figureName) && figure.Type.ToString() == figureName;
                    bool matchByPosition = !string.IsNullOrWhiteSpace(currentPosition) && figure.CurrentPosition == currentPosition;

                    if ((matchByName && matchByPosition) && figure.Status == FigureParams.TypeOfStatus.OnGame)
                    {
                        if (CreateMoveAtributes(figure, newPosition, pawnPromotion))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Debug.LogError($"Invalid square format: {newPosition}. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error occurred: {ex.Message}");
            }

            return false;
        }

        private bool CreateMoveAtributes(FigureParams figure, string newPosition, string? pawnPromotion)
        {
            Square currentSquare = Square.Parse(figure.CurrentPosition);
            Square destinationSquare = Square.Parse(newPosition);
            PawnPromotion promotion = new PawnPromotion();
            if (!string.IsNullOrWhiteSpace(pawnPromotion))
            {
                switch (pawnPromotion)
                {
                    case "Knight":
                        promotion = PawnPromotion.Knight;
                        break;
                    case "Rook":
                        promotion = PawnPromotion.Rook;
                        break;
                    case "Bishop":
                        promotion = PawnPromotion.Bishop;
                        break;
                    case "Queen":
                        promotion = PawnPromotion.Queen;
                        break;
                }

                Move move = new Move(currentSquare, destinationSquare, Board.WhoseTurn(), promotion);
                if (Board.IsValidMove(move))
                {
                    MakeMove(move, figure, newPosition);
                    return true;
                }
            }
            else
            {
                Move move = new Move(currentSquare, destinationSquare, Board.WhoseTurn());
                if (Board.IsValidMove(move))
                {
                    MakeMove(move, figure, newPosition);
                    return true;
                }
            }
            return false;
        }

        private void MakeMove(Move move, FigureParams figure, string newPosition)
        {
            if (Board.MakeMove(move, isMoveValidated: true))
            {
                switch (move.PromoteTo)
                {
                    case PawnPromotion.Rook:
                        figure.Type = FigureParams.TypeOfFigure.Rook;
                        break;
                    case PawnPromotion.Knight:
                        figure.Type = FigureParams.TypeOfFigure.Knight;
                        break;
                    case PawnPromotion.Bishop:
                        figure.Type = FigureParams.TypeOfFigure.Bishop;
                        break;
                    case PawnPromotion.Queen:
                        figure.Type = FigureParams.TypeOfFigure.Queen;
                        break;
                }

                figure.PreviousPosition = figure.CurrentPosition;
                figure.CurrentPosition = newPosition;

                if (figure.Type == FigureParams.TypeOfFigure.King)
                {
                    int deltaFile = Square.Parse(newPosition).File - Square.Parse(figure.PreviousPosition).File;
                    if (Math.Abs(deltaFile) == 2) // рокіровка - хід короля на 2 клітинки
                    {
                        HandleCastlingRookMove(figure, deltaFile > 0); // права рокіровка?
                    }
                }
            }
            else
            {
                Debug.Log($"Failed to execute move for {figure.Type} from {figure.CurrentPosition} to {newPosition}.");
            }
        }

        private void HandleCastlingRookMove(FigureParams king, bool isKingside)
        {
            string rookStart, rookEnd;

            // Обираємо колір
            bool isWhite = king.TeamColor == FigureParams.TypeOfTeam.WhiteTeam;

            if (isWhite)
            {
                rookStart = isKingside ? "H1" : "A1";
                rookEnd = isKingside ? "F1" : "D1";
            }
            else
            {
                rookStart = isKingside ? "H8" : "A8";
                rookEnd = isKingside ? "F8" : "D8";
            }

            // Знаходимо туру, яка має currentPosition = rookStart
            var rookFigure = Figures.FirstOrDefault(f =>
                f.Type == FigureParams.TypeOfFigure.Rook &&
                f.CurrentPosition == rookStart &&
                f.Status == FigureParams.TypeOfStatus.OnGame
            );

            if (rookFigure != null)
            {
                rookFigure.PreviousPosition = rookFigure.CurrentPosition;
                rookFigure.CurrentPosition = rookEnd;

                IsCastlingMove = true;
                RookTargetPosition = rookEnd;
            }
        }
    }
}
