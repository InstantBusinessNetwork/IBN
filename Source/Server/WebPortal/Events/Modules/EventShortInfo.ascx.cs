namespace Mediachase.UI.Web.Events.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for EventGeneral.
	/// </summary>
	public partial class EventShortInfo : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strEventGeneral", typeof(EventShortInfo).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(EventShortInfo).Assembly);

		#region EventID
		private int EventID
		{
			get
			{
				try
				{
					return int.Parse(Request["EventID"]);
				}
				catch
				{
					throw new Exception("Invalid Event ID");
				}
			}
		}
		#endregion

		#region SharedID
		private int SharedID
		{
			get
			{
				return CommonHelper.GetRequestInteger(Request, "SharedID", -1);
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (SharedID > 0)
			{
				apShared.Visible = true;
				lblEntryOwner.Text = CommonHelper.GetUserStatus(SharedID);
			}
			else
				apShared.Visible = false;

			if (!IsPostBack)
			{
				BindData();
			}
		}

		#region BindData
		private void BindData()
		{
			///  EventId, ProjectId, ProjectTitle, CreatorId, ManagerId, 
			///  Title, Description, Location, CreationDate, StartDate, 
			///  FinishDate, PriorityId, PriorityName, TypeId, TypeName, ReminderInterval, HasRecurrence
			try
			{
				using (IDataReader rdr = CalendarEntry.GetEvent(EventID))
				{
					if (rdr.Read())
					{
						if (rdr["ProjectId"] != DBNull.Value)
						{
							if (Project.CanRead((int)rdr["ProjectId"]))
								lblTitle.Text = String.Format("<a href='../Projects/ProjectView.aspx?ProjectId={0}'>{1}</a>",
									rdr["ProjectId"].ToString(), rdr["ProjectTitle"].ToString());
							else
								lblTitle.Text = rdr["ProjectTitle"].ToString();

							lblTitle.Text += " &gt; ";
						}
						lblTitle.Text += rdr["Title"].ToString();

						if (rdr["Description"] != DBNull.Value)
						{
							string txt = CommonHelper.parsetext(rdr["Description"].ToString(), false);
							if (PortalConfig.ShortInfoDescriptionLength > 0 && txt.Length > PortalConfig.ShortInfoDescriptionLength)
								txt = txt.Substring(0, PortalConfig.ShortInfoDescriptionLength) + "...";
							lblDescription.Text = txt;
						}
						lblType.Text = rdr["TypeName"].ToString();

						lblPriority.Text = rdr["PriorityName"].ToString() + " " + LocRM.GetString("Priority").ToLower();
						lblPriority.ForeColor = Util.CommonHelper.GetPriorityColor((int)rdr["PriorityId"]);
						lblPriority.Visible = PortalConfig.CommonCEntryAllowViewPriorityField;

						lblState.ForeColor = Util.CommonHelper.GetStateColor((int)rdr["StateId"]);
						lblState.Text = rdr["StateName"].ToString();

						lblStartDate.Text = ((DateTime)rdr["StartDate"]).ToShortDateString() + " " + ((DateTime)rdr["StartDate"]).ToShortTimeString();
						lblEndDate.Text = ((DateTime)rdr["FinishDate"]).ToShortDateString() + " " + ((DateTime)rdr["FinishDate"]).ToShortTimeString();
						if ((int)rdr["HasRecurrence"] == 1)
						{
							TimeSpan Offset = new TimeSpan(0, -User.GetTimeZoneBias(Security.CurrentUser.TimeZoneId), 0);
							lblTimeZone.Text = "(GMT ";
							if (Offset.TotalMinutes != 0)
							{
								string str = "";
								str += (Offset.TotalMinutes > 0) ? "+" : "-";
								if (Math.Abs(Offset.Hours) < 10)
									str += "0";
								str += Math.Abs(Offset.Hours).ToString() + ":";
								if (Math.Abs(Offset.Minutes) < 10)
									str += "0";
								str += Math.Abs(Offset.Minutes).ToString();
								lblTimeZone.Text += str + ")";
							}
						}
					}
					else
						Response.Redirect("../Common/NotExistingID.aspx?EventID=1");
				}
			}
			catch (AccessDeniedException)
			{
				Response.Redirect("~/Common/NotExistingID.aspx?AD=1");
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
