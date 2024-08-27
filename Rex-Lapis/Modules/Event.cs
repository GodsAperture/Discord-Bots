/*
Event:
    Events: GuildId [KEY], EventId, HostRoles
    CurrentEvents: EventId [KEY], Description, EventRoles, Users
*/
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        //Check to see if a server has been registered.
        //If not, then add their server to the database.
        if(eventDB.Server.Where(x => x.GuildId == guildId).Count() == 0){
            eventDB.Server.Add(new RexLapis.Database.ServerClass(){
                GuildId = guildId,
                EventId = [],
                HostRoles = []
            });

            eventDB.SaveChanges();
        }

        //If there are any server roles, they will be grabbed here.
        string[] serverRoles = eventDB.Server.Where(x => x.GuildId == guildId).First().HostRoles.ToArray();


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
            AddOption("Launch an event for the public", "Announcement").
            AddOption("Begin drawing names", "Drawing");



            MessageComponent finalMenu = new ComponentBuilder().WithSelectMenu(menuBuilder).Build();

            await RespondAsync(components: finalMenu, ephemeral: true);

            return;
        }

        //Otherwise, the user is not a host and will be asked if they want to join an event.
        ServerClass thisServer = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();
        if(thisServer.EventId.Count() > 0){
            await menuHandler("Participation");
            return;
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

    //This interaction handler 
    [ComponentInteraction("EventMenuSelector")]
    async Task menuHandler(string input){
        if(input == "EventCreator"){
            //Find out how many events this server currently has.
            int count = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First().EventId.Count();

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
            string[] eventIdList = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First().EventId.ToArray();
            SelectMenuBuilder thisMenu = new SelectMenuBuilder().
            WithCustomId("EventRemover").
            WithMinValues(1).
            WithMaxValues(eventIdList.Length);

            string[] greetings = {
                "Which event is being finished up? I'll begin the paper work to finish it.",
                "Greetings " + Context.User.GlobalName + ". Which of these events are being removed?",
                "Is an event coming to an end? Tell me which one, and I'll begin the clean up for you.",
                "Hello again " + Context.User.GlobalName + ", I understand there is an event to be taken down from the board?"
            };

            //Add all the events into the events list.
            for(int i = 0; i < eventIdList.Length; i++){
                thisMenu.AddOption(eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString() & x.EventId == eventIdList[i]).First().EventName, eventIdList[i]);
            }

            MessageComponent finalMenu = new ComponentBuilder().
            WithSelectMenu(thisMenu).
            Build();

            //Present the menu to the user to delete events.
            await RespondAsync(Global.picker(greetings), components: finalMenu, ephemeral: true);
            return;
        }
        if(input == "Participation"){
            //Grab a list of all the current server events.
            List<CurrentEventsClass> eventIdList = eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString()).ToList();
            List<string> eventIds = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First().EventId;
            List<string> eventList = new List<string>();
            string[] greetings = {
                "Sure, which event would you like to join? It would be very nice to swipe victory against all odds.",
                "Hello " + Context.User.GlobalName + "! I see you would like to join some of the server events.",
                "`" + Context.Guild.Name + "` will be delighted to have you join any ongoing events of theirs!",
                "Understood, I'll begin the paperwork necessary for you to join an event. Just pick one of the events."
            };

            for(int i = 0; i < eventIdList.Count(); i++){
                eventList.Add(eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString()).ElementAt(i).EventName);
            }

            SelectMenuBuilder thisMenu = new SelectMenuBuilder().
            WithCustomId("ParticipationHandler").
            WithPlaceholder("Which event did you want to join or leave?").
            WithMinValues(1).
            WithMaxValues(1);
            string isJoined;

            //Find all server events and find whether or not the user is a participant.
            for(int i = 0; i < eventList.Count(); i++){
                //Find the events that are all in the server and with the event names.
                CurrentEventsClass participants = eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();
                if(participants.Users.Contains(Context.User.Id.ToString())){
                    isJoined = "Joined";
                } else {
                    isJoined = "Not joined";
                }
                thisMenu.AddOption(eventList[i], eventIds[i], isJoined);
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
            ServerClass thisServer = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();
            SelectMenuBuilder thisMenu = new SelectMenuBuilder().
            WithCustomId("UserRolesHandler").
            WithMinValues(1).
            WithMaxValues(1);

            //Add all current server events to the list.
            for(int i = 0; i < thisServer.EventId.Count(); i++){
                thisMenu.AddOption(eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString() & x.EventId == thisServer.EventId[i]).First().EventName, thisServer.EventId[i]);
            }

            MessageComponent finalMenu = new ComponentBuilder().
            WithSelectMenu(thisMenu).
            Build();

            await RespondAsync(Global.picker(greetings), components: finalMenu, ephemeral: true);
            return;

        }
        if(input == "DefaultRoles"){
            string[] greetings = {
                "Greetings " + Context.User.GlobalName + ". I am here to help you adjust the default event roles of the server.",
            };

            ButtonBuilder addButton = new ButtonBuilder().
            WithCustomId("DefaultRoleAddModal").
            WithLabel("Add default role").
            WithStyle(ButtonStyle.Success);

            ButtonBuilder removeButton = new ButtonBuilder().
            WithCustomId("DefaultRoleRemove").
            WithLabel("Remove default role").
            WithStyle(ButtonStyle.Danger);

            MessageComponent twoButtons = new ComponentBuilder().
            WithButton(addButton).
            WithButton(removeButton).
            Build();

            await RespondAsync(Global.picker(greetings), components: twoButtons, ephemeral: true);
        }
        if(input == "Drawing"){ 
            ServerClass thisServer = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();
            int count = thisServer.EventId.Count();
            string[] eventNames = new string[count];
            //Check to see how many server events there are.
            //If there are events, then proceed.
            //If there are no event, then inform the user and end the interaction.
            if(count > 0){
                string[] greetings = {
                    "Hello " + Context.User.GlobalName + ", is time for another event giveaway?",
                    "Greetings " + Context.User.GlobalName + ", which event will we be doing drawings for?",
                    "I see, is it time to draw names for an event in `" + Context.Guild.Name + "`? I will fetch the raffle box.",
                    "`" + Context.Guild.Name + "` is picking names now? I would love to see who gets drawn!"
                };

                for(int i = 0; i < eventNames.Length; i++){
                    eventNames[i] = eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString() & x.EventId == thisServer.EventId[i]).First().EventName;
                }

                SelectMenuBuilder thisMenu = new SelectMenuBuilder().
                WithCustomId("DrawingSelector").
                WithPlaceholder("Please, select a server event.").
                WithMinValues(1).
                WithMaxValues(1);

                //Add all current server events to the select menu.
                for(int i = 0; i < count; i++){
                    thisMenu.AddOption(eventNames[i], thisServer.EventId[i], "User count: " + eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString() & x.EventId == thisServer.EventId[i]).First().Users.Count().ToString());
                }

                MessageComponent finalMenu = new ComponentBuilder().
                WithSelectMenu(thisMenu).
                Build();

                //Present the drop down menu to the user and await a choice.
                await RespondAsync(Global.picker(greetings), components: finalMenu, ephemeral: true);
                return;
            } else {
                string[] apologies = {
                    "Sorry, but there are no events in `" + Context.Guild.Name + "` right now.",
                    "Hello " + Context.User.GlobalName + ", I'm sorry to inform you, but there are no events going on in this server.",
                    "Unfortunately, `" + Context.Guild.Name + "` doesn't have any events right now. Perhaps, you should make one?",
                    "Apologies, but there are currently no events going on in the server. Perhaps you should ask for one to be made?" 
                };

                //Let the user know there are no events and end the interaction.
                await RespondAsync(Global.picker(apologies) + Global.lastStatement(), ephemeral: true);
                return;
            }

        }
        if(input == "Announcement"){
            ServerClass thisServer = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();

            //If there are no events, then notify the user.
            if(thisServer.EventId.Count() == 0){
                string[] noEvents = {
                    "Apologies " + Context.User.GlobalName + ", but there are currently no events to notify anyone about.",
                    "`" + Context.Guild.Name + "` is not hosting any events right now. Why not make some first and *then* announce them?",
                    "While I understands you may be eager to tell everyone of your events, you have never created any with me. Try making one first.",
                    Context.User.GlobalName + ", I checked twice, but this server does not have any ongoing events."
                };

                await RespondAsync(Global.picker(noEvents) + Global.lastStatement(), ephemeral:true);
                return;
            }

            string[] greetings = {
                "Hello " + Context.User.GlobalName + ", which event would you like to announce today?",
                "Good day to you, which `" + Context.Guild.Name + "` event are we going to tell everyone about?",
                "I see, it's time to tell everyone about an upcoming event in `" + Context.Guild.Name + "`, yes?",
                "Ah, so we're going to be telling others of an event now? Good, which one?"
            };

            
            SelectMenuBuilder thisMenu = new SelectMenuBuilder().
            WithCustomId("EventAnnouncer").
            WithPlaceholder("Please, select an available event.").
            WithMinValues(1).
            WithMaxValues(1);

            for(int i = 0; i < thisServer.EventId.Count(); i++){
                thisMenu.AddOption(eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString() & x.EventId == thisServer.EventId[i]).First().EventName, thisServer.EventId[i]);
            }

            MessageComponent finalMenu = new ComponentBuilder().
            WithSelectMenu(thisMenu).
            Build();

            //Greet the user and present the menu to them.
            await RespondAsync(Global.picker(greetings), components: finalMenu, ephemeral: true);
            return;
        }
    }
