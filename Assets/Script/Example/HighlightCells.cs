using ChessSharp;
using System.Collections.Generic;
using UnityEngine;
using VoiceChess.BoardCellsParameters;
using VoiceChess.Example.Manager;
using VoiceChess.FigureParameters;

namespace VoiceChess.Example.PaintingCells
{
    public class HighlightCells : MonoBehaviour
    {
        [HideInInspector]
        public static List<BoardCellsParams> HighlightedCellsObjects = new List<BoardCellsParams>();

        public static void PaintCells(List<BoardCellsParams> cellsObjects, bool isHighlight)
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
            BoardCellsParams kingCellObject = null;
            FigureParams.TypeOfTeam teamInCheckColor;

            // Визначаємо, яка команда під шахом
            if (gameState == GameState.WhiteInCheck.ToString())
            {
                teamInCheckColor = FigureParams.TypeOfTeam.WhiteTeam;
            }
            else if (gameState == GameState.BlackInCheck.ToString())
            {
                teamInCheckColor = FigureParams.TypeOfTeam.BlackTeam;
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
                if (figure.Type == FigureParams.TypeOfFigure.King && figure.TeamColor == teamInCheckColor)
                {
                    kingCellObject = GameManager.BoardCells.Find(cell => cell.NameOfCell == figure.CurrentPosition);
                    break;
                }
            }

            // Підсвічуємо клітинку короля, якщо знайшли
            if (kingCellObject != null)
            {
                PaintingCellInColor(kingCellObject, Color.red);
            }
        }


        private static void ShowingPossibleMoves(List<BoardCellsParams> cells, bool isHighlight)
        {
            foreach (BoardCellsParams cell in cells)
            {
                if (cell.CellRenderer != null)
                {
                    if (isHighlight)
                    {
                        FigureParams figureOnCell = GameManager.GetFigureOnCell(cell);

                        if (figureOnCell != null && figureOnCell.Status == FigureParams.TypeOfStatus.OnGame && 
                            GameManager.IsItDifferentTeamByColor(figureOnCell.TeamColor, GameManager.SelectedFigure.TeamColor))
                        {
                            PaintingCellInColor(cell, new Color(1f, 0.647f, 0f));
                            //cell.CellRenderer.material.color = new Color(1f, 0.647f, 0f);
                        }
                        else
                        {
                            PaintingCellInColor(cell, Color.green);
                            //cell.CellRenderer.material.color = Color.green;
                        }

                        HighlightedCellsObjects.Add(cell);
                    }
                }
            }
        }

        private static void PaintingCellInColor(BoardCellsParams boardCells, Color newColor)
        {
            if (boardCells.CellRenderer != null)
            {
                boardCells.CellRenderer.material.color = newColor;
            }
        }

        private static void UpdateCellsColor()
        {
            foreach (var cell in GameManager.BoardCells)
            {
                if (cell != null && cell.ColorOfCell != null)
                {
                    cell.GetComponent<Renderer>().material.color = cell.ColorOfCell;
                }
            }
        }
    }
}