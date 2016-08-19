using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Objects
{
    public class Engine
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Power { get; set; }
        public string Voltage { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? InstallationDate { get; set; }

        public ICollection<EngineState> EngineStates { get; set; }
        public ICollection<Statistic> Statistic { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}