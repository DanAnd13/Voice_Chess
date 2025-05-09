using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoiceChess.Speaking;

public class CallVoicing : MonoBehaviour
{
    public AudioSource audioPlayer;

    public void PlayVoicing(string text)
    {
        TextToSpeech.SetTextAndSpeak(text, audioPlayer);
    }
}
