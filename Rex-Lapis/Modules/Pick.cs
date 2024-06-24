using System.Collections;
using System.ComponentModel.Design;
using System.Reflection.Metadata;
using Discord.Net.Queue;
using Microsoft.VisualBasic;
using System.IO;
using System.Globalization;

public class PickCommand :  InteractionModuleBase<SocketInteractionContext> {

    private Random number = new Random((int) (DateTime.Now - DateTime.Today).TotalSeconds);


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


    [SlashCommand("pick", "Pick four (or less) random characters you have registered.")]
    public async Task ExecuteCommandAsync(){
        //List of titles Rex Lapis will use to greet the user.
        string[] titleList = {
            "Hello " + Context.User.GlobalName + "!\n\tPlease select a profile:",
            "Welcome " + Context.User.GlobalName + "!\n\tWhich profile are you using?",
            "Greetings " + Context.User.GlobalName + "!\n\tPlease choose one:"
        };
        //In case someone has no accounts, this will prevent any weird edge case behavior.
        string[] nullList = {
            "Excuse me " + Context.User.GlobalName + ", but you haven't registered with us before.",
            "Ahem, " + Context.User.GlobalName + ", it seems you haven't registered any accounts.",
            "Pardon me " + Context.User.GlobalName + ", but you have no accounts with the Adventurer's Guild.",
            "There seems to be a mistake " + Context.User.GlobalName + ", you haven't added any accounts to our data base."
        };

        try{
            
            //Determine all files owned by the user and then have the user pick one file.
            string[] allFiles = Directory.GetFiles(Path.Combine("Users", Context.User.Id.ToString()));
            string[] allProfiles = allFiles;

            //This code will remove the file directory, and the file extensions.
            for(int i = 0; i < allFiles.Length; i++){
                allProfiles[i] = allFiles[i].Split('/')[2].Remove(9);
            }

            //Pick a title.
            string titlePicked = titleList[number.Next(0, titleList.Length)];

            SelectMenuBuilder menu = new SelectMenuBuilder(){
                CustomId = "pick",
                MinValues = 1,
                MaxValues = 1,
                Placeholder = "Genshin UIDs"
            };

            //Add all available files to the options list with server identification.
            for(int i = 0; i < allProfiles.Length; i++){
                menu.AddOption(allProfiles[i], allProfiles[i], description: "UID: " + serverType(allProfiles[i]));
            }

            //Finalize the creation of the drop down menu.
            MessageComponent dropDownMenu = new ComponentBuilder().WithSelectMenu(menu).Build();

            //Send the user the drop down menu and record the choice.
            await RespondAsync(titlePicked, components: dropDownMenu, ephemeral: true);
        } catch(Exception){
            //If there are no registered accounts, Rex Lapis will notify the user.
            string nullChoice = nullList[number.Next(0, nullList.Length)];
            await RespondAsync(nullChoice, ephemeral: true);
            return;
        }
    }

    
    //Afer a user picks a profile, they'll be able to 
    [ComponentInteraction("pick")]
    public async Task MenuHandler(string[] input){
        string[] characters = File.ReadAllLines(Path.Combine("Users", Context.User.Id.ToString(), input[0] + ".csv"));

////TODO: Fix this, so that each line is broken down into a list of characters. 
        //`list` will be the list of characters, and will be sent to the user in the form `first + list + end`.
        string list = "";
        int[] num = new int[4];
        //The first part of the message to be combined with the list of characters.
        string[] firstList = {
            "I would think to cavort with ",
            "Perhaps you should spend some time with ",
            "It would be a good time to meet with ",
            "Today is a nice day to walk with ",
            "I've decided to pick "
        };
        //The end of the message, also to combined with the list of characters.
        string[] lastList = {
            " for the first time in a while.",
            ", don't you think?",
            ".",
            ". Whatever befalls you, stay loyal to them and they will remain loyal to you."
        };

        string first = firstList[number.Next(0, firstList.Length)];
        string last = lastList[number.Next(0, lastList.Length)];

        //Give num four random numbers. I'll use a loop to check that none are the same.
        for(int i = 0; i < 4; i++){
            num[i] = number.Next(0, characters.Length);
        }

        //If any of the numbers are equal, the one that appears later in the list will be changed.
        for(int i = 0; i < 4; i++){
            for(int k = i + 1; k < 4; k++){
                while(num[i] == num[k]){
                    num[k] = number.Next(0, characters.Length);
                }
            }
        }
        

        //Now to combine the characters into a string separated with commas.
        for(int i = 0; i < 3; i++){
            list += characters[num[i]] + ", ";
        }
        //Add the last character without the ", " at the end.
        list += "and " + characters[num[3]];

        await RespondAsync(first + list + last, ephemeral: true);

    }

}