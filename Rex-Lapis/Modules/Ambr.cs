/*All listed objects are menus, all hyphenated objects are sub menus.
Characters
- Pyro
- Hydro
- Anemo
- Electro
- Dendro
- Cryo
- Geo

Artifacts
- Monstadt
- Liyue
- Inazuma
- Sumeru
- Fontaine
- Natlan
- Snezhnaya

Side Quests
- Monstadt
- Liyue
- Inazuma
- Sumeru
- Fontaine
- Natlan
- Snezhnaya

Weapons will also have another drop down menu to determine the rarity.
Weapons
5* / 4* / 3*
- Sword
- Claymore
- Polearm
- Bow
- Catalyst
*/

using HtmlAgilityPack;

public class AmbrClass : InteractionModuleBase<SocketInteractionContext>{
    private Color getColor(string input){
        if(input == "Pyro"){
            return new Color(236, 73, 35);
        }
        if(input == "Hydro"){
            return new Color(0, 191, 255);
        }
        if(input == "Anemo"){
            return new Color(53, 150, 151);
        }
        if(input == "Electro"){
            return new Color(148, 93, 196);
        }
        if(input == "Dendro"){
            return new Color(96, 138, 0);
        }
        if(input == "Cryo"){
            return new Color(70, 130, 180);
        }
        if(input == "Geo"){
            return new Color(222, 189, 108);
        } else {
            //Idfk what color to put for defaulting to when it comes to Traveler/whatever magical shit HoYo makes.
            return new Color(128, 128, 128);
        }
    }

    [SlashCommand("ambr", "Find information from Genshin Ambr.")]
    public async Task ambrCommand(){
        CounterClass.ambrCount++;
        SelectMenuBuilder firstMenu = new SelectMenuBuilder();

        firstMenu.WithPlaceholder("What would you like to know about?");
        firstMenu.AddOption("Characters", "Characters", "Find info about individual characters, sorted by element.");
        firstMenu.AddOption("Artifacts", "Artifacts", "Find information about each artifact domain, sorted by region.");
        firstMenu.AddOption("Side Quests", "Side Quests", "Find information about side quests, sorted by region.");
        firstMenu.AddOption("Weapons", "Weapons", "Find information about weapons, sorted by rarity and then type.");
        firstMenu.WithCustomId("ambrHandler");

        MessageComponent overallMenu = new ComponentBuilder().
        WithSelectMenu(firstMenu).
        Build();

        await RespondAsync(components: overallMenu, ephemeral: true);
    }

    [ComponentInteraction("ambr")]
    public async Task ambrInteraction(){
        CounterClass.ambrCount++;
        SelectMenuBuilder firstMenu = new SelectMenuBuilder();

        firstMenu.WithPlaceholder("What would you like to know about?");
        firstMenu.AddOption("Characters", "Characters", "Find info about individual characters, sorted by element.");
        firstMenu.AddOption("Artifacts", "Artifacts", "Find information about each artifact domain, sorted by region.");
        firstMenu.AddOption("Side Quests", "Side Quests", "Find information about side quests, sorted by region.");
        firstMenu.AddOption("Weapons", "Weapons", "Find information about weapons, sorted by rarity and then type.");
        firstMenu.WithCustomId("fandomHandler");

        MessageComponent overallMenu = new ComponentBuilder().
        WithSelectMenu(firstMenu).
        Build();

        await ((IComponentInteraction) Context.Interaction).UpdateAsync(x => {x.Components = overallMenu; x.Embed = null;});
    }

