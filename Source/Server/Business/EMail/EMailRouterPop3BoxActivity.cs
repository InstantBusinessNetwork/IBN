using System;
using System.Collections;
using Mediachase.IBN.Database.EMail;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for EMailRouterPop3BoxActivity.
	/// </summary>
	public class EMailRouterPop3BoxActivity
	{
		private EMailRouterPop3BoxActivityRow _srcRow = null;

		private DateTime _userLastRequest = DateTime.MinValue;
		private DateTime _userLastSuccessfulRequest = DateTime.MinValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="EMailRouterPop3BoxActivity"/> class.
		/// </summary>
		/// <param name="row">The row.</param>
		private EMailRouterPop3BoxActivity(EMailRouterPop3BoxActivityRow row)
		{
			_srcRow = row;
		}

		/// <summary>
		/// Loads the specified E mail router POP3 box id.
		/// </summary>
		/// <param name="EMailRouterPop3BoxId">The E mail router POP3 box id.</param>
		/// <returns></returns>
		public static EMailRouterPop3BoxActivity Load(int EMailRouterPop3BoxId)
		{
			EMailRouterPop3BoxActivityRow[] rows = EMailRouterPop3BoxActivityRow.List(EMailRouterPop3BoxId);

			if(rows.Length>0)
			{
				return new EMailRouterPop3BoxActivity(rows[0]);
			}

			EMailRouterPop3BoxActivityRow newRow = new EMailRouterPop3BoxActivityRow();
			newRow.EMailRouterPop3BoxId = EMailRouterPop3BoxId;

			return new EMailRouterPop3BoxActivity(newRow);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is active.
		/// </summary>
		/// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
		public bool IsActive
		{
			get 
			{
				return _srcRow.IsActive;
			}
		}

		/// <summary>
		/// Gets the last request.
		/// </summary>
		/// <value>The last request.</value>
		public DateTime LastRequest
		{
			get 
			{
				if(_userLastRequest==DateTime.MinValue)
					_userLastRequest = Database.DBCommon.GetLocalDate(Security.CurrentUser.TimeZoneId,_srcRow.LastRequest);

				return _userLastRequest;
			}
		}

		/// <summary>
		/// Gets the last successful request.
		/// </summary>
		/// <value>The last successful request.</value>
		public DateTime LastSuccessfulRequest
		{
			get 
			{
				if(_userLastSuccessfulRequest==DateTime.MinValue)
					_userLastSuccessfulRequest = Database.DBCommon.GetLocalDate(Security.CurrentUser.TimeZoneId,_srcRow.LastSuccessfulRequest);

				return _userLastSuccessfulRequest;
			}
		}

		/// <summary>
		/// Gets the error text.
		/// </summary>
		/// <value>The error text.</value>
		public string ErrorText
		{
			get 
			{
				return _srcRow.ErrorText;
			}
		}

		/// <summary>
		/// Gets the total message count.
		/// </summary>
		/// <value>The total message count.</value>
		public int TotalMessageCount
		{
			get 
			{
				return _srcRow.TotalMessageCount;
			}
		}
	}
}
