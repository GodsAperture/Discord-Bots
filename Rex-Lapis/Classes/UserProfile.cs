using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("Users")]
public class UserInfo
{
    [Key]
    [Column("GenshinId")]
    ulong GenshinId { get; set; }
    
    [Column("DiscordId")]
    ulong DiscordId { get; set; }
    
    public ICollection<CharacterInfo> Characters { get; } = [];
}

[Table("Characters")]
public class CharacterInfo
{
  [Column("Id")]
  [Key]
  int SomeUniqueId { get; set; }

  public UserInfo User { get; set; } = new UserInfo();
}