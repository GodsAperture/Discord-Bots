using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RexLapis.Database{
    //UserInfoClass represents the Discord user.
    public partial class UserInfoClass{
        [Key]
        public string DiscordId { get; set; } = "";
        public List<string> GenshinId { get; set; } = new List<string>();

        public void Append(string input){
            GenshinId.Add(input);
        }
    } 

    //GenshinIdClass represents a specific Genshin player account.
    public partial class GenshinIdClass{
        [Key]
        public string GenshinId { get; set; } = "";
        public string DiscordId { get; set; } = "";
        public List<string> Pyro { get; set; } = new List<string>();
        public List<string> Hydro { get; set; } = new List<string>();
        public List<string> Anemo { get; set; } = new List<string>();
        public List<string> Electro { get; set; } = new List<string>();
        public List<string> Dendro { get; set; } = new List<string>();
        public List<string> Cryo { get; set; } = new List<string>();
        public List<string> Geo { get; set; } = new List<string>();

    }
    //
    // Summary:
    //  DBClass represents the database I have, however the database itself will not
    //  be on GitHub.
    public class DBClass : DbContext{   

        public virtual DbSet<UserInfoClass> UserInfo { get; set; }
        public virtual DbSet<GenshinIdClass> GenshinInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=UserInfo.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            modelBuilder.Entity<UserInfoClass>().ToTable("UserInfo");
            modelBuilder.Entity<GenshinIdClass>().ToTable("GenshinId");
        }

    }
}