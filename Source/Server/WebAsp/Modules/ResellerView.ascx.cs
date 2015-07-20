using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for ResellerView.
	/// </summary>
	public partial  class ResellerView : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label tbTitle;
		protected System.Web.UI.WebControls.Label tbContactName;
		protected System.Web.UI.WebControls.Label tbContactEmail;
		protected int ResellerId
		{
			get
			{
				try
				{
					return Request["ResellerId"] != "" ? int.Parse(Request["ResellerId"]) : 0;
				}
				catch
				{
					return 0;
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();
			if(!Page.IsPostBack && ResellerId != 0)
			{
				BindValues();
			}
			// Put user code to initialize the page here
		}

		private void BindToolbar()
		{
			secH.AddLink("<img alt='' src='../Layouts/Images/edit.gif'/> Edit Reseller","../Pages/ResellerEdit.aspx?ResellerId=" + ResellerId);
			secH.AddSeparator();
			secH.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> Back to the List","../Pages/Resellers.aspx");
		}

		private void BindValues()
		{
			using(IDataReader reader = CManage.ResellerGet(ResellerId))
			{
				reader.Read();
				lbTitle.Text = reader["Title"].ToString();
				lbContactName.Text = reader["ContactName"].ToString();
				lbContactEmail.Text = reader["ContactEmail"].ToString();
				lbContactPhone.Text = reader["ContactPhone"].ToString();
				lblGuid.Text = reader["Guid"].ToString();
				lblPercentage.Text = reader["CommissionPercentage"].ToString()+"%";
			}
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
