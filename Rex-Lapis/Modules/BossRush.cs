public class BossRushClass : InteractionModuleBase<SocketInteractionContext>{
    public static string[] bossList = {
        "Anemo Hypostasis",
        "Electro Hypostasis",
        "Cryo Regisvine",
        "Cryo Hypostasis",
        "Geo Hypostasis",
        "Pyro Regisvine",
        "Rhodeia of Loch",
        "Primo Geovishap",
        "Ruin Serpent",
        "Solitary Suanni",
        "Maguu Kenki",
        "Perpetual Mechanical Array",
        "Pyro Hypostasis",
        "Hydro Hypostasis",
        "Thunder Manifestation",
        "Golden Wolflord",
        "Coral Defenders",
        "Jadeplume Terrorshroom",
        "Electro Regisvine",
        "Aeonblight Drake",
        "Algorithm of Semi-Intransient Matrix of Overseer Network",
        "Dendro Hypostasis",
        "Setekh Wenut",
        "Iniquitous Baptist",
        "Icewind Suite",
        "Emperor of Fire and Ruin",
        "Millenial Pearl Seahorse",
        "Prototype Cal. Breguet",
        "Hydro Tulpa",
        "Statue of Marble and Brass",
        "Holawaqa Ngoubou",
        "Kongamato",
        "Secret Source Automaton: Configuration Device",
        "Tenebrous Papilla",
        "Warward Heretic Spiritspeaker",
        "Lava Dragon Statue",
        "Secret Source Automaton: Overseer Device"
    };

    public static string[] trounceList = {
        "Andrius",
        "Stormterror Dvalin",
        "Childe",
        "Azhdaha",
        "La Signora",
        "Magatsu Mitake Narukami no Mikoto",
        "Everlasting Lord of Arcane Wisdom",
        "Guardian of Apep's Oasis",
        "All-Devouring Narwhal",
        "The Knave",
        "Lord of Eroded Primal Fire",
        "The Game Before the Gate",
        "The Doctor"
    };

    [SlashCommand("boss", "The user is prompted with a set of bosses to fight.")]
    public async Task bossRushMethod(string? integer = null){
        CounterClass.bossCount++;
        const int MAX_INTEGER = 10;

        //First part of the response.
        string[] beginning = [
            "I see you're quite eager for a set of trials" + ", " + Context.User.GlobalName + ". Try these:",
            "A day to be remembered would be challenging:",
            "A monumentous trial to overcome would be to face off against:",
            "After you and your brothers in arms have feasted and made merry, go challenge these:",
            Context.User.GlobalName + ", why not try combatting these:"
        ];

        string first = Global.picker(beginning);
        
        //If the user does not provide a non-zero non-negative integer, then they'll get multiple bosses.
        //This case defaults to being three sequences.
        if(integer == null){
        string bosses = "```";
            for(int i = 0; i < 3; i++){
                bosses += "\n- ";
                bosses += Global.picker(bossList) + ", ";
                bosses += Global.picker(bossList) + ", ";
                bosses += Global.picker(bossList) + ", ";

                bosses += Global.picker(trounceList);
            }

            await RespondAsync(first + bosses + "```" + Global.lastStatement(), ephemeral: true);

        } else {

            if(int.TryParse(integer, out int value)){
                //If the user provides a negative number, then this is the response.
                if(value < 0){
                    string[] negativeResponse = {
                        "Ummm...you can't request a negative amount of bosses to fight.",
                        "You do realize, you cannot battle a negative amount of any enemy, yes?",
                        "Surely this was a mistake, you seemed to have requested " + value + " bosses to fight, correct??",
                        "Perhaps you've had too much osmanthus wine. No one is capable of fighting \"" + value + "\" bosses."
                    };

                    await RespondAsync(Global.picker(negativeResponse), ephemeral: true);
                    return;

                }
                //If the user is a smart ass and provides an answer of zero, this is the response.
                if(value == 0){
                    string[] zeroResponse = {
                        "Congratulations, you've already completed the challenge by doing nothing.",
                        "Ahem, you cannot be serious. Zero bosses?? You don't seem very ambitious.",
                        "I thought you adventurers were all willing to face all challenges and overcome all obstacles, isn't that right??",
                        "Zero bosses down means zero personal progress. You won't grow doing nothing.",
                        "Perhaps have a *little* more ambition than saying you wish to challenge *zero* bosses."
                    };

                    await RespondAsync(Global.picker(zeroResponse), ephemeral: true);
                    return;

                }
                //Meh, fuck it, unnecessary if statement to make it look nice.
                if(value > 0 & value <= MAX_INTEGER){
                    //The number of bosses will always be a multiple of the integer provided.
                    //The number of normal bosses is always a multiple of three.
                    //The number of trounce bosses is always the amount provided by the `integer`.
                    string bosses = "```";
                    for(int i = 0; i < value; i++){
                        bosses += "\n- ";
                        bosses += Global.picker(bossList) + ", ";
                        bosses += Global.picker(bossList) + ", ";
                        bosses += Global.picker(bossList) + ", ";

                        bosses += Global.picker(trounceList);
                    }

                    await RespondAsync(first + bosses + "```" + Global.lastStatement(), ephemeral: true);
                    return;
                } else {
                    //If the user is too ambitious, the bot will decline. This is to prevent self DDoSing, and to prevent user self spam.
                    string[] tooManyResponse = {
                        "While ambition is a good thing, you'll probably get yourself killed fighting this many sequences...",
                        "Ahem, are you sure you can fight this many sequences in a row without growing weary? Choose " + MAX_INTEGER + " or less sequences.",
                        "Even the mightiest of archons could not face off this many enemies without trouble. Try " + MAX_INTEGER + " or fewer sequences.",
                        value + " sequences? Goodness me, you sound like you wish to fight the world. Please, pick a number from 1 to " + MAX_INTEGER + ".",
                        "My my, that troublesome one, Childe, would love your sense of challenge. However, you should not fight everyone and everything like he does. Instead, try 6 or fewer at a time.",
                        value + "? " + value + " *sequnences*?? You cannot be serious, you want to fight " + (value * 4) + " bosses, one after the other, and with no reprieve?\nChoose a smaller amount. Preferrably less than or equal to " + MAX_INTEGER + " sequences."
                    };

                    await RespondAsync(Global.picker(tooManyResponse), ephemeral: true);
                    return;
                }
            } else {
                //In case the user mis-types or gives something that isn't a number whatsoever, this will tell them.
                string[] nonNumber = {
                    "Excuse me, but I was expecting a number, like 4. Not " + value + ".",
                    "This must be a mistake " + Context.User.GlobalName + ", but you seem to have given a non-number for a prompt that requires a *number*.",
                    "Excuse me, but '*" + integer + "*' isn't an acceptable numerical response.",
                    "*" + integer + "*" + " might represent a number in some sort of other language, but it is not an acceptable number for this.",
                    "How does one fight *" + integer + "* bosses? Please, provide a reasonable number."
                };

                await RespondAsync(Global.picker(nonNumber), ephemeral: true);
                return;
            }
        }
    }
}