namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Text;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.IBN.Business.SpreadSheet;
	using System.Globalization;

	/// <summary>
	///		Summary description for FinanceSpreadSheetPopUp.
	/// </summary>
	public partial class FinanceSpreadSheetPopUp : System.Web.UI.UserControl
	{
		#region Html Controls

    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(FinanceSpreadSheetPopUp).Assembly);
    protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectFinances", typeof(FinanceSpreadSheetPopUp).Assembly);
		#endregion

		#region prop: RowId
		public string RowId
		{
			get 
			{
				if (Request["RowId"] == null) 
					return string.Empty;
				return Request["RowId"];
			}
		}
		#endregion

		#region prop: Index
		public int Index
		{
			get 
			{
				if (Request["Index"] == null) 
					return -1;
				return int.Parse(Request["Index"]);
			}
		}
		#endregion

		#region prop: Value
		public string Value
		{
			get 
			{
				if (Request["Value"] == null) 
					return string.Empty;
				return Request["Value"];
			}
		}
		#endregion

		#region prop: Created
		public int Created
		{
			get
			{
				if (Request["Created"] == null)
					return -1;
				return int.Parse(Request["Created"]);
			}
		}
		#endregion

		#region Page_Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnOk.ServerClick +=new EventHandler(btnOk_ServerClick);
			btnCancel.ServerClick +=new EventHandler(btnCancel_ServerClick);
			btnOk.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");
			btnOk.CustomImage = this.Page.ResolveUrl("~/layouts/images/accept.gif");
			btnCancel.CustomImage = this.Page.ResolveUrl("~/layouts/images/cancel.gif");

			bool allowAdd = true;
			ActualFinances[] aFinances = ActualFinances.List(int.Parse(Request["ProjectId"], CultureInfo.InvariantCulture), Mediachase.IBN.Business.ObjectTypes.Project);

			foreach (ActualFinances af in aFinances)
			{
				if (af.RowId == this.RowId.Substring(0, RowId.IndexOf("-")))
				{
					allowAdd = false;
					break;
				}
			}

			if (!allowAdd)
			{
				btnOk.Style.Add("display", "none");
				rowUsual.Style.Add("display", "none");
				rowUsual2.Style.Add("display", "none");
				rowError.Style.Add("display", "block");
			}
			else
			{
				btnOk.Style.Add("display", "inline");
				rowUsual.Style.Add("display", "block");
				rowUsual2.Style.Add("display", "block");
				rowError.Style.Add("display", "none");
			}

			if (Value != string.Empty)
			{
				if (Value == "!EMPTY!")
				{
					tbName.Text = LocRM2.GetString("tNewItem");
					this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), string.Format("<script language=JavaScript>window.opener.mygrid.setItemText('{0}','{1}');</script>", RowId, tbName.Text));
				}
				else
				{
					tbName.Text = Value;
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
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region btnOk_ServerClick
		/// <summary>
		/// Handles the ServerClick event of the btnOk control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnOk_ServerClick(object sender, EventArgs e)
		{
			ProjectSpreadSheet.SetUserRowName(int.Parse(Request["ProjectId"]), RowId, tbName.Text);

			StringBuilder sb = new StringBuilder();
			sb.Append("<script language=JavaScript>");
            sb.AppendFormat("window.opener.mygrid.setItemText('{0}','{2}');", RowId, Index, tbName.Text);
			sb.Append("window.close();");
			sb.Append("</script>");
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), sb.ToString());
		}
		#endregion

		#region RegisterJsClose
		/// <summary>
		/// Registers the js close.
		/// </summary>
		private void RegisterJsClose()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<script language=JavaScript>");
			if (Created > 0)
			{
				sb.AppendFormat("window.opener.dhtmlXGrid_DeleteRow('{0}');", RowId);
			}
			sb.Append("window.close();");
			sb.Append("</script>");
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), sb.ToString());

		} 
		#endregion

		#region btnCancel_ServerClick
		/// <summary>
		/// Handles the ServerClick event of the btnCancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnCancel_ServerClick(object sender, EventArgs e)
		{
			RegisterJsClose();
		}
		#endregion
	}
}
