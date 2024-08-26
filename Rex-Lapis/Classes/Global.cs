using System.Globalization;

public static class Global{
public static Dictionary<string, infoCarrier> dict = new Dictionary<string, infoCarrier>();
private static Random number = new Random((int) (DateTime.Now - DateTime.Today).TotalSeconds);
public static string[] finisher = 
    {"\n\n\tShow your friends you love them on the journey,\n\t\tSigned, `Rex Lapis` " + new Emoji("<:Geo:1273048447179558975>"),
    "\n\n\tEnjoy the journey with more company,\n\t\tSigned, `Rex Lapis` " + new Emoji("<:Geo:1273048447179558975>"),
    "\n\n\tSight-seeing is always best with friends,\n\t\tSigned, `Rex Lapis` " + new Emoji("<:Geo:1273048447179558975>"),
    "\n\n\tTake your time, enjoy the scenery with your friends,\n\t\tSigned, `Rex Lapis` " + new Emoji("<:Geo:1273048447179558975>")};

public static string lastStatement(){
    return Global.finisher[Global.number.Next(0, Global.finisher.Length)];
}


/// <summary>
/// Given a string array, pseduo-randomly picks a string to return.
/// </summary>
/// <param name="input"></param>
/// <returns></returns>
public static string picker(IEnumerable<string> input){
    return input.ElementAt(number.Next(0, input.Count()));
}

public static int longNum(int max){
    return number.Next(0, max);
}

};