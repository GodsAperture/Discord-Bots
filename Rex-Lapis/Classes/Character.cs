using System.Runtime.CompilerServices;

public class Character{
    public string Name {get;}
    public string Card {get;}
    public string Icon {get;}
    public string Element {get;}
    public string Weapon {get;}
    public string Region {get;}
    public string[] Type {get;}

    //This will make dealing with characters much much easier than using JSONs.
    public Character(string inName, string inCard, string inIcon, string inElement, string inWeapon, string inRegion, string[] inType){
        Name = inName;
        Card = inCard;
        Icon = inIcon;
        Element = inElement;
        Weapon = inWeapon;
        Region = inRegion;
        Type = inType;
    }

}