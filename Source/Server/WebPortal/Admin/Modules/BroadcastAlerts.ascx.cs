namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using System.Reflection;

	/// <summary>
	///		Summary description for BroadcastAlerts.
	/// </summary>
	public partial class BroadcastAlerts : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolBar();
			BindData();
		}

		private void BindToolBar()
		{
			secHeader.Title = LocRM.GetString("tBroadcastAlerts");
			secHeader.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle'>&nbsp;{1}",
				ResolveClientUrl("~/Layouts/Images/newitem.gif"), LocRM.GetString("tAlertAdd")), 
				ResolveUrl("~/Admin/AddBroadcastAlert.aspx"));
			secHeader.AddLink(String.Format("<img width='16' height='16' title='{0}' border='0' align='top' src='{1}'/>&nbsp;{0}",
				LocRM.GetString("tAddTools"),
				this.Page.ResolveUrl("~/Layouts/Images/cancel.gif")),
				ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin8"));
		}

		private void BindData()
		{
			MessRep.DataSource = Mediachase.IBN.Business.Common.GetBroadCastMessages(false);
			MessRep.DataBind();
			if (MessRep.Items.Count == 0)
			{
				MessRep.Visible = false;
				lblNoItems.Visible = true;
				lblNoItems.Text = LocRM.GetString("tNoItems");
			}
			else
			{
				MessRep.Visible = true;
				lblNoItems.Visible = false;
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

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
