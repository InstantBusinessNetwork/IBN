using System;
using System.Configuration;

namespace Mediachase.IBN.Business.UserReport
{
	/// <summary>
	/// Summary description for UserReportConfig.
	/// </summary>
	public class UserReportConfig
	{
		private UserReportInfoList	_reports = new UserReportInfoList();

		/// <summary>
		/// Initializes a new instance of the <see cref="UserReportConfig"/> class.
		/// </summary>
		internal UserReportConfig()
		{
		}

		/// <summary>
		/// Gets the reports.
		/// </summary>
		/// <value>The reports.</value>
		public UserReportInfoList Reports
		{
			get
			{
				return _reports;
			}
		}

		/// <summary>
		/// Gets the config.
		/// </summary>
		/// <returns></returns>
		public static UserReportConfig GetConfig()
		{
			UserReportConfig config = (UserReportConfig)ConfigurationManager.GetSection("userReports");
			return config;
		}
	}
}
