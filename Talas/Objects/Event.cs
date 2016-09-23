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
        public Int32 EngineStateId { get; set; }
        public EngineState EngineState { get; set; }
        public DateTime Date { get; set; }
    }


}