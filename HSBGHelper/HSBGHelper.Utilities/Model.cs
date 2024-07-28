using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using HSBGHelper.Server.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

// namespace HSBGHelper.Utilities
// {
//     public class ApplicationDbContext : DbContext
//     {
//         public IConfiguration Configuration { get; }

//         public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
//         {
//             // options = new DbContextOptions<ApplicationDbContext>();
//             // this.OnConfiguring(new DbContextOptionsBuilder<ApplicationDbContext>());
//         }
//         // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

//         public DbSet<Composition> Compositions { get; set; }
//         public DbSet<Hero> Heroes { get; set; }
//         public DbSet<Minion> Minions { get; set; }
//         public DbSet<Spell> Spells { get; set; }
//         public DbSet<Buddy> Buddies { get; set; }
//     }
// }