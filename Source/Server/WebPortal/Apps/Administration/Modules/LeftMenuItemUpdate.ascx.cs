using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml.XPath;

using Mediachase.Ibn.Business.Customization;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Administration.Modules
{
	public partial class LeftMenuItemUpdate : System.Web.UI.UserControl
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

		#region FullId
		private string FullId
		{
			get
			{
				string retval = string.Empty;
				if (Request["FullId"] != null)
					retval = Request["FullId"];
				return retval;
			}
		}
		#endregion

		#region SavedText
		private string SavedText
		{
			get
			{
				string retval = string.Empty;
				if (ViewState["Text"] != null)
					retval = ViewState["Text"].ToString();
				return retval;
			}
			set
			{
				ViewState["Text"] = value;
			}
		}
		#endregion

		#region SavedOrder
		private int SavedOrder
		{
			get
			{
				int retval = 10000;
				if (ViewState["Order"] != null)
					retval = (int)ViewState["Order"];
				return retval;
			}
			set
			{
				ViewState["Order"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindValues();

				CancelButton.Attributes.Add("onclick", Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true));
			}
		}

		#region BindValues
		private void BindValues()
		{
			string[] parts = FullId.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

			string path = "Navigation/Tabs";
			if (parts.Length > 0)
				path += String.Format(CultureInfo.InvariantCulture, "/Tab[@id='{0}']", parts[0]);

			for (int i = 1; i < parts.Length; i++)
				path += String.Format(CultureInfo.InvariantCulture, "/Link[@id='{0}']", parts[i]);

			string profileString = ProfileId.HasValue ? ProfileId.Value.ToString() : String.Empty;
			string principalString = PrincipalId.HasValue ? PrincipalId.Value.ToString() : String.Empty;
			if (PrincipalId.HasValue)
				profileString = ProfileManager.GetProfileIdByUser(PrincipalId.Value).ToString();

			Mediachase.Ibn.XmlTools.Selector selector = new Mediachase.Ibn.XmlTools.Selector(string.Empty, string.Empty, string.Empty, profileString, principalString);
			IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetCustomizationXml(null, Mediachase.Ibn.XmlTools.StructureType.Navigation, selector);
			XPathNavigator node = navigable.CreateNavigator().SelectSingleNode(path);
			if (node != null)
			{
				ctrlTitleText.Text = node.GetAttribute("text", string.Empty);

				ItemOrder.Text = node.GetAttribute("order", string.Empty);

				SavedText = ctrlTitleText.Text;
				SavedOrder = int.Parse(ItemOrder.Text);
			}
		}
		#endregion

		#region SaveButton_ServerClick
		protected void SaveButton_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			string text = ctrlTitleText.Text.Trim();

			int order;
			if (!int.TryParse(ItemOrder.Text, out order))
				order = 10000;

			if (text != SavedText || order != SavedOrder)
				NavigationManager.ModifyNavigationItem(FullId, order, text, ProfileId, PrincipalId);

			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, string.Empty);
		}
		#endregion
	}
}