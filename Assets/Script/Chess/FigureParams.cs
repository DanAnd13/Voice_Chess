using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoiceChess.FigureParameters
{
    public class FigureParams : MonoBehaviour
    {
        public TypeOfFigure Type;
        public string CurrentPosition;
        public string PreviousPosition;
        public TypeOfTeam TeamColor;
        public enum TypeOfTeam
        {
            BlackTeam,
            WhiteTeam
        }
        public enum TypeOfFigure
        {
            Bishop,
            King,
            Queen,
            Knight,
            Pawn,
            Rook
        }

    }
}
