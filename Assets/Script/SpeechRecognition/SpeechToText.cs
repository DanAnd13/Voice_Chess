using System.IO;
using UnityEngine;
using HuggingFace.API;
using System.Text.RegularExpressions;

namespace VoiceChess.SpeechRecognition
{
    public class SpeechToText : MonoBehaviour
    {
        private AudioClip _clip;
        private byte[] _bytes;
        private bool _isRecording;

        public string RecognizedText { get; private set; } = ""; // 🔹 Додаємо змінну для тексту

        private void Update()
        {
            if (_isRecording && Microphone.GetPosition(null) >= _clip.samples)
            {
                StopRecording();
            }
        }

        public void StartRecording()
        {
            _clip = Microphone.Start(null, false, 10, 44100);
            _isRecording = true;
        }

        public void StopRecording()
        {
            var position = Microphone.GetPosition(null);
            if (position <= 0) return;

            Microphone.End(null);
            var samples = new float[position * _clip.channels];
            _clip.GetData(samples, 0);

            _bytes = EncodeAsWAV(samples, _clip.frequency, _clip.channels);
            _isRecording = false;
            SendRecording();
        }

        private void SendRecording()
        {
            HuggingFaceAPI.AutomaticSpeechRecognition(_bytes, response =>
            {
                RecognizedText = TextCorrection(response); // 🔹 Оновлюємо RecognizedText
            }, error =>
            {
                RecognizedText = "Error: " + error;
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
            //Debug.Log($"Raw speech text: '{text}'");
            text = ReplacementOfMistakes(text);
            return PatternAnalyzer(text);
        }

        private string ReplacementOfMistakes(string text)
        {
            return text.ToLower().Trim()
                .Replace("for", "four")
                .Replace("to", "two")
                .Replace("too", "two")
                .Replace("tree", "three")
                .Replace("tri", "three")
                .Replace("free", "three")
                .Replace("fire", "five")
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
                .Replace("nine", "9")
                .Replace("bawn", "pawn")
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
                .Replace("in", "king")
                .Replace("himg", "king")
                .Replace("kink", "king")
                .Replace("game", "king")
                .Replace("kim", "king")
                .Replace("thing", "king")
                .Replace("b-shop", "bishop")
                .Replace("bshop", "bishop");
        }

        private string PatternAnalyzer(string text)
        {
            //Debug.Log($"Processed text: '{text}'");
            string figurePositionPattern = @"\s*(pawn|knight|bishop|rook|queen|king)\s*(?:to|on)?\s*([a-hA-H])\s*(\d+)\s*";
            string figurePositionPositionPattern = @"\s*(pawn|knight|bishop|rook|queen|king)\s*(?:from)?\s*([a-hA-H])\s*(\d+)\s*(?:to|on)?\s*([a-hA-H])\s*(\d+)\s*";
            string positionPositionPattern = @"\s*(?:from)?\s*([a-hA-H])\s*(\d+)\s*(?:to|on)?\s*([a-hA-H])\s*(\d+)\s*";

            Match match;

            match = Regex.Match(text, figurePositionPositionPattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return $"Detected Move: {match.Groups[1].Value} from {match.Groups[2].Value}{match.Groups[3].Value} to {match.Groups[4].Value}{match.Groups[5].Value}";
            }

            match = Regex.Match(text, positionPositionPattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return $"Detected Move: from {match.Groups[1].Value}{match.Groups[2].Value} to {match.Groups[3].Value}{match.Groups[4].Value}";
            }

            match = Regex.Match(text, figurePositionPattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return $"Detected Move: {match.Groups[1].Value} to {match.Groups[2].Value}{match.Groups[3].Value}";
            }

            return "Unrecognized command. Try again.";
        }
    }
}
