using System;
using System.IO;
using System.Collections;
using System.Globalization;

namespace Mediachase.Net.Mail
{
	/// <summary>
	/// Summary description for Pop3Message.
	/// </summary>
	public class Pop3Message
	{
		private MemoryStream			_EMailBinaryData	=	null;

		private Rfc822HeaderCollection	_headers			= new Rfc822HeaderCollection();
		private MimeEntryCollection		_mimeEntries		= new MimeEntryCollection();		

		public Pop3Message(MemoryStream	EMailBinaryData)
		{
			_EMailBinaryData = EMailBinaryData;
			Parse();
		}

		public Stream InputStream
		{
			get
			{
				return (Stream)_EMailBinaryData;
			}
		}

		protected virtual void Parse()
		{
			int				unparsed = 0;
			DataParseStatus parseStatus = DataParseStatus.Invalid;

			try
			{
				// Step 1. Parse header [1/23/2004]
				parseStatus = _headers.ParseHeaders(_EMailBinaryData.GetBuffer(),(int)_EMailBinaryData.Length,ref unparsed);
				if(parseStatus==DataParseStatus.Done)
				{
					// Step 2. Parse Messagy Body [1/23/2004]
					parseStatus = _mimeEntries.ParseMimeEntries(_EMailBinaryData.GetBuffer(),(int)_EMailBinaryData.Length,ref unparsed,this.Headers);
				}
			}
			catch(Exception ex)
			{
				throw new Pop3ServerIncorectEMailFormatException("Internal Parser Error",ex);
			}

			if(parseStatus!=DataParseStatus.Done)
				throw new Pop3ServerIncorectEMailFormatException();
		}


		public MimeEntryCollection MimeEntries
		{
			get
			{
				return _mimeEntries;
			}
		}

		private static void ExtractAttachment(MimeEntryCollection Attachments, MimeEntryCollection MimeEntries)
		{
			foreach(MimeEntry entry in MimeEntries)
			{
				if(entry.ContentDisposition==Disposition.Attachment)
				{
					Attachments.Add(entry);
				}

				if(entry.MimeEntries.Count>0)
					ExtractAttachment(Attachments,entry.MimeEntries);
			}
		}

		public MimeEntryCollection Attachments
		{
			get
			{
				MimeEntryCollection	_Attachments	=	new MimeEntryCollection();

				ExtractAttachment(_Attachments, this.MimeEntries);

				return _Attachments;
			}
		}


		public Rfc822HeaderCollection Headers
		{
			get
			{
				return _headers;
			}
		}

		public MailAddress From
		{
			get
			{
				string[] strHeaderValue = this.Headers.GetValues("From");
				if(strHeaderValue!=null&&strHeaderValue.Length>0)
					return MailAddress.Parse(strHeaderValue[0]);
				return null;
			}
		}

		public MailAddressCollection FromCollection
		{
			get
			{
				MailAddressCollection retVal = new MailAddressCollection();

				string[] strHeaderValue = this.Headers.GetValues("From");

				if(strHeaderValue!=null)
				{
					foreach(string EmailString in strHeaderValue)
					{
						retVal.Add(MailAddress.Parse(EmailString));
					}
				}

				return retVal;
			}
		}

		public MailAddress Sender
		{
			get
			{
				string strHeaderValue = this.Headers["Sender"];
				if(strHeaderValue!=null)
					return MailAddress.Parse(strHeaderValue);
				return null;
			}
		}

		public string To
		{
			get
			{
				string strHeaderValue = this.Headers["To"];
				return strHeaderValue;
//				if(strHeaderValue!=null)
//					return EMailBox.Parse(strHeaderValue);
//				return null;
			}
		}

		public MailAddress ResentFrom
		{
			get
			{
				string[] strHeaderValue = this.Headers.GetValues("Resent-From");
				if(strHeaderValue!=null&&strHeaderValue.Length>0)
					return MailAddress.Parse(strHeaderValue[0]);
				return null;
			}
		}

		public MailAddressCollection ResentFromCollection
		{
			get
			{
				MailAddressCollection retVal = new MailAddressCollection();

				string[] strHeaderValue = this.Headers.GetValues("Resent-From");

				if(strHeaderValue!=null)
				{
					foreach(string EmailString in strHeaderValue)
					{
						retVal.Add(MailAddress.Parse(EmailString));
					}
				}

				return retVal;
			}
		}

		public MailAddress ResentSender
		{
			get
			{
				string strHeaderValue = this.Headers["Resent-Sender"];
				if(strHeaderValue!=null)
					return MailAddress.Parse(strHeaderValue);
				return null;
			}
		}

		public string Subject
		{
			get
			{
				return this.Headers["Subject"];
			}
		}

