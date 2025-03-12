using UnityEditor;
using UnityEngine;
using VoiceChess.SpeechRecognition;

[CustomEditor(typeof(SpeechToText))]
public class SpeechToTextEditor : Editor
{
    public override void OnInspectorGUI()
    {
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
