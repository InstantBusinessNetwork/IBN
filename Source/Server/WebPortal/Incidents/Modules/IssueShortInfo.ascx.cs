namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Resources;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.UI;

	/// <summary>
	///		Summary description for IssueShortInfo.
	/// </summary>
	public partial class IssueShortInfo : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(IssueShortInfo).Assembly);

		#region IncidentId
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
					throw new Exception("Ivalid Incident ID!!!");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!Page.IsPostBack)
				BindValues();
		}

		#region BindValues
		private void BindValues()
		{
			if (IncidentId != 0)
			{
				try
				{
					using (IDataReader reader = Incident.GetIncident(IncidentId))
					{
						if (reader.Read())
						{
							string sTitle = "";
							if (Configuration.ProjectManagementEnabled && reader["ProjectId"] != DBNull.Value)
							{
								string projectPostfix = CHelper.GetProjectNumPostfix((int)reader["ProjectId"], (string)reader["ProjectCode"]);
								if (Project.CanRead((int)reader["ProjectId"]))
									sTitle += String.Format(CultureInfo.InvariantCulture,
										"<a href='../Projects/ProjectView.aspx?ProjectId={0}' title='{1}'>{2}{3}</a> \\ ", 
										reader["ProjectId"].ToString(), 
										LocRM.GetString("Project"),
										reader["ProjectTitle"].ToString(),
										projectPostfix
										);
								else
									sTitle += String.Format(CultureInfo.InvariantCulture,
										"<span title='{0}'>{1}{2}</span> \\ ",
										LocRM.GetString("Project"), 
										reader["ProjectTitle"].ToString(), 
										LocRM.GetString("Project"));
							}
							sTitle += reader["Title"].ToString() + "&nbsp;(#" + reader["IncidentId"].ToString() + ")&nbsp;";
							string sIdentifier = "";
							if (reader["Identifier"] != DBNull.Value)
								sIdentifier = reader["Identifier"].ToString();
							if (reader["IncidentBoxId"] != DBNull.Value)
							{
								IncidentBox box = IncidentBox.Load((int)reader["IncidentBoxId"]);
								sTitle += "(" + ((sIdentifier == "") ? TicketUidUtil.Create(box.IdentifierMask, IncidentId) : sIdentifier) + ")";
							}
							lblTitle.Text = sTitle;
							lblType.Text = reader["TypeName"].ToString();
							lblSeverity.Text = reader["SeverityName"].ToString();

							lblState.ForeColor = Util.CommonHelper.GetStateColor((int)reader["StateId"]);
							lblState.Text = reader["StateName"].ToString();
							if ((bool)reader["IsOverdue"])
							{
								lblState.ForeColor = Util.CommonHelper.GetStateColor((int)ObjectStates.Overdue);
								lblState.Text = String.Format(CultureInfo.InvariantCulture,
									"{0}, {1}",
									reader["StateName"].ToString(),
									GetGlobalResourceObject("IbnFramework.Incident", "Overdue"));
							}

							lblPriority.Text = reader["PriorityName"].ToString() + " " + LocRM.GetString("Priority").ToLower();
							lblPriority.ForeColor = Util.CommonHelper.GetPriorityColor((int)reader["PriorityId"]);

							if (reader["Description"] != DBNull.Value)
							{
								string txt = CommonHelper.parsetext(reader["Description"].ToString(), false);
								if (PortalConfig.ShortInfoDescriptionLength > 0 && txt.Length > PortalConfig.ShortInfoDescriptionLength)
									txt = txt.Substring(0, PortalConfig.ShortInfoDescriptionLength) + "...";

								lblDescription.Text = txt;
							}

							//						lblExpRespDate.Text = ((DateTime)reader["ExpectedResponseDate"]).ToString("g");
							//						lblExpResDur.Text = ((DateTime)reader["ExpectedResolveDate"]).ToString("g");
						}
						else
							Response.Redirect("../Common/NotExistingID.aspx?IncidentID=1");
					}
				}
				catch (AccessDeniedException)
				{
					Response.Redirect("~/Common/NotExistingID.aspx?AD=1");
				}
				divType.Visible = lblType.Visible = PortalConfig.IncidentAllowViewTypeField;
				divSeverity.Visible = lblSeverity.Visible = PortalConfig.IncidentAllowViewSeverityField;
				lblPriority.Visible = PortalConfig.CommonIncidentAllowViewPriorityField;
				trAdd.Visible = divType.Visible || divSeverity.Visible || lblPriority.Visible;
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
		}
		#endregion
	}
}
