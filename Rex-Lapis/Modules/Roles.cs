using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using Discord.Rest;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RexLapis.Database;

public class RolesClass : InteractionModuleBase<SocketInteractionContext>{

    DBClass eventDB = new DBClass();

    private class RoleModal : IModal {
        public string Title => "Submit a role ID";

        [RequiredInput(true)]
        [InputLabel("RoleID")]
        [ModalTextInput("Role ID", TextInputStyle.Short, placeholder: "ex: 1160082367675912243")]
        public string ID { get; set; } = "";

        [RequiredInput(true)]
        [InputLabel("Description")]
        [ModalTextInput("Description", TextInputStyle.Paragraph, placeholder: "What is your role for?")]
        public string Description { get; set; } = "";

        [RequiredInput(true)]
        [InputLabel("ThumbnailURL")]
        [ModalTextInput("Thumbnail URL", TextInputStyle.Short, placeholder: "ExampleUrl.com/ThisImage.png")]
        public string ThumbNailUrl { get; set; } = "";
    }

    [SlashCommand("roles", "request public roles")]
    async Task RolesMethod(){
        ServerClass thisServer = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();

        //Check to see if the user has permissions to manage roles or is the owner.
        //If so, they'll have access to the manager menu.
        if(((SocketGuildUser) (Context.User)).GuildPermissions.Has(GuildPermission.ManageRoles) | Context.User.Id == Context.Guild.OwnerId){
            string[] greetings = {
                "Good day to you " + Context.User.GlobalName + ", how may I assist you in adjusting public roles?",
                "Are we going to be adding roles or removing them from public accessibility in `" + Context.Guild.Name + "`?",
                "Hello, I am now able to assist you in adjusting the publicly accessible roles for `" + Context.Guild.Name + "`."
            };

            //This will be for adding a role.
            ButtonBuilder AddRole = new ButtonBuilder().
            WithCustomId("AddRole").
            WithLabel("Add a Role").
            WithStyle(ButtonStyle.Success).
            WithDisabled(false);

            //This will be for viewing the roles.
            ButtonBuilder ViewRole = new ButtonBuilder().
            WithCustomId("ViewRole").
            WithLabel("View Roles").
            WithStyle(ButtonStyle.Primary).
            WithDisabled(false);

            //This will be for updating the roles.
            ButtonBuilder EditRole = new ButtonBuilder().
            WithCustomId("EditRole").
            WithLabel("Edit Roles").
            WithStyle(ButtonStyle.Primary).
            WithDisabled(false);

            //This will be for removing roles.
            ButtonBuilder RemoveRole = new ButtonBuilder().
            WithCustomId("RemoveRole").
            WithLabel("Remove Roles").
            WithStyle(ButtonStyle.Danger).
            WithDisabled(false);

            //Combine all the buttons.
            MessageComponent finalButtons = new ComponentBuilder().
            WithButton(AddRole).
            WithButton(ViewRole).
            WithButton(EditRole).
            WithButton(RemoveRole).
            Build();

            await RespondAsync(":egg:\n" + Global.picker(greetings), components: finalButtons, ephemeral: true);
            return;

        }

        //If nothing else happens, just call this function.
        await getRoles("null", "null");

    }

