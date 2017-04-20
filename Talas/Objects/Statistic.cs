using System;
using System.ComponentModel.DataAnnotations;

namespace Objects
{
    public class Statistic
    {
        [Key]
        public Int32 Id { get; set; }      
        public DateTime Date { get; set; }
        public Int32 Value { get; set; }

        public Int32 EngineId { get; set; }
        public Engine Engine { get; set; }

        public Statistic() { }
        public Statistic(DateTime dateTime, Int32 value, Int32 engineId)
        {
            Date = dateTime;
            Value = value;
            EngineId = engineId;
        }

        public String DateString
        {
            get
            {
                return Date.ToString("dd.MM.yyyy HH:mm:ss");
            }
        }
    }
}