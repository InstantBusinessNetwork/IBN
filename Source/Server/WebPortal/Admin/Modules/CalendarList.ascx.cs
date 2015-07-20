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
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for CalendarList1.
	/// </summary>
	public partial class calendarList : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strCalendarList", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
			BindToolbar();
			if (!IsPostBack)
				BindDG();
		}

		#region BindDG
		private void BindDG()
		{
			dgCalendar.Columns[1].HeaderText = LocRM.GetString("Title");
			dgCalendar.Columns[2].HeaderText = LocRM.GetString("Project");
			dgCalendar.Columns[3].HeaderText = LocRM.GetString("TimeZone");
			dgCalendar.DataSource = Mediachase.IBN.Business.Calendar.GetListCalendar();
			dgCalendar.DataBind();

			foreach (DataGridItem dgi in dgCalendar.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
				{
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");
					ib.ToolTip = LocRM.GetString("Delete");
				}
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tTitle");
			secHeader.AddLink("<img alt='' src='" + this.Page.ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("BusData"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin2"));
		}
		#endregion

		#region ShowTimeZone
		public string ShowTimeZone(int ZoneId)
		{
			return CommonHelper.GetStringTimeZoneOffset(ZoneId);
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
			this.dgCalendar.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_delete);

		}
		#endregion

		#region dg_delete
		private void dg_delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int CID = int.Parse(e.Item.Cells[0].Text);
			Mediachase.IBN.Business.Calendar.Delete(CID);
			BindDG();
		}
		#endregion

		#region btnRefresh_Click
		protected void btnRefresh_Click(object sender, System.EventArgs e)
		{
			BindDG();
		}
		#endregion
	}
}
