using System.Threading.Tasks.Dataflow;

public class VersionClass : InteractionModuleBase<SocketInteractionContext>{
    [SlashCommand("version", "Tells the user the current version and most recent updates.")]
    public async Task versionMethod(){
        CounterClass.versionCount++;
        string currentVersion = "1.4";
        //The list of all greetings along with a singly picked greeting.
        string[] greetings = {
            "Greetings " + Context.User.GlobalName + ", I am currently in version `" + currentVersion + "`.\n",
            "Hello " + Context.User.GlobalName + ", the most up to date version is `" + currentVersion + "`.\n",
            "Good day " + Context.User.GlobalName + ", I am currently running as verion `" + currentVersion + "`.\n"
        };

        //All relevant updates
        /*  
        version 1.0:
        release, no version command.

        version 1.1:
        {
            "- Added the /version command.",
            "- Added the /card.",
            "- Added the /help command."
        }

        version 1.2:
        {
            "- Edited /card to display three cards now.",
            "- Fixed /card so it no longer displays the literal same card every day. Repeats can still occur though.",
            "- /help no longer deletes the drop down menu.",
            "- /character no longer requires a minimum of one character per drop down menu.",
            "- Made some minor tweaks to the rules of /boss."
        };

        version 1.3:
        {
            "- Added the /counter command.",
            "- Added a new card to `/card`."
        }

        version 1.4:
        {
            "- Added the `/event` command",
        };

        */

        string[] allUpdates = {
            "- Added the `/event` command",
        };

        //Final bit of information.
        string final = "For any more information, please use the `/help` command.";

        await RespondAsync(Global.picker(greetings) + "```" + String.Join('\n', allUpdates) + "```" + final, ephemeral: true);

    }
}