using Nancy.Authentication.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Security;

namespace GlueBin
{
    public class UserValidator : IUserValidator
    {
        IUserIdentity IUserValidator.Validate(string username, string password)
        {
            // test for auth
            if (username == "demo" && password == "demo")
                return new UserIdentity(username);
            else return null;
        }
    }
}