using System;
using System.ComponentModel.DataAnnotations;

namespace Objects
{
    public class Statistic
    {
        [Key]
        public int Id { get; set; }      
        public DateTime Date { get; set; }
        public Int16 Value { get; set; }

        public int EngineId { get; set; }
        public Engine Engine { get; set; }
    }
}