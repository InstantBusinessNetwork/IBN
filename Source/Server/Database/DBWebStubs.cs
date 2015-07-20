using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBWebStubs.
	/// </summary>
	public class DBWebStubs
	{
		#region Create
		public static int Create(object UserId, string Abbreviation, string ToolTip,
			string Url, bool OpenInBrowser, int Width, int Height)
		{
			return DbHelper2.RunSpInteger("StubCreate",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@Abbreviation", SqlDbType.NChar, 2, Abbreviation),
				DbHelper2.mp("@ToolTip", SqlDbType.NVarChar, 250, ToolTip),
				DbHelper2.mp("@Url", SqlDbType.VarChar, 1000, Url),
				DbHelper2.mp("@OpenInBrowser", SqlDbType.Bit, OpenInBrowser),
				DbHelper2.mp("@Width", SqlDbType.Int, Width),
				DbHelper2.mp("@Height", SqlDbType.Int, Height));
		}
		#endregion

		#region Update
		public static void Update(int StubId, string Abbreviation, string ToolTip,
			string Url, bool OpenInBrowser, int Width, int Height)
		{
			DbHelper2.RunSp("StubUpdate",
				DbHelper2.mp("@StubId", SqlDbType.Int, StubId),
				DbHelper2.mp("@Abbreviation", SqlDbType.NChar, 2, Abbreviation),
				DbHelper2.mp("@ToolTip", SqlDbType.NVarChar, 250, ToolTip),
				DbHelper2.mp("@Url", SqlDbType.VarChar, 1000, Url),
				DbHelper2.mp("@OpenInBrowser", SqlDbType.Bit, OpenInBrowser),
				DbHelper2.mp("@Width", SqlDbType.Int, Width),
				DbHelper2.mp("@Height", SqlDbType.Int, Height));
		}
		#endregion

		#region Delete
		public static void Delete(int StubId)
		{
			DbHelper2.RunSp("StubDelete", 
				DbHelper2.mp("@StubId", SqlDbType.Int, StubId));
		}
		#endregion

		#region AddStubGroup
		public static void AddStubGroup(int StubId, int GroupId)
		{
			DbHelper2.RunSp("StubGroupAdd",
				DbHelper2.mp("@StubId", SqlDbType.Int, StubId),
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region DeleteStubGroup
		public static void DeleteStubGroup(int StubId, int GroupId)
		{
			DbHelper2.RunSp("StubGroupDelete",
				DbHelper2.mp("@StubId", SqlDbType.Int, StubId),
				DbHelper2.mp("@GroupId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region GetListGroupsByStub
		/// <summary>
		/// GroupId, GroupName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListGroupsByStub(int StubId)
		{
			return DbHelper2.RunSpDataReader("StubGroupsGetByStub",
				DbHelper2.mp("@StubId", SqlDbType.Int, StubId));
		}
		#endregion

		#region GetStubIcon
		/// <summary>
		/// Reader returns fields:
		///		Icon
		/// </summary>
		public static IDataReader GetStubIcon(int StubId)
		{
			return DbHelper2.RunSpDataReaderBlob("StubGetIcon", 
				DbHelper2.mp("@StubId", SqlDbType.Int, StubId));
		}
		#endregion

		#region UpdateStubIcon
		public static void UpdateStubIcon(int StubId, byte[] Icon)
		{
			if (Icon != null)
				DbHelper2.RunSp("StubUpdateIcon",
					DbHelper2.mp("@StubId", SqlDbType.Int, StubId), 
					DbHelper2.mp("@Icon", SqlDbType.Image, Icon.Length, Icon));
			else
				DbHelper2.RunSp("StubUpdateIcon",
					DbHelper2.mp("@StubId", SqlDbType.Int, StubId),
					DbHelper2.mp("@Icon", SqlDbType.Image, DBNull.Value));
		}
		#endregion

		#region Hide
		public static void Hide(int StubId, int UserId)
		{
			DbHelper2.RunSp("StubHide", 
				DbHelper2.mp("@StubId", SqlDbType.Int, StubId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region Show
		public static void Show(int StubId, int UserId)
		{
			DbHelper2.RunSp("StubShow", 
				DbHelper2.mp("@StubId", SqlDbType.Int, StubId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListStubsForAdmin
		/// <summary>
		/// StubId, Abbreviation, ToolTip, Url, OpenInBrowser, Width, Height, IsInternal, HasIcon
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListStubsForAdmin()
		{
			return DbHelper2.RunSpDataReader("StubsGetWithoutUser");
		}
		#endregion

		#region GetListStubsForUser
		/// <summary>
		/// StubId, Abbreviation, ToolTip, Url, OpenInBrowser, Width, Height, IsInternal, HasIcon
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListStubsForUser(int UserId)
		{
			return DbHelper2.RunSpDataReader("StubsGetWithUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListGroupStubsForUser
		/// <summary>
		/// StubId, Abbreviation, ToolTip, Url, OpenInBrowser, Width, Height, IsInternal, IsVisible, HasIcon 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListGroupStubsForUser(int UserId)
		{
			return DbHelper2.RunSpDataReader("StubsGetInheritedForUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetStub
		/// <summary>
		/// StubId, Abbreviation, ToolTip, Url, OpenInBrowser, Width, Height, IsInternal, UserId, HasIcon 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetStub(int StubId)
		{
			return DbHelper2.RunSpDataReader("StubGet",
				DbHelper2.mp("@StubId", SqlDbType.Int, StubId));
		}
		#endregion

		#region GetListVisibleStubsForUser
		/// <summary>
		/// StubId, Abbreviation, ToolTip, Url, OpenInBrowser, Width, Height, IsInternal, HasIcon
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListVisibleStubsForUser(int UserId)
		{
			return DbHelper2.RunSpDataReader("StubsGetVisibleForUser",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion
	}
}
