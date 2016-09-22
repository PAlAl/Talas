﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Objects
{
    public class Engine
    {
        [Key]
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public String Type { get; set; }
        public String Power { get; set; }
        public String Voltage { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? InstallationDate { get; set; }
        public ICollection<EngineState> EngineStates { get; set; }
        public ICollection<Statistic> Statistic { get; set; }        
        public Int32 UserId { get; set; }
        public User User { get; set; }
    }
}