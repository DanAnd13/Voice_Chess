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
            // ���������� ���� �����
            _testBoard = new GameBoard();
        }

        private void Start()
        {
            BlackInCheck();
            //Fight();
        }

        private void BlackInCheck()
        {
            FigureMoveManager.IsMoveAvailable("Pawn",null, "C4");
            FigureMoveManager.IsMoveAvailable("Pawn", null, "D5");
            FigureMoveManager.IsMoveAvailable("Queen", null, "A4");
            UpdateGameState();
        }

        private void Fight()
        {
            FigureMoveManager.IsMoveAvailable("Pawn", null, "E4");
            FigureMoveManager.IsMoveAvailable("Pawn", null, "D5");
            FigureMoveManager.IsMoveAvailable("Pawn", null, "D5");
        }

        // ����� ��� ��������� ����� ��� �� ����������� ����������
        private void UpdateGameState()
        { 
            _testBoard = FigureMoveManager.GetGameBoard();
            GameState currentState = _testBoard.GameState;
            Debug.Log($"Current game state: {currentState}");
        }
    }
}
