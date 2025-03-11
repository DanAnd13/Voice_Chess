using UnityEngine;
using Unity.Sentis;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Profiling;

namespace VoiceChess.Speaking
{
    public class TextToSpeech : MonoBehaviour
    {
        // Змінна для отримання тексту з Editor Script
        private string inputText = "Default text.";

        private bool _hasPhonemeDictionary = true;
        private Dictionary<string, string> _dictionary = new();
        private Worker _engine;
        private AudioClip _clip;
        private Stopwatch stopwatch = new Stopwatch();

        private readonly string[] _phonemes = new string[]
        {
            "<blank>", "<unk>", "AH0", "N", "T", "D", "S", "R", "L", "DH", "K", "Z", "IH1",
            "IH0", "M", "EH1", "W", "P", "AE1", "AH1", "V", "ER0", "F", ",", "AA1", "B",
            "HH", "IY1", "UW1", "IY0", "AO1", "EY1", "AY1", ".", "OW1", "SH", "NG", "G",
            "ER1", "CH", "JH", "Y", "AW1", "TH", "UH1", "EH2", "OW0", "EY2", "AO0", "IH2",
            "AE2", "AY2", "AA2", "UW0", "EH0", "OY1", "EY0", "AO2", "ZH", "OW2", "AE0", "UW2",
            "AH2", "AY0", "IY2", "AW2", "AA0", "\"", "ER2", "UH2", "?", "OY2", "!", "AW0",
            "UH0", "OY0", "..", "<sos/eos>"
        };

        const int Constant_Samplerate = 22050;

        private void Awake()
        {
            LoadModel();
            ReadDictionary();
        }

        private void OnDestroy()
        {
            _engine?.Dispose();
        }

        public void SetTextAndSpeak(string text)
        {
            inputText = text;
            PlayText();
        }

        private void PlayText()
        {
            stopwatch.Reset();
            stopwatch.Start();

            SpeakingByText();

            stopwatch.Stop();
            UnityEngine.Debug.Log($"Total initialization time: {stopwatch.ElapsedMilliseconds} ms");
        }

        private void LoadModel()
        {
            Profiler.BeginSample("Load Sentis Model");
            var model = ModelLoader.Load(Path.Join(Application.streamingAssetsPath, "jets-text-to-speech.sentis"));
            _engine = new Worker(model, BackendType.GPUCompute);
            Profiler.EndSample();
        }

        private void ReadDictionary()
        {
            if (!_hasPhonemeDictionary || _dictionary.Count > 0) return;

            string[] wordsFromPhonemeDictionary = File.ReadAllLines(Path.Join(Application.streamingAssetsPath, "phoneme_dict.txt"));
            foreach (string s in wordsFromPhonemeDictionary)
            {
                string[] parts = s.Split();
                if (parts[0] != ";;;")
                {
                    string key = parts[0];
                    _dictionary.TryAdd(key, s.Substring(key.Length + 2));
                }
            }

            _dictionary.TryAdd(",", ",");
            _dictionary.TryAdd(".", ".");
            _dictionary.TryAdd("!", "!");
            _dictionary.TryAdd("?", "?");
            _dictionary.TryAdd("\"", "\"");
        }

        private void SpeakingByText()
        {
            string phonemeText;
            if (_hasPhonemeDictionary)
            {
                phonemeText = TextToPhonemes(inputText);
                UnityEngine.Debug.Log(phonemeText);
            }
            else
            {
                UnityEngine.Debug.Log("Have no phoneme dictionary");
                phonemeText = null;
            }
            DoInference(phonemeText);
        }

        private string TextToPhonemes(string text)
        {
            string outputText = "";
            text = ExpandNumbers(text).ToUpper();

            string[] splitWords = text.Split();
            for (int i = 0; i < splitWords.Length; i++)
            {
                outputText += DecodeWord(splitWords[i]);
            }
            return outputText;
        }

        private string DecodeWord(string word)
        {
            string output = "";
            int start = 0;
            for (int end = word.Length; end >= 0 && start < word.Length; end--)
            {
                if (end <= start)
                {
                    start++;
                    end = word.Length + 1;
                    continue;
                }
                string subword = word.Substring(start, end - start);
                if (_dictionary.TryGetValue(subword, out string value))
                {
                    output += value + " ";
                    start = end;
                    end = word.Length + 1;
                }
            }
            return output;
        }

        private void DoInference(string phonemeText)
        {
            int[] tokens = GetTokens(phonemeText);
            using var input = new Tensor<int>(new TensorShape(tokens.Length), tokens);
            _engine.Schedule(input);

            var output = _engine.PeekOutput("wav") as Tensor<float>;
            var samples = output.DownloadToArray();

            UnityEngine.Debug.Log($"Audio size = {samples.Length / Constant_Samplerate} seconds");

            _clip = AudioClip.Create("voice audio", samples.Length, 1, Constant_Samplerate, false);
            _clip.SetData(samples, 0);

            Speak();
        }

        private int[] GetTokens(string phonemeText)
        {
            string[] words = phonemeText.Split();
            var tokens = new int[words.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                tokens[i] = Mathf.Max(0, System.Array.IndexOf(_phonemes, words[i]));
            }
            return tokens;
        }

        private void Speak()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.clip = _clip;
                audioSource.Play();
            }
            else
            {
                UnityEngine.Debug.Log("There is no audio source");
            }
        }

        private string ExpandNumbers(string text)
        {
            return text
                .Replace("0", " ZERO ")
                .Replace("1", " ONE ")
                .Replace("2", " TWO ")
                .Replace("3", " THREE ")
                .Replace("4", " FOUR ")
                .Replace("5", " FIVE ")
                .Replace("6", " SIX ")
                .Replace("7", " SEVEN ")
                .Replace("8", " EIGHT ")
                .Replace("9", " NINE ");
        }
    }
}
