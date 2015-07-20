using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Web;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for EditManagers.
	/// </summary>
	public partial class EditManagers : System.Web.UI.Page
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(EditManagers).Assembly);

		#region ProjectId
		private int ProjectId
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectId"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			if (!IsPostBack)
				BindValues();

			btnSave.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");

			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			if (Request["closeFramePopup"] != null)
				btnCancel.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{;}}", Request["closeFramePopup"]));
			else
				btnCancel.Attributes.Add("onclick", "window.close()");
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
		}
		#endregion

		#region BindValues()
		private void BindValues()
		{
			Dictionary<string, string> items;
			List<KeyValuePair<string, string>> sortedItems;

			int managerId = -1;
			int execManagerId = -1;
			using (IDataReader reader = Project.GetProject(ProjectId))
			{
				if (reader.Read())
				{
					managerId = (int)reader["ManagerId"];
					if (reader["ExecutiveManagerId"] != DBNull.Value)
						execManagerId = (int)reader["ExecutiveManagerId"];
				}
			}

			#region project managers
			items = new Dictionary<string, string>();
			using (IDataReader reader = SecureGroup.GetListAllUsersInGroup((int)InternalSecureGroups.ProjectManager))
			{
				while (reader.Read())
				{
					items.Add(reader["UserId"].ToString(), reader["LastName"].ToString() + " " + reader["FirstName"].ToString());
				}
			}

			// Sort by value
			sortedItems = new List<KeyValuePair<string, string>>(items);
			sortedItems.Sort(
				delegate(KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
				{
					return firstPair.Value.CompareTo(nextPair.Value);
				}
			);

			// DropDown with Managers
			if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
			{
				foreach (KeyValuePair<string, string> kvp in sortedItems)
					ddlManager.Items.Add(new ListItem(kvp.Value, kvp.Key));

				CommonHelper.SafeSelect(ddlManager, managerId.ToString());
				lblManager.Visible = false;
			}
			else
			{
				ddlManager.Visible = false;
				lblManager.Text = CommonHelper.GetUserStatus(managerId);
			}
			#endregion

			#region executive managers
			using (IDataReader reader = SecureGroup.GetListAllUsersInGroup((int)InternalSecureGroups.ExecutiveManager))
			{
				while (reader.Read())
				{
					if (!items.ContainsKey(reader["UserId"].ToString()))
						items.Add(reader["UserId"].ToString(), reader["LastName"].ToString() + " " + reader["FirstName"].ToString());
				}
			}

			// Sort by value
			sortedItems = new List<KeyValuePair<string, string>>(items);
			sortedItems.Sort(
				delegate(KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
				{
					return firstPair.Value.CompareTo(nextPair.Value);
				}
			);

			// DropDown with Executive Managers
			ListItem li = new ListItem(LocRM.GetString("NotSet"), "0");
			ddlExecManager.Items.Add(li);
			foreach (KeyValuePair<string, string> kvp in sortedItems)
				ddlExecManager.Items.Add(new ListItem(kvp.Value, kvp.Key));
			CommonHelper.SafeSelect(ddlExecManager, execManagerId.ToString());
			#endregion
		}
		#endregion

		#region btnSave_ServerClick
		private void btnSave_ServerClick(object sender, EventArgs e)
		{
			if (ddlManager.Visible)
				Project2.UpdateManagers(ProjectId, int.Parse(ddlManager.SelectedValue), int.Parse(ddlExecManager.SelectedValue));
			else
				Project2.UpdateExecutiveManager(ProjectId, int.Parse(ddlExecManager.SelectedValue));

			if (Request["closeFramePopup"] != null)
			{
				CommandParameters cp = new CommandParameters("MC_PM_Managers");
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					"try {window.opener.top.frames['right'].location.href='../Projects/ProjectView.aspx?ProjectId=" + ProjectId + "';}" +
					"catch (e){} window.close();", true);
		}
		#endregion
	}
}
