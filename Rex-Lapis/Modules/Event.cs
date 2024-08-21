/*
Event:
    Events: GuildId [KEY], EventId, HostRoles
    CurrentEvents: EventId [KEY], Description, EventRoles, Users
*/
using System.CodeDom.Compiler;
using System.Security.Cryptography.X509Certificates;
using RexLapis.Database;

public class GuildEventClass : InteractionModuleBase<SocketInteractionContext>{

    DBClass eventDB = new DBClass();

    private class HostRoleModal : IModal{
        public string Title => "Submit a role ID";
        [RequiredInput(true)]
        [InputLabel("Role ID")]
        [ModalTextInput("Rold ID", TextInputStyle.Short, placeholder: "ex: 1160082367675912243")]
        public string ID { get; set; } = "";
    }

    private class EventModal : IModal{
        public string Title => "Event Creator";
        [RequiredInput(true)]
        [InputLabel("Event Name")]
        [ModalTextInput("Name", TextInputStyle.Short, placeholder: "Celebrate with this server!", maxLength: 32, minLength: 4)]
        public string Name { get; set; } = "";

        [RequiredInput(true)]
        [InputLabel("Event Description")]
        [ModalTextInput("Description", TextInputStyle.Paragraph, placeholder: "Bring your friends and join the raffle for prizes!", maxLength: 256, minLength: 8)]
        public string Description {get; set;} = "";

    }

    [SlashCommand("event", "Join events you're eligible for.")]
    public async Task EventMethod(){
        string guildId = Context.Interaction.GuildId.ToString()!;
        string[] serverRoles = eventDB.Event.Where(x => x.GuildId == guildId).First().HostRoles.ToArray();

        //Check to see if a server has been registered.
        //If not, then add their server to the database.
        if(eventDB.Event.Where(x => x.GuildId == guildId).Count() == 0){
            eventDB.Event.Add(new RexLapis.Database.EventsClass(){
                GuildId = guildId,
                EventId = [],
                HostRoles = []
            });

            eventDB.SaveChanges();
        }

        //isAllowed determines if the user is allowed to create events.
        bool isAllowed = false;
        SocketGuildUser user = ((SocketGuildUser) Context.User)!;

        //Check if the user is the owner or has been granted administrator permissions.
        isAllowed = user.GuildPermissions.Administrator | Context.Guild.OwnerId == Context.User.Id;

        //Check to see if any of the roles match, assuming the user isn't the owner or granted admin power.
        for(int i = 0; !isAllowed & i < user.Roles.Count(); i++){
            if(
                serverRoles.Any(x => x == user.Roles.ElementAt(i).Name
            )){
                isAllowed = true;
                break;
            }
        }

        //If a user has rights to generate an event, then this menu will always pop up instead.
        if(isAllowed){


            //Build a drown down menu to handle events.
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder(){
                CustomId = "EventMenuSelector",
                Placeholder = "What would you like to do today?",
                MinValues = 1,
                MaxValues = 1   
            }.AddOption("Create an event", "EventCreator").
            AddOption("Remove an event", "EventRemoval").
            AddOption("Join or leave an event", "Participation").
            AddOption("Manage host event roles", "HostRoles").
            AddOption("Manage roles for individual events", "UserRoles").
            AddOption("Manage default event roles", "DefaultRoles").
            AddOption("Launch an event for the public", "Launch");



            MessageComponent finalMenu = new ComponentBuilder().WithSelectMenu(menuBuilder).Build();

            await RespondAsync(components: finalMenu, ephemeral: true);

            return;
        }

        EventsClass thisServer = eventDB.Event.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();
        if(thisServer.EventId.Count() > 0){
            //Grab a list of all the current server events.
            List<string> eventList = eventDB.Event.Where(x => x.GuildId == Context.Guild.Id.ToString()).First().EventId;

            SelectMenuBuilder thisMenu = new SelectMenuBuilder().
            WithCustomId("ParticipationHandler").
            WithPlaceholder("Which event did you want to join or leave?").
            WithMinValues(1).
            WithMaxValues(1);
            string isJoined;

            //Find all server events and find whether or not the user is a participant.
            for(int i = 0; i < eventList.Count(); i++){
                //Find the events that are all in the server and with the event names.
                CurrentEventsClass participants = eventDB.CurrentEvents.Where(x => x.EventId == Context.Guild.Id.ToString() & x.EventName == eventList[i]).First();
                if(participants.Users.Contains(Context.User.Id.ToString())){
                    isJoined = "Joined";
                } else {
                    isJoined = "Not joined";
                }
                thisMenu.AddOption(eventList[i], eventList[i], isJoined);
            }

            //Finalize the menu and present it to the user.
            MessageComponent finalMenu = new ComponentBuilder().
            WithSelectMenu(thisMenu).
            Build();

            await RespondAsync(components: finalMenu, ephemeral: true);
        }
        //If the server currently has no active events, then nothing else will happen.
        else{
            string[] apologies = {
                "Apologies " + Context.User.GlobalName + ", but it seems " + Context.Guild.Name + " currently has no active events.",
                "It seems there are no events going on in this right now. Why not check other servers or ask if one may be started?",
                "I'm sorry to report; there are no ongoing events in " + Context.Guild.Name + ".",
            };

            //Let the user know that there are no events and finish.
            await RespondAsync(Global.picker(apologies) + Global.lastStatement(), ephemeral: true);
            return;

        }
    }

