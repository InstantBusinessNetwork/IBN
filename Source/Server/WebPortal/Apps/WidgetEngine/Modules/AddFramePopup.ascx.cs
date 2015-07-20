using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.Apps.WidgetEngine;
using Mediachase.Ibn.Business.Customization;


namespace Mediachase.Ibn.Web.UI.Apps.Common.Layout.Modules
{
	public partial class AddFramePopup : System.Web.UI.UserControl
	{
		#region prop: PageUid
		/// <summary>
		/// Gets the page id.
		/// </summary>
		/// <value>The page id.</value>
		private Guid PageUid
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

		#region prop: IsAdmin

		/// <summary>
		/// Gets or sets a value indicating whether this instance is admin.
		/// </summary>
		/// <value><c>true</c> if this instance is admin; otherwise, <c>false</c>.</value>
		public bool IsAdmin
		{
			get
			{
				if (ViewState["_IsAdmin"] == null && Request["IsAdmin"] == null)
					return false;

				if (Request["IsAdmin"] != null && ViewState["_IsAdmin"] == null)
					return Convert.ToBoolean(Request["IsAdmin"], CultureInfo.InvariantCulture);

				return Convert.ToBoolean(ViewState["_IsAdmin"].ToString(), CultureInfo.InvariantCulture);
			}
			set
			{
				ViewState["_IsAdmin"] = value;
			}
		}
		#endregion

		#region prop: ColumnId
		/// <summary>
		/// Gets the column id.
		/// </summary>
		/// <value>The column id.</value>
		private string ColumnId
		{
			get
			{
				if (Request["ColumnId"] == null)
					return string.Empty;

				return Request["ColumnId"].ToString();
			}
		}
		#endregion

		#region prop: selectedTab
		/// <summary>
		/// Gets or sets the selected tab.
		/// </summary>
		/// <value>The selected tab.</value>
		public int selectedTab
		{
			get
			{
				if (ViewState["_selectedTab"] == null)
					return 0;

				return Convert.ToInt32(ViewState["_selectedTab"].ToString(), CultureInfo.InvariantCulture);
			}
			set
			{
				ViewState["_selectedTab"] = value;
			}
		}
		#endregion

		#region prop: selectedUid
		/// <summary>
		/// Gets or sets the selected uid.
		/// </summary>
		/// <value>The selected uid.</value>
		private string selectedUid
		{
			get
			{
				if (ViewState["__repTemplatesSelected"] != null)
					return ViewState["__repTemplatesSelected"].ToString();

				return string.Empty;
			}
			set
			{
				ViewState["__repTemplatesSelected"] = value.ToUpperInvariant();
			}
		}
		#endregion

		#region prop: ForcePostback
		/// <summary>
		/// Gets or sets a value indicating whether [force postback].
		/// </summary>
		/// <value><c>true</c> if [force postback]; otherwise, <c>false</c>.</value>
		public bool ForcePostback
		{
			get
			{
				if (ViewState["_ForcePostback"] == null)
					return false;

				return Convert.ToBoolean(ViewState["_ForcePostback"].ToString());
			}
			set
			{
				ViewState["_ForcePostback"] = value;
			}
		}
		#endregion

