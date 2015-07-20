namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Text;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using System.Resources;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for AddActualItem.
	/// </summary>
	public partial class AddActualItem : System.Web.UI.UserControl
	{
		#region HTML Vars
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectFinances", typeof(AddActualItem).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		#region IDs
		private int ProjectId
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int TaskId
		{
			get
			{
				try
				{
					return int.Parse(Request["TaskId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int IncidentId
		{
			get
			{
				try
				{
					return int.Parse(Request["IncidentId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int DocumentId
		{
			get
			{
				try
				{
					return int.Parse(Request["DocumentId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int EventId
		{
			get
			{
				try
				{
					return int.Parse(Request["EventId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int ToDoId
		{
			get
			{
				try
				{
					return int.Parse(Request["ToDoId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int ParentProjectId = -1;
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			DefineParentProjectId();
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Text = LocRM.GetString("tSave");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnCancel.Text = LocRM.GetString("tCancel");
			secHeader.Title = LocRM.GetString("tNewActual");

			if (!Page.IsPostBack)
			{
				BindAccounts();
			}
		}

		#region DefineParentProjectId
		private void DefineParentProjectId()
		{
			if (ProjectId > 0)
				ParentProjectId = ProjectId;
			else if (TaskId > 0)
				ParentProjectId = Task.GetProject(TaskId);
			else if (IncidentId > 0)
				using (IDataReader reader = Incident.GetIncident(IncidentId))
				{
					if (reader.Read() && reader["ProjectId"] != DBNull.Value)
						ParentProjectId = (int)reader["ProjectId"];
				}
			else if (DocumentId > 0)
				using (IDataReader reader = Document.GetDocument(DocumentId))
				{
					if (reader.Read() && reader["ProjectId"] != DBNull.Value)
						ParentProjectId = (int)reader["ProjectId"];
				}
			else if (EventId > 0)
				using (IDataReader reader = CalendarEntry.GetEvent(EventId))
				{
					if (reader.Read() && reader["ProjectId"] != DBNull.Value)
						ParentProjectId = (int)reader["ProjectId"];
				}
			else if (ToDoId > 0)
				using (IDataReader reader = ToDo.GetToDo(ToDoId))
				{
					if (reader.Read() && reader["ProjectId"] != DBNull.Value)
						ParentProjectId = (int)reader["ProjectId"];
				}
		}
		#endregion

		#region BindAccounts
		private void BindAccounts()
		{
			DataTable dt = Finance.GetListAccountsDataTable(ParentProjectId);
			foreach (DataRow dr in dt.Rows)
			{
				if ((int)dr["OutlineLevel"] == 1)
				{
					dr["Title"] = LocRM.GetString("tRoot");
					break;
				}
			}
			ddAccounts.DataSource = dt.DefaultView;
			ddAccounts.DataTextField = "Title";
			ddAccounts.DataValueField = "AccountId";
			ddAccounts.DataBind();
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

		#region btnSave_click
		protected void btnSave_click(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			if (ProjectId > 0)
				Finance.AddActualFinancesForProject(int.Parse(ddAccounts.SelectedValue), ProjectId, txtDescription.Text, decimal.Parse(txtValue.Text));
			else if (TaskId > 0)
				Finance.AddActualFinancesForTask(int.Parse(ddAccounts.SelectedValue), TaskId, txtDescription.Text, decimal.Parse(txtValue.Text));
			else if (IncidentId > 0)
				Finance.AddActualFinancesForIncident(int.Parse(ddAccounts.SelectedValue), IncidentId, txtDescription.Text, decimal.Parse(txtValue.Text));
			else if (DocumentId > 0)
				Finance.AddActualFinancesForDocument(int.Parse(ddAccounts.SelectedValue), DocumentId, txtDescription.Text, decimal.Parse(txtValue.Text));
			else if (EventId > 0)
				Finance.AddActualFinancesForEvent(int.Parse(ddAccounts.SelectedValue), EventId, txtDescription.Text, decimal.Parse(txtValue.Text));
			else if (ToDoId > 0)
				Finance.AddActualFinancesForToDo(int.Parse(ddAccounts.SelectedValue), ToDoId, txtDescription.Text, decimal.Parse(txtValue.Text));
			CancelFunc();
		}

		protected void btnCancel_click(object sender, System.EventArgs e)
		{
			CancelFunc();
		}
		#endregion

		#region CancelFunc
		private void CancelFunc()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<script language=javascript>");
			sb.Append("window.opener.location.href=window.opener.location.href;");
			sb.Append("window.close();");
			sb.Append("</script>");
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), sb.ToString());
		}
		#endregion

	}
}
