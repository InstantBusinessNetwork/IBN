namespace Mediachase.UI.Web.UserReports.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	//using Mediachase.IBN.Business;
	using Mediachase.UI.Web.UserReports.Util;
	using Mediachase.IBN.DefaultUserReports;

	/// <summary>
	///		Summary description for AlertsHistory.
	/// </summary>
	/// 

	public partial class AlertsHistory : System.Web.UI.UserControl
	{
		//protected System.Web.UI.WebControls.TextBox txtDateFrom;
		//protected System.Web.UI.WebControls.RangeValidator txtDateFromRangeValidator;
		//protected System.Web.UI.WebControls.RequiredFieldValidator txtDateFromRFValidator;
		//protected System.Web.UI.WebControls.TextBox txtDateTo;
		//protected System.Web.UI.WebControls.RangeValidator txtDateToRangevalidator;
		//protected System.Web.UI.WebControls.RequiredFieldValidator txtDateToRFValidator;
		int iCurUserId = Mediachase.IBN.Business.Security.CurrentUser.UserID;
		bool IsAdmin;
		bool IsPartner;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.Resources.strAlertsHistory", typeof(AlertsHistory).Assembly);
		string Type
		{
			get
			{
				try
				{
					return Request["Type"] != null ? Request["Type"] : "";
				}
				catch
				{
					return "";
				}
			}
		}
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Alex Fix
			//IsAdmin = Type == "Usr" ? false : Security.IsUserInGroup(InternalSecureGroups.Administrator);
			IsAdmin = (Request.QueryString["ReportType"] == "Admin");
			IsPartner = (Request.QueryString["ReportType"] == "Partner");
			if (!IsPostBack)
			{
				BindLists();
				BindData();
				chkOrder.Checked = true;
			}
		}

		#region Bind
		private void BindLists()
		{
			btnSubmit.Text = LocRM.GetString("tShow");
		}

		private void BindData()
		{
			//txtDateFrom.Text = "";
			//txtDateTo.Text = UserDateTime.UserNow.ToShortDateString();
			dtcEndDate.SelectedDate = UserDateTime.Now;
			//txtDateFrom.Text = UserDateTime.UserNow.AddDays(-1).ToShortDateString();
			dtcStartDate.SelectedDate = UserDateTime.Today.AddDays(-1);
			chkOrder.Text = LocRM.GetString("tLast");

			BindGroups();
		}

		private void BindGroups()
		{
			if (!IsAdmin)
			{
				GroupRow.Visible = false;
				lblUser.Visible = true;
				lstGroup1.Visible = false;
				lstUser.Visible = false;
				lblUser.Text = CommonHelper.GetUserStatus(iCurUserId);
			}
			using (IDataReader reader = UserReport.GetListGroupsAsTree(IsPartner))
			{
				while (reader.Read())
				{
					if (IsAdmin)
					{
						string GroupName = CommonHelper.GetResFileString(reader["GroupName"].ToString());
						string GroupId = reader["GroupId"].ToString();
						int Level = (int)reader["Level"];
						for (int i = 0; i < Level; i++)
							GroupName = "  " + GroupName;
						ListItem liItem = new ListItem(GroupName, GroupId);
						lstGroup1.Items.Add(liItem);
						if (GroupId == "2") //Admins
						{
							lstGroup1.SelectedItem.Selected = false;
							liItem.Selected = true;
							BindUser(int.Parse(GroupId));
						}
					}
				}
			}
		}

		private void BindUser(int GroupID)
		{
			int i = 0;
			using (IDataReader reader = UserReport.GetListAllUsersInGroup(GroupID))
			{
				while (reader.Read())
				{
					string str = reader["LastName"].ToString() + ", " + reader["FirstName"].ToString();
					lstUser.Items.Add(new ListItem(str, reader["UserId"].ToString()));
					if (int.Parse(lstUser.Items[i].Value) == iCurUserId)
					{
						lstUser.Items[lstUser.SelectedIndex].Selected = false;
						lstUser.Items[i].Selected = true;
					}
					i++;
				}
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
		protected void lstGroup1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			lstUser.Items.Clear();
			BindUser(int.Parse(lstGroup1.SelectedValue));
			if (lstUser.Items.Count == 0)
				lstUser.Items.Add(new ListItem(LocRM.GetString("tAny"), "0"));
		}

		protected void btnSubmit_Click(object sender, System.EventArgs e)
		{
			//DateTime dtFrom = txtDateFrom.Text != "" ? DateTime.Parse(txtDateFrom.Text): DateTime.MinValue;
			DateTime dtFrom = DateTime.MinValue;
			//if(dtcStartDate.Date.Text != "") dtFrom = dtcStartDate.Value;
			dtFrom = dtcStartDate.SelectedDate;
			//DateTime dtTo = txtDateTo.Text != "" ? DateTime.Parse(txtDateTo.Text): DateTime.MinValue;
			DateTime dtTo = DateTime.MinValue;
			dtTo = dtcEndDate.SelectedDate;
			DataView dv = UserReport.GetAlertsHistory(IsAdmin ? int.Parse(lstUser.Value) : iCurUserId, dtFrom, dtTo.AddDays(1).AddSeconds(-1), chkOrder.Checked);

			MessRep.DataSource = dv;
			MessRep.DataBind();
			//reader.Close();
		}
	}
}
