using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlueBin.Models
{
    /// <summary>
    /// Represents a paste and its metadata.
    /// </summary>
    public class Paste
    {
        /// <summary>
        /// The MongoDB object ID. This must be unique.
        /// </summary>
        public ObjectId _id;

        /// <summary>
        /// The name of the paste. This must be unique.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The contents of the paste.
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// The username of the user who posted this paste.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// The syntax highlighting language that the paste uses.
        /// </summary>
        /// <remarks>
        /// This must be a name that <see cref="Highlight.Highlighter"/>
        /// understands.
        /// </remarks>
        public string HighlightLanguage { get; set; }
        /// <summary>
        /// The time the paste was updated.
        /// </summary>
        public DateTime Posted { get; set; }
        /// <summary>
        /// If the paste should be shown publically in listings.
        /// </summary>
        public bool Public { get; set; } 

        /// <summary>
        /// If the content should be escaped when rendered or not.
        /// </summary>
        /// <remarks>
        /// This is always false when stored - that is, the variable is only
        /// set during rendering.
        /// </remarks>
        public bool RenderAsHtml { get; set; }
    }
}