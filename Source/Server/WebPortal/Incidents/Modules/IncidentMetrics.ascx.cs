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

	/// <summary>
	///		Summary description for IncidentMetrics.
	/// </summary>
	public partial class IncidentMetrics : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(IncidentMetrics).Assembly);

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

			secHeader.AddText(LocRM.GetString("tIncMetrics"));
		}

		#region BindValues
		private void BindValues()
		{
			using (IDataReader reader = Incident.GetIncident(IncidentId))
			{
				if (reader.Read())
				{
					int stateId = (int)reader["StateId"];
					DateTime userNow = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow);

					CreationDateLabel.Text = ((DateTime)reader["CreationDate"]).ToString("g");
					if (reader["ActualOpenDate"] != DBNull.Value)
					{
						ActualOpenDateLabel.Text = ((DateTime)reader["ActualOpenDate"]).ToString("g");
					}
					if (reader["ModifiedDate"] != DBNull.Value)
					{
						ModifiedDateLabel.Text = ((DateTime)reader["ModifiedDate"]).ToString("g");
					}
					if (reader["ExpectedResponseDate"] != DBNull.Value)
					{
						ExpResponseDateLabel.Text = ((DateTime)reader["ExpectedResponseDate"]).ToString("g");

						if ((stateId == (int)ObjectStates.Active || stateId == (int)ObjectStates.OnCheck || stateId == (int)ObjectStates.ReOpen || stateId == (int)ObjectStates.Upcoming)
							&& (bool)reader["IsNewMessage"] && (DateTime)reader["ExpectedResponseDate"] < userNow)
							ExpResponseDateLabel.CssClass = "ibn-error";
					}
					if (reader["ExpectedResolveDate"] != DBNull.Value)
					{
						ExpResolveDateLabel.Text = ((DateTime)reader["ExpectedResolveDate"]).ToString("g");

						if ((stateId == (int)ObjectStates.Active || stateId == (int)ObjectStates.OnCheck || stateId == (int)ObjectStates.ReOpen || stateId == (int)ObjectStates.Upcoming)
							&& (DateTime)reader["ExpectedResolveDate"] < userNow)
							ExpResolveDateLabel.CssClass = "ibn-error";
					}
					if (reader["ExpectedAssignDate"] != DBNull.Value)
					{
						ExpAssignDateLabel.Text = ((DateTime)reader["ExpectedAssignDate"]).ToString("g");

						if ((stateId == (int)ObjectStates.Active || stateId == (int)ObjectStates.ReOpen || stateId == (int)ObjectStates.Upcoming)
							&& (int)reader["ResponsibleId"] <= 0 && (DateTime)reader["ExpectedAssignDate"] < userNow)
							ExpAssignDateLabel.CssClass = "ibn-error";
					}
					if (reader["ActualFinishDate"] != DBNull.Value)
					{
						ActualFinishLabel.Text = ((DateTime)reader["ActualFinishDate"]).ToString("g");
						trFinish.Visible = true;
					}
					else
					{
						trFinish.Visible = false;
					}
				}
			}

			trExpAssign.Visible = PortalConfig.IncidentAllowViewExpAssDeadlineField;
			trExpReply.Visible = PortalConfig.IncidentAllowViewExpReplyDeadlineField;
			trExpResolution.Visible = PortalConfig.IncidentAllowViewExpResolutionDeadlineField;
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
