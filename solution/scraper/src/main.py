from fastapi import FastAPI
import redis
import requests
from bs4 import BeautifulSoup
from pydantic import BaseModel
from typing import List, Dict

app = FastAPI()
redis_client = redis.Redis(host='f1_redis', port=6379, db=0)

class LinkRequest(BaseModel):
    links: List[str]
    id: str 

class LinkContentResponse(BaseModel):
    id: str
    content: str

@app.post("/process")
async def process_file(request: LinkRequest):
    links = request.links
    id = request.id
    
    processed_links: List[LinkContentResponse] = []

    for link in links:
        response = requests.get(link)
        
        if response.status_code == 200:
            soup = BeautifulSoup(response.content, 'html.parser')
            content = str(soup)

            processed_links.append(LinkContentResponse(id=id, content=content))
        else:
            processed_links.append(LinkContentResponse(id=id, content=f"Error: {response.status_code}"))

    return {"status": "processed", "processed_links": processed_links}