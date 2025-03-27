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
        public GameObject CellObject;

        private void Awake()
        {
            NameOfCell = gameObject.name;

            CellObject = gameObject;

            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                ColorOfCell = renderer.material.color;
            }
        }

    }
}
