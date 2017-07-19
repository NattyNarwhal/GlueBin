using GlueBin.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Nancy;
using Nancy.ErrorHandling;
using Nancy.ModelBinding;
using Nancy.Security;
using System;
using System.IO;

namespace GlueBin
{
    public class GlueBinModule : NancyModule
    {
        // TODO: Move config into... config (that doesn't depend on ASP.NET)
        const bool pasteNeedsAuth = true;

        public GlueBinModule()
        {
            // Static pages
            Get["/"] = _ => View["new.html"];
            // Paste management
            Get["/(?<raw>paste|raw)/name/{name}"] = param =>
            {
                var name = (string)param.name;
                var item = DatabaseConnector.PasteCollection
                    .Find(x => x.Name == name).FirstOrDefault();
                return GetPastePage(item, param.raw == "raw");
            };
            Get["/(?<raw>paste|raw)/id/{id}"] = param =>
            {
                ObjectId id;
                if (!ObjectId.TryParse(param.id, out id))
                    return HttpStatusCode.BadRequest;
                var item = DatabaseConnector.PasteCollection
                    .Find(x => x._id == id).FirstOrDefault();
                return GetPastePage(item, param.raw == "raw");
            };
            Post["/paste"] = _ =>
            {
                if (pasteNeedsAuth)
                    this.RequiresClaims("Paste");

                var p = this.Bind<Paste>("UserName", "Posted", "_id");
                p.Name = string.IsNullOrWhiteSpace(p.Name) ?
                    Path.GetRandomFileName() : p.Name;
                p.Posted = DateTime.Now;
                p.UserName = Context.CurrentUser?.UserName;

                // keep generating new names until one works
                var inserted = false;
                do
                {
                    var likeItem = DatabaseConnector.PasteCollection
                        .Find(x => x.Name == p.Name).FirstOrDefault();
                    if (likeItem == null)
                    {
                        DatabaseConnector.PasteCollection.InsertOne(p);
                        inserted = true;
                    }
                    else if (likeItem.Name == p.Name && likeItem.UserName == p.UserName)
                    {
                        // if we're the same person, we can just replace inline
                        p._id = likeItem._id;
                        DatabaseConnector.PasteCollection.ReplaceOne(x => x._id == likeItem._id, p);
                        inserted = true;
                    }
                    else
                    {
                        p.Name = Path.GetRandomFileName();
                    }
                } while (!inserted);

                return Response.AsRedirect("/paste/id/" + p._id);
            };
            // Listings
            Get["/pastes"] = _ =>
            {
                var items = DatabaseConnector.PasteCollection
                    .Find(x => x.Public).Limit(25).ToList();
                return View["pastes.html", new { Items = items }];
            };
#if DEBUG
            // Debug routes
            Get["/debug/auth"] = _ =>
            {
                this.RequiresAuthentication();
                return string.Format("User: {0}; Claims: {1}",
                    Context.CurrentUser?.UserName, string.Join(", ",Context.CurrentUser?.Claims));
            };
#endif
        }

        dynamic GetPastePage(Paste paste, bool raw)
        {
            if (paste != null)
                if (raw)
                {
                    var r = Response.AsText(paste.Content);
                    // populate headers for future clients
                    r.Headers.Add("X-Paste-Posted", paste.Posted.ToString());
                    r.Headers.Add("X-Paste-Public", paste.Public.ToString());
                    if (paste.UserName != null)
                        r.Headers.Add("X-Paste-UserName", paste.UserName);
                    r.Headers.Add("X-Paste-Name", paste.Name);
                    r.Headers.Add("X-Paste-Id", paste._id.ToString());
                    return r;
                }
                else
                    return View["paste.html", paste];
            else
                return HttpStatusCode.NotFound;
        }
    }
}