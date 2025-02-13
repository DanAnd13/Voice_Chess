using UnityEngine;
using Unity.Sentis;
using System.IO;
using System.Collections.Generic;

namespace VoiceChess.Speaking
{
    public class TextToSpeech : MonoBehaviour
    {
        public string InputText = "Once upon a time, there lived a girl called Alice. She lived in a house in the woods.";

        //Set to true if we have put the phoneme_dict.txt in the Assets/StreamingAssets folder
        private bool _hasPhonemeDictionary = true;

        private Dictionary<string, string> _dictionary = new();

        private Worker _engine;

        private AudioClip _clip;

        private readonly string[] _phonemes = new string[] {
        "<blank>", "<unk>", "AH0", "N", "T", "D", "S", "R", "L", "DH", "K", "Z", "IH1",
        "IH0", "M", "EH1", "W", "P", "AE1", "AH1", "V", "ER0", "F", ",", "AA1", "B",
        "HH", "IY1", "UW1", "IY0", "AO1", "EY1", "AY1", ".", "OW1", "SH", "NG", "G",
        "ER1", "CH", "JH", "Y", "AW1", "TH", "UH1", "EH2", "OW0", "EY2", "AO0", "IH2",
        "AE2", "AY2", "AA2", "UW0", "EH0", "OY1", "EY0", "AO2", "ZH", "OW2", "AE0", "UW2",
        "AH2", "AY0", "IY2", "AW2", "AA0", "\"", "ER2", "UH2", "?", "OY2", "!", "AW0",
        "UH0", "OY0", "..", "<sos/eos>" };

        //Can change pitch and speed with this for a slightly different voice:
        const int Constant_Samplerate = 22050;

        void Start()
        {
            LoadModel();
            ReadDictionary();
            SpeakingByText();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpeakingByText();
            }
        }

        private void OnDestroy()
        {
            _engine?.Dispose();
        }

        private void LoadModel()
        {
            var model = ModelLoader.Load(Path.Join(Application.streamingAssetsPath, "jets-text-to-speech.sentis"));
            _engine = new Worker(model, BackendType.GPUCompute);
        }

        private void ReadDictionary()
        {
            if (!_hasPhonemeDictionary) return;
            string[] wordsFromPhonemeDictionary = File.ReadAllLines(Path.Join(Application.streamingAssetsPath, "phoneme_dict.txt"));
            for (int i = 0; i < wordsFromPhonemeDictionary.Length; i++)
            {
                string s = wordsFromPhonemeDictionary[i];
                string[] parts = s.Split();
                if (parts[0] != ";;;") //ignore comments in file
                {
                    string key = parts[0];
                    _dictionary.Add(key, s.Substring(key.Length + 2));
                }
            }
            // Add codes for punctuation to the dictionary
            _dictionary.Add(",", ",");
            _dictionary.Add(".", ".");
            _dictionary.Add("!", "!");
            _dictionary.Add("?", "?");
            _dictionary.Add("\"", "\"");
            // You could add extra word pronounciations here e.g.
            //dict.Add("somenewword","[phonemes]");
        }

        private void SpeakingByText()
        {
            string phonemeText;
            if (_hasPhonemeDictionary)
            {
                phonemeText = TextToPhonemes(InputText);
                Debug.Log(phonemeText);
            }
            else
            {
                //If we have no phenome dictionary
                Debug.Log("Have no phenome dictionary");
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

        //Decode the word into phenomes by looking for the longest word in the dictionary that matches
        //the first part of the word and so on. 
        //This works fairly well but could be improved. The original paper had a model that
        //dealt with guessing the phonemes of words
        private string DecodeWord(string word)
        {
            string output = "";
            int start = 0;
            for (int end = word.Length; end >= 0 && start < word.Length; end--)
            {
                if (end <= start) //no matches
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

            Debug.Log($"Audio size = {samples.Length / Constant_Samplerate} seconds");

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
                Debug.Log("There is no audio source");
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
