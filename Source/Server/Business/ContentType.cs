using System;
using System.IO;
using System.Data;
using System.Collections;
using Mediachase.IBN.Database;
using Mediachase.IBN.Business.ControlSystem;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for ContentType.
	/// </summary>
	public class ContentType
	{
		#region Create
		public static int Create(string Extension, string ContentTypeString,
			string FriendlyName, string FileName, Stream _inputStream, string BigFileName,
			Stream _bigInputStream, bool AllowWebDav, bool AllowNewWindow, bool AllowForceDownload)
		{
			int ContentTypeId = -1;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				int IconFileId = -1;
				int BigIconFileId = -1;
				if (FileName != null && FileName != "")
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = "ContentType";

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					Mediachase.IBN.Business.ControlSystem.FileInfo fi = fs.SaveFile(fs.Root.Id, FileName, _inputStream);
					IconFileId = fi.Id;
				}
				if (BigFileName != null && BigFileName != "")
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = "ContentType";

					BaseIbnContainer bic1 = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs1 = (FileStorage)bic1.LoadControl("FileStorage");
					Mediachase.IBN.Business.ControlSystem.FileInfo fi1 = fs1.SaveFile(fs1.Root.Id, BigFileName, _bigInputStream);
					BigIconFileId = fi1.Id;
				}

				object oIconFileId = DBNull.Value;
				if (IconFileId > 0)
					oIconFileId = IconFileId;
				object oBigIconFileId = DBNull.Value;
				if (BigIconFileId > 0)
					oBigIconFileId = BigIconFileId;
				ContentTypeId = DBContentType.Create(Extension, ContentTypeString, FriendlyName, oIconFileId, oBigIconFileId, AllowWebDav, AllowNewWindow, AllowForceDownload);

				tran.Commit();
			}
			return ContentTypeId;
		}
		#endregion

		#region Update
		public static void Update(int ContentTypeId, string FriendlyName,
			string FileName, Stream _inputStream, string BigFileName,
			Stream _bigInputStream, bool AllowWebDav, bool AllowNewWindow, bool AllowForceDownload)
		{
			//		int OldIconFileId = DBContentType.GetIconFileId(ContentTypeId);
			//		int OldBigIconFileId = DBContentType.GetBigIconFileId(ContentTypeId);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBContentType.Update(ContentTypeId, FriendlyName, AllowWebDav, AllowNewWindow, AllowForceDownload);
				if (FileName != null && FileName != "")
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = "ContentType";

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					Mediachase.IBN.Business.ControlSystem.FileInfo fi = fs.SaveFile(fs.Root.Id, FileName, _inputStream);

					DBContentType.UpdateIconFileId(ContentTypeId, fi.Id);
					//				if(OldIconFileId>0 && OldIconFileId!=fi.Id)
					//					fs.DeleteFile(OldIconFileId);
				}

				if (BigFileName != null && BigFileName != "")
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = "ContentType";

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					Mediachase.IBN.Business.ControlSystem.FileInfo fi = fs.SaveFile(fs.Root.Id, BigFileName, _bigInputStream);

					DBContentType.UpdateBigIconFileId(ContentTypeId, fi.Id);
					//				if(OldBigIconFileId>0 && OldBigIconFileId!=fi.Id)
					//					fs.DeleteFile(OldBigIconFileId);
				}

				tran.Commit();
			}
		}
		#endregion

		#region Delete
		public static void Delete(int ContentTypeId)
		{
			DBContentType.Delete(ContentTypeId);
		}
		#endregion

		#region GetContentType
		/// <summary>
		/// ContentTypeId, Extension, ContentTypeString, FriendlyName, 
		/// IconFileId, BigIconFileId, AllowWebDav, AllowNewWindow
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetContentType(int ContentTypeId)
		{
			return DBContentType.GetContentType(ContentTypeId);
		}

		/// <summary>
		/// ContentTypeId, Extension, ContentTypeString, FriendlyName, AllowWebDav, AllowNewWindow
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetContentTypeByExtension(string Extension)
		{
			return DBContentType.GetContentTypeByExtension(Extension);
		}

		#endregion

		#region GetListContentTypes
		/// <summary>
		/// ContentTypeId, Extension, ContentTypeString, FriendlyName, 
		/// IconFileId, BigIconFileId, AllowWebDav, AllowNewWindow
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListContentTypes()
		{
			return DBContentType.GetListContentTypes();
		}
		#endregion

		#region GetListContentTypesDataTable
		/// <summary>
		/// ContentTypeId, Extension, ContentTypeString, FriendlyName, 
		/// IconFileId, BigIconFileId, AllowWebDav, AllowNewWindow
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListContentTypesDataTable()
		{
			return DBContentType.GetListContentTypesDataTable();
		}
		#endregion

		#region GetContentTypeByString
		/// <summary>
		/// ContentTypeId, Extension, ContentTypeString, FriendlyName, 
		/// IconFileId, BigIconFileId, AllowWebDav, AllowNewWindow
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetContentTypeByString(string ContentTypeString)
		{
			return DBContentType.GetContentTypeByString(ContentTypeString);
		}
		#endregion
	}
}
