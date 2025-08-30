If you have a problem accessing the HugginFace API, follow these steps for local deployment:
- install Python (optional packages: transformers, torch, soundfile)
- install ffmpeg to work with text files (
pip install ffmpeg-python)
- install the openai-whisper package (
pip install openai-whisper==20230314 or 
pip install fastapi uvicorn openai-whisper)
- download the library from the repository.
When you start working, open the command line to the location of the whisper_server.py file and run the command: 
uvicorn whisper_server:app --host 0.0.0.0 --port 8000
You can change the port if necessary. In the code in the SendRecording method of the SpeechToText script, change the expected URL.