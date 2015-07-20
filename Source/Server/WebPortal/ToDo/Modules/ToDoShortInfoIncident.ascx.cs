namespace Mediachase.UI.Web.ToDo.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Resources;
	using Mediachase.UI.Web.Modules;

	/// <summary>
	///		Summary description for ToDoGeneral.
	/// </summary>
	public partial class ToDoShortInfoIncident : System.Web.UI.UserControl
	{

		public ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(ToDoShortInfoIncident).Assembly);

		#region ToDoID
		private int ToDoID
		{
			get
			{
				try
				{
					return int.Parse(Request["ToDoId"]);
				}
				catch
				{
					throw new Exception("Invalid ToDO Id!!!");
				}
			}
		}
		#endregion


		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Configuration.HelpDeskEnabled)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			if (!IsPostBack)
			{
				BindValues();
			}
			BindToolBar();
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region BindToolbar
		private void BindToolBar()
		{
			tbView.AddText(LocRM.GetString("tbIncidentInfo"));

			if (ViewState["IncidentId"] != null && !Security.CurrentUser.IsExternal)
				tbView.AddRightLink("<img alt='' src='../Layouts/Images/icon-search.gif'/> " + LocRM.GetString("View"), "../Incidents/IncidentView.aspx?IncidentId=" + ViewState["IncidentId"].ToString());
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			using (IDataReader rdr = ToDo.GetToDo(ToDoID))
			{
				if (rdr.Read())
				{
					if (rdr["IncidentId"] != DBNull.Value)
						ViewState["IncidentId"] = (int)rdr["IncidentId"];
				}
			}

			if (ViewState["IncidentId"] != null)
			{
				int IncidentId = (int)ViewState["IncidentId"];
				using (IDataReader reader = Incident.GetIncident(IncidentId))
				{
					if (reader.Read())
					{
						///  IncidentId, ProjectId, ProjectTitle, CreatorId, 
						///  Title, Description, Resolution, Workaround, CreationDate, 
						///  TypeId, TypeName, PriorityId, PriorityName, 
						///  SeverityId, SeverityName, IsEmail, StateId

						bool IsExternal = Security.CurrentUser.IsExternal;
						if (!IsExternal)
							lblTitle.Text = String.Format("<a href='../Incidents/IncidentView.aspx?IncidentId={0}'>{1} (#{0})</a>", reader["IncidentId"].ToString(), reader["Title"].ToString());
						else
							lblTitle.Text = String.Format("{0} (#{1})", reader["Title"].ToString(), reader["IncidentId"].ToString());

						lblStatus.ForeColor = Util.CommonHelper.GetStateColor((int)reader["StateId"]);
						lblStatus.Text = reader["StateName"].ToString();

						lblPriority.Text = reader["PriorityName"].ToString() + " " + LocRM.GetString("Priority").ToLower();
						lblPriority.ForeColor = Util.CommonHelper.GetPriorityColor((int)reader["PriorityId"]);
						lblPriority.Visible = PortalConfig.CommonIncidentAllowViewPriorityField;

						if ((int)reader["TypeId"] > 1)
							lblType.Text = reader["TypeName"].ToString();
						lblType.Visible = PortalConfig.IncidentAllowViewTypeField;

						if ((int)reader["SeverityId"] > 1)
							lblSeverity.Text = reader["SeverityName"].ToString();
						lblSeverity.Visible = PortalConfig.IncidentAllowViewSeverityField;

						if (reader["Description"] != DBNull.Value)
						{
							string txt = CommonHelper.parsetext(reader["Description"].ToString(), false);
							if (PortalConfig.ShortInfoDescriptionLength > 0 && txt.Length > PortalConfig.ShortInfoDescriptionLength)
								txt = txt.Substring(0, PortalConfig.ShortInfoDescriptionLength) + "...";
							lblDescription.Text = txt;
						}
					}
				}
			}
		}
		#endregion
	}
}
