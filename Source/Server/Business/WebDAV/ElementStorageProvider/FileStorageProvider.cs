using System;
using System.Data;
using System.IO;
using System.Web;

using Mediachase.Ibn.Data;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Business.WebDAV.Definition;
using Mediachase.Net.Wdom;

namespace Mediachase.IBN.Business.WebDAV.ElementStorageProvider
{
	/// <summary>
	/// Предоставляет доступ к элементам библиотеки FileStorage
	/// </summary>
	public class FileStorageProvider : WebDavPropertyStorage
	{
		//Обертка над тупостью !!!
		private class DummyFileInfoWrapper
		{
			private string _contentType;
			private int _fileBinaryId;
			private DateTime _created;
			private DateTime _modified;
			private int _legth;
			private string _name;

			private string _containerKey;
			private int _parentDirectoryId;

			public DummyFileInfoWrapper(Mediachase.IBN.Business.ControlSystem.FileInfo fileInfo, FileHistoryInfo historyInfo)
			{
				_contentType = fileInfo.FileBinaryContentType;
				_name = fileInfo.Name;
				_fileBinaryId = fileInfo.FileBinaryId;
				_legth = fileInfo.Length;
				_created = fileInfo.Created;
				_modified = fileInfo.Modified;
				_containerKey = fileInfo.ContainerKey;
				_parentDirectoryId = fileInfo.ParentDirectoryId;

				if (historyInfo != null)
				{
					_contentType = historyInfo.FileBinaryContentType;
					_name = historyInfo.Name;
					_fileBinaryId = historyInfo.FileBinaryId;
					_legth = historyInfo.Length;
					_modified = historyInfo.Modified;
				}
			}

			public string ContentType { get { return _contentType; } }
			public string Name { get { return _name; } }
			public int FileBinaryId { get { return _fileBinaryId; } }
			public int Length { get { return _legth; } }
			public DateTime Created { get { return _created; } }
			public DateTime Modified { get { return _modified; } }
			public string ContainerKey { get { return _containerKey; } }
			public int ParrentDirectoryId { get { return _parentDirectoryId; } }
		}

		/// <summary>
		/// Initializes the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="config">The config.</param>
		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			base.Initialize(name, config);
		}


		/// <summary>
		/// Gets the element info.
		/// </summary>
		/// <param name="webDavAbsPath">The web dav abs path.</param>
		/// <returns></returns>
		public override WebDavElementInfo GetElementInfo(string strTicket)
		{
			WebDavElementInfo retVal = null;
			try
			{
				WebDavTicket ticket = WebDavTicket.Parse(strTicket);
				if (ticket.IsCollection)
				{
					retVal = GetCollectionInfo(ticket);
				}
				else
				{

					retVal = GetResourceInfo(ticket);
				}

			}
			catch (System.FormatException)
			{
			}

			return retVal;
		}

		protected CollectionInfo GetCollectionInfo(WebDavTicket ticket)
		{

			CollectionInfo retVal = new CollectionInfo();

			retVal.Name = "root";
			retVal.Created = DateTime.MinValue;
			retVal.Modified = DateTime.MinValue;
			retVal.AbsolutePath = ticket.ToString();

			return retVal;
		}

		protected ResourceInfo GetResourceInfo(WebDavTicket ticket)
		{
			FileStorageAbsolutePath absPath = ticket.AbsolutePath as FileStorageAbsolutePath;
			if (absPath == null)
				throw new ArgumentException("ticket.AbsolutePath is null.");

			ResourceInfo result = new ResourceInfo();

			Mediachase.IBN.Business.ControlSystem.FileInfo fileInfo = null;

			//Получаем оригинальный файл
			using (IDataReader reader = Mediachase.IBN.Database.ControlSystem.DBFile.GetById(Security.CurrentUser.TimeZoneId, absPath.UniqueId))
			{
				if (reader.Read())
					fileInfo = new Mediachase.IBN.Business.ControlSystem.FileInfo(reader);
			}

			if (fileInfo == null)
				throw new HttpException(404, "Not found");

			FileHistoryInfo historyInfo = null;

			//Поддержка версий
			if (absPath.IsHistory)
			{
				foreach (FileHistoryInfo history in fileInfo.GetHistory())
				{
					if (history.Id == absPath.HistoryId)
					{
						historyInfo = history;
						break;
					}
				}
			}

			DummyFileInfoWrapper dummyInfo = new DummyFileInfoWrapper(fileInfo, historyInfo);
			result.AbsolutePath = ticket.ToString();
			result.Tag = dummyInfo;
			result.Name = dummyInfo.Name;
			result.ContentType = dummyInfo.ContentType;
			result.ContentLength = dummyInfo.Length;
			result.ParentName = "root";
			result.Modified = dummyInfo.Modified;
			result.Created = dummyInfo.Created;

			return result;
		}

		#region Stream

