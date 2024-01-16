import asyncio
import genshin

async def getCharacters():
    cookies = {"ltuid": 119480035, "ltoken": "cnF7TiZqHAAvYqgCBoSPx5EjwezOh1ZHoqSHf7dT"}
    client = genshin.Client(cookies)

    data = await client.get_genshin_user(643390146)
    return str(data.characters)