    [ComponentInteraction("roleGetter*")]
    async Task getRoles(string isNull, string number){
        List<ServerRolesClass> publicRoles = eventDB.ServerRoles.Where(x => x.GuildId == Context.Guild.Id.ToString()).ToList();
        //For the case where it's called from the original method, I 
        //handle the edge case where the input is not a valid Role ID.
        //This will just put it at the very first role shown.

        if(publicRoles.Count() == 0){
            string[] apologies = {
                "Sorry, but there are no roles in this server to be obtained freely. Either check for a roles channel or ask staff.",
                "Sorry, it seems that `" + Context.Guild.Name + "` has not added any roles to the public yet.",
                "Apologies " + Context.User.GlobalName + ", but there are no public roles that I may assign to you."
            };

            await RespondAsync(Global.picker(apologies) + Global.lastStatement(), ephemeral: true);
            return;
        }

        if(number == "null"){
            number = "0";
        }

        string RoleID = publicRoles[int.Parse(number)].RoleId;
        int num = int.Parse(number);

        //The embed will just have the role name, the description, the image, and use the role color.
        EmbedBuilder thisEmbed = new EmbedBuilder();
        thisEmbed.WithFooter((num + 1) + " of " + publicRoles.Count());
        thisEmbed.WithDescription(publicRoles[num].RoleDescription);
        thisEmbed.WithImageUrl(publicRoles[num].RoleImage);
        thisEmbed.WithColor(Context.Guild.GetRole(ulong.Parse(publicRoles[num].RoleId)).Color);
        thisEmbed.WithTitle(Context.Guild.GetRole(ulong.Parse(publicRoles[num].RoleId)).Name);

        //This will be for the previous role.
        ButtonBuilder previousRole = new ButtonBuilder().
        WithCustomId("PreviousRole" + number).
        WithLabel("Previous Role").
        WithStyle(ButtonStyle.Primary).
        WithDisabled(false);

        //This will be to add/remove the role.
        ButtonBuilder changeRole = new ButtonBuilder();

        if(((SocketGuildUser) (Context.User)).Roles.Where(x => x.Id == ulong.Parse(publicRoles[num].RoleId)).Count() == 0){
            changeRole.
            WithCustomId("ChangeRole" + RoleID).
            WithLabel("Get role").
            WithStyle(ButtonStyle.Success).
            WithDisabled(false);
        } else {
            changeRole.
            WithCustomId("ChangeRole" + RoleID).
            WithLabel("Remove role").
            WithStyle(ButtonStyle.Danger).
            WithDisabled(false);
        }

        ButtonBuilder nextRole = new ButtonBuilder().
        WithCustomId("NextRole" + number).
        WithLabel("Next Role").
        WithStyle(ButtonStyle.Primary).
        WithDisabled(false);

        Embed finalEmbed = thisEmbed.Build();

        MessageComponent finalButtons = new ComponentBuilder().
        WithButton(previousRole).
        WithButton(changeRole).
        WithButton(nextRole).
        Build();

        if(isNull == "!null"){
            await DeferAsync();
            await ((IComponentInteraction) Context.Interaction).ModifyOriginalResponseAsync(x => {
                x.Embed = finalEmbed;
                x.Components = finalButtons;
            });
            return;
        } else {
            await RespondAsync(embed: finalEmbed, components: finalButtons, ephemeral: true);
            return;
        }

    }

    [ComponentInteraction("ChangeRole*")]
    async Task changeRole(string roleID){
        await DeferAsync();
        IQueryable<ServerRolesClass> allRoles = eventDB.ServerRoles.Where(x => x.GuildId == Context.Guild.Id.ToString());
        ServerRolesClass currentRole = eventDB.ServerRoles.Where(x => x.GuildId == Context.Guild.Id.ToString() && x.RoleId == roleID).First();

        int roleNum = 0;

        //Find where in the IQueryable the role exists.
        for(int i = 0; i < allRoles.Count(); i++){
            if(allRoles.ElementAt(i).RoleId == roleID){
                roleNum = i;
                break;
            }
        }

        //If the user has the role, then remove it.
        //Otherwise, add it to the user.
        if(((SocketGuildUser) (Context.User)).Roles.Where(x => x.Id == ulong.Parse(roleID)).Count() != 0){
            await ((SocketGuildUser) (Context.User)).RemoveRoleAsync(ulong.Parse(roleID));
        } else {
            await ((SocketGuildUser) (Context.User)).AddRoleAsync(ulong.Parse(roleID));
        }

        //The embed will just have the role name, the description, and the image.
        EmbedBuilder thisEmbed = new EmbedBuilder();
        thisEmbed.WithFooter((roleNum + 1) + " of " + allRoles.Count());
        thisEmbed.WithDescription(currentRole.RoleDescription).
        WithImageUrl(currentRole.RoleImage).
        WithColor(Context.Guild.GetRole(ulong.Parse(currentRole.RoleId)).Color).
        WithTitle(Context.Guild.GetRole(ulong.Parse(currentRole.RoleId)).Name);

        //This will be for the previous role.
        ButtonBuilder previousRole = new ButtonBuilder().
        WithCustomId("PreviousRole" + roleNum).
        WithLabel("Previous Role").
        WithStyle(ButtonStyle.Primary).
        WithDisabled(false);

        //This will be to add/remove the role.
        ButtonBuilder changeRole = new ButtonBuilder();

        //If the role is not equipped, then prompt the user to get the role.
        //Otherwise, prompt the user to remove the role. 
        if(((SocketGuildUser) (Context.User)).Roles.Where(x => x.Id == ulong.Parse(currentRole.RoleId)).Count() == 0){
            changeRole.
            WithCustomId("ChangeRole" + roleID).
            WithLabel("Get role").
            WithStyle(ButtonStyle.Success).
            WithDisabled(false);
        } else {
            changeRole.
            WithCustomId("ChangeRole" + roleID).
            WithLabel("Remove role").
            WithStyle(ButtonStyle.Danger).
            WithDisabled(false);
        }

        ButtonBuilder nextRole = new ButtonBuilder().
        WithCustomId("NextRole" + roleNum).
        WithLabel("Next Role").
        WithStyle(ButtonStyle.Primary).
        WithDisabled(false);

        Embed finalEmbed = thisEmbed.Build();

        MessageComponent finalButtons = new ComponentBuilder().
        WithButton(previousRole).
        WithButton(changeRole).
        WithButton(nextRole).
        Build();

        //Edit the original message and display the new embed.
        await ((IComponentInteraction) Context.Interaction).ModifyOriginalResponseAsync(x =>{
            x.Embed = finalEmbed;
            x.Components = finalButtons;
        });
        return;
    }

