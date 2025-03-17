using System.Collections.Generic;
using UnityEngine;
using VoiceChess.FigureParameters;
using VoiceChess.MoveFigureManager;
using ChessSharp;
using ChessSharp.SquareData;

public class FigureMover : MonoBehaviour
{
    public FigureMoveManager MoveManager;
    public Transform ParentBoard;

    private List<GameObject> _boardCells = new List<GameObject>();
    private FigureParams[] _figures;
    private FigureParams _selectedFigure;
    private List<GameObject> _highlightedCells = new List<GameObject>();

    private void Awake()
    {
        _figures = MoveManager.Figures;

        foreach (Transform cell in ParentBoard)
        {
            _boardCells.Add(cell.gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            FigureParams clickedFigure = hit.collider.GetComponent<FigureParams>();
            GameObject clickedCell = hit.collider.gameObject;

            // якщо натиснута ф≥гура
            if (clickedFigure != null)
            {
                Player currentPlayer = MoveManager.Board.WhoseTurn();
                if (IsFigureBelongsToCurrentPlayer(clickedFigure, currentPlayer))
                {
                    if (_selectedFigure == clickedFigure)
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
                    // якщо натискаЇш на ворожу ф≥гуру, перев≥р€Їмо можлив≥сть бою
                    if (CanAttack(clickedFigure))
                    {
                        clickedFigure.gameObject.SetActive(false);
                        FigureParams figureParams = clickedFigure;
                        if (figureParams != null)
                        {
                            clickedCell.name = figureParams.CurrentPosition;
                            TryMoveFigure(clickedCell);
                        }
                    }
                }
            }
            // якщо натиснута кл≥тинка
            else if (_selectedFigure != null && _boardCells.Contains(clickedCell))
            {
                TryMoveFigure(clickedCell);
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

        _selectedFigure = figure;
        _selectedFigure.transform.position += Vector3.up * 0.2f; // ѕ≥дн≥маЇмо ф≥гуру

        List<GameObject> validMoves = GetValidMoveCells(figure);
        HighlightCells(validMoves, highlight: true);
    }

    private void DeselectFigure()
    {
        if (_selectedFigure != null)
        {
            _selectedFigure.transform.position -= Vector3.up * 0.2f;
            _selectedFigure = null;
        }

        HighlightCells(_highlightedCells, highlight: false);
        _highlightedCells.Clear();
    }

    private void TryMoveFigure(GameObject targetCell)
    {
        // якщо кл≥тинка м≥стить ворожу ф≥гуру, використовуЇмо њњ позиц≥ю
        FigureParams figureOnCell = GetFigureOnCell(targetCell);
        string newPosition;

        if (figureOnCell != null && figureOnCell.TeamColor != _selectedFigure.TeamColor)
        {
            newPosition = figureOnCell.CurrentPosition;
        }
        else
        {
            newPosition = targetCell.name;
        }

        // ѕерев≥рка вал≥дност≥ ходу
        if (MoveManager.IsMoveAvailable(_selectedFigure.Type.ToString(), _selectedFigure.CurrentPosition, newPosition))
        {
            Vector3 currentPosition = _selectedFigure.transform.position;
            Vector3 newPositionInWorld = targetCell.transform.position;
            newPositionInWorld.y = currentPosition.y;
            _selectedFigure.transform.position = newPositionInWorld;

            _selectedFigure.PreviousPosition = _selectedFigure.CurrentPosition;
            _selectedFigure.CurrentPosition = newPosition;

            DeselectFigure();
        }
    }

    private List<GameObject> GetValidMoveCells(FigureParams figure)
    {
        List<GameObject> validCells = new List<GameObject>();

        foreach (GameObject cell in _boardCells)
        {
            string cellName = cell.name;
            Square destinationSquare = Square.Parse(cellName);

            if (CreateMoveAtributes(destinationSquare, figure, cellName))
            {
                validCells.Add(cell);
            }
        }
        return validCells;
    }

    private bool CreateMoveAtributes(Square destinationSquare, FigureParams figure, string newPosition)
    {
        Square currentSquare = Square.Parse(figure.CurrentPosition);
        Move move = new Move(currentSquare, destinationSquare, MoveManager.Board.WhoseTurn());

        if (MoveManager.Board.IsValidMove(move))
        {
            return true;
        }

        return false; 
    }

    private Dictionary<GameObject, Color> originalCellColors = new Dictionary<GameObject, Color>();

    private void HighlightCells(List<GameObject> cells, bool highlight)
    {
        foreach (GameObject cell in cells)
        {
            Renderer cellRenderer = cell.GetComponent<Renderer>();
            if (cellRenderer != null)
            {
                if (highlight)
                {

                    if (!originalCellColors.ContainsKey(cell))
                    {
                        originalCellColors[cell] = cellRenderer.material.color;
                    }

                    FigureParams figureOnCell = GetFigureOnCell(cell);
                    if (figureOnCell != null && figureOnCell.TeamColor != _selectedFigure.TeamColor)
                    {
                        cellRenderer.material.color = new Color(1f, 0.647f, 0f);
                    }
                    else
                    {
                        cellRenderer.material.color = Color.green;
                    }

                    _highlightedCells.Add(cell);
                }
                else
                {
                    // ¬≥дновлюЇмо ориг≥нальний кол≥р кл≥тинки
                    if (originalCellColors.TryGetValue(cell, out Color originalColor))
                    {
                        cellRenderer.material.color = originalColor;
                    }
                }
            }
        }

        if (!highlight)
        {
            _highlightedCells.Clear();
            originalCellColors.Clear();
        }
    }

    private FigureParams GetFigureOnCell(GameObject cell)
    {
        foreach (FigureParams figure in _figures)
        {
            if (figure.CurrentPosition == cell.name)
            {
                return figure;
            }
        }
        return null;
    }

    private bool CanAttack(FigureParams enemyFigure)
    {
        return _selectedFigure.TeamColor != enemyFigure.TeamColor;
    }
}
