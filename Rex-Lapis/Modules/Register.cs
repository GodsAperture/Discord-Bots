using RexLapis.Database;

public class Register2Command : InteractionModuleBase<SocketInteractionContext>{
    //This function will let users submit their Genshin UID.

    [SlashCommand("register", "Associate a Genshin UID with your Discord account.")]
    public async Task ExecuteCommandAsync([Summary("GenshinUID", "Associate a Genshin UID with your Discord account.")] string GenshinUID){
        CounterClass.registerCount++;
        try{
            //The Discord UID will be their own personal file directory.
            string UID = Context.User.Id.ToString();
            DBClass info = new DBClass();
            Random number = new Random((int) (DateTime.Now - DateTime.Today).TotalSeconds);

            string[] finisher = 
            {"\n\n\tShow your friends you love them on the journey.\n\t\tSigned, `Rex Lapis` ",
            "\n\n\tEnjoy the journey with more company.\n\t\tSigned, `Rex Lapis` ",
            "\n\n\tSight-seeing is always best with friends.\n\t\tSigned, `Rex Lapis` ",
            "\n\n\tTake your time, enjoy the scenery with your friends.\n\t\tSigned, `Rex Lapis` "};

            Emoji geoEmote = new Emoji("<:Geo:1158853006364774491>");

            string final = finisher[number.Next(0, finisher.Length)] + geoEmote;

           

            string[] sorryResponse = {
                "Sorry, but it what you gave isn't a UID.\nAn example of an acceptable UID is 643390146.",
                "A Genshin UID is always a number. There are no alphabetical letters in it.\nAn example is 643390146.",
                "I'm sorry " + Context.User.GlobalName + ", but this isn't an acceptable ID.\n643390146 is a valid ID, however " + GenshinUID + " is not."
            };

            //So...out is just the keyword to do C++'s version of pass by reference. Nice.
            //Why though???

            //If the user gives a bad argument, I just tell them to fuck off and give a legit ID.
            if(ulong.TryParse(GenshinUID, out ulong GUID) & GenshinUID.Length != 9){
                await RespondAsync(sorryResponse[number.Next(0, sorryResponse.Length)], ephemeral: true);
                return;
            }


            //If the key for the user doesn't exist then create one.
            //Otherwise, this does nothing.
            IQueryable<UserInfoClass> userList = info.UserInfo.Where(x => x.DiscordId == UID);
            if(userList.Count() == 0){
                info.UserInfo.Add(new UserInfoClass(){
                    DiscordId = UID
                });
                await info.SaveChangesAsync();
            }

            //In case the user gives a Genshin ID they already have, they don't overwrite their previous stuff by mistake.
            string[] ExistenceResponse = {
                "Excuse me " + Context.User.GlobalName + ", but it seems you've already added the account " + GUID + ".",
                "Forgive me " + Context.User.GlobalName + ", but I checked twice and " + GUID + " has already been added to your account.",
                "Greetings " + Context.User.GlobalName + ", I apologize but it seems you've already claimed " + GUID + " as part of your account."
            };

            //Otherwise, it will create a database member with the Genshin ID.
            string[] SuccessResponse = {
                "I have successfully completed the paper work to associate " + GUID + " with you, " + Context.User.GlobalName + "!",
                "I have finished setting up the necessary work, " + GUID + " is now tied to you, " + Context.User.GlobalName + "!",
                "The work has been finished, " + GUID + " is now yours to work with from here on out, " + Context.User.GlobalName + "."
            };

            //Check to see if the Genshin ID provided exists already.
            UserInfoClass user = userList.ElementAt(0);
            if(user.GenshinId.Contains(GenshinUID)){
                await RespondAsync(ExistenceResponse[number.Next(0, ExistenceResponse.Length)] + final, ephemeral: true);
                return;
            } else {

                //Check if the profile is real.
                //TO-DO...eventually.
                //I'll use Enka API to verify accounts are real.

                if(user.GenshinId.Count() <= 10){
                    //Add the Genshin ID to the list of IDs.
                    user.Append(GenshinUID);
                    info.UserInfo.Update(user);

                    //Create a new GenshinId profile in the GenshinId database.
                    info.GenshinInfo.Add(new GenshinIdClass(){
                        GenshinId = GenshinUID,
                        DiscordId = UID,
                    });
                    await info.SaveChangesAsync();

                    await RespondAsync(SuccessResponse[number.Next(0, SuccessResponse.Length)] + final, ephemeral: true);
                } else {
                    //If by magical chance the user has too many accounts, they'll be told they can't add another.
                    string[] tooManyResponse = [
                        "Excuse me " + Context.User.GlobalName + ", but you have too many accounts already.",
                        "Pardon me, but we only allow up to ten user accounts to be saved, " + Context.User.GlobalName,
                        "The rules state that you may not have more than ten Genshin accounts saved to your profile.",
                        "You may not have more than ten accounts tethered to you, although I am more curious as to why you have so many to begin with."
                        ];

                        string choice = tooManyResponse[number.Next(0, tooManyResponse.Length)];

                    await RespondAsync(choice, ephemeral: true);
                }
            }

        }
        catch(Exception e){
            await RespondAsync("Hmmm, it seems an error has occured. Please notify GodsAperture of this:\n\n" + e.Message, ephemeral: true);
            return;
        }
    }
}