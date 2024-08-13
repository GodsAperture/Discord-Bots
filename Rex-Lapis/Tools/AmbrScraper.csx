using System.Net.Http;



//Helper function. This will help me determine what element a character is.
//I don't know why, but HoYo uses words other than what's in game-
//to describe everything they have personally written in English.
//Why?? It's so confusing.
string wElement(string input){
    if(input == "Fire"){
        return "Pyro";
    }
    if(input == "Water"){
        return "Hydro";
    }
    if(input == "Wind"){
        return "Anemo";
    }
    if(input == "Electric"){
        return "Electro";
    }
    if(input == "Grass"){
        return "Dendro";
    }
    if(input == "Ice"){
        return "Cryo";
    }
    if(input == "Rock"){
        return "Geo";
    }
    else{
        return "???";
    }
}

//This class only exists to make JSON deserialization easier.
public class AMBRCharacter{
    private class data{

    }

    //This will help with file generation.
    //I'll be able to use all characters anywhere with all relevant info.
    override public string ToString(string[] inConstellations, string[] inGreetings){
        return "new AMBRCharacter(){" +
            name + ";\n\t" +
            card + ";\n\t" +
            icon + ";\n\t" +
            wElement(element) + ";\n\t" +
            weaponType + ";\n\t" +
            region + ";\n\t" + 
            "[]" + 
        "}\n\n"
    }
}

async Task characterScraper(){
    //Open the character's portion of Ambr.
    HttpClient web = new HttpClient();
    HttpResponseMessage response = await web.GetAsync("https://api.ambr.top/v2/en/avatar");

    //Check to see if it work. If it did, then continue.
    if(response.IsSuccessStatusCode){
        string result = await response.Content.ReadAsStringAsync();
        JsonConvert

        return;
    } else {
        Console.WriteLine("Something went wrong, check your code.");
        return;
    }


}

await characterScraper();