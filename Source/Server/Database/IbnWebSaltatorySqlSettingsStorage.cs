using System;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using Mediachase.WebSaltatoryControl;
using Mediachase.WebSaltatoryControl.SqlClient;
using System.Collections;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for SqlSettingsStorage.
	/// </summary>
	public class IbnWebSaltatorySqlSettingsStorage: ISettingsStorage
	{
		private static string _connectionString	=	string.Empty;

		public static void Init(string connectionString)
		{
			_connectionString = connectionString;
		}

		public IbnWebSaltatorySqlSettingsStorage()
		{
		}

		protected string ConnectionString
		{
			get
			{
				return _connectionString;
			}
		}

		#region ISettingsStorage Members

		public void Init(XmlNode	element)
		{
		}

		public ControlData[] Load(PageView currentPage, string controlPlaceId)
		{
			ArrayList retVal=new ArrayList();
			if(_connectionString!=string.Empty)
			{
				SqlContext context = new SqlContext();

				context.ConnectionString=this.ConnectionString;

				using(IDataReader reader = SqlHelper.ExecuteReader(context,CommandType.StoredProcedure,"page_ControlPropertiesSelectAllInControlPlace",
						  new SqlParameter("@PageUID",currentPage.Id),
						  new SqlParameter("@ControlPlaceID",controlPlaceId)))
				{
					while(reader.Read())
					{
						retVal.Add(new ControlData(reader.GetString(1),
							reader.GetString(2)));
					}//while(reader.Read())

				}//using(IDataReader reader = SqlHelper.ExecuteReader(context,CommandType.StoredProcedure,"page_ControlsSelectAllInControlPlace",
			}//if(_connectionString!=string.Empty)
						
			
			return (ControlData[])retVal.ToArray(typeof(ControlData));
		}

		public void Save(PageView currentPage, string controlPlaceId, ControlData[] data)
		{
			if(_connectionString!=string.Empty)
			{
				SqlContext context = new SqlContext();
				context.ConnectionString=_connectionString;
				try
				{
					context.BeginTransaction();
					//delete all controls in control place
					SqlHelper.ExecuteNonQuery(context,CommandType.StoredProcedure,"page_ControlPropertiesDeleteAllInControlPlace",
						new SqlParameter("@PageUID",currentPage.Id),new SqlParameter("@ControlPlaceID",controlPlaceId));
					//add new controls from array "data"
					for(int i=0;i<data.Length;i++)
					{
						SqlHelper.ExecuteNonQuery(context,CommandType.StoredProcedure,"page_ControlPropertiesInsertInControlPlace",
							new SqlParameter("@PageUID",currentPage.Id),
							new SqlParameter("@ControlPlaceID",controlPlaceId),
							new SqlParameter("@ControlIndex",i), 
							new SqlParameter("@ControlUID",data[i].ControlUID),
							new SqlParameter("@ControlProperties",data[i].Settings));
					}
					context.Commit();
				}
				catch
				{
					context.Rollback();
					throw;
				}
			}//if(_connectionString!=string.Empty)
		}

		#endregion
	}
}
