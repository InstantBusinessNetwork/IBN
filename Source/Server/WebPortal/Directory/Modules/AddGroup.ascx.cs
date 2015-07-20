using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.UI.WebControls;

using Mediachase.Ibn;
using Mediachase.Ibn.Clients;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Directory.Modules
{
	/// <summary>
	/// Summary description for AddGroup.
	/// </summary>
	public partial class AddGroup : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strAddGroup", Assembly.GetExecutingAssembly());
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", Assembly.GetExecutingAssembly());

		#region groupId
		private int groupId
		{
			get
			{
				try
				{
					return Request["GroupID"] != null ? int.Parse(Request["GroupID"]) : 1;
				}
				catch
				{
					return 1;
				}
			}
		}
		#endregion

		#region edit
		private string edit
		{
			get
			{
				return Request["Edit"];
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			BindToolBar();
			if (!IsPostBack)
			{
				BindDefaultValues();
				BindData();
			}
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "SaveGroups();DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			RequiredFieldValidator1.ErrorMessage = LocRM.GetString("Required");
			lblAvailable.Text = LocRM.GetString("Available");
			lblVisible.Text = LocRM.GetString("VisibleGroups");
			lblSelected.Text = LocRM.GetString("Visible");
			lblClient.Text = LocRM2.GetString("Client");
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			if (groupId == (int)InternalSecureGroups.Partner || SecureGroup.IsPartner(groupId))
			{
				btnAddOneGr.Attributes.Add("onclick", "MoveOne(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); return false;");
				btnAddAllGr.Attributes.Add("onclick", "MoveAll(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); return false;");
				btnRemoveOneGr.Attributes.Add("onclick", "MoveOne(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + "); return false;");
				btnRemoveAllGr.Attributes.Add("onclick", "MoveAll(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + "); return false;");

				lbAvailableGroups.Attributes.Add("ondblclick", "MoveOne(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); return false;");
				lbSelectedGroups.Attributes.Add("ondblclick", "MoveOne(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + "); return false;");

				using (IDataReader reader = SecureGroup.GetListGroupsWithParameters(true, true, true, true, true, true, true, true, false, true, true))
				{
					while (reader.Read())
					{
						lbAvailableGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(reader["GroupName"].ToString()), reader["GroupId"].ToString()));
					}
				}

				ListItem li = lbAvailableGroups.Items.FindByValue(groupId.ToString());
				if (li != null)
					lbAvailableGroups.Items.Remove(li);

				ClientControl.ObjectType = String.Empty;
				ClientControl.ObjectId = PrimaryKeyId.Empty;
			}
			else
			{
				trGroups.Visible = false;
				trClient.Visible = false;
			}

		}
		#endregion

		#region BindToolBar
		private void BindToolBar()
		{
			if (edit == "1")
				secHeader.Title = LocRM.GetString("EditGroup");
			else
			{
				using (IDataReader reader = SecureGroup.GetGroup(groupId))
				{
					if (reader.Read())
						secHeader.Title = LocRM.GetString("CreateGroup") + " '" + CommonHelper.GetResFileString((string)reader["GroupName"]) + "'";
				}
			}

			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
		}
		#endregion

		#region BindData
		private void BindData()
		{
			lblGroupTitle.Text = LocRM.GetString("GroupTitle");
			if (edit == "1")
			{
				string gTitle = "";
				PrimaryKeyId orgUid = PrimaryKeyId.Empty;
				PrimaryKeyId contactUid = PrimaryKeyId.Empty;
				using (IDataReader rdr = SecureGroup.GetGroup(groupId))
				{
					if (rdr.Read())
					{
						gTitle = (string)rdr["GroupName"];
						if (rdr["OrgUid"] != DBNull.Value)
							orgUid = PrimaryKeyId.Parse(rdr["OrgUid"].ToString());
						if (rdr["ContactUid"] != DBNull.Value)
							contactUid = PrimaryKeyId.Parse(rdr["ContactUid"].ToString());
					}
				}
				tbGroupTitle.Text = HttpUtility.HtmlDecode(gTitle);

				if (groupId == (int)InternalSecureGroups.Partner || SecureGroup.IsPartner(groupId))
				{
					using (IDataReader reader = SecureGroup.GetListGroupsByPartner(groupId))
					{
						while (reader.Read())
						{
							lbSelectedGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(reader["GroupName"].ToString()), reader["GroupId"].ToString()));
						}
					}

					for (int i = 0; i < lbSelectedGroups.Items.Count; i++)
					{
						if (lbAvailableGroups.Items.FindByValue(lbSelectedGroups.Items[i].Value) != null)
							lbAvailableGroups.Items.Remove(lbAvailableGroups.Items.FindByValue(lbSelectedGroups.Items[i].Value));
						iGroups.Value += lbSelectedGroups.Items[i].Value + ",";
					}

					if (orgUid != PrimaryKeyId.Empty)
					{
						ClientControl.ObjectType = OrganizationEntity.GetAssignedMetaClassName();
						ClientControl.ObjectId = orgUid;
					}
					else if (contactUid != PrimaryKeyId.Empty)
					{
						ClientControl.ObjectType = ContactEntity.GetAssignedMetaClassName();
						ClientControl.ObjectId = contactUid;
					}
					else
					{
						ClientControl.ObjectType = String.Empty;
						ClientControl.ObjectId = PrimaryKeyId.Empty;
					}
				}
			}
		}
		#endregion

		#region Save
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			string sGroups = iGroups.Value;
			ArrayList alGroups = new ArrayList();

			while (sGroups.Length > 0)
			{
				alGroups.Add(Int32.Parse(sGroups.Substring(0, sGroups.IndexOf(","))));
				sGroups = sGroups.Remove(0, sGroups.IndexOf(",") + 1);
			}

			// Client
			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName())
				orgUid = ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName())
				contactUid = ClientControl.ObjectId;

			if (edit == "1")
			{
				if (groupId == (int)InternalSecureGroups.Partner || SecureGroup.IsPartner(groupId))
				{
					SecureGroup.UpdatePartner(groupId, contactUid, orgUid, tbGroupTitle.Text, alGroups);
				}
				else
					SecureGroup.UpdateGroup(groupId, tbGroupTitle.Text);
				Response.Redirect("../Directory/Directory.aspx?Tab=0&SGroupID=" + groupId.ToString());
			}
			else
			{
				int newGroupId = 1;
				if (groupId == (int)InternalSecureGroups.Partner || SecureGroup.IsPartner(groupId))
				{
					byte[] groupLogo = null;

					FileInfo fi = new FileInfo(Server.MapPath(GlobalResourceManager.Strings["IMGroupLogoUrl"]));
					FileStream fs = fi.OpenRead();
					groupLogo = new byte[fi.Length];
					int nBytesRead = fs.Read(groupLogo, 0, (int)fi.Length);

					newGroupId = SecureGroup.CreatePartner(tbGroupTitle.Text, contactUid, orgUid, alGroups, groupLogo);
				}
				else
					newGroupId = SecureGroup.Create(groupId, tbGroupTitle.Text);
				Response.Redirect("../Directory/Directory.aspx?Tab=0");
			}
		}
		#endregion

		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			Response.Redirect("../Directory/Directory.aspx?Tab=0");
		}
	}
}
