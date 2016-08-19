using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Objects
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Greeting { get; set; }         
        public string Salt { get; set; }
        public ICollection<Engine> Engines { get; set; }
    }

}