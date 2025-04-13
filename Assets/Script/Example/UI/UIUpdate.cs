using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using ChessSharp;
using VoiceChess.MoveFigureManager;

namespace VoiceChess.Example.UI
{

    public class UIUpdate : MonoBehaviour
    {
        public FigureMoveManager FigureMoveManager;
        public TextMeshProUGUI WhoseTurnTitle;

        private void Start()
        {
            WhoseTurnTitle.text = Player.White.ToString();
        }

        private void Update()
        {
            CurrentPlayer();
        }

        public static void UpdateHistoryText(string currentValue, string newValue)
        {
            currentValue = currentValue + "\n" + newValue;
        }

        private void CurrentPlayer()
        {
            Player currentPlayer = FigureMoveManager.Board.WhoseTurn();
            if (currentPlayer == Player.White)
            {
                WhoseTurnTitle.color = Color.white;
                WhoseTurnTitle.text = Player.White.ToString();
            }
            else
            {
                WhoseTurnTitle.color = Color.black;
                WhoseTurnTitle.text = Player.Black.ToString();
            }
        }


    }
}
