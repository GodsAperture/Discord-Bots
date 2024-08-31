public class HelpClass : InteractionModuleBase<SocketInteractionContext>{

    //"Random" number generator to pick arbitrary numbers for the user.
    Emoji geoEmote = new Emoji("<:Geo:1273048447179558975>");

    [SlashCommand("help", "Rex Lapis describes all current commands.")]
    public async Task HelpCommand(){
        CounterClass.helpCount++;
        //Create a drop down menu for the user to select what they need help with.
        SelectMenuBuilder thisMenu = new SelectMenuBuilder();
        thisMenu.CustomId = "HelperMenu";
        thisMenu.Placeholder = "What would you like help with?";
        thisMenu.AddOption("Boss", "Boss", "Rex Lapis suggest a set of bosses to fight against.");
        thisMenu.AddOption("Card", "Card", "Rex Lapis gives you a prompt for the day.");
        thisMenu.AddOption("Counter", "Counter", "Very basic data on how many times a command has been used.");
        thisMenu.AddOption("Character", "Character", "Add characters to your currently selected profile.");
        thisMenu.AddOption("Event", "Events", "Create, delete, update, and announce mock events.");
        thisMenu.AddOption("Random", "Random", "Pick up to four random characters you have registered.");
        thisMenu.AddOption("Register", "Register", "Associate a Genshin UID with your Discord account.");
        thisMenu.AddOption("Travel", "Travel", "Rex Lapis prompts the user with a journey.");
        thisMenu.AddOption("Version", "Version", "Tells the user the current version and most recent updates.");
        thisMenu.AddOption("Bugs/Betas/Suggestions", "Other", "This will send you a link to the server for support.");

        MessageComponent finalMenu = new ComponentBuilder().WithSelectMenu(thisMenu).Build();

        await RespondAsync(components: finalMenu, ephemeral: true);

    }

    [ComponentInteraction("HelperMenu")]
    public async Task helperMethod(string input){
        await DeferAsync();

        string[] End = [
            "\n\tI sincerely hope that this was of utmost help to you.\n\t\tSigned, `Rex Lapis` " + geoEmote,
            "\n\tWe, at Liyue Harbor, are all here to be of support to you.\n\t\tSigned, `Rex Lapis` " + geoEmote,
            "\n\tEvery journey has its final day. Don't rush.\n\t\tSigned, `Rex Lapis` " + geoEmote,
            "\n\tPlease, enjoy your time at the harbor as we all have.\n\t\tSigned, `Rex Lapis` " + geoEmote
        ];

        if(input == "Boss"){
            string ending = Global.picker(End); 
            string statement = "```Rex Lapis will provide the user with a list of bosses to fight. You are meant to fight all of these bosses in the order they are given.\nRules:\n\n- You may only revive/heal through food and team mate abilities (such as C6 Qiqi).\n- Every sequence starts with three normal bosses and ends with a trounce boss.\n-  Players that die must wait for the next sequence to be defeated to rejoin the party.\n- You may swap characters between sequences.\n- You may heal between sequences at a statue of the Seven.\n- Resurrection due to Narwhal phase transition is acceptable.\n- If you all die, then you may either restart that sequence from the beginning or choose a new one.\n\nThis mode is better with friends.\nPlan accordingly, good luck, and have fun with your boss rush!```";
            await ((IComponentInteraction) Context.Interaction).ModifyOriginalResponseAsync(x => x.Content = statement + ending);
            return;
        }        
        if(input == "Card"){
            string ending = Global.picker(End);
            string statement = "```Rex Lapis chooses three cards, each with a prompt for the user to do for that day in Genshin. Cards change daily and some prompts have sub-prompts that also change daily too.```";
            await ((IComponentInteraction) Context.Interaction).ModifyOriginalResponseAsync(x => x.Content = statement + ending);
            return;
        }
        if(input == "Counter"){
            string ending = Global.picker(End);
            string statement = "```This command provides the number of times a command has been used. This command is more useful for the developer than anyone else.```";
            await ((IComponentInteraction) Context.Interaction).ModifyOriginalResponseAsync(x => x.Content = statement + ending);
            return;
        }
        if(input == "Character"){
            string ending = Global.picker(End);
            string statement = "```When you want to add or remove your favorite characters from the pool of characters suggested by Rex Lapis, use the character slash command.\nTo add a character, select them and then a check mark will appear by their name. To remove a character, select them again so that their check mark goes away. The rest is handled by Rex Lapis.```";
            await ((IComponentInteraction) Context.Interaction).ModifyOriginalResponseAsync(x => x.Content = statement + ending);
            return;
        }  
        if(input == "Events"){
            string ending = Global.picker(End);
            string statement = "```Host mock events that can be open to everyone or exclusive to particular roles. Events will automatically remove members when they are picked.\n\nAnnouncing an event will announce it in the channel the bot was called in.```";
            await ((IComponentInteraction) Context.Interaction).ModifyOriginalResponseAsync(x => x.Content = statement + ending);
            return;
        }
        if(input == "Random"){
            string ending = Global.picker(End);
            string statement = "```If you would like a team of characters to be randomly selected from your pool of characters, Rex Lapis will randomly pick four characters. There is no promise of a sensible team from this.\nThis is to primarily unbench your old favorite characers again.```";
            await ((IComponentInteraction) Context.Interaction).ModifyOriginalResponseAsync(x => x.Content = statement + ending);
            return;
        }
        if(input == "Register"){
            string ending = Global.picker(End);
            string statement = "```In order to be able to use various commands provided by Rex Lapis, you must first register a Genshin UID.\nOnce you have given Rex Lapis a UID, it will be associated with your Discord account. You may register more than one Genshin UID with your Discord account.```";
            await ((IComponentInteraction) Context.Interaction).ModifyOriginalResponseAsync(x => x.Content = statement + ending);
            return;
        }
        if(input == "Travel")
        {
            string ending = Global.picker(End);
            string statement = "```Whenever you find there's nothing else to do in Genshin, maybe you and some friends can take a journery across Teyvat together. Rex Lapis will always pick a route that won't require teleporting. This slash command assumes you have Enkanomiya and the Chasm both fully unlocked.```";
            await ((IComponentInteraction) Context.Interaction).ModifyOriginalResponseAsync(x => x.Content = statement + ending);
            return;
        }
        if(input == "Version"){
            string ending = Global.picker(End);
            string statement = "```This tells the user the current version of the bot and any new, relevant updates. As always, if you see a new command, check under /help for more information.```";
            await ((IComponentInteraction) Context.Interaction).ModifyOriginalResponseAsync(x => x.Content = statement + ending);
            return;
        }
        if(input == "Other"){
            string ending = Global.picker(End);
            string statement = "```Found a bug, want to suggest a feature, or want to beta test Rex Lapis? Join Geo Mains, because I'm not making a server dedicated to the bot.```\n https://discord.gg/MVaJE9rKfH";
            await ((IComponentInteraction) Context.Interaction).ModifyOriginalResponseAsync(x => x.Content = statement + ending);
        }

    }

}