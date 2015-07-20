using System;
using System.Collections.Generic;
using System.Text;

namespace RssToolkit.Rss
{
    /// <summary>
    /// Represents an Enumeration of Document Types supported
    /// </summary>
    public enum DocumentType
    {
        /// <summary>
        /// Unknown document Type
        /// </summary>
        Unknown,

        /// <summary>
        /// Rss Document Type
        /// </summary>
        Rss,

        /// <summary>
        /// Opml Document Type
        /// </summary>
        Opml,

        /// <summary>
        /// Atom Document Type
        /// </summary>
        Atom,

        /// <summary>
        /// Rdf/Rss 1.0 Document Type
        /// </summary>
        Rdf
    }
}
