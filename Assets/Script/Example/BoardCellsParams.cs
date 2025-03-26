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

        private void Awake()
        {
            NameOfCell = gameObject.name;

            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                ColorOfCell = renderer.material.color;
            }
        }

    }
}
