using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for Resellers.
	/// </summary>
	public partial  class Resellers : System.Web.UI.UserControl
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				ViewState["SortExpression"] = "Title";
				BindDGResellers();
			}
			BindToolbar();
			dgResellers.DeleteCommand += new DataGridCommandEventHandler(dgResellers_DeleteCommand);
		}

		private void dgResellers_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int id = int.Parse(e.CommandArgument.ToString());
			CManage.DeleteReseller(id);

			BindDGResellers();
		}

		private void BindToolbar()
		{
			secHeader.AddLink("Add Reseller","../Pages/ResellerEdit.aspx");
		}


		private void BindDGResellers()
		{
			DataView dv = new DataView(CManage.ResellerGetDataTable());
			dv.Sort = (string)ViewState["SortExpression"];
			if ((string)ViewState["SortAscending"] == "no")
				dv.Sort +=" DESC";

			dgResellers.DataSource  =  dv;
			dgResellers.DataBind();

			foreach (DataGridItem itm in dgResellers.Items)
			{
				ImageButton ib = (ImageButton)itm.FindControl("ibDelete");
				if (ib!=null)
						ib.Attributes.Add("onclick","return confirm('Do you really want to delete this Reseller?')");
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


		private void dgResellers_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			String strSortBy = (String) ViewState["SortExpression"];
			String strSortAscending = (String) ViewState["SortAscending"];

			ViewState["SortExpression"] = e.SortExpression;

			if (e.SortExpression == strSortBy)
				ViewState["SortAscending"] = (strSortAscending=="yes" 
					? "no" :"yes");
			else
				ViewState["SortAscending"] = "yes";

			BindDGResellers();
		}

		protected string GetPercentage(int _val)
		{
			return _val.ToString()+"%";
		}
	}
}
