using System.Collections.Generic;
using UnityEngine;
using ChessSharp;
using VoiceChess.FigureParameters;
using ChessSharp.Pieces;
using ChessSharp.SquareData;
using System;
using System.Linq;

namespace VoiceChess.MoveFigureManager
{
    public class FigureMoveManager : MonoBehaviour
    {
        public FigureParams[] Figures;

        private GameBoard _board;
        private bool _moveSuccessful = false;

        private void Awake()
        {
            _board = new GameBoard();
        }

        public GameBoard GetGameBoard()
        {
            return _board;
        }

        public bool IsMoveAvailable(string? figureName, string? currentPosition, string newPosition)
        {
            _moveSuccessful = false;

            try
            {
                Square destinationSquare = Square.Parse(newPosition);

                foreach (var figure in Figures)
                {
                    if ((figureName != null && currentPosition == null) || (figureName != null && currentPosition != null))
                    {
                        if (figure.Type.ToString() == figureName)
                        {
                            if (CreateMoveAtributes(destinationSquare, figure, newPosition))
                            {
                                return true;
                            }
                        }
                    }
                    else if (figureName == null && currentPosition != null)
                    {
                        if (figure.CurrentPosition == currentPosition)
                        {
                            if (CreateMoveAtributes(destinationSquare, figure, newPosition))
                            {
                                return true;
                            }
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

            return _moveSuccessful;
        }

        private bool CreateMoveAtributes(Square destinationSquare, FigureParams figure, string newPosition)
        {
            Square currentSquare = Square.Parse(figure.CurrentPosition);

            Move move = new Move(currentSquare, destinationSquare, _board.WhoseTurn());

            if (_board.IsValidMove(move))
            {
                MakeMove(move, figure, newPosition);
                _moveSuccessful = true;
                return true;
            }
            return false;
        }


        private void MakeMove(Move move, FigureParams figure, string newPosition)
        {
            if (_board.MakeMove(move, isMoveValidated: true))
            {
                // Оновлюємо позицію фігури
                figure.PreviousPosition = figure.CurrentPosition;
                figure.CurrentPosition = newPosition;

                Debug.Log($"{figure.Type} moved from {figure.PreviousPosition} to {figure.CurrentPosition}");

                PrintBoard();
            }
            else
            {
                Debug.Log($"Failed to execute move for {figure.Type} from {figure.CurrentPosition} to {newPosition}.");
            }
        }

        public void PrintBoard()
        {
            Debug.Log("Current Board State:");

            for (int rank = 0; rank < 8; rank++)
            {
                string row = "";
                for (int file = 0; file < 8; file++)
                {
                    Piece piece = _board.Board[file, rank];
                    if (piece == null)
                    {
                        row += "[ ] ";
                    }
                    else
                    {
                        row += $"[{piece}] ";
                    }
                }
                Debug.Log(row);
            }
        }
    }
}
