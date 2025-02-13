using ChessSharp.Pieces;
using ChessSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using VoiceChess.MoveFigureManager;

namespace VoiceChess.ChessAnalyzer
{
    public class ChessGameAnalyzer : MonoBehaviour
    {
        public FigureMoveManager FigureMoveManager;
        
        private GameBoard _testBoard;

        private void Awake()
        {
            // Ініціалізуємо нову дошку
            _testBoard = new GameBoard();
        }

        private void Start()
        {
            WhiteWinner();
        }

        private void WhiteWinner()
        {
            FigureMoveManager.IsMoveAvailable("Pawn", "C4");
            FigureMoveManager.IsMoveAvailable("Pawn", "D5");
            FigureMoveManager.IsMoveAvailable("Queen", "A4");
            UpdateGameState();
        }

        // Метод для оновлення стану гри та відображення результату
        private void UpdateGameState()
        { 
            _testBoard = FigureMoveManager.GetGameBoard();
            GameState currentState = _testBoard.GameState;
            Debug.Log($"Current game state: {currentState}");
        }
    }
}
