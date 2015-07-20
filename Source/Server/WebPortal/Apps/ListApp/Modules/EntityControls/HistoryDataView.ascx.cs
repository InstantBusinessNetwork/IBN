using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Data.Meta.Management;
using System.Globalization;
using System.Collections.Generic;

namespace Mediachase.Ibn.Web.UI.ListApp.Modules.EntityControls
{
	public partial class HistoryDataView : System.Web.UI.UserControl
	{
		#region ClassName
		/// <summary>
		/// Gets the name of the class.
		/// </summary>
		/// <value>The name of the class.</value>
		protected string ClassName
		{
			get
			{
				string retval = String.Empty;
				if (Request["ClassName"] != null)
					retval = Request["ClassName"];
				return retval;
			}
		}
		#endregion

		#region HistoryClassName
		/// <summary>
		/// Gets the name of the history class.
		/// </summary>
		/// <value>The name of the history class.</value>
		protected string HistoryClassName
		{
			get
			{
				string retval = String.Empty;
				if (!String.IsNullOrEmpty(ClassName))
					retval = HistoryManager.GetHistoryMetaClassName(ClassName);
				return retval;
			}
		}
		#endregion

		private string _placeName = "ItemHistoryList";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				// Check Security
				ListInfo li = ListManager.GetListInfoByMetaClassName(ClassName);
				PrimaryKeyId listId = li.PrimaryKeyId.Value;
				if (!Mediachase.IBN.Business.ListInfoBus.CanRead((int)listId))
					throw new AccessDeniedException();
			}

			if (!String.IsNullOrEmpty(HistoryClassName))
			{
				gridHistory.DoPadding = false;
				gridHistory.ShowCheckBoxes = false;
				gridHistory.ClassName = HistoryClassName;
				gridHistory.PlaceName = _placeName;
				string profileName = CHelper.GetHistorySystemListViewProfile(HistoryClassName, _placeName);
				gridHistory.ProfileName = profileName;
				CHelper.AddToContext("HistoryClassName", HistoryClassName);
				
				gridHistory.DataBind();
			}
		}
	}
}