		/// <summary>
		/// Opens the read.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns></returns>
		public override System.IO.Stream OpenRead(Mediachase.Net.Wdom.WebDavElementInfo element)
		{
			FileStorageAutoCommitedTransactedStream result = null;

			if (element != null && !(element is CollectionInfo))
			{
				DummyFileInfoWrapper fileInfo = element.Tag as DummyFileInfoWrapper;
				if (fileInfo != null)
				{
					CheckRight(fileInfo, "Read");

					//update download counter
					Mediachase.IBN.Database.ControlSystem.DBFile.FileBinaryModifyCounter(fileInfo.FileBinaryId);
					try
					{
						result = new FileStorageAutoCommitedTransactedStream(DataContext.Current.BeginTransaction(), 0);
						result.InnerStream = Mediachase.IBN.Database.ControlSystem.DBFile.OpenRead(fileInfo.FileBinaryId);
					}
					catch (Exception)
					{
						throw new HttpException(404, "Not Found");
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Opens the write.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns></returns>
		public override System.IO.Stream OpenWrite(Mediachase.Net.Wdom.WebDavElementInfo element, long contentLength)
		{
			FileStorageAutoCommitedTransactedStream result = null;

			if (element != null && !(element is CollectionInfo))
			{
				DummyFileInfoWrapper fileInfo = element.Tag as DummyFileInfoWrapper;
				if (fileInfo != null)
				{
					CheckRight(fileInfo, "Write");

					try
					{
						//Auto committed transaction stream wrapper
						result = new FileStorageAutoCommitedTransactedStream(DataContext.Current.BeginTransaction(), fileInfo.ContainerKey, 
																  fileInfo.ParrentDirectoryId, fileInfo.Name, contentLength);
						result.InnerStream = new MemoryStream();
					}
					catch (Exception)
					{
						throw new HttpException(404, "Not Found");
					}
				}
			}

			return result;
		}

		#endregion

		private static void CheckRight(DummyFileInfoWrapper fileInfo, string action)
		{
			bool isActionAllowed = false;

			int userId = Security.CurrentUser.UserID;

			if (fileInfo.ContainerKey.StartsWith("ForumNodeId_"))
			{
				// Extract forumNodeId
				int forumNodeId = int.Parse(fileInfo.ContainerKey.Split('_')[1]);

				// Find incidentId by ForumNodeId
				string forumContainerKey = ForumThreadNodeInfo.GetOwnerContainerKey(forumNodeId);
				int incidentId = int.Parse(forumContainerKey.Split('_')[1]);

				// Check Security
				switch (action)
				{
					case "Read":
						isActionAllowed = Incident.CanRead(incidentId);
						break;
					case "Write":
						isActionAllowed = Incident.CanUpdate(incidentId);
						break;
				}
			}
			else if (fileInfo.ContainerKey.StartsWith("DocumentVers_"))
			{
				// Extract documentVersionId
				int documentId = int.Parse(fileInfo.ContainerKey.Split('_')[1]);

				// Check Security
				switch (action)
				{
					case "Read":
						isActionAllowed = Document.CanRead(documentId);
						break;
					case "Write":
						isActionAllowed = Document.CanAddVersion(documentId);
						break;
				}
			}
			else
			{
				isActionAllowed = FileStorage.CanUserRunAction(userId, fileInfo.ContainerKey, fileInfo.ParrentDirectoryId, action);
				//retVal = FileStorage.CanUserRead(Security.CurrentUser.UserID, fileInfo.ContainerKey, fileInfo.ParrentDirectoryId);
			}

			if (!isActionAllowed)
				throw new HttpException(403, "Operation '" + action + "' is forbidden.");
		}

		#region Not implemented
		/// <summary>
		/// Creates the resource.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		public override Mediachase.Net.Wdom.ResourceInfo CreateResource(string fileName)
		{
			throw new NotImplementedException();
			//ResourceInfo retVal = null;
			//int ContentTypeId = DSFile.GetContentTypeByExtension(Path.GetExtension(fileName));

			//try
			//{
			//    using (IDataReader reader = DBFile.Create(Security.CurrentUser.TimeZoneId, fileName,
			//                                              string.Empty, Security.CurrentUser.UserID, DateTime.UtcNow, string.Empty))
			//    {
			//        if (reader.Read())
			//        {
			//            retVal = FileInfo2ResourceInfo(new FileInfo(this, reader));
			//        }
			//    }
			//}
			//catch (System.Exception e)
			//{
			//    throw;
			//}

			//return retVal;
		}
		public override void Delete(Mediachase.Net.Wdom.WebDavElementInfo element)
		{
			throw new NotImplementedException();
		}

		public override void CopyTo(Mediachase.Net.Wdom.WebDavElementInfo srcElement, Mediachase.Net.Wdom.WebDavElementInfo destElInfo)
		{
			throw new NotImplementedException();
		}

		public override Mediachase.Net.Wdom.CollectionInfo CreateCollection(string path)
		{
			throw new NotImplementedException();
		}


		public override Mediachase.Net.Wdom.WebDavElementInfo[] GetChildElements(Mediachase.Net.Wdom.WebDavElementInfo element)
		{
			throw new NotImplementedException();
		}

		public override void MoveTo(Mediachase.Net.Wdom.WebDavElementInfo srcElement, Mediachase.Net.Wdom.WebDavElementInfo destElInfo)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
