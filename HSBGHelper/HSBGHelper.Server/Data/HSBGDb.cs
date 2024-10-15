using Microsoft.EntityFrameworkCore;
using HSBGHelper.Server.Models;
using DotNetEnv;
namespace HSBGHelper.Server.Data
{
    public class HSBGDb : DbContext
    {
        public HSBGDb(DbContextOptions<HSBGDb> options) : base(options) { 
        }
        public DbSet<Composition> Compositions { get; set; }
        public DbSet<Hero> Heroes { get; set; }
        public DbSet<HeroPower> HeroPowers { get; set; }
        public DbSet<Minion> Minions { get; set; }
        public DbSet<Spell> Spells { get; set; }
        public DbSet<LesserTrinket> LesserTrinkets { get; set; }
        public DbSet<GreaterTrinket> GreaterTrinkets { get; set; }  
        public DbSet<User> Users { get; set; }
    }
}