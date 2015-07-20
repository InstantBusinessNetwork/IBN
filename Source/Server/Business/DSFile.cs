using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Text;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// 
	/// </summary>
	public class DSFile
	{
		private const string sFileID = "fileid="; // for building/parsing file URL.
		private static Hashtable m_Files = new Hashtable();

		public static int GetDefaultContentType()
		{
			return GetContentTypeByExtension("");
		}

		public static int GetContentTypeByFileName(string fileName)
		{
			string ext = "";
			int dotIndex = fileName.LastIndexOf('.');
			if(dotIndex != -1)
				ext = fileName.Substring(dotIndex+1);
			return GetContentTypeByExtension(ext);
		}

		public static int GetContentTypeIconFileId(int ContentTypeId)
		{
			return DBContentType.GetIconFileId(ContentTypeId);
		}


		public static int GetContentTypeByExtension(string extension)
		{
			if(null == extension)
				throw new ArgumentNullException("extension");

			if (extension.StartsWith("."))
				extension = extension.Substring(1);

			int id = -1;

			using(IDataReader reader = DBContentType.GetContentTypeByExtension(extension))
			{
				if(reader != null)
				{
					if(reader.Read())
						id = (int)reader["ContentTypeId"];
				}
			}

			if(id == -1)
			{
				if(extension.Length > 0)
					id = GetContentTypeByExtension("");
				else
					throw new NoDefaultContentTypeException();
			}

			return id;
		}

		public static int Create(string fileName, Stream data)
		{
			int id;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				//Add file to DB
				id = DBFile.Create(fileName, GetContentTypeByFileName(fileName));

				//Update file data
				DBFile.ReadFromStream(id, data);

				tran.Commit();
			}

			return id;
		}

		public static void Delete(string fileURL)
		{
			Delete(fileURL, true);
		}

		public static void Delete(string fileURL, bool CheckForDelete)
		{
			int fileID = GetFileIDFromURL(fileURL);
			DBFile.Delete(fileID);
		}

		public static void Delete(int fileID)
		{
			DBFile.Delete(fileID);
		}

		public static void GetInfo(int fileID, out string fileName, out string contentType, out int size)
		{
			using(IDataReader reader = DBFile.GetInfo(fileID))
			{
				if(null == reader || !reader.Read())
					throw new InvalidFileException();
			
				fileID = (int)reader["FileId"];
				fileName = reader["FileName"].ToString();
				contentType = reader["ContentType"].ToString();
				size = (int)reader["Size"];
			}
		}

		public static void WriteToStream(int fileID, Stream stream)
		{
			if(null == stream)
				throw new ArgumentNullException("Stream cannot be null.");

			DBFile.WriteToStream(fileID, stream);
		}

		public static void ReadFromStream(int fileID, Stream stream)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBFile.ReadFromStream(fileID, stream);

				tran.Commit();
			}
		}

		public static void DownloadFile(string url, Stream stream)
		{
			WriteToStream(GetFileIDFromURL(url), stream);
		}

		public static void DownloadFile(int fileId, HttpResponse response)
		{
			DownloadFile(fileId, response, true);
		}

		public static void DownloadFile(string url, HttpResponse response)
		{
			DownloadFile(url, response, true);
		}

		public static void DownloadFile(string url, HttpResponse response, bool AddContentDispositionHeader)
		{
			DownloadFile(GetFileIDFromURL(url), response, AddContentDispositionHeader);
		}

		public static void DownloadFile(int fileId, HttpResponse response, bool AddContentDispositionHeader)
		{
			string fileName;
			string contentType;
			int size;

			GetInfo(fileId, out fileName, out contentType, out size);

			response.ContentType = contentType;

			string susag = HttpContext.Current.Request.UserAgent.ToString();
			if(susag.IndexOf("MSIE")>0)
			{
				string sextension = "";
				string _fileName = fileName;
				int lastDot = fileName.LastIndexOf(".");
				if(lastDot >= 0)
				{
					sextension = fileName.Substring(lastDot);
					_fileName = fileName.Substring(0, lastDot);
				}
				bool MoreThan127 = false;
				for(int i=0; i<_fileName.Length; i++)
				{
					char ch = _fileName[i];
					int j = (int)ch;
					if(j>127)
					{
						MoreThan127 = true;
						break;
					}
				}
				if(MoreThan127 && _fileName.Length>25)
					_fileName = _fileName.Substring(0,25);
				fileName = HttpContext.Current.Server.UrlPathEncode(_fileName+sextension);
			}

			if (AddContentDispositionHeader)
				response.AddHeader("content-disposition", String.Format("attachment; filename=\"{0}\"", fileName));

			const int iBufferSize = 65536; // 64 KB
			byte[] outbyte = new byte[iBufferSize]; 

			using(IDataReader reader = DBFile.GetBinaryData(fileId))
			{
				if(reader != null)
				{
					if(reader.Read())
					{
						long read = 0, startIndex = 0;
						do
						{
							if(response.IsClientConnected)
							{
								read = reader.GetBytes(0, startIndex, outbyte, 0, iBufferSize);
								response.OutputStream.Write(outbyte, 0, System.Convert.ToInt32(read));
								response.Flush();
								startIndex += read;
							}
							else
								read = 0;
						} while(read == iBufferSize);
					}
				}
			}
		}

		#region CreatePath
		static void CreatePath(string path)
		{
			string p = path, s = "";
			int i;
			while(true)
			{
				i = p.IndexOf('\\');
				if(i < 0)
					break;
				s += p.Substring(0, i+1);
				System.IO.Directory.CreateDirectory(s);
				p = p.Substring(i+1);
			}
		}
		#endregion

		public static string UploadFile(string fileName, Stream data)
		{ 
			data.Seek(0, SeekOrigin.Begin);
			int dsfID = Create(fileName, data);
			return String.Format("http://localhost?{0}{1}", sFileID, dsfID);
		}

		#region GetFileIDFromURL
		public static int GetFileIDFromURL(string url)
		{
			Uri uri = new Uri(url.ToLower());
			string strQuery = uri.Query;
			int FileID = 0;

			int idIndex = strQuery.IndexOf(sFileID);
			if(idIndex != -1)
			{
				int start = idIndex + sFileID.Length;
				int ampIndex = strQuery.IndexOf("&", idIndex);
				try
				{
					if(ampIndex >= 0)
						FileID = Int32.Parse(strQuery.Substring(start, ampIndex - start));
					else
						FileID = Int32.Parse(strQuery.Substring(start));
				}
				catch(Exception)
				{
				}
			}
			return FileID;
		}
		#endregion

		private static string Base64Encode(string inStr)
		{
			string EncodedWordHeader = "=?utf-8?B?";
			EncodedWordHeader  +=  Convert.ToBase64String(Encoding.UTF8.GetBytes(inStr));
			EncodedWordHeader  +=  "?=";
			return EncodedWordHeader;
		}
	}
}
