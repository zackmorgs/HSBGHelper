using Microsoft.EntityFrameworkCore;
using HSBGHelper.Server.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HSBGHelper.Server.Data
{
    public class HSBGDb : IdentityDbContext<User>
    {
        public HSBGDb(DbContextOptions<HSBGDb> options) : base(options) { 
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>();
            //builder.HasDefaultSchema
        }
        public DbSet<Composition> Compositions { get; set; }
        public DbSet<Hero> Heroes { get; set; }
        public DbSet<HeroPower> HeroPowers { get; set; }
        public DbSet<Minion> Minions { get; set; }
        public DbSet<Spell> Spells { get; set; }
        public DbSet<LesserTrinket> LesserTrinkets { get; set; }
        public DbSet<GreaterTrinket> GreaterTrinkets { get; set; }  
        //public override DbSet<User> Users { get; set; }
    }
}