using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBContentType.
	/// </summary>
	public class DBContentType
	{
		#region Create
		public static int Create(string Extension, string ContentTypeString,
			string FriendlyName, object IconFileId, object BigIconFileId, bool AllowWebDav, bool AllowNewWindow, bool AllowForceDownload)
		{
			return DbHelper2.RunSpInteger("ContentTypeCreate",
				DbHelper2.mp("@Extension", SqlDbType.NVarChar, 10, Extension),
				DbHelper2.mp("@ContentTypeString", SqlDbType.NVarChar, 250, ContentTypeString),
				DbHelper2.mp("@FriendlyName", SqlDbType.NVarChar, 50, FriendlyName),
				DbHelper2.mp("@IconFileId", SqlDbType.Int, IconFileId),
				DbHelper2.mp("@BigIconFileId", SqlDbType.Int, BigIconFileId),
				DbHelper2.mp("@AllowWebDav", SqlDbType.Bit, AllowWebDav),
				DbHelper2.mp("@AllowNewWindow", SqlDbType.Bit, AllowNewWindow),
				DbHelper2.mp("@AllowForceDownload", SqlDbType.Bit, AllowForceDownload));
		}
		#endregion

		#region Update
		public static void Update(int ContentTypeId, string FriendlyName, bool AllowWebDav, bool AllowNewWindow, bool AllowForceDownload)
		{
			DbHelper2.RunSp("ContentTypeUpdate",
				DbHelper2.mp("@ContentTypeId", SqlDbType.Int, ContentTypeId),
				DbHelper2.mp("@FriendlyName", SqlDbType.NVarChar, 50, FriendlyName),
				DbHelper2.mp("@AllowWebDav", SqlDbType.Bit, AllowWebDav),
				DbHelper2.mp("@AllowNewWindow", SqlDbType.Bit, AllowNewWindow),
				DbHelper2.mp("@AllowForceDownload", SqlDbType.Bit, AllowForceDownload));
		}
		#endregion

		#region GetContentTypeByExtension
		/// <summary>
		/// ContentTypeId, Extension, ContentTypeString, FriendlyName, AllowWebDav, AllowNewWindow, AllowForceDownload
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetContentTypeByExtension(string Extension)
		{
			return DbHelper2.RunSpDataReader("ContentTypeGetByExtension",
				DbHelper2.mp("@Extension", SqlDbType.NVarChar, 10, Extension));
		}
		#endregion

		#region GetIconFileId
		public static int GetIconFileId(int ContentTypeId)
		{
			return DbHelper2.RunSpInteger("ContentTypeGetIconFileId",
				DbHelper2.mp("@ContentTypeId", SqlDbType.Int, ContentTypeId));
		}
		#endregion

		#region GetBigIconFileId
		public static int GetBigIconFileId(int ContentTypeId)
		{
			return DbHelper2.RunSpInteger("ContentTypeGetBigIconFileId",
				DbHelper2.mp("@ContentTypeId", SqlDbType.Int, ContentTypeId));
		}
		#endregion

		#region UpdateIconFileId
		public static void UpdateIconFileId(int ContentTypeId, int IconFileId)
		{
			DbHelper2.RunSp("ContentTypeUpdateIconFileId",
				DbHelper2.mp("@ContentTypeId", SqlDbType.Int, ContentTypeId),
				DbHelper2.mp("@IconFileId", SqlDbType.Int, IconFileId));
		}
		#endregion

		#region UpdateBigIconFileId
		public static void UpdateBigIconFileId(int ContentTypeId, int BigIconFileId)
		{
			DbHelper2.RunSp("ContentTypeUpdateBigIconFileId",
				DbHelper2.mp("@ContentTypeId", SqlDbType.Int, ContentTypeId),
				DbHelper2.mp("@BigIconFileId", SqlDbType.Int, BigIconFileId));
		}
		#endregion

		#region Delete
		public static void Delete(int ContentTypeId)
		{
			DbHelper2.RunSp("ContentTypeDelete",
				DbHelper2.mp("@ContentTypeId", SqlDbType.Int, ContentTypeId));
		}
		#endregion

		#region GetContentType
		/// <summary>
		/// ContentTypeId, Extension, ContentTypeString, FriendlyName, 
		/// IconFileId, BigIconFileId, AllowWebDav, AllowNewWindow, AllowForceDownload
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetContentType(int ContentTypeId)
		{
			return DbHelper2.RunSpDataReader("ContentTypeGet",
				DbHelper2.mp("@ContentTypeId", SqlDbType.Int, ContentTypeId));
		}
		#endregion

		#region GetListContentTypes
		/// <summary>
		/// ContentTypeId, Extension, ContentTypeString, FriendlyName, 
		/// IconFileId, BigIconFileId, AllowWebDav, AllowNewWindow, AllowForceDownload
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListContentTypes()
		{
			return DbHelper2.RunSpDataReader("ContentTypesGet");
		}
		#endregion

		#region GetListContentTypesDataTable
		/// <summary>
		/// ContentTypeId, Extension, ContentTypeString, FriendlyName, IconFileId, BigIconFileId, 
		/// AllowWebDav, AllowNewWindow, AllowForceDownload
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListContentTypesDataTable()
		{
			return DbHelper2.RunSpDataTable("ContentTypesGet");
		}
		#endregion

		#region GetContentTypeByString
		/// <summary>
		/// ContentTypeId, Extension, ContentTypeString, FriendlyName, 
		/// IconFileId, BigIconFileId, AllowWebDav, AllowNewWindow, AllowForceDownload
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetContentTypeByString(string ContentTypeString)
		{
			return DbHelper2.RunSpDataReader("ContentTypeGetByString",
			  DbHelper2.mp("@ContentTypeString", SqlDbType.NVarChar, 250, ContentTypeString));
		}
		#endregion
	}
}
