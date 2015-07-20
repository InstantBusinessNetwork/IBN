namespace Mediachase.UI.Web.UserReports.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Collections;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;

	//using Mediachase.IBN.Business;
	using Mediachase.IBN.DefaultUserReports;
	using Mediachase.UI.Web.UserReports.Util;

	/// <summary>
	///		Summary description for MessageHistory.
	/// </summary>
	/// 

	public partial  class MessageHistory : System.Web.UI.UserControl
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
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.Resources.strMessageHistory", typeof(MessageHistory).Assembly);
		string Type
		{
			get
			{
				try
				{
					return Request["Type"]!=null ? Request["Type"] : "";
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
			IsAdmin= (Request.QueryString["ReportType"] == "Admin");
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
			lstInOut.Items.Clear();
			lstInOut.Items.Add(new ListItem(LocRM.GetString("tInOut"),"1"));
			lstInOut.Items.Add(new ListItem(LocRM.GetString("tIn"),"2"));
			lstInOut.Items.Add(new ListItem(LocRM.GetString("tOut"),"3"));
			
			//lstGroup1.Items.Add(new ListItem(LocRM.GetString("tAny"),"0"));
			//lstGroup2.Items.Add(new ListItem(LocRM.GetString("tAny"),"0"));
			//lstContact.Items.Add(new ListItem(LocRM.GetString("tAny"),"0"));
			chkOrder.Text=LocRM.GetString("tLast");
			btnSubmit.Text=LocRM.GetString("tShow");
		}

		private void BindData()
		{
			//txtDateFrom.Text = UserDateTime.UserToday.ToShortDateString();
			dtcStartDate.SelectedDate = UserDateTime.Today.AddDays(-1);
			//txtDateTo.Text = UserDateTime.UserToday.ToShortDateString();
			dtcEndDate.SelectedDate = UserDateTime.Now;
			BindGroups();
		}

		private void BindGroups()
		{
			if(!IsAdmin)
			{
				GroupRow.Visible=false;
				lblUser.Visible=true;
				lstGroup1.Visible=false;
				lstUser.Visible=false;
				lblUser.Text=CommonHelper.GetUserStatus(iCurUserId);
			}
			
			using(IDataReader reader = UserReport.GetListGroupsAsTree(IsPartner))
			{
				while (reader.Read())
				{
					string GroupName = CommonHelper.GetResFileString(reader["GroupName"].ToString());
					string GroupId = reader["GroupId"].ToString();
					int Level = (int)reader["Level"];
					for (int i = 0; i < Level; i++)
						GroupName = "  " + GroupName;
					
					lstGroup2.Items.Add(new ListItem(GroupName, GroupId));
					if(IsAdmin)
					{
						ListItem liItem = new ListItem(GroupName, GroupId);
						lstGroup1.Items.Add(liItem);
						if(GroupId == "2") //Admins
						{
							lstGroup1.SelectedItem.Selected = false;
							liItem.Selected = true;
							BindUser(int.Parse(GroupId));
						}
					}
				}
			}

			BindContactUser(int.Parse(lstGroup2.SelectedValue));
		}

		private void BindUser(int GroupID)
		{
			lstUser.Items.Add(new ListItem(LocRM.GetString("tAny"),"0"));
			int i = 1;
			using (IDataReader reader = UserReport.GetListAllUsersInGroup(GroupID))
			{
				while (reader.Read())
				{
					string str = reader["LastName"].ToString() + ", " + reader["FirstName"].ToString();
					lstUser.Items.Add(new ListItem(str, reader["UserId"].ToString()));
					if(int.Parse(lstUser.Items[i].Value) == iCurUserId)
					{
						lstUser.Items[lstUser.SelectedIndex].Selected = false;
						lstUser.Items[i].Selected = true;
					}
					i++;
				}
			}
		}

		private void BindContactUser(int GroupID)
		{
			lstContact.Items.Add(new ListItem(LocRM.GetString("tAny"),"0"));
			using (IDataReader reader = UserReport.GetListAllUsersInGroup(GroupID))
			{
				while (reader.Read())
				{
					string str = reader["LastName"].ToString()+ ", " + reader["FirstName"].ToString();
					lstContact.Items.Add(new ListItem(str, reader["UserId"].ToString()));
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
			BindUser(int.Parse(lstGroup1.SelectedItem.Value));
		}

		protected void lstGroup2_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			lstContact.Items.Clear();
			BindContactUser(int.Parse(lstGroup2.SelectedItem.Value));
		}

		protected void btnSubmit_Click(object sender, System.EventArgs e)
		{
			int userGroup = 1;
			// Alex Fix
			if(IsAdmin)
				userGroup=int.Parse(lstGroup1.SelectedItem.Value);
			else if(IsPartner/*Security.IsUserInGroup(InternalSecureGroups.Partner)*/)
				userGroup = UserReport.GetGroupForPartnerUser(iCurUserId);
			DataTable dt = UserReport.SearchHistory(
					/*DateTime.Parse(txtDateFrom.Text)*/dtcStartDate.SelectedDate,
					/*DateTime.Parse(txtDateTo.Text).AddDays(1)*/dtcEndDate.SelectedDate.AddDays(1).AddSeconds(-1),
					IsAdmin?int.Parse(lstUser.Value):iCurUserId,
					userGroup,
					int.Parse(lstContact.Value),
					int.Parse(lstGroup2.SelectedItem.Value),
					int.Parse(lstInOut.Value),
					txtKeyword.Text,
					chkOrder.Checked);
			
			MessRep.DataSource=dt.DefaultView;
			MessRep.DataBind();
			//reader.Close();
		}
	}


}
