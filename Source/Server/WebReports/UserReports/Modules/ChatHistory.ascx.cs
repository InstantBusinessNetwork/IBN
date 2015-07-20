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
	///		Summary description for ChatHistory.
	/// </summary>
	/// 

	public partial  class ChatHistory : System.Web.UI.UserControl
	{

		//protected System.Web.UI.WebControls.TextBox txtDateFrom;
		//protected System.Web.UI.WebControls.RangeValidator txtDateFromRangeValidator;
		//protected System.Web.UI.WebControls.RequiredFieldValidator txtDateFromRFValidator;
		//protected System.Web.UI.WebControls.TextBox txtDateTo;
		//protected System.Web.UI.WebControls.RangeValidator txtDateToRangevalidator;
		//protected System.Web.UI.WebControls.RequiredFieldValidator txtDateToRFValidator;
		int iCurUserId = Mediachase.IBN.Business.Security.CurrentUser.UserID;
		bool IsAdmin;
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.Resources.strChatHistory", typeof(ChatHistory).Assembly);
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
			if (!IsPostBack)
			{
				BindValues();
				BindData();
				chkOrder.Checked = true;
			}
			//txtDateFromRangeValidator.ErrorMessage = LocRM.GetString("tWrongDate");
			//txtDateToRangevalidator.ErrorMessage = LocRM.GetString("tWrongDate");
		}

		#region Bind
		private void BindValues()
		{
			chkOrder.Text=LocRM.GetString("tLast");
			btnSubmit.Text=LocRM.GetString("tShow");
		}

		private void BindData()
		{
			dtcStartDate.SelectedDate = UserDateTime.Today.AddDays(-1);
			dtcEndDate.SelectedDate = UserDateTime.Now;

			BindChats();
		}

		private void BindChats()
		{
			using (IDataReader reader = UserReport.GetChats(IsAdmin?0:iCurUserId))
			{
				while (reader.Read())
				{
					lstChats.Items.Add(new ListItem(reader["name"].ToString(), reader["chat_id"].ToString()));
				}
			}
			if(lstChats.Items.Count <= 0)
			{
				lstChats.Style.Add("display","none");
				noConferences.Text = LocRM.GetString("NoConferences");
				noConferences.Visible = true;	
			}
			else
			{
				lstChats.Style.Add("display","inline");
				noConferences.Visible = false;	
			}

		}
		#endregion

		protected void btnSubmit_Click(object sender, System.EventArgs e)
		{
			IDataReader reader = UserReport.SearchChat(
				IsAdmin?0:iCurUserId,
				lstChats.Value != "" ? int.Parse(lstChats.Value):0,
				dtcStartDate.SelectedDate,
				dtcEndDate.SelectedDate.AddDays(1).AddSeconds(-1),
				txtKeyword.Text,
				chkOrder.Checked?1:0);
			MessRep.DataSource=reader;
			MessRep.DataBind();
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
	}


}
