using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Objects
{
    public class User
    {
        [Key]
        public Int32 Id { get; set; }
        public String Login { get; set; }
        public String Password { get; set; }
        public String Email { get; set; }
        public String Greeting { get; set; }         
        public String Salt { get; set; }
        public DateTime LastVisited { get; set; }
        public String Location { get; set; }
        public Boolean IsClamp { get; set; }
        public ICollection<Engine> Engines { get; set; }
    }

}