    [ComponentInteraction("ambrHandler")]
    public async Task CharactersHandler(string input){
        if(input == "Characters"){
            MessageComponent overallMenu = new ComponentBuilder().
            WithSelectMenu(new SelectMenuBuilder().
            WithCustomId("CharacterHandler").
            WithPlaceholder("Plese, select an element.").
            AddOption("Pyro", "Pyro").
            AddOption("Hydro", "Hydro").
            AddOption("Anemo", "Anemo").
            AddOption("Electro", "Electro").
            AddOption("Dendro", "Dendro").
            AddOption("Cryo", "Cryo").
            AddOption("Geo", "Geo")
            ).
            Build();

            await ((IComponentInteraction) Context.Interaction).UpdateAsync(x => x.Components = overallMenu);
        }
        if(input == "Artifacts"){
            MessageComponent overallMenu = new ComponentBuilder().
            WithSelectMenu(new SelectMenuBuilder().
            WithCustomId("ArtifactHandler").
            WithPlaceholder("Plese, select an artifact set.").
            AddOption("Monstadt", "Monstadt").
            AddOption("Liyue", "Liyue").
            AddOption("Inazuma", "Inazuma").
            AddOption("Sumeru", "Sumeru").
            AddOption("Fontaine", "Fontaine")
            ).
            Build();

            await ((IComponentInteraction) Context.Interaction).UpdateAsync(x => x.Components = overallMenu);
        }
        if(input == "Side Quests"){
            MessageComponent overallMenu = new ComponentBuilder().
            WithSelectMenu(new SelectMenuBuilder().
            WithCustomId("SideQuestsHandler").
            WithPlaceholder("Plese, select a side quest.").
            AddOption("Monstadt", "Monstadt").
            AddOption("Liyue", "Liyue").
            AddOption("Inazuma", "Inazuma").
            AddOption("Sumeru", "Sumeru").
            AddOption("Fontaine", "Fontaine")
            ).
            Build();

            await ((IComponentInteraction) Context.Interaction).UpdateAsync(x => x.Components = overallMenu);
        }
        if(input == "Weapons"){
            MessageComponent overallMenu = new ComponentBuilder().
            WithSelectMenu(new SelectMenuBuilder().
            WithCustomId("WeaponsHandler").
            WithPlaceholder("Plese, select a weapon.").
            AddOption("Swords", "Swords").
            AddOption("Claymores", "Claymores").
            AddOption("Polearms", "Polearms").
            AddOption("Bows", "Bows").
            AddOption("Catalysts", "Catalysts")
            ).
            Build();

            await ((IComponentInteraction) Context.Interaction).UpdateAsync(x => x.Components = overallMenu);
        }
    }

    
////Character handlers
    [ComponentInteraction("CharacterHandler")]
    public async Task CharacterHandlerHandler(string input){
        SelectMenuBuilder Menu = new SelectMenuBuilder();
        Menu.CustomId = "CharacterGetter";
        Menu.Placeholder = "Now, select a character.";

        //Add all characters of the matching element to Menu.
        for(int i = 0; i < characterList.charList.Length; i++){
            if(characterList.charList[i].Element == input){
                Menu.AddOption(characterList.charList[i].Name, characterList.charList[i].Name, characterList.charList[i].Name);
                continue;
            }
        }

        MessageComponent finalMenu = new ComponentBuilder().
        WithSelectMenu(Menu).
        WithButton(new ButtonBuilder("Return to menu", "ambr")).
        Build();
        //Present the finalized menu to the user.
        await ((IComponentInteraction) Context.Interaction).UpdateAsync(x => x.Components = finalMenu);

    }    

    [ComponentInteraction("CharacterGetter")]
    public async Task characterGetter(string input){

        //Open up https://genshin-impact.fandom.com/wiki and grab the associated character's wish splash art.
        string URL = "https://genshin-impact.fandom.com/wiki/";
        HtmlDocument docs = new HtmlWeb().Load(URL);
        HtmlNodeCollection SplashResult = docs.DocumentNode.SelectNodes("/html/body/div/div/div/main/div/div/div/aside/div/div/figure/a/img");
        string splashArt = SplashResult[1].Attributes["src"].Value;

        //Fetch the character's Fandom description.
        HtmlNode DescriptionResult = docs.DocumentNode.SelectSingleNode("/html/body/div[4]/div[4]/div[3]/main/div[3]/div[2]/div/p[3]");
        string DescriptionType = DescriptionResult.InnerText;


        //Take the splashart and then make an embed with it.
        Embed finalEmbed = new EmbedBuilder().
        WithImageUrl(splashArt).
        //WithDescription(constellationType).
        WithTitle(input).
        WithFooter(DescriptionType).
        WithColor(getColor("elementType")).
        Build();

        //Make buttons to return to the main menu and go to the associated character's Fandom page
        MessageComponent finalMenu = new ComponentBuilder().
        WithButton(new ButtonBuilder("Return to menu", "fandom")).
        WithButton(new ButtonBuilder("Fandom - " + input, url: URL){Style = ButtonStyle.Link}).
        Build();

        await ((IComponentInteraction) Context.Interaction).UpdateAsync(x => {x.Embed = finalEmbed; x.Components = finalMenu;});
    }

////Side Quest handlers
    [ComponentInteraction("SideQuestsHandler")]
    public async Task SideQuestsHandler(string input){
        await DeferAsync();
    }

}