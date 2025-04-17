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
        [HideInInspector]
        public GameObject CellPrefab;

        private void Awake()
        {
            NameOfCell = gameObject.name;

            CellPrefab = gameObject;

            CellRenderer = GetComponent<Renderer>();
            if (CellRenderer != null)
            {
                ColorOfCell = CellRenderer.material.color;
            }

            Debug.Log($"🧩 Cell: {NameOfCell}, WorldPos: {transform.position}");
        }

    }
}
