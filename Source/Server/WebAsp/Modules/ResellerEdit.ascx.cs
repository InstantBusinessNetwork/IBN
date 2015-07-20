using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for ResellerEdit.
	/// </summary>
	public partial  class ResellerEdit : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.TextBox tdTitle;
		protected int ResellerId = 0;


		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				ResellerId = Request["ResellerId"] != "" ? int.Parse(Request["ResellerId"]) : 0;
			}
			catch
			{
				ResellerId = 0;
			}
			if(!Page.IsPostBack && ResellerId != 0)
				BindInfo();
		}
	
	

		private void BindInfo()
		{
			using(IDataReader reader = CManage.ResellerGet(ResellerId))
			{
				reader.Read();
				tbTitle.Text = reader["Title"].ToString();
				tbContactName.Text = reader["ContactName"].ToString();
				tbContactEmail.Text = reader["ContactEmail"].ToString();
				tbContactPhone.Text = reader["ContactPhone"].ToString();
				tbCommPerc.Text = reader["CommissionPercentage"].ToString();
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

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			ResellerId = CManage.ResellerCreateUpdate(ResellerId,tbTitle.Text,tbContactName.Text,tbContactEmail.Text,tbContactPhone.Text, int.Parse(tbCommPerc.Text));
			Response.Redirect("../Pages/ResellerView.aspx?ResellerId=" + ResellerId);
		}
	}
}
