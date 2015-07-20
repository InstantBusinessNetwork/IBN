using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Resources;
using Mediachase.Net.Mail;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business.Pop3
{
	/// <summary>
	/// Summary description for Pop3FileExtendedMessageHandler.
	/// </summary>
	public class FileExtendedPop3MessageHandler : BasePop3MessageHandler
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.IBN.Business.Resources.Pop3MessageHadlers",typeof(FileExtendedPop3MessageHandler).Assembly);

		protected class FileSettings
		{
			public int		FolderId = 0;
			public string	FolderPattern = String.Empty;
			public bool		OnlyExternalSenders = false;
			public bool		AutoKillForUnknown = false;
			public bool		SaveMessageBodyAsEml = false;
			public bool		SaveMessageBodyAsMht = true;
			public bool		SaveMessageBodyAsMsg = false;
			public string   ContainerKey = String.Empty;

			public FileSettings()
			{
			}
		}
		// Alex Rakov
		//protected FileStorage	storage = null;

		public FileExtendedPop3MessageHandler()
			: base("FileExtended.Pop3MessageHandler", "Extended Pop3 Message Handler")
		{
			this.InnerDescription = LocRM.GetString("FileExtendedPop3MessageHandler_Description");

			// Alex Rakov
			//BaseIbnContainer baseContainer = BaseIbnContainer.Create("FileLibrary", "Workspace");
			//storage = (FileStorage)baseContainer.LoadControl("FileStorage");
		}

		private bool str2bool(string value)
		{
			return (value == "1");
		}

		protected virtual FileSettings GetSettings(Pop3Box box)
		{
			FileSettings settings = new FileSettings();

			if (box.Parameters.Contains("ContainerKey")) {
				settings.ContainerKey = box.Parameters["ContainerKey"];
			}
			if (box.Parameters.Contains("FolderId"))
			{
				settings.FolderId = int.Parse(box.Parameters["FolderId"]);
			}
			if (box.Parameters.Contains("FolderPattern"))
			{
				settings.FolderPattern = box.Parameters["FolderPattern"].Trim();
			}
			if (box.Parameters.Contains("OnlyExternalSenders"))
			{
				settings.OnlyExternalSenders = str2bool(box.Parameters["OnlyExternalSenders"]);
			}
			if (box.Parameters.Contains("AutoKillForUnknown"))
			{
				settings.AutoKillForUnknown = str2bool(box.Parameters["AutoKillForUnknown"]);
			}
			if (box.Parameters.Contains("SaveMessageBodyAsEml"))
			{
				settings.SaveMessageBodyAsEml = str2bool(box.Parameters["SaveMessageBodyAsEml"]);
			}
			if (box.Parameters.Contains("SaveMessageBodyAsMht"))
			{
				settings.SaveMessageBodyAsMht = str2bool(box.Parameters["SaveMessageBodyAsMht"]);
			}
			if (box.Parameters.Contains("SaveMessageBodyAsMsg"))
			{
				settings.SaveMessageBodyAsMsg = str2bool(box.Parameters["SaveMessageBodyAsMsg"]);
			}
			return settings;
		}

		protected string ParseFolderPattern(string FolderPattern, Pop3Message message, int userId)
		{
			Regex contentParser = new Regex(@"(?<AT>\x5B(?<ATName>(\w|\.|_)+)(\x28(?<ATParam>[^\x29]*)\x29)?\x5D)");
			MatchCollection matchList = contentParser.Matches(FolderPattern);
			foreach(Match matchItem in matchList)
			{
				string strAT = matchItem.Groups["AT"].Value;
				string strATName = matchItem.Groups["ATName"].Value;
				string strATParam = matchItem.Groups["ATParam"].Value;
				string str = String.Empty;

				MailAddress from = message.Sender;
				if(from == null)
					from = message.From;

				switch(strATName)
				{
					case "UserName":
						if (userId != -1) 
						{
							str = User.GetUserName(userId);
						}
						else str = message.Sender.Address;
						break;
					case "From":
						str = from.Address;
						break;
					case "Subject":
						str = message.Subject;
						break;
					case "Date.Now":
						str = DateTime.Now.ToString(strATParam);
						break;
					case "Date.UtcNow":
						str = DateTime.UtcNow.ToString(strATParam);
						break;
					case "Date":
						str = message.Date.ToString(strATParam);
						break;
					case "Importance":
						str = message.Importance;
						break;
					case "MessageID":
						str = message.MessageID;
						break;
					case "Sender":
						str = message.Sender==null?"SenderIsNull":message.Sender.Address;
						break;
					case "To":
						str = message.To;
						break;
					case "Header":
						str = message.Headers[strATParam];
						break;
					default:
						str = strAT;
						break;
				}
				FolderPattern = FolderPattern.Replace(strAT, str);
			}
			return FolderPattern.Trim(); 
		}

		public static string PreviewFolderPattern(string FolderPattern, int userId)
		{
			Regex contentParser = new Regex(@"(?<AT>\x5B(?<ATName>(\w|\.|_)+)(\x28(?<ATParam>[^\x29]*)\x29)?\x5D)");
			MatchCollection matchList = contentParser.Matches(FolderPattern);
			foreach(Match matchItem in matchList)
			{
				string strAT = matchItem.Groups["AT"].Value;
				string strATName = matchItem.Groups["ATName"].Value;
				string strATParam = matchItem.Groups["ATParam"].Value;
				string str = String.Empty;

				switch(strATName)
				{
					case "UserName":
						if (userId != -1) 
						{
							str = User.GetUserName(userId);
						}
						else str = "e-mail";
						break;
					case "From":
						str = "From e-mail";
						break;
					case "Subject":
						str = "Subject";
						break;
					case "Date.Now":
						str = DateTime.Now.ToString(strATParam);
						break;
					case "Date.UtcNow":
						str = DateTime.UtcNow.ToString(strATParam);
						break;
					case "Date":
						str = DateTime.Now.ToString(strATParam);
						break;
					case "Importance":
						str = "Email importance";
						break;
					case "MessageID":
						str = "Message ID";
						break;
					case "Sender":
						str = "Email Sender";
						break;
					case "To":
						str = "Email adressant";
						break;
					case "Header":
						str = strATParam + " header";
						break;
					default:
						str = strAT;
						break;
				}
				FolderPattern = FolderPattern.Replace(strAT, str);
			}
			return FolderPattern.Trim(); 
		}

		protected virtual int GetUserIdByEmail(string from, bool onlyExternalSenders)
		{
			int userId = DBUser.GetUserByEmail(from, onlyExternalSenders);

			// Fix: If we've found inactive user. [8/18/2004]
			using(IDataReader reader = DBUser.GetUserInfo(userId))
			{
				if(reader.Read())
				{
					if(!(bool)reader["IsActive"])
						userId = -1;
				}
				else
					userId = -1;
			}
			// End fix [8/18/2004]

			return userId;
		}

		public static string ValidatePatternPath(string Path)
		{
			while (Path.IndexOf("\\\\") != - 1)
			{
				Path = Path.Replace("\\\\", "\\");
			}

			if (Path.StartsWith("\\"))
			{
				Path = Path.Substring(1);
			}
			if (Path.EndsWith("\\"))
			{
				Path = Path.Substring(0, Path.Length - 1);
			}
			return Path;
		}

		protected override void OnProcessPop3Message(Pop3Box box, Mediachase.Net.Mail.Pop3Message message)
		{
			try
			{
				// Step 0. Get settings
				FileSettings settings = GetSettings(box);
		
				// Step 0.5 Create storage
				string containerKey = "Workspace";
				if (settings.ContainerKey != String.Empty)
				{
					containerKey = settings.ContainerKey;
				}
				BaseIbnContainer baseContainer = BaseIbnContainer.Create("FileLibrary", containerKey);
				FileStorage storage = (FileStorage)baseContainer.LoadControl("FileStorage");

				// Step 1. Get Email sender info
				MailAddress from = message.Sender;
				if(from == null)
					from = message.From;

				// Step 2. Get Ibn User
				int userId = GetUserIdByEmail(from.Address, settings.OnlyExternalSenders);

				if (userId != -1)
				{
				}
				else
				{
					if(settings.AutoKillForUnknown)
						return;
				}

				// Step 3. Find Folder
				int FolderId = settings.FolderId;
				if (settings.FolderPattern != String.Empty)
				{
					String FolderName = ValidatePatternPath(ParseFolderPattern(settings.FolderPattern, message, userId));

					Mediachase.IBN.Business.ControlSystem.DirectoryInfo dirInfo = storage.GetDirectory(FolderId, FolderName, true);
					if (dirInfo != null)
					{
						FolderId = dirInfo.Id;
					}
					else FolderId = -1;
				}

				// Step 3. Create Files
				AddAttachments(storage, FolderId, message, userId);

				// Step 4. Create EML File
				if (settings.SaveMessageBodyAsEml)
				{
					UploadEmlFile(storage, FolderId, userId, message);
				}
				// Step 5. Create MHT File
				if (settings.SaveMessageBodyAsMht)
				{
					UploadMhtFile(storage, FolderId, userId, message);
				}
				// Step 5. Create MSG File
				if (settings.SaveMessageBodyAsMsg)
				{
					UploadMsgFile(storage, FolderId, userId, message);
				}
			}
			catch(Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex);
				throw;
			}

		}

		protected void AddAttachments(FileStorage storage, int FolderId, Pop3Message message, int userId)
		{
			ArrayList Attachments = new ArrayList();

			ExtractAttachments(Attachments, message.MimeEntries);
			UploadAttachments(storage, FolderId, Attachments, userId);
		}

		protected void ExtractAttachments(ArrayList Attachments, MimeEntryCollection MimeEntries)
		{
			foreach(MimeEntry entry in MimeEntries)
			{
				if(entry.ContentDisposition==Disposition.Attachment)
				{
					Attachments.Add(entry);
				}
			}
		}

		protected void UploadAttachments(FileStorage storage, int FolderId, ArrayList attach, int UserId)
		{
			foreach(MimeEntry entry in attach )
			{
				string FileName = entry.FileName;
				if (FileName == null)
				{
					if (entry.ContentType != null)
					{
						if(entry.ContentType.StartsWith("text/html"))
							continue;

						Hashtable parametrs = new Hashtable();

						Regex contentParser = new Regex("(?<type>[^;]+)(;(\\s)*(?<parameter>((?<attribute>[^=]+)=(?<value>((\"[^\"]*\")?[^;]*)))))*");

						Match match = contentParser.Match(entry.ContentType);

						string Type = match.Groups["type"].Value;

						int CaptureLen = match.Groups["parameter"].Captures.Count;
						for(int iIndex=0;iIndex<CaptureLen;iIndex++)
						{
							parametrs[match.Groups["attribute"].Captures[iIndex].Value.ToLower()] = match.Groups["value"].Captures[iIndex].Value.Replace("\"","");
						}
						FileName = (string)parametrs["name"];
					}
					if(FileName==null)
						FileName = string.Format("{0}.dat",(new Random()).Next());

				}
				using(MemoryStream memStream = new MemoryStream(entry.Body))
				{
					storage.SaveFile(FolderId, FileName, string.Empty, UserId, DateTime.Now, memStream);
				}
			}			
		}

		protected void UploadEmlFile(FileStorage storage, int FolderId, int UserId, Pop3Message message)
		{
			message.InputStream.Position = 0;

			string FileName = "Original_Message.eml";

			int index = 0;
			while (storage.FileExist(FileName, FolderId)) 
			{
				FileName = String.Format("Original_Message{0}.eml", index++);
			}

			storage.SaveFile(FolderId, FileName, string.Empty, UserId, DateTime.Now, message.InputStream);
		}

		protected void UploadMhtFile(FileStorage storage, int FolderId, int UserId, Pop3Message message)
		{
			MHTHelper helper = new MHTHelper(message);

			using(MemoryStream memStream = new MemoryStream())
			{
				using (StreamWriter writer = new StreamWriter(memStream))
				{
					helper.CreateMHT(writer);

					writer.Flush();

					memStream.Seek(0,SeekOrigin.Begin);

					string FileName = "Original_Message.mht";
					int index = 0;
					while (storage.FileExist(FileName, FolderId)) {
						FileName = String.Format("Original_Message{0}.mht", index++);
					}

					storage.SaveFile(FolderId, FileName, string.Empty, UserId, DateTime.Now, memStream);
				}
			}
		}

		protected void UploadMsgFile(FileStorage storage, int FolderId, int UserId, Pop3Message message)
		{
			System.Reflection.Assembly asm = typeof(FileExtendedPop3MessageHandler).Assembly;
			using (Stream stream = asm.GetManifestResourceStream("Mediachase.IBN.Business.Resources.template.msg"))
			{
				using(MemoryStream memStream = new MemoryStream())
				{
					using (MsgHelper helper = new MsgHelper(stream))
					{
						//helper.SetHtmlBody(message.From.Email);
						helper.SetSubject(message.Subject);
						helper.SetBody(message.BodyText);
						if (message.BodyHtml==null || message.BodyHtml.Trim()==String.Empty) 
						{
							helper.SetHtmlBody(message.BodyText);
						}
						else helper.SetHtmlBody(message.BodyHtml);

						MailAddress from = message.Sender;
						if (from == null)
							from = message.From;

						helper.SetSenderEmail(from.Address);
						helper.SetSenderName(from.DisplayName);
						helper.SetReceiverName(message.To);
						helper.SetDisplayTo(message.To);
						helper.SetCreationTimes(DateTime.UtcNow);

						helper.Commit();

						helper.createMSG(memStream);
						memStream.Flush();
						memStream.Seek(0,SeekOrigin.Begin);

						string FileName = "Original_Message.msg";
						int index = 0;
						while (storage.FileExist(FileName, FolderId)) 
						{
							FileName = String.Format("Original_Message{0}.msg", index++);
						}
						storage.SaveFile(FolderId, FileName, string.Empty, UserId, DateTime.Now, memStream);
					}
				}
			}
		}
	}
}
