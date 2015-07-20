/*=======================================================================
  Copyright (C) Microsoft Corporation.  All rights reserved.

  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
=======================================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Serialization;
using RssToolkit.Rss;

namespace RssToolkit.Rss
{
    /// <summary>
    /// helper class that provides memory and disk caching of the downloaded feeds
    /// </summary>
    public class DownloadManager
    {
        private static DownloadManager _downloadManager = new DownloadManager();
        private readonly Dictionary<string, CacheInfo> _cache;
        private readonly int _defaultTtlMinutes;
        private readonly string _directoryOnDisk;

        private DownloadManager()
        {
            // create in-memory cache
            _cache = new Dictionary<string, CacheInfo>();

            // get default ttl value from config
            _defaultTtlMinutes = GetTtlFromString(ConfigurationManager.AppSettings["defaultRssTtl"], 1);

            // prepare disk directory
            _directoryOnDisk = PrepareTempDir();
        }

        /// <summary>
        /// Gets the Feed information.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>string</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
        public static Stream GetFeed(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("url");
            }

            return _downloadManager.GetFeedDom(url).Data;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static string PrepareTempDir()
        {
            try
            {
                string tempDir = ConfigurationManager.AppSettings["rssTempDir"];

                if (string.IsNullOrEmpty(tempDir))
                {
                    if (HostingEnvironment.IsHosted)
                    {
                        tempDir = HttpRuntime.CodegenDir;
                    }
                    else
                    {
                        tempDir = Environment.GetEnvironmentVariable("TEMP");

                        if (string.IsNullOrEmpty(tempDir))
                        {
                            tempDir = Environment.GetEnvironmentVariable("TMP");

                            if (string.IsNullOrEmpty(tempDir))
                            {
                                tempDir = Directory.GetCurrentDirectory();
                            }
                        }
                    }

                    tempDir = Path.Combine(tempDir, "rss");
                }

                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                return tempDir;
            }
            catch
            {
                // don't cache on disk if can't do it
                return null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static string GetTempFileNamePrefixFromUrl(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}_{1:x8}",
                    uri.Host.Replace('.', '_'),
                    StableHash(uri.AbsolutePath));
            }
            catch (UriFormatException)
            {
                return "rss";
            }
        }

        // returns a hash that is always the same (unlink String.GetHashCode() which varies)
        public static int StableHash(string value)
        {
            int hash = 0x61E04917; // slurped from .Net runtime internals...
            foreach (char c in value)
            {
                hash <<= 5;
                hash ^= c;
            }

            return hash;
        }

        private static int GetTtlFromString(string ttlString, int defaultTtlMinutes)
        {
            if (!string.IsNullOrEmpty(ttlString))
            {
                int ttlMinutes;
                if (int.TryParse(ttlString, out ttlMinutes))
                {
                    if (ttlMinutes >= 0)
                    {
                        return ttlMinutes;
                    }
                }
            }

            return defaultTtlMinutes;
        }

        private CacheInfo GetFeedDom(string url)
        {
            CacheInfo dom = null;

            lock (_cache)
            {
                if (_cache.TryGetValue(url, out dom))
                {
                    if (DateTime.UtcNow > dom.Expiry)
                    {
                        TryDeleteFile(dom.FeedFilename);
                        _cache.Remove(url);
                        dom = null;
                    }
                }
            }

            if (!CacheReadable(dom))
            {
                dom = DownLoadFeedDom(url);

                lock (_cache)
                {
                    _cache[url] = dom;
                }
            }

            return dom;
        }

        private static bool CacheReadable(CacheInfo dom)
        {
            return (dom != null && dom.Data != null && dom.Data.CanRead);
        }

        private CacheInfo DownLoadFeedDom(string url)
        {
            //// look for disk cache first
            CacheInfo dom = TryLoadFromDisk(url);

            if (CacheReadable(dom))
            {
                return dom;
            }

            string ttlString = null;
            XmlDocument doc = new XmlDocument();
            doc.Load(url);

            if (doc.SelectSingleNode("/rss/channel/ttl") != null)
            {
                ttlString = doc.SelectSingleNode("/rss/channel/ttl").Value;
            }

            //// compute expiry
            int ttlMinutes = GetTtlFromString(ttlString, _defaultTtlMinutes);
            DateTime utcExpiry = DateTime.UtcNow.AddMinutes(ttlMinutes);

            //// save to disk
            return TrySaveToDisk(doc, url, utcExpiry);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private CacheInfo TryLoadFromDisk(string url)
        {
            if (_directoryOnDisk == null)
            {
                return null; // no place to cache
            }

            CacheInfo found = null;

            // look for all files matching the prefix
            // looking for the one matching url that is not expired
            // removing expired (or invalid) ones
            string pattern = GetTempFileNamePrefixFromUrl(url) + "_*.feed";
            string[] files = Directory.GetFiles(_directoryOnDisk, pattern, SearchOption.TopDirectoryOnly);

            foreach (string feedFilename in files)
            {
                XmlDocument feedDoc = null;
                bool isFeedFileValid = false;
                DateTime utcExpiryFromFeedFile = DateTime.MinValue;
                string urlFromFeedFile = null;

                try
                {
                    feedDoc = new XmlDocument();
                    feedDoc.Load(feedFilename);

                    // look for special XML comment (before the root tag)'
                    // containing expiration and url
                    XmlComment comment = feedDoc.DocumentElement.PreviousSibling as XmlComment;

                    if (comment != null)
                    {
                        string c = comment.Value ?? string.Empty;
                        int i = c.IndexOf('@');
                        long expiry;

                        if (i >= 0 && long.TryParse(c.Substring(0, i), out expiry))
                        {
                            utcExpiryFromFeedFile = DateTime.FromBinary(expiry);
                            urlFromFeedFile = c.Substring(i + 1);
                            isFeedFileValid = true;
                        }
                    }
                }
                catch (XmlException)
                {
                    // error processing one file shouldn't stop processing other files
                }

                // remove invalid or expired file
                if (!isFeedFileValid || utcExpiryFromFeedFile < DateTime.UtcNow)
                {
                    TryDeleteFile(feedFilename);
                    // try next file
                    continue;
                }

                // match url
                if (urlFromFeedFile == url)
                {
                    // keep the one that expires last
                    if (found == null || found.Expiry < utcExpiryFromFeedFile)
                    {
                        // if we have a previously found older expiration, kill it...
                        if (found != null)
                        {
                            found.Dispose();
                            TryDeleteFile(found.FeedFilename);
                        }

                        // create DOM and set expiry (as found on disk)
                        found = new CacheInfo(feedDoc, utcExpiryFromFeedFile, feedFilename);
                    }
                }
            }

            // return best fit
            return found;
        }

        private void TryDeleteFile(string feedFilename)
        {
            if (string.IsNullOrEmpty(feedFilename))
                return;

            try
            {
                File.Delete(feedFilename);
            }
            catch
            {
                // error deleting a file shouldn't stop processing other files
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private CacheInfo TrySaveToDisk(XmlDocument doc, string url, DateTime utcExpiry)
        {
            string fileName = null;

            if (_directoryOnDisk != null)
            {
                XmlComment comment = doc.CreateComment(string.Format(CultureInfo.InvariantCulture, "{0}@{1}", utcExpiry.ToBinary(), url));
                doc.InsertBefore(comment, doc.DocumentElement);

                fileName = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}_{1:x8}.feed",
                    GetTempFileNamePrefixFromUrl(url),
                    Guid.NewGuid().GetHashCode());

                try
                {
                    doc.Save(Path.Combine(_directoryOnDisk, fileName));
                }
                catch
                {
                    // can't save to disk - not a problem   
                    fileName = null;
                }
            }

            return new CacheInfo(doc, utcExpiry, fileName);
        }

        private class CacheInfo : IDisposable
        {
            private readonly Stream data;
            private readonly DateTime expiry;
            private readonly string filename;

            public CacheInfo(XmlDocument doc, DateTime utcExpiry, string feedFilename)
            {
                MemoryStream documentStream = new MemoryStream();
                doc.Save(documentStream);
                documentStream.Flush();
                documentStream.Position = 0;
                this.data = documentStream;
                this.expiry = utcExpiry;
                this.filename = feedFilename;
            }

            /// <summary>
            /// Gets the expiration time in UTC.
            /// </summary>
            /// <value>The expiry.</value>
            public DateTime Expiry
            {
                get
                {
                    return expiry;
                }
            }

            /// <summary>
            /// Gets the data stream
            /// </summary>
            /// <value>The data.</value>
            public Stream Data
            {
                get
                {
                    return data;
                }
            }

            /// <summary>
            /// Gets the filename 
            /// </summary>
            public string FeedFilename
            {
                get
                {
                    return filename;
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                data.Dispose();
            }
        }
    }
}