////Adjust roles for individual events in the server
    [ComponentInteraction("UserRolesHandler")]
    async Task eventUserRolesHandler(){
        string[] greetings = {
            "Greetings " + Context.User.GlobalName + ", I saw your request to change what roles can participate in an event.",
            "Which event from `" + Context.Guild.Name + "` are we adjusting?",
            "I have received your request for adjusting the roles of an event; I'm now able to assist you in that regard."
        };

        ButtonBuilder addButton = new ButtonBuilder().
        WithCustomId("UserRoleAddModal").
        WithLabel("Add user role").
        WithStyle(ButtonStyle.Success);

        ButtonBuilder removeButton = new ButtonBuilder().
        WithCustomId("UserRoleRemover").
        WithLabel("Remove user role").
        WithStyle(ButtonStyle.Danger);

        MessageComponent twoButtons = new ComponentBuilder().
        WithButton(addButton).
        WithButton(removeButton).
        Build();

        await RespondAsync(Global.picker(greetings), components: twoButtons, ephemeral: true);

    }

    //TO DO Later when I'm not dying inside from all the code.
    // [ComponentInteraction("UserRoleAddModal")]
    // async Task addUserRoleModal(){
    //     string[] greetings = {

    //     };


    // }



////Event management interactions
    //This interaction handler is responsible for letting hosts
    //create the events for the server.
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
        thisEvent.GuildId = Context.Guild.Id.ToString();
        thisEvent.EventId = Context.Interaction.Id.ToString();
        thisEvent.EventName = input.Name;
        thisEvent.Description = input.Description;
        thisEvent.EventRoles = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First().DefaultUserRoles;

        //guildEvent will be used to update the guild's ongoing events.
        ServerClass guildEvent = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();
        guildEvent.EventId.Add(Context.Interaction.Id.ToString());

        //Add thisEvent to tracked changes and then save it to the database.
        eventDB.CurrentEvents.Add(thisEvent);
        eventDB.Server.Update(guildEvent);
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

    //This interaction handler lets hosts remove server events.
    [ComponentInteraction("EventRemover")]
    async Task eventRemover(string[] input){
        //Find out how many ongoing events are going on in the server.
        string[] allEvents = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First().EventId.ToArray();
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

        ServerClass guildEvent = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();
        
        //remove all instances of the mentioned events from the Events table and the CurrentEvents table.
        for(int i = 0; i < input.Length; i++){
            guildEvent.EventId.Remove(input[i]);
            eventDB.CurrentEvents.Remove(eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString() & x.EventId == input[i]).First());
        }

        string eventName = eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString() & x.EventId == input[0]).First().EventName;

        eventDB.Server.Update(guildEvent);
        eventDB.SaveChanges();

        //This will change dialogue depending on whether more than one event was removed.
        if(count == 1){

            string[] singleRemoval = {
                "I have removed `" + eventName + "` from the events. You're free to make another now.",
                "`" + eventName + "` has been deleted from the records. No further action is required.",
                Context.User.GlobalName + ", I have taken the necessary steps to clear `" + eventName + "` from the active events list.",
                "`" + eventName + "` has been removed from the events list for `" + Context.Guild.Name + "`."
            };

            await RespondAsync(Global.picker(singleRemoval) + Global.lastStatement(), ephemeral: true);
            return;

        } else {
            string[] multipleRemoval = {
                count.ToString() + " events have been removed from the `" + Context.Guild.Name + "` events list.",
                "All " + count.ToString() + " events have been removed from the events board. You're free to make up to " + (10 - count).ToString() + " new events.",
                Context.User.GlobalName + ", all the events have been removed, as requested. Now, perhaps a rest is in order?",
                "All the server events mentioned have been wiped from the board. Perhaps you should contact " + Context.Guild.Owner.Mention + " and ask whether another should be started?"
            };

            await RespondAsync(Global.picker(multipleRemoval) + Global.lastStatement(), ephemeral: true);
            return;
        }

    }


