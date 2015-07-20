using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;


namespace Mediachase.Net.Mail
{
	public enum Disposition
	{
		/// <summary>
		/// Content is attachment.
		/// </summary>
		Attachment = 0,

		/// <summary>
		/// Content is embbed resource.
		/// </summary>
		Inline = 1,

		/// <summary>
		/// Content is unknown.
		/// </summary>
		Unknown = 40
	}

	/// <summary>
	/// Summary description for MimeEntry.
	/// </summary>
	public class MimeEntry
	{
		private MemoryStream			_BinaryData			=	null;
		private int						_BodyOffset			=	0;

		private byte[]					_body				=	null;
		private Rfc822HeaderCollection	_headers			=	new Rfc822HeaderCollection();
		private MimeEntryCollection		_mimeEntries		=	new MimeEntryCollection();		

		private Disposition				_disposition		=	Disposition.Unknown;
		private string					_fileName			=	null;
		private string					_contentType		=	null;
		private string					_charSet			=	null;

		internal MimeEntry(MemoryStream	MultipartBinaryData)
		{
			_BinaryData = MultipartBinaryData;

			ParseHeaderAndBody();

			_BinaryData = null;
		}

		internal MimeEntry(MemoryStream	BinaryData, Rfc822HeaderCollection	headers)
		{
			_BinaryData = BinaryData;
			_headers = headers;

			ParseBody();

			_BinaryData = null;
		}

		public MimeEntryCollection MimeEntries
		{
			get
			{
				return _mimeEntries;
			}
		}

		public Rfc822HeaderCollection Headers
		{
			get
			{
				return _headers;
			}
		}

		public byte[] Body
		{
			get
			{
				return _body;
			}
		}

		public string BodyText
		{
			get
			{
				if(this.Body!=null)
				{
					try
					{
						return Encoding.GetEncoding(this.CharSet).GetString(this.Body);
					}
					catch
					{
						return Encoding.Default.GetString(this.Body);
					}
				}
				return null;
			}
		}



		public static string QDecode(System.Text.Encoding encoding,string data)
		{
			MemoryStream strm = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(data));
			int b = strm.ReadByte();

			MemoryStream dStrm = new MemoryStream();

			byte[] tmpBuffer = new byte[4];
			int tmpBufferLen = 0;
			int tmpBufferIndex = 0;

			while(b > -1)
			{
				// Hex eg. =E4
				if(b == '=')
				{
					byte[] buf = new byte[2];
					strm.Read(buf,0,2);

					// <CRLF> followed by =, it's splitted line
					if(!(buf[0] == '\r' && buf[1] == '\n'))
					{
						try
						{
							int val = int.Parse(System.Text.Encoding.ASCII.GetString(buf),System.Globalization.NumberStyles.HexNumber);

							if(tmpBufferIndex==0)
							{
								if(val<127 || encoding!=System.Text.Encoding.UTF8)
								{
									// character is a byte
									tmpBufferLen = 1;
								}
								else if (val >= 192 && val <= 223)
								{
									// character is two bytes
									tmpBufferLen = 2;
								}
								else if (val >= 224 && val <= 239)
								{
									// character is three bytes
									tmpBufferLen = 3;
								}
								else if (val >= 240 && val <= 247)
								{
									// character is four bytes
									tmpBufferLen = 4;
								}
								else
								{
									tmpBufferLen = 4;
								}

								tmpBuffer[tmpBufferIndex++] = (byte)val;
							}
							else
							{
								tmpBuffer[tmpBufferIndex++] = (byte)(val);
							}

							if(tmpBufferIndex==tmpBufferLen)
							{
								tmpBufferIndex = 0;
								tmpBufferLen = 0;

								string encodedChar = encoding.GetString(tmpBuffer);

								byte[] d = System.Text.Encoding.Unicode.GetBytes(encodedChar.ToCharArray(0,1));
								dStrm.Write(d,0,d.Length);
							}
						}
						catch
						{ 
							// If wrong hex value, just skip this chars
						}
					}
				}
				else
				{
					string encodedChar = encoding.GetString(new byte[]{(byte)b});
					byte[] d = System.Text.Encoding.Unicode.GetBytes(encodedChar);
					dStrm.Write(d,0,d.Length);
				}

				b = strm.ReadByte();
			}

