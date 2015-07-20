using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for MainHome.
	/// </summary>
	public partial class MainHome : System.Web.UI.UserControl, IPageTemplateTitle
	{
		private string Tab
		{
			get
			{
				try
				{
					return int.Parse(Request["Tab"]).ToString();
				}
				catch
				{
					return null;
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindTabs();
		}

		#region BindTabs
		private void BindTabs()
		{
			string curTab = Tab;
			if (curTab == null)
				curTab = "0";

			ctrlTopTab.AddTab("Administration", "0");
			ctrlTopTab.AddTab("Reports", "1");
			//			ctrlTopTab.AddTab("Billing/Commissions","2");
			///ctrlTopTab.AddTab("Customer Support","3");
			ctrlTopTab.AddTab("Tariffs", "4");
			ctrlTopTab.TabWidth = "150px";

			ctrlTopTab.SelectItem(curTab);

			string controlName = "";
			switch (curTab)
			{
				case "1":
					controlName = "Reports.ascx";
					break;
				/*case "2":
					controlName = "Billing.ascx";
					break;
				case "3":
					controlName = "CustomerSupport.ascx";
					break;*/
				case "4":
					controlName = "Tariffs.ascx";
					break;
				default:
					controlName = "ASPHome.ascx";
					break;
			}
			System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
			phItems.Controls.Add(control);
		}

		#endregion

		public string Modify(string str)
		{
			return "Welcome to Site Administration";
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
