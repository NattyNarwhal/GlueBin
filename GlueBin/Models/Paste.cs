using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlueBin.Models
{
    public class Paste
    {
        public ObjectId _id;

        public string Name { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public string HighlightLanguage { get; set; }
        public DateTime Posted { get; set; }
        public bool Public { get; set; } 

        // always false at storage time, this is only set at runtime
        // for rendering
        public bool RenderAsHtml { get; set; }
    }
}