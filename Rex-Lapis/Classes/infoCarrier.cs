public class infoCarrier{
    public int ProfileCount {get; set;} = 0;

    //Menus will always have characters owned by the user as selected by default.
    //These lists will have owned characters loaded in.
    public List<string> PyroList = [];
    public List<string> HydroList = [];
    public List<string> AnemoList = [];
    public List<string> ElectroList = [];
    public List<string> DendroList = [];
    public List<string> CryoList = [];
    public List<string> GeoList = [];
    public string Profile = "";

}