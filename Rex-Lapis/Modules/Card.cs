public class CardClass : InteractionModuleBase<SocketInteractionContext>{
    [SlashCommand("card", "Rex Lapis gives you your daily card.")]
    public async Task cardMethod(){
        CounterClass.cardCount++;
        Random number = new Random((int) Context.User.Id + (int) (DateTime.Today - DateTime.MinValue).TotalDays);
        ulong UID = Context.User.Id;

        string[] creatures = [
            "hilichurls", 
            "slimes",
            "fungi",
            "specters",
            "crabs",
            "seals",
            "seahorses",
            "fatui",
            "treasure hoarders",
            "ruin machinery",
            "samurai",
            "rogue Eremites",
            "scorpions",
            "serpents",
            "Xuanwen beasts",
            "Serpent knights"
        ];

        string[] locations = [
            "Monstadt",
            "Liyue",
            "Inazuma",
            "Sumeru",
            "Fontaine"
        ];

        string[] cards = [
            "Today, you should play with a few of your friends!",
            "Join a random person in another world, maybe you'll make a new friend.",
            "Try doing a different domain today. However, it is up to you how to spend your resin.",
            "Take a nice, long journey with your friends today.",
            "You should use a character you haven't played with in a while.",
            "A boss rush to test your skills can give a rewarding victory.",
            "Today is a very good day to head out and catch some fish in " + locations[number.Next(0, locations.Length)] + ".",
            "I would think going out to fight some " + creatures[number.Next(0, creatures.Length)] + " would make for a good time today.",
            "Maybe a game or two of TCG with a friend will be entertaining.",
            "A challenge sounds good, an Abyss run sounds like a rather amusing time.",
            "A little theater wouldn't hurt, why not go for the Imaginarium Theater today?",
            "Perhaps we should take some time to mine in " + locations[number.Next(0, locations.Length)] + ". The ores would serve us well in the future.",
            "It might be a good idea to gather some wood from the trees in " + locations[number.Next(0, locations.Length)] + ". We could make some nice furniture with it, after all.",
            "Maybe today would be a good time to invest in a benched character.",
            "Why not play a game of hide and seek with your friends?"
        ];

        string pick1 = cards[number.Next(0, cards.Length)];
        string pick2 = cards[number.Next(0, cards.Length)];
        string pick3 = cards[number.Next(0, cards.Length)];

        //If the cards are the same, cycle until they aren't.
        while(pick1 == pick2){
            pick2 = cards[number.Next(0, cards.Length)];
        }
        while(pick1 == pick3 | pick2 == pick3){
            pick3 = cards[number.Next(0, cards.Length)];
        }

        await RespondAsync("`" + DateTime.Today.ToShortDateString() + "`: ```- " + pick1 + "\n- " + pick2 + "\n- " + pick3 + "```" + Global.lastStatement(), ephemeral: true);

    }
}