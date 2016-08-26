using System;
using System.ComponentModel.DataAnnotations;

namespace Talas.Models
{
    public class RegisterModel
    {
        [Required]
        public String Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public String ConfirmPassword { get; set; }
    }
}