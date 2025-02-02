using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HuggingFace.API;

namespace VoiceChess.SpeechRecognition
{
    public class SpeechToText : MonoBehaviour
    {
        public Button StartButton;
        public Button StopButton;
        public TextMeshProUGUI OutputText;

        private AudioClip _clip;
        private byte[] _bytes;
        private bool _isRecording;

        private void Awake()
        {
            StartButton.onClick.AddListener(StartRecording);
            StopButton.onClick.AddListener(StopRecording);
            StopButton.interactable = false;
        }

        private void Update()
        {
            if (_isRecording && Microphone.GetPosition(null) >= _clip.samples)
            {
                StopRecording();
            }
        }

        private void StartRecording()
        {
            OutputText.color = Color.white;
            OutputText.text = "Recording...";
            StartButton.interactable = false;
            StopButton.interactable = true;
            _clip = Microphone.Start(null, false, 10, 44100);
            _isRecording = true;
        }

        private void StopRecording()
        {
            var position = Microphone.GetPosition(null);
            Microphone.End(null);
            var samples = new float[position * _clip.channels];
            _clip.GetData(samples, 0);
            _bytes = EncodeAsWAV(samples, _clip.frequency, _clip.channels);
            _isRecording = false;
            SendRecording();
        }

        private void SendRecording()
        {
            OutputText.color = Color.yellow;
            OutputText.text = "Sending...";
            StopButton.interactable = false;
            HuggingFaceAPI.AutomaticSpeechRecognition(_bytes, response =>
            {
                OutputText.color = Color.white;
                OutputText.text = response;
                StartButton.interactable = true;
            }, error =>
            {
                OutputText.color = Color.red;
                OutputText.text = error;
                StartButton.interactable = true;
            });
        }

        private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
        {
            using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
            {
                using (var writer = new BinaryWriter(memoryStream))
                {
                    writer.Write("RIFF".ToCharArray());
                    writer.Write(36 + samples.Length * 2);
                    writer.Write("WAVE".ToCharArray());
                    writer.Write("fmt ".ToCharArray());
                    writer.Write(16);
                    writer.Write((ushort)1);
                    writer.Write((ushort)channels);
                    writer.Write(frequency);
                    writer.Write(frequency * channels * 2);
                    writer.Write((ushort)(channels * 2));
                    writer.Write((ushort)16);
                    writer.Write("data".ToCharArray());
                    writer.Write(samples.Length * 2);

                    foreach (var sample in samples)
                    {
                        writer.Write((short)(sample * short.MaxValue));
                    }
                }
                return memoryStream.ToArray();
            }
        }
    }
}