			return System.Text.Encoding.Unicode.GetString(dStrm.ToArray());
		}

		public static void ParseHeader(string Header, out string Type, out Hashtable parametrs)
		{
			parametrs = new Hashtable();

			Match match = Regex.Match(Header, "(?<type>[^;]+)(;(\\s)*(?<parameter>((?<attribute>[^=]+)=(?<value>((\"[^\"]*\")?[^;]*)))))*");

			Type = match.Groups["type"].Value;

			int CaptureLen = match.Groups["parameter"].Captures.Count;

			List<string> rfc2231Attributes = new List<string>();

			for(int iIndex=0;iIndex<CaptureLen;iIndex++)
			{
				string strAttrName = match.Groups["attribute"].Captures[iIndex].Value.Trim().ToLower();
				string strAttrValue = match.Groups["value"].Captures[iIndex].Value.Trim().Replace("\"", "");

				// SUPPORTS RFC-2231
				int asteriskCharacter = strAttrName.IndexOf('*');
				if (asteriskCharacter!=-1)
				{
					strAttrName = strAttrName.Substring(0, asteriskCharacter);

					rfc2231Attributes.Add(strAttrName);

					if (parametrs.ContainsKey(strAttrName))
					{
						parametrs[strAttrName] = ((string)parametrs[strAttrName]) + strAttrValue;
					}
					else
					{
						parametrs[strAttrName] = strAttrValue;
					}
				}
				else
				{
					parametrs[strAttrName] = strAttrValue;
				}
			}

			// 2008-03-18: RFC-2231 Added Parameter Value Character Set and Language Information 
			foreach (string rfc2231AttrName in rfc2231Attributes)
			{
				string rfc2231AttrValue = (string)parametrs[rfc2231AttrName];

				int charsetDel = rfc2231AttrValue.IndexOf('\'');
				int lanDel = rfc2231AttrValue.IndexOf('\'', charsetDel + 1);

				if (charsetDel != -1 && lanDel != -1)
				{
					try
					{
						string charset = rfc2231AttrValue.Substring(0, charsetDel);
						string lan = rfc2231AttrValue.Substring(charsetDel + 1, lanDel - charsetDel - 1);

						Encoding rfc2231Encoding = Encoding.GetEncoding(charset);

						string strDecodedString = rfc2231AttrValue.Substring(lanDel + 1);

						parametrs[rfc2231AttrName] = System.Web.HttpUtility.UrlDecode(strDecodedString, rfc2231Encoding);
					}
					catch (Exception ex)
					{
						System.Diagnostics.Trace.WriteLine(ex);
					}
				}
			}
			// End Fix
		}

