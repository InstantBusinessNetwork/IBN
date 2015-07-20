using System;
using System.Collections;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using Mediachase.Ibn;
using Mediachase.Net.Mail;


namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for MHTHelper.
	/// </summary>
	public class MHTHelper
	{
		private Pop3Message _OwnerMessage	=	null;
		private bool		_HasHtmlMessage =	false;

		public MHTHelper(Pop3Message message)
		{
			_OwnerMessage = message;

			CheckHTMLText(this.OwnerMessage.MimeEntries);
		}

		private void CheckHTMLText(MimeEntryCollection mimeEntries)
		{
			foreach(MimeEntry entry in mimeEntries)
			{
				CheckHTMLText(entry.MimeEntries);
				if(_HasHtmlMessage)
					break;

				if(entry.ContentDisposition!=Disposition.Attachment && entry.ContentType.StartsWith("text/html"))
				{
					_HasHtmlMessage = true;
					break;
				}
			}
		}

		
		public Pop3Message	OwnerMessage
		{
			get
			{
				return _OwnerMessage;
			}
		}

		protected string ChangeCharset(string input) 
		{
			string retVal = input;

			int iStartPosition  =-1;
			while(retVal.IndexOf("charset",iStartPosition)!=-1)
			{
				int iCharSetIndex = retVal.IndexOf("charset");
				int iStartIndex = retVal.IndexOf("=",iCharSetIndex);
				int iEndIndex = retVal.IndexOf(";",iStartIndex);
				if(iEndIndex==-1)
					iEndIndex = retVal.Length;
					
				string CharSet = retVal.Substring(iStartIndex+1,iEndIndex-iStartIndex-1);
				retVal = retVal.Replace(CharSet,"\"utf-8\"");

				iStartPosition = iEndIndex;
			}
			return retVal;
		}

		protected void SaveHeaders(System.IO.StreamWriter	writer, Rfc822HeaderCollection	headers)
		{
			StringBuilder stringBuilder = new StringBuilder(30 * headers.Count);
			//			for (int i = 0; i < headers.Count; i++)
			//			{
			//				string str1 = headers.GetKey(i);
			//				string str2 = headers.Get(i);
			//
			//				if(str1=="Content-Transfer-Encoding")
			//					continue;
			//
			//				str2 = str2.Replace("text/plain","text/html");
			//
			//				if(str2.IndexOf("charset")!=-1)
			//				{
			//					int iCharSetIndex = str2.IndexOf("charset");
			//					int iStartIndex = str2.IndexOf("=",iCharSetIndex);
			//					int iEndIndex = str2.IndexOf(";",iStartIndex);
			//					if(iEndIndex==-1)
			//						iEndIndex = str2.Length;
			//					
			//					string CharSet = str2.Substring(iStartIndex+1,iEndIndex-iStartIndex-1);
			//					str2 = str2.Replace(CharSet,"\"utf-8\"");
			//				}
			//
			//				stringBuilder.Append(str1).Append(": ");
			//				stringBuilder.Append(str2).Append("\r\n");
			//			}

			if(headers["Content-Type"]==null)
			{
				stringBuilder.Append("Content-Type").Append(": ");
				stringBuilder.Append("text/html; charset=\"utf-8\"").Append("\r\n");
			}
			else
			{
				stringBuilder.Append("Content-Type").Append(": ");

				string strContentTypeValue = headers["Content-Type"];

				if(!_HasHtmlMessage)
					strContentTypeValue = strContentTypeValue.Replace("text/plain","text/html");

				///strContentTypeValue = ChangeCharset(strContentTypeValue);

				stringBuilder.Append(strContentTypeValue).Append("\r\n");
			}

			stringBuilder.Append("Content-Transfer-Encoding").Append(": ");
			stringBuilder.Append("base64").Append("\r\n");

			if(headers["Content-Location"]!=null)
			{
				stringBuilder.Append("Content-Location").Append(": ");
				stringBuilder.Append(headers["Content-Location"]).Append("\r\n");
			}

			if(headers["Content-ID"]!=null)
			{
				stringBuilder.Append("Content-ID").Append(": ");
				stringBuilder.Append(headers["Content-ID"]).Append("\r\n");
			}

			stringBuilder.Append("\r\n");

			writer.Write(stringBuilder.ToString());
		}

		protected void SaveMimeEntry(System.IO.StreamWriter	writer, MimeEntry	entry)
		{
			if(entry.ContentDisposition==Disposition.Attachment)
				return;

			// Step 2. Process Mime Entry [5/11/2004]

			string ContentType = entry.Headers["Content-Type"];
			Hashtable	ContentTypeParametrs = ParseHeaderLine(ContentType);
			string ContentTypeBoundary = (string)ContentTypeParametrs["boundary"];

			//			if(ContentType==null)
			//				ContentType = "Content-Type: text/html; charset=\"utf-8\"";

			if(entry.Body!=null &&ContentTypeBoundary==null)
			{
				writer.Write("\r\n------=_NextPart_000_0000_01C26874.EA4F5D40\r\n");
				SaveHeaders(writer,entry.Headers);

				//				if(ContentType.IndexOf("text/html")!=-1)
				//				{
				//					writer.Write(Convert.ToBase64String(Encoding.UTF8.GetBytes(entry.BodyText)));
				//				}

				if(ContentType==null)
				{
					ContentType = "Content-Type: text/html; charset=\"utf-8\"";
					writer.Write(Convert.ToBase64String(Encoding.UTF8.GetBytes(entry.BodyText)));
				}
				else
					writer.Write(Convert.ToBase64String(entry.Body));
			}

			foreach(MimeEntry	childEntry in entry.MimeEntries)
			{
				SaveMimeEntry(writer,childEntry);
			}
		}

		protected Hashtable ParseHeaderLine(string HeaderLine)
		{
			Hashtable parametrs	=	new Hashtable();

			if(HeaderLine==null)
				return parametrs;

			Regex	contentParser = new Regex("(?<type>[^;]+)(;(\\s)*(?<parameter>((?<attribute>[^=]+)=(?<value>((\"[^\"]*\")?[^;]*)))))*");

			Match	match = contentParser.Match(HeaderLine);

			string Type = match.Groups["type"].Value;

			int CaptureLen = match.Groups["parameter"].Captures.Count;
			for(int iIndex=0;iIndex<CaptureLen;iIndex++)
			{
				parametrs[match.Groups["attribute"].Captures[iIndex].Value.ToLower()] = match.Groups["value"].Captures[iIndex].Value.Replace("\"","");
			}
			return parametrs;
		}

		public void CreateMHT(System.IO.StreamWriter writer)
		{
			// Step 1. Process Main Headers [5/11/2004]
			writer.Write("From: <Saved by Mediachase Instance Business Network " + IbnConst.VersionMajorDotMinor + ">\r\n" +
				"Subject:\r\n"+
				"Date: "+ 
				(this.OwnerMessage.Headers["Date"]==null?"":this.OwnerMessage.Headers["Date"])+
				"\r\n"+
				"MIME-Version: 1.0\r\n"+
				"Content-Type: multipart/related; boundary=\"----=_NextPart_000_0000_01C26874.EA4F5D40\";type=\"text/html\"\r\n"+
				"\r\n"+
				"This is a multi-part message in MIME format.\r\n");
			
			foreach(MimeEntry	entry in this.OwnerMessage.MimeEntries)
			{
				SaveMimeEntry(writer,entry);
			}

			writer.Write("\r\n------=_NextPart_000_0000_01C26874.EA4F5D40--");
		}
	}
}
