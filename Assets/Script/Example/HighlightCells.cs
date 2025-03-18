using ChessSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VoiceChess.Example.Moving;
using VoiceChess.FigureParameters;

namespace VoiceChess.Example.PaintingCells
{
    public class HighlightCells : MonoBehaviour
    {
        [HideInInspector]
        public static List<GameObject> HighlightedCellsObjects = new List<GameObject>();

        private static Dictionary<GameObject, Color> _originalCellColors = new Dictionary<GameObject, Color>();

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
            // Знайти клітинку короля
            foreach (FigureParams figure in FigureMover.Figures)
            {
                if (figure.Type == FigureParams.TypeOfFigure.King)
                {
                    kingCellObject = FigureMover.BoardCells.Find(cell => cell.name == figure.CurrentPosition);
                    if (kingCellObject != null) break;
                }
            }

            // Якщо король під шахом, фарбуємо його клітинку в червоний
            if (gameState == GameState.WhiteInCheck.ToString() || gameState == GameState.BlackInCheck.ToString())
            {
                if (kingCellObject != null)
                {
                    Renderer kingCellRenderer = kingCellObject.GetComponent<Renderer>();
                    if (kingCellRenderer != null)
                    {
                        ColorFilling(Color.red, kingCellRenderer, kingCellObject);
                    }
                }
            }
            // Якщо шаху більше немає, повертаємо клітинці короля її оригінальний колір
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
                if (cellRenderer != null)
                {
                    if (isHighlight)
                    {
                        FigureParams figureOnCell = FigureMover.GetFigureOnCell(cell);
                        if (figureOnCell != null && figureOnCell.TeamColor != FigureMover.SelectedFigure.TeamColor)
                        {
                            ColorFilling(new Color(1f, 0.647f, 0f), cellRenderer, cell);
                        }
                        else
                        {
                            ColorFilling(Color.green, cellRenderer, cell);
                        }

                        HighlightedCellsObjects.Add(cell);
                    }
                }
            }
        }

        private static void ColorFilling(Color newColor, Renderer cellRenderer, GameObject cellObject)
        {
            if (!_originalCellColors.ContainsKey(cellObject))
            {
                _originalCellColors[cellObject] = cellRenderer.material.color;
            }
            cellRenderer.material.color = newColor;
        }

        private static void UpdateCellsColor()
        {
            // Оновлюємо кольори всіх клітинок перед підсвічуванням
            foreach (var cell in FigureMover.BoardCells)
            {
                if (_originalCellColors.TryGetValue(cell, out Color originalColor))
                {
                    cell.GetComponent<Renderer>().material.color = originalColor;
                }
            }
        }
    }
}
