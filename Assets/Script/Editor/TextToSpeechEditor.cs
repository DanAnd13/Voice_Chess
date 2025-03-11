using UnityEngine;
using UnityEditor;
using VoiceChess.Speaking;

[CustomEditor(typeof(TextToSpeech))]
public class TextToSpeechEditor : Editor
{
    private string inputText = "Enter text here...";

    public override void OnInspectorGUI()
    {
        TextToSpeech tts = (TextToSpeech)target;

        inputText = EditorGUILayout.TextField("Text to Speak:", inputText);

        if (GUILayout.Button("Speak"))
        {
            tts.SetTextAndSpeak(inputText);
        }

        DrawDefaultInspector();
    }
}
