using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VoiceChess.BoardCellsParameters
{
    public class BoardCellsParams : MonoBehaviour
    {
        [HideInInspector]
        public string NameOfCell;
        [HideInInspector]
        public Color ColorOfCell;
        [HideInInspector]
        public Renderer CellRenderer;

        private void Awake()
        {
            NameOfCell = gameObject.name;

            CellRenderer = GetComponent<Renderer>();
            if (CellRenderer != null)
            {
                ColorOfCell = CellRenderer.material.color;
            }
        }

    }
}
