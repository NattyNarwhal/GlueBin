using MongoDB.Bson;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlueBin.Models
{
    /// <summary>
    /// Represents a user; which is then passed to Nancy as well as stored in
    /// the database.
    /// </summary>
    public class UserIdentity : IUserIdentity
    {
        /// <summary>
        /// The MongoDB object ID. This must be unique.
        /// </summary>
        public ObjectId _id;

        /// <summary>
        /// The rights a user has.
        /// </summary>
        /// <remarks>
        /// Possible rights:
        /// <list type="">
        /// <item>Paste</item>
        /// <item>DeleteAny</item>
        /// </list>
        /// </remarks>
        public IEnumerable<string> Claims { get; set; }
        /// <summary>
        /// The user's name.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// The password after being combined with the salt and hashed.
        /// </summary>
        public string HashedPassword { get; set; }
        /// <summary>
        /// The salt, which is combined with the password before hashing.
        /// </summary>
        public string Salt { get; set; }
    }
}