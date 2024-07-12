public static class Global{
public static Dictionary<string, infoCarrier> dict = new Dictionary<string, infoCarrier>();
public static Random number = new Random((int) (DateTime.Now - DateTime.Today).TotalSeconds);
} 