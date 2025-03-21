using ChessSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VoiceChess.BoardCellsParameters;
using VoiceChess.Example.Moving;
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
            string gameState = FigureMover.MoveManager.UpdateGameState();
            GameObject kingCellObject = null;

            UpdateCellsColor();

            FindeKingCells(kingCellObject, gameState);

            ShowingPossibleMoves(cellsObjects, isHighlight);


            if (!isHighlight)
            {
                HighlightedCellsObjects.Clear();
            }
        }

        private static void FindeKingCells(GameObject kingCellObject, string gameState)
        {
            foreach (FigureParams figure in FigureMover.Figures)
            {
                if (figure.Type == FigureParams.TypeOfFigure.King)
                {
                    kingCellObject = FigureMover.BoardCells.Find(cell => cell.name == figure.CurrentPosition);
                    if (kingCellObject != null) break;
                }
            }

            if (gameState == GameState.WhiteInCheck.ToString() || gameState == GameState.BlackInCheck.ToString())
            {
                if (kingCellObject != null)
                {
                    Renderer kingCellRenderer = kingCellObject.GetComponent<Renderer>();
                    if (kingCellRenderer != null)
                    {
                        kingCellRenderer.material.color = Color.red;
                    }
                }
            }
            else if (gameState == GameState.NotCompleted.ToString() && kingCellObject != null)
            {
                UpdateCellsColor();
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
                        FigureParams figureOnCell = FigureMover.GetFigureOnCell(_boardCellsParams);
                        if (figureOnCell != null && figureOnCell.TeamColor != FigureMover.SelectedFigure.TeamColor)
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
            foreach (var cell in FigureMover.BoardCells)
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