    [ComponentInteraction("EventMenuSelector")]
    async Task menuHandler(string input){
        if(input == "EventCreator"){
            //Find out how many events this server currently has.
            int count = eventDB.Event.Where(x => x.GuildId == Context.Guild.Id.ToString()).First().EventId.Count();

            //I doubt anyone will manage to create more than 10 events, but if somehow
            //someone does or has 10 events, this will prevent them from making more.
            if(count >= 10){
                string[] tooMany = {
                    "Excuse me " + Context.User.GlobalName + ", but `" + Context.Guild.Name + "` already has 10 events ongoing, please conclude an event.",
                    "My apologies, but you may not have more than 10 concurrent events in the server. Consider waiting or removing a current event?",
                    "I must deny you the resources for another event, you're already at 10 events. You must have less than 10 events to make new ones.",
                    "You should probably ask " + Context.Guild.Owner.ToString() + ", or other relevant personel, about clearing up some current events before making new ones."
                };  

                await RespondAsync(Global.picker(tooMany) + Global.lastStatement(), ephemeral: true);
                return;
            }

            //Ask the user for the event name and its description.
            await RespondWithModalAsync<EventModal>("EventCreator");
            return;
        }
        if(input == "EventRemoval"){
            //Grab all current events and show them to the user.
            string[] eventList = eventDB.Event.Where(x => x.GuildId == Context.Guild.Id.ToString()).First().EventId.ToArray();

            SelectMenuBuilder thisMenu = new SelectMenuBuilder().
            WithCustomId("EventRemover").
            WithMinValues(1).
            WithMaxValues(eventList.Length);

            //Add all the events into the events list.
            for(int i = 0; i < eventList.Length; i++){
                thisMenu.AddOption(eventList[i], eventList[i]);
            }

            MessageComponent finalMenu = new ComponentBuilder().
            WithSelectMenu(thisMenu).
            Build();

            //Present the menu to the user to delete events.
            await RespondAsync(components: finalMenu, ephemeral: true);
            return;
        }
        if(input == "Participation"){
            //Grab a list of all the current server events.
            List<string> eventList = eventDB.Event.Where(x => x.GuildId == Context.Guild.Id.ToString()).First().EventId;

            SelectMenuBuilder thisMenu = new SelectMenuBuilder().
            WithCustomId("ParticipationHandler").
            WithPlaceholder("Which event did you want to join or leave?").
            WithMinValues(1).
            WithMaxValues(1);
            string isJoined;

            //Find all server events and find whether or not the user is a participant.
            for(int i = 0; i < eventList.Count(); i++){
                //Find the events that are all in the server and with the event names.
                CurrentEventsClass participants = eventDB.CurrentEvents.Where(x => x.EventId == Context.Guild.Id.ToString() & x.EventName == eventList[i]).First();
                if(participants.Users.Contains(Context.User.Id.ToString())){
                    isJoined = "Joined";
                } else {
                    isJoined = "Not joined";
                }
                thisMenu.AddOption(eventList[i], eventList[i], isJoined);
            }

            //Finalize the menu and present it to the user.
            MessageComponent finalMenu = new ComponentBuilder().
            WithSelectMenu(thisMenu).
            Build();

            await RespondAsync(components: finalMenu, ephemeral: true);
        }
        if(input == "HostRoles"){
            string[] greeting = {
                "Good day " + Context.User.GlobalName + ", I was told you needed assistance for host roles, yes?",
                "Host roles are quite a necessity to handle, how may I help you with them today?",
                "I see that `" + Context.Guild.Name + "` is in need of host role adjustment, how shall I abet this?",
                "You posted a request for adjusting server host roles, I will be helping you with that today."
            };

            ButtonBuilder addButton = new ButtonBuilder().
            WithCustomId("HostRoleAddModal").
            WithLabel("Add host role").
            WithStyle(ButtonStyle.Success);

            ButtonBuilder removeButton = new ButtonBuilder().
            WithCustomId("HostRoleRemove").
            WithLabel("Remove host role").
            WithStyle(ButtonStyle.Danger);

            MessageComponent twoButtons = new ComponentBuilder().
            WithButton(addButton).
            WithButton(removeButton).
            Build();

            await RespondAsync(Global.picker(greeting), components: twoButtons, ephemeral: true);

        }
        if(input == "UserRoles"){
            string[] greetings = {
                "Hello, I saw you wanted to adjust what roles can participate in an event for `" + Context.Guild.Name + "`, correct?",
                "Greetings " + Context.User.GlobalName + ", which event did you want to adjust the roles for?",
                "Which event did you want to have roles adjusted for?",
                "I saw your request to adjust roles; I am now able to assist you in that endeavour."
            };
            //Grab this server's database profile to get its events.
            EventsClass thisServer = eventDB.Event.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();

            SelectMenuBuilder thisMenu = new SelectMenuBuilder().
            WithCustomId("UserRolesHandler").
            WithMinValues(1).
            WithMaxValues(1);

            //Add all current server events to the list.
            for(int i = 0; i < thisServer.EventId.Count(); i++){
                thisMenu.AddOption(thisServer.EventId[i], thisServer.EventId[i]);
            }

            MessageComponent finalMenu = new ComponentBuilder().
            WithSelectMenu(thisMenu).
            Build();

            await RespondAsync(Global.picker(greetings), components: finalMenu, ephemeral: true);
            return;

        }
        if(input == "DefaultRoles"){
            //Greet the users.
            string[] greetings = {
                "Greetings " + Context.User.GlobalName + "! I heard you were wanting to adjust what roles can always participate in events?",
                "Is `" + Context.Guild.Name + "` in need of default role changes? What roles should it have then?",
                "I see you want to adjust what roles will be included by default, which ones would you want to add or remove?",
                "Hello again " + Context.User.GlobalName + ", "
            };
        }
        if(input == "Drawing"){

        }
    }

