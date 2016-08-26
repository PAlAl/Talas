using System;
using System.ComponentModel.DataAnnotations;

namespace Talas.Models
{
    public class LoginModel
    {
        [Required]
        public String Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        public Boolean RememberMe { get; set; }
    }
}