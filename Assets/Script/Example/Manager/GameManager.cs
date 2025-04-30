using System.Collections.Generic;
using UnityEngine;
using VoiceChess.FigureParameters;
using VoiceChess.MoveFigureManager;
using VoiceChess.Example.PaintingCells;
using VoiceChess.Example.CameraMoves;
using VoiceChess.Example.FigureMoves;
using VoiceChess.Example.UI;
using ChessSharp;
using ChessSharp.SquareData;
using VoiceChess.BoardCellsParameters;
using TMPro;
using VoiceChess.SpeechRecognition;
using VoiceChess.Speaking;
using System.Linq;
using System;
using System.Collections;

namespace VoiceChess.Example.Manager
{

    public class GameManager : MonoBehaviour
    {
        public FigureMoveManager FigureMoveManager;
        public Transform ParentBoard;
        public Transform WhiteCapturedArea; // Позиція для вибитих чорних фігур
        public Transform BlackCapturedArea; // Позиція для вибитих білих фігур
        public UIUpdate UI;
        public AudioSource AudioPlayer;

        [HideInInspector]
        public static FigureMoveManager MoveManager;
        [HideInInspector]
        public static FigureParams SelectedFigure;
        [HideInInspector]
        public static List<BoardCellsParams> BoardCells = new List<BoardCellsParams>();
        [HideInInspector]
        public static FigureParams[] Figures;

        private void Awake()
        {
            BoardCells.Clear();

            MoveManager = FigureMoveManager;

            Figures = FigureMoveManager.Figures;

            foreach (Transform cell in ParentBoard)
            {
                BoardCells.Add(cell.gameObject.GetComponent<BoardCellsParams>());
            }
            SpeechToText.OnMoveParsed += HandleVoiceMove;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !UI.IsWindowOpen)
            {
                HandleClick();
            }
        }

        public static FigureParams GetFigureOnCell(BoardCellsParams cell)
        {
            foreach (FigureParams figure in Figures)
            {
                if (figure.Status == FigureParams.TypeOfStatus.OnGame && figure.CurrentPosition == cell.NameOfCell)
                {
                    return figure;
                }
            }
            return null;
        }

        public static bool IsItDifferentTeamByColor(FigureParams.TypeOfTeam selectedFigure, FigureParams.TypeOfTeam enemyFigure)
        {
            try
            {
                return selectedFigure != enemyFigure;
            }
            catch
            {
                return false;
            }
        }

        private void HandleClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                FigureParams clickedFigure = hit.collider.GetComponent<FigureParams>();
                BoardCellsParams clickedCell = hit.collider.GetComponent<BoardCellsParams>();

                if (clickedFigure != null)
                {
                    Player currentPlayer = FigureMoveManager.Board.WhoseTurn();
                    if (IsFigureBelongsToCurrentPlayer(clickedFigure, currentPlayer))
                    {
                        if (SelectedFigure == clickedFigure)
                        {
                            DeselectFigure();
                        }
                        else
                        {
                            SelectFigure(clickedFigure);
                        }
                    }
                    else
                    {
                            string attackedFigurePosition = clickedFigure.CurrentPosition;
                            clickedCell = BoardCells.Find(cell => cell.NameOfCell == attackedFigurePosition);
                            if (clickedCell != null)
                            {
                                MakeFigureMove(clickedCell);
                            }
                    }
                }

