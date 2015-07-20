using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Modules;

namespace Mediachase.Ibn.Web.UI.ListApp.Pages
{
	public partial class ListInfoView : System.Web.UI.Page
	{
		#region ClassName
		/// <summary>
		/// Gets the name of the class.
		/// </summary>
		/// <value>The name of the class.</value>
		protected string ClassName
		{
			get
			{
				string retval = String.Empty;
				if (Request["class"] != null)
					retval = Request["class"];
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			ListInfo li = ListManager.GetListInfoByMetaClassName(ClassName);
			if (!IsPostBack)
			{	
				if (!li.IsTemplate && !Mediachase.IBN.Business.ListInfoBus.CanAdmin(li.PrimaryKeyId.Value))
					throw new AccessDeniedException();
			}
			if(li.IsTemplate)
				pT.Title = CHelper.GetResFileString("{IbnFramework.ListInfo:ListTempManagement}");
			else
				pT.Title = CHelper.GetResFileString("{IbnFramework.ListInfo:ListManagement}");
			pT.SetControlProperties("BindSectionHeader", false);
			pT.SetControlProperties("PlaceName", "ListInfoView");
			pT.SetControlProperties("ShowSystemInfo", false);
			
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (String.IsNullOrEmpty(ClassName))
				return;

			MetaClass mc = MetaDataWrapper.GetMetaClassByName(ClassName);

			try
			{
				object obj = pT.CurrentControl.GetType().BaseType.GetProperty("SectionHeader").GetValue(pT.CurrentControl, null);
				if (obj is PageViewMenu)
				{
					PageViewMenu BlockHeaderMain = obj as PageViewMenu;

					ListInfo li = ListManager.GetListInfoByMetaClass(mc);

					BlockHeaderMain.Title = GetGlobalResourceObject("IbnFramework.ListInfo", "ListManagement").ToString();

					// Menu
					ComponentArt.Web.UI.MenuItem topMenuItem;
					ComponentArt.Web.UI.MenuItem subItem;

					// New Field
/*					topMenuItem = new ComponentArt.Web.UI.MenuItem();
					topMenuItem.Look.LeftIconUrl = "~/images/IbnFramework/newitem.gif";
					topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
					topMenuItem.Look.LeftIconHeight = Unit.Pixel(16);
					topMenuItem.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewField").ToString();
					topMenuItem.NavigateUrl = String.Format(CultureInfo.InvariantCulture, "~/Apps/ListApp/Pages/MetaFieldEdit.aspx?class={0}", mc.Name);
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

					// Edit
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconUrl = "~/images/IbnFramework/edit.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "Edit").ToString();
					subItem.NavigateUrl = String.Format(CultureInfo.InvariantCulture, "~/Apps/ListApp/Pages/ListInfoEdit.aspx?class={0}", ClassName);
					topMenuItem.Items.Add(subItem);

					if (!li.IsTemplate)
					{
						// Publish
						if (Mediachase.IBN.Business.Security.IsUserInGroup(Mediachase.IBN.Business.InternalSecureGroups.Administrator))
						{
							ListFolder folder = new ListFolder(li.FolderId.Value);
							if (folder.FolderType == ListFolderType.Public)
							{
								subItem = new ComponentArt.Web.UI.MenuItem();
								subItem.Look.LeftIconUrl = "~/Images/IbnFramework/PublishList.png";
								subItem.Look.LeftIconWidth = Unit.Pixel(16);
								subItem.Look.LeftIconHeight = Unit.Pixel(16);
								subItem.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "Publish").ToString();
								subItem.ClientSideCommand = CommandManager.GetCurrent(this.Page).AddCommand("", "", "ListInfoView", "MC_ListApp_Publish", new Dictionary<string, string>());
								topMenuItem.Items.Add(subItem);
							}
						}

						// Security
						subItem = new ComponentArt.Web.UI.MenuItem();
						subItem.Look.LeftIconUrl = "~/Layouts/Images/icon-key.gif";
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
						subItem.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "Security").ToString();
						subItem.ClientSideCommand = CommandManager.GetCurrent(this.Page).AddCommand("", "", "ListInfoView", "MC_ListApp_Security", new Dictionary<string, string>());
						topMenuItem.Items.Add(subItem);
					}

					// Delete
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconUrl = "~/images/IbnFramework/delete.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "Delete").ToString();
					if(li.IsTemplate)
						subItem.ClientSideCommand = CommandManager.GetCurrent(this.Page).AddCommand("", "", "ListInfoView", "MC_ListApp_DeleteTemplateList", new Dictionary<string, string>());
					else
						subItem.ClientSideCommand = CommandManager.GetCurrent(this.Page).AddCommand("", "", "ListInfoView", "MC_ListApp_DeleteList", new Dictionary<string, string>());

					topMenuItem.Items.Add(subItem);
					
					// Delimeter
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.LookId = "BreakItem";
					topMenuItem.Items.Add(subItem);

					// Back 
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconUrl = "~/images/IbnFramework/cancel.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "BackToListInfoList").ToString();
					if(li.IsTemplate)
						subItem.NavigateUrl = "~/Apps/ListApp/Pages/ListTemplates.aspx";
					else
						subItem.NavigateUrl = String.Format(CultureInfo.InvariantCulture, "~/Apps/ListApp/Pages/ListInfoList.aspx?ListFolderId={0}", li.FolderId);
					topMenuItem.Items.Add(subItem);

					BlockHeaderMain.ActionsMenu.Items.Add(topMenuItem);
				}
			}
			catch { }
		}
	}
}
