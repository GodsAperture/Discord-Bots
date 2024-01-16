from flask import Flask
import asyncio
from Main import getCharacters
import json

app = Flask(__name__)

@app.route("/")
def GeshinPythonProgram():
    return asyncio.run(getCharacters())