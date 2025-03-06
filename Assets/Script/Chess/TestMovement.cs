using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoiceChess.MoveFigureManager;

namespace VoiceChess.Testing
{
    public class TestMovement : MonoBehaviour
    {
        public FigureMoveManager mMoveManager;
        public string? FigureName;
        public string? CurrentFigurePosition;
        public string NewFigurePosition;

        void Start()
        {
            mMoveManager.IsMoveAvailable(null, CurrentFigurePosition, NewFigurePosition);
        }
    }
}