		#region prop: ViewMode
		/// <summary>
		/// Gets or sets the view mode.
		/// </summary>
		/// <value>The view mode.</value>
		public string ViewMode
		{
			get
			{
				if (ViewState["_ViewMode"] == null)
					return "0";

				return ViewState["_ViewMode"].ToString();
			}
			set
			{
				ViewState["_ViewMode"] = value;
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

		#region prop: CurrentGroup
		/// <summary>
		/// Gets or sets the current group.
		/// </summary>
		/// <value>The current group.</value>
		public string CurrentGroup
		{
			get
			{
				if (ViewState["_CurrentGroup"] == null)
					return Guid.Empty.ToString();

				return ViewState["_CurrentGroup"].ToString();
			}
			set { ViewState["_CurrentGroup"] = value; }
		} 
		#endregion

		#region Page_Load
		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			repCategories.ItemCommand += new RepeaterCommandEventHandler(repCategories_ItemCommand);
			repCategories.ItemDataBound += new RepeaterItemEventHandler(repCategories_ItemDataBound);

			if (!IsPostBack)
			{
				BindGrid();
				BindCategories();
				divControlSet.Style.Add(HtmlTextWriterStyle.Display, "block");
			}

			BindToolbar();
			
			btnAddAndClose.ServerClick += new EventHandler(btnAddAndClose_ServerClick);
			btnCancel.ServerClick += new EventHandler(btnCancel_ServerClick);

			if (this.selectedTab == 0)
			{
				divControlSet.Style.Add(HtmlTextWriterStyle.Display, "block");

				BindGrid();
			}
			else
			{
				divControlSet.Style.Add(HtmlTextWriterStyle.Display, "none");
			}

			divTemplateUpdate.Style.Add(HtmlTextWriterStyle.Display, "none");

			if (this.ViewMode == "0")
			{
				BindGrid();
				divControlSet.Style.Add(HtmlTextWriterStyle.Display, "block");
			}
			else if (this.ViewMode == "1")
			{
				divControlSet.Style.Add(HtmlTextWriterStyle.Display, "none");
			}

			lblEditTemplate.Click += new EventHandler(lblEditTemplate_Click);
			grdMain.ItemCommand += new DataGridCommandEventHandler(grdMain_ItemCommand);
		}
		#endregion

		#region grdMain_ItemCommand
		/// <summary>
		/// Handles the ItemCommand event of the grdMain control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		void grdMain_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Add")
			{
				string Ids = e.CommandArgument.ToString();
				List<CpInfo> userList = new List<CpInfo>();

				CustomPageEntity page = null;
				if (!this.IsAdmin)
					page = CustomPageManager.GetCustomPage(PageUid, null, Mediachase.IBN.Business.Security.CurrentUser.UserID);
				else
					page = CustomPageManager.GetCustomPage(PageUid, ProfileId, UserId);

				string userData = page.JsonData;
				Guid templateUid = page.TemplateId;

				List<ColumnInfo> list = new List<ColumnInfo>();
				if (this.IsAdmin)
				{
					WorkspaceTemplateInfo wti = WorkspaceTemplateFactory.GetTemplateInfo(templateUid.ToString());
					list = UtilHelper.JsonDeserialize<List<ColumnInfo>>(wti.ColumnInfo);
				}
				userList = UtilHelper.JsonDeserialize<List<CpInfo>>(userData.Replace("\\", ""));
				
				List<string> currentColumnsId = new List<string>();
				foreach (CpInfo cpItem in userList)
					currentColumnsId.Add(cpItem.Id);

				List<CpInfo> newUserList = new List<CpInfo>();
				newUserList.AddRange(userList);

				if (this.IsAdmin)
				{
					foreach (CpInfo cpItem in userList)
					{
						foreach (ColumnInfo info in list)
						{
							if (currentColumnsId.Contains(info.Id))
								continue;

							CpInfo cInfo = new CpInfo();
							cInfo.Id = info.Id;
							cInfo.Items = new List<CpInfoItem>();
							newUserList.Add(cInfo);
						}
					}
				}

				foreach (CpInfo cpItem in newUserList)
				{
					if (cpItem.Id == this.ColumnId)
					{
						CpInfoItem newItem = new CpInfoItem();
						newItem.Collapsed = "false";
						newItem.Id = Ids;
						newItem.InstanseUid = Guid.NewGuid().ToString("N");
						cpItem.Items.Add(newItem);
					}
				}

				string newJsonData = UtilHelper.JsonSerialize(newUserList);

				// Perform save to storage
				if (!IsAdmin)
					CustomPageManager.UpdateCustomPage(PageUid, newJsonData, templateUid, null, Mediachase.IBN.Business.Security.UserID);
				else
					CustomPageManager.UpdateCustomPage(PageUid, newJsonData, templateUid, ProfileId, UserId);

				divTemplateUpdate.Style.Add(HtmlTextWriterStyle.Display, "block");
				this.ForcePostback = true;
			}
		}
		#endregion

		#region lblEditTemplate_Click
		/// <summary>
		/// Handles the Click event of the lblEditTemplate control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void lblEditTemplate_Click(object sender, EventArgs e)
		{
			Response.Redirect(
				String.Format(CultureInfo.InvariantCulture,
				"~/Apps/WidgetEngine/Pages/AddFrameTemplateEditor.aspx?PageUid={0}&IsAdmin={1}&ClassName={2}&ObjectId={3}&refreshName=workspaceGetRefreshLight&closeFramePopup=McCommandHandler_ClosePopup", 
				this.PageUid, 
				this.IsAdmin, 
				Request["ClassName"],
				Request["ObjectId"]));
		}
		#endregion

