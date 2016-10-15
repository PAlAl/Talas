using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Objects
{
    public class EngineState
    {
        [Key]
        public Int32 Id { get; set; }
        public Int16 Value { get; set; }
        public DateTime Date { get; set; }
        public Boolean? Status { get; set; }
        public Boolean? Status_M { get; set; }
        public Int16? Quality { get; set; }
        public Boolean? Work { get; set; }
        public Boolean? NoStart { get; set; }
        public Boolean? LowR { get; set; }
        public Boolean? MainCont { get; set; }
        public Boolean? SecondMeas { get; set; }
        public Boolean? Test { get; set; }
        public Int16? Ravg { get; set; }
        public Int16? R30 { get; set; }
        public Int16? R60 { get; set; }
        public Int32 EngineId { get; set; }
        public Engine Engine { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}