using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Globalization;


namespace Mediachase.Net.Mail
{
	/// <summary>
	/// Summary description for MimeEntryCollection.
	/// </summary>
	public class MimeEntryCollection: CollectionBase
	{
		internal void Add(MimeEntry newItem)
		{
			this.List.Add(newItem);
		}

		internal MimeEntryCollection()
		{
		}

		public MimeEntry this[int Index]
		{
			get
			{
				return (MimeEntry)this.List[Index];
			}
		}

		internal DataParseStatus ParseMimeEntries(byte[] buffer, int size, ref int unparsed, Rfc822HeaderCollection headers)
		{
			string strFullConentType	= headers["Content-Type"];
			if(strFullConentType==null)
				strFullConentType="";

			string strContentTypeValue	= null;
			
			Hashtable	parametrs;
			MimeEntry.ParseHeader(strFullConentType, out strContentTypeValue, out parametrs);

			if(!strContentTypeValue.StartsWith("multipart/", true, CultureInfo.InvariantCulture))
			{
				// Simple Email [1/27/2004]
				MemoryStream strmEntry = new MemoryStream();
				strmEntry.Write(buffer, unparsed, size - unparsed);

				this.Add(new MimeEntry(strmEntry,headers));
			}
			else
			{ 
				// Parse Multipart Value
				string boundaryID = (string)parametrs["boundary"];

				// Step 1. Unpack multipart [2/10/2004]
				StreamLineReader	lineReader = new StreamLineReader (buffer, unparsed, size);
				byte[] lineData = lineReader.ReadLine();

				// Step 1.1 Search first entry
				while(lineData!=null)
				{
					string strLine = System.Text.Encoding.Default.GetString(lineData);
					if(strLine.StartsWith("--" + boundaryID))
					{
						lineData = lineReader.ReadLine();
						break;
					}
					lineData = lineReader.ReadLine();
				}

				// Step 1.2 Start reading entries
				MemoryStream strmEntry = new MemoryStream();

				while(lineData!=null)
				{
					string strLine = System.Text.Encoding.Default.GetString(lineData);
					if(strLine.StartsWith("--" + boundaryID) && strmEntry.Length > 0)
					{
						// Step 1.3 Add Entry
						try
						{
							MimeEntry newEntry = new MimeEntry(strmEntry);
							this.Add(newEntry);
						}
						catch (Pop3ServerIncorectEMailFormatException ex)
						{
							// Skeep Broken Entry
							if (ex.InnerException != null)
								throw;
						}

						strmEntry.SetLength(0);
					}
					else
					{
						strmEntry.Write(lineData,0,lineData.Length);
						strmEntry.Write(new byte[]{(byte)'\r',(byte)'\n'},0,2);
					}

					lineData = lineReader.ReadLine();
				}
			}


			return DataParseStatus.Done;
		}

	}
}