                else if (SelectedFigure != null && BoardCells.Contains(clickedCell))
                {
                    MakeFigureMove(clickedCell);
                }
            }
        }

        private bool IsFigureBelongsToCurrentPlayer(FigureParams figure, Player currentPlayer)
        {
            return (currentPlayer == Player.White && figure.TeamColor == FigureParams.TypeOfTeam.WhiteTeam) ||
                   (currentPlayer == Player.Black && figure.TeamColor == FigureParams.TypeOfTeam.BlackTeam);
        }

        private void SelectFigure(FigureParams figure)
        {
            DeselectFigure();

            SelectedFigure = figure;
            SelectedFigure.transform.position += Vector3.up * 0.5f;

            List<BoardCellsParams> validMoves = GetValidMoveCells(figure);
            HighlightCells.PaintCells(validMoves, isHighlight: true);
        }

        private void DeselectFigure()
        {
            if (SelectedFigure != null)
            {
                SelectedFigure.transform.position -= Vector3.up * 0.5f;
                SelectedFigure = null;
            }

            HighlightCells.PaintCells(HighlightCells.HighlightedCellsObjects, isHighlight: false);
            HighlightCells.HighlightedCellsObjects.Clear();
        }

        private List<BoardCellsParams> GetValidMoveCells(FigureParams figure)
        {
            List<BoardCellsParams> validCells = new List<BoardCellsParams>();

            foreach (BoardCellsParams cell in BoardCells)
            {
                string cellName = cell.NameOfCell;
                Square destinationSquare = Square.Parse(cellName);

                if (CheckValidMove(destinationSquare, figure, cellName))
                {
                    validCells.Add(cell);
                }
            }
            return validCells;
        }


        private bool CheckValidMove(Square destinationSquare, FigureParams figure, string newPosition)
        {
            Square currentSquare = Square.Parse(figure.CurrentPosition);
            Move move = new Move(currentSquare, destinationSquare, FigureMoveManager.Board.WhoseTurn());

            if (FigureMoveManager.Board.IsValidMove(move))
            {
                return true;
            }

            return false;
        }

        private void MakeFigureMove(BoardCellsParams targetCell)
        {

            FigureParams figureOnCell = GetFigureOnCell(targetCell);
            string newPosition;

            try
            {
                if (figureOnCell != null && IsItDifferentTeamByColor(figureOnCell.TeamColor, SelectedFigure.TeamColor))
                {
                    newPosition = figureOnCell.CurrentPosition;
                }
                else
                {
                    newPosition = targetCell.NameOfCell;
                }

                if (FigureMoveManager.IsMoveAvailable(SelectedFigure.Type.ToString(), SelectedFigure.CurrentPosition, newPosition))
                {
                    if (figureOnCell != null)
                    {
                        FigureMovement.CaptureFigure(figureOnCell, BlackCapturedArea, WhiteCapturedArea);
                    }

                    string textResult = SelectedFigure.Type.ToString() + " - " + SelectedFigure.PreviousPosition + " - " + SelectedFigure.CurrentPosition;
                    UI.UpdateHistoryText(textResult);

                    PlayAudio();

                    FigureMovement.MovingObject(newPosition, targetCell, SelectedFigure, () =>
                    {
                        UI.SecondaryWindow.SetActive(false);
                        CameraMovement.SwitchCameraPosition();
                        DeselectFigure();
                    });
                }
                else
                {
                    UI.WriteRecordingResults("Move is not posible");
                    DeselectFigure();
                }
            }
            catch { }
        }

        private void PlayAudio()
        {
            StartCoroutine(PlayAudioCoroutine());
        }

        private IEnumerator PlayAudioCoroutine()
        {
            string moveText = $"{SelectedFigure.Type} {SelectedFigure.PreviousPosition} {SelectedFigure.CurrentPosition}";
            TextToSpeech.SetTextAndSpeak(moveText, AudioPlayer);

            // Чекаємо поки закінчиться відтворення першого аудіо
            while (AudioPlayer.isPlaying)
            {
                yield return null;
            }

            if (FigureMoveManager.Board.GameState != GameState.NotCompleted)
            {
                string gameStateText = FigureMoveManager.Board.GameState.ToString();
                TextToSpeech.SetTextAndSpeak(gameStateText, AudioPlayer);
            }
        }

        private void HandleVoiceMove(FigureMoveParams move)
        {
            BoardCellsParams targetCell = BoardCells.Find(cell =>
                                          string.Equals(cell.NameOfCell, move.NewPosition, StringComparison.OrdinalIgnoreCase));

            FigureParams figureToMove = null;

            switch (move.TypeOfPattern)
            {
                case "FigureFromTo":
                    figureToMove = Figures.FirstOrDefault(f =>
                        f.Type.ToString().ToLower() == move.FigureName.ToLower() &&
                        f.CurrentPosition.Equals(move.CurrentPosition, StringComparison.OrdinalIgnoreCase));
                    break;

                case "FromTo":
                    figureToMove = Figures.FirstOrDefault(f =>
                        f.CurrentPosition.Equals(move.CurrentPosition, StringComparison.OrdinalIgnoreCase));
                    break;

                case "FigureToPos":
                    {
                        Square targetSquare = Square.Parse(move.NewPosition);
                        var candidateFigures = Figures.Where(f =>
                            f.Type.ToString().ToLower() == move.FigureName.ToLower() &&
                            IsFigureBelongsToCurrentPlayer(f,FigureMoveManager.Board.WhoseTurn()));

                        foreach (var candidate in candidateFigures)
                        {
                            Square currentSquare = Square.Parse(candidate.CurrentPosition);
                            Move potentialMove = new Move(currentSquare, targetSquare, FigureMoveManager.Board.WhoseTurn());
                            if (FigureMoveManager.Board.IsValidMove(potentialMove))
                            {
                                figureToMove = candidate;
                                break;
                            }
                        }
                        break;
                    }

                default:
                    break;
            }

            if (figureToMove != null)
            {
                SelectFigure(figureToMove);
                SelectedFigure = figureToMove;
                MakeFigureMove(targetCell);
            }
        }
    }
}