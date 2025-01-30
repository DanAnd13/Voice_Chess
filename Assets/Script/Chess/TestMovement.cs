using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoiceChess.MoveFigureManager;

public class TestMovement : MonoBehaviour
{
    public FigureMoveManager mMoveManager;
    public string FigureName;
    public string FigurePosition;

    void Start()
    {
        mMoveManager.MoveFigure(FigureName, FigurePosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
