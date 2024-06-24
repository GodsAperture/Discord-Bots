using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Discord.Rest;

public class CharacterMethod : InteractionModuleBase<SocketInteractionContext>{

    infoCarrier info = new infoCarrier();
    characterMenus menu = new characterMenus();

    public void trySave(){
        //If the file has been set, then check which menus have been edited.
        if(info.FileBool)
            //Check to see if PyroBool has been set to true.
            Console.WriteLine(info.PyroBool);
/*Here*/        if(info.PyroBool){
                    //This process takes all selected Pyro characters and combines them, saving them to the file.
                    string PyroString = "";
                    string[] thisFile = File.ReadAllLines(Path.Combine("Users", Context.User.Id.ToString(), info.File));
                    
/*Here*/            for(int i = 0; i < info.PyroList.Length - 1; i++){
/*Here*/                PyroString += info.PyroList[i] + ",";
                    }
/*Here*/            PyroString += info.PyroList[info.PyroList.Length - 1];

                    //The first element of thisFile is Pyro.
/*Here*/            thisFile[0] = PyroString;

                    Console.WriteLine("Here!");

                    File.WriteAllLines(info.File, thisFile);
                    info.PyroBool = false;
            }
    }
    public void tryLoad(){

    }
    private string serverType(string input){
        switch(input[0]){
            case '0':
                return "HoYo Internal Server";
            case '1':
                return "China";
            case '2':
                return "China";
            case '3':
                return "China";
            case '5':
                return "Xiaomi China";
            case '6':
                return "Americas";
            case '7':
                return "Europe";
            case '8':
                return "Asia/Oceania";
            case '9':
                return "Tw/Hk/Mo";
            default:
                return "???";
        }

    }



    [SlashCommand("character", "Add characters to your currently selected profile.")]
    public async Task CharactersMethod(){
        Random number = new Random();
        //nullList will be used for users who haven't registered an account.
        string[] nullList = {
            "Excuse me " + Context.User.GlobalName + ", but you haven't registered with us before.",
            "Ahem, " + Context.User.GlobalName + ", it seems you haven't registered any accounts.",
            "Pardon me " + Context.User.GlobalName + ", but you have no accounts with the Adventurer's Guild.",
            "There seems to be a mistake " + Context.User.GlobalName + ", you haven't added any accounts to our data base."
        };


        //If the user hasn't registered any accounts, they'll be prompted to register.
        if(!Directory.Exists(Path.Combine("Users", Context.User.Id.ToString()))){
            string nullChoice = nullList[number.Next(0, nullList.Length)];
            await RespondAsync(nullChoice, ephemeral: true);
            return;
        }

        //Load all known profiles as a list.
        string[] profiles = Directory.GetFiles(Path.Combine("Users", Context.User.Id.ToString()));
        //Make a profile selectable.
        for(int i = 0; i < profiles.Length; i++){
            menu.ProfileMenu.AddOption(profiles[i].Split('/')[2].Remove(9), profiles[i].Split('/')[2].Remove(9), serverType(profiles[i].Split('/')[2].Remove(9)));
        }


        //Load all characters into each menu.
        //If the character is owned, it is selected by default.
        //The state of every character will be added/removed based on selection.
        for(int i = 0; i < characterList.charList.Length; i++){
            if(characterList.charList[i].Element == "Pyro"){
                menu.PyroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
                menu.PyroMenu.MaxValues += 1;
                continue;
            }
            if(characterList.charList[i].Element == "Hydro"){
                menu.HydroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
                menu.HydroMenu.MaxValues += 1;
                continue;
            }
            if(characterList.charList[i].Element == "Anemo"){
                menu.AnemoMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
                menu.AnemoMenu.MaxValues += 1;
                continue;
            }            
            // if(characterList.charList[i].Element == "Electo"){
            //     menu.ElectroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
            //     menu.ElectroMenu.MaxValues += 1;
            //     continue;
            // }            
            // if(characterList.charList[i].Element == "Dendro"){
            //     menu.DendroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
            //     menu.DendroMenu.MaxValues += 1;
            //     continue;
            // }            
            // if(characterList.charList[i].Element == "Cryo"){
            //     menu.CryoMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
            //     menu.CryoMenu.MaxValues += 1;
            //     continue;
            // }
            // if(characterList.charList[i].Element == "Geo"){
            //     menu.GeoMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
            //     menu.GeoMenu.MaxValues += 1;
            //     continue;
            // }
            continue;
        }

        //Generate the drop down menus.
        MessageComponent dropDownMenu = new ComponentBuilder().
        WithSelectMenu(menu.ProfileMenu).
        WithSelectMenu(menu.PyroMenu).
        WithSelectMenu(menu.HydroMenu).
        WithSelectMenu(menu.AnemoMenu).
        WithButton(
            new ButtonBuilder("Previous", customId: "leftButton", isDisabled: true)
        ).
        WithButton(
            new ButtonBuilder("Next", customId: "rightButton", isDisabled: false)
        )
        .Build();


        await RespondAsync(components: dropDownMenu, ephemeral: true);
    
    }

    [ComponentInteraction("Profile")]
    async Task ProfileMenuHandler(string[] input){
        await DeferAsync();
        info.FileBool = true;
        info.File = Path.Combine("Users", Context.User.Id.ToString(), input[0] + ".lsv");
        Console.WriteLine(info.File + "\n^^^ File");
        trySave();

    }

