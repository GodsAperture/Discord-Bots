using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

public class Location{
    

    string region;
    string location;
    string[] sublocation;
    string[] specialties;
    Random number = new Random();


    public Location(string inRegion, string inLocation, string[] inSublocation, string[] inSpecialties){
        region = inRegion;
        location = inLocation;
        sublocation = inSublocation;
        specialties = inSpecialties;
    }

    public string choose(){

        return sublocation[number.Next(0, sublocation.Length - 1)];
        
    }

    public string getPlace(){
        return location;
    }

    public string pickSpec(){
        return specialties[number.Next(0, specialties.Length - 1)];
    }
}