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

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.XmlTools;
using Mediachase.Ibn.Business.Customization;
using Mediachase.Ibn.Lists;

namespace Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls
{
	public partial class Publish : System.Web.UI.UserControl
	{
		#region ClassName
		private string ClassName
		{
			get
			{
				string retval = string.Empty;
				if (Request["ClassName"] != null)
					retval = Request["ClassName"];
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(ClassName))
				throw new ArgumentException("ClassName");

			if (!IsPostBack)
			{
				MetaClass mc = MetaDataWrapper.GetMetaClassByName(ClassName);

				// Tree
				BindTree();

				// Default Values
				ItemText.Text = CHelper.GetResFileString(mc.PluralName);

				// Style
				Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), Guid.NewGuid().ToString(),
					String.Format("<link type='text/css' rel='stylesheet' href='{0}' />", ResolveClientUrl("~/Styles/Shell/mainLeftTemplate.css")));

				// Header
				MainHeader.AddLink(
					CHelper.GetIconText(CHelper.GetResFileString("{IbnFramework.ListInfo:tClose}"), ResolveClientUrl("~/layouts/images/cancel.gif")), 
					"javascript:window.close();");

				// Buttons
				PublishButton.Attributes.Add("onclick", "DisableButtons(this);");
				PublishButton.Style.Add(HtmlTextWriterStyle.Width, "150px;");
				CloseButton.Attributes.Add("onclick", "window.close();");
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

			string url = CHelper.GetLinkEntityList(ClassName);
			ListInfo li = ListManager.GetListInfoByMetaClassName(ClassName);

			NavigationManager.AddNavigationItemForList(NodeIdField.Value, order, ItemText.Text, url, (int)li.PrimaryKeyId.Value);

			CHelper.CloseIt(Response);
		}
		#endregion
	}
}