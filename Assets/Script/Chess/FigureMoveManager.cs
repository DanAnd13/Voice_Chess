using System.Collections.Generic;
using UnityEngine;
using ChessSharp;
using VoiceChess.FigureParameters;
using ChessSharp.Pieces;
using ChessSharp.SquareData;
using System;

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

        public bool IsMoveAvailable(string figureName, string newPosition)
        {
            try
            {
                // Конвертуємо рядкову позицію `newPosition` у об'єкт Square
                Square destinationSquare = Square.Parse(newPosition);

                foreach (var figure in Figures)
                {
                    if (figure.Type.ToString() == figureName)
                    {
                        // Конвертуємо поточну позицію фігури в Square
                        Square currentSquare = Square.Parse(figure.CurrentPosition);

                        // Створюємо об'єкт Move
                        Move move = new Move(currentSquare, destinationSquare, _board.WhoseTurn());

                        // Перевіряємо валідність ходу
                        if (_board.IsValidMove(move))
                        {
                            // Виконуємо хід
                            MakeMove(move, figure, newPosition);
                            break;
                        }
                        else
                        {
                           Debug.Log($"Invalid move for {figure.Type} from {figure.CurrentPosition} to {newPosition}.");
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

            if (!_moveSuccessful)
            {
                Debug.LogError($"No valid moves found for figure {figureName} to position {newPosition}.");
            }

            return _moveSuccessful;
        }

        private void MakeMove(Move move, FigureParams figure, string newPosition)
        {
            if (_board.MakeMove(move, isMoveValidated: true))
            {
                // Оновлюємо позицію фігури
                figure.PreviousPosition = figure.CurrentPosition;
                figure.CurrentPosition = newPosition;

                Debug.Log($"{figure.Type} moved from {figure.PreviousPosition} to {figure.CurrentPosition}");
                _moveSuccessful = true;

                //PrintBoard();
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
