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
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml.Serialization;

namespace RssToolkit.Rss
{
    /// <summary>
    /// Helper class
    /// </summary>
    public static class RssXmlHelper
    {
        private const string TimeZoneCacheKey = "DateTimeParser";

        /// <summary>
        /// Resolves the app relative link to URL.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <returns>string</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        public static string ResolveAppRelativeLinkToUrl(string link)
        {
            if (string.IsNullOrEmpty(link))
            {
                throw new ArgumentException("link");
            }

            if (!string.IsNullOrEmpty(link) && link.StartsWith("~/"))
            {
                HttpContext context = HttpContext.Current;

                if (context != null)
                {
                    string query = string.Empty;
                    int iquery = link.IndexOf('?');

                    if (iquery >= 0)
                    {
                        query = link.Substring(iquery);
                        link = link.Substring(0, iquery);
                    }

                    link = VirtualPathUtility.ToAbsolute(link);
                    link = new Uri(context.Request.Url, link).ToString() + query;
                }
            }

            return link;
        }

        /// <summary>
        /// Deserialize from XML using string reader.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <typeparam name="T">RssDocumentBase</typeparam>
        /// <returns>T</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T DeserializeFromXmlUsingStringReader<T>(string xml) where T : RssDocumentBase
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentException("xml");
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Returns XML of the Generic Type.
        /// </summary>
        /// <param name="rssDocument">The RSS document.</param>
        /// <typeparam name="T">RssDocumentBase</typeparam>
        /// <returns>string</returns>
        public static string ToRssXml<T>(T rssDocument) where T : RssDocumentBase
        {
            if (rssDocument == null)
            {
                throw new ArgumentNullException("rssDocument");
            }

            using (StringWriter output = new StringWriter(new StringBuilder(), CultureInfo.InvariantCulture))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(output, rssDocument);
                return output.ToString();
            }
        }

        /// <summary>
        /// Gets the type of the document.
        /// </summary>
        /// <param name="nodeName">Node Name</param>
        /// <returns>RssDocument</returns>
        public static DocumentType GetDocumentType(string nodeName)
        {
            if (string.IsNullOrEmpty(nodeName))
            {
                throw new ArgumentException("nodeName");
            }

            if (nodeName.Equals("rss", StringComparison.OrdinalIgnoreCase))
            {
                return DocumentType.Rss;
            }
            else if (nodeName.Equals("opml", StringComparison.OrdinalIgnoreCase))
            {
                return DocumentType.Opml;
            }
            else if (nodeName.Equals("feed", StringComparison.OrdinalIgnoreCase))
            {
                return DocumentType.Atom;
            }
            else if (nodeName.Equals("rdf", StringComparison.OrdinalIgnoreCase))
            {
                return DocumentType.Rdf;
            }

            return DocumentType.Unknown;
        }

        /// <summary>
        /// Converts to RSS XML.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Xml string in Rss Format</returns>
        public static string ConvertToRssXml(string input)
        {
            using (StringReader stringReader = new StringReader(input))
            {
                using (XmlTextReader reader = new XmlTextReader(stringReader))
                {
                    return ConvertToRssXml(reader);
                }
            }
        }

        /// <summary>
        /// Converts to RSS XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Xml string in Rss Format</returns>
        public static string ConvertToRssXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            string rssXml = string.Empty;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    DocumentType feedType = RssXmlHelper.GetDocumentType(reader.LocalName);
                    string outerXml = reader.ReadOuterXml();
                    switch (feedType)
                    {
                        case DocumentType.Rss:
                            rssXml = outerXml;
                            break;
                        case DocumentType.Opml:
							throw new NotImplementedException();
							//RssAggregator aggregator = new RssAggregator();
							//aggregator.Load(outerXml);
							//rssXml = aggregator.RssXml;
                            //break;
                        case DocumentType.Atom:
                            //rssXml = DoXslTransform(outerXml, GetStreamFromResource(Constants.AtomToRssXsl));
                            break;
                        case DocumentType.Rdf:
                            //rssXml = DoXslTransform(outerXml, GetStreamFromResource(Constants.RdfToRssXsl));
                            break;
                    }

