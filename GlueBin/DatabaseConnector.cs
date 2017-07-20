using GlueBin.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace GlueBin
{
    /// <summary>
    /// Manages connections for the database between modules.
    /// </summary>
    public static class DatabaseConnector
    {
        static MongoClient dbClient;
        static IMongoDatabase db;

        public static IMongoCollection<Paste> PasteCollection;
        public static IMongoCollection<UserIdentity> UserCollection;

        static DatabaseConnector()
        {
            var constStr = WebConfigurationManager.AppSettings["mongo"];

            dbClient = new MongoClient(constStr);
            db = dbClient.GetDatabase("pastes");

            PasteCollection = db.GetCollection<Paste>("pastes");
            UserCollection = db.GetCollection<UserIdentity>("users");
        }
    }
}