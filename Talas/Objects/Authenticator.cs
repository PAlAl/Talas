using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Talas.Models;

namespace Objects
{
    static public class Authenticator
    {       
        //public enum AuthenticateState { Succes, PasswordNotCorrect, UserNotFound }
        public static String Id { get; set; } 
        public static AuthenticateState Authenticate(String login, String password)
        {         
            AuthenticateState result;
            User user = null;
            String inHashPassword;
            using (AppContext db = new AppContext())
                user = db.Users.FirstOrDefault(u => u.Login == login);
            if (user != null)
            {
                Id = user.Id.ToString();
                inHashPassword = GenerateHashPassword(password,user.Salt);            
             
                if (user.Password.Equals(inHashPassword))
                    result = AuthenticateState.Succes;
                else
                    result = AuthenticateState.PasswordNotCorrect;
            }
            else
                result = AuthenticateState.UserNotFound;
            return result;
        }

        private static String GenerateHashPassword(String password, String userSalt)
        {
            const String staticSalt = "T@l@s";
            String result = "";          
            using (MD5 md5Hash = MD5.Create())
            {
                Byte[] hashEncodes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(password + userSalt + staticSalt));
                foreach (var encode in hashEncodes)
                {
                    result += encode.ToString("x2");
                }
            }
            return result;
        }


        
    }
}