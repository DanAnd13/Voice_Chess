using UnityEngine;
using UnityEditor;
using VoiceChess.Speaking;

[CustomEditor(typeof(TextToSpeech))]
public class TextToSpeechEditor : Editor
{
    private string inputText = "Enter text here...";
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

        TextToSpeech tts = (TextToSpeech)target;

        inputText = EditorGUILayout.TextField("Text to Speak:", inputText);

        if (GUILayout.Button("Speak"))
        {
            tts.SetTextAndSpeak(inputText);
        }

        DrawDefaultInspector();
    }
}
