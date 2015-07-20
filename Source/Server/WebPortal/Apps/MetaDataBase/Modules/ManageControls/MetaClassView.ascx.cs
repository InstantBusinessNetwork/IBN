using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.UI.Web.Modules;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class MetaClassView : System.Web.UI.UserControl
	{
		private string _className = "";

		private readonly string bindSectionHeaderKey = "BindSectionHeader";
		private readonly string placeNameKey = "PlaceName";
		private readonly string showSystemInfoKey = "ShowSystemInfo";

		protected MetaClass mc = null;

		#region ClassName
		public string ClassName
		{
			get { return _className; }
			set { _className = value; }
		}
		#endregion

		#region SectionHeader
		public PageViewMenu SectionHeader
		{
			get
			{
				return BlockHeaderMain;
			}
		}
		#endregion

		#region BindSectionHeader
		public bool BindSectionHeader
		{
			get
			{
				bool retval = true;
				if (ViewState[bindSectionHeaderKey] != null)
					retval = (bool)ViewState[bindSectionHeaderKey];
				return retval;
			}
			set
			{
				ViewState[bindSectionHeaderKey] = value;
			}
		}
		#endregion

		#region PlaceName
		public string PlaceName
		{
			get 
			{
				if (ViewState[placeNameKey] != null)
					return (string)ViewState[placeNameKey];
				else
					return "";
			}
			set 
			{
				ViewState[placeNameKey] = value;
			}
		}
		#endregion

		#region ShowSystemInfo
		public bool ShowSystemInfo
		{
			get
			{
				bool retval = true;
				if (ViewState[showSystemInfoKey] != null)
					retval = (bool)ViewState[showSystemInfoKey];
				return retval;
			}
			set
			{
				ViewState[showSystemInfoKey] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			// [2008-05-27] O.R.: Ibn 4.7 only
			if (!Mediachase.IBN.Business.Configuration.TimeTrackingCustomization)
			{
				if (ClassName.ToLowerInvariant() == "timetrackingblock" || ClassName.ToLowerInvariant() == "timetrackingentry" || ClassName.ToLowerInvariant() == "timetrackingblocktypeinstance")
					throw new LicenseRestrictionException();
			}

			LoadRequestVariables();

			CHelper.AddToContext("ClassName", ClassName);
			//CHelper.AddToContext(NavigationBlock.KeyContextMenu, "MetaClassView");
			//CHelper.AddToContext(NavigationBlock.KeyContextMenuTitle, CHelper.GetResFileString(mc.FriendlyName));

			this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
			xmlStruct.InnerDataBind += new XmlFormBuilder.InnerDataBindEventHandler(xmlStruct_InnerDataBind);

			if (!Page.IsPostBack)
			{
				xmlStruct.ClassName = ClassName;
				xmlStruct.LayoutType = LayoutType.MetaClassView;
				if (!String.IsNullOrEmpty(PlaceName))
					xmlStruct.PlaceName = PlaceName;
				xmlStruct.LayoutMode = LayoutMode.WithTabs;
				xmlStruct.CheckVisibleTab = mc;

				xmlStruct.DataBind();
			}
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (BindSectionHeader)
				BindToolbar();

			if (CHelper.NeedToDataBind())
			{
				xmlStruct.CheckVisibleTab = mc;
				xmlStruct.DataBind();
			}

			object rebindPage = CHelper.GetFromContext("RebindPage");
			if (rebindPage != null && rebindPage.ToString() == "true")
			{
				MakeDataBind(this);
			}
		}
		#endregion

		#region Page_PreRenderComplete
		void Page_PreRenderComplete(object sender, EventArgs e)
		{
			CHelper.RequireDataBind(false);
		}
		#endregion

		#region xmlStruct_InnerDataBind
		void xmlStruct_InnerDataBind(object sender, EventArgs e)
		{
			MakeDataBind(this);
		}
		#endregion

		#region LoadRequestVariables
		private void LoadRequestVariables()
		{
			if (Request.QueryString["class"] != null)
			{
				ClassName = Request.QueryString["class"];
				mc = MetaDataWrapper.GetMetaClassByName(ClassName);
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			string title = "";
			if (mc != null)
			{
				if (mc.IsCard)
					title = "CardCustomization";
				else if (mc.IsBridge)
					title = "BridgeCustomization";
				else
					title = "InfoCustomization";
			}
			BlockHeaderMain.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", title).ToString();

			// Menu
			ComponentArt.Web.UI.MenuItem topMenuItem;
			ComponentArt.Web.UI.MenuItem subItem;


			// O.R. [2008-11-12]: New Field is in the Fields tab
			/*
			// New Field
			topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Look.LeftIconUrl = "~/images/IbnFramework/newitem.gif";
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(16);
			topMenuItem.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewField").ToString();
			topMenuItem.NavigateUrl = String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaDataBase/Pages/Admin/MetaFieldEdit.aspx?class={0}", mc.Name);
			topMenuItem.LookId = "TopItemLook";
			BlockHeaderMain.ActionsMenu.Items.Add(topMenuItem);
			 */

			// O.R.: IBN 4.7 fix
			/*			// New Link
						topMenuItem = new ComponentArt.Web.UI.MenuItem();
						topMenuItem.Look.LeftIconUrl = "~/images/IbnFramework/newitem.gif";
						topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
						topMenuItem.Look.LeftIconHeight = Unit.Pixel(16);
						topMenuItem.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewLink").ToString();
						topMenuItem.NavigateUrl = String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaDataBase/Pages/Admin/MetaFieldEdit.aspx?class={0}", mc.Name);
						topMenuItem.LookId = "TopItemLook";
						BlockHeaderMain.ActionsMenu.Items.Add(topMenuItem);
			*/

			// Actions
			topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "tActions").ToString();
			topMenuItem.DefaultSubGroupExpandDirection = ComponentArt.Web.UI.GroupExpandDirection.BelowRight;
			topMenuItem.Look.LeftIconUrl = "~/Layouts/Images/downbtn1.gif";
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			if (mc.Attributes.ContainsKey(MetaClassAttribute.IsBridge))
			{
				// Edit Bridge
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/images/IbnFramework/edit.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Edit").ToString();
				subItem.NavigateUrl = String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaDataBase/Pages/Admin/MetaBridgeEdit.aspx?class={0}&back=view", mc.Name);
				topMenuItem.Items.Add(subItem);
			}
			else if (mc.Attributes.ContainsKey(MetaClassAttribute.IsCard))
			{
				// Edit Card
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/images/IbnFramework/edit.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Edit").ToString();
				subItem.NavigateUrl = String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaDataBase/Pages/Admin/MetaCardEdit.aspx?class={0}&back=view", mc.Name);
				topMenuItem.Items.Add(subItem);
			}
			else
			{
				if (mc.SupportsCards)
				{
					// Add Card
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconUrl = "~/images/IbnFramework/metainfo/card.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewCard").ToString();
					subItem.NavigateUrl = String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaDataBase/Pages/Admin/MetaCardEdit.aspx?owner={0}&back=owner", mc.Name);
					topMenuItem.Items.Add(subItem);
				}

				// Edit 
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/images/IbnFramework/edit.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Edit").ToString();
				subItem.NavigateUrl = String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaDataBase/Pages/Admin/MetaClassEdit.aspx?class={0}&back=view", mc.Name);
				topMenuItem.Items.Add(subItem);
			}

			// O.R.: IBN 4.7 fix
/*			// Back 
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/images/IbnFramework/cancel.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			subItem.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BackToList").ToString();
			subItem.NavigateUrl = "~/Apps/MetaDataBase/Pages/Admin/MetaClassList.aspx";
			topMenuItem.Items.Add(subItem);
 */ 

			BlockHeaderMain.ActionsMenu.Items.Add(topMenuItem);
		}
		#endregion

		#region MakeDataBind
		private void MakeDataBind(Control _cntrl)
		{
			foreach (Control c in _cntrl.Controls)
			{
				MCDataBoundControl boundControl = c as MCDataBoundControl;
				if (boundControl != null)
				{
					boundControl.ShowSystemInfo = ShowSystemInfo;
					boundControl.DataItem = mc;
					boundControl.DataBind();
					continue;
				}
				else if (c.Controls.Count > 0)
					MakeDataBind(c);
			}
		}
		#endregion
	}
}