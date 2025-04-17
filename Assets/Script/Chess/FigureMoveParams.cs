using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FigureMoveParams
{
    public string? FigureName;
    public string? CurrentPosition;
    public string NewPosition;
    public string TypeOfPattern;

    public FigureMoveParams(string figure, string currentPos, string newPos, string patternType)
    {
        FigureName = figure;
        CurrentPosition = currentPos;
        NewPosition = newPos;
        TypeOfPattern = patternType;
    }
}

