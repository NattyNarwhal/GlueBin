using MongoDB.Bson;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlueBin.Models
{
    public class UserIdentity : IUserIdentity
    {
        public ObjectId _id;

        // Claims: Paste
        public IEnumerable<string> Claims { get; set; }
        public string UserName { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
    }
}