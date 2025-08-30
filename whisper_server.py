from fastapi import FastAPI, UploadFile, File
from transformers import pipeline

import soundfile as sf
import tempfile

app = FastAPI()

# Завантаження моделі
asr = pipeline("automatic-speech-recognition", model="openai/whisper-tiny")

@app.post("/transcribe/")
async def transcribe_audio(file: UploadFile = File(...)):
    try:
        contents = await file.read()

        with tempfile.NamedTemporaryFile(suffix=".wav", delete=False) as temp_audio:
            temp_audio.write(contents)
            temp_audio_path = temp_audio.name

        result = asr(temp_audio_path)
        return {"text": result["text"]}

    except Exception as e:
        return {"error": str(e)}
