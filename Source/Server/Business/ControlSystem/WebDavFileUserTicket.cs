using System;
using Mediachase.IBN.Business.EMail;
using System.IO;
using System.Data;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for FileUserTicket.
	/// </summary>
    [Obsolete]
	public class WebDavFileUserTicket
	{
		private int _userId;
		private int _fileId;
		private DateTime _created = DateTime.Now;
		private int _historyInfoId = -1;

		private WebDavFileUserTicket()
		{
		}

		private WebDavFileUserTicket(int UserId, int FileId)
		{
			_userId = UserId;
			_fileId = FileId;
		}

		private WebDavFileUserTicket(int UserId, int FileId, int HistoryInfoId)
		{
			_userId = UserId;
			_fileId = FileId;
			_historyInfoId = HistoryInfoId;
		}

		private WebDavFileUserTicket(DateTime Created, int UserId, int FileId, int HistoryInfoId)
		{
			_userId = UserId;
			_fileId = FileId;
			_created = Created;
			_historyInfoId = HistoryInfoId;
		}

		public int UserId {get { return _userId;}}
		public int FileId {get { return _fileId;}}
		public DateTime Created {get { return _created;}}
		public int HistoryInfoId {get { return _historyInfoId;}}

		public static string GetDownloadPath(int FileId, string FileName, int HistoryInfoId)
		{
			return GetDownloadPath(Security.CurrentUser.UserID, FileId, FileName, HistoryInfoId);
		}

		public static string GetDownloadPath(int FileId, string FileName)
		{
			return GetDownloadPath(Security.CurrentUser.UserID, FileId, FileName, -1);
		}

		public static string GetDownloadPath(int UserId, int FileId, string FileName)
		{
			return GetDownloadPath(UserId, FileId, FileName, -1);
		}

        public static bool IsWebDAVSupportedExtension(string extension)
        {
            using (IDataReader reader = ContentType.GetContentTypeByExtension(extension))
            {
                if (reader.Read())
                {
                    if (reader["AllowWebDav"] != DBNull.Value && (bool)reader["AllowWebDav"])
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private static string GetExtensionWithoutDot(string path)
        {
            if (Path.GetExtension(path) != String.Empty)
            {
                return Path.GetExtension(path).Substring(1);
            }
            else return String.Empty;
        }

		public static string GetDownloadPath(int UserId, int FileId, string FileName, int HistoryInfoId)
		{
			bool? bWebDavTurnOn =  PortalConfig.UseWebDav;

			// ASP.NET returns 400 Bad Request IF url includes: * & % ..
            // OZ [2007-09-27] Solve % char problem
            FileName = FileName.Replace("%", string.Empty);
			// OZ [2007-11-01] Solve & char problem
			FileName = FileName.Replace("&", string.Empty);

			//FileName = System.Web.HttpUtility.UrlPathEncode(FileName);


            if (bWebDavTurnOn.HasValue && bWebDavTurnOn.Value && IsWebDAVSupportedExtension(GetExtensionWithoutDot(FileName)))
			{
				WebDavFileUserTicket ticket = new WebDavFileUserTicket(UserId, FileId,HistoryInfoId);
				return string.Format("/webdav/{0}/{1}", ticket.ToString(), FileName);
			}
			else
			{
				WebDavFileUserTicket ticket = new WebDavFileUserTicket(-1, FileId,HistoryInfoId);
				return string.Format("/files/{0}/{1}", ticket.ToString(), FileName);
			}
		}

		public static WebDavFileUserTicket Parse(string strTicket)
		{
            bool? bWebDavTurnOn = PortalConfig.UseWebDav;

			string strRealData =  System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(strTicket));

			string[] strSpitList = strRealData.Split('/');

			if(strSpitList.Length<4)
				throw new ArgumentOutOfRangeException("strTicket");


			WebDavFileUserTicket retValue = new WebDavFileUserTicket(
				new DateTime(long.Parse(strSpitList[0])*0xc92a69c000),
                bWebDavTurnOn.HasValue && bWebDavTurnOn.Value ? int.Parse(strSpitList[1]) : -1,
				int.Parse(strSpitList[2]), 
				strSpitList.Length>4?int.Parse(strSpitList[3]):-1);

			if(retValue.ToString()!=strTicket)
				throw new ArgumentOutOfRangeException("strTicket");

            if (bWebDavTurnOn.HasValue && bWebDavTurnOn.Value && Security.CurrentUser == null)
			{
				// TODO: Check Ticket Expiration
			}

			return retValue;
		}

		public override string ToString()
		{
			string strFormat = string.Format("{0}/{1}/{2}", this.Created.Ticks/0xc92a69c000, this.UserId, this.FileId);

			if(this.HistoryInfoId>0)
				strFormat += string.Format("/{0}",this.HistoryInfoId.ToString());


			byte[] buffer = System.Text.Encoding.ASCII.GetBytes(strFormat);
			int crc16 = Crc16.Calculate(buffer,0, buffer.Length);
			
			strFormat += "/" + crc16.ToString("D5");

			return Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(strFormat));
		}
	}
}
