using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ChessSharp;
using UnityEngine.UI;
using VoiceChess.MoveFigureManager;
using VoiceChess.SpeechRecognition;
using VoiceChess.Example.Manager;

namespace VoiceChess.Example.UI
{

    public class UIUpdate : MonoBehaviour
    {
        public FigureMoveManager FigureMoveManager;
        public TextMeshProUGUI WhoseTurnTitle;
        public TextMeshProUGUI ResultOfRecordingField;
        public Button StartRecordingButton;
        public Button StopRecordingButton;

        private void Awake()
        {
            StartRecordingButton.onClick.AddListener(SpeechToText.StartRecording);
            StopRecordingButton.onClick.AddListener(SpeechToText.StopRecording);
        }

        private void Start()
        {
            WhoseTurnTitle.text = Player.White.ToString();
        }

        private void Update()
        {
            

            CurrentPlayer();

            if (SpeechToText.IsGetRequest)
            {
                StartRecordingButton.interactable = true;
            }
            else
            {
                StartRecordingButton.interactable = false;
            }
        }

        public static string UpdateHistoryText(string currentValue, string newValue)
        {
            return newValue + "\n" + currentValue;
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
