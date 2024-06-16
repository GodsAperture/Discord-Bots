public class HelpCommand : InteractionModuleBase<SocketInteractionContext>{

    //"Random" number generator to pick arbitrary numbers for the user.
    private static Random num = new Random((DateTime.Now - DateTime.Today).Seconds);
    Emoji geoEmote = new Emoji("<:Geo:1158853006364774491>");

    private static string[] Intro = {
        "Hello, welcome to Liyue. Many people tend to inquire about what I have to offer as an archon:```",
        "I see you have come for some assistance, here's what I can do for you:```",
        "Hello there, I see you are in need of information. Here is what I have for you:```",
        "I see you are lost. I have some time to spare to serve you:```"
        };



//Commands
    private static string[] Travel = {
        "\ttravel: Sometimes people inquire me on where to travel, I'll give them suggestions.\n\n",
        "\ttravel: I have lived long and I have seen many wonderous realms. I will share some of those same paths I took with you.\n\n",
        "\ttravel: Looking for a place to travel? I'll tell you which journies I enjoyed the most.\n\n",
        "\ttravel: A long journey is always better with friends, why not take the route I suggest for you?\n\n"
        };



    private static string[] End = {
        "```\n\tI sincerely hope that this was of utmost help to you.\n\t\tSigned, `Rex Lapis` ",
        "```\n\tWe, at Liyue Harbor, are all here to be of support to you.\n\t\tSigned, `Rex Lapis` ",
        "```\n\tEvery journey has its final day. Don't rush.\n\t\tSigned, `Rex Lapis` ",
        "```\n\tPlease, enjoy your time at the harbor as we all have.\n\t\tSigned, `Rex Lapis` "
    };

    [SlashCommand("help", "Rex Lapis describes all current commands.")]
    public async Task ExecuteCommandAsync(){

        //Pick a random substring from each category.
        string intro = Intro[num.Next(0, Intro.Length)];
        string travel = Travel[num.Next(0, Travel.Length)];
        string end = End[num.Next(0, End.Length)];

        //Combine the strings to make a final statement by Rex Lapis.
        await RespondAsync(intro + travel + end + geoEmote);

    }
}