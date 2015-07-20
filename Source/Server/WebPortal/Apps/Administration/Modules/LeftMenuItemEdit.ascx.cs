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
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.Administration.Modules
{
	public partial class LeftMenuItemEdit : System.Web.UI.UserControl
	{
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

		#region PrincipalId
		protected PrimaryKeyId? PrincipalId
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
			Dictionary<string, CustomizationItemArgumentEntity> args = NavigationManager.GetCustomizationItemArguments(FullId, CustomizationStructureType.NavigationMenu, ItemCommandType.Add, ProfileId, PrincipalId);
			if (args.ContainsKey(NavigationManager.ItemArgumentText))
				ctrlTitleText.Text = args[NavigationManager.ItemArgumentText].Value;
			if (args.ContainsKey(NavigationManager.ItemArgumentUrl))
				ItemUrl.Text = args[NavigationManager.ItemArgumentUrl].Value;
			if (args.ContainsKey(NavigationManager.ItemArgumentOrder))
				ItemOrder.Text = args[NavigationManager.ItemArgumentOrder].Value;
		}
		#endregion

		#region SaveButton_ServerClick
		protected void SaveButton_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			string text = ctrlTitleText.Text.Trim();
			string url = ItemUrl.Text.Trim();

			int order;
			if (!int.TryParse(ItemOrder.Text, out order))
				order = 10000;

			NavigationManager.UpdateNavigationItem(FullId, order, text, url, ProfileId, PrincipalId);

			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, string.Empty);
		}
		#endregion
	}
}