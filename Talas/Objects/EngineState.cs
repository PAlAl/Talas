using System;
using System.ComponentModel.DataAnnotations;

namespace Objects
{
    public class EngineState
    {
        [Key]
        public int Id { get; set; }
        public Int16 Value { get; set; }
        public DateTime Date { get; set; }
        public bool? Status { get; set; }
        public Int16? Status_M { get; set; }
        public Int16? Quality { get; set; }
        public bool? Work { get; set; }
        public bool? NoStart { get; set; }
        public bool? LowR { get; set; }
        public bool? MainCont { get; set; }
        public bool? SecondMeas { get; set; }
        public bool? Test { get; set; }

        public int EngineId { get; set; }
        public Engine Engine { get; set; }
    }
}