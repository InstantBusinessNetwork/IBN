namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	
	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for AddCalendar.
	/// </summary>
	public partial class AddCalendar : System.Web.UI.UserControl, IPageTemplateTitle
	{
		protected System.Web.UI.WebControls.Button btnAdd;
		protected System.Web.UI.WebControls.DropDownList ddGroups;
		protected System.Web.UI.WebControls.ListBox lbUsers;
		protected System.Web.UI.WebControls.DataGrid dgMembers;

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strCalendarList", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		#region CalendarID
		private int CalendarID
		{
			get
			{
				try
				{
					return int.Parse(Request["CalendarID"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region Copy
		private string Copy
		{
			get
			{
				return Request["Copy"];
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();
			if (!Page.IsPostBack)
				BindValues();
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
		}

		#region BindValues
		private void BindValues()
		{
			lstTimeZone.DataSource = User.GetListTimeZone();
			lstTimeZone.DataTextField = "DisplayName";
			lstTimeZone.DataValueField = "TimeZoneId";
			lstTimeZone.DataBind();

			string sTimeZoneId = Security.CurrentUser.TimeZoneId.ToString();
			int projectId = -1;
			if (CalendarID >= 0)
			{
				// CalendarId, ProjectId, ProjectName, CalendarName, TimeZoneId
				using (IDataReader rdr = Mediachase.IBN.Business.Calendar.GetCalendar(CalendarID))
				{
					if (rdr.Read())
					{
						if (Copy == null)
						{
							string title = (string)rdr["CalendarName"];
							tbTitle.Text = title;
						}
						if (rdr["TimeZoneId"] != DBNull.Value)
							sTimeZoneId = rdr["TimeZoneId"].ToString();
						if (rdr["ProjectId"] != DBNull.Value)
							projectId = (int)rdr["ProjectId"];
					}
				}
			}

			if (projectId > 0 && Project.GetIsMSProject(projectId))
				lstTimeZone.Enabled = false;

			ListItem lItem = lstTimeZone.Items.FindByValue(sTimeZoneId);
			if (lItem != null)
			{
				lstTimeZone.ClearSelection();
				lItem.Selected = true;
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			if (CalendarID < 0)
			{
				secHeader.Title = LocRM.GetString("Create");
				btnSave.Text = LocRM.GetString("Save");
			}
			else if (CalendarID >= 0 && Copy == null)
			{
				secHeader.Title = LocRM.GetString("EditCalendar");
				btnSave.Text = LocRM.GetString("SaveEdit");
			}
			else
			{
				secHeader.Title = LocRM.GetString("CopyC");
				btnSave.Text = LocRM.GetString("Copy");
			}
			btnCancel.Text = LocRM.GetString("Cancel");
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion


		#region Implementation of IPageTemplateTitle
		public string Modify(string oldValue)
		{
			LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strCalendarList", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
			if (CalendarID < 0)
				return LocRM.GetString("NewCalendar");
			else
				if (CalendarID >= 0 && Copy == null)
					return LocRM.GetString("EditCalendar");
				else
					return LocRM.GetString("CopyC");
		}
		#endregion

		#region btnCancel_ServerClick
		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Admin/CalendarList.aspx");
		}
		#endregion

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			if (Copy == null)
				Mediachase.IBN.Business.Calendar.Update(CalendarID, tbTitle.Text, int.Parse(lstTimeZone.SelectedItem.Value));
			else
				Mediachase.IBN.Business.Calendar.Create(tbTitle.Text, CalendarID, int.Parse(lstTimeZone.SelectedItem.Value));

			Response.Redirect("~/Admin/CalendarList.aspx");
		}
		#endregion
	}
}