    [ModalInteraction("EventCreator")]
    async Task eventHandler(EventModal input){

        int result = eventDB.CurrentEvents.Where(x => x.EventId == input.Name).Count();

        //If there is an event that shares the same name, it will be denied.
        if(result != 0){
            string[] failure = {
                "Excuse me, but " + Context.Guild.Name + " has an ongoing event named `" + input.Name + "`. Please consider finishing this event first before reusing its name.",
                Context.User.GlobalName + ", it seems there is already an event with the name `" + input.Name + "` currently ongoing. Please, try using another name instead.",
                "While it may be that `" + input.Name + "` might be doing super successful, you may only have one event with that name at a time.",
                "While I enjoy a good event, you may not have multiple events with the same name. Consider using an event name other than `" + input.Name + "`"
            };

            await RespondAsync(Global.picker(failure) + Global.lastStatement(), ephemeral: true);
            return;
        }


        //thisEvent represents the event the user wants to create.
        //roles will be added separately because Discord doesn't allow select menus in modals...why though??
        //The server's default event roles are still assigned to the event though.
        CurrentEventsClass thisEvent = new CurrentEventsClass();
        thisEvent.EventId = Context.Guild.Id.ToString();
        thisEvent.EventName = input.Name;
        thisEvent.Description = input.Description;
        thisEvent.EventRoles = eventDB.Event.Where(x => x.GuildId == Context.Guild.Id.ToString()).First().DefaultUserRoles;

        //guildEvent will be used to update the guild's ongoing events.
        EventsClass guildEvent = eventDB.Event.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();
        guildEvent.EventId.Add(input.Name);

        //Add thisEvent to tracked changes and then save it to the database.
        eventDB.CurrentEvents.Add(thisEvent);
        eventDB.Event.Update(guildEvent);
        eventDB.SaveChanges();

        string[] success = {
            "I have successfully created the server event `" + input.Name + "`, best of luck to all its participants!",
            "The paper work will be completed soon, the event `" + input.Name + "` has been created and you are good to go! Have fun " + Context.User.GlobalName + "!",
            "`" + input.Name + "`, has been created, now all that's left for you is to send out the invitations.",
            Context.User.GlobalName + ", `" + input.Name + "` has been set up, I hope all goes well for this event for you and its participants!"
        };

        //Let the user know that the event has been created.
        await RespondAsync(Global.picker(success) + Global.lastStatement(), ephemeral: true);
        return;
    }

