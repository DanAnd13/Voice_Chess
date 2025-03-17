using UnityEditor;
using UnityEngine;
using VoiceChess.SpeechRecognition;

[CustomEditor(typeof(SpeechToText))]
public class SpeechToTextEditor : Editor
{
    private static EditorConfig editorConfig;

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

        SpeechToText speechToText = (SpeechToText)target;

        EditorGUILayout.LabelField("Speech to Text Controller", EditorStyles.boldLabel);

        if (GUILayout.Button("Start Recording"))
        {
            speechToText.StartRecording();
            Debug.Log("Recording started...");
        }

        if (GUILayout.Button("Stop Recording"))
        {
            speechToText.StopRecording();
            Debug.Log("Recording stopped...");
        }

        // 🔹 Оновлюємо поле в редакторі
        EditorGUILayout.LabelField("Recognized Text:");
        EditorGUILayout.TextArea(speechToText.RecognizedText, GUILayout.Height(50));

        // 🔹 Автоматично оновлюємо інспектор
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
