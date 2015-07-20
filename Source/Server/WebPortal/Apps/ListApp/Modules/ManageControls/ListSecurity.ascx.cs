using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Resources;
using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls
{
	public partial class ListSecurity : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Lists.Resources.strLists", typeof(ListSecurity).Assembly);

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

		#region CommandName
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		/// <value>The name of the command.</value>
		protected string CommandName
		{
			get
			{
				string retval = String.Empty;
				if (Request["CommandName"] != null)
					retval = Request["CommandName"];
				return retval;
			}
		}
		#endregion

		#region iListId
		private int _iListId = -1;
		private int iListId
		{
			get
			{
				if (_iListId < 0)
				{
					Mediachase.Ibn.Lists.ListInfo li = Mediachase.Ibn.Lists.ListManager.GetListInfoByMetaClassName(ClassName);
					_iListId = li.PrimaryKeyId.Value;
				}
				return _iListId;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			if (!IsPostBack)
			{
				BindToolbar();
				btnAdd.InnerText = LocRM.GetString("tAdd");
				btnAddGroup.InnerText = LocRM.GetString("tAddGroup");
				GetACLData();
				BindGroups();
				BindRights();
				BinddgMembers();
			}
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			btnAddGroup.Disabled = false;
			DataTable dt = (DataTable)ViewState["ACL"];
			foreach (DataRow dr in dt.Rows)
			{
				if (dr["PrincipalId"].ToString() == ddGroups.SelectedValue)
					btnAddGroup.Disabled = true;
				ListItem liItem = lbUsers.Items.FindByValue(dr["PrincipalId"].ToString());
				if (liItem != null)
					lbUsers.Items.Remove(liItem);
			}
			if (ddGroups.SelectedValue == "-1" || ddGroups.SelectedValue == "0" || int.Parse(ddGroups.SelectedValue) == (int)InternalSecureGroups.Roles)
				btnAddGroup.Disabled = true;
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			Mediachase.Ibn.Lists.ListInfo li = Mediachase.Ibn.Lists.ListManager.GetListInfoByMetaClassName(ClassName);
			string sName = li.Title;
			if (sName.Length > 33)
				sName = sName.Substring(0, 30) + "...";
			secHeader.Title = LocRM.GetString("tEditSec");

			secHeader.AddLink("~/Layouts/Images/saveitem.gif", LocRM.GetString("Save"), Page.ClientScript.GetPostBackClientHyperlink(btnSave, ""));
			secHeader.AddLink("~/Layouts/Images/cancel.gif", LocRM.GetString("Cancel"), "javascript:window.close();");
		}
		#endregion

		#region GetACLData
		private void GetACLData()
		{
			DataTable dt = ACLToDateTable(ListInfoBus.GetListAccessesDT(iListId));
			ViewState["ACL"] = dt;
		}

		private DataTable ACLToDateTable(DataTable ACLr)
		{
			DataTable dt = new DataTable();
			DataRow dr;

			dt.Columns.Add(new DataColumn("PrincipalId", typeof(int)));					// 0
			dt.Columns.Add(new DataColumn("IsGroup", typeof(bool)));					//1
			dt.Columns.Add(new DataColumn("Rights", typeof(string)));						//2
			dt.Columns.Add(new DataColumn("AllowLevel", typeof(byte)));
			foreach (DataRow dr1 in ACLr.Rows)
			{
				dr = dt.NewRow();
				dr["PrincipalId"] = dr1["PrincipalId"];
				dr["IsGroup"] = dr1["IsGroup"];
				switch ((byte)dr1["AllowLevel"])
				{
					case 3:
						dr["Rights"] = LocRM.GetString("tAdmin");
						break;
					case 1:
						dr["Rights"] = LocRM.GetString("tRead");
						break;
					case 2:
						dr["Rights"] = LocRM.GetString("tWrite");
						break;
					default:
						break;
				}
				dr["AllowLevel"] = (byte)dr1["AllowLevel"];
				dt.Rows.Add(dr);
			}
			return dt;
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			ddGroups.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Common", "FavoritesGroup").ToString(), "0"));

			if (Mediachase.Ibn.Lists.ListManager.GetListInfoByMetaClassName(ClassName).ProjectId.HasValue)
			{
				ddGroups.Items.Add(new ListItem(LocRM.GetString("tProject"), "-1"));
			}

			DataTable dt = SecureGroup.GetListGroupsAsTreeDataTable();
			foreach (DataRow row in dt.Rows)
			{
				ddGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(row["GroupName"].ToString()), row["GroupId"].ToString()));
			}
			
			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindUsers(int.Parse(li.Value));
		}
		#endregion

		#region BindUsers
		private void BindUsers(int groupId)
		{
			lbUsers.Items.Clear();
			DataTable dt = new DataTable();
			DataRow dr;

			dt.Columns.Add(new DataColumn("UserId", typeof(int)));					// 0
			dt.Columns.Add(new DataColumn("UserName", typeof(string)));				// 1

			if (groupId == -1)
			{
				dr = dt.NewRow();
				dr[0] = -1;
				dr[1] = LocRM.GetString("tManager");
				dt.Rows.Add(dr);

				dr = dt.NewRow();
				dr[0] = -2;
				dr[1] = LocRM.GetString("tTeam");
				dt.Rows.Add(dr);

				dr = dt.NewRow();
				dr[0] = -3;
				dr[1] = LocRM.GetString("tSponsors");
				dt.Rows.Add(dr);

				dr = dt.NewRow();
				dr[0] = -4;
				dr[1] = LocRM.GetString("tStake");
				dt.Rows.Add(dr);

				dr = dt.NewRow();
				dr[0] = -5;
				dr[1] = LocRM.GetString("tExMan");
				dt.Rows.Add(dr);
			}
			else if (groupId == 0) // Favorites
			{
				DataTable favorites = Mediachase.IBN.Business.Common.GetListFavoritesDT(ObjectTypes.User);
				foreach (DataRow row in favorites.Rows)
				{
					dr = dt.NewRow();
					dr[0] = (int)row["ObjectId"];
					dr[1] = (string)row["Title"];
					dt.Rows.Add(dr);
				}
			}
			else
			{
				using (IDataReader reader = SecureGroup.GetListAllUsersInGroup(groupId))
				{
					while (reader.Read())
					{
						int iUserId = (int)reader["UserId"];
						bool IsExternal = (bool)reader["IsExternal"];
						byte Activity = (byte)reader["Activity"];
						if (!(IsExternal || Activity == (byte)Mediachase.IBN.Business.User.UserActivity.Pending))
						{
							dr = dt.NewRow();
							dr[0] = iUserId;
							dr[1] = reader["LastName"].ToString() + " " + reader["FirstName"].ToString();
							dt.Rows.Add(dr);
						}
					}
				}
			}

			lbUsers.DataTextField = "UserName";
			lbUsers.DataValueField = "UserId";
			lbUsers.DataSource = dt.DefaultView;
			lbUsers.DataBind();
		}
		#endregion

		#region BindRights
		private void BindRights()
		{
			ddRights.Items.Clear();
			DataTable dt = new DataTable();
			DataRow dr;

			dt.Columns.Add(new DataColumn("RightId", typeof(int)));					// 0
			dt.Columns.Add(new DataColumn("RightName", typeof(string)));				// 1
			dr = dt.NewRow();
			dr[0] = 1;
			dr[1] = LocRM.GetString("tRead");
			dt.Rows.Add(dr);
			dr = dt.NewRow();
			dr[0] = 2;
			dr[1] = LocRM.GetString("tWrite");
			dt.Rows.Add(dr);
			dr = dt.NewRow();
			dr[0] = 3;
			dr[1] = LocRM.GetString("tAdmin");
			dt.Rows.Add(dr);

			ddRights.DataTextField = "RightName";
			ddRights.DataValueField = "RightId";
			ddRights.DataSource = dt.DefaultView;
			ddRights.DataBind();
		}
		#endregion

		#region BinddgMembers
		private void BinddgMembers()
		{
			dgMembers.Columns[1].HeaderText = LocRM.GetString("tName");
			dgMembers.Columns[2].HeaderText = LocRM.GetString("tRights");

			DataTable dt = (DataTable)ViewState["ACL"];
			DataView dv = dt.DefaultView;
			dv.Sort = "IsGroup DESC, PrincipalId";
			dgMembers.DataSource = dv;
			dgMembers.DataBind();

			foreach (DataGridItem dgi in dgMembers.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				ib.Attributes.Add("onclick", "if(confirm('" + LocRM.GetString("tWarning") + "')){DisableButtons(this);return true;}else return false;");
				ib.ToolTip = LocRM.GetString("Delete");
			}
		}
		#endregion

		#region GetLink
		protected string GetLink(int PID, bool IsGroup)
		{
			if (PID < 0)
			{
				switch (PID)
				{
					case -1:
						return String.Format("<img src='{0}' border=0 align='absmiddle'>&nbsp;{1}", ResolveClientUrl("~/layouts/images/icons/manager.GIF"), LocRM.GetString("tManager"));
					case -2:
						return String.Format("<img src='{0}' border=0 align='absmiddle'>&nbsp;{1}", ResolveClientUrl("~/layouts/images/icons/newgroup.GIF"), LocRM.GetString("tTeam"));
					case -3:
						return String.Format("<img src='{0}' border=0 align='absmiddle'>&nbsp;{1}", ResolveClientUrl("~/layouts/images/icons/sponsors.GIF"), LocRM.GetString("tSponsors"));
					case -4:
						return String.Format("<img src='{0}' border=0 align='absmiddle'>&nbsp;{1}", ResolveClientUrl("~/layouts/images/icons/stakeholders.GIF"), LocRM.GetString("tStake"));
					default:
						return String.Format("<img src='{0}' border=0 align='absmiddle'>&nbsp;{1}", ResolveClientUrl("~/layouts/images/icons/exmanager.GIF"), LocRM.GetString("tExMan"));
				}
			}
			else if (IsGroup)
				return CommonHelper.GetGroupLinkUL(PID);
			else
				return CommonHelper.GetUserStatusUL(PID);
		}
		#endregion

		#region ddGroups_ChangeGroup
		protected void ddGroups_ChangeGroup(object sender, System.EventArgs e)
		{
			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindUsers(int.Parse(li.Value));
			BinddgMembers();
		}
		#endregion

		#region btnAddGroup_Click
		protected void btnAddGroup_Click(object sender, System.EventArgs e)
		{
			DataTable ACLr = (DataTable)ViewState["ACL"];
			DataRow dr = ACLr.NewRow();
			dr["PrincipalId"] = int.Parse(ddGroups.SelectedValue);
			dr["IsGroup"] = true;
			switch (ddRights.SelectedValue)
			{
				case "1":
					dr["Rights"] = LocRM.GetString("tRead");
					dr["AllowLevel"] = 1;
					break;
				case "2":
					dr["Rights"] = LocRM.GetString("tWrite");
					dr["AllowLevel"] = 2;
					break;
				case "3":
					dr["Rights"] = LocRM.GetString("tAdmin");
					dr["AllowLevel"] = 3;
					break;
			}
			ACLr.Rows.Add(dr);

			ViewState["ACL"] = ACLr;

			ListItem _li = ddGroups.SelectedItem;
			if (_li != null)
				BindUsers(int.Parse(_li.Value));
			BinddgMembers();
		}
		#endregion 

		#region btnAdd_Click
		protected void btnAdd_Click(object sender, System.EventArgs e)
		{
			DataTable ACLr = (DataTable)ViewState["ACL"];
			DataRow dr;
			foreach (ListItem liItem in lbUsers.Items)
			{
				if (liItem.Selected)
				{
					dr = ACLr.NewRow();
					dr["PrincipalId"] = int.Parse(liItem.Value);
					dr["IsGroup"] = false;
					switch (ddRights.SelectedValue)
					{
						case "1":
							dr["Rights"] = LocRM.GetString("tRead");
							dr["AllowLevel"] = 1;
							break;
						case "2":
							dr["Rights"] = LocRM.GetString("tWrite");
							dr["AllowLevel"] = 2;
							break;
						case "3":
							dr["Rights"] = LocRM.GetString("tAdmin");
							dr["AllowLevel"] = 3;
							break;
					}
					ACLr.Rows.Add(dr);
				}
			}

			ViewState["ACL"] = ACLr;

			ListItem _li = ddGroups.SelectedItem;
			if (_li != null)
				BindUsers(int.Parse(_li.Value));
			BinddgMembers();
		}
		#endregion 

		#region dgMembers_DeleteCommand
		protected void dgMembers_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			DataTable ACLr = (DataTable)ViewState["ACL"];
			int Id = int.Parse(e.Item.Cells[0].Text);
			DataRow drRemove = null;
			foreach (DataRow dr in ACLr.Rows)
			{
				if ((int)dr["PrincipalId"] == Id)
				{
					drRemove = dr;
					break;
				}
			}
			if (drRemove != null)
				ACLr.Rows.Remove(drRemove);
			ViewState["ACL"] = ACLr;

			ListItem _li = ddGroups.SelectedItem;
			if (_li != null)
				BindUsers(int.Parse(_li.Value));
			BinddgMembers();
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			Mediachase.IBN.Business.ListInfoBus.UpdateListAccess(iListId, (DataTable)ViewState["ACL"]);
			if (String.IsNullOrEmpty(CommandName))
				CHelper.CloseIt(Response);
			else
			{
				CommandParameters cp = new CommandParameters(CommandName);
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, cp.ToString());
			}
		}
		#endregion
	}
}