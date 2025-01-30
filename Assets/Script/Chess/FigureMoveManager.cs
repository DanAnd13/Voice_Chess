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
        private Player _whitePlayer;
        private Player _blackPlayer;

        private Pawn _pawn;
        private Rook _rook;
        private Knight _knight;
        private Bishop _bishop;
        private Queen _queen;
        private King _king;

        private void Awake()
        {
            _board = new GameBoard();
            _whitePlayer = new Player();
            _blackPlayer = new Player();
        }

        public bool MoveFigure(string figureName, string newPosition)
        {
            bool moveSuccessful = false;

            try
            {
                // Конвертуємо рядкову позицію `newPosition` у об'єкт Square
                Square destinationSquare = Square.Parse(newPosition);

                // Шукаємо фігуру за її ім'ям
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
                            if (_board.MakeMove(move, isMoveValidated: true))
                            {
                                // Оновлюємо позицію фігури
                                figure.PreviousPosition = figure.CurrentPosition;
                                figure.CurrentPosition = newPosition;

                                Debug.Log($"{figure.Type} moved from {figure.PreviousPosition} to {figure.CurrentPosition}");
                                moveSuccessful = true;
                                
                                PrintBoard();
                                break;
                            }
                            else
                            {
                                Debug.Log($"Failed to execute move for {figure.Type} from {figure.CurrentPosition} to {newPosition}.");
                            }
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

            if (!moveSuccessful)
            {
                Debug.LogError($"No valid moves found for figure {figureName} to position {newPosition}.");
            }

            return moveSuccessful;
        }
        private void PrintBoard()
        {
            Debug.Log("Current Board State:");

            // Перебір всіх клітинок дошки 8x8
            for (int rank = 0; rank < 8; rank++)
            {
                string row = "";
                for (int file = 0; file < 8; file++)
                {
                    Piece piece = _board.Board[file, rank];
                    if (piece == null)
                    {
                        row += "[ ] ";  // Порожня клітинка
                    }
                    else
                    {
                        row += $"[{piece}] ";  // Тип фігури в клітинці
                    }
                }
                Debug.Log(row); // Виводимо рядок для поточної лінії
            }
        }
    }
}
