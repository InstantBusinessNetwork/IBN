using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

using Mediachase.IBN.Database.ControlSystem;
using Mediachase.IBN.Business.UserReport;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for ReportStorage.
	/// </summary>
	public class ReportStorage: IIbnControl
	{
		private IIbnContainer		_ownerContainer = null;
		private IbnControlInfo		_info = null;

		private ReportInfo[]		_hash = null;

		public ReportStorage()
		{
		}

		#region IIbnControl Members

		public void Init(IIbnContainer owner, IbnControlInfo controlInfo)
		{
			_ownerContainer = owner;
			_info =  controlInfo;
		}

		public string Name
		{
			get
			{
				return "ReportStorage";
			}
		}

		public string[] Actions
		{
			get
			{
				return new string[]{"Read"};
			}
		}

		public System.Collections.Specialized.NameValueCollection Parameters
		{
			get
			{
				return _info.Parameters;
			}
		}

		public IIbnContainer OwnerContainer
		{
			get
			{
				return _ownerContainer;
			}
		}

		public string[] GetBaseActions(string Action)
		{
			return new string[]{};
		}

		public string[] GetDerivedActions(string Action)
		{
			return new string[]{};
		}

		#endregion

		/// <summary>
		/// Gets the container key.
		/// </summary>
		/// <value>The container key.</value>
		protected string ContainerKey
		{
			get
			{
				return _ownerContainer.Key;
			}
		}

		protected void ClearHash()
		{
			_hash = null;
		}

		#region -- GetReport
		/// <summary>
		/// Gets the report.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public ReportInfo GetReport(int id)
		{
			ReportInfo retVal = null;

			using(IDataReader reader = DBReport.GetById(this.ContainerKey, id))
			{
				if(reader.Read())
				{
					UserReportInfo info = UserReportConfig.GetConfig().Reports[(string)reader["Name"]];
					if(info!=null)
                        retVal = new ReportInfo((int)reader["ReportId"], info, 
							(int)SqlHelper.DBNull2Null(reader["ReportCategoryId"],-1), 
							(string)SqlHelper.DBNull2Null(reader["ReportCategoryName"]));
				}
			}

			return retVal;
		}

		/// <summary>
		/// Gets the report.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public ReportInfo GetReport(string name)
		{
			ReportInfo retVal = null;

			using(IDataReader reader = DBReport.GetByName(this.ContainerKey, name))
			{
				if(reader.Read())
				{
					UserReportInfo info = UserReportConfig.GetConfig().Reports[(string)reader["Name"]];
					if(info!=null)
						retVal = new ReportInfo((int)reader["ReportId"], info, 
							(int)SqlHelper.DBNull2Null(reader["ReportCategoryId"],-1),
							(string)SqlHelper.DBNull2Null(reader["ReportCategoryName"]));
				}
			}

			return retVal;
		}

		#endregion

		#region -- GetReports --

		/// <summary>
		/// Gets the reports.
		/// </summary>
		/// <returns></returns>
		public ReportInfo[] GetReports()
		{
			if(_hash!=null)
				return _hash;

			lock(this)
			{
				if(_hash!=null)
					return _hash;

				ArrayList retVal = new ArrayList();

				foreach(UserReportInfo info in UserReportConfig.GetConfig().Reports)
				{
					using(IDataReader reader = DBReport.GetByName(this.ContainerKey, info.Name))
					{
						if(reader.Read())
						{
							// Existed report
							retVal.Add(new ReportInfo((int)reader["ReportId"], info, 
								(int)SqlHelper.DBNull2Null(reader["ReportCategoryId"],-1),
								(string)SqlHelper.DBNull2Null(reader["ReportCategoryName"])));
							continue;
						}
					}

					// A new report
					int ReportId = DBReport.Create(this.ContainerKey, info.Name);
					retVal.Add(new ReportInfo(ReportId, info, -1, null));
				}

				_hash = (ReportInfo[])retVal.ToArray(typeof(ReportInfo));
			}

			return _hash;
		}

		/// <summary>
		/// Gets the reports.
		/// </summary>
		/// <param name="category">The category.</param>
		/// <returns></returns>
		public ReportInfo[] GetReports(ReportCategoryInfo category)
		{
			return GetReports(category==null?null:category.Name);		
		}

		/// <summary>
		/// Gets the reports.
		/// </summary>
		/// <param name="categoryId">The category id.</param>
		/// <returns></returns>
		public ReportInfo[] GetReports(int categoryId)
		{
			ArrayList retVal = new ArrayList();

			foreach(ReportInfo report in this.GetReports())
			{
				if((report.Category==null && categoryId<=0) || 
					(report.Category!=null && report.Category.Id == categoryId))
					retVal.Add(report);
			}

			return (ReportInfo[])retVal.ToArray(typeof(ReportInfo));		
		}

		/// <summary>
		/// Gets the reports.
		/// </summary>
		/// <param name="category">The category.</param>
		/// <returns></returns>
		public ReportInfo[] GetReports(string category)
		{
			ArrayList retVal = new ArrayList();

			foreach(ReportInfo report in this.GetReports())
			{
				if((report.Category==null && category==null) || 
					(report.Category!=null && string.Compare(report.Category.Name, category,true)==0))
					retVal.Add(report);
			}

			return (ReportInfo[])retVal.ToArray(typeof(ReportInfo));		
		}
		#endregion

		#region -- GetCategories
		/// <summary>
		/// Gets the categories.
		/// </summary>
		/// <returns></returns>
		public static ReportCategoryInfo[] GetCategories()
		{
			ArrayList retVal = new ArrayList();

			using(IDataReader reader = DBReport.GetCategoryList())
			{
				while(reader.Read())
				{
					retVal.Add(new ReportCategoryInfo((int)reader["ReportCategoryId"], (string)reader["Name"]));
				}
			}

			return (ReportCategoryInfo[])retVal.ToArray(typeof(ReportCategoryInfo));
		}
		#endregion

		#region -- CreateCategory --
		public ReportCategoryInfo CreateCategory(string CategoryName)
		{
			int id =  DBReport.CreateCategory(CategoryName);
			return new ReportCategoryInfo(id, CategoryName);
		}
		#endregion

		#region -- SetReportCategory --
		public void SetReportCategory(ReportInfo report, ReportCategoryInfo category)
		{
			SetReportCategory(report.Id, category==null?-1:category.Id);
		}

		public void SetReportCategory(int ReportId, int ReportCategoryId)
		{
			DBReport.SetReportCategory(ReportId, ReportCategoryId);

			ClearHash();
		}
		#endregion

		#region -- CanUserRunAction --
		public static bool CanUserRunAction(int UserId, string ContainerKey, int ReportId, string Action)
		{
			return DBReport.CanUserRunAction(UserId, ContainerKey, ReportId, Action);
		}

		public bool CanUserRunAction(int UserId, ReportInfo info, string Action)
		{
			return CanUserRunAction(UserId, info.Id, Action);
		}

		public bool CanUserRunAction(int UserId, int ReportId, string Action)
		{
			return DBReport.CanUserRunAction(UserId, _ownerContainer.Key, ReportId, Action);
		}

		public static bool CanUserRead(int UserId, string ContainerKey, int ReportId)
		{
			return CanUserRunAction(UserId, ContainerKey, ReportId, "Read");
		}

		public bool CanUserRead(int UserId, ReportInfo info)
		{
			return CanUserRead(UserId, info.Id);
		}

		public bool CanUserRead(int UserId, int ReportId)
		{
			return CanUserRunAction(UserId, ReportId, "Read");
		}
		#endregion

	}
}
