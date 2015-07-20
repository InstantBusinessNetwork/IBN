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
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.XmlTools;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.Administration.Modules
{
	public partial class LeftMenuItemAdd : System.Web.UI.UserControl
	{
		#region ProfileId
		protected int? ProfileId
		{
			get
			{
				int? retval = null;

				if (String.Compare(Request["ClassName"], CustomizationProfileEntity.ClassName, true) == 0
					&& !String.IsNullOrEmpty(Request["ObjectId"]))
					retval = int.Parse(Request["ObjectId"]);
				return retval;
			}
		}
		#endregion

		#region PrincipalId
		protected int? PrincipalId
		{
			get
			{
				int? retval = null;

				if (String.Compare(Request["ClassName"], "Principal", true) == 0
					&& !String.IsNullOrEmpty(Request["ObjectId"]))
					retval = int.Parse(Request["ObjectId"]);
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
			MenuTree.TreeSourceUrl = ResolveClientUrl("~/Apps/Shell/Pages/TreeSource.aspx?mode=full");
			MenuTree.BlankImageUrl = ResolveClientUrl("~/Images/IbnFramework/s.gif");
			MenuTree.RootVisible = false;
			MenuTree.AfterRenderScript = "InitMenuTree()";
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

			NavigationManager.AddNavigationItem(NodeIdField.Value, order, ctrlTitleText.Text.Trim(), ItemUrl.Text.Trim(), (PrimaryKeyId?)ProfileId, (PrimaryKeyId?)PrincipalId);

			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, string.Empty);
		}
		#endregion
	}
}