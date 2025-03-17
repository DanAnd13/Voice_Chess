using UnityEngine;
using UnityEditor;
using VoiceChess.MoveFigureManager;

[CustomEditor(typeof(FigureMoveManager))]
public class FigureMoveManagerEditor : Editor
{
    private static EditorConfig editorConfig;
    private string _figureName = "";       
    private string _currentPosition = "";   
    private string _newPosition = "";       
    private string _moveResult = "";        
    private string _currentGameState = "";

    private void OnEnable()
    {
        if (editorConfig == null)
        {
            editorConfig = Resources.Load<EditorConfig>("EditorConfig");
        }
    }

    public override void OnInspectorGUI()
    {
        if (editorConfig == null)
        {
            EditorGUILayout.HelpBox("EditorConfig not found! Create it in Resources folder.", MessageType.Error);
            return;
        }

        // Додаємо кнопку для зміни стану
        editorConfig.enableEditorScript = EditorGUILayout.Toggle("Enable Editor Script", editorConfig.enableEditorScript);

        if (!editorConfig.enableEditorScript)
        {
            EditorGUILayout.HelpBox("Editor Script is disabled", MessageType.Warning);
            return;
        }

        FigureMoveManager moveManager = (FigureMoveManager)target;

        serializedObject.Update();

        EditorGUILayout.LabelField("Figure Movement", EditorStyles.boldLabel);

        _figureName = EditorGUILayout.TextField("Figure Name:", _figureName);
        _currentPosition = EditorGUILayout.TextField("Current Position:", _currentPosition);
        _newPosition = EditorGUILayout.TextField("New Position:", _newPosition);

        if (GUILayout.Button("Move Figure"))
        {
            if (!string.IsNullOrWhiteSpace(_newPosition))
            {
                moveManager.IsMoveAvailable(
                    string.IsNullOrWhiteSpace(_figureName) ? null : _figureName,
                    string.IsNullOrWhiteSpace(_currentPosition) ? null : _currentPosition,
                    _newPosition
                );

                _moveResult = moveManager.GetLastMoveResult();
                _currentGameState = moveManager.UpdateGameState();
            }
            else
            {
                _moveResult = "New position cannot be empty!";
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Move Result:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(_moveResult);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current Game State:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(_currentGameState);

        serializedObject.ApplyModifiedProperties();
    }
}
