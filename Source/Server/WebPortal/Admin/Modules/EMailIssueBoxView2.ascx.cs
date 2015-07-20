namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using System.Reflection;

	/// <summary>
	///		Summary description for EMailIssueBoxView2.
	/// </summary>
	public partial class EMailIssueBoxView2 : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		#region IssBoxId
		protected int IssBoxId
		{
			get
			{
				if(Request["IssBoxId"]!=null)
					return int.Parse(Request["IssBoxId"]);
				else
					return -1;
			}
		}
		#endregion

		#region IncidentId
		protected int IncidentId
		{
			get
			{
				if(Request["IncidentId"]!=null)
					return int.Parse(Request["IncidentId"]);
				else
					return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			if(!Page.IsPostBack)
				BindValues();
			
			BindToolbar();
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			IncidentBox ib = IncidentBox.Load(IssBoxId);
			lblIssBoxName.Text = ib.Name;

			IncidentBoxDocument ibd = IncidentBoxDocument.Load(IssBoxId);
//
//			//EMailRouterBlock
//			EMailRouterIncidentBoxBlock eribb = ibd.EMailRouterBlock;
//
//			cbAllowEMail.Checked = eribb.AllowEMailRouting;
//
//			Util.CommonHelper.SafeSelect(ddExtActionType, ((int)eribb.IncomingEMailAction).ToString());
//			Util.CommonHelper.SafeSelect(ddIntActionType, ((int)eribb.OutgoingEMailAction).ToString());
//			
//			BindReipients(eribb);
//
			//GeneralBlock
			GeneralIncidentBoxBlock gibb = ibd.GeneralBlock;
						
			lblManager.Text = Util.CommonHelper.GetUserStatusUL((int)gibb.Manager);

			trControl.Visible = gibb.AllowControl;
			if(gibb.AllowControl)
			{
				if(gibb.ControllerAssignType == ControllerAssignType.CustomUser)
					lblController.Text = Util.CommonHelper.GetUserStatusUL((int)gibb.Controller);
				else
				{
					if(gibb.ControllerAssignType == ControllerAssignType.Manager)
						lblController.Text = Util.CommonHelper.GetUserStatusUL((int)gibb.Manager);
					else if(gibb.ControllerAssignType == ControllerAssignType.Creator)
						using(IDataReader reader = Incident.GetIncident(IncidentId))
						{
							if(reader.Read())
								lblController.Text = Util.CommonHelper.GetUserStatusUL((int)reader["CreatorId"]);
						}
				}
			}

			lblExpDuration.Text = Util.CommonHelper.GetHours(gibb.ExpectedDuration);
			lblExpRespTime.Text = Util.CommonHelper.GetHours(gibb.ExpectedResponseTime);
			
			if(gibb.ResponsibleAssignType == ResponsibleAssignType.CustomUser)
				lblResponsible.Text = Util.CommonHelper.GetUserStatusUL((int)gibb.Responsible);
			else
			{
				trResponsible.Visible = false;
				using(IDataReader reader = Incident.GetIncidentTrackingState(IncidentId))
				{
					if(reader.Read() && reader["ResponsibleId"]!=DBNull.Value && (int)reader["ResponsibleId"]>0)
					{
						trResponsible.Visible = true;
						lblResponsible.Text = Util.CommonHelper.GetUserStatusUL((int)reader["ResponsibleId"]);
					}
				}
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tIssBoxView");
			if(Security.IsUserInGroup(InternalSecureGroups.Administrator))
			{
				secHeader.AddLink(String.Format("<img alt='' src='{0}'/> {1}",
						this.Page.ResolveClientUrl("~/Layouts/Images/edit.gif"), LocRM.GetString("tEdit")), 
					String.Format("javascript:{{window.opener.location.href='{0}?IssBoxId={1}';window.close();}}", this.Page.ResolveUrl("~/Admin/EMailIssueBoxView.aspx"), IssBoxId));
			}
			secHeader.AddLink(String.Format("<img alt='' src='{0}'/> {1}",
					this.Page.ResolveClientUrl("~/Layouts/Images/cancel.gif"), LocRM.GetString("tClose")), 
				"javascript:window.close();");
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
		}
		#endregion
	}
}
