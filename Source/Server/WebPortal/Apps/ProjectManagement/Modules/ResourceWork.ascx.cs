using System;
using System.Collections.Generic;
using System.Data;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.Modules
{
	public partial class ResourceWork : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ResourceWork).Assembly);
		private UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		#region GroupId
		protected string GroupId
		{
			get
			{
				if (pc["ResWork_Group"] == null)
					pc["ResWork_Group"] = ((int)InternalSecureGroups.Everyone).ToString();
				return pc["ResWork_Group"];
			}
			set
			{
				pc["ResWork_Group"] = value;
			}
		}
		#endregion

		#region UserId
		protected string UserId
		{
			get
			{
				if (pc["ResWork_User"] == null)
					pc["ResWork_User"] = Mediachase.IBN.Business.Security.CurrentUser.UserID.ToString();
				return pc["ResWork_User"];
			}
			set
			{
				if (int.Parse(value) > 0)
					pc["ResWork_User"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				BindGroups();
			}
		}

		#region BindGroups
		private void BindGroups()
		{
			//Groups Binding
			GroupsList.Items.Clear();

			// Everyone
			GroupsList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Global", "GroupEveryone").ToString(), "1"));

			using (IDataReader reader = SecureGroup.GetListGroupsAsTree())
			{
				while (reader.Read())
				{
					string GroupName = CommonHelper.GetResFileString(reader["GroupName"].ToString());
					string Id = reader["GroupId"].ToString();
					int Level = (int)reader["Level"];
					for (int i = 0; i < Level; i++)
						GroupName = "  " + GroupName;
					ListItem item = new ListItem(GroupName, Id);
					GroupsList.Items.Add(item);
				}
			}

			//Saved Value
			ListItem li = GroupsList.Items.FindByValue(GroupId);
			if (li != null)
			{
				li.Selected = true;
			}
			else
			{
				GroupsList.SelectedIndex = 0;
				GroupId = GroupsList.SelectedValue;
			}

			//Users Binding
			BindUsers();
		}
		#endregion

		#region BindUsers
		private void BindUsers()
		{
			UsersList.Items.Clear();
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("UserId", typeof(string)));				// 0
			dt.Columns.Add(new DataColumn("UserName", typeof(string)));				// 1
			DataRow dr;

			using (IDataReader reader = SecureGroup.GetListAllUsersInGroup(int.Parse(GroupsList.SelectedValue)))
			{
				while (reader.Read())
				{
					if ((byte)reader["Activity"] == (byte)Mediachase.IBN.Business.User.UserActivity.Active)
					{
						dr = dt.NewRow();
						dr[0] = reader["UserId"].ToString();
						dr[1] = reader["LastName"].ToString() + " " + reader["FirstName"].ToString();
						dt.Rows.Add(dr);
					}
				}
			}

			if (dt.Rows.Count == 0)	// no users in group
			{
				dr = dt.NewRow();
				dr[0] = "-1";
				dr[1] = GetGlobalResourceObject("IbnFramework.Project", "NoUsers").ToString();
				dt.Rows.Add(dr);
			}

			DataView dv = dt.DefaultView;

			UsersList.DataSource = dv;
			UsersList.DataTextField = "UserName";
			UsersList.DataValueField = "UserId";
			UsersList.DataBind();

			//Saved Value
			ListItem li = UsersList.Items.FindByValue(UserId);
			if (li != null)
			{
				li.Selected = true;
			}
			else
			{
				UsersList.SelectedIndex = 0;
				UserId = UsersList.SelectedValue;
			}

			BindData();
		}
		#endregion

		#region BindData
		private void BindData()
		{
			ResourceWorkCtrl.Visible = true;
			ResourceWorkCtrl.UserId = int.Parse(UsersList.SelectedValue);
			ResourceWorkCtrl.BindData(true);
			
			if (int.Parse(UsersList.SelectedValue) > 0)
			{
				GraphControlMain.Visible = true;
				GraphControlMain.Users = UsersList.SelectedValue;
				GraphControlMain.IntervalDuration = 7;	// one week
				GraphControlMain.StartDate = Mediachase.IBN.Business.Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow).Date;
				GraphControlMain.CurDate = Mediachase.IBN.Business.Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow);
				GraphControlMain.ObjectTypes = string.Concat(
					((int)ObjectTypes.CalendarEntry).ToString(), ",",
					((int)ObjectTypes.ToDo).ToString(), ",",
					((int)ObjectTypes.Task).ToString(), ",",
					((int)ObjectTypes.Document).ToString(), ",",
					((int)ObjectTypes.Issue).ToString());
				GraphControlMain.DataBind();
			}
			else
			{
				GraphControlMain.Visible = false;
			}
		}
		#endregion

		#region GroupsList_SelectedIndexChanged
		protected void GroupsList_SelectedIndexChanged(object sender, EventArgs e)
		{
			GroupId = GroupsList.SelectedValue;
			BindUsers();
		}
		#endregion

		#region UsersList_SelectedIndexChanged
		protected void UsersList_SelectedIndexChanged(object sender, EventArgs e)
		{
			UserId = UsersList.SelectedValue;
			BindData();
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			if (int.Parse(UsersList.SelectedValue) > 0)
				secHeader.AddLink(GetGlobalResourceObject("IbnFramework.Calendar", "tLegend").ToString(), "javascript:ShowLegend()");
		}
		#endregion
	}
}