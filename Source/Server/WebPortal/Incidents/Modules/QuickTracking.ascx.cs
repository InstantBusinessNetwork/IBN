namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Collections;
	using Mediachase.IBN.Business;

	/// <summary>
	///		Summary description for QuickTracking.
	/// </summary>
	public partial class QuickTracking : System.Web.UI.UserControl
	{
		public ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(QuickTracking).Assembly);
		private int IncidentId
		{
			get
			{
				try
				{
					return int.Parse(Request["IncidentId"]);
				}
				catch
				{
					throw new Exception("Not valid Incident ID!");
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			if (Incident.ShowAcceptDeny(IncidentId))
			{
				trAD.Visible = true;
				trCanwrite.Visible = false;
				trCanread.Visible = false;
			}
			else
				this.Visible = false;
			//			else// if (!Incident.CanUpdateStatus(IncidentId))
			//			{
			//				trCanwrite.Visible = false;
			//				trCanread.Visible = true;
			//				trAD.Visible = false;
			//			}
			//			else
			//			{
			//				trCanwrite.Visible = true;
			//				trCanread.Visible = false;
			//				trAD.Visible = false;
			//			}

			BindToolbar();
			//			btnSave.Text = LocRM.GetString("btnSave");
			btnAccept.Text = LocRM.GetString("Accept");
			btnDecline.Text = LocRM.GetString("Decline");
			if (!Page.IsPostBack)
			{
				BindValues();
			}
			// Put user code to initialize the page here
		}

		#region BindToolbar
		private void BindToolbar()
		{
			//tbQuickTracking.AddText(LocRM.GetString("tbQuickTracking"));
			//tbQuickTracking1.AddText(LocRM.GetString("tbQuickTracking"));
			tbQuickTracking2.AddText(LocRM.GetString("tbQuickTracking2"));
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			//States
			//			using (IDataReader reader = Mediachase.IBN.Business.Common.GetListObjectStates())
			//			{
			//				while (reader.Read())
			//				{
			//					if ((int)reader["StateId"] != (int)ObjectStates.Overdue)
			//						ddlStatus.Items.Add(new ListItem(reader["StateName"].ToString(), reader["StateId"].ToString()));
			//				}
			//			}

			//			using (IDataReader reader = Incident.GetIncident(IncidentId))
			//			{
			//				if(reader.Read())
			//				{
			//					///  IncidentId, ProjectId, ProjectTitle, CreatorId, 
			//					///  Title, Description, Resolution, Workaround, CreationDate, 
			//					///  TypeId, TypeName, PriorityId, PriorityName, 
			//					///  SeverityId, SeverityName
			//					if(reader["Resolution"] != DBNull.Value)
			//					{
			//						txtResolution.Text = reader["Resolution"].ToString();
			//						lblResolution.Text = reader["Resolution"].ToString();
			//					}
			//					if(reader["Workaround"] != DBNull.Value)
			//					{
			//						txtWorkaround.Text = reader["Workaround"].ToString();
			//						lblWorkaround.Text = reader["Workaround"].ToString();
			//					}
			//					ListItem lItem = ddlStatus.Items.FindByValue(reader["StatusId"].ToString());
			//					if (lItem != null)
			//					{
			//						if (lItem.Value !="1" && ddlStatus.Items[0].Value=="1")
			//							ddlStatus.Items.RemoveAt(0);
			//						ddlStatus.ClearSelection();
			//						lItem.Selected = true;
			//						lblStatus.Text = lItem.Text;
			//					}
			//				}
			//			}
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

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			int status = 0;
			if (ddlStatus.SelectedItem != null)
			{
				if (ddlStatus.SelectedItem.Value != "1" && ddlStatus.Items[0].Value == "1")
					ddlStatus.Items.RemoveAt(0);
				status = int.Parse(ddlStatus.SelectedItem.Value);
				//				Issue2.UpdateResolutionAndStatus(IncidentId,status,txtResolution.Text,txtWorkaround.Text);
				if (!Security.CurrentUser.IsExternal)
					Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../Incidents/IncidentView.aspx?IncidentId=" + IncidentId, Response);
				else
					Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../External/ExternalIncident.aspx?IncidentId=" + IncidentId, Response);
			}

		}

		protected void btnAccept_ServerClick(object sender, System.EventArgs e)
		{
			Issue2.AcceptResource(IncidentId);
			if (!Security.CurrentUser.IsExternal)
				Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../Incidents/IncidentView.aspx?IncidentId=" + IncidentId, Response);
			else
				Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../External/ExternalIncident.aspx?IncidentId=" + IncidentId, Response);
		}

		protected void btnDecline_ServerClick(object sender, System.EventArgs e)
		{
			Issue2.DeclineResource(IncidentId);
			if (!Security.CurrentUser.IsExternal)
				Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../Workspace/default.aspx?BTab=Workspace", Response);
			else
				Response.Redirect("~/External/MissingObject.aspx");
		}

		//===========================================================================
		// This public property was added by conversion wizard to allow the access of a protected, autogenerated member variable tbQuickTracking2.
		//===========================================================================
		public Mediachase.UI.Web.Modules.BlockHeaderLightWithMenu tbQuickTracking2
		{
			get { return Migrated_tbQuickTracking2; }
			//set { Migrated_tbQuickTracking2 = value; }
		}
		//===========================================================================
		// This public property was added by conversion wizard to allow the access of a protected, autogenerated member variable tbQuickTracking1.
		//===========================================================================
		public Mediachase.UI.Web.Modules.BlockHeaderLightWithMenu tbQuickTracking1
		{
			get { return Migrated_tbQuickTracking1; }
			//set { Migrated_tbQuickTracking1 = value; }
		}
		//===========================================================================
		// This public property was added by conversion wizard to allow the access of a protected, autogenerated member variable tbQuickTracking.
		//===========================================================================
		public Mediachase.UI.Web.Modules.BlockHeaderLightWithMenu tbQuickTracking
		{
			get { return Migrated_tbQuickTracking; }
			//set { Migrated_tbQuickTracking = value; }
		}
	}
}
