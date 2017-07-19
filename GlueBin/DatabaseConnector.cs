using GlueBin.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlueBin
{
    /// <summary>
    /// Manages connections for the database between modules.
    /// </summary>
    public static class DatabaseConnector
    {
        const string constStr =
            "mongodb://dbadmin:password@localhost:27017/pastes?authSource=admin";

        static MongoClient dbClient;
        static IMongoDatabase db;

        public static IMongoCollection<Paste> PasteCollection;
        public static IMongoCollection<UserIdentity> UserCollection;

        static DatabaseConnector()
        {
            dbClient = new MongoClient(constStr);
            db = dbClient.GetDatabase("pastes");

            PasteCollection = db.GetCollection<Paste>("pastes");
            UserCollection = db.GetCollection<UserIdentity>("users");
        }
    }
}