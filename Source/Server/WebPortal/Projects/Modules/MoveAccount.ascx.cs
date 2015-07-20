namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;

	/// <summary>
	///		Summary description for MoveAccount.
	/// </summary>
	public partial class MoveAccount : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectFinances", typeof(MoveAccount).Assembly);

		private int AccountId
		{
			get 
			{
				try
				{
					return int.Parse(Request["AccountId"]);
				}
				catch
				{
					return -1;
				}
			}
		}

		private int ProjectId
		{
			get 
			{
				try
				{
					return int.Parse(Request["ProjectId"]);
				}
				catch
				{
					return -1;
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnMove.Attributes.Add("onclick","DisableButtons(this);");
			btnMove.CustomImage = this.Page.ResolveUrl("~/layouts/images/upload.gif");
			btnCancel.Attributes.Add("onclick","DisableButtons(this);");
			BindToolbar();
			if (!IsPostBack)
			{
				lblMoveTo.Text = LocRM.GetString("tMoveTo");
				BindAccounts();
			}
		}

		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tMoveAcc");
			btnMove.Text = LocRM.GetString("tSave");
			btnCancel.Text = LocRM.GetString("tCancel");
		}

		private void BindAccounts()
		{
			DataTable dt = Finance.GetListAccountsForMoveDataTable(AccountId);
			foreach(DataRow dr in dt.Rows)
			{
				if((int)dr["OutlineLevel"]==1)
				{
					dr["Title"] = LocRM.GetString("tRoot");
					break;
				}
			}
			ddAccounts.DataSource = dt.DefaultView;
			ddAccounts.DataTextField = "Title";
			ddAccounts.DataValueField = "AccountId";
			ddAccounts.DataBind();
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
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		protected void btnMove_ServerClick(object sender, System.EventArgs e)
		{
			Finance.MoveAccount(AccountId, int.Parse(ddAccounts.SelectedValue));
			Response.Redirect("../Projects/ProjectView.aspx?ProjectId="+ProjectId+"&Tab=4&FTab=0");
		}

		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			Response.Redirect("../Projects/ProjectView.aspx?ProjectId="+ProjectId+"&Tab=4&FTab=0");
		}
	}
}
