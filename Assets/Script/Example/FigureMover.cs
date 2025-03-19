using System.Collections.Generic;
using UnityEngine;
using VoiceChess.FigureParameters;
using VoiceChess.MoveFigureManager;
using VoiceChess.Example.PaintingCells;
using VoiceChess.Example.CameraMoves;
using ChessSharp;
using ChessSharp.SquareData;

namespace VoiceChess.Example.Moving
{

    public class FigureMover : MonoBehaviour
    {
        public FigureMoveManager FigureMoveManager;
        public Transform ParentBoard;

        [HideInInspector]
        public static FigureMoveManager MoveManager;
        [HideInInspector]
        public static FigureParams SelectedFigure;
        [HideInInspector]
        public static List<GameObject> BoardCells = new List<GameObject>();
        [HideInInspector]
        public static FigureParams[] Figures;

        private void Awake()
        {
            MoveManager = FigureMoveManager;

            Figures = FigureMoveManager.Figures;

            foreach (Transform cell in ParentBoard)
            {
                BoardCells.Add(cell.gameObject);
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleClick();
            }
        }

        public static FigureParams GetFigureOnCell(GameObject cell)
        {
            foreach (FigureParams figure in Figures)
            {
                if (figure.CurrentPosition == cell.name)
                {
                    return figure;
                }
            }
            return null;
        }

        private void HandleClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                FigureParams clickedFigure = hit.collider.GetComponent<FigureParams>();
                GameObject clickedCell = hit.collider.gameObject;

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

                        if (CanAttack(clickedFigure))
                        {
                            clickedFigure.gameObject.SetActive(false);
                            FigureParams figureParams = clickedFigure;
                            if (figureParams != null)
                            {
                                clickedCell.name = figureParams.CurrentPosition;
                                MakeFigureMove(clickedCell);
                                CameraMovement.SwitchCameraPosition();
                            }
                        }
                    }
                }

                else if (SelectedFigure != null && BoardCells.Contains(clickedCell))
                {
                    MakeFigureMove(clickedCell);
                    CameraMovement.SwitchCameraPosition();
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

            List<GameObject> validMoves = GetValidMoveCells(figure);
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

        private List<GameObject> GetValidMoveCells(FigureParams figure)
        {
            List<GameObject> validCells = new List<GameObject>();

            foreach (GameObject cell in BoardCells)
            {
                string cellName = cell.name;
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

        private void MakeFigureMove(GameObject targetCell)
        {

            FigureParams figureOnCell = GetFigureOnCell(targetCell);
            string newPosition;

            if (figureOnCell != null && figureOnCell.TeamColor != SelectedFigure.TeamColor)
            {
                newPosition = figureOnCell.CurrentPosition;
            }
            else
            {
                newPosition = targetCell.name;
            }

            if (FigureMoveManager.IsMoveAvailable(SelectedFigure.Type.ToString(), SelectedFigure.CurrentPosition, newPosition))
            {
                MovingObject(newPosition, targetCell);

                DeselectFigure();
            }
        }

        private void MovingObject(string newPosition, GameObject targetCell)
        {
            Vector3 currentPosition = SelectedFigure.transform.position;
            Vector3 newPositionInWorld = targetCell.transform.position;
            newPositionInWorld.y = currentPosition.y;
            SelectedFigure.transform.position = newPositionInWorld;

            SelectedFigure.PreviousPosition = SelectedFigure.CurrentPosition;
            SelectedFigure.CurrentPosition = newPosition;
        }

        private bool CanAttack(FigureParams enemyFigure)
        {
            try
            {
                return SelectedFigure.TeamColor != enemyFigure.TeamColor;
            }
            catch
            {
                return false;
            }
        }
    }
}