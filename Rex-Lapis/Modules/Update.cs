using System.Threading.Tasks.Dataflow;

public class UpdateClass : InteractionModuleBase<SocketInteractionContext>{
    [SlashCommand("version", "Tells the user the current version and most recent updates.")]
    public async Task versionMethod(){
        string currentVersion = "1.1";
        //The list of all greetings along with a singly picked greeting.
        string[] greetings = {
            "Greetings " + Context.User.GlobalName + ", I am currently in version `" + currentVersion + "`.\n",
            "Hello " + Context.User.GlobalName + ", the most up to date version is `" + currentVersion + "`.\n",
            "Good day" + Context.User.GlobalName + ", I am currently running as verion `" + currentVersion + "`.\n"
        };
        string picked = greetings[Global.number.Next(0, greetings.Length)];

        //All relevant updates
        string[] allUpdates = {
            "- Added a new command called version.",
            "- Added a new command called card.",
            "- Added a new command called boss.",
        };

        //Final bit of information.
        string final = "For any more information, please use the `/help` command.";

        await RespondAsync(picked + "```" + String.Join('\n', allUpdates) + "```" + final, ephemeral: true);

    }
}