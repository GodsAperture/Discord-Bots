using System.Security.Permissions;

public class CounterMethodClass : InteractionModuleBase<SocketInteractionContext>{
    [SlashCommand("counter", "Displays how many times all commands have been used this version of Rex Lapis.")]
    public async Task counterMethod(){
        //Rex Lapis will greet the user with one of the following.
        string[] greetings = {
            "Hello " + Context.User.GlobalName + ", I have fetched for you the number of times all commands have been used. Here:\n\n",
            "Here is the current count of each method use this version:\n\n",
            "I've gathered the necessary data for you " + Context.User.GlobalName + ", this is how many times each of my commands have been called this version:\n\n",
            "I see you have made a request for usage disclosure. Here is that information:\n\n"
        };

        //I tie all of the counts together into a single string.
        string count = "```";
        count += "Boss Rush:   " + CounterClass.bossCount + '\n';
        count += "Card:        " + CounterClass.cardCount + '\n';
        count += "Character:   " + CounterClass.characterCount + '\n';
        count += "Fandom:      " + CounterClass.fandomCount + '\n';
        count += "Help:        " + CounterClass.helpCount + '\n';
        count += "Random:      " + CounterClass.randomCount + '\n';
        count += "Register:    " + CounterClass.registerCount + '\n';
        count += "Travel:      " + CounterClass.travelCount + '\n';
        count += "Version:     " + CounterClass.versionCount + '\n';
        count += "```";

        //Return the counts to the user.
        await RespondAsync(greetings[Global.number.Next(0, greetings.Length)] + count + Global.lastStatement(), ephemeral: true);

    }
}