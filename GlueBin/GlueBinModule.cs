using GlueBin.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Nancy;
using Nancy.ErrorHandling;
using Nancy.ModelBinding;
using Nancy.Security;
using System;
using System.Linq;
using System.IO;
using Highlight;
using Highlight.Engines;

using static GlueBin.DatabaseConnector;

namespace GlueBin
{
    public class GlueBinModule : NancyModule
    {
        // TODO: Move config into... config (that doesn't depend on ASP.NET)
        const bool pasteNeedsAuth = true;

        public GlueBinModule()
        {
            var hl = new Highlighter(new HtmlEngine());

            // Paste pages
            Get["/"] = _ =>
            {
                return View["NewPaste.cshtml", new {
                    Languages = hl.Configuration.Definitions.Keys
                }];
            };
            // Show pages
            Get["/(?<raw>paste|raw)/name/{name}"] = param =>
            {
                var raw = param.raw == "raw";
                var name = (string)param.name;
                var item = PasteCollection
                    .Find(x => x.Name == name).FirstOrDefault();
                if (item == null) return HttpStatusCode.NotFound;
                if (!raw && !string.IsNullOrWhiteSpace(item.HighlightLanguage))
                {
                    item.Content = hl
                        .Highlight(item.HighlightLanguage, item.Content);
                    item.RenderAsHtml = true;
                    return GetPastePage(item, raw);
                }
                else
                    return GetPastePage(item, raw);
            };
            Get["/(?<raw>paste|raw)/id/{id}"] = param =>
            {
                var raw = param.raw == "raw";
                ObjectId id;
                if (!ObjectId.TryParse(param.id, out id))
                    return HttpStatusCode.BadRequest;
                var item = PasteCollection
                    .Find(x => x._id == id).FirstOrDefault();
                if (item == null) return HttpStatusCode.NotFound;
                if (!raw && !string.IsNullOrWhiteSpace(item.HighlightLanguage))
                {
                    item.Content = hl
                        .Highlight(item.HighlightLanguage, item.Content);
                    item.RenderAsHtml = true;
                    return GetPastePage(item, raw);
                }
                else
                    return GetPastePage(item, raw);
            };
            // Deletion
            Delete["/(?<raw>paste|raw)/id/{id}"] = DeleteRoute;
            Get["/delete/id/{id}"] = DeleteRoute;
            // Posting a paste
            Post["/paste"] = _ =>
            {
                if (pasteNeedsAuth)
                    this.RequiresClaims("Paste");

                var p = this.Bind<Paste>("UserName", "Posted", "_id",
                    "RenderAsHtml");
                p.Name = string.IsNullOrWhiteSpace(p.Name) ?
                    Path.GetRandomFileName() : p.Name;
                p.Posted = DateTime.Now;
                p.UserName = Context.CurrentUser?.UserName;

                // keep generating new names until one works
                var inserted = false;
                do
                {
                    var likeItem = PasteCollection
                        .Find(x => x.Name == p.Name).FirstOrDefault();
                    if (likeItem == null)
                    {
                        PasteCollection.InsertOne(p);
                        inserted = true;
                    }
                    else if (likeItem.Name == p.Name && 
                             likeItem.UserName == p.UserName)
                    {
                        // if we're the same person, we can just replace inline
                        p._id = likeItem._id;
                        PasteCollection
                            .ReplaceOne(x => x._id == likeItem._id, p);
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
            // TODO: Pagination
            Get["/pastes"] = _ =>
            {
                var items = PasteCollection
                    .Find(x => x.Public).Limit(25).ToList();
                return View["Pastes.cshtml", new
                {
                    Items = items,
                    Title = "Public Pastes",
                    Query = ""
                }];
            };
            Get["/pastes/own"] = _ =>
            {
                this.RequiresAuthentication();
                var username = Context.CurrentUser?.UserName;
                var items = PasteCollection
                    .Find(x => x.UserName == username)
                    .Limit(25).ToList();
                return View["Pastes.cshtml", new {
                    Items = items,
                    Title = "Your Pastes",
                    Query = ""
                }];
            };
            Post["/pastes/search"] = _ =>
            {
                var query = (string)Request.Form.Query;
                var username = Context.CurrentUser?.UserName;
                var items = PasteCollection
                    .Find(x =>
                        (x.Public == true ||
                            (username != null && x.UserName == username)) &&
                        (x.Name.Contains(query) || x.Content.Contains(query)))
                    .Limit(25).ToList();
                return View["Pastes.cshtml", new
                {
                    Items = items,
                    Title = "Results for " + query,
                    Query = query
                }];
            };
#if DEBUG
            // Debug routes
            Get["/debug/auth"] = _ =>
            {
                this.RequiresAuthentication();
                return string.Format("User: {0}; Claims: {1}",
                    Context.CurrentUser?.UserName,
                    string.Join(", ",Context.CurrentUser?.Claims));
            };
#endif
        }

        dynamic DeleteRoute(dynamic param)
        {
            this.RequiresAuthentication();

            ObjectId id;
            if (!ObjectId.TryParse(param.id, out id))
                return HttpStatusCode.BadRequest;
            var item = PasteCollection
                .Find(x => x._id == id).FirstOrDefault();

            if (item.UserName == Context.CurrentUser?.UserName ||
                (Context.CurrentUser?.Claims.Contains("DeleteAny") ?? false))
                PasteCollection
                    .DeleteOne(x => x._id == item._id);
            else return HttpStatusCode.Unauthorized;

            return Response.AsRedirect(
                Request.Headers.Referrer.Contains(id.ToString())
                ? "/" : Request.Headers.Referrer );
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
                    return View["Paste.cshtml", paste];
            else
                return HttpStatusCode.NotFound;
        }
    }
}