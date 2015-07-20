using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.XPath;

using Mediachase.Ibn.Business.Customization;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.XmlTools;
using Mediachase.Ibn.Web.UI.WebControls;


namespace Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Modules
{
	public partial class CustomPagePublish : System.Web.UI.UserControl
	{
		#region PageUid
		protected Guid PageUid
		{
			get
			{
				Guid retval = Guid.Empty;
				if (!String.IsNullOrEmpty(Request["Uid"]))
					retval = new Guid(Request["Uid"]);

				return retval;
			}
		}
		#endregion

		#region ProfileId
		protected PrimaryKeyId? ProfileId
		{
			get
			{
				PrimaryKeyId? retval = null;

				if (String.Compare(Request["ClassName"], CustomizationProfileEntity.ClassName, true) == 0
					&& !String.IsNullOrEmpty(Request["ObjectId"]))
					retval = PrimaryKeyId.Parse(Request["ObjectId"]);
				return retval;
			}
		}
		#endregion

		#region UserId
		protected PrimaryKeyId? UserId
		{
			get
			{
				PrimaryKeyId? retval = null;

				if (String.Compare(Request["ClassName"], "Principal", true) == 0
					&& !String.IsNullOrEmpty(Request["ObjectId"]))
					retval = PrimaryKeyId.Parse(Request["ObjectId"]);
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				// Tree
				BindTree();

				// Title
				BindTitle();

				// Style
				Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), Guid.NewGuid().ToString(),
					String.Format("<link type='text/css' rel='stylesheet' href='{0}' />", ResolveClientUrl("~/Styles/Shell/mainLeftTemplate.css")));

				// Buttons
				PublishButton.Attributes.Add("onclick", "DisableButtons(this);");
				PublishButton.Style.Add(HtmlTextWriterStyle.Width, "150px;");
				CloseButton.Attributes.Add("onclick", Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true));
				CloseButton.Style.Add(HtmlTextWriterStyle.Width, "150px;");
			}
		}

		#region BindTree
		private void BindTree()
		{
			string url = String.Format(CultureInfo.InvariantCulture, 
				"~/Apps/Shell/Pages/TreeSource.aspx?mode=full&ProfileId={0}&UserId={1}",
				ProfileId.HasValue ? ProfileId.Value.ToString() : String.Empty,
				UserId.HasValue ? UserId.Value.ToString() : String.Empty);
			MenuTree.TreeSourceUrl = ResolveClientUrl(url);
			MenuTree.BlankImageUrl = ResolveClientUrl("~/Images/IbnFramework/s.gif");
			MenuTree.RootVisible = false;
			MenuTree.AfterRenderScript = "InitMenuTree()";
		}
		#endregion

		#region BindTitle
		private void BindTitle()
		{
			CustomPageEntity page = CustomPageManager.GetCustomPage(PageUid, (int?)ProfileId, (int?)UserId);
			if (page != null)
				ctrlTitleText.Text = page.Title;
		}
		#endregion

		#region PublishButton_ServerClick
		protected void PublishButton_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid || NodeIdField.Value == String.Empty)
				return;

			int order;
			if (!int.TryParse(ItemOrder.Text, out order))
				order = 10000;

			string url = String.Format(CultureInfo.InvariantCulture,
				"~/Apps/WidgetEngine/Pages/CustomPageView.aspx?PageUid={0}",
				PageUid.ToString());
			NavigationManager.AddNavigationItem(NodeIdField.Value, order, ctrlTitleText.Text.Trim(), url, ProfileId, UserId);

			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, string.Empty);
		}
		#endregion
	}
}