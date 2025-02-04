from fastapi import FastAPI
import redis
import requests
from bs4 import BeautifulSoup

app = FastAPI()
redis_client = redis.Redis(host='redis', port=6379, db=0)

@app.post("/process")
async def process_file(file_id: str):
    links = ["http://example.com"]
    for link in links:
        response = requests.get(link)
        soup = BeautifulSoup(response.content, 'html.parser')
    return {"status": "processed"}

@app.get("/items/{item_id}")
def read_item(item_id: int, q: str = None):
    return {"item_id": item_id, "q": q}