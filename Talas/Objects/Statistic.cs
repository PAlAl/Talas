using System;
using System.ComponentModel.DataAnnotations;

namespace Objects
{
    public class Statistic
    {
        [Key]
        public Int32 Id { get; set; }      
        public DateTime Date { get; set; }
        public Int16 Value { get; set; }

        public Int32 EngineId { get; set; }
        public Engine Engine { get; set; }
    }
}