using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoiceChess.FigureParameters;

public class PawnPromotionSpawner : MonoBehaviour
{
    public GameObject WhiteRook;
    public GameObject BlackRook;
    public GameObject WhiteKnight;
    public GameObject BlackKnight;
    public GameObject WhiteBishop;
    public GameObject BlackBishop;
    public GameObject WhiteQueen;
    public GameObject BlackQueen;

    public void SpawnPromotedFigure(FigureParams oldPawn, FigureParams[] Figures)
    {
        GameObject prefabToSpawn = null;

        if (oldPawn.TeamColor == FigureParams.TypeOfTeam.WhiteTeam)
        {
            switch (oldPawn.Type)
            {
                case FigureParams.TypeOfFigure.Rook:
                    prefabToSpawn = WhiteRook;
                    break;
                case FigureParams.TypeOfFigure.Knight:
                    prefabToSpawn = WhiteKnight;
                    break;
                case FigureParams.TypeOfFigure.Bishop:
                    prefabToSpawn = WhiteBishop;
                    break;
                case FigureParams.TypeOfFigure.Queen:
                    prefabToSpawn = WhiteQueen;
                    break;
            }
        }
        else
        {
            switch (oldPawn.Type)
            {
                case FigureParams.TypeOfFigure.Rook:
                    prefabToSpawn = BlackRook;
                    break;
                case FigureParams.TypeOfFigure.Knight:
                    prefabToSpawn = BlackKnight;
                    break;
                case FigureParams.TypeOfFigure.Bishop:
                    prefabToSpawn = BlackBishop;
                    break;
                case FigureParams.TypeOfFigure.Queen:
                    prefabToSpawn = BlackQueen;
                    break;
            }
        }

        if (prefabToSpawn == null)
        {
            Debug.LogError("No prefab assigned for promotion type: " + oldPawn.Type);
            return;
        }

        Vector3 localPosition = new Vector3(oldPawn.transform.localPosition.x, -4.1f, oldPawn.transform.localPosition.z);
        Quaternion rotation = oldPawn.transform.rotation;
        Transform parent = oldPawn.transform.parent;

        // Створюємо без parent, потім додаємо вручну
        GameObject newFigureObj = Instantiate(prefabToSpawn);
        newFigureObj.transform.SetParent(parent, worldPositionStays: false); // <-- важливо

        // Встановлюємо локальну позицію, як у решти фігур
        newFigureObj.transform.localPosition = localPosition;
        newFigureObj.transform.localRotation = rotation;
        FigureParams newFigure = newFigureObj.GetComponent<FigureParams>();

        // Копіюємо дані зі старої фігури
        newFigure.CurrentPosition = oldPawn.CurrentPosition;
        newFigure.PreviousPosition = oldPawn.PreviousPosition;
        newFigure.Type = oldPawn.Type;
        newFigure.TeamColor = oldPawn.TeamColor;
        newFigure.Status = oldPawn.Status;

        // Вимикаємо стару фігуру
        WriteFigureInFiguresArray(Figures, oldPawn, newFigure);
        oldPawn.gameObject.SetActive(false);
    }

    public void WriteFigureInFiguresArray(FigureParams[] Figures, FigureParams oldFigure, FigureParams newFigure)
    {
        for(int i = 0; i < Figures.Length; i++)
        {
            if(Figures[i] == oldFigure)
            {
                Figures[i] = newFigure;
                break;
            }
        }

    }
}
