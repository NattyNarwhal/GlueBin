using Nancy.Authentication.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using Nancy.Security;
using GlueBin.Models;
using System.Security.Cryptography;

namespace GlueBin
{
    public class UserValidator : IUserValidator
    {
        /*
         * a program to generate passwords
            static void Main(string[] args) {
                var salt = new byte[32];
                new Random().NextBytes(salt);

                var pkbdf = new Rfc2898DeriveBytes(args[0], salt);
                Console.WriteLine("Password Hashed: {0}\r\nSalt: {1}",
                        Convert.ToBase64String(pkbdf.GetBytes(32)),
                        Convert.ToBase64String(salt));
            }
         */
        public static string GetHashedPassword(string password, string salt)
        {
            var pkbdf = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt));
            return Convert.ToBase64String(pkbdf.GetBytes(32));
        }

        IUserIdentity IUserValidator.Validate(string username, string password)
        {
            var dbUser = DatabaseConnector.UserCollection
                .Find(x => x.UserName == username).FirstOrDefault();
            var hashedPassword = GetHashedPassword(password, dbUser.Salt);
            // test for auth
            if (dbUser != null && (username == dbUser.UserName && hashedPassword == dbUser.HashedPassword))
                return dbUser;
            else return null;
        }
    }
}