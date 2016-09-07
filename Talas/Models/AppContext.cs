using System.Data.Entity;
using Objects;

namespace Talas.Models
{
    public class AppContext: DbContext
    {  
            public DbSet<User> Users { get; set; }
            public DbSet<Engine> Engines { get; set; }
            public DbSet<EngineState> EngineStates { get; set; }
            public DbSet<Statistic> Statistics { get; set; }

        public AppContext() : base("AzureConnection")
            { }
       
    }
}