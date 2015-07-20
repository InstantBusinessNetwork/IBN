namespace Mediachase.UI.Web.Incidents.Modules
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
	using Mediachase.UI.Web.Util;
	using Mediachase.UI.Web.Modules;
	using System.Collections.Generic;
	using System.Globalization;

	/// <summary>
	///		Summary description for AdditionalInfo.
	/// </summary>
	public partial class AdditionalInfo : System.Web.UI.UserControl
	{

		public ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(AdditionalInfo).Assembly);
		public ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentEdit", typeof(AdditionalInfo).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.TimeTracking.Resources.strTimeTrackingInfo", typeof(AdditionalInfo).Assembly);

		#region IncidentId
		private int IncidentId
		{
			get
			{
				return CommonHelper.GetRequestInteger(Request, "IncidentId", 0);
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();
			if (!IsPostBack)
				BindValues();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("AdditionalInfo"));
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			if (IncidentId != 0)
			{
				int FolderId = -1;
				string sIdentifier = "";
				bool canViewFinances = Incident.CanViewFinances(IncidentId);
				bool canViewTimeTrackingInfo = Incident.CanViewTimeTrackingInfo(IncidentId);

				using (IDataReader reader = Incident.GetIncident(IncidentId))
				{
					if (reader.Read())
					{
						///  IncidentId, ProjectId, ProjectTitle, CreatorId, 
						///  Title, Description, Resolution, Workaround, CreationDate, 
						///  TypeId, TypeName, PriorityId, PriorityName, 
						///  SeverityId, SeverityName, IsEmail, MailSenderEmail, StateId, TaskTime, 
						///  IncidentBoxId, OrgUid, ContactUid, ClientName, ControllerId, 
						///  ResponsibleGroupState, TotalMinutes, TotalApproved

						lblCreator.Text = CommonHelper.GetUserStatus((int)reader["CreatorId"]);
						lblCreationDate.Text = ((DateTime)reader["CreationDate"]).ToShortDateString() + " " + ((DateTime)reader["CreationDate"]).ToShortTimeString();

						if (reader["Identifier"] != DBNull.Value)
							sIdentifier = reader["Identifier"].ToString();
						if (reader["IncidentBoxId"] != DBNull.Value)
							FolderId = (int)reader["IncidentBoxId"];
						lblClient.Text = Util.CommonHelper.GetClientLink(this.Page, reader["OrgUid"], reader["ContactUid"], reader["ClientName"]);
						lblTaskTime.Text = Util.CommonHelper.GetHours((int)reader["TaskTime"]);

						if (canViewTimeTrackingInfo)
						{
							SpentTimeLabel.Text = String.Format(CultureInfo.InvariantCulture,
								"{0} / {1}:",
								LocRM3.GetString("spentTime"),
								LocRM3.GetString("approvedTime"));

							lblSpentTime.Text = String.Format(CultureInfo.InvariantCulture,
								"{0} / {1}",
								Util.CommonHelper.GetHours((int)reader["TotalMinutes"]),
								Util.CommonHelper.GetHours((int)reader["TotalApproved"]));
						}
					}
				}

				List<string> sCategories = new List<string>();
				using (IDataReader reader = Incident.GetListCategories(IncidentId))
				{
					while (reader.Read())
					{
						sCategories.Add(reader["CategoryName"].ToString());
					}
				}
				string[] mas = sCategories.ToArray();
				if (mas.Length > 0)
				{
					lblGenCats.Text = String.Join(", ", mas);
					trGenCats.Visible = true;
				}
				else
					trGenCats.Visible = false;

				List<string> sIssCategories = new List<string>();
				using (IDataReader reader = Incident.GetListIncidentCategoriesByIncident(IncidentId))
				{
					while (reader.Read())
					{
						sIssCategories.Add(reader["CategoryName"].ToString());
					}
				}
				string[] mas1 = sIssCategories.ToArray();
				if (mas1.Length > 0)
				{
					lblIssCats.Text = String.Join(", ", mas1);
					trIssCats.Visible = true;
				}
				else
					trIssCats.Visible = false;

				if (FolderId > 0)
				{
					IncidentBox box = IncidentBox.Load(FolderId);
					IncidentBoxDocument settings = IncidentBoxDocument.Load(FolderId);
					if (Security.CurrentUser.IsExternal)
						lblFolder.Text = String.Format("{0}", box.Name);
					else if (Security.IsUserInGroup(InternalSecureGroups.Administrator))
						lblFolder.Text = String.Format("<a href='../Admin/EMailIssueBoxView.aspx?IssBoxId={1}'>{0}</a>", box.Name, box.IncidentBoxId);
					else
						lblFolder.Text = String.Format("<a href=\"javascript:OpenPopUpWindow(&quot;../Incidents/IncidentBoxView.aspx?IssBoxId={1}&IncidentId={2}&quot;,500,375)\">{0}</a>",
							box.Name, box.IncidentBoxId, IncidentId.ToString());
					lblManager.Text = CommonHelper.GetUserStatus(settings.GeneralBlock.Manager);
					if (sIdentifier == "")
						lblTicket.Text = TicketUidUtil.Create(box.IdentifierMask, IncidentId);
					else
						lblTicket.Text = sIdentifier;
				}

				trClient.Visible = PortalConfig.CommonIncidentAllowViewClientField;
				trGenCats.Visible = PortalConfig.CommonIncidentAllowViewGeneralCategoriesField;
				trIssCats.Visible = PortalConfig.IncidentAllowViewIncidentCategoriesField;
				trTaskTime.Visible = PortalConfig.CommonIncidentAllowViewTaskTimeField;
			}
		}
		#endregion
	}
}