////Participation related handler
    //This interaction handler allows users with the
    //appropriate roles to join an event.
    [ComponentInteraction("ParticipationHandler")]
    async Task participationHandler(string input){
        CurrentEventsClass thisEvent = eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString() & x.EventId == input).First();
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
                userRoles[i] = allRoles[i].Id.ToString();
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
                CurrentEventsClass currentEvent = eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString() & x.EventId == input).First();
                string eventName = currentEvent.EventName;
                string eventDescription = currentEvent.Description;

                await RespondAsync(Global.picker(success) + "```" + eventName + "\n\n========\n\n" + eventDescription + "```" + Global.lastStatement(), ephemeral: true);
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


////Host related interaction handlers
    //This interaction handler prompts the user to provide a
    //role ID to be added to the default roles.
    [ComponentInteraction("HostRoleAddModal")]
    async Task addRoleModal(){
        await RespondWithModalAsync<HostRoleModal>("HostRoleAdd");
        return;//I know it's not necessary to put `return` here, but I'm doing it out of habit.
    }

    //This interaction handler lets the owner/admin add host roles
    //to this server's event manager.
    [ModalInteraction("HostRoleAdd")]
    async Task HostButtonHandler(HostRoleModal input){
            //Grab all the host roles for this server.
            ServerClass thisServer = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();
            
            if(Context.Guild.GetRole(ulong.Parse(input.ID)) != null){
                string roleName = Context.Guild.GetRole(ulong.Parse(input.ID)).Name;
                thisServer.HostRoles.Add(input.ID);
                eventDB.SaveChanges();

                string[] success = {
                    Context.Guild.GetRole(ulong.Parse(input.ID)).Mention + " has been succesfully added to the host roles of `" + Context.Guild.Name + "`.",
                    "Good day " + Context.User.GlobalName + ", members of the role " + Context.Guild.GetRole(ulong.Parse(input.ID)).Mention + " may now host events.",
                    "Host roles have been updated for this server, and now " + Context.Guild.GetRole(ulong.Parse(input.ID)).Mention + " is able to host and manage events.",
                    "I've successfully updated the host roles for events. Now, " + Context.Guild.GetRole(ulong.Parse(input.ID)).Mention + " users may handle events." 
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
                return;
            }
    }



    [ComponentInteraction("HostRoleRemove")]
    async Task removeHostRole(){
        string[] greetings = {
            "Good day to you " + Context.User.GlobalName + ", which roles will no longer be able to handle events?",
            "Are we purging others from handling events? If so, which ones are we barring from this work?",
            "I see that `" + Context.Guild.Name + "` has requested some roles to be pruned, which ones are to be removed?",
            "Hello " + Context.User.GlobalName + ", which of the server roles will no longer have event related permissions?"
        };

        ServerClass thisServer = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();

        SelectMenuBuilder thisMenu = new SelectMenuBuilder().
        WithCustomId("RemoveHosts").
        WithPlaceholder("Which roles will be removed?").
        WithMinValues(1).
        WithMaxValues(thisServer.HostRoles.Count());

        for(int i = 0; i < thisServer.HostRoles.Count(); i++){
            thisMenu.AddOption(Context.Guild.GetRole(ulong.Parse(thisServer.HostRoles[i])).Name, thisServer.HostRoles[i]);
        }

        MessageComponent finalMenu = new ComponentBuilder().
        WithSelectMenu(thisMenu).
        Build();

        await RespondAsync(Global.picker(greetings), components: finalMenu, ephemeral: true);
    }

    [ComponentInteraction("RemoveHosts")]
    async Task hostRemovalHandler(string[] input){
        ServerClass thisServer = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();
        //If only one role is removed, a different response will be sent.
        if(input.Length == 1){
            string[] removal = {
                "I have seen to it that the " + Context.Guild.GetRole(ulong.Parse(input[0])).Mention + " role is no longer a host role.",
                "Greetings " + Context.User.GlobalName + ", " + Context.Guild.GetRole(ulong.Parse(input[0])).Mention + " has been removed from the host roles.",
                Context.Guild.GetRole(ulong.Parse(input[0])).Mention + " is no longer able to handle server events by default anymore.",
                "I have completed the necessary paperwork; " + Context.Guild.GetRole(ulong.Parse(input[0])).Mention + " may not handle events by defualt."
            };

            //Update the roles in the database.
            thisServer.DefaultUserRoles.Remove(input[0]);
            eventDB.Update(thisServer);
            eventDB.SaveChanges();

            await RespondAsync(Global.picker(removal) + Global.lastStatement(), ephemeral: true);
            return;
        } else {
            string[] removals = {
                "Good day " + Context.User.GlobalName + ", I have removed a total of " + input.Length + " roles from the host roles.",
                "Greetings, all the selected roles have been removed from the host roles section of the `" + Context.Guild.Name + "` server.",
                "All " + input.Length + " roles have been removed from the host roles.",
                Context.User.GlobalName + ", I have removed the roles you requested. They will no longer be able to handle events by default."
            };

            //Remove all of the picked roles.
            for(int i = 0; i < input.Length; i++){
                thisServer.DefaultUserRoles.Remove(input[i]);
            }
            eventDB.Update(thisServer);
            eventDB.SaveChanges();

            await RespondAsync(Global.picker(removals) + Global.lastStatement(), ephemeral: true);
            return;
        }
    }



    [ComponentInteraction("DefaultRoleAddModal")]
    async Task DefaultButtonHandler(){
        await RespondWithModalAsync<HostRoleModal>("DefaultRoleAddModalHandler");
    }

    [ModalInteraction("DefaultRoleAddModalHandler")]
    async Task DefaultRoleModalHandler(HostRoleModal input){
            //Grab all the host roles for this server.
            ServerClass thisServer = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();
            
            //Check to see if the role ID given is an actual server role.
            if(Context.Guild.GetRole(ulong.Parse(input.ID)) != null){
                string roleName = Context.Guild.GetRole(ulong.Parse(input.ID)).Name;
                thisServer.DefaultUserRoles.Add(input.ID);
                eventDB.SaveChanges();

                string[] success = {
                    "Good day to you " + Context.User.GlobalName + ", the " + Context.Guild.GetRole(ulong.Parse(input.ID)).Mention + " role is now approved for all future events.",
                    "Unless specified otherwise, " + Context.Guild.GetRole(ulong.Parse(input.ID)).Mention + " is now able to attend all server events.",
                    Context.Guild.GetRole(ulong.Parse(input.ID)).Mention + " may now join all future events by default, unlesss stated otherwise.",
                    "From now on, members with the " + Context.Guild.GetRole(ulong.Parse(input.ID)).Mention + " role will be able to participate in all future events." 
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
                return;
            }
    }



    [ComponentInteraction("DefaultRoleRemove")]
    async Task DefaultRoleRemoval(){
        ServerClass thisServer = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();

        if(thisServer.DefaultUserRoles.Count() != 0){
            string[] greetings = {
                "Which roles will no longer be able to join events?",
                "Good day to you " + Context.User.GlobalName + ". What roles will no longer be defaults in `" + Context.Guild.Name + "`?",
                "I've received word that you wish to revoke default event rights of particular roles, correct?"
            };

            SelectMenuBuilder thisMenu = new SelectMenuBuilder().
            WithCustomId("DefaultRoleRemovalHandler").
            WithPlaceholder("Which roles will be removed?").
            WithMinValues(1).
            WithMaxValues(thisServer.DefaultUserRoles.Count());

            //Add all default user roles.
            for(int i = 0; i < thisServer.DefaultUserRoles.Count(); i++){
                thisMenu.AddOption(Context.Guild.GetRole(ulong.Parse(thisServer.DefaultUserRoles[i])).Name, thisServer.DefaultUserRoles[i]);
            }

            MessageComponent finalMenu = new ComponentBuilder().
            WithSelectMenu(thisMenu).
            Build();

            await RespondAsync(Global.picker(greetings), components: finalMenu, ephemeral: true);
            return;
        } else {
            string[] noRoles = {
                "Apologies, but there are no default user roles in `" + Context.Guild.Name + "` to be removed.",
                Context.User.GlobalName + ", there aren't any default roles in this server for events.",
                "Ahem, I'm pretty sure you're unable to make the number of default event roles any less than zero.",
                "There are no roles in the default roles section. If you were to add a few, *then* you would be able to remove some."
            };

            await RespondAsync(Global.picker(noRoles), ephemeral: true);
            return;
        }
    }

    [ComponentInteraction("DefaultRoleRemovalHandler")]
    async Task removeRolesHandler(string[] input){
        ServerClass thisServer = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();
        //If only one role is removed, a different response will be sent.
        if(input.Length == 1){
            string[] removal = {
                "I have seen to it that the " + Context.Guild.GetRole(ulong.Parse(input[0])).Mention + " role is no longer a default role.",
                "Greetings " + Context.User.GlobalName + ", " + Context.Guild.GetRole(ulong.Parse(input[0])).Mention + " has been removed from the default roles.",
                Context.Guild.GetRole(ulong.Parse(input[0])).Mention + " is no longer able to access server events by default anymore.",
                "I have completed the necessary paperwork; " + Context.Guild.GetRole(ulong.Parse(input[0])).Mention + " may not join events by defualt."
            };

            //Update the roles in the database.
            thisServer.DefaultUserRoles.Remove(input[0]);
            eventDB.Update(thisServer);
            eventDB.SaveChanges();

            await RespondAsync(Global.picker(removal) + Global.lastStatement(), ephemeral: true);
            return;
        } else {
            string[] removals = {
                "Good day " + Context.User.GlobalName + ", I have removed a total of " + input.Length + " roles from the default roles.",
                "Greetings, all the selected roles have been removed from the default roles section of the `" + Context.Guild.Name + "` server.",
                "All " + input.Length + " roles have been removed from the default pool of event roles.",
                Context.User.GlobalName + ", I have removed the roles you requested. They will no longer be able to participate in events by default."
            };

            //Remove all of the picked roles.
            for(int i = 0; i < input.Length; i++){
                thisServer.DefaultUserRoles.Remove(input[i]);
            }
            eventDB.Update(thisServer);
            eventDB.SaveChanges();

            await RespondAsync(Global.picker(removals) + Global.lastStatement(), ephemeral: true);
            return;
        }
        
    }



    [ComponentInteraction("DrawingSelector")]
    async Task eventMenuHandler(string input){
        CurrentEventsClass thisEvent = eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString() & x.EventId == input).First();
        string[] theRoles = thisEvent.EventRoles.ToArray();
        string[] greetings = {
            "Greetings " + Context.User.GlobalName + ", which role will you pick the winner to be drawn from?",
            "I see, what role should we get use for the `" + thisEvent.EventName + "`?",
            "What role shall be picked today for the event? I'm interested to see who will be selected.",
            "While I might not be displaying it, I am quite curious to see who is going to claim victory for this raggle."
        };

        //If the server has no restrictions on entry, everyone is included by default.
        if(thisEvent.EventRoles.Count() == 0){
            await GiveawayRoleHandler("null");
        }

        //We'll keep track of how many of each user each role has.
        int[] roleCounts = new int[theRoles.Length];


        //If there are no users, then notify the host.
        //Otherwise, this proceeds.
        if(thisEvent.Users.Count() == 0){
            string[] failure = {
                "Excuse me " + Context.User.GlobalName + ", but it seems that there are no people in `" + input + "`.",
                "I am quite certain that you can't draw names for any event that has a member count of 0. Maybe try waiting for more members?",
                Context.User.GlobalName + ", you should probably either announce this to the server or wait for more members.",
                "There are no members in this event, perhaps try waiting longer or announcing this event to " + Context.Guild.Name + "?"
            };

            await RespondAsync(Global.picker(failure) + Global.lastStatement(), ephemeral: true);

        }

        //Find out how many of each role there is in this event.
        for(int i = 0; i < roleCounts.Length; i++){
            for(int k = 0; k < thisEvent.Users.Count(); k++){
                if(
                    Context.Guild.GetUser(ulong.Parse(thisEvent.Users[k])).Roles.Any(x => x.Id.ToString() == theRoles[i])
                ){
                    roleCounts[i]++;
                }
            }
        }

        SelectMenuBuilder thisMenu = new SelectMenuBuilder().
        WithCustomId("DrawingRoles").
        WithPlaceholder("Pick a role to draw from!");

        //Add the event roles to a select menu with the number of this role remaining.
        //The part received will take the form "{length}inputRole" so I can lex it later and retain the chosen event.   
        for(int i = 0; i < theRoles.Length; i++){
            thisMenu.AddOption(Context.Guild.GetRole(ulong.Parse(theRoles[i])).Name, input + '|' + theRoles[i], "User Count: " + roleCounts[i].ToString());
        }

        MessageComponent finalMenu = new ComponentBuilder().
        WithSelectMenu(thisMenu).
        Build();

        await RespondAsync(Global.picker(greetings), components: finalMenu, ephemeral: true);
        return;

    }

    [ComponentInteraction("DrawingRoles")]
    async Task GiveawayRoleHandler(string input){
        //EventId|Role
        string[] thisInput = input.Split('|');

        //If there are no roles, it will default to picking from everyone in the server.
        if(input == "null"){
            ulong userPicked = Context.Guild.Users.ElementAt(Global.longNum(Context.Guild.Users.Count())).Id;
            string[] otherCongrats = {
                "Everyone, please give a round of applause to " + Context.Guild.GetUser(userPicked).Mention + " for claiming the victory!",
                "Congratulations is due to " + Context.Guild.GetUser(userPicked).Mention + " for their newly obtained success!",
                Context.Guild.GetUser(userPicked).Mention + " is a victor of the `" + eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString() & x.EventId == thisInput[0]).First().EventName + "` server event!",
                "We have a winner, and it is " + Context.Guild.GetUser(userPicked).Mention + "! My condolences to everyone else, better luck next time!"
            };

            await RespondAsync(Global.picker(otherCongrats) + Global.lastStatement());
        }


        string[] users = eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString() & x.EventId == thisInput[0]).First().Users.ToArray();
        List<string> approvedList = new List<string>();

        //Find all users in the event who have the role.
        for(int i = 0; i < users.Length; i++){
            if(Context.Guild.GetUser(ulong.Parse(users[i])).Roles.Any(x => x.Id.ToString() == thisInput[1])){
                approvedList.Add(users[i]);
            }
        }

        //Pick a random user.
        ulong pickedUser = ulong.Parse(Global.picker(approvedList));

        //Find the event and remove the user from the event.
        CurrentEventsClass thisEvent = eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString() & x.EventId == thisInput[0]).First();
        thisEvent.Users.Remove(pickedUser.ToString());
        eventDB.CurrentEvents.Update(thisEvent);
        eventDB.SaveChanges();

        string[] congrats = {
            "Everyone, please give a round of applause to " + Context.Guild.GetUser(pickedUser).Mention + " for claiming the victory!",
            "Congratulations is due to " + Context.Guild.GetUser(pickedUser).Mention + " for their newly obtained success!",
            Context.Guild.GetUser(pickedUser).Mention + " is a victor of the `" + eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString() & x.EventId == thisInput[0]).First().EventName + "` server event!",
            "We have a winner, and it is " + Context.Guild.GetUser(pickedUser).Mention + "! My condolences to everyone else, better luck next time!"
        };

        await RespondAsync(Global.picker(congrats));
        return;
    }



    [ComponentInteraction("EventAnnouncer")]
    async Task eventAnnouncer(string input){
        CurrentEventsClass thisEvent = eventDB.CurrentEvents.Where(x => x.GuildId == Context.Guild.Id.ToString() & x.EventId == input).First();

        string[] blessings = {
            "\n\nI am personally wishing everyone luck this event!",
            "\n\nMay the archons smile down on you with favor!",
            "\n\nGood luck to everyone, and may only the best be chosen!",
            "\n\nI hope that everyone here enjoys their time in this event!"
        };

        string finalMessage = "```\n" + thisEvent.EventName;
        finalMessage += "\n\n========\n\n";
        finalMessage += thisEvent.Description + "```\n";

        finalMessage += "Eligible members:\n";
        
        //If there are no role restrictions, then everyone is included.
        //If there are restrictions, then only the selected roles are included.
        if(thisEvent.EventRoles.Count() == 0){
            finalMessage += "- <@everyone>";
        } else {
            for(int i = 0; i < thisEvent.EventRoles.Count(); i++){
                finalMessage += "- " + Context.Guild.GetRole(ulong.Parse(thisEvent.EventRoles[i])).Mention + "\n";
            }
        }

        //Rex Lapis personally wishes every participant luck.
        finalMessage += Global.picker(blessings);

        await RespondAsync(finalMessage + Global.lastStatement());
        return;
    }

}