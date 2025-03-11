using UnityEngine;
using UnityEditor;
using VoiceChess.MoveFigureManager;

[CustomEditor(typeof(FigureMoveManager))]
public class FigureMoveManagerEditor : Editor
{
    private string figureName = "";       // Тип фігури (наприклад, Pawn, Rook)
    private string currentPosition = "";   // Поточна позиція (наприклад, E2)
    private string newPosition = "";       // Нова позиція (наприклад, E4)

    public override void OnInspectorGUI()
    {
        FigureMoveManager moveManager = (FigureMoveManager)target;

        serializedObject.Update(); // Оновлення значень перед відображенням

        EditorGUILayout.LabelField("Figure Movement", EditorStyles.boldLabel);

        figureName = EditorGUILayout.TextField("Figure Name:", figureName);
        currentPosition = EditorGUILayout.TextField("Current Position:", currentPosition);
        newPosition = EditorGUILayout.TextField("New Position:", newPosition);

        if (GUILayout.Button("Move Figure"))
        {
            if (!string.IsNullOrWhiteSpace(newPosition))
            {
                bool success = moveManager.IsMoveAvailable(
                    string.IsNullOrWhiteSpace(figureName) ? null : figureName,
                    string.IsNullOrWhiteSpace(currentPosition) ? null : currentPosition,
                    newPosition
                );

                Debug.Log($"Move executed: {success}");
            }
            else
            {
                Debug.LogError("New position cannot be empty!");
            }
        }

        serializedObject.ApplyModifiedProperties(); // Застосування змін
    }
}
