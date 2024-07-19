using System.Globalization;

public static class Global{
public static Dictionary<string, infoCarrier> dict = new Dictionary<string, infoCarrier>();
public static Random number = new Random((int) (DateTime.Now - DateTime.Today).TotalSeconds);
public static string[] finisher = 
    {"\n\n\tShow your friends you love them on the journey,\n\t\tSigned, `Rex Lapis` " + new Emoji("<:Geo:1158853006364774491>"),
    "\n\n\tEnjoy the journey with more company,\n\t\tSigned, `Rex Lapis` " + new Emoji("<:Geo:1158853006364774491>"),
    "\n\n\tSight-seeing is always best with friends,\n\t\tSigned, `Rex Lapis` " + new Emoji("<:Geo:1158853006364774491>"),
    "\n\n\tTake your time, enjoy the scenery with your friends,\n\t\tSigned, `Rex Lapis` " + new Emoji("<:Geo:1158853006364774491>")};

public static string lastStatement(){
    return Global.finisher[Global.number.Next(0, Global.finisher.Length)];
}

};