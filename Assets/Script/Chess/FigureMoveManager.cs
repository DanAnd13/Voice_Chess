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

        [HideInInspector]
        public GameBoard Board;

        private bool _moveSuccessful = false;
        private string _lastMoveResult = "";

        private void Awake()
        {
            Board = new GameBoard();
        }

        public string GetLastMoveResult()
        {
            return _lastMoveResult;
        }

        private GameBoard GetGameBoard()
        {
            return Board;
        }
        public string UpdateGameState()
        {
            string currentGameState;
            Board = GetGameBoard();
            GameState currentState = Board.GameState;
            currentGameState = currentState.ToString();
            return currentGameState;
        }

        public bool IsMoveAvailable(string? figureName, string? currentPosition, string newPosition)
        {
            _moveSuccessful = false;

            try
            {
                if (string.IsNullOrWhiteSpace(newPosition))
                {
                    Debug.LogError("Error: New position is empty or null.");
                    return false;
                }

                Square destinationSquare = Square.Parse(newPosition);

                foreach (var figure in Figures)
                {
                    bool matchByName = !string.IsNullOrWhiteSpace(figureName) && figure.Type.ToString() == figureName;
                    bool matchByPosition = !string.IsNullOrWhiteSpace(currentPosition) && figure.CurrentPosition == currentPosition;

                    if (matchByName || matchByPosition)
                    {
                        if (CreateMoveAtributes(destinationSquare, figure, newPosition))
                        {
                            //Debug.Log(_lastMoveResult);
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

            return _moveSuccessful;
        }

        private bool CreateMoveAtributes(Square destinationSquare, FigureParams figure, string newPosition)
        {
            Square currentSquare = Square.Parse(figure.CurrentPosition);

            Move move = new Move(currentSquare, destinationSquare, Board.WhoseTurn());

            if (Board.IsValidMove(move))
            {
                MakeMove(move, figure, newPosition);
                _moveSuccessful = true;
                return true;
            }
            return false;
        }

        private void MakeMove(Move move, FigureParams figure, string newPosition)
        {
            if (Board.MakeMove(move, isMoveValidated: true))
            {
                // Оновлюємо позицію фігури
                figure.PreviousPosition = figure.CurrentPosition;
                figure.CurrentPosition = newPosition;

                _lastMoveResult = $"{figure.Type} moved from {figure.PreviousPosition} to {figure.CurrentPosition}";
            }
            else
            {
                _lastMoveResult = $"Failed to execute move for {figure.Type} from {figure.CurrentPosition} to {newPosition}.";
            }
        }
    }
}