    [ComponentInteraction("EventRemover")]
    async Task eventRemover(string[] input){
        //Find out how many ongoing events are going on in the server.
        string[] allEvents = eventDB.Event.Where(x => x.GuildId == Context.Guild.Id.ToString()).First().EventId.ToArray();
        int count = allEvents.Length;

        if(count == 0){
            string[] noEvents = {
                "Forgive me " + Context.User.GlobalName + ", but there are no events going on in `" + Context.Guild.Name + "`. Perhaps check elsewhere?",
                "Hmmm, it seems that `" + Context.Guild.Name + "` currently has no events. Why not make one?",
                "I made sure to check with the Adventurer's Guild twice, but we are both certain that `" + Context.Guild.Name + "` does not have any events right now.",
                "Perhaps you are mistaken, `" + Context.Guild.Name + "` has no events. Are you checking in the right server?"
            };

            await RespondAsync(Global.picker(noEvents) + Global.lastStatement(), ephemeral: true);
            return;
        }

        EventsClass guildEvent = eventDB.Event.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();
        
        //remove all instances of the mentioned events from the Events table and the CurrentEvents table.
        for(int i = 0; i < input.Length; i++){
            guildEvent.EventId.Remove(input[i]);
            eventDB.CurrentEvents.Remove(eventDB.CurrentEvents.Where(x => x.EventId == Context.Guild.Id.ToString() & x.EventName == input[i]).First());
        }

        eventDB.Event.Update(guildEvent);
        eventDB.SaveChanges();

        //This will change dialogue depending on whether more than one event was removed.
        if(count == 1){
            string[] singleRemoval = {
                "I have removed `" + input[0] + "` from the events. You're free to make another now.",
                "`" + input[0] + "` has been deleted from the records. No further action is required.",
                Context.User.GlobalName + ", I have taken the steps necessary to clear `" + input[0] + "` from the active events list.",
                "`" + input[0] + "` has been removed from the events list for `" + Context.Guild.Name + "."
            };

            await RespondAsync(Global.picker(singleRemoval) + Global.lastStatement(), ephemeral: true);
            return;

        } else {
            string[] multipleRemoval = {
                count.ToString() + " total events have been removed from the `" + Context.Guild.Name + "` events list.",
                "All " + count.ToString() + " have been removed from the events board. You're free to make up to " + (10 - count).ToString() + "new events.",
                Context.User.GlobalName + ", all the events have been removed, as requested. Now, perhaps a rest is in order?",
                "All the server events mentioned have been wiped from the board. Perhaps you should contact " + Context.Guild.Owner + " and ask whether another should be started?"
            };

            await RespondAsync(Global.picker(multipleRemoval) + Global.lastStatement(), ephemeral: true);
            return;
        }

    }

