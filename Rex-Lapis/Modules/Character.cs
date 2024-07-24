using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Discord.Rest;
using RexLapis.Database;

public class CharacterClass : InteractionModuleBase<SocketInteractionContext>{

    characterMenus menu = new characterMenus();
    DBClass user = new DBClass();
    infoCarrier info = new infoCarrier();

    public void trySave(){
        //Result will carry all existing selections.
        GenshinIdClass userSelections = new GenshinIdClass();

        //Save the corresponding information.
        userSelections.GenshinId = info.Profile;
        userSelections.DiscordId = Context.User.Id.ToString();

        //Load all the selected characters into userSelections
        userSelections.Pyro = info.PyroList;
        userSelections.Hydro = info.HydroList;
        userSelections.Anemo = info.AnemoList;
        userSelections.Electro = info.ElectroList;
        userSelections.Dendro = info.DendroList;
        userSelections.Cryo = info.CryoList;
        userSelections.Geo = info.GeoList;
        
        //Save the results to the database.
        user.GenshinInfo.Update(userSelections);
        user.SaveChanges();

        Global.dict[Context.User.Id.ToString()] = info;
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
    public async Task CharacterCommand(){
        CounterClass.characterCount++;
        Random number = new Random();
        //Characters that users have will be selected in the character menu.
        //nullList will be used for users who haven't registered an account.
        string[] nullList = {
            "Excuse me " + Context.User.GlobalName + ", but you haven't registered with us before.",
            "Ahem, " + Context.User.GlobalName + ", it seems you haven't registered any accounts.",
            "Pardon me " + Context.User.GlobalName + ", but you have no accounts with the Adventurer's Guild.",
            "There seems to be a mistake " + Context.User.GlobalName + ", you haven't added any accounts to our data base."
        };

        //If the user hasn't registered any accounts, they'll be prompted to register.
        if(user.UserInfo.Where(x => x.DiscordId == Context.User.Id.ToString()).Count() == 0){
            string nullChoice = nullList[number.Next(0, nullList.Length)];
            await RespondAsync(nullChoice, ephemeral: true);
            return;
        }        

        SelectMenuBuilder startMenu = new SelectMenuBuilder();
        startMenu.CustomId = "firstMenu";
        startMenu.Placeholder = "Genshin UIDs";

        //Load all known profiles as a list.
        string[] profiles = user.UserInfo.Where(x => x.DiscordId == Context.User.Id.ToString()).ElementAt(0).GenshinId.ToArray();
        //If there's only one profile, then load it immediately.
        // if(profiles.Length == 1){
        //     await leftButtonHandler(profiles[0]);
        // }
        
        //Make a profile selectable.
        for(int i = 0; i < profiles.Length; i++){
            startMenu.AddOption(profiles[i], profiles[i], serverType(profiles[i]));
        }

        //Generate the drop down menus.
        MessageComponent dropDownMenu = new ComponentBuilder().
        WithSelectMenu(startMenu).
        Build();

        await RespondAsync(components: dropDownMenu, ephemeral: true);
    
    }

    [ComponentInteraction("Profile")]
    async Task ProfileMenuHandler(){
        info = Global.dict[Context.User.Id.ToString()];
        //Load all known profiles as a list.
        string[] profiles = user.UserInfo.Where(x => x.DiscordId == Context.User.Id.ToString()).ElementAt(0).GenshinId.ToArray();
        //Make a profile selectable.
        for(int i = 0; i < profiles.Length; i++){
            menu.ProfileMenu.AddOption(profiles[i], profiles[i], serverType(profiles[i]), isDefault: info.Profile == profiles[i]);
        }

        menu.ProfileMenu.CustomId = "firstMenu";

        //Create the menu.
        MessageComponent dropDownMenu = new ComponentBuilder().
        WithSelectMenu(menu.ProfileMenu).
        Build();

        //Present the menu to the user.
        await ((IComponentInteraction) Context.Interaction).UpdateAsync(x => x.Components = dropDownMenu);

    }

    [ComponentInteraction("firstMenu")]
    async Task leftButtonHandler(string input){
        //Update the infoCarrier definition and update the dictionary definition.
        info.Profile = input;
        Global.dict[Context.User.Id.ToString()] = info;

        //Load in all the characters owned by the user.
        info.PyroList = user.GenshinInfo.Where(x => x.GenshinId == input).ElementAt(0).Pyro;
        info.HydroList = user.GenshinInfo.Where(x => x.GenshinId == input).ElementAt(0).Hydro;
        info.AnemoList = user.GenshinInfo.Where(x => x.GenshinId == input).ElementAt(0).Anemo;
        info.DendroList = user.GenshinInfo.Where(x => x.GenshinId == input).ElementAt(0).Dendro;
        info.ElectroList = user.GenshinInfo.Where(x => x.GenshinId == input).ElementAt(0).Electro;
        info.CryoList = user.GenshinInfo.Where(x => x.GenshinId == input).ElementAt(0).Cryo;
        info.GeoList = user.GenshinInfo.Where(x => x.GenshinId == input).ElementAt(0).Geo;

        //Load the relevant characters into each dropdown menu with or without defaults.
        for(int i = 0; i < characterList.charList.Length; i++){
            if(characterList.charList[i].Element == "Pyro"){
                menu.PyroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name, isDefault: Array.Exists(info.PyroList.ToArray(), x=> x == characterList.charList[i].Name));
                menu.PyroMenu.MaxValues += 1;
                continue;
            }
            if(characterList.charList[i].Element == "Hydro"){
                menu.HydroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name, isDefault: Array.Exists(info.HydroList.ToArray(), x=> x == characterList.charList[i].Name));
                menu.HydroMenu.MaxValues += 1;
                continue;
            }
            if(characterList.charList[i].Element == "Anemo"){
                menu.AnemoMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name, isDefault: Array.Exists(info.AnemoList.ToArray(), x=> x == characterList.charList[i].Name));
                menu.AnemoMenu.MaxValues += 1;
                continue;
            }            
        }

        //Create the menu.
        ComponentBuilder dropDownMenu = new ComponentBuilder().
        WithSelectMenu(menu.PyroMenu).
        WithSelectMenu(menu.HydroMenu).
        WithSelectMenu(menu.AnemoMenu).
        WithButton(
            new ButtonBuilder("Previous", customId: "leftButton", isDisabled: true)
        );

        //If the user has more than one profile, then this button becomes available.
        if(info.ProfileCount > 1){
            dropDownMenu.WithButton(
                new ButtonBuilder("Change profiles", "Profile")
            );
        }

        dropDownMenu.WithButton(
            new ButtonBuilder("Next", customId: "rightButton", isDisabled: false)
        );

        MessageComponent finalMessage = dropDownMenu.Build();

        //Present the menu to the user.
        await ((IComponentInteraction) Context.Interaction).UpdateAsync(x => x.Components = finalMessage);

    }

    [ComponentInteraction("leftButton")]
    async Task leftButtonHandler(){
        //Load the context of infoCarrier.
        info = Global.dict[Context.User.Id.ToString()];

        //Load the relevant characters into each dropdown menu with or without defaults.
        for(int i = 0; i < characterList.charList.Length; i++){
            if(characterList.charList[i].Element == "Pyro"){
                menu.PyroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name, isDefault: Array.Exists(info.PyroList.ToArray(), x=> x == characterList.charList[i].Name));
                menu.PyroMenu.MaxValues += 1;
                continue;
            }
            if(characterList.charList[i].Element == "Hydro"){
                menu.HydroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name, isDefault: Array.Exists(info.HydroList.ToArray(), x=> x == characterList.charList[i].Name));
                menu.HydroMenu.MaxValues += 1;
                continue;
            }
            if(characterList.charList[i].Element == "Anemo"){
                menu.AnemoMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name, isDefault: Array.Exists(info.AnemoList.ToArray(), x=> x == characterList.charList[i].Name));
                menu.AnemoMenu.MaxValues += 1;
                continue;
            }
        }

        //Create the menu.
        ComponentBuilder dropDownMenu = new ComponentBuilder().
        WithSelectMenu(menu.PyroMenu).
        WithSelectMenu(menu.HydroMenu).
        WithSelectMenu(menu.AnemoMenu).
        WithButton(
            new ButtonBuilder("Previous", customId: "leftButton", isDisabled: true)
        );

        if(info.ProfileCount > 1){
            dropDownMenu.WithButton(
                new ButtonBuilder("Change profiles", "Profile")
            );
        }

        dropDownMenu.WithButton(
            new ButtonBuilder("Next", customId: "rightButton", isDisabled: false)
        );

        MessageComponent finalMessage = dropDownMenu.Build();

        //Present the menu to the user.
        await ((IComponentInteraction) Context.Interaction).UpdateAsync(x => x.Components = finalMessage);

    }

    [ComponentInteraction("rightButton")]
    async Task rightButtonHandler(){
        //Load in the context of infoCarrier.
        info = Global.dict[Context.User.Id.ToString()];

        //Load all characters into their respective menus.
        for(int i = 0; i < characterList.charList.Length; i++){
            if(characterList.charList[i].Element == "Electro"){
                menu.ElectroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name, isDefault: Array.Exists(info.ElectroList.ToArray(), x=> x == characterList.charList[i].Name));
                menu.ElectroMenu.MaxValues += 1;
                continue;
            }            
            if(characterList.charList[i].Element == "Dendro"){
                menu.DendroMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name, isDefault: Array.Exists(info.DendroList.ToArray(), x=> x == characterList.charList[i].Name));
                menu.DendroMenu.MaxValues += 1;
                continue;
            }            
            if(characterList.charList[i].Element == "Cryo"){
                menu.CryoMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name, isDefault: Array.Exists(info.CryoList.ToArray(), x=> x == characterList.charList[i].Name));
                menu.CryoMenu.MaxValues += 1;
                continue;
            }
            if(characterList.charList[i].Element == "Geo"){
                menu.GeoMenu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name, isDefault: Array.Exists(info.GeoList.ToArray(), x=> x == characterList.charList[i].Name));
                menu.GeoMenu.MaxValues += 1;
                continue;
            }
            continue;
        }

        ComponentBuilder dropDownMenu = new ComponentBuilder().
        WithSelectMenu(menu.ElectroMenu).
        WithSelectMenu(menu.DendroMenu).
        WithSelectMenu(menu.CryoMenu).
        WithSelectMenu(menu.GeoMenu).
        WithButton(
            new ButtonBuilder("Previous", "leftButton", isDisabled: false)
        );

        if(info.ProfileCount > 1){
            dropDownMenu.WithButton(
                new ButtonBuilder("Change profiles", "Profile")
            );
        }

        dropDownMenu.WithButton(
            new ButtonBuilder("Next", customId: "rightButton", isDisabled: true)
        );

        MessageComponent finalMessage = dropDownMenu.Build();

        //Present the menu to the user.
        await ((IComponentInteraction) Context.Interaction).UpdateAsync(x => x.Components = finalMessage);

    }

    //Event for handling the Pyro characters drop down menu event.
    [ComponentInteraction("Pyro")]
    async Task PyroMenuHandler(string[] input){
        info = Global.dict[Context.User.Id.ToString()];
        await DeferAsync();
        info.PyroList = input.ToList();

        trySave();//This function is way too long to just write here.

    }

    //Event for handling the Hydro characters drop down menu event.
    [ComponentInteraction("Hydro")]
    async Task HydroMenuHandler(string[] input){
        info = Global.dict[Context.User.Id.ToString()];
        await DeferAsync();
        info.HydroList = input.ToList();

        trySave();
    }

    //Event for handling the Anemo characters drop down menu event.
    [ComponentInteraction("Anemo")]
    async Task AnemoMenuHandler(string[] input){
        info = Global.dict[Context.User.Id.ToString()];
        await DeferAsync();
        info.AnemoList = input.ToList();

        trySave();
    }

    //Event for handling the Electro characters drop down menu event.
    [ComponentInteraction("Electro")]
    async Task ElectroMenuHandler(string[] input){
        info = Global.dict[Context.User.Id.ToString()];
        await DeferAsync();
        info.ElectroList = input.ToList();

        trySave();
    }

    //Event for handling the Pyro characters drop down menu event.
    [ComponentInteraction("Dendro")]
    async Task DendroMenuHandler(string[] input){
        info = Global.dict[Context.User.Id.ToString()];
        await DeferAsync();
        info.DendroList = input.ToList();

        trySave();
    }

    //Event for handling the Cryo characters drop down menu event.
    [ComponentInteraction("Cryo")]
    async Task CryoMenuHandler(string[] input){
        info = Global.dict[Context.User.Id.ToString()];
        await DeferAsync();
        info.CryoList = input.ToList();

        trySave();
    }

    //Event for handling the Pyro characters drop down menu event.
    [ComponentInteraction("Geo")]
    async Task GeoMenuHandler(string[] input){
        info = Global.dict[Context.User.Id.ToString()];
        await DeferAsync();
        info.GeoList = input.ToList();

        trySave();
    }

}