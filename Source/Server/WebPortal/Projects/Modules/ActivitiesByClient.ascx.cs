namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Data;
	using Mediachase.Ibn.Clients;

	/// <summary>
	///		Summary description for ActivitiesByClient.
	/// </summary>
	public partial class ActivitiesByClient : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ActivitiesByClient).Assembly);
		private UserLightPropertyCollection _pc = Security.CurrentUser.Properties;
		private Hashtable _hash = new Hashtable();

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
				BindDefaultValues();
			BindSavedValues();
			BindDG();

			secHeader.Title = LocRM.GetString("tActTrackPrjByPrjClient");
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (dgProjects.Items.Count == 0)
			{
				lblNoItems.Text = LocRM.GetString("tTheAreNoItems");
				spanLbl.Visible = true;
				dgProjects.Visible = false;
			}
			else
			{
				spanLbl.Visible = false;
				dgProjects.Visible = true;
				foreach (DataGridItem dgi in dgProjects.Items)
				{
					if (int.Parse(dgi.Cells[0].Text) <= 0)
					{
						string s1 = dgi.Cells[1].Text;
						string s2 = dgi.Cells[2].Text;
						string key = "";
						if (s1 != PrimaryKeyId.Empty.ToString())
							key = "contact_" + s1;
						else if (s2 != PrimaryKeyId.Empty.ToString())
							key = "org_" + s2;
						else
							key = "noclient";

						dgi.Attributes.Add("onclick", _hash[key].ToString());
					}
				}
			}
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			ddPrjGroup.DataSource = ProjectGroup.GetProjectGroups();
			ddPrjGroup.DataTextField = "Title";
			ddPrjGroup.DataValueField = "ProjectGroupId";
			ddPrjGroup.DataBind();
			ddPrjGroup.Items.Insert(0, new ListItem(LocRM.GetString("AllFem"), "0"));

			ddPrjPhase.DataSource = Project.GetListProjectPhases();
			ddPrjPhase.DataTextField = "PhaseName";
			ddPrjPhase.DataValueField = "PhaseId";
			ddPrjPhase.DataBind();
			ddPrjPhase.Items.Insert(0, new ListItem(LocRM.GetString("AllFem"), "0"));

			ddStatus.DataSource = Project.GetListProjectStatus();
			ddStatus.DataTextField = "StatusName";
			ddStatus.DataValueField = "StatusId";
			ddStatus.DataBind();
			ddStatus.Items.Insert(0, new ListItem(LocRM.GetString("All"), "0"));

			//Client
			ClientControl.ObjectType = String.Empty;
			ClientControl.ObjectId = PrimaryKeyId.Empty;

			btnApplyFilter.Text = LocRM.GetString("Apply");
			btnResetFilter.Text = LocRM.GetString("Reset");
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			if (_pc["ActivitiesTracking_PrjGrp"] == null)
				_pc["ActivitiesTracking_PrjGrp"] = "0";
			else
			{
				ddPrjGroup.ClearSelection();
				CommonHelper.SafeSelect(ddPrjGroup, _pc["ActivitiesTracking_PrjGrp"].ToString());
			}
			if (_pc["ActivitiesTracking_PrjPhase"] == null)
				_pc["ActivitiesTracking_PrjPhase"] = "0";
			else
			{
				ddPrjPhase.ClearSelection();
				CommonHelper.SafeSelect(ddPrjPhase, _pc["ActivitiesTracking_PrjPhase"].ToString());
			}
			if (_pc["ActivitiesTracking_Status"] == null)
				_pc["ActivitiesTracking_Status"] = "0";
			else
			{
				ddStatus.ClearSelection();
				CommonHelper.SafeSelect(ddStatus, _pc["ActivitiesTracking_Status"].ToString());
			}

			//Client
			if (_pc["ActivitiesTracking_ClientNew"] != null && (!Page.IsPostBack || ClientControl.ObjectId == PrimaryKeyId.Empty))
			{
				string ss = _pc["ActivitiesTracking_ClientNew"];
				if (ss.IndexOf("_") >= 0)
				{
					string stype = ss.Substring(0, ss.IndexOf("_"));
					if (stype.ToLower() == "org")
					{
						ClientControl.ObjectType = OrganizationEntity.GetAssignedMetaClassName();
						ClientControl.ObjectId = PrimaryKeyId.Parse(ss.Substring(ss.IndexOf("_") + 1));
					}
					else if (stype.ToLower() == "contact")
					{
						ClientControl.ObjectType = ContactEntity.GetAssignedMetaClassName();
						ClientControl.ObjectId = PrimaryKeyId.Parse(ss.Substring(ss.IndexOf("_") + 1));
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

		#region SaveValues
		private void SaveValues()
		{
			_pc["ActivitiesTracking_PrjGrp"] = ddPrjGroup.SelectedValue;
			_pc["ActivitiesTracking_PrjPhase"] = ddPrjPhase.SelectedValue;
			_pc["ActivitiesTracking_Status"] = ddStatus.SelectedValue;
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				_pc["ActivitiesTracking_ClientNew"] = "org_" + ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName() && ClientControl.ObjectId != PrimaryKeyId.Empty)
				_pc["ActivitiesTracking_ClientNew"] = "contact_" + ClientControl.ObjectId;
			else
				_pc["ActivitiesTracking_ClientNew"] = "_";
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			_hash.Clear();

			int i = 3;
			dgProjects.Columns[i++].HeaderText = LocRM.GetString("Title");
			dgProjects.Columns[i++].HeaderText = LocRM.GetString("Status");
			dgProjects.Columns[i++].HeaderText = LocRM.GetString("tOpenTasks");
			dgProjects.Columns[i++].HeaderText = LocRM.GetString("tCompletedTasks");
			dgProjects.Columns[i++].HeaderText = LocRM.GetString("tIssues");

			int portfolioId = int.Parse(_pc["ActivitiesTracking_PrjGrp"].ToString());
			int phaseId = int.Parse(_pc["ActivitiesTracking_PrjPhase"].ToString());
			int statusId = int.Parse(_pc["ActivitiesTracking_Status"].ToString());

			// Client
			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			if (_pc["ActivitiesTracking_ClientNew"] != null)
			{
				string ss = _pc["ActivitiesTracking_ClientNew"];
				string stype = ss.Substring(0, ss.IndexOf("_"));
				if (stype.ToLower() == "org")
				{
					orgUid = PrimaryKeyId.Parse(ss.Substring(ss.IndexOf("_") + 1));
				}
				else if (stype.ToLower() == "contact")
				{
					contactUid = PrimaryKeyId.Parse(ss.Substring(ss.IndexOf("_") + 1));
				}
			}

			DataTable dt = Project.GetListProjectsGroupedByClient(portfolioId, phaseId,
				statusId, 0, orgUid, contactUid);
			
			dgProjects.DataSource = dt.DefaultView;
			dgProjects.DataBind();
			if (!Configuration.HelpDeskEnabled)
				dgProjects.Columns[7].Visible = false;
		}
		#endregion

		#region Protected DG Strings
		protected string GetTitle(bool isClient, int prjId, PrimaryKeyId contactUid, PrimaryKeyId orgUid,
			string clientName, bool isCollapsed)
		{
			if (isClient)
			{
				string client = "";
				string key = "";
				if (contactUid != PrimaryKeyId.Empty)
				{
					client = CommonHelper.GetContactLink(this.Page, contactUid, clientName);
					key = "contact_" + contactUid.ToString();
				}
				else if (orgUid != PrimaryKeyId.Empty)
				{
					client = CommonHelper.GetOrganizationLink(this.Page, orgUid, clientName);
					key = "org_" + orgUid.ToString();
				}
				else
				{
					client = "<span style='width:4px'>&nbsp;</span><font color='#003399'>" + LocRM.GetString("tNoClient") + "</font>";
					key = "noclient";
				}

				_hash.Add(key, "CollapseExpand(" + (isCollapsed ? "1" : "0") + ",'" + contactUid.ToString() + "','" + orgUid.ToString() + "', event)");
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='text'>" +
					"<b>" + "&nbsp;" + GetIcon(!isCollapsed) + "&nbsp;&nbsp;" + client + "</b>" +
					"</td></tr></table>";
			}
			else
			{
				return "<span class='text' style='padding-left:15px'>" + CommonHelper.GetProjectStatusWithId(prjId) + "</span>";
			}
		}

		protected string GetIcon(bool isExpand)
		{
			if (isExpand)
				return "<img class='mousepointer' border=0 src='" + ResolveUrl("~/Layouts/images/minus.gif") + "'>";
			else
				return "<img class='mousepointer' border=0 src='" + ResolveUrl("~/Layouts/images/plus.gif") + "'>";
		}

		protected string GetStatus(bool isHeader, string statusName)
		{
			if (isHeader)
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='boldtext'>" +
					"</td></tr></table>";
			}
			else
			{
				return "<span class='text' style='padding-left:5px'>" + statusName + "</span>";
			}
		}

		protected string GetOpenTasks(bool isHeader, int openTasksCount)
		{
			if (isHeader)
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='text'>" +
					"</td></tr></table>";
			}
			else
			{
				return "<span class='text' style='padding-left:5px'>" + openTasksCount + "</span>";
			}
		}

		protected string GetCompletedTasks(bool isHeader, int totalCount)
		{
			if (isHeader)
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='text'>" +
					"</td></tr></table>";
			}
			else
			{
				return "<span class='text' style='padding-left:5px'>" + totalCount + "</span>";
			}
		}

		protected string GetIssues(bool isHeader, int issCount)
		{
			if (isHeader)
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='boldtext'>" +
					"</td></tr></table>";
			}
			else
			{
				return "<span class='text' style='padding-left:5px'>" + issCount + "</span>";
			}
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

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lbCollapseExpand.Click += new EventHandler(lbCollapseExpand_Click);
			this.btnApplyFilter.Click += new EventHandler(btnApplyFilter_Click);
			this.btnResetFilter.Click += new EventHandler(btnResetFilter_Click);
		}
		#endregion

		#region Collapse_Expand
		private void lbCollapseExpand_Click(object sender, EventArgs e)
		{
			string sType = hdnColType.Value;
			string ceType = hdnCollapseExpand.Value;
			if (sType.ToLower() == "contact")
			{
				PrimaryKeyId contactUid = PrimaryKeyId.Parse(hdnId.Value);
				if (ceType == "0")
					Project.CollapseByClient(contactUid, PrimaryKeyId.Empty);
				else
					Project.ExpandByClient(contactUid, PrimaryKeyId.Empty);
			}
			else if (sType.ToLower() == "org")
			{
				PrimaryKeyId orgUid = PrimaryKeyId.Parse(hdnId.Value);
				if (ceType == "0")
					Project.CollapseByClient(PrimaryKeyId.Empty, orgUid);
				else
					Project.ExpandByClient(PrimaryKeyId.Empty, orgUid);
			}
			else if (sType.ToLower() == "noclient")
			{
				if (ceType == "0")
					Project.CollapseByClient(PrimaryKeyId.Empty, PrimaryKeyId.Empty);
				else
					Project.ExpandByClient(PrimaryKeyId.Empty, PrimaryKeyId.Empty);
			}
			hdnColType.Value = "";
			hdnId.Value = "";
			BindDG();
		}
		#endregion

		#region Apply-Reset
		private void btnApplyFilter_Click(object sender, EventArgs e)
		{
			SaveValues();
			BindDG();
		}

		private void btnResetFilter_Click(object sender, EventArgs e)
		{
			_pc["ActivitiesTracking_PrjGrp"] = "0";
			_pc["ActivitiesTracking_PrjPhase"] = "0";
			_pc["ActivitiesTracking_Status"] = "0";
			_pc["ActivitiesTracking_ClientNew"] = "_";
			BindDefaultValues();
			BindDG();
		} 
		#endregion
	}
}
