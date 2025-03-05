using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HuggingFace.API;
using System.Text.RegularExpressions;

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
            if (position <= 0) return;  // Переконатися, що запис відбувся

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
            OutputText.text = "Processing...";
            StopButton.interactable = false;

            HuggingFaceAPI.AutomaticSpeechRecognition(_bytes, response =>
            {
                var processedText = TextCorrection(response);
                OutputText.color = Color.white;
                OutputText.text = processedText;
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

        private string TextCorrection(string text)
        {
            Debug.Log($"Raw speech text: '{text}'");
            text = ReplacementOfMistakes(text);
            return PatternAnalyzer(text);
        }

        private string ReplacementOfMistakes(string text)
        {
            text = text.ToLower().Trim();

            text = text.Replace("for", "four")
                       .Replace("to", "two")
                       .Replace("too", "two")
                       .Replace("tree", "three")
                       .Replace("tri", "three")
                       .Replace("free", "three")
                       .Replace("one", "1")
                       .Replace("won", "1")
                       .Replace("two", "2")
                       .Replace("three", "3")
                       .Replace("four", "4")
                       .Replace("five", "5")
                       .Replace("six", "6")
                       .Replace("sick", "6")
                       .Replace("seven", "7")
                       .Replace("eight", "8")
                       .Replace("ate", "8")
                       .Replace("nine", "9");

            // 🔹 Виправлення розпізнавання літер
            text = text.Replace("age", "h")
                       .Replace("hey", "h")
                       .Replace("see", "c")
                       .Replace("sea", "c")
                       .Replace("tea", "t")
                       .Replace("bee", "b")
                       .Replace("dee", "d")
                       .Replace("de", "d")
                       .Replace("i", "e")
                       .Replace("eff", "f")
                       .Replace("gee", "g");

            // 🔹 Виправлення розпізнавання шахових фігур
            text = text.Replace("bawn", "pawn")
                       .Replace("bown", "pawn")
                       .Replace("boun", "pawn")
                       .Replace("pawnd", "pawn")
                       .Replace("pound", "pawn")
                       .Replace("night", "knight")
                       .Replace("nite", "knight")
                       .Replace("knite", "knight")
                       .Replace("brook", "rook")
                       .Replace("rock", "rook")
                       .Replace("ruke", "rook")
                       .Replace("green", "queen")
                       .Replace("bean", "queen")
                       .Replace("twin", "queen")
                       .Replace("kink", "king")
                       .Replace("game", "king")
                       .Replace("kim", "king")
                       .Replace("kem", "king")
                       .Replace("thing", "king")
                       .Replace("ying", "king")
                       .Replace("kimk", "king")
                       .Replace("b-shop", "bishop")
                       .Replace("bshop", "bishop")
                       .Replace("B-shop", "bishop");

            return text;
        }

        private string PatternAnalyzer(string text)
        {
            Debug.Log($"Processed text: '{text}'");

            string pattern = @"\s*(pawn|knight|bishop|rook|queen|king)\s*(?:to|on)?\s*([a-hA-H])\s*(\d+)\s*";

            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                string piece = match.Groups[1].Value;
                string column = match.Groups[2].Value;
                string row = match.Groups[3].Value;

                Debug.Log($"Detected move: {piece} {column}{row}");
                return $"Detected Move: {piece} {column}{row}";
            }

            Debug.LogError($"Regex failed on input: '{text}'");
            return "Unrecognized command. Try again.";
        }

    }
}
