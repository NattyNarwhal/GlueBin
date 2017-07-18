using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlueBin
{
    // minimal viable IUserIdentity
    public class UserIdentity : IUserIdentity
    {
        public IEnumerable<string> Claims
        {
            get; set;
        }

        public string UserName
        {
            get; set;
        }

        public UserIdentity(string userName)
        {
            UserName = userName;
        }
    }
}