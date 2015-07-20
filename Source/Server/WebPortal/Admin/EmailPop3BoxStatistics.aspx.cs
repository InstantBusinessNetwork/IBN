using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail;
using System.Resources;
using System.Reflection;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Admin
{
	public partial class EmailPop3BoxStatistics : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected string sTitle
		{
			get 
			{
				return LocRM.GetString("tStatistics");
			}
		}

		#region BoxId
		public int BoxId
		{
			get
			{
				if (Request["BoxId"] != null)
					return int.Parse(Request["BoxId"]);
				else return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/dialog.css");

			ApplyLocalization();
			if (!Page.IsPostBack)
			{
				BindBox();
			}
			imbCancel.CustomImage = this.Page.ResolveUrl("~/Layouts/Images/cancel.gif");
		}

		protected void ApplyLocalization()
		{
			secHeader.Title = LocRM.GetString("tStatistics");
			secHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tClose"), "javascript:window.close();");
			imbCancel.Text = LocRM.GetString("tClose");
		}

		protected void BindBox()
		{
			EMailRouterPop3Box box = EMailRouterPop3Box.Load(BoxId);
			if (box != null)
			{
				lbName.Text = box.Name;
				EMailRouterPop3BoxActivity act = box.Activity;
				if (act.IsActive)
				{
					lbIsActive.Text = LocRM.GetString("tYes");
				}
				else
				{
					lbIsActive.Text = LocRM.GetString("tNo");
				}
				lbLastReq.Text = act.LastRequest.ToString("g");
				lbLastSuccReq.Text = act.LastSuccessfulRequest.ToString("g");
				lbMessageCount.Text = act.TotalMessageCount.ToString();
				if (act.ErrorText.Trim() != string.Empty)
				{
					trLastErrText.Visible = true;
					lbLastErrText.Text = act.ErrorText.Trim();
				}
			}
		}
	}
}
