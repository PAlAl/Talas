using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Talas.Models;

namespace Objects
{
    static public class Authenticator
    {       
        public enum AuthenticateState { Succes, PasswordNotCorrect, UserNotFound }
        public static string Id { get; set; } 
        public static AuthenticateState Authenticate(string Login, string Password)
        {         
            AuthenticateState result;
            User user = null;
            string inHashPassword;
            using (AppContext db = new AppContext())
                user = db.Users.FirstOrDefault(u => u.Login == Login);
            if (user != null)
            {
                Id = user.Id.ToString();
                inHashPassword = GenerateHashPassword(Password,user.Salt);            
             
                if (user.Password.Equals(inHashPassword))
                    result = AuthenticateState.Succes;
                else
                    result = AuthenticateState.PasswordNotCorrect;
            }
            else
                result = AuthenticateState.UserNotFound;
            return result;
        }

        private static string GenerateHashPassword(string Password, string UserSalt)
        {
            const string staticSalt = "T@l@s";
            string result = "";          
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] hashenc = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(Password + UserSalt + staticSalt));
                foreach (var b in hashenc)
                {
                    result += b.ToString("x2");
                }
            }
            return result;
        }


        
    }
}