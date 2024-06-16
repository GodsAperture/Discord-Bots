public class RegisterCommand : InteractionModuleBase<SocketInteractionContext>{
    //This function will let users submit their Genshin UID.

    [SlashCommand("register", "Associate a Genshin UID with your Discord account.")]
    public async Task ExecuteCommandAsync([Summary("GenshinUID", "Associate a Genshin UID with your Discord account.")] string GenshinUID){
        
        try{
            //The Discord UID will be their own personal file directory.
            ulong UID = Context.User.Id;
            Random number = new Random((int) (DateTime.Now - DateTime.Today).TotalSeconds);

            string[] finisher = 
            {"\n\n\tShow your friends you love them on the journey,\n\t\tSigned, `Rex Lapis` ",
            "\n\n\tEnjoy the journey with more company,\n\t\tSigned, `Rex Lapis` ",
            "\n\n\tSight-seeing is always best with friends,\n\t\tSigned, `Rex Lapis` ",
            "\n\n\tTake your time, enjoy the scenery with your friends,\n\t\tSigned, `Rex Lapis` "};

            string final = finisher[number.Next(0, finisher.Length)];

            Emoji geoEmote = new Emoji("<:Geo:1158853006364774491>");

            string[] sorryResponse = {
                "Sorry, but it what you gave isn't a UID.\nAn example of an acceptable UID is 643390146.",
                "A Genshin UID is always a number. There are no alphabetical letters in it.\nAn example is 643390146.",
                "I'm sorry " + Context.User.GlobalName + ", but this isn't an acceptable ID.\n643390146 is a valid ID, however " + GenshinUID + " is not."
            };

            //So...out is just the keyword to do C++'s version of pass by reference. Nice.
            //Why though???

            //If the user gives a bad argument, I just tell them to fuck off and give a legit ID.

            if(!ulong.TryParse(GenshinUID, out ulong GUID) || GenshinUID.Length != 9){
                await RespondAsync(sorryResponse[number.Next(0, sorryResponse.Length)], ephemeral: true);
                return;
            }



            string folderPath = Path.Combine("Users", UID.ToString());

            //If the folder for the user doesn't exist then create one.
            if(!Directory.Exists(folderPath)){
                Directory.CreateDirectory(folderPath);
            }


            //In case the user gives a GUID they already have, they don't overwrite their previous stuff by mistake.
            string[] ExistenceResponse = {
                "Excuse me " + Context.User.GlobalName + ", but it seems you've already added the account " + GUID + ".",
                "Forgive me " + Context.User.GlobalName + ", but I checked twice and " + GUID + " has already been added to your account.",
                "Greetings " + Context.User.GlobalName + ", I apologize but it seems you've already claimed " + GUID + " as part of your account."
            };

            //Otherwise, it will create a file for the user with the Genshin UID being the file name.
            string[] SuccessResponse = {
                "I have successfully completed the paper work to associate " + GUID + " with you, " + Context.User.GlobalName + "!",
                "I have finished setting up the necessary work, " + GUID + " is now tied to you, " + Context.User.GlobalName + "!",
                "The work has been finished, " + GUID + " is now yours to work with from here on out, " + Context.User.GlobalName + "."
            };


            string filePath = Path.Combine(folderPath, GUID.ToString() + ".csv");

            //If the file using that UID already exists, then notify the user.
            //Otherwise, generate an empty file named after the GUID.
            if(File.Exists(filePath)){
                await RespondAsync(ExistenceResponse[number.Next(0, ExistenceResponse.Length)], ephemeral: true);
                return;
            } else {
                //Apparently creating the file doesn't just create it. It opens it. Today I learned.
                
                File.Create(filePath).Close();
                await RespondAsync(SuccessResponse[number.Next(0, SuccessResponse.Length)] + final + geoEmote, ephemeral: true);

                return;
            }

        }
        catch(Exception e){
            await RespondAsync("Hmmm, it seems an error has occured. Please notify GodsAperture of this.\n\n" + e.Message, ephemeral: true);
            return;
        }
    }
}