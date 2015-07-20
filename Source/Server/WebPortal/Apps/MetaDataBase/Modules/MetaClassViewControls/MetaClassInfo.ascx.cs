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
using System.Web.UI.WebControls.WebParts;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;


namespace Mediachase.Ibn.Web.UI.MetaDataBase.Modules.MetaClassViewControls
{
	public partial class MetaClassInfo : MCDataBoundControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region DataBind
		public override void DataBind()
		{
			if (DataItem != null)
			{
				MetaClass mc = DataItem as MetaClass;
				if (mc != null)
					BindData(mc);
			}
			base.DataBind();
		}
		#endregion

		#region BindData
		private void BindData(MetaClass mc)
		{
			MoreInfoRow.Visible = false;

			// Labels
			ClassNameLabel.Text = mc.Name;

			FriendlyNameLabel.Text = CHelper.GetResFileString(mc.FriendlyName);

			FriendlyNameLink.Text = CHelper.GetResFileString(mc.FriendlyName);
			FriendlyNameLink.NavigateUrl = String.Format(CultureInfo.InvariantCulture,
				"~/Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName={0}",
				mc.Name);

			// Ibn 4.7 fix
			if (mc.Name.ToLower().IndexOf("timetracking") >= 0)
			{
				FriendlyNameLabel.Visible = true;
				FriendlyNameLink.Visible = false;
			}
			else
			{
				FriendlyNameLabel.Visible = false;
				FriendlyNameLink.Visible = true;
			}

			PluralNameLabel.Text = CHelper.GetResFileString(mc.PluralName);
			if (mc.Attributes.ContainsKey(MetaClassAttribute.IsBridge))
			{
				TypeLabel.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Bridge").ToString();
				ClassTypeImage.ImageUrl = "~/images/IbnFramework/metainfo/bridge.gif";
				if (mc.Attributes.ContainsKey(MetaClassAttribute.IsSystem))
					ClassTypeImage.ImageUrl = "~/images/IbnFramework/metainfo/bridge_sys.gif";
			}
			else if (mc.Attributes.ContainsKey(MetaClassAttribute.IsCard))
			{
				TypeLabel.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BusinessObjectExtension").ToString();
				ClassTypeImage.ImageUrl = "~/images/IbnFramework/metainfo/card.gif";
				if (mc.Attributes.ContainsKey(MetaClassAttribute.IsSystem))
					ClassTypeImage.ImageUrl = "~/images/IbnFramework/metainfo/card_sys.gif";
			}
			else
			{
				TypeLabel.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Info").ToString();
				ClassTypeImage.ImageUrl = "~/images/IbnFramework/metainfo/metaclass.gif";
				if (mc.Attributes.ContainsKey(MetaClassAttribute.IsSystem))
					ClassTypeImage.ImageUrl = "~/images/IbnFramework/metainfo/metaclass_sys.gif";
			}

			// O.R.: we don't use this attribute
			/*
			// Public or Private (Department or User)
			if (mc.Attributes.ContainsKey(MetaDataWrapper.OwnerTypeAttr))
				TypeLabel.Text += String.Format(" ({0})", mc.Attributes[MetaDataWrapper.OwnerTypeAttr].ToString());
			 */

			// Owner class for Card
			MetaClass ownerClass = MetaDataWrapper.GetOwnerClass(mc);
			if (ownerClass != null)
			{
				MoreInfoValue.Text = String.Format("<a href='MetaClassView.aspx?class={0}'>{1}</a>", ownerClass.Name, CHelper.GetResFileString(ownerClass.FriendlyName));

				MoreInfoRow.Visible = true;
				MoreInfoLabel.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "TableOwner").ToString() + ":";
			}

			// Cards for owner class
			if (mc.SupportsCards)
			{
				MetaClass[] cards = mc.GetCards();
				if (cards.Length > 0)
				{
					MoreInfoRow.Visible = true;
					MoreInfoLabel.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Cards").ToString() + ":";

					string sCards = "";
					foreach (MetaClass card in cards)
					{
						if (sCards != "")
							sCards += ", ";

						sCards += String.Format("<a href='MetaClassView.aspx?class={0}'>{1}</a>", card.Name, CHelper.GetResFileString(card.FriendlyName));
					}
					MoreInfoValue.Text = sCards;
				}
			}
		}
		#endregion
	}
}