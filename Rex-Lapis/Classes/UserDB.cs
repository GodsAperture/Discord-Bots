using Microsoft.EntityFrameworkCore;

public class UserDB : DbContext
{
  public UserDB (DbContextOptions options)
    : base(options) {  }

  public DbSet<UserDB> UserDBTable  { get; set; }
}