    [ComponentInteraction("ParticipationHandler")]
    async Task participationHandler(string input){
        CurrentEventsClass thisEvent = eventDB.CurrentEvents.Where(x => x.EventId == Context.Guild.Id.ToString() & x.EventName == input).First();
        //Check if the user has joined already. If they have, they'll be asked if they want to leave.
        //If not, they'll be asked if they want to join, if they meet the criteria.
        if(thisEvent.Users.Contains(Context.User.Id.ToString())){
            string[] leaving = {
                "Hello " + Context.User.GlobalName + ", as per your request, your position in the event has been relinquished.",
                "You have left the `" + thisEvent.EventName + "` event. You may rejoin afterward if you still meet the criteria.",
                "You have departed this event, " + Context.User.GlobalName + ". No further action is required.",
                "The paperwork has been handled, you have been removed from `" + thisEvent.EventName + "` at your behest."
            };

            thisEvent.Users.Remove(Context.User.Id.ToString());
            eventDB.SaveChanges();

            await RespondAsync(Global.picker(leaving) + Global.lastStatement(), ephemeral: true);
            return;
        } else {
            //If the count is zero, then everyone is allowed by default.
            bool hasEveryone = thisEvent.EventRoles.Count() == 0;
            SocketRole[] allRoles = ((SocketGuildUser) Context.User).Roles.ToArray();
            string[] userRoles = new string[allRoles.Length];
            string[] eventRoles = thisEvent.EventRoles.ToArray();

            //Extract all the role names.
            for(int i = 0; i < allRoles.Length; i++){
                userRoles[i] = allRoles[i].Name;
            }

            bool hasRole = false;

            //Determine if the user has any of the roles for participating in the event.
            for(int i = 0; i < eventRoles.Length; i++){
                if(userRoles.Contains(eventRoles[i])){
                    hasRole = true;
                    break;
                }
            }

            //If the user has any of the necessary roles, they can join.
            //Otherwise they will be told they can't join.
            if(hasRole | hasEveryone){
                string[] success = {
                    "Greetings " + Context.User.GlobalName + ", you have been added to `" + thisEvent.EventName + "`, enjoy!",
                    "I've finished signing off on the paper work. You've been entered into the `" + thisEvent.EventName + "`!",
                    "You've been included in the `" + Context.Guild.Name + "` server's " + thisEvent.EventName + " event!",
                    "The criteria for entry has been met. I've placed you in the event roster. So now, good luck and have fun!"
                };
                thisEvent.Users.Add(Context.User.Id.ToString());
                eventDB.SaveChanges();

                await RespondAsync(Global.picker(success) + Global.lastStatement(), ephemeral: true);
                return;
            } else {
                string[] apology = {
                    "Apologies " + Context.User.GlobalName + ", but you do not meet the criteria to join the event.",
                    "You'll have to talk to one of the event hosts. `" + thisEvent.EventName + "` has been restricted to a set of roles that you do not have.",
                    "Hello " + Context.User.GlobalName + ", unfortunately you don't have any of the roles necessary to join this event.",
                    "I have checked over the criteria twice, and lamentably you are unable to join the event for role reasons."
                };


                await RespondAsync(Global.picker(apology) + Global.lastStatement(), ephemeral: true);
                return;
            }
            
        }

    }

    [ComponentInteraction("HostRoleAddModal")]
    async Task addRoleModal(){
        await RespondWithModalAsync<HostRoleModal>("HostRoleAdd");
    }

    [ModalInteraction("HostRoleAdd")]
    async Task HostButtonHandler(HostRoleModal input){
            //Grab all the host roles for this server.
            EventsClass thisServer = eventDB.Event.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();
            
            if(Context.Guild.GetRole(ulong.Parse(input.ID)) != null){
                string roleName = Context.Guild.GetRole(ulong.Parse(input.ID)).Name;
                thisServer.HostRoles.Add(input.ID);
                eventDB.SaveChanges();

                string[] success = {
                    "<@" + ulong.Parse(input.ID) + "> has been succesfully added to the host roles of `" + Context.Guild.Name + "`.",
                    "Good day " + Context.User.GlobalName + ", members of the role <@" + ulong.Parse(input.ID) + "> may now host events.",
                    "Host roles have been updated for this server, and now <@" + ulong.Parse(input.ID) + "> is able to host and manage events.",
                    "I've successfully updated the host roles for events. Now, <@" + ulong.Parse(input.ID) + "> users may handle events." 
                };

                await RespondAsync(Global.picker(success) + Global.lastStatement(), ephemeral: true);
                return;
            } else {
                string[] failure = {
                    "Sorry, but \"" + input + "\" is not an acceptable Role ID. Please double check it's correct.",
                    "\"" + input + "\" is not the ID of any roles of `" + Context.Guild.Name + "`.",
                    "There's been an error, what you have provided is not an ID to a server role.",
                    Context.User.GlobalName + ", the ID you provided is not a valid role ID."
                };

                await RespondAsync(Global.picker(failure) + Global.lastStatement(), ephemeral: true);
            }
    }

    // [ComponentInteraction("UserRolesHandler")]
    // async Task userRolesHandler(string input){
    //     CurrentEventsClass thisServer = eventDB.CurrentEvents.Where(x => x.EventId == Context.Guild.Id.ToString() & x.EventName == input).First();

    // }

    // async Task<IModal> ReturnRespondWithModalAsync(string customId, RequestOptions? options = null, Action<ModalBuilder>? modifyModal = null){
        
    // }
}