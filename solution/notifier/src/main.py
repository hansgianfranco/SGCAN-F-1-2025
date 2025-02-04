from fastapi import FastAPI
import smtplib

app = FastAPI()

@app.post("/notify")
async def notify(email: str):
    server = smtplib.SMTP('smtp.gmail.com', 587)
    server.starttls()
    server.login("hansgianfranco@gmail.com", "")
    server.sendmail("hansgianfranco@gmail.com", email, "Su archivo ha sido procesado.")
    server.quit()
    return {"status": "notification sent"}