using System;
using System.Collections;
using System.Collections.Specialized;
using Mediachase.IBN.Database.EMail;
using Mediachase.Net.Mail;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Text;
using Mediachase.Ibn;
using System.Globalization;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for EMailMessageInfo.
	/// </summary>
	public class EMailMessageInfo
	{
		private int _EMailMessageId;
		private NameValueCollection _headers = new NameValueCollection();
		private AttachmentInfo[] _attachments = null;
		private string _htmlBody = null;
		private string _senderEmail = string.Empty;
		private string _senderName = string.Empty;
		private int _emailBoxId = -1;
		private DateTime _created;

		#region Mediachase.UI.Web.Util.CommonHelper
		private static string GetAbsolutePath(string xs_Path)
		{
			if (HttpContext.Current != null)
			{
				StringBuilder builder = new StringBuilder();

				string UrlScheme = System.Configuration.ConfigurationManager.AppSettings["UrlScheme"];

				if (UrlScheme != null)
					builder.Append(UrlScheme);
				else
					builder.Append(HttpContext.Current.Request.Url.Scheme);
				builder.Append("://");

				// Oleg Rylin: Fixing the problem with non-default port [6/20/2006]
				builder.Append(HttpContext.Current.Request.Url.Authority);

				builder.Append(HttpContext.Current.Request.ApplicationPath);
				builder.Append("/");
				if (xs_Path != string.Empty)
				{
					if (xs_Path[0] == '/')
						xs_Path = xs_Path.Substring(1, xs_Path.Length - 1);
					builder.Append(xs_Path);
				}
				return builder.ToString();
			}
			else
			{
				return (Configuration.PortalLink + xs_Path);
			}
		}
		#endregion

		public class AttachmentData
		{
			public string ContentType;
			public string FileName;
			public byte[] Data;

			public static AttachmentData Create(MimeEntry entry)
			{
				AttachmentData retVal = new AttachmentData();

				retVal.ContentType = entry.ContentType;
				retVal.FileName = EMailMessageInfo.GetFileName(entry);
				retVal.Data = entry.Body;

				return retVal;
			}
		}

		/// <summary>
		/// Pushes the attachment to web response.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <param name="EMailMessageId">The E mail message id.</param>
		/// <param name="AttachementIndex">The attachement id.</param>
		public static void PushAttachmentToWebResponse(System.Web.HttpResponse response, int EMailMessageId, int AttachementIndex)
		{
			EMailMessageRow row = new EMailMessageRow(EMailMessageId);

			MemoryStream memStream = new MemoryStream(row.EmlMessage.Length);
			memStream.Write(row.EmlMessage, 0, row.EmlMessage.Length);
			memStream.Position = 0;

			Pop3Message message = new Pop3Message(memStream);

			AttachmentData entry = GetAttachment(message.MimeEntries, ref AttachementIndex);

			response.ContentType = entry.ContentType;
			//response.AddHeader("content-disposition", String.Format("attachment; filename=\"{0}\"", GetFileName(entry)));
			if (Common.OpenInNewWindow(entry.ContentType))
				response.AddHeader("content-disposition", String.Format("inline; filename=\"{0}\"", entry.FileName));
			else
				response.AddHeader("content-disposition", String.Format("attachment; filename=\"{0}\"", entry.FileName));


			response.OutputStream.Write(entry.Data, 0, entry.Data.Length);
			response.OutputStream.Flush();

			response.End();
		}

		/// <summary>
		/// Pushes the embedded resource to web response.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <param name="EMailMessageId">The E mail message id.</param>
		/// <param name="Cid">The cid.</param>
		public static void PushEmbeddedResourceToWebResponse(System.Web.HttpResponse response, int EMailMessageId, string Cid)
		{
			EMailMessageRow row = new EMailMessageRow(EMailMessageId);

			MemoryStream memStream = new MemoryStream(row.EmlMessage.Length);
			memStream.Write(row.EmlMessage, 0, row.EmlMessage.Length);
			memStream.Position = 0;

			///////////////// Test
			// E:\Util\BlobDataExtractor\output24700.eml
			//System.IO.StreamReader st = new System.IO.StreamReader(@"D:\EMailMessage_55325.eml", System.Text.Encoding.Default);
			//string strmsg = st.ReadToEnd();
			//byte[] buffer = System.Text.Encoding.Default.GetBytes(strmsg);
			//System.IO.MemoryStream memStream = new System.IO.MemoryStream(buffer, 0, buffer.Length, true, true);
			///////////////// 

			Pop3Message message = new Pop3Message(memStream);

			MimeEntry entry = GetAttachment(message.MimeEntries, Cid);

			if (entry == null)
			{
				//throw new HttpException(404, string.Empty);
				response.Clear();
				response.StatusCode = 404;
				response.Status = "File " + Cid + " not found";
			}
			else
			{
				response.ContentType = entry.ContentType;
				response.AddHeader("content-disposition", String.Format("attachment; filename=\"{0}\"", GetFileName(entry)));

				response.OutputStream.Write(entry.Body, 0, entry.Body.Length);
				response.OutputStream.Flush();
			}

			response.End();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EMailMessageInfo"/> class.
		/// </summary>
		/// <param name="EMailMessageId">The E mail message id.</param>
		protected EMailMessageInfo(int EMailMessageId)
		{
			_EMailMessageId = EMailMessageId;

			// Load Message Row
			EMailMessageRow row = new EMailMessageRow(EMailMessageId);

			///////////////// Test
			// E:\Util\BlobDataExtractor\output24700.eml
			//System.IO.StreamReader st = new System.IO.StreamReader(@"D:\EMailMessage_72848.eml", System.Text.Encoding.Default);
			//string strmsg = st.ReadToEnd();
			//byte[] buffer = System.Text.Encoding.Default.GetBytes(strmsg);
			//System.IO.MemoryStream memStream = new System.IO.MemoryStream(buffer, 0, buffer.Length, true, true);
			/////////////// 

			_created = Database.DBCommon.GetLocalDate(Security.CurrentUser.TimeZoneId, row.Created);

			MemoryStream memStream = new MemoryStream(row.EmlMessage.Length);
			memStream.Write(row.EmlMessage, 0, row.EmlMessage.Length);
			memStream.Position = 0;

			Pop3Message message = new Pop3Message(memStream);

			// Load Headers
			_headers.Add(message.Headers);

			// Extract Html Body
			_htmlBody = ConvertBody(message);

			// Extract Attachments
			_attachments = (AttachmentInfo[])ExtractAttachments(message.MimeEntries).ToArray(typeof(AttachmentInfo));

			_senderEmail = EMailMessage.GetSenderEmail(message);
			_senderName = EMailMessage.GetSenderName(message);

			_emailBoxId = row.EMailRouterPop3BoxId;
		}

		/// <summary>
		/// Converts the body.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		private string ConvertBody(Pop3Message message)
		{
			string retVal = message.BodyHtml;
			if (retVal != null)
			{
				// Step 1. Extract Body
				Match match = Regex.Match(retVal, @"<body[^>]*>(?<HtmlBody>[\s\S]*?)</body>", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

				if (match.Success)
				{
					retVal = match.Groups["HtmlBody"].Value;
				}

				// Step 2. Remove SCRIPT
				retVal = Regex.Replace(retVal, @"<script[^>]*>[\s\S]*?</script>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

				// Step 3. Replace "cid:link"
				retVal = Regex.Replace(retVal, "(\"|')cid:(?<cid>([^\"']+))(\"|')", new MatchEvaluator(OnReplaceCidMatchEvaluator), RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

				// Step 4. Replace cid:link
				retVal = Regex.Replace(retVal, "=[\x20]*cid:(?<cid>([^ >]+))", new MatchEvaluator(OnReplaceCidMatchEvaluatorWithoutScope), RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

				// Step 5 (OZ: 2008-07-02): Remove Input, Button, TextArea, Object
				retVal = Regex.Replace(retVal, @"</?(form|input|button|textarea|object)[^>]*>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
			}
			else if (message.BodyText != null)
			{
				retVal = HttpUtility.HtmlEncode(message.BodyText);
				retVal = retVal.Replace("\r\n", "<br />");
				retVal = retVal.Replace("\n", "<br />");
			}
			else
				retVal = String.Empty;

			return retVal;
		}

		/// <summary>
		/// Extracts the text from HTML.
		/// </summary>
		/// <param name="html">The HTML.</param>
		/// <returns></returns>
		public static string ExtractTextFromHtml(string html)
		{
			if (string.IsNullOrEmpty(html))
				return string.Empty;

			string retVal = string.Empty;
			// Step 1. Extract Body
			Match match = Regex.Match(html, @"<body[^>]*>(?<HtmlBody>[\s\S]*?)</body>", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

			if (match.Success && match.Groups["HtmlBody"].Value != null)
			{
				retVal = match.Groups["HtmlBody"].Value;
			}
			else
			{
				retVal = html;
			}

			// Step 2. Remove SCRIPT
			retVal = Regex.Replace(retVal, @"<script[^>]*>[\s\S]*?</script>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

			// Step 3. Remove Comment
			retVal = Regex.Replace(retVal, "<!--.*?-->", string.Empty,
				RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

			// Replace Br to \r\n
			retVal = Regex.Replace(retVal, "<BR[^>]*/?>", "\r\n",
				RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

			// Step 4. Remove Tags
			retVal = Regex.Replace(retVal, "</?(?<TagName>([A-Z][A-Z0-9]*))[^>]*/?>", " ",
				RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);


			// 2008-09-18 Fix Html Decoded chars
			retVal = HttpUtility.HtmlDecode(retVal);

			return retVal;
		}

		/// <summary>
		/// Extracts the text from HTML.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		public static string ExtractTextFromHtml(Pop3Message message)
		{
			if (message == null)
				throw new ArgumentNullException("message");

			string htmlBody = message.BodyHtml;

			if (htmlBody != null)
			{
				return ExtractTextFromHtml(htmlBody);
			}
			else
			{

				string textBody = message.BodyText;

				if (textBody != null)
				{
					return textBody;
				}

			}

			return string.Empty;
		}

		/// <summary>
		/// Called when [replace cid match evaluator].
		/// </summary>
		/// <param name="match">The match.</param>
		/// <returns></returns>
		private string OnReplaceCidMatchEvaluator(Match match)
		{
			string cid = match.Groups["cid"].Value;
			return "\"" + GetAbsolutePath(string.Format("/Incidents/EMailEmbeddedResource.aspx?EMailId={0}&Cid={1}",
				this.EMailMessageId,
				System.Web.HttpUtility.UrlEncode(cid))) + "\"";
		}

		/// <summary>
		/// Called when [replace cid match evaluator with scope].
		/// </summary>
		/// <param name="match">The match.</param>
		/// <returns></returns>
		private string OnReplaceCidMatchEvaluatorWithoutScope(Match match)
		{
			string cid = match.Groups["cid"].Value;
			return "=\"" + GetAbsolutePath(string.Format("/Incidents/EMailEmbeddedResource.aspx?EMailId={0}&Cid={1}",
				this.EMailMessageId,
				System.Web.HttpUtility.UrlEncode(cid))) + "\"";
		}

		/// <summary>
		/// Loads the specified E mail message id.
		/// </summary>
		/// <param name="EMailMessageId">The E mail message id.</param>
		/// <returns></returns>
		public static EMailMessageInfo Load(int EMailMessageId)
		{
			string category = "HelpDesk";
			string key = string.Format(CultureInfo.InvariantCulture,  "EMailMessage::{0}", EMailMessageId);

			EMailMessageInfo retVal;

			if (DataCache.TryGetValue<EMailMessageInfo>(category, DataCache.EmptyUser, key, out retVal))
				return retVal;

			retVal = new EMailMessageInfo(EMailMessageId);

			DataCache.Add(category, DataCache.EmptyUser, key, retVal);

			return retVal;
		}

		/// <summary>
		/// Gets the E mail message id.
		/// </summary>
		/// <value>The E mail message id.</value>
		public int EMailMessageId
		{
			get
			{
				return _EMailMessageId;
			}
		}

		/// <summary>
		/// Gets the created.
		/// </summary>
		/// <value>The created.</value>
		public DateTime Created
		{
			get
			{
				return _created;
			}
		}


		/// <summary>
		/// Gets the sender email.
		/// </summary>
		/// <value>The sender email.</value>
		public string SenderEmail
		{
			get
			{
				return _senderEmail;
			}
		}

		/// <summary>
		/// Gets the E mail router POP3 box id.
		/// </summary>
		/// <value>The E mail router POP3 box id.</value>
		public int EMailRouterPop3BoxId
		{
			get
			{
				return _emailBoxId;
			}
		}

		/// <summary>
		/// Gets the name of the sender.
		/// </summary>
		/// <value>The name of the sender.</value>
		public string SenderName
		{
			get
			{
				return _senderName;
			}
		}

		/// <summary>
		/// Gets from.
		/// </summary>
		/// <value>From.</value>
		public string From
		{
			get
			{
				return _headers["From"];
			}
		}

		/// <summary>
		/// Gets to.
		/// </summary>
		/// <value>To.</value>
		public string To
		{
			get
			{
				return _headers["To"];
			}
		}

		/// <summary>
		/// Gets the subject.
		/// </summary>
		/// <value>The subject.</value>
		public string Subject
		{
			get
			{
				return _headers["Subject"] == null ? string.Empty : _headers["Subject"];
			}
		}

		/// <summary>
		/// Gets the headers.
		/// </summary>
		/// <value>The headers.</value>
		public NameValueCollection Headers
		{
			get
			{
				return _headers;
			}
		}

		/// <summary>
		/// Gets the HTML body.
		/// </summary>
		/// <value>The HTML body.</value>
		public string HtmlBody
		{
			get
			{
				return _htmlBody;
			}
		}

		public static string CutHtmlBody(string HtmlBody, int MaxCharLength)
		{
			return CutHtmlBody(HtmlBody, MaxCharLength, null);
		}

		/// <summary>
		/// Gets the cut HTML body.
		/// </summary>
		/// <param name="MaxCharLength">Length of the max char.</param>
		/// <returns></returns>
		public static string CutHtmlBody(string HtmlBody, int MaxCharLength, string AppendSring)
		{
			if (HtmlBody.Length < MaxCharLength)
				return HtmlBody;

			// OZ [2007-03-02] Remove Html Comment
			HtmlBody = Regex.Replace(HtmlBody, "<!--.*?-->", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

			MatchCollection matchCol = Regex.Matches(HtmlBody, "</?(?<TagName>([A-Z][A-Z0-9]*))[^>]*/?>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

			if (matchCol.Count > 0)
			{
				int TotalCharCount = 0;
				int LastCharIndex = 0;

				Stack stack = new Stack();

				foreach (Match matchItem in matchCol)
				{
					string newTagName = matchItem.Groups["TagName"].Value;

					if ((TotalCharCount + (matchItem.Index - LastCharIndex)) > MaxCharLength)
					{
						LastCharIndex += (MaxCharLength - TotalCharCount);

						StringBuilder sb = new StringBuilder(LastCharIndex + 128);

						sb.Append(HtmlBody.Substring(0, LastCharIndex));

						if (AppendSring != null)
							sb.Append(AppendSring);

						while (stack.Count > 0)
						{
							string TagName = (string)stack.Pop();
							sb.AppendFormat("</{0}>", TagName);
						}

						return sb.ToString();
					}

					if (matchItem.Value.StartsWith("</"))
					{
						while (stack.Count > 0)
						{
							string TagName = (string)stack.Pop();

							if (string.Compare(TagName, newTagName, true) == 0)
								break;
						}
					}

					TotalCharCount += (matchItem.Index - LastCharIndex);

					if (!matchItem.Value.EndsWith("/>") &&
						!(string.Compare(newTagName, "BR", true) == 0 ||
						string.Compare(newTagName, "HR", true) == 0))
						stack.Push(newTagName);

					LastCharIndex = matchItem.Index + matchItem.Length;
				}

				return HtmlBody;
			}
			else
			{
				string str = HtmlBody.Substring(0, Math.Min(HtmlBody.Length, MaxCharLength));
				return str + AppendSring;
			}
		}

		/// <summary>
		/// Gets the attachments.
		/// </summary>
		/// <value>The attachments.</value>
		public AttachmentInfo[] Attachments
		{
			get
			{
				return _attachments;
			}
		}

		/// <summary>
		/// Gets the attachment.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public static AttachmentData GetAttachment(MimeEntryCollection MimeEntries, ref int index)
		{
			foreach (MimeEntry entry in MimeEntries)
			{
				if (entry.ContentDisposition == Disposition.Attachment ||
					entry.ContentDisposition == Disposition.Inline||
					entry.ContentType == "message/rfc822" ||
					entry.ContentType.StartsWith("image/", StringComparison.InvariantCultureIgnoreCase)) // OZ: Show attached image with out ContentDisposition
				{
					//if (entry.ContentType.StartsWith("text/html"))
					//    continue;

					if (entry.ContentType == "application/ms-tnef")
					{
						TnefParser tnefParser = new TnefParser(entry.Body);

						if (tnefParser.Parse())
						{
							int thefAttachCount = tnefParser.Attachments.Length;

							if (thefAttachCount > index)
							{
								AttachmentData retVal = new AttachmentData();

								retVal.FileName = tnefParser.Attachments[index].FileName;
								retVal.ContentType = tnefParser.Attachments[index].ContentType;
								retVal.Data = tnefParser.GetAttachmentBody(index);

								return retVal;
							}

							index -= (thefAttachCount - 1);
						}
						else
						{
							if (index == 0)
								return AttachmentData.Create(entry);
							index--;
						}
					}
					else
					{
						if (index == 0)
							return AttachmentData.Create(entry);
						index--;
					}
				}

				if (entry.MimeEntries.Count > 0)
				{
					AttachmentData retVal = GetAttachment(entry.MimeEntries, ref index);
					if (retVal != null)
						return retVal;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the attachment.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="cid">The cid.</param>
		/// <returns></returns>
		public static MimeEntry GetAttachment(MimeEntryCollection MimeEntries, string cid)
		{
			foreach (MimeEntry entry in MimeEntries)
			{
				string ContentID = entry.Headers["Content-ID"];
				if (ContentID != null)
				{
					if (ContentID.IndexOf(cid) != -1)
						return entry;
				}

				if (entry.MimeEntries.Count > 0)
				{
					MimeEntry retVal = GetAttachment(entry.MimeEntries, cid);
					if (retVal != null)
						return retVal;
				}

			}

			return null;
		}


		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <returns></returns>
		private static string GetFileName(MimeEntry entry)
		{
			string fileName = entry.FileName;
			if (fileName == null)
			{
				if (entry.ContentType != null)
				{
					if (entry.ContentType == "message/rfc822")
					{
						fileName = "Attached.eml";
					}
					else
					{
						Hashtable parametrs = new Hashtable();

						Regex contentParser = new Regex("(?<type>[^;]+)(;(\\s)*(?<parameter>((?<attribute>[^=]+)=(?<value>((\"[^\"]*\")?[^;]*)))))*");

						Match match = contentParser.Match(entry.Headers["Content-Type"]);

						string Type = match.Groups["type"].Value;

						int CaptureLen = match.Groups["parameter"].Captures.Count;
						for (int iIndex = 0; iIndex < CaptureLen; iIndex++)
						{
							parametrs[match.Groups["attribute"].Captures[iIndex].Value.ToLower()] = match.Groups["value"].Captures[iIndex].Value.Replace("\"", "");
						}
						fileName = (string)parametrs["name"];

						// 2007-11-15 Try get default extension by ContentType
						if (fileName == null)
						{
							using (System.Data.IDataReader reader = ContentType.GetContentTypeByString(entry.ContentType))
							{
								if (reader.Read())
									fileName = "Attached." + (string)reader["Extension"];
							}
						}
					}
				}

				if (fileName == null)
					fileName = "Unknown.dat";
			}
			return fileName;
		}

		public ArrayList ExtractAttachments(MimeEntryCollection MimeEntries)
		{
			ArrayList _tmpAttachs = new ArrayList();

			foreach (MimeEntry entry in MimeEntries)
			{
				if (entry.ContentDisposition == Disposition.Attachment ||
					entry.ContentDisposition == Disposition.Inline ||
					entry.ContentType == "message/rfc822" ||// OZ: Fix Attached Email Problem
					entry.ContentType == "application/ms-tnef" || // OZ: Fix Attached Wimmail.dat
					entry.ContentType.StartsWith("image/", StringComparison.InvariantCultureIgnoreCase) || // OZ: Show attached image with out ContentDisposition
					entry.ContentType.StartsWith("application/", StringComparison.InvariantCultureIgnoreCase)) // OZ: Fixed problem with attached file without Disposition.Attachment
				{
					//if (entry.ContentType.StartsWith("text/html"))
					//    continue;

					string fileName = GetFileName(entry);

					if (entry.ContentType == "application/ms-tnef")
					{
						TnefParser tnefParser = new TnefParser(entry.Body);

						if (tnefParser.Parse())
						{
							_tmpAttachs.AddRange(tnefParser.Attachments);
						}
						else
						{
							AttachmentInfo att = new AttachmentInfo(fileName, entry.ContentType, entry.Body.Length);
							att.Headers.Add(entry.Headers);
							_tmpAttachs.Add(att);
						}
						// TODO: Parse tnef entry
					}
					else
					{
						AttachmentInfo att = new AttachmentInfo(fileName, entry.ContentType, entry.Body.Length);
						att.Headers.Add(entry.Headers);
						_tmpAttachs.Add(att);
					}
				}

				if (entry.MimeEntries.Count > 0)
					_tmpAttachs.AddRange(ExtractAttachments(entry.MimeEntries));
			}

			return _tmpAttachs;
		}
	}
}
