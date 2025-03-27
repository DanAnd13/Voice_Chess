using ChessSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VoiceChess.BoardCellsParameters;
using VoiceChess.Example.Manager;
using VoiceChess.FigureParameters;

namespace VoiceChess.Example.PaintingCells
{
    public class HighlightCells : MonoBehaviour
    {
        [HideInInspector]
        public static List<GameObject> HighlightedCellsObjects = new List<GameObject>();

        private static BoardCellsParams _boardCellsParams;

        public static void PaintCells(List<GameObject> cellsObjects, bool isHighlight)
        {
            string gameState = GameManager.MoveManager.UpdateGameState();

            UpdateCellsColor();

            FindKingCell(gameState);

            ShowingPossibleMoves(cellsObjects, isHighlight);


            if (!isHighlight)
            {
                HighlightedCellsObjects.Clear();
            }
        }

        private static void FindKingCell(string gameState)
        {
            GameObject kingCellObject = null;
            FigureParams.TypeOfTeam teamInCheck;

            // Визначаємо, яка команда під шахом
            if (gameState == GameState.WhiteInCheck.ToString())
            {
                teamInCheck = FigureParams.TypeOfTeam.WhiteTeam;
            }
            else if (gameState == GameState.BlackInCheck.ToString())
            {
                teamInCheck = FigureParams.TypeOfTeam.BlackTeam;
            }
            else
            {
                // Якщо немає шаху, просто оновлюємо кольори клітинок
                UpdateCellsColor();
                return;
            }

            // Знаходимо короля саме тієї команди, що під шахом
            foreach (FigureParams figure in GameManager.Figures)
            {
                if (figure.Type == FigureParams.TypeOfFigure.King && figure.TeamColor == teamInCheck)
                {
                    kingCellObject = GameManager.BoardCells.Find(cell => cell.NameOfCell == figure.CurrentPosition).gameObject;
                    break; // Зупиняємо пошук, як тільки знайшли
                }
            }

            // Підсвічуємо клітинку короля, якщо знайшли
            if (kingCellObject != null)
            {
                Renderer kingCellRenderer = kingCellObject.GetComponent<Renderer>();
                if (kingCellRenderer != null)
                {
                    kingCellRenderer.material.color = Color.red;
                }
            }
        }


        private static void ShowingPossibleMoves(List<GameObject> cells, bool isHighlight)
        {
            foreach (GameObject cell in cells)
            {
                Renderer cellRenderer = cell.GetComponent<Renderer>();
                _boardCellsParams = cell.GetComponent<BoardCellsParams>();

                if (cellRenderer != null)
                {
                    if (isHighlight)
                    {
                        FigureParams figureOnCell = GameManager.GetFigureOnCell(_boardCellsParams);

                        if (figureOnCell != null && figureOnCell.Status == FigureParams.TypeOfStatus.OnGame && figureOnCell.TeamColor != GameManager.SelectedFigure.TeamColor)
                        {
                            cellRenderer.material.color = new Color(1f, 0.647f, 0f);
                        }
                        else
                        {
                            cellRenderer.material.color = Color.green;
                        }

                        HighlightedCellsObjects.Add(cell.gameObject);
                    }
                }
            }
        }


        private static void UpdateCellsColor()
        {
            foreach (var cell in GameManager.BoardCells)
            {
                BoardCellsParams cellParams = cell.GetComponent<BoardCellsParams>();
                if (cellParams != null && cellParams.ColorOfCell != null)
                {
                    cell.GetComponent<Renderer>().material.color = cellParams.ColorOfCell;
                }
            }
        }
    }
}