		#region repCategories_ItemDataBound
		/// <summary>
		/// Handles the ItemDataBound event of the repCategories control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.</param>
		void repCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (((DynamicCategoryInfo)e.Item.DataItem).Uid == this.CurrentGroup)
			{
				LinkButton lb = (LinkButton)e.Item.FindControl("lblGroup");
				HtmlGenericControl div = (HtmlGenericControl)e.Item.FindControl("divContainer");
				if (div != null)
				{
					div.Attributes.Add("class", "SelectedCategory");
				}
				else
				{
					div.Attributes.Add("class", "NonSelectedCategory");
				}
			}
		}
		#endregion

		#region repCategories_ItemCommand
		/// <summary>
		/// Handles the ItemCommand event of the repCategories control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
		void repCategories_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName == "Edit")
			{
				LinkButton lbl = (LinkButton)e.CommandSource;
				if (lbl != null)
				{
					this.CurrentGroup = lbl.CommandArgument;//lbl.Text;
					BindGrid();
					BindCategories();
				}
			}
		}
		#endregion

		#region Page_PreRender
		/// <summary>
		/// Handles the PreRender event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_PreRender(object sender, EventArgs e)
		{
			//if (!this.ForcePostback)
			//    btnCancel.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{}} return false;", Request["closeFramePopup"]));

			ApplyLocalization();
		} 
		#endregion

		#region btnCancel_ServerClick
		/// <summary>
		/// Handles the ServerClick event of the btnCancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnCancel_ServerClick(object sender, EventArgs e)
		{
			CommandParameters cp = new CommandParameters("MC_Workspace_AddControl");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion

		#region btnAddAndClose_ServerClick
		/// <summary>
		/// Handles the ServerClick event of the btnAddAndClose control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnAddAndClose_ServerClick(object sender, EventArgs e)
		{
			string keyTemplate = string.Format("WS_Personalize_Template_{0}", this.PageUid);
			this.ForcePostback = true;

			PerformSaveData();

			CommandParameters cp = new CommandParameters("MC_Workspace_AddControl");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion

		#region GetCheckedItems
		/// <summary>
		/// Gets the checked items.
		/// </summary>
		/// <returns></returns>
		private string[] GetCheckedItems()
		{
			StringBuilder retVal = new StringBuilder();

			foreach (DataGridItem item in grdMain.Items)
			{
				CheckBox cb = (CheckBox)item.FindControl("cbId");

				if (cb != null && cb.Checked)
				{
					retVal.AppendFormat("{0};", item.Cells[0].Text);
				}
			}

			return retVal.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
		}
		#endregion

		#region PerformSaveData
		/// <summary>
		/// Performs the save data.
		/// </summary>
		private void PerformSaveData()
		{
			string[] Ids = GetCheckedItems();

			List<CpInfo> userList = new List<CpInfo>();

			CustomPageEntity page = null;
			if (!this.IsAdmin)
				page = CustomPageManager.GetCustomPage(PageUid, null, Mediachase.IBN.Business.Security.CurrentUser.UserID);
			else
				page = CustomPageManager.GetCustomPage(PageUid, ProfileId, UserId);
			string userData = page.JsonData;
			Guid templateUid = page.TemplateId;

			userList = UtilHelper.JsonDeserialize<List<CpInfo>>(userData.Replace("\\", ""));

			foreach (CpInfo cpItem in userList)
			{
				if (cpItem.Id == this.ColumnId)
				{
					foreach (string id in Ids)
					{
						CpInfoItem newItem = new CpInfoItem();
						newItem.Collapsed = "false";
						newItem.Id = id;
						newItem.InstanseUid = Guid.NewGuid().ToString("N");
						cpItem.Items.Add(newItem);
					}
				}
			}

			string newJsonData = UtilHelper.JsonSerialize(userList);

			//Perform save to storage
			if (!IsAdmin)
				CustomPageManager.UpdateCustomPage(PageUid, newJsonData, templateUid, null, Mediachase.IBN.Business.Security.UserID);
			else
				CustomPageManager.UpdateCustomPage(PageUid, newJsonData, templateUid, ProfileId, UserId);
		}
		#endregion

		#region PerformSaveTemplate
		/// <summary>
		/// Performs the save template.
		/// </summary>
		private void PerformSaveTemplate()
		{
			CustomPageEntity page = null;
			if (!IsAdmin)
				page = CustomPageManager.GetCustomPage(PageUid, null, Mediachase.IBN.Business.Security.CurrentUser.UserID);
			else
				page = CustomPageManager.GetCustomPage(PageUid, ProfileId, UserId);

			if (!IsAdmin)
				CustomPageManager.UpdateCustomPage(PageUid, page.JsonData, new Guid(this.selectedUid), null, Mediachase.IBN.Business.Security.UserID);
			else
				CustomPageManager.UpdateCustomPage(PageUid, page.JsonData, new Guid(this.selectedUid), ProfileId, UserId);
		}
		#endregion

		#region BindGrid
		/// <summary>
		/// Binds the grid.
		/// </summary>
		private void BindGrid()
		{
			DataTable dt = GenerateSource();
			DataView dv = dt.DefaultView;
			if (!String.IsNullOrEmpty(this.CurrentGroup) && this.CurrentGroup != Guid.Empty.ToString())
				dv.RowFilter = string.Format("Category = '{0}'", this.CurrentGroup);
			else
				dv.RowFilter = string.Empty;

			dv.Sort = "Title ASC";
			grdMain.DataSource = dv;
			grdMain.DataBind();
		}
		#endregion

		#region BindCategories
		/// <summary>
		/// Binds the categories.
		/// </summary>
		void BindCategories()
		{
			repCategories.DataSource = DynamicCategoryInfoLoader.Load(); //dt;
			repCategories.DataBind();
		}
		#endregion

		#region ApplyLocalization
		/// <summary>
		/// Applies the localization.
		/// </summary>
		private void ApplyLocalization()
		{

			btnAddAndClose.Text = CHelper.GetResFileString("{IbnFramework.WidgetEngine:_mc_AddSelected}");
			btnAddAndClose.CustomImage = this.ResolveUrl("~/Images/IbnFramework/saveclose.gif");

			btnCancel.Text = CHelper.GetResFileString("{IbnFramework.Global:_mc_Close}");
			btnCancel.CustomImage = this.ResolveUrl("~/Images/IbnFramework/deny.gif");

			btnAddAndClose.Attributes.Add("class", "floatRight");
			btnCancel.Attributes.Add("class", "floatRight");
		}
		#endregion

		#region GenerateSource
		/// <summary>
		/// Generates the source.
		/// </summary>
		/// <returns></returns>
		private DataTable GenerateSource()
		{
			DataTable retVal = new DataTable();

			retVal.Columns.Add(new DataColumn("Id", typeof(string)));
			retVal.Columns.Add(new DataColumn("Title", typeof(string)));
			retVal.Columns.Add(new DataColumn("Description", typeof(string)));
			retVal.Columns.Add(new DataColumn("Category", typeof(string)));
			retVal.Columns.Add(new DataColumn("IconPath", typeof(string)));
			//retVal.Columns.Add(new DataColumn("ShowInfo", typeof(int)));

			foreach (DynamicControlInfo dci in DynamicControlFactory.GetControlInfos())
			{
				DataRow row = retVal.NewRow();

				row["Id"] = dci.Uid;
				row["Title"] = CommonHelper.GetResFileString(dci.Title);
				row["Description"] = CommonHelper.GetResFileString(dci.Description);
				row["Category"] = CommonHelper.GetResFileString(dci.Category);
				if (!String.IsNullOrEmpty(dci.IconPath))
					row["IconPath"] = this.ResolveUrl(dci.IconPath);
				else
					row["IconPath"] = this.ResolveUrl("~/Images/IbnFramework/nologo.png");
				//row["ShowInfo"] = GetShowInfo(dci.Uid);

				retVal.Rows.Add(row);
			}

			return retVal;
		}
		#endregion

		#region BindToolbar
		/// <summary>
		/// Binds the toolbar.
		/// </summary>
		private void BindToolbar()
		{
			secControls.AddText(CHelper.GetResFileString("{IbnFramework.Global:_mc_ControlsSettings}"));
		}
		#endregion
	}
}
