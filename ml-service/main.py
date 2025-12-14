from fastapi import FastAPI, File, UploadFile
import tensorflow as tf
import numpy as np
from PIL import Image
import io

# -----------------------------
# App
# -----------------------------
app = FastAPI(title="Diet AI - ML Service")

# -----------------------------
# Model yükle
# -----------------------------
MODEL_PATH = "food_classifier_SERVE (1).keras"


model = tf.keras.models.load_model(
    MODEL_PATH,
    compile=False
)

# -----------------------------
# SINIFLAR (EĞİTİMDEKİ GERÇEK SINIFLAR)
# -----------------------------
CLASS_NAMES = [
    "armut",
    "atistirmalik",
    "balik",
    "bulgur",
    "cikolatali_pasta",
    "cilek",
    "corba",
    "ekmek",
    "kiraz",
    "kivi",
    "makarna",
    "manti",
    "pilav",
    "portakal",
    "salata",
    "sebze_yemegi",
    "tavuk",
    "yesil_elma",
    "yumurta"
]

# -----------------------------
# SABİT GRAM & KALORİ TABLOLARI
# -----------------------------
GRAM_TABLE = {
    "armut": 150,
    "atistirmalik": 50,
    "balik": 200,
    "bulgur": 180,
    "cikolatali_pasta": 120,
    "cilek": 150,
    "corba": 250,
    "ekmek": 50,
    "kiraz": 150,
    "kivi": 140,
    "makarna": 200,
    "manti": 180,
    "pilav": 180,
    "portakal": 180,
    "salata": 120,
    "sebze_yemegi": 200,
    "tavuk": 150,
    "yesil_elma": 150,
    "yumurta": 60
}

CALORIE_PER_100G = {
    "armut": 52,
    "atistirmalik": 500,
    "balik": 206,
    "bulgur": 83,
    "cikolatali_pasta": 389,
    "cilek": 32,
    "corba": 60,
    "ekmek": 265,
    "kiraz": 63,
    "kivi": 61,
    "makarna": 158,
    "manti": 250,
    "pilav": 130,
    "portakal": 47,
    "salata": 40,
    "sebze_yemegi": 90,
    "tavuk": 165,
    "yesil_elma": 52,
    "yumurta": 155
}

# -----------------------------
# Yardımcı fonksiyonlar
# -----------------------------
def preprocess_image(image: Image.Image):
    image = image.resize((224, 224))
    image = np.array(image) / 255.0
    image = np.expand_dims(image, axis=0)
    return image

def calculate_nutrition(food_name: str):
    gram = GRAM_TABLE.get(food_name, 0)
    kcal_100 = CALORIE_PER_100G.get(food_name, 0)
    total_kcal = gram * kcal_100 / 100

    return {
        "food": food_name,
        "estimated_gram": gram,
        "estimated_calorie": round(total_kcal, 1)
    }

# -----------------------------
# ENDPOINTLER
# -----------------------------
@app.get("/health")
def health_check():
    return {"status": "ok"}

@app.post("/analyze-meal")
async def analyze_meal(file: UploadFile = File(...)):
    image_bytes = await file.read()
    image = Image.open(io.BytesIO(image_bytes)).convert("RGB")

    processed = preprocess_image(image)
    predictions = model.predict(processed)
    class_index = int(np.argmax(predictions))
    predicted_food = CLASS_NAMES[class_index]
    confidence = float(np.max(predictions))

    nutrition = calculate_nutrition(predicted_food)

    return {
        "predicted_food": predicted_food,
        "confidence": round(confidence, 3),
        "nutrition": nutrition
    }
