using System.Data.Entity;
using Objects;
using Talas.Objects;

namespace Talas.Models
{
    public class AppContext: DbContext
    {  
            public DbSet<User> Users { get; set; }
            public DbSet<Engine> Engines { get; set; }
            public DbSet<EngineState> EngineStates { get; set; }
            public DbSet<Statistic> Statistics { get; set; }
            public DbSet<Event> Events { get; set; }
            public DbSet<Message> Messages { get; set; }
            public DbSet<LastEngineState> LastEngineStates { get; set; }
            public DbSet<NewEmailsMessage> NewEmailsMessage { get; set; }

        public AppContext() : base("AzureConnection")
            { }
       
    }
}