    [ComponentInteraction("leftButton")]
    async Task leftButtonHandler(){
        //Load the user profiles.
        string[] profiles = Directory.GetFiles(Path.Combine("Users", Context.User.Id.ToString()));
        for(int i = 0; i < profiles.Length; i++){
            menu.ProfileMenu.AddOption(profiles[i].Split('/')[2].Remove(9), profiles[i].Split('/')[2].Remove(9), serverType(profiles[i].Split('/')[2].Remove(9)));
        }

        //Load the relevant characters.
        for(int i = 0; i < characterList.charList.Length; i++){
            if(characterList.charList[i].Element == "Pyro"){
                menu.PyroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
                menu.PyroMenu.MaxValues += 1;
                continue;
            }
            if(characterList.charList[i].Element == "Hydro"){
                menu.HydroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
                menu.HydroMenu.MaxValues += 1;
                continue;
            }
            if(characterList.charList[i].Element == "Anemo"){
                menu.AnemoMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
                menu.AnemoMenu.MaxValues += 1;
                continue;
            }            
            // if(characterList.charList[i].Element == "Electo"){
            //     menu.ElectroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
            //     menu.ElectroMenu.MaxValues += 1;
            //     continue;
            // }            
            // if(characterList.charList[i].Element == "Dendro"){
            //     menu.DendroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
            //     menu.DendroMenu.MaxValues += 1;
            //     continue;
            // }            
            // if(characterList.charList[i].Element == "Cryo"){
            //     menu.CryoMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
            //     menu.CryoMenu.MaxValues += 1;
            //     continue;
            // }
            // if(characterList.charList[i].Element == "Geo"){
            //     menu.GeoMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
            //     menu.GeoMenu.MaxValues += 1;
            //     continue;
            // }
        }

        MessageComponent dropDownMenu = new ComponentBuilder().
        WithSelectMenu(menu.ProfileMenu).
        WithSelectMenu(menu.PyroMenu).
        WithSelectMenu(menu.HydroMenu).
        WithSelectMenu(menu.AnemoMenu).
        WithButton(
            new ButtonBuilder("Previous", customId: "leftButton", isDisabled: true)
        ).
        WithButton(
            new ButtonBuilder("Next", customId: "rightButton", isDisabled: false)
        ).
        Build();

        await ((IComponentInteraction) Context.Interaction).UpdateAsync(x => x.Components = dropDownMenu);
    }

    [ComponentInteraction("rightButton")]
    async Task rightButtonHandler(){

        for(int i = 0; i < characterList.charList.Length; i++){
            if(characterList.charList[i].Element == "Electro"){
                menu.ElectroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
                menu.ElectroMenu.MaxValues += 1;
                continue;
            }            
            if(characterList.charList[i].Element == "Dendro"){
                menu.DendroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
                menu.DendroMenu.MaxValues += 1;
                continue;
            }            
            if(characterList.charList[i].Element == "Cryo"){
                menu.CryoMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
                menu.CryoMenu.MaxValues += 1;
                continue;
            }
            if(characterList.charList[i].Element == "Geo"){
                menu.GeoMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
                menu.GeoMenu.MaxValues += 1;
                continue;
            }
            continue;
        }

        MessageComponent dropDownMenu = new ComponentBuilder().
        WithSelectMenu(menu.ElectroMenu).
        WithSelectMenu(menu.DendroMenu).
        WithSelectMenu(menu.CryoMenu).
        WithSelectMenu(menu.GeoMenu).
        WithButton(
            new ButtonBuilder("Previous", "leftButton", isDisabled: false)
        ).
        WithButton(
            new ButtonBuilder("Next", "rightButton", isDisabled: true)
        ).
        Build();

        await ((IComponentInteraction) Context.Interaction).UpdateAsync(x => x.Components = dropDownMenu);
    }

    //Event for handling the Pyro characters drop down menu event.
    [ComponentInteraction("Pyro")]
    async Task PyroMenuHandler(string[] input){
        await DeferAsync();
        info.PyroList = input;
        info.PyroBool = true;

        trySave();//This function is way too long to just write here.

    }

    //Event for handling the Hydro characters drop down menu event.
    [ComponentInteraction("Hydro")]
    void HydroMenuHandler(string[] input){
        info.HydroList = input;
        info.HydroBool = true;
    }

    //Event for handling the Anemo characters drop down menu event.
    [ComponentInteraction("Anemo")]
    async Task AnemoMenuHandler(string[] input){
        info.AnemoList = input;
        info.AnemoBool = true;
        await DeferAsync();
    }

    //Event for handling the Electro characters drop down menu event.
    [ComponentInteraction("Electro")]
    void ElectroMenuHandler(string[] input){
        info.ElectroList = input;
        info.ElectroBool = true;
    }

    //Event for handling the Pyro characters drop down menu event.
    [ComponentInteraction("Dendro")]
    void DendroMenuHandler(string[] input){
        info.DendroList = input;
        info.DendroBool = true;
    }

    //Event for handling the Cryo characters drop down menu event.
    [ComponentInteraction("Cryo")]
    void CryoMenuHandler(string[] input){
        info.CryoList = input;
        info.CryoBool = true;
    }

    //Event for handling the Pyro characters drop down menu event.
    [ComponentInteraction("Geo")]
    void GeoMenuHandler(string[] input){
        info.GeoList = input;
        info.GeoBool = true;
    }

}