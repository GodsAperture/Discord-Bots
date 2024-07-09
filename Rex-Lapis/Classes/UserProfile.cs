using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("UserInfo")]
public class UserInfo{
    [Key]
    [Column("DiscordId")]
    long DiscordId { get; set; }

    [Column("GenshinId")]
    int[] GenshinId { get; set; } = [];
}

[Table("CharacterInfo")]
public class CharacterInfo{
  [Key]
  [Column("GenshinId")]
  int GenshinId { get; set; }

  [Column("DiscordId")]
  long DiscordId {get; set; }

  [Column("Characters")]
  string[] Characters { get; set; } = [];

  public UserInfo User { get; set; } = new UserInfo();
}