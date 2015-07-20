using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database.Pop3
{
	/// <summary>
	/// Summary description for DBPop3Box.
	/// </summary>
	public class DBPop3Box
	{
		private DBPop3Box()
		{
		}

		#region Create
		public static IDataReader Create(string Name, string Server, int Port, string Login,
			string Password, bool Active, int Interval, DateTime LastRequest, 
			DateTime LastSuccessfulRequest, string LastErrorText, bool AutoKillForRead)
		{
			return DbHelper2.RunSpDataReader("Pop3BoxCreate",
				DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, Name),
				DbHelper2.mp("@Server", SqlDbType.NVarChar, 255, Server),
				DbHelper2.mp("@Port", SqlDbType.Int, Port),
				DbHelper2.mp("@Login", SqlDbType.NVarChar, 255, Login),
				DbHelper2.mp("@Password", SqlDbType.NVarChar, 255, Password),
				DbHelper2.mp("@Active", SqlDbType.Bit, Active),
				DbHelper2.mp("@Interval", SqlDbType.Int, Interval),
				DbHelper2.mp("@LastRequest", SqlDbType.DateTime, LastRequest),
				DbHelper2.mp("@LastSuccessfulRequest", SqlDbType.DateTime, LastSuccessfulRequest),
				DbHelper2.mp("@LastErrorText", SqlDbType.NVarChar, 255, LastErrorText),
				DbHelper2.mp("@AutoKillForRead", SqlDbType.Bit, AutoKillForRead));
		}
		#endregion

		#region CreateHandler
		public static void CreateHandler(int Pop3BoxId, string HandlerName)
		{
			DbHelper2.RunSp("Pop3BoxCreateHandler",
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, Pop3BoxId),
				DbHelper2.mp("@HandlerName", SqlDbType.NVarChar, 255, HandlerName));
		}
		#endregion

		#region GetList
		// returns: Pop3BoxId, Name, Server, Port, Login, Password, [Interval], LastRequest, LastSuccessfulRequest, AutoKillForRead
		public static IDataReader GetList()
		{
			return DbHelper2.RunSpDataReader("Pop3BoxGetList");
		}
		#endregion

		#region UpdateTime
		public static void UpdateTime(int Pop3BoxId, DateTime LastRequest, DateTime LastSuccessfulRequest, string LastErrorText)
		{
			DbHelper2.RunSp("Pop3BoxUpdateTime",
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, Pop3BoxId),
				DbHelper2.mp("@LastRequest", SqlDbType.DateTime, LastRequest),
				DbHelper2.mp("@LastSuccessfulRequest", SqlDbType.DateTime, LastSuccessfulRequest),
				DbHelper2.mp("@LastErrorText", SqlDbType.NVarChar, 255, LastErrorText));
		}
		#endregion

		#region Update
		public static void Update(int Pop3BoxId, string Name, string Server, int Port, 
			string Login, string Password, bool Active, int Interval, DateTime LastRequest, 
			DateTime LastSuccessfulRequest, string LastErrorText, bool AutoKillForRead)
		{
			DbHelper2.RunSp("Pop3BoxUpdate",
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, Pop3BoxId),
				DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, Name),
				DbHelper2.mp("@Server", SqlDbType.NVarChar, 255, Server),
				DbHelper2.mp("@Port", SqlDbType.Int, Port),
				DbHelper2.mp("@Login", SqlDbType.NVarChar, 255, Login),
				DbHelper2.mp("@Password", SqlDbType.NVarChar, 255, Password),
				DbHelper2.mp("@Active", SqlDbType.Bit, Active),
				DbHelper2.mp("@Interval", SqlDbType.Int, Interval),
				DbHelper2.mp("@LastRequest", SqlDbType.DateTime, LastRequest),
				DbHelper2.mp("@LastSuccessfulRequest", SqlDbType.DateTime, LastSuccessfulRequest),
				DbHelper2.mp("@LastErrorText", SqlDbType.NVarChar, 255, LastErrorText),
				DbHelper2.mp("@AutoKillForRead", SqlDbType.Bit, AutoKillForRead));
		}
		#endregion

		#region GetParameterList
		public static IDataReader GetParameterList(int Pop3BoxId)
		{
			return DbHelper2.RunSpDataReader("Pop3BoxGetParameterList",
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, Pop3BoxId));
		}
		#endregion

		#region AddParameter
		public static void AddParameter(int Pop3BoxId, string Name, string Value)
		{
			DbHelper2.RunSp("Pop3BoxAddParameter",
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, Pop3BoxId),
				DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, Name),
				DbHelper2.mp("@Value", SqlDbType.NVarChar, 255, Value));
		}
		#endregion

		#region DeleteParameters
		public static void DeleteParameters(int Pop3BoxId)
		{
			DbHelper2.RunSp("Pop3BoxDeleteParameters",
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, Pop3BoxId));
		}
		#endregion

		#region Delete
		public static void Delete(int Pop3BoxId)
		{
			DbHelper2.RunSp("Pop3BoxDelete",
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, Pop3BoxId));
		}
		#endregion

		#region DeleteHandlers
		public static void DeleteHandlers(int Pop3BoxId)
		{
			DbHelper2.RunSp("Pop3BoxDeleteHandlers",
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, Pop3BoxId));
		}
		#endregion

		#region CheckMessageUId
		public static bool CheckMessageUId(int Pop3BoxId, string MessageUId)
		{
			return (DbHelper2.RunSpInteger("Pop3BoxCheckMessageUId",
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, Pop3BoxId),
				DbHelper2.mp("@MessageUId", SqlDbType.VarChar, 70, MessageUId))==1);
		}
		#endregion	

		#region AddMessageUId
		public static void AddMessageUId(int Pop3BoxId, string MessageUId)
		{
			DbHelper2.RunSp("Pop3BoxAddMessageUId",
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, Pop3BoxId),
				DbHelper2.mp("@MessageUId", SqlDbType.VarChar, 70, MessageUId));
		}
		#endregion	

		#region DeleteMessageUIds
		public static void DeleteMessageUIds(int Pop3BoxId)
		{
			DbHelper2.RunSp("Pop3BoxDeleteMessageUIds",
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, Pop3BoxId));
		}
		#endregion
	}
}
