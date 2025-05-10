using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ChessSharp;
using UnityEngine.UI;
using VoiceChess.MoveFigureManager;
using VoiceChess.SpeechRecognition;
using VoiceChess.Example.Manager;
using UnityEditor.PackageManager;

namespace VoiceChess.Example.UI
{

    public class UIUpdate : MonoBehaviour
    {
        public FigureMoveManager FigureMoveManager;
        public GameObject SecondaryWindow;
        public GameObject EndGameWindow;
        public GameObject PromotionWindow;
        public TextMeshProUGUI WhoseTurnTitle;
        public TextMeshProUGUI HistoryField;
        public TextMeshProUGUI ResultOfRecordingField;
        public TextMeshProUGUI WindowTitle;
        public TextMeshProUGUI PawnPromotionValue;
        public Button CloseWindowButton;
        public Button StartRecordingButton;
        public Button StopRecordingButton;
        [HideInInspector]
        public bool IsWindowOpen = false;

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
            if (SpeechToText.IsGetRequest)
            {
                WriteRecordingResults(SpeechToText.RecognizedText);
            }

            CheckSecondaryWindow();

            FinalWindow();

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

        public void CheckSecondaryWindow()
        {
            if (SecondaryWindow.activeInHierarchy)
            {
                IsWindowOpen = true;
            }
            else
            {
                IsWindowOpen = false;
            }
        }

        public void UpdateHistoryText(string newValue)
        {
            HistoryField.text = newValue + "\n" + HistoryField.text;
        }

        public void WriteRecordingResults(string result)
        {
            ResultOfRecordingField.text = result;
        }

        public void PromotionPawnWindow()
        {
            SecondaryWindow.SetActive(true);
            PromotionWindow.SetActive(true);
            CloseWindowButton.gameObject.SetActive(false);
            WindowTitle.text = "Pawn on second-to-last field\nChoose promotion";
        }

        private void FinalWindow()
        {
            try
            {
                string gameState = GameManager.MoveManager.UpdateGameState();
                if (gameState == GameState.BlackWinner.ToString() || gameState == GameState.WhiteWinner.ToString())
                {
                    SecondaryWindow.SetActive(true);
                    WindowTitle.text = gameState;
                    CloseWindowButton.gameObject.SetActive(false);
                    EndGameWindow.gameObject.SetActive(true);
                }
            }
            catch { }
        }

        private void CurrentPlayer()
        {
            try
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
            catch {}
        }

    }
}
