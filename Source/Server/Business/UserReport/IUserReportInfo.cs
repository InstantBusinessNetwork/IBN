using System;

namespace Mediachase.IBN.Business.UserReport
{
	/// <summary>
	/// Summary description for IUserReportInfo.
	/// </summary>
	public interface IUserReportInfo
	{
		/// <summary>
		/// Gets the name of the show.
		/// </summary>
		/// <value>The name of the show.</value>
		string ShowName{get;}

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The description.</value>
		string Description{get;}
	}
}
