namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Globalization;

	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Web.UI;
	using System.Reflection;

	/// <summary>
	///		Summary description for WorkTimeSetup.
	/// </summary>
	public partial class WorkTimeSetup : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnSave.CustomImage = ResolveUrl("~/layouts/Images/SaveItem.gif");
			btnCancel.CustomImage = ResolveUrl("~/layouts/Images/cancel.gif");
			btnCancel.IsDecline = true;
			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
			BindToolbar();

			if (!Page.IsPostBack)
			{
				startTime.SelectedDate = DateTime.Parse(PortalConfig.WorkTimeStart, CultureInfo.InvariantCulture);
				endTime.SelectedDate = DateTime.Parse(PortalConfig.WorkTimeFinish, CultureInfo.InvariantCulture);

				BindCalendars();
				
			}
		}

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("WorkTimeSetup");
		} 
		#endregion

		#region BindCalendars
		private void BindCalendars()
		{
			using (IDataReader reader = Mediachase.IBN.Business.Calendar.GetListCalendar())
			{
				while (reader.Read())
				{
					if (reader["ProjectId"] == DBNull.Value)
						DefaultCalendarList.Items.Add(new ListItem(reader["CalendarName"].ToString(), reader["CalendarId"].ToString()));
				}
			}

			CHelper.SafeSelect(DefaultCalendarList, PortalConfig.DefaultCalendar.ToString());
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
			btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
			btnCancel.ServerClick += new EventHandler(btnCancel_ServerClick);
		}
		#endregion

		#region btnCancel_ServerClick
		void btnCancel_ServerClick(object sender, EventArgs e)
		{
			Response.Redirect("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin2");
		} 
		#endregion

		#region btnSave_ServerClick
		void btnSave_ServerClick(object sender, EventArgs e)
		{
			string start = startTime.SelectedDate.ToString("t", CultureInfo.InvariantCulture);
			PortalConfig.WorkTimeStart = start;
			string finish = endTime.SelectedDate.ToString("t", CultureInfo.InvariantCulture);
			PortalConfig.WorkTimeFinish = finish;
			PortalConfig.DefaultCalendar = int.Parse(DefaultCalendarList.SelectedValue);

			Response.Redirect("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin2");
		} 
		#endregion
	}
}
