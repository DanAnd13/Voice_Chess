﻿using System.IO;
using UnityEngine;
using HuggingFace.API;
using System.Text.RegularExpressions;
using System.Collections;
using System;
using System.ComponentModel;

namespace VoiceChess.SpeechRecognition
{
    public class SpeechToText : MonoBehaviour
    {
        public static FigureMoveParams? LastParsedMove { get; private set; } = null;
        public static string RecognizedText { get; private set; } = ""; // 🔹 Додаємо змінну для тексту
        public static bool IsGetRequest = true;
        public static event Action<FigureMoveParams> OnMoveParsed;

        private static AudioClip _clip;
        private static byte[] _bytes;
        private static bool _isRecording;
        private static string _figurePositionPattern = @"\s*(pawn|knight|bishop|rook|queen|king)\s*(?:to|on)?\s*([a-hA-H])\s*(\d+)\s*";
        private static string _figurePositionPositionPattern = @"\s*(pawn|knight|bishop|rook|queen|king)\s*(?:from)?\s*([a-hA-H])\s*(\d+)\s*(?:to|on)?\s*([a-hA-H])\s*(\d+)\s*";
        private static string _positionPositionPattern = @"\s*(?:from)?\s*([a-hA-H])\s*(\d+)\s*(?:to|on)?\s*([a-hA-H])\s*(\d+)\s*";

        private void Update()
        {
            if (_isRecording && Microphone.GetPosition(null) >= _clip.samples)
            {
                StopRecording();
            }
        }

        public static void StartRecording()
        {
            _clip = Microphone.Start(null, false, 10, 44100);
            _isRecording = true;
            IsGetRequest = false;
        }

        public static void StopRecording()
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

        private static void SendRecording()
        {
            HuggingFaceAPI.AutomaticSpeechRecognition(_bytes, response =>
            {
                RecognizedText = ReplacementOfMistakes(response);
                RecognizedText = PatternAnalyzer(RecognizedText);
                IsGetRequest = true;
            }, error =>
            {
                RecognizedText = "API connection error";
                IsGetRequest = true;
                
            });
        }

        private static byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
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

        private static string ReplacementOfMistakes(string text)
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
                .Replace("powng", "pawn")
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

        private static string PatternAnalyzer(string text)
        {
            Match match;

            match = Regex.Match(text, _figurePositionPositionPattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return CreateMoveParams(match, "FigureFromTo");
            }

            match = Regex.Match(text, _positionPositionPattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return CreateMoveParams(match, "FromTo");
            }

            match = Regex.Match(text, _figurePositionPattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return CreateMoveParams(match, "FigureToPos");
            }

            return "Unrecognized command.\nTry again.";
        }

        private static string CreateMoveParams(Match match, string patternType)
        {
            var moveParams = new FigureMoveParams();

            switch (patternType)
            {
                case "FigureFromTo":
                    moveParams.FigureName = match.Groups[1].Value;
                    moveParams.CurrentPosition = $"{match.Groups[2].Value}{match.Groups[3].Value}";
                    moveParams.NewPosition = $"{match.Groups[4].Value}{match.Groups[5].Value}";
                    break;

                case "FromTo":
                    moveParams.FigureName = "";
                    moveParams.CurrentPosition = $"{match.Groups[1].Value}{match.Groups[2].Value}";
                    moveParams.NewPosition = $"{match.Groups[3].Value}{match.Groups[4].Value}";
                    break;

                case "FigureToPos":
                    moveParams.FigureName = match.Groups[1].Value;
                    moveParams.CurrentPosition = ""; // немає
                    moveParams.NewPosition = $"{match.Groups[2].Value}{match.Groups[3].Value}";
                    break;
            }

            moveParams.TypeOfPattern = patternType;

            LastParsedMove = moveParams;
            OnMoveParsed?.Invoke(moveParams); // 🔸 Виклик події
            return $"{moveParams.FigureName} {moveParams.CurrentPosition} {moveParams.NewPosition}";
        }
    }
}
