namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for SecureGroups.
	/// </summary>
	public partial class SecureGroupsPartners : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strSecureGroups", typeof(SecureGroupsPartners).Assembly);
		protected ArrayList VisiblePartnerGroups = new ArrayList();
		protected ArrayList VisibleUnpartnerGroups = new ArrayList();
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		private int iCurGroupId = -1;
		private int GroupID = -1;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			GetGroups();
			BindToolbar();
			if (!IsPostBack)
			{
				BindData();
			}
		}

		#region GetGroups
		private void GetGroups()
		{
			using (IDataReader reader = User.GetListSecureGroup(Security.CurrentUser.UserID))
			{
				if (reader.Read())
				{
					iCurGroupId = (int)reader["GroupId"];
				}
			}

			try
			{
				if (Request["SGroupID"] != null)
				{
					GroupID = int.Parse(Request["SGroupID"]);
					if (GroupID != 1 && GroupID != 6 && !SecureGroup.CanRead(GroupID))
						GroupID = iCurGroupId;
					pc["SecureGroup_CurrentGroup"] = GroupID.ToString();
				}
				else
				{
					try
					{
						GroupID = int.Parse(pc["SecureGroup_CurrentGroup"]);
						using (IDataReader r = SecureGroup.GetGroup(GroupID))
						{
							if (!r.Read())
							{
								GroupID = iCurGroupId;
								pc["SecureGroup_CurrentGroup"] = iCurGroupId.ToString();
							}
							else
							{
								if (GroupID != 1 && GroupID != 6 && !SecureGroup.CanRead(GroupID))
								{
									GroupID = iCurGroupId;
									pc["SecureGroup_CurrentGroup"] = iCurGroupId.ToString();
								}
							}
						}
					}
					catch (Exception)
					{
						pc["SecureGroup_CurrentGroup"] = iCurGroupId.ToString();
						GroupID = iCurGroupId;
					}
				}
			}
			catch
			{
				pc["SecureGroup_CurrentGroup"] = iCurGroupId.ToString();
				GroupID = iCurGroupId;
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			using (IDataReader reader = SecureGroup.GetGroup(GroupID))
			{
				if (reader.Read())
				{
					secHeader.Title = CommonHelper.GetResFileString((string)reader["GroupName"]);
				}
			}

			if (User.CanCreatePending())
			{
				secHeader.AddSeparator();
				secHeader.AddLink("<img alt='' src='../Layouts/Images/icons/pending_create.gif' title='" + LocRM.GetString("AddPendingUser") + "'/>", "../Directory/PendingEdit.aspx");
			}
		}
		#endregion

		#region BindData
		private void BindData()
		{
			VisiblePartnerGroups.Clear();
			VisibleUnpartnerGroups.Clear();
			VisiblePartnerGroups.Add(iCurGroupId);
			using (IDataReader reader = SecureGroup.GetListGroupsByPartner(iCurGroupId))
			{
				while (reader.Read())
				{
					int iGroupId = (int)reader["GroupId"];
					if (SecureGroup.IsPartner(iGroupId))
						VisiblePartnerGroups.Add(iGroupId);
					else
						VisibleUnpartnerGroups.Add(iGroupId);
				}
			}

			ClearUnpartnerGroups();
			BinddgGroupsUsers();
		}
		#endregion

		#region BinddgGroupsUsers()
		private void BinddgGroupsUsers()
		{
			dgUsers.Columns[2].HeaderText = LocRM.GetString("GroupUser");
			dgUsers.Columns[3].HeaderText = LocRM.GetString("Email");

			DataTable dt = new DataTable();
			dt.Columns.Add("ObjectId", typeof(int));
			dt.Columns.Add("Type");
			dt.Columns.Add("Title");
			dt.Columns.Add("Email");
			dt.Columns.Add("ActionView");
			DataRow dr;

			if (GroupID > 1) // [..]
			{
				bool retval = false;
				int iParentId = GroupID;
				while (iParentId > 1)
				{
					iParentId = SecureGroup.GetParentGroup(iParentId);
					if (VisibleUnpartnerGroups.Contains(iParentId) || VisiblePartnerGroups.Contains(iParentId) || iParentId == 6)
					{
						retval = true;
						break;
					}
				}
				if (retval)
					iParentId = SecureGroup.GetParentGroup(GroupID);
				else
					iParentId = 1;
				dr = dt.NewRow();
				dr["ObjectId"] = 0;
				dr["Type"] = "Group";
				dr["Title"] = "<span style='padding-left:15px'>&nbsp;</span><a href='../Directory/directory.aspx?Tab=0&amp;SGroupID=" + iParentId.ToString() + "'>[..]</a>";
				dt.Rows.Add(dr);
			}

			if ((GroupID != 1 && GroupID != 6) ||
				(GroupID == 1 && VisibleUnpartnerGroups.Contains(1)) ||
				(GroupID == 6 && VisibleUnpartnerGroups.Contains(6)))
			{
				using (IDataReader reader = SecureGroup.GetListChildGroups(GroupID))
				{
					while (reader.Read())
					{
						dr = dt.NewRow();
						dr["ObjectId"] = reader["GroupId"];
						dr["Type"] = "Group";
						int iGroupId = (int)reader["GroupId"];
						dr["Title"] = CommonHelper.GetGroupLink(iGroupId, CommonHelper.GetResFileString((string)reader["GroupName"]));
						dt.Rows.Add(dr);
					}
				}

				using (IDataReader reader = SecureGroup.GetListUsersInGroup(GroupID))
				{
					while (reader.Read())
					{

						dr = dt.NewRow();
						dr["ObjectId"] = reader["UserId"];

						dr["Type"] = "User";
						dr["Title"] = CommonHelper.GetUserStatus((int)reader["UserId"]);
						dr["Email"] = "<a href='mailto:" + (string)reader["Email"] + "'>" + (string)reader["Email"] + "</a>";
						dr["ActionView"] = "<a href='../Directory/UserView.aspx?UserID=" + reader["UserId"].ToString() + "'>"
							+ "<img alt='' src='../layouts/images/Icon-Search.GIF'/></a>";
						dt.Rows.Add(dr);
					}
				}
			}
			else if (GroupID == 1 && !VisibleUnpartnerGroups.Contains(1))
			{
				foreach (int iGroup in VisibleUnpartnerGroups)
				{
					dr = dt.NewRow();
					dr["ObjectId"] = iGroup;
					dr["Type"] = "Group";
					dr["Title"] = CommonHelper.GetGroupLink(iGroup);
					dt.Rows.Add(dr);
				}
				dr = dt.NewRow();
				dr["ObjectId"] = 6;
				dr["Type"] = "Group";
				dr["Title"] = CommonHelper.GetGroupLink(6);
				dt.Rows.Add(dr);
			}
			else if (GroupID == 6 && !VisibleUnpartnerGroups.Contains(6))
			{
				foreach (int iGroup in VisiblePartnerGroups)
				{
					dr = dt.NewRow();
					dr["ObjectId"] = iGroup;
					dr["Type"] = "Group";
					dr["Title"] = CommonHelper.GetGroupLink(iGroup);
					dt.Rows.Add(dr);
				}
			}

			dgUsers.DataSource = dt.DefaultView;
			dgUsers.DataBind();
		}
		#endregion

		#region ClearUnpartnerGroups
		private void ClearUnpartnerGroups()
		{
			ArrayList alDelete = new ArrayList();
			for (int i = 0; i < VisibleUnpartnerGroups.Count - 1; i++)
			{
				for (int j = i + 1; j < VisibleUnpartnerGroups.Count; j++)
				{
					int a1 = (int)VisibleUnpartnerGroups[i];
					int a2 = (int)VisibleUnpartnerGroups[j];
					if (IsChild(a1, a2) && !alDelete.Contains(a2))
						alDelete.Add(a2);
				}
			}
			foreach (int k in alDelete)
			{
				VisibleUnpartnerGroups.Remove(k);
			}
		}

		private bool IsChild(int ParentId, int ChildId)
		{
			bool retVal = false;
			int CurParent = ChildId;
			while (CurParent > 1)
			{
				CurParent = SecureGroup.GetParentGroup(CurParent);
				if (CurParent == ParentId)
				{
					retVal = true;
					break;
				}
			}

			return retVal;
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