    [ComponentInteraction("PreviousRole*")]
    async Task previousRole(string number){
        int thisNumber = int.Parse(number);
        int roleCount = eventDB.ServerRoles.Where(x => x.GuildId == Context.Guild.Id.ToString()).Count();

        if(thisNumber == 0){
            await getRoles("!null", (roleCount - 1).ToString());
        } else {
            await getRoles("!null", (thisNumber - 1).ToString());
        }
    }

    [ComponentInteraction("NextRole*")]
    async Task nextRole(string number){
        int thisNumber = int.Parse(number);
        int roleCount = eventDB.ServerRoles.Where(x => x.GuildId == Context.Guild.Id.ToString()).Count();

        if(thisNumber == roleCount - 1){
            await getRoles("!null", "0");
        } else {
            await getRoles("!null", (thisNumber + 1).ToString());
        }
    }

    [ComponentInteraction("AddRole")]
    async Task addRole(){
        await RespondWithModalAsync<RoleModal>("RolesAddRoleHandler");
    }

    [ModalInteraction("RolesAddRoleHandler")]
    async Task addRoleHandler(RoleModal input){
        IQueryable<ServerRolesClass> theseRoles = eventDB.ServerRoles.Where(x => x.GuildId == Context.Guild.Id.ToString());
        //Check to see if the role ID is valid.
        ServerRolesClass thisRole = new ServerRolesClass();
        if(!Context.Guild.Roles.Any(x => x.Id.ToString() == input.ID)){
            string[] apologies = {
                "It seems that `" + Context.Guild.Name + "` does not have a role with the id of `" + input.ID + "`.",
                "Please double check the role ID you sent me. `" + input.ID + "` is not a valid role ID here.",
                "Excuse me " + Context.User.GlobalName + ", but the ID you have provided does not exist in `" + Context.Guild.Name + "`.",
                "Apologies, however there is no role with the ID you have provided here. Please, double check it for me?"
            };

            await RespondAsync(Global.picker(apologies) + Global.lastStatement(), ephemeral: true);
            return;
        }

        if(theseRoles.Any(x => x.RoleId == input.ID)){
            string[] apologies = {
                "It seems that the role you are trying to add has already been added.",
                "Excuse me " + Context.User.GlobalName + ", but I already have permission to add and remove this role.",
                "Apologies, but you cannot add the same role to my roster twice."
            };

            await RespondAsync(Global.picker(apologies) + Global.lastStatement(), ephemeral: true);
            return;
        }

        string[] success = {
            "The `" + Context.Guild.GetRole(ulong.Parse(thisRole.RoleId)).Name + "` role has been to my roster of accessible roles.",
            "`" + Context.Guild.GetRole(ulong.Parse(thisRole.RoleId)).Name + "` may now be delegated to the users.",
            "The following role has been successfully added to my roster of public roles."
        };

        EmbedBuilder thisEmbed = new EmbedBuilder();
        thisEmbed.WithDescription(input.Description).
        WithImageUrl(input.ThumbNailUrl).
        WithColor(Context.Guild.GetRole(ulong.Parse(input.ID)).Color).
        WithTitle(Context.Guild.GetRole(ulong.Parse(input.ID)).Name);
        
        //With the provided information, create the role.
        thisRole.GuildId = Context.Guild.Id.ToString();
        thisRole.RoleId = input.ID;
        thisRole.RoleDescription = input.Description;
        thisRole.RoleImage = input.ThumbNailUrl;

        eventDB.ServerRoles.Add(thisRole);

        eventDB.SaveChanges();

        await RespondAsync(Global.picker(success) + Global.lastStatement(), embed: thisEmbed.Build(), ephemeral: true);

    }

