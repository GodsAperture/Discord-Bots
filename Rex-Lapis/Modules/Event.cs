/*
Event:
    Events: GuildId [KEY], EventId, HostRoles
    CurrentEvents: EventId [KEY], Description, EventRoles, Users
*/


using Discord.Commands;
using Discord.Rest;
using RexLapis.Database;

public class EventClass : InteractionModuleBase<SocketInteractionContext>{

    DBClass eventDB = new DBClass();

    [SlashCommand("event", "Join events you're eligible for.")]
    public async Task EventMethod(){
        string guildId = Context.Interaction.GuildId.ToString()!;
        string[] serverRoles = eventDB.Event.Where(x => x.GuildId == guildId).First().HostRoles.ToArray();

        //Check to see if a server has been registered.
        //If not, then add their server to the database.
        if(eventDB.Event.Where(x => x.GuildId == guildId).Count() == 0){
            eventDB.Event.Add(new RexLapis.Database.EventClass(){
                GuildId = guildId,
                EventId = [],
                HostRoles = []
            });

            eventDB.SaveChanges();
        }

        //isAllowed determines if the user is allowed to create events.
        bool isAllowed = false;
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> allUsers = Context.Guild.GetUsersAsync();
        Console.WriteLine(allUsers.CountAsync());
        


        //Check if the user is the owner or has been granted administrator permissions.
        // if(user.GuildPermissions.Administrator | Context.Guild.OwnerId == Context.User.Id){
        //     isAllowed = true;
        // }


        //Check to see if any of the roles match, assuming the user isn't the owner or granted admin power.
        for(int i = 0; !isAllowed & i < ((SocketGuildUser) Context.User).Roles.Count(); i++){
            if(
                serverRoles.Any(x => x == ((SocketGuildUser) Context.User).Roles.ElementAt(i).Name
            )){
                isAllowed = true;
                break;
            }
        }

        //If a user has righst to generate an event, then this menu will always pop up instead.
        if(isAllowed){
            await RespondAsync(Context.Guild.ToString() + "Event creator.");
            return;
        }


        //If the server currently has no active events, then nothing else will happen.
        if(eventDB.Event.Where(x => x.GuildId == guildId).First().EventId.Count() == 0){
            string[] apologies = {
                "Apologies " + Context.User.GlobalName + ", but it seems `This Server` currently has no active events.",
                "It seems there are no events going on in this right now. Why not check other servers or ask if one may be started?",
                "I'm sorry to report; there are no ongoing events in `This Server`."
            };

            //Let the user know that there are no events and finish.
            await RespondAsync(apologies[Global.number.Next(0, apologies.Length)], ephemeral: true);
            return;

        }

        string[] greetings = {
            "Hello " + Context.User.GlobalName + ", which event did you want to join today?",

        };

    }
}