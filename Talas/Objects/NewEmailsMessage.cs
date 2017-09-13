using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Objects;

namespace Talas.Objects
{
    public class NewEmailsMessage
    {
        [Key]
        public Int32 Id { get; set; }
        public Int32 EngineId { get; set; }
        public Engine Engine { get; set; }
        public Int32 MessageId { get; set; }
        public Engine Message { get; set; }
        public DateTime Date { get; set; }

    }
}