		protected virtual void ParseBody()
		{
			string strFullConentType	= _headers["Content-Type"];
			if(strFullConentType==null)
				strFullConentType="";

			//string strContentTypeValue	= null;
			
			Hashtable	parametrs;
			MimeEntry.ParseHeader(strFullConentType, out _contentType, out parametrs);

			// Step 2. Parse Messagy Body [1/23/2004]
			if(!_contentType.StartsWith("multipart/"))
			{
				_charSet         = (string)parametrs["charset"] ;

				if(_charSet==null)
					_charSet = Encoding.Default.HeaderName;

				string ContentEncoding = _headers["Content-Transfer-Encoding"];

				if(ContentEncoding==null)
					ContentEncoding = "8bit";

				string strDisposition     = _headers["Content-Disposition"];

				if(strDisposition!=null)
				{
					Hashtable	DispositionParameters;
					string		DispositionType;
					MimeEntry.ParseHeader(strDisposition, out DispositionType, out DispositionParameters);

					DispositionType = DispositionType.ToLower();

					if(DispositionType=="attachment")
						this._disposition = Disposition.Attachment;
					else if(DispositionType=="inline")
						this._disposition = Disposition.Inline;

					_fileName = (string)DispositionParameters["filename"];
					if(_fileName!=null)
						_fileName = Rfc822HeaderCollection.DeocodeHeaderValue(_fileName);
				}

				//string BodyString = Encoding.Default.GetString(this._BinaryData.GetBuffer(),this._BodyOffset,(int)(this._BinaryData.Length  - this._BodyOffset));
				Encoding encoding = null;
				try
				{
					encoding = Encoding.GetEncoding(CharSet);
				}
				catch
				{
					encoding = Encoding.Default;
				}


				string BodyString = encoding.GetString(this._BinaryData.GetBuffer(),this._BodyOffset,(int)(this._BinaryData.Length  - this._BodyOffset));

				//string BodyString = Encoding.ASCII.GetString(this._BinaryData.GetBuffer(),this._BodyOffset,(int)(this._BinaryData.Length  - this._BodyOffset));
				//string BodyString2 = Encoding.UTF8.GetString(this._BinaryData.GetBuffer(),this._BodyOffset,(int)(this._BinaryData.Length  - this._BodyOffset));

				switch(ContentEncoding.ToLower())
				{
					case "quoted-printable":
						_body = encoding.GetBytes(MimeEntry.QDecode(encoding,BodyString));
						break;
					case "7bit":
						//_body = Encoding.ASCII.GetBytes(BodyString);
						_body = encoding.GetBytes(BodyString);
						break;
					default:
					case "8bit":
						_body = encoding.GetBytes(BodyString);
						break;
					case "base64":
						BodyString = BodyString.Trim();

						if(BodyString.Length>0 )
						{
							int base64FixCount = 0;

							// Fix If Base 64 is broken
							while (true)
							{
								try
								{
									_body = Convert.FromBase64String(BodyString);
									break;
								}
								catch (System.FormatException)
								{
									// Remove not supported chars
									if (base64FixCount == 0)
										BodyString = Regex.Replace(BodyString, "[^a-zA-Z0-9+/=]+", string.Empty);
									else if (base64FixCount == 1)
										BodyString += "=";
									else
										BodyString = BodyString.Substring(0, BodyString.Length - 1);

									if (BodyString.Length == 0 || base64FixCount == 25) // Max 25 Attempts to fix chars
									{
										_body = new byte[] { };
										break;
									}

									base64FixCount++;
								} 
							}
						}
						else
							_body = new byte[]{};
						break;
					case "binary":
						_body = encoding.GetBytes(BodyString);
						break;
					//default:
					//    throw new Pop3ServerIncorectEMailFormatException("Not supported content-encoding " + ContentEncoding + " !");
				}
			}
			else
			{
				DataParseStatus parseStatus = _mimeEntries.ParseMimeEntries(_BinaryData.GetBuffer(),(int)_BinaryData.Length,ref _BodyOffset,this.Headers);
			}
		}

		protected virtual void ParseHeaderAndBody()
		{
			DataParseStatus parseStatus = DataParseStatus.Invalid;

			try
			{
				// Step 1. Parse header [1/23/2004]
				parseStatus = _headers.ParseHeaders(_BinaryData.GetBuffer(), (int)_BinaryData.Length, ref _BodyOffset);
				if (parseStatus == DataParseStatus.Done)
				{
					ParseBody();
				}
			}
			catch (Exception ex)
			{
				throw new Pop3ServerIncorectEMailFormatException("Internal Parser Error: "+ ex.Message, ex);
			}

			if(parseStatus!=DataParseStatus.Done)
				throw new Pop3ServerIncorectEMailFormatException();
		}

		public Disposition ContentDisposition
		{
			get
			{
				return 	_disposition;		
			}
		}

		public string ContentType
		{
			get
			{
				return _contentType;
			}
		}

		public string CharSet
		{
			get
			{
				return _charSet;
			}
		}

		public string FileName
		{
			get
			{
				return _fileName;
			}
		}

	}
}
