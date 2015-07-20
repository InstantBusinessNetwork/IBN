using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ClientManagement.Modules
{
	public partial class OrgItemsDeleteConfirm : System.Web.UI.UserControl
	{
		// if ObjectId is in QueryString, then we work with single object
		// otherwise we work with list 

		#region ObjectId
		protected PrimaryKeyId ObjectId
		{
			get
			{
				PrimaryKeyId retval = PrimaryKeyId.Empty;
				if (!String.IsNullOrEmpty(Request["ObjectId"]))
					retval = PrimaryKeyId.Parse(Request["ObjectId"]);
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				CancelButton.OnClientClick = Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true);

				if (ObjectId == PrimaryKeyId.Empty)	// list of organizations
				{
					DeleteTypeText.Text = GetGlobalResourceObject("IbnFrameWork.Client", "OrganizationsDeleteType").ToString();
					DeleteTypeList.Items[0].Text = GetGlobalResourceObject("IbnFrameWork.Client", "DeleteOrganizationsOnly").ToString();
					DeleteTypeList.Items[1].Text = GetGlobalResourceObject("IbnFrameWork.Client", "DeleteOrganizationsAndContacts").ToString();
				}
				else // single organization
				{
					DeleteTypeText.Text = GetGlobalResourceObject("IbnFrameWork.Client", "OrganizationDeleteType").ToString();
					DeleteTypeList.Items[0].Text = GetGlobalResourceObject("IbnFrameWork.Client", "DeleteOrganizationOnly").ToString();
					DeleteTypeList.Items[1].Text = GetGlobalResourceObject("IbnFrameWork.Client", "DeleteOrganizationAndContacts").ToString();
				}
			}
		}

		#region OkButton_Click
		protected void OkButton_Click(object sender, EventArgs e)
		{
			string refreshCommand = Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetRefreshCommand(this.Page);
			string paramString = String.Empty;

			if (!String.IsNullOrEmpty(refreshCommand))
			{
				CommandParameters cp = new CommandParameters(refreshCommand);
				cp.CommandArguments = new Dictionary<string, string>();
				cp.AddCommandArgument("DeleteType", DeleteTypeList.SelectedValue);

				if (ObjectId != PrimaryKeyId.Empty)
				{
					cp.AddCommandArgument("ObjectId", ObjectId.ToString());
					paramString = cp.ToString();
				}
				else if (Request.QueryString["GridId"] != null)
				{
					cp.AddCommandArgument("GridId", Request.QueryString["GridId"]);
					paramString = cp.ToString();
				}
				else
				{
					paramString = String.Empty;
				}
			}
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, paramString);
		}
		#endregion
	}
}