    [ComponentInteraction("ViewRole")]
    async Task viewRoles(){
        await getRoles("null", "null");
    }

    [ComponentInteraction("RemoveRole")]
    async Task removeRoles(){
        if(eventDB.ServerRoles.Where(x => x.GuildId == Context.Guild.Id.ToString()).Count() == 0){
            string[] apologies = {
                ""
            };

            await RespondAsync(Global.picker(apologies) + Global.lastStatement(), ephemeral: true);

        }

        string[] greetings = {
            "Hello " + Context.User.GlobalName + ". Which roles are being purged from public access?",
            "Which roles will no longer be publicly accessible in `" + Context.Guild.Name + "`?",
            "I've been notified of your request to have some roles removed from public access, yes?",
            "Good day " + Context.User.GlobalName + ", I see you're wanting to have some roles severed from public access."
        };

        List<ServerRolesClass> theseRoles = eventDB.ServerRoles.Where(x => x.GuildId == Context.Guild.Id.ToString()).ToList();
        int itemCount = 0;

        SelectMenuBuilder thisMenu = new SelectMenuBuilder().
        WithCustomId("RemoveRoleHandler").
        WithMinValues(1).
        WithPlaceholder("Which roles are you wanting to remove?");

        //Add all public server roles
        for(int i = 0; i < theseRoles.Count(); i++){
            thisMenu.AddOption(Context.Guild.GetRole(ulong.Parse(theseRoles[i].RoleId)).Name, theseRoles[i].RoleId);
            itemCount++;
        }

        thisMenu.WithMaxValues(itemCount);

        MessageComponent finalMenu = new ComponentBuilder().
        WithSelectMenu(thisMenu).
        Build();

        await RespondAsync(Global.picker(greetings), components: finalMenu, ephemeral: true);

    }

    [ComponentInteraction("RemoveRoleHandler")]
    async Task removeRolesHandler(string[] roles){
        if(roles.Length == 1){
            string[] removal = {
                "I have gone ahead and removed " + Context.Guild.GetRole(ulong.Parse(roles[0])).Name + " from public access.",
                "All is done, " + Context.Guild.GetRole(ulong.Parse(roles[0])).Name + " is no longer publicly available in `" + Context.Guild.Name + "`.",
                "Alright, the selected role has been removed from the list of roles I may give out."
            };

            eventDB.ServerRoles.Remove(eventDB.ServerRoles.Where(x => x.GuildId == Context.Guild.Id.ToString() && x.RoleId == roles[0]).First());
            eventDB.SaveChanges();

            await RespondAsync(Global.picker(removal) + Global.lastStatement(), ephemeral: true);

        } else {
            string[] removal = {
                "Greetings, I've gona ahead and removed the " + roles.Length + " roles you had requested to be removed.",
                "Good day " + Context.User.GlobalName + ", the selected roles have been removed.",
                "All " + roles.Length + " have been removed from my pool. Please be sure to let the users know."
            };

            for(int i = 0; i < roles.Length; i++){
                eventDB.ServerRoles.Remove(eventDB.ServerRoles.Where(x => x.GuildId == Context.Guild.Id.ToString() && x.RoleId == roles[i]).First());
            }

            eventDB.SaveChanges();

            await RespondAsync(Global.picker(removal) + Global.lastStatement(), ephemeral: true);

        }
    }

    [ComponentInteraction("EditRole")]
    async Task editRoleHandler(){
        await RespondAsync("This feature is not currently implemented. Nag GodsAperture to implement it.", ephemeral: true);
    }
}