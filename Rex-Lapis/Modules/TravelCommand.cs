//This has been last updated v.4.3

using Discord;
using Discord.Interactions;
using System;

//Note to self, the three Locus in Enkanomiya don't reach back to the main land.    
public class TravelCommand : InteractionModuleBase<SocketInteractionContext>{

    [SlashCommand("travel", "Rex Lapis prompts the user with a journey.")]
    public async Task ExecuteCommandAsync(){
        try{
            string start;
            string end;

            string beginning = speech1[number.Next(0, speech1.Length)];
            string[] middle1 = 
            {" and sight-see my way toward ",
            " whilst enjoying the scenery along the way to ",
            " and all the while enjoy roaming to ",
            " and relax during a trek for ",
            " during which I sauntered my way to "};

            Emoji geoEmote = new Emoji("<:Geo:1158853006364774491>");

            string[] finisher = 
            {"\n\n\tShow your friends you love them on the journey,\n\t\tSigned, `Rex Lapis` ",
            "\n\n\tEnjoy the journey with more company,\n\t\tSigned, `Rex Lapis` ",
            "\n\n\tSight-seeing is always best with friends,\n\t\tSigned, `Rex Lapis` ",
            "\n\n\tTake your time, enjoy the scenery with your friends,\n\t\tSigned, `Rex Lapis` "};

            string final = finisher[number.Next(0, finisher.Length)];

            string middle = middle1[number.Next(0, middle1.Length)];

            //Combine all locations into regions, except for Sumeru. Sumeru is divided into desert and forest.
            Location[] Monstadt = {BrightcrownMountains, Dragonspine, GalesongHill, StarfellValley, WindwailHighland};
            Location[] Liyue = {BishuiPlain, Lisha, Minlin, QiongjiEstuary, SeaOfClouds, TheChasmAboveGround, TheChasmUnderground};
            Location[] Inazuma = {Kannazuka, NarukamiIsland, SeiraiIsland, TsurumiIsland, WatatsumiIsland, YashioriIsland, EnkanomiyaRegion};
            Location[] SumeruForest = {ArdraviValley, AshavanRealm, AvidyaForest, LokapalaJungle, LostNursery, Vanarana, VissudhaField};
            Location[] SumeruDesert = {DesertOfHadramaveth, HypostyleDesert, LandOfLowerSetekh, LandOfUpperSetekh, GavirehLajavard, RealmOfFarakhkert};
            Location[] Fontaine = {BelleauRegion, BerylRegion, CourtOfFontaine, ErinnyesForest, FRI, LiffeyRegion, MorteRegion};
            Location[][] AllLocations = {Inazuma, Monstadt, Liyue, SumeruForest, SumeruDesert, Fontaine};


            //Determine out starting region, such as Sumeru, Fontaine, etc.
            int startNum1 = number.Next(0, AllLocations.Length);



            //If `num` is 0, then Inazuma is picked. If Inazuma is picked, then we stay in Inazuma.
            if(startNum1 == 0){
                startNum1 = number.Next(0, Inazuma.Length);
                //If `num` is 6, then Enkanomiya is picked. If Enkanomiya is picked, then the `end` is also in Enkanomiya.
                //I also ensure that none of the isolated islands are picked as starting points.
                if(startNum1 == 6){
                    //`choose` with an integer argument will exclude `integer` amount of objects at the end of the list.
                    start = EnkanomiyaRegion.choose(3);
                    end = EnkanomiyaRegion.choose();

                    await RespondAsync(beginning + start + middle + end + "." + final + geoEmote);
                    return;
                }

                //If `num` is not 6, then Enkanomiya is not picked. However, Enkanomiya can still be picked as an ending destination.
                start = Inazuma[number.Next(0, Inazuma.Length - 1)].choose();
                end = Inazuma[number.Next(0, Inazuma.Length)].choose();

            await RespondAsync(beginning + start + middle + end + "." + final + geoEmote);
            return;   
            }
            


            //Using `num` I determine what region to pick to start with. I then pick a location to begin in.
            Location[] region1 = AllLocations[startNum1];
            Location startRegion = region1[number.Next(0, region1.Length)];
            start = startRegion.choose();


            //If the beginning is the underground portion of the Chasm, then the end is also in the underground portion of the Chasm.
            if(startRegion.getPlace() == "The Chasm: Underground Mines"){
                string[] speech2 = 
                {"the underground parts of the Chasm and visit the ",
                "the central mining area of the Chasm and pass by the ",
                "the abandoned ruins of the Chasm and observe the "};

                string middle2 = speech2[number.Next(0, speech2.Length)];
                end = TheChasmUnderground.choose();

                await RespondAsync(beginning + middle2 + end + "." + final + geoEmote);
                return;
            }


            //An ending location is picked at random, and is then used to be the ending of the journey.
            //Inazuma is number 0, and is always excluded from the random choice list.
            Location[] region2 = AllLocations[number.Next(1, AllLocations.Length)];
            int endNum = number.Next(0, region2.Length);

            //If by chance the starting number and ending number are the same, the end will be rerolling until it is no longer the same. 
            while(startNum1 == endNum){
                endNum = number.Next(0, region2.Length);
            }

            end = region2[endNum].choose();



            await RespondAsync(beginning + start + middle + end + "." + final + geoEmote);

        }catch(Exception ex){
            Console.WriteLine(ex);
            await RespondAsync($"{ex}\n\nPlease, report this error to the developer.");
        }
    }


