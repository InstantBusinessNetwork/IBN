namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;

	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.UI.Web.Modules;

	/// <summary>
	///		Summary description for IncidentGeneralInfo.
	/// </summary>
	public partial class IncidentGeneralInfo : System.Web.UI.UserControl
	{

    public ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(IncidentGeneralInfo).Assembly);
    public ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidents", typeof(IncidentGeneralInfo).Assembly);

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
			secHeader.AddText(LocRM2.GetString("QuickInfo"));

			if (Incident.CanUpdate(IncidentId))
				secHeader.AddRightLink(
					String.Format("<img src='../Layouts/Images/Edit.gif' width='16' height='16' border=0 align=absmiddle> {0}", LocRM.GetString("tbIncidentInfoEdit")), 
					String.Format("javascript:ShowWizard('EditGeneralInfo.aspx?IncidentId={0}', 500, 400);", IncidentId));
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			if(IncidentId != 0)
				try
				{
					using (IDataReader reader = Incident.GetIncident(IncidentId))
					{
						if (reader.Read())
						{
							///  IncidentId, ProjectId, ProjectTitle, CreatorId, 
							///  Title, Description, Resolution, Workaround, CreationDate, 
							///  TypeId, TypeName, PriorityId, PriorityName, 
							///  SeverityId, SeverityName
	
							lblTitle.Text = (string)reader["Title"];
							lblType.Text = reader["TypeName"].ToString();
							lblSeverity.Text = reader["SeverityName"].ToString();
							lblPriority.Text = reader["PriorityName"].ToString();
							if(reader["Description"] != DBNull.Value)
								lblDescription.Text = CommonHelper.parsetext(reader["Description"].ToString(),false);
						}
						else
							Response.Redirect("../Common/NotExistingID.aspx?IncidentID=1");
					}
				}
				catch (AccessDeniedException)
				{
					Response.Redirect("~/Common/NotExistingID.aspx?AD=1");
				}
		}
		#endregion
	}
}
