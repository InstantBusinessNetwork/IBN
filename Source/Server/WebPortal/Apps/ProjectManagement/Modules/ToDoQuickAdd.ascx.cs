using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Data;
using System.Collections.Generic;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.Modules
{
	public partial class ToDoQuickAdd : System.Web.UI.UserControl
	{
		private const string GroupKey = "QuickToDoCreateGroupKey";
		protected UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		#region _projectId
		private int _projectId
		{
			get
			{
				int retVal = -1;
				if (Request["ProjectId"] != null && int.TryParse(Request["ProjectId"], out retVal))
					return retVal;
				return -1;
			}
		} 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			BindButtons();
			Delegates();

			if (!Page.IsPostBack)
			{
				dueDate.DefaultTimeString = PortalConfig.WorkTimeFinish;
				BindPriorities();
				BindGroups();
			}
		}

		#region Delegates
		private void Delegates()
		{
			btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
			ddGroups.SelectedIndexChanged += new EventHandler(ddGroups_SelectedIndexChanged);
			btnAdd.Click += new EventHandler(btnAdd_Click);
			btnDelete.Click += new EventHandler(btnDelete_Click);
			btnSearch.Click += new EventHandler(btnSearch_Click);
		} 
		#endregion

		#region BindButtons
		private void BindButtons()
		{
			btnSave.Text = GetGlobalResourceObject("IbnFramework.Common", "tSave").ToString();
			btnSave.CustomImage = this.Page.ResolveUrl("~/Layouts/images/saveitem.gif");
			btnCancel.Text = GetGlobalResourceObject("IbnFramework.Common", "tClose").ToString();
			btnCancel.CustomImage = this.Page.ResolveUrl("~/Layouts/images/cancel.gif");
			btnCancel.Attributes.Add("onclick", Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true));

			btnSearch.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_FindNow").ToString();
			
			lbAvailable.Attributes.Add("ondblclick", Page.ClientScript.GetPostBackEventReference(btnAdd, ""));
			lbSelected.Attributes.Add("ondblclick", Page.ClientScript.GetPostBackEventReference(btnDelete, ""));

			cbUpdateParent.Text = " " + GetGlobalResourceObject("IbnFramework.Global", "UpdateParent");
		}
		#endregion

		#region BindPriorities
		private void BindPriorities()
		{
			ddPriority.DataSource = ToDo.GetListPriorities();
			ddPriority.DataTextField = "PriorityName";
			ddPriority.DataValueField = "PriorityId";
			ddPriority.DataBind();

			CHelper.SafeSelect(ddPriority, PortalConfig.ToDoDefaultValuePriorityField);

			td1.Visible = td2.Visible = PortalConfig.ToDoAllowEditPriorityField;
		} 
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			ddGroups.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Common", "FavoritesGroup").ToString(), "0"));

			if (_projectId > 0)
			{
				int pID = -_projectId;
				ddGroups.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Global", "_mc_ProjectTeam").ToString(), pID.ToString()));
			}

			DataTable dt = SecureGroup.GetListGroupsAsTreeDataTable();
			foreach (DataRow row in dt.Rows)
			{
				ddGroups.Items.Add(new ListItem(CHelper.GetResFileString(row["GroupName"].ToString()), row["GroupId"].ToString()));
			}

			if (pc[_projectId + GroupKey] != null)
				CHelper.SafeSelect(ddGroups, pc[_projectId + GroupKey]);

			if (ddGroups.SelectedItem != null)
				BindGroupUsers(int.Parse(ddGroups.SelectedValue));
		} 
		#endregion

		#region BindGroupUsers
		private void BindGroupUsers(int groupId)
		{
			lbAvailable.Items.Clear();
			ArrayList alSelected = new ArrayList();
			foreach (ListItem liSelected in lbSelected.Items)
				alSelected.Add(int.Parse(liSelected.Value));

			if (groupId > 0)
			{
				using (IDataReader reader = SecureGroup.GetListActiveUsersInGroup(groupId))
				{
					while (reader.Read())
					{
						if (!alSelected.Contains((int)reader["UserId"]))
							lbAvailable.Items.Add(new ListItem(reader["LastName"].ToString() + " " + reader["FirstName"].ToString(), reader["UserId"].ToString()));
					}
				}
			}
			else if (groupId == 0)	// Favorites
			{
				DataTable favorites = Mediachase.IBN.Business.Common.GetListFavoritesDT(ObjectTypes.User);
				foreach (DataRow row in favorites.Rows)
				{
					if (!alSelected.Contains((int)row["ObjectId"]))
						lbAvailable.Items.Add(new ListItem((string)row["Title"], row["ObjectId"].ToString()));
				}
			}
			else
			{
				using (IDataReader reader = Project.GetListTeamMemberNamesWithManager(-groupId))
				{
					while (reader.Read())
					{
						if (!alSelected.Contains((int)reader["UserId"]))
							lbAvailable.Items.Add(new ListItem(reader["LastName"].ToString() + " " + reader["FirstName"].ToString(), reader["UserId"].ToString()));
					}
				}
			}
		}
		#endregion

		#region BindSearchedUsers
		private void BindSearchedUsers(string searchstr)
		{
			lbAvailable.Items.Clear();
			ArrayList alSelected = new ArrayList();
			foreach (ListItem liSelected in lbSelected.Items)
				alSelected.Add(int.Parse(liSelected.Value));

			using (IDataReader rdr = Mediachase.IBN.Business.User.GetListUsersBySubstring(searchstr))
			{
				while (rdr.Read())
				{
					if (!alSelected.Contains((int)rdr["UserId"]))
						lbAvailable.Items.Add(new ListItem((string)rdr["LastName"] + ", " + (string)rdr["FirstName"], rdr["UserId"].ToString()));
				}
			}
		}
		#endregion

		#region BindUsers
		private void BindUsers()
		{
			if (ViewState["SearchString"] == null)
			{
				if (ddGroups.SelectedItem != null)
					BindGroupUsers(int.Parse(ddGroups.SelectedValue));
			}
			else
				BindSearchedUsers(ViewState["SearchString"].ToString());
		}
		#endregion

		#region ddGroups_SelectedIndexChanged
		void ddGroups_SelectedIndexChanged(object sender, EventArgs e)
		{
			txtSearch.Text = "";
			ViewState["SearchString"] = null;

			if (ddGroups.SelectedItem != null)
			{
				pc[_projectId + GroupKey] = ddGroups.SelectedValue;
				BindGroupUsers(int.Parse(ddGroups.SelectedValue));
			}
		} 
		#endregion

		#region btnDelete_Click
		void btnDelete_Click(object sender, EventArgs e)
		{
			List<ListItem> liSelected = new List<ListItem>();
			foreach (ListItem li in lbSelected.Items)
				if (li.Selected)
					liSelected.Add(li);
			if (liSelected.Count > 0)
			{
				foreach (ListItem li in liSelected)
					lbSelected.Items.Remove(li);
				BindUsers();
				upAvailable.Update();
			}
		} 
		#endregion

		#region btnAdd_Click
		void btnAdd_Click(object sender, EventArgs e)
		{
			List<ListItem> liSelected = new List<ListItem>();
			foreach (ListItem li in lbAvailable.Items)
				if (li.Selected)
					liSelected.Add(li);
			if (liSelected.Count > 0)
			{
				foreach (ListItem li in liSelected)
				{
					li.Selected = false;
					lbSelected.Items.Add(li);
				}
				BindUsers();
				upSelected.Update();
			}
		} 
		#endregion

		#region btnSearch_Click
		void btnSearch_Click(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(txtSearch.Text))
			{
				ViewState["SearchString"] = txtSearch.Text;
				BindSearchedUsers(txtSearch.Text);
			}
		} 
		#endregion

		#region btnSave_ServerClick
		void btnSave_ServerClick(object sender, EventArgs e)
		{
			txtTitle.Text = HttpUtility.HtmlEncode(txtTitle.Text);
			txtDescription.Text = HttpUtility.HtmlEncode(txtDescription.Text);

			ArrayList resources = new ArrayList();
			foreach (ListItem li in lbSelected.Items)
				resources.Add(int.Parse(li.Value));
			if (resources.Count == 0)
				resources.Add(Mediachase.IBN.Business.Security.CurrentUser.UserID);

			PrimaryKeyId org_id = PrimaryKeyId.Empty;
			PrimaryKeyId contact_id = PrimaryKeyId.Empty;
			Mediachase.IBN.Business.Common.GetDefaultClient(PortalConfig.ToDoDefaultValueClientField, out contact_id, out org_id);
			
			ToDo.Create(_projectId, Mediachase.IBN.Business.Security.CurrentUser.UserID, txtTitle.Text,
				txtDescription.Text, DateTime.MinValue, dueDate.SelectedDate, int.Parse(ddPriority.SelectedValue),
				int.Parse(PortalConfig.ToDoDefaultValueActivationTypeField), 
				int.Parse(PortalConfig.ToDoDefaultValueCompetionTypeField), 
				bool.Parse(PortalConfig.ToDoDefaultValueMustConfirmField), 
				int.Parse(PortalConfig.ToDoDefaultValueTaskTimeField),
				Mediachase.IBN.Business.Common.StringToArrayList(PortalConfig.ToDoDefaultValueGeneralCategoriesField),
				null, null, resources, contact_id, org_id);

			string cmd = String.Empty;
			if (Request["CommandName"] != null)
			{
				CommandParameters cp = new CommandParameters(Request["CommandName"]);
				cmd = cp.ToString();
			}

			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cmd, cbUpdateParent.Checked);
		} 
		#endregion
	}
}