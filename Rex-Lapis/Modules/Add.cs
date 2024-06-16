public class CharacterMethod : InteractionModuleBase<SocketInteractionContext>{
    [SlashCommand("character", "Add characters to your currently selected profile.")]
    public async Task CharactersMethod(){

        //Idfk, I'm still working on the logic flow.



        //Pyro characters
        SelectMenuBuilder PyroMenu = new SelectMenuBuilder(){
            CustomId = "Pyro",
            MinValues = 0,
            MaxValues = 1,
            Placeholder = "Pyro Characters"
        };

        //Hydro characters
        SelectMenuBuilder HydroMenu = new SelectMenuBuilder(){
            CustomId = "Hydro",
            MinValues = 0,
            MaxValues = 1,
            Placeholder = "Hydro Characters"
        };

        //Anemo characters
        SelectMenuBuilder AnemoMenu = new SelectMenuBuilder(){
            CustomId = "Anemo",
            MinValues = 0,
            MaxValues = 1,
            Placeholder = "Anemo Characters"
        };

        //Electro characters
        SelectMenuBuilder ElectroMenu = new SelectMenuBuilder(){
            CustomId = "Electro",
            MinValues = 0,
            MaxValues = 1,
            Placeholder = "Electro Characters"
        };

        //Dendro characters
        SelectMenuBuilder DendroMenu = new SelectMenuBuilder(){
            CustomId = "Dendro",
            MinValues = 0,
            MaxValues = 1,
            Placeholder = "Dendro Characters"
        };

        //Cryo characters
        SelectMenuBuilder CryoMenu = new SelectMenuBuilder(){
            CustomId = "Cryo",
            MinValues = 0,
            MaxValues = 1,
            Placeholder = "Cryo Characters"
        };

        //Geo characters
        SelectMenuBuilder GeoMenu = new SelectMenuBuilder(){
            CustomId = "Geo",
            MinValues = 0,
            MaxValues = 1,
            Placeholder = "Geo Characters"
        };

        //Profile char-oh wait, no, profile menu.
        SelectMenuBuilder ProfileMenu = new SelectMenuBuilder(){
            CustomId = "Profile",
            MinValues = 1,
            MaxValues = 1,
            Placeholder = "Genshin UIDs"
        };
    }

    //Event for handling the Pyro characters drop down menu event.
    [ComponentInteraction("Pyro")]
    string[] PyroMenuHandler(string[] input){
        string[] PyroList = [];

        return PyroList;
    }

    //Event for handling the Hydro characters drop down menu event.
    [ComponentInteraction("Hydro")]
    string[] HydroMenuHandler(string[] input){
        string[] HydroList = [];
    
        return HydroList;
    }

    //Event for handling the Anemo characters drop down menu event.
    [ComponentInteraction("Anemo")]
    string[] AnemoMenuHandler(string[] input){
        string[] AnemoList = [];

        return AnemoList;
    }

    //Event for handling the Electro characters drop down menu event.
    [ComponentInteraction("Electro")]
    string[] ElectroMenuHandler(string[] input){
        string[] ElectroList = [];
    
        return ElectroList;
    }

    //Event for handling the Pyro characters drop down menu event.
    [ComponentInteraction("Dendro")]
    string[] DendroMenuHandler(string[] input){
        string[] DendroList = [];
    
        return DendroList;
    }

    //Event for handling the Cryo characters drop down menu event.
    [ComponentInteraction("Cryo")]
    string[] CryoMenuHandler(string[] input){
        string[] CryoList = [];
    
        return CryoList;
    }

    //Event for handling the Pyro characters drop down menu event.
    [ComponentInteraction("Geo")]
    string[] GeoMenuHandler(string[] input){
        string[] GeoList = [];
    
        return GeoList;
    }

}