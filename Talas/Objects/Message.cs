using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Objects
{
    public enum TypeMessage
    {
        ALERT,
        NOTIFICATION
    }
    public class Message
    {
        [Key]
        public Int32 Id { get; set; }
        public String Text { get; set; }
        public TypeMessage Type { get; set; }
    }
}