		private static string ExtractText(string ContentType, MimeEntryCollection mimeEntries)
		{
			string retVal = null;

			foreach(MimeEntry entry in mimeEntries)
			{
				retVal = ExtractText(ContentType, entry.MimeEntries);
				if(retVal!=null)
					break;

				if(entry.ContentType.StartsWith(ContentType, true, CultureInfo.InvariantCulture))
				{
					retVal = entry.BodyText;
					break;
				}
			}

			return retVal;
		}

		public string BodyText
		{
			get
			{
				string retVal = Pop3Message.ExtractText("text/plain", this.MimeEntries);

				if (retVal == null && this.MimeEntries.Count == 1 && this.MimeEntries[0].ContentDisposition != Disposition.Attachment)
					return this.MimeEntries[0].BodyText;

//				if(retVal==null)
//					retVal = Pop3Message.ExtractText("text/");
				return retVal;
			}
		}

		public string BodyHtml
		{
			get
			{
				string retVal = Pop3Message.ExtractText("text/html", this.MimeEntries);
//				if(retVal==null)
//					retVal = Pop3Message.ExtractText("text/");
				return retVal;
			}
		}

		public DateTime Date
		{
			get
			{
				string strDateHeader = this.Headers["Date"];
				if(strDateHeader==null || strDateHeader.Trim().Length==0)
					return DateTime.MinValue;
				else
				{
					System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(strDateHeader, @"(?<datetime>((?<day>Mon|Tue|Wed|Thu|Fri|Sat|Sun), )?(?<date>\d{1,2} (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) \d{2,4}) (?<time>\d{2}:\d{2}(:\d{2})?) (?<zone>UT|GMT|EST|EDT|CST|CDT|MST|MDT|PST|PDT|\w{1}|(\+|\-)\d{4}))");

					String strDay = match.Groups["day"].Value;
					String strDate = match.Groups["date"].Value;
					String strTime = match.Groups["time"].Value;
					String strZone = match.Groups["zone"].Value;

					switch(strZone) 
					{
						case "UT":
							strZone = "-0000";
							break;
						case "GMT":
							strZone = "-0000";
							break;
						case "EST":
							strZone = "-0500";
							break;
						case "EDT":
							strZone = "-0400";
							break;
						case "CST":
							strZone = "-0600";
							break;
						case "CDT":
							strZone = "-0500";
							break;
						case "MST":
							strZone = "-0700";
							break;
						case "MDT":
							strZone = "-0600";
							break;
						case "PST":
							strZone = "-0800";
							break;
						case "PDT":
							strZone = "-0700";
							break;
						case "Z":
							strZone = "-0000";
							break;
						case "A":
							strZone = "-0100";
							break;
						case "B":
							strZone = "-0200";
							break;
						case "C":
							strZone = "-0300";
							break;
						case "D":
							strZone = "-0400";
							break;
						case "E":
							strZone = "-0500";
							break;
						case "F":
							strZone = "-0600";
							break;
						case "G":
							strZone = "-0700";
							break;
						case "H":
							strZone = "-0800";
							break;
						case "I":
							strZone = "-0900";
							break;
						case "K":
							strZone = "-1000";
							break;
						case "L":
							strZone = "-1100";
							break;
						case "M":
							strZone = "-1200";
							break;
						case "N":
							strZone = "+0100";
							break;
						case "O":
							strZone = "+0200";
							break;
						case "P":
							strZone = "+0300";
							break;
						case "Q":
							strZone = "+0400";
							break;
						case "R":
							strZone = "+0500";
							break;
						case "S":
							strZone = "+0600";
							break;
						case "T":
							strZone = "+0700";
							break;
						case "U":
							strZone = "+0800";
							break;
						case "V":
							strZone = "+0900";
							break;
						case "W":
							strZone = "+1000";
							break;
						case "X":
							strZone = "+1100";
							break;
						case "Y":
							strZone = "+1200";
							break;
					}

					string strFullDate = string.Format("{0}, {1} {2} {3}",strDay, strDate, strTime, strZone);

					string[] formats = new string[]{
													   "r",
													   "ddd, d MMM yyyy HH':'mm':'ss zzz",
													   "ddd, dd MMM yyyy HH':'mm':'ss zzz",
													   "dd'-'MMM'-'yyyy HH':'mm':'ss zzz",
													   "d'-'MMM'-'yyyy HH':'mm':'ss zzz"
												   };

					return DateTime.ParseExact(strFullDate,formats,System.Globalization.DateTimeFormatInfo.InvariantInfo,System.Globalization.DateTimeStyles.None); 
				}
			}
		}

		public string MessageID
		{
			get
			{
				return this.Headers["Message-ID"];
			}
		}

		public string Importance
		{
			get
			{
				return this.Headers["Importance"];
			}
		}


	}
}
