using System;
using Mediachase.IBN.Business.ControlSystem;

namespace Mediachase.IBN.Business.UserReport
{
	[Flags]
	public enum UserReportType
	{
		Global		= 1,
		Personal	= 2,
		Project		= 4
	}

	/// <summary>
	/// Summary description for UserReportInfo.
	/// </summary>
	public class UserReportInfo
	{
		private string _name = string.Empty;
		private string _url = string.Empty;
		private UserReportType _type = UserReportType.Global;
		private string _infoClass =  string.Empty;
		private IUserReportInfo _info = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="UserReportInfo"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="url">The URL.</param>
		/// <param name="type">The type.</param>
		/// <param name="infoType">Type of the info.</param>
		public UserReportInfo(string name, string url, UserReportType type, string infoClass)
		{
			_name = name;
			_url = url;

			_type = type;

			_infoClass = infoClass;

			if(infoClass!=null && infoClass!= string.Empty)
			{
				try
				{
					_info = (IUserReportInfo)AssemblyHelper.LoadObject(infoClass, typeof(IUserReportInfo));
				}
				catch(Exception ex)
				{
					System.Diagnostics.Trace.WriteLine(ex);
				}
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
				return _name;
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
				if(_info!=null)
					return _info.ShowName;
				return _name;
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
				return _type;
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
				return this.Type==UserReportType.Personal;
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
				return this.Type==UserReportType.Global;
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
				return this.Type==UserReportType.Project;
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
				if(_info!=null)
					return _info.Description;
				return string.Empty;
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
				return _url;
			}
		}
	}
}