                    break;
                }
            }

            return rssXml;
        }

        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <value>The data set.</value>
        /// <param name="xml">XML</param>
        /// <returns>DataSet</returns>
        public static DataSet ToDataSet(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentException("xml");
            }

            DataSet dataset = new DataSet();
            dataset.Locale = CultureInfo.InvariantCulture;
            using (StringReader stringReader = new StringReader(xml))
            {
                dataset.ReadXml(stringReader);
            }

            return dataset;
        }

        /// <summary>
        /// Loads the RSS from opml URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <typeparam name="T">RssDocumentBase</typeparam>
        /// <returns>Generic of RssDocumentBase</returns>
		//[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
		//public static T LoadRssFromOpmlUrl<T>(string url) where T : RssDocumentBase
		//{
		//    //if (string.IsNullOrEmpty(url))
		//    //{
		//    //    throw new ArgumentException("url");
		//    //}

		//    //// resolve app-relative URLs
		//    //url = RssXmlHelper.ResolveAppRelativeLinkToUrl(url);

		//    //RssAggregator aggregator = new RssAggregator();
		//    //aggregator.Load(new System.Uri(url));
		//    //string rssXml = aggregator.RssXml;

		//    //return RssXmlHelper.DeserializeFromXmlUsingStringReader<T>(rssXml);
			
		//}

        /// <summary>
        /// Parse is able to parse RFC2822/RFC822 formatted dates.
        /// It has a fallback mechanism: if the string does not match,
        /// the normal DateTime.Parse() function is called.
        /// 
        /// Copyright of RssBandit.org
        /// Author - t_rendelmann
        /// </summary>
        /// <param name="dateTime">Date Time to parse</param>
        /// <returns>DateTime instance</returns>
        /// <exception cref="FormatException">On format errors parsing the DateTime</exception>
        public static DateTime Parse(string dateTime)
        {
            Regex rfc2822 = new Regex(@"\s*(?:(?:Mon|Tue|Wed|Thu|Fri|Sat|Sun)\s*,\s*)?(\d{1,2})\s+(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s+(\d{2,})\s+(\d{2})\s*:\s*(\d{2})\s*(?::\s*(\d{2}))?\s+([+\-]\d{4}|UT|GMT|EST|EDT|CST|CDT|MST|MDT|PST|PDT|[A-IK-Z])", RegexOptions.Compiled);
            ArrayList months = new ArrayList(new string[] { "ZeroIndex", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" });

            if (dateTime == null)
            {
                return DateTime.Now.ToUniversalTime();
            }

            if (dateTime.Trim().Length == 0)
            {
                return DateTime.Now.ToUniversalTime();
            }

            Match m = rfc2822.Match(dateTime);
            if (m.Success)
            {
                try
                {
                    int dd = Int32.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                    int mth = months.IndexOf(m.Groups[2].Value);
                    int yy = Int32.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture);
                    //// following year completion is compliant with RFC 2822.
                    yy = (yy < 50 ? 2000 + yy : (yy < 1000 ? 1900 + yy : yy));
                    int hh = Int32.Parse(m.Groups[4].Value, CultureInfo.InvariantCulture);
                    int mm = Int32.Parse(m.Groups[5].Value, CultureInfo.InvariantCulture);
                    int ss = Int32.Parse(m.Groups[6].Value, CultureInfo.InvariantCulture);
                    string zone = m.Groups[7].Value;

                    DateTime xd = new DateTime(yy, mth, dd, hh, mm, ss);
                    return xd.AddHours(RFCTimeZoneToGMTBias(zone) * -1);
                }
                catch (FormatException)
                {
                    throw;
                }
            }
            else
            {
                // fall-back, if regex does not match:
                return DateTime.Parse(dateTime, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Converts a DateTime to a valid RFC 2822/822 string
        /// </summary>
        /// <param name="dt">The DateTime to convert, recognizes Zulu/GMT</param>
        /// <returns>Returns the local time in RFC format with the time offset properly appended</returns>
        public static string ToRfc822(DateTime dt)
        {
            string timeZone;

            if (dt.Kind == DateTimeKind.Utc)
            {
                timeZone = "Z";
            }
            else
            {
                TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset(dt);

                if (offset.Ticks < 0)
                {
                    offset = -offset;
                    timeZone = "-";
                }
                else
                {
                    timeZone = "+";
                }

                timeZone += offset.Hours.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
                timeZone += offset.Minutes.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
            }

            return dt.ToString("ddd, dd MMM yyyy HH:mm:ss " + timeZone, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Does the XSL transform.
        /// </summary>
        /// <param name="inputXml">The input XML.</param>
        /// <param name="xslResource">The XSL resource.</param>
        /// <returns></returns>
        public static string DoXslTransform(string inputXml, Stream xslResource)
        {
            ///TODO Make the culture an argument
            using (StringWriter outputWriter = new StringWriter(System.Threading.Thread.CurrentThread.CurrentUICulture))
            {
                using (StringReader stringReader = new StringReader(inputXml))
                {
                    XPathDocument xpathDoc = new XPathDocument(stringReader);
                    XslCompiledTransform transform = new XslCompiledTransform();
                    using (XmlTextReader styleSheetReader = new XmlTextReader(xslResource))
                    {
                        transform.Load(styleSheetReader);
                        transform.Transform(xpathDoc, null, outputWriter);
                    }
                }

                return outputWriter.ToString();
            }
        }

        /// <summary>
        /// Changes Time zone based on GMT
        /// 
        /// Copyright of RssBandit.org
        /// Author - t_rendelmann
        /// </summary>
        /// <param name="zone">The zone.</param>
        /// <returns>RFCTimeZoneToGMTBias</returns>
        private static double RFCTimeZoneToGMTBias(string zone)
        {
            Dictionary<string, int> timeZones = null;

            if (HttpContext.Current != null)
            {
                timeZones = HttpContext.Current.Cache[TimeZoneCacheKey] as Dictionary<string, int>;
            }

            if (timeZones == null)
            {
                timeZones = new Dictionary<string, int>();
                timeZones.Add("GMT", 0);
                timeZones.Add("UT", 0);
                timeZones.Add("EST", -5 * 60);
                timeZones.Add("EDT", -4 * 60);
                timeZones.Add("CST", -6 * 60);
                timeZones.Add("CDT", -5 * 60);
                timeZones.Add("MST", -7 * 60);
                timeZones.Add("MDT", -6 * 60);
                timeZones.Add("PST", -8 * 60);
                timeZones.Add("PDT", -7 * 60);
                timeZones.Add("Z", 0);
                timeZones.Add("A", -1 * 60);
                timeZones.Add("B", -2 * 60);
                timeZones.Add("C", -3 * 60);
                timeZones.Add("D", -4 * 60);
                timeZones.Add("E", -5 * 60);
                timeZones.Add("F", -6 * 60);
                timeZones.Add("G", -7 * 60);
                timeZones.Add("H", -8 * 60);
                timeZones.Add("I", -9 * 60);
                timeZones.Add("K", -10 * 60);
                timeZones.Add("L", -11 * 60);
                timeZones.Add("M", -12 * 60);
                timeZones.Add("N", 1 * 60);
                timeZones.Add("O", 2 * 60);
                timeZones.Add("P", 3 * 60);
                timeZones.Add("Q", 4 * 60);
                timeZones.Add("R", 3 * 60);
                timeZones.Add("S", 6 * 60);
                timeZones.Add("T", 3 * 60);
                timeZones.Add("U", 8 * 60);
                timeZones.Add("V", 3 * 60);
                timeZones.Add("W", 10 * 60);
                timeZones.Add("X", 3 * 60);
                timeZones.Add("Y", 12 * 60);

                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Cache.Insert(TimeZoneCacheKey, timeZones);
                }
            }

            if (zone.IndexOfAny(new char[] { '+', '-' }) == 0)  // +hhmm format
            {
                int sign = (zone[0] == '-' ? -1 : 1);
                string s = zone.Substring(1).TrimEnd();
                int hh = Math.Min(23, Int32.Parse(s.Substring(0, 2), CultureInfo.InvariantCulture));
                int mm = Math.Min(59, Int32.Parse(s.Substring(2, 2), CultureInfo.InvariantCulture));
                return sign * (hh + (mm / 60.0));
            }
            else
            { // named format
                string s = zone.ToUpper(CultureInfo.InvariantCulture).Trim();
                foreach (string key in timeZones.Keys)
                {
                    if (key.Equals(s))
                    {
                        return timeZones[key] / 60.0;
                    }
                }
            }

            return 0.0;
        }

        /// <summary>
        /// Converts the atom to RSS.
        /// </summary>
        /// <param name="documentType">Rss Document Type</param>
        /// <param name="inputXml">The input XML.</param>
        /// <returns>string</returns>
        public static string ConvertRssTo(DocumentType ouputType, string rssXml)
        {
            if (string.IsNullOrEmpty(rssXml))
            {
                return null;
            }

            switch (ouputType)
            {
                case DocumentType.Rss:
                    return rssXml;
                case DocumentType.Atom:
					//using (Stream stream = GetStreamFromResource(Constants.RssToAtomXsl))
					//{
					//    return RssXmlHelper.DoXslTransform(rssXml, stream);
					//}
                case DocumentType.Rdf:
					//using (Stream stream = GetStreamFromResource(Constants.RssToRdfXsl))
					//{
					//    return RssXmlHelper.DoXslTransform(rssXml, stream);
					//}
                case DocumentType.Opml:
					//using (Stream stream = GetStreamFromResource(Constants.RssToOpmlXsl))
					//{
					//    return RssXmlHelper.DoXslTransform(rssXml, stream);
					//}
                default:
                    return null;
            }
        }

        private static Stream GetStreamFromResource(string resourceFileName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            if (assembly != null)
            {
                return assembly.GetManifestResourceStream(resourceFileName);
            }

            return null;
        }
    }
}
