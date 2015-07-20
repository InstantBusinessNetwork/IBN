using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Business.Customization;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Modules
{
	public partial class CustomPageDesign : System.Web.UI.UserControl
	{
		#region ClassName
		protected string ClassName
		{
			get
			{
				return Request["ClassName"];
			}
		}
		#endregion

		#region ObjectId
		protected int? ObjectId
		{
			get
			{
				try
				{
					return int.Parse(Request["ObjectId"]);
				}
				catch
				{
					return null;
				}
			}
		}
		#endregion

		#region CustomPageId
		protected Guid CustomPageId
		{
			get
			{
				Guid retval = Guid.Empty;
				if (!String.IsNullOrEmpty(Request["Id"]))
					retval = new Guid(Request["Id"]);

				return retval;
			}
		}
		#endregion

		#region PageUid
		protected Guid PageUid
		{
			get
			{
				Guid retval = Guid.Empty;
				if (!String.IsNullOrEmpty(Request["PageUid"]))
					retval = new Guid(Request["PageUid"]);

				return retval;
			}
		}
		#endregion

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

		#region UserId
		protected int? UserId
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
				BindInfo();
				BindToolbar();
			}

			if (!Page.ClientScript.IsClientScriptBlockRegistered("grid.css"))
			{
				string cssLink = String.Format(CultureInfo.InvariantCulture,
					"<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />",
					Mediachase.Ibn.Web.UI.WebControls.McScriptLoader.Current.GetScriptUrl("~/Styles/IbnFramework/grid.css", this.Page));
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "grid.css", cssLink);
			}

		}

		#region BindInfo
		private void BindInfo()
		{
			// Title
			if (CustomPageId != Guid.Empty)
			{
				CustomPageEntity entity = (CustomPageEntity)BusinessManager.Load(CustomPageEntity.ClassName, (PrimaryKeyId)CustomPageId);
				if (entity != null)
				{
					PageLabel.Text = CHelper.GetResFileString(entity.Title);
				}
			}

			if (ProfileId.HasValue)
			{
				// Link to profile
				LayerLabel.Text = String.Concat(GetGlobalResourceObject("IbnFramework.Profile", "PortalProfile").ToString(), ":");
				CustomizationProfileEntity profile = (CustomizationProfileEntity)BusinessManager.Load(CustomizationProfileEntity.ClassName, (PrimaryKeyId)ProfileId.Value);
				if (profile != null)
				{
					LayerLink.Text = CHelper.GetResFileString(profile.Name);
					LayerLink.NavigateUrl = String.Format(CultureInfo.InvariantCulture,
						"~/Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName={0}&ObjectId={1}&Tab=PageCustomization",
						CustomizationProfileEntity.ClassName,
						ProfileId.Value);

					ClearUserSettingsButton.Attributes.Add("onclick",
						String.Concat("return confirm('", GetGlobalResourceObject("IbnFramework.Global", "_mc_WsAdminConfirmation").ToString(), "?');"));
				}

				// Clear settings
				ClearUserSettingsButton.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_WsAdminPageApply").ToString();
			}
		} 
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			MainHeader.Title = GetGlobalResourceObject("IbnFramework.Profile", "PageCustomization").ToString();

			string link = string.Empty;

			if (String.IsNullOrEmpty(ClassName))
				link = ResolveClientUrl("~/Apps/Administration/Pages/PortalCustomization.aspx?Tab=PageCustomization");
			else if (String.Compare(ClassName, CustomizationProfileEntity.ClassName, true) == 0)
				link = String.Format(CultureInfo.InvariantCulture,
					"{0}?ClassName={1}&ObjectId={2}&Tab=PageCustomization",
					ResolveClientUrl("~/Apps/MetaUIEntity/Pages/EntityView.aspx"),
					ClassName,
					ObjectId);

			string text = CHelper.GetIconText(GetGlobalResourceObject("IbnFramework.Common", "Back").ToString(), ResolveClientUrl("~/Images/IbnFramework/cancel.GIF"));
			if (!String.IsNullOrEmpty(link))
				MainHeader.AddLink(text, link);
		}
		#endregion

		#region ClearUserSettingsButton_Click
		protected void ClearUserSettingsButton_Click(object sender, EventArgs e)
		{
			CustomPageManager.ResetUserSettingsByProfile(PageUid, ProfileId.Value);
			Response.Redirect(Request.Url.AbsoluteUri);
		}
		#endregion
	}
}