using Discord.Rest;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RexLapis.Database;

public class RolesClass : InteractionModuleBase<SocketInteractionContext>{

    DBClass eventDB = new DBClass();

    private int base16To10(char input){
        switch(input){
            case '0':
                return 0;
            case '1':
                return 1;
            case '2':
                return 2;
            case '3':
                return 3;
            case '4':
                return 4;
            case '5':
                return 5;
            case '6':
                return 6;
            case '7':
                return 7;
            case '8':
                return 8;
            case '9':
                return 9;
            case 'a':
                return 10;
            case 'b':
                return 11;
            case 'c':
                return 12;
            case 'd':
                return 13;
            case 'e':
                return 14;
            case 'f':
                return 15;
            default:
                return -1;
        }
    }

    private Color HexToRGB(string input){

        //Convert the Hex values to the corresponding colors.
        int R = base16To10(input[0]) * 16 + base16To10(input[1]);
        int G = base16To10(input[2]) * 16 + base16To10(input[3]);
        int B = base16To10(input[4]) * 16 + base16To10(input[5]);

        return new Color(R, G, B);
    }

    [SlashCommand("roles", "request public roles")]
    async Task RolesMethod(){
        ServerClass thisServer = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();

        //Check to see if this server is in the database. If not, then add it.
        if(eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).Count() == 0){
            eventDB.Server.Add(new RexLapis.Database.ServerClass(){
                GuildId = Context.Guild.Id.ToString(),
                EventId = [],
                HostRoles = [],
                Roles = [],
                RoleImages = [],
                RoleDescriptions = [],
                RoleColors = []
            });

            eventDB.SaveChanges();
        }

        //Check to see if the user has permissions to manage roles.
        //If so, they'll have access to the manager menu.
        if(((SocketGuildUser) (Context.User)).GuildPermissions.Has(GuildPermission.ManageRoles) | Context.User.Id == Context.Guild.OwnerId){
            //TODO
            //AddRoles
            //RemoveRoles
        }

        //If nothing else happens, just call this function.
        await getRoles("null");

    }

    /// <summary>
    /// This function will either display to the user a menu for role selection or tell them there are no roles.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ComponentInteraction("roleGetter")]
    async Task getRoles(string number){
        ServerClass thisServer = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();
        //For the case where it's called from the original method, I 
        //handle the edge case where the input is not a valid Role ID.
        //This will just put it at the very first role shown.

        if(thisServer.Roles.Count() == 0){
            string[] apologies = {
                "Sorry, but there are no roles in this server to be obtained freely. Either check for a roles channel or ask staff.",
                "Sorry, it seems that `" + Context.Guild.Name + "` has not added any roles to the public yet.",
                "Apologies " + Context.User.GlobalName + ", but there are no public roles that I may assign to you."
            };

            await RespondAsync(Global.picker(apologies) + Global.lastStatement(), ephemeral: true);
            return;
        }

        //If the input is null, then replace it with the first role.
        if(number == "null"){
            number = "0";
        }

        string RoleID = Context.Guild.GetRole(ulong.Parse(thisServer.Roles[int.Parse(number)])).Id.ToString();

        //The embed will just have the role name, the description, and the image.
        EmbedBuilder thisEmbed = new EmbedBuilder().
        WithDescription(thisServer.RoleDescriptions[int.Parse(number)]).
        WithImageUrl(thisServer.RoleImages[int.Parse(number)]).
        WithColor(HexToRGB(thisServer.RoleColors[int.Parse(number)])).
        WithTitle(Context.Guild.GetRole(ulong.Parse(thisServer.Roles[int.Parse(number)])).Name);

        //This will be for the previous role.
        ButtonBuilder previousRole = new ButtonBuilder().
        WithCustomId("PreviousRole").
        WithLabel("Previous Role").
        WithStyle(ButtonStyle.Primary).
        WithDisabled(false);

        //This will be to add/remove the role.
        ButtonBuilder changeRole = new ButtonBuilder();

        if(((SocketGuildUser) (Context.User)).Roles.Where(x => x.Id == ulong.Parse(thisServer.Roles[int.Parse(number)])).Count() == 0){
            changeRole.
            WithCustomId("ChangeRole" + RoleID + '|' + number).
            WithLabel("Get role").
            WithStyle(ButtonStyle.Success).
            WithDisabled(false);
        } else {
            changeRole.
            WithCustomId("ChangeRole" + RoleID + '|' + number).
            WithLabel("Remove role").
            WithStyle(ButtonStyle.Danger).
            WithDisabled(false);
        }

        ButtonBuilder nextRole = new ButtonBuilder().
        WithCustomId("NextRole").
        WithLabel("Next Role").
        WithStyle(ButtonStyle.Primary).
        WithDisabled(false);

        Embed finalEmbed = thisEmbed.Build();

        MessageComponent finalButtons = new ComponentBuilder().
        WithButton(previousRole).
        WithButton(changeRole).
        WithButton(nextRole).
        Build();

        await RespondAsync(embed: finalEmbed, components: finalButtons, ephemeral: true);
        return;
    }

    [ComponentInteraction("ChangeRole*|*")]
    public async Task changeRole(string roleID, string number){
        ServerClass thisServer = eventDB.Server.Where(x => x.GuildId == Context.Guild.Id.ToString()).First();

        //If the user has the role, then remove it.
        //Otherwise, add it to the user.
        if(((SocketGuildUser) (Context.User)).Roles.Where(x => x.Id == ulong.Parse(roleID)).Count() != 0){
            await ((SocketGuildUser) (Context.User)).RemoveRoleAsync(ulong.Parse(roleID));
        } else {
            await ((SocketGuildUser) (Context.User)).AddRoleAsync(ulong.Parse(roleID));
        }

        //The embed will just have the role name, the description, and the image.
        EmbedBuilder thisEmbed = new EmbedBuilder().
        WithDescription(thisServer.RoleDescriptions[int.Parse(number)]).
        WithImageUrl(thisServer.RoleImages[int.Parse(number)]).
        WithColor(HexToRGB(thisServer.RoleColors[int.Parse(number)])).
        WithTitle(Context.Guild.GetRole(ulong.Parse(thisServer.Roles[int.Parse(number)])).Name);

        //This will be for the previous role.
        ButtonBuilder previousRole = new ButtonBuilder().
        WithCustomId("PreviousRole").
        WithLabel("Previous Role").
        WithStyle(ButtonStyle.Primary).
        WithDisabled(false);

        //This will be to add/remove the role.
        ButtonBuilder changeRole = new ButtonBuilder();

        //If the role is not equipped, then prompt the user to get the role.
        //Otherwise, prompt the user to remove the role. 
        if(((SocketGuildUser) (Context.User)).Roles.Where(x => x.Id == ulong.Parse(thisServer.Roles[int.Parse(number)])).Count() == 0){
            changeRole.
            WithCustomId("ChangeRole" + roleID + '|' + number).
            WithLabel("Get role").
            WithStyle(ButtonStyle.Success).
            WithDisabled(false);
        } else {
            changeRole.
            WithCustomId("ChangeRole" + roleID + '|' + number).
            WithLabel("Remove role").
            WithStyle(ButtonStyle.Danger).
            WithDisabled(false);
        }

        ButtonBuilder nextRole = new ButtonBuilder().
        WithCustomId("NextRole").
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

}