    public string[] speech1 = {
        "Hmmm, a walk would be nice to go from ",
        "Perhaps, today I should journey from ",
        "An old friend of mine told me to travel from ",
        "I've heard that I should wander from ",
        "I have always fancied the idea of exploring from ",
        "I was once told a story of adventurers who explored from "
    };

    public Random number = new Random((int) (DateTime.Now - DateTime.Today).TotalSeconds);

//Monstadt
    public Location BrightcrownMountains = 
        new Location("Monstadt",
        "Brightcrown Mountains",
        ["Brightcrown Canyon", "Stormterror's Lair"], 
        ["Calla Lily", "Small Lamp Grass", "Windwheel Aster"]
        );

    public Location GalesongHill =
        new Location("Monstadt",
        "Galesong Hill",
        ["Cape Oath", "Falcon Coast", "Windrise", "Dadaupa Gorge"],
        ["Dandelion Seed", "Calla Lily", "Small Lamp Grass", "Windwheel Aster"]
        );

    Location StarfellValley = 
        new Location("Monstadt",
        "Starfell Valley",
        ["Stormbearer Mountains", "Monstadt", "Stormbearer Point", "Starfell Lake", "Thousand Winds Temple", "Starsnatch Cliff", "Whispering Woods"],
        ["Dandelion Seed", "Valberry", "Philanemo Mushroom", "Calla Lily", "Cecilia", "Small Lamp Grass"]
        );

    Location WindwailHighland =
        new Location("Monstadt",
        "Windwail Highland",
        ["Dawn Winery", "Springvale", "Wolvendom"],
        ["Calla Lily", "Philanemo Mushroom", "Windwheel Aster", "Dandelion Seed", "Wolfhook"]
        );

    Location Dragonspine = 
        new Location("Monstadt",
        "Dragonspine",
        ["Entombed City - Ancient Palace", "Snow-Covered Path", "Entombed City - Outskirts", "Starglow Cavern", "Skyfrost Nail", "Wyrmrest Valley"],
        []
        );

//Liyue
    Location BishuiPlain = 
        new Location("Liyue",
        "Bishui Plain",
        ["Dihua Marsh", "Stone Gate", "Qingce Village", "Wandshu Inn", "Sal Terrae", "Wuwang Hill"],
        ["Cor Lapis", "Dandelion Seed", "Jueyun Chili", "Glaze Lily", "Qingxin", "Violetgrass", "Silk Flower", "Violetgrass", "Noctilucous Jade"]
        );

    Location Lisha =
        new Location("Liyue",
        "Lisha",
        ["Dunyu Ruins", "Lingju Pass", "Qingxu Pool"],
        ["Cor Lapis", "Violetgrass", "Noctilucous Jade", "Qingxin"]
        );  

    Location Minlin = 
        new Location("Liyue",
        "Minlin",
        ["Cuijue Slope", "Mt. Hulao", "Huaguang Stone Forest", "Nantianmen", "Jueyun Karst", "Qingyun Peak", "Mt. Aocang", "Tianqiu Valley"],
        ["Cor Lapis", "Qingxin", "Violetgrass", "Jueyun Chili"]
        );

    Location QiongjiEstuary =
        new Location("Liyue",
        "Qiongji Estuary",
        ["Guili Plains", "Luhua Pool", "Mingyun Village", "Yaoguang Shoal"],
        ["Violetgrass", "Cor Lapis", "Noctilucous Jade", "Qingxin", "Starconch"]
        );

    Location SeaOfClouds = 
        new Location("Liyue",
        "Sea of Clouds",
        ["Liyue Harbor", "Mt. Tianheng"],
        ["Cor Lapis", "Noctilucous Jade", "Qingxin", "Starconch", "Glaze Lily", "Silk Flower"]
        );

    Location TheChasmAboveGround = 
        new Location("Liyue",
        "The Chasm",
        ["Cinnabar Cliff", "Fuao Vale", "Glaze Peak", "Lumberpick Valley", "The Surface", "Tiangong Gorge"],
        ["Cor Lapis", "Violetgrass", "Qingxin", "Noctilucous Jade"]
        );

    Location TheChasmUnderground =
        new Location("Liyue",
        "The Chasm: Underground Mines",
         ["Ad-Hoc Main Tunnel", "Nameless Ruins", "Stony Halls", "The Chasm: Main Mining Area", "The Glowing Narrows", "The Serpent's Cave", "Underground Waterway"],
         ["Cor Lapis", "Noctilucous Jade"]
        );

//Inazuma

    Location NarukamiIsland =
        new Location("Inazuma",
        "Narukami Island",
        ["Amakane Island", "Inazuma City", "Araumi", "Jinren Island", "Kamisato Estate", "Byakko Plain", "Chinju Forest", "Konda Village", "Grand Narukami Shrine", "Mt. Yougou", "Ritou"],
        ["Naku Weed", "Onikabuto", "Sakura Bloom", "Sea Ganoderma"]
        );

    Location Kannazuka = 
        new Location("Inazuma",
        "Kannazuka",
        ["Kujou Encampment", "Tatarasuna"],
        ["Dendrobium", "Crystal Marrow", "Naku Weed", "Onikabuto", "Sea Ganoderma"]
        );

    Location YashioriIsland =
        new Location("Inazuma",
        "Yashiori Island",
        ["Fort Fujitou", "Jakotsu Mine", "Fort Mumei", "Musoujin Gorge", "Higi Village", "Nazuchi Beach", "Serpent's Head"],
        ["Dendrobium", "Crystal Marrow", "Naku Weed", "Onikabuto", "Sea Ganoderma"]
        );

    Location WatatsumiIsland =
        new Location("Inazuma",
        "Watatsumi Island",
        ["Bourou Village", "Sangonomiya Village", "Mouun Shrine", "Suigetsu Pool"],
        ["Sango Pearl", "Sea Ganoderma"]
        );

    Location SeiraiIsland =
        new Location("Inazuma",
        "Seirai Island",
        ["\"Seiraimaru\"", "Asase Shrine", "Amakumo Peak", "Fort Hiraumi", "Koseki Village"],
        ["Amakumo Fruit", "Naku Weed", "Onikabuto", "Sea Ganoderma"]
        );

    Location TsurumiIsland =
        new Location("Inazuma",
        "Tsurumi Island",
        ["Autake Plains", "Mt. Kanna", "Chirai Shrine", "Oina Beach", "Moshiri Ceremonial Site", "Shirikoro Peak", "Wakukau Shoal"],
        ["Fluorescent Fungus", "Sea Ganoderma"]
        );

    Location EnkanomiyaRegion =
        new Location("Inazuma",
        "Enkanomiya",
        ["Dainichi Mikoshi", "The Serpent's Bowels", "Evernight Temple", "The Serpent's Heart", "The Narrows", "Kunado's Locus", "Yachimatahiko's Locus", "Yachimatahime's Locus"],
        ["Sango Pearl", "Sea Ganoderma"]);

//Sumeru
    Location ArdraviValley = 
        new Location("Sumeru",
        "Ardravi Valley",
        ["Devantaka Mountain", "Port Ormos", "Vimara Village"],
        ["Nilotpala Lotus", "Rukkhashava Mushrooms"]
        );

    Location AshavanRealm = 
        new Location("Sumeru",
        "Ashavan Realm",
        ["Apam Woods", "Caravan Ribat", "Pardis Dhyai", "Ruins of Dahri", "Yasna Monument"],
        ["Kalpalata Lotus", "Rukkhashava Mushrooms", "Padisarah"]
        );

    Location AvidyaForest =
        new Location("Sumeru",
        "Avidya Forest",
        ["Chinvat Ravine", "Gandha Hill", "Gandharva Ville", "Sumeru City", "Yazadaha Pool"],
        ["Kalpalata Lotus", "Nilotpala Lotus", "Padisarah", "Rukkhashava Mushrooms"]
        );

    Location LokapalaJungle = 
        new Location("Sumeru",
        "Avidya",
        ["Chatrakam Cave", "Mawtiyima Forest", "The Palace of Alcazarzaray"],
        ["Rukkhashava Mushrooms"]
        );

    Location LostNursery =
        new Location("Sumeru",
        "Lost Nursery",
        ["Old Vanarana"],
        []
        );

    Location Vanarana =
        new Location("Sumeru",
        "Vanarana",
        ["Vanarana"],
        []
        );

    Location VissudhaField =
        new Location("Sumeru",
        "Vissudha Field",
        ["Fane of Ashvattha"],
        ["Rukkhashava Mushroom", "Nilotpala Lotus"]
        );

    Location DesertOfHadramaveth =
        new Location("Sumeru",
        "Desert of Hadramaveth",
        ["Debris of Panjvahe", "Dunes of Steel", "Mt. Damavand", "Passage of Ghouls", "Qusayr Al-Inkhida'", "Safhe Shatranj", "Tanit Camps", "The Sands of Al-Azif", "The Sands of Three Canals", "Wadi Al-Majuj", "Wounded Shin Valley"],
        ["Henna Berry", "Sand Grease Pupa"]
        );

    Location HypostyleDesert =
        new Location("Sumeru",
        "Hypostyle Desert",
        ["Khemenu Temple", "Sobek Oasis", "The Dune of Carouses", "The Dune of Elusion", "The Dune of Magma", "The Mausoleum of King Deshret"],
        ["Henna Berry", "Scarab"]
        );

    Location LandOfLowerSetekh = 
        new Location("Sumeru",
        "Land of Lower Setekh",
        ["Aaru Village", "Abdju Pit", "Dar al-Shifa", "Khaj-Nisut"],
        ["Henna Berry", "Scarab"]
        );

    Location LandOfUpperSetekh =
        new Location("Sumeru",
        "Land of Upper Setekh",
        ["Valley of Dahri"],
        ["Scarab"]
        );

    Location GavirehLajavard =
        new Location("Sumeru",
        "Gavireh Lajavard",
        ["Gate of Zulqarnain", "Temir Mountains", "Tunigi Hollow"],
        ["Mourning Flower", "Trishiraite"]
        );

    Location RealmOfFarakhkert =
        new Location("Sumeru",
        "Realm of Farakhkert",
        ["Asipattravana Swamp", "Hills of Barsom", "Samudra Coast", "Vourukasha Oasis"],
        ["Mourning Flower", "Trishiraite"]
        );

//Fontaine
    Location BelleauRegion =
        new Location("Fontaine",
        "Belleau Region",
        ["Poisson", "Romaritime Harbor", "Wast Slopes of Mont Automnequi"],
        ["Lumidouche Bell", "Pluie Lotus", "Rainbow Rose"]
        );

    Location BerylRegion =
        new Location("Fontaine",
        "Beryl Region",
        ["\"A Lonely Place\"", "\"A Very Bright Place\"", "\"A Very Warm Place\"", "Elton Trench", "Elynas", "Merusea Village"],
        ["Lumidouce Bell", "Rainbow Rose", "Romaritime Flower", "Beryl Conch"]
        );

    Location CourtOfFontaine =
        new Location("Fontaine",
        "Court of Fontaine",
        ["Annapausis", "Chemin de L'Espoir", "Court of Fontaine", "Fountain of Lucine", "Institute of Natural Philosophy", ""],
        ["Rainbow Rose", "Romaritime Flower", "Beryl Conch"]
        );

    Location ErinnyesForest =
        new Location("Fontaine",
        "Erinnyes Forest",
        ["Foggy Forest Path", "Loch Urania", "Lumidouce Harbor", "Weeping Willow of the Lake"],
        ["Lakelight Lily", "Lumidouce Bell", "Rainbow Rose", "Romaritime Flower"]
        );

    Location FRI =
        new Location("Fontaine",
        "Fontaine Research Institute",
        ["Central Laboratory Ruins", "New Fontaine Research Institute"],
        ["Lumitoile", "Subdetection Unit", "Romaritime Flower", "Rainbow Rose"]
        );

    Location LiffeyRegion =
        new Location("Fontaine",
        "Liffey Region",
        ["Mont Esus East"],
        ["Lumitoile", "Subdetection Units"]
        );

    Location MorteRegion =
        new Location("Fontaine",
        "Morte Region",
        ["East Slopes of Mont Automnequi", "Fort Charybdis Ruins", "Tower of Ipsissimus"],
        ["Lumidouce Bell", "Rainbow Rose", "Romaritime Flower", "Spring of the First Dewdrop"]
        );


}