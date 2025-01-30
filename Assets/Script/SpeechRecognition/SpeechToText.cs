using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HuggingFace.API;

namespace VoiceChess.SpeechRecognition
{
    public class SpeechToText : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private TextMeshProUGUI _text;

        private AudioClip _clip;
        private byte[] _bytes;
        private bool _recording;

        private void Start()
        {
            _startButton.onClick.AddListener(StartRecording);
            _stopButton.onClick.AddListener(StopRecording);
            _stopButton.interactable = false;
        }

        private void Update()
        {
            if (_recording && Microphone.GetPosition(null) >= _clip.samples)
            {
                StopRecording();
            }
        }

        private void StartRecording()
        {
            _text.color = Color.white;
            _text.text = "Recording...";
            _startButton.interactable = false;
            _stopButton.interactable = true;
            _clip = Microphone.Start(null, false, 10, 44100);
            _recording = true;
        }

        private void StopRecording()
        {
            var position = Microphone.GetPosition(null);
            Microphone.End(null);
            var samples = new float[position * _clip.channels];
            _clip.GetData(samples, 0);
            _bytes = EncodeAsWAV(samples, _clip.frequency, _clip.channels);
            _recording = false;
            SendRecording();
        }

        private void SendRecording()
        {
            _text.color = Color.yellow;
            _text.text = "Sending...";
            _stopButton.interactable = false;
            HuggingFaceAPI.AutomaticSpeechRecognition(_bytes, response =>
            {
                _text.color = Color.white;
                _text.text = response;
                _startButton.interactable = true;
            }, error =>
            {
                _text.color = Color.red;
                _text.text = error;
                _startButton.interactable = true;
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