using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoiceChess.MoveFigureManager;

namespace VoiceChess.Testing
{
    public class TestMovement : MonoBehaviour
    {
        public FigureMoveManager mMoveManager;
        public string FigureName;
        public string FigurePosition;

        void Start()
        {
            mMoveManager.IsMoveAvailable(FigureName, FigurePosition);
        }
    }
}
