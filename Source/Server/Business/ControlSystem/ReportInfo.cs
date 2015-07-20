using System;
using Mediachase.IBN.Business.UserReport;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for ReportInfo.
	/// </summary>
	public class ReportInfo
	{
		private int _id = -1;
		private UserReportInfo _innerInfo = null;
		private ReportCategoryInfo _category = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportInfo"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="innerInfo">The inner info.</param>
		/// <param name="categoryId">The category id.</param>
		/// <param name="categoryName">Name of the category.</param>
		internal ReportInfo(int id, UserReportInfo innerInfo, int categoryId, string categoryName)
		{
			_id = id;
			_innerInfo = innerInfo;
			if(categoryName!=null)
				_category = new ReportCategoryInfo(categoryId, categoryName);
		}

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		public int Id
		{
			get
			{
				return _id;
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get
			{
				return _innerInfo.Name;
			}
		}

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>The type.</value>
		public UserReportType Type
		{
			get
			{
				return _innerInfo.Type;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is personal.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is personal; otherwise, <c>false</c>.
		/// </value>
		public bool IsPersonal
		{
			get
			{
				return _innerInfo.IsPersonal;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is global.
		/// </summary>
		/// <value><c>true</c> if this instance is global; otherwise, <c>false</c>.</value>
		public bool IsGlobal
		{
			get
			{
				return _innerInfo.IsGlobal;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is project.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is project; otherwise, <c>false</c>.
		/// </value>
		public bool IsProject
		{
			get
			{
				return _innerInfo.IsProject;
			}
		}

		/// <summary>
		/// Gets the URL.
		/// </summary>
		/// <value>The URL.</value>
		public string Url
		{
			get
			{
				return _innerInfo.Url;
			}
		}

		/// <summary>
		/// Gets the name of the show.
		/// </summary>
		/// <value>The name of the show.</value>
		public string ShowName
		{
			get
			{
				return _innerInfo.ShowName;
			}
		}

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Description
		{
			get
			{
				return _innerInfo.Description;
			}
		}

		/// <summary>
		/// Gets the category.
		/// </summary>
		/// <value>The category.</value>
		public ReportCategoryInfo Category
		{
			get
			{
				return _category;
			}
		}
	}
}
