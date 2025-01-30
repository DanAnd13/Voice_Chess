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
                // ���������� ������� ������� `newPosition` � ��'��� Square
                Square destinationSquare = Square.Parse(newPosition);

                // ������ ������ �� �� ��'��
                foreach (var figure in Figures)
                {
                    if (figure.Type.ToString() == figureName)
                    {
                        // ���������� ������� ������� ������ � Square
                        Square currentSquare = Square.Parse(figure.CurrentPosition);

                        // ��������� ��'��� Move
                        Move move = new Move(currentSquare, destinationSquare, _board.WhoseTurn());

                        // ���������� �������� ����
                        if (_board.IsValidMove(move))
                        {
                            // �������� ���
                            if (_board.MakeMove(move, isMoveValidated: true))
                            {
                                // ��������� ������� ������
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

            // ������ ��� ������� ����� 8x8
            for (int rank = 0; rank < 8; rank++)
            {
                string row = "";
                for (int file = 0; file < 8; file++)
                {
                    Piece piece = _board.Board[file, rank];
                    if (piece == null)
                    {
                        row += "[ ] ";  // ������� �������
                    }
                    else
                    {
                        row += $"[{piece}] ";  // ��� ������ � �������
                    }
                }
                Debug.Log(row); // �������� ����� ��� ������� ��
            }
        }
    }
}
