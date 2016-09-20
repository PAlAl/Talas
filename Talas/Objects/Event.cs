using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Objects
{
    public class Event
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 MessageId { get; set; }
        public Message Message { get; set; }
        public Boolean IsNew { get; set; }
        public Int32 EngineId { get; set; }
        public Engine Engine { get; set; }
        public DateTime Date { get; set; }
    }
}