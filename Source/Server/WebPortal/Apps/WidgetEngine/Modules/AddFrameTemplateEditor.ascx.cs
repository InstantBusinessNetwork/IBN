using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Business.Customization;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Modules
{
	public partial class AddFrameTemplateEditor : System.Web.UI.UserControl
	{
		private UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

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

		#region prop: selectedUid
		/// <summary>
		/// Gets or sets the selected uid.
		/// </summary>
		/// <value>The selected uid.</value>
		private Guid selectedUid
		{
			get
			{
				if (ViewState["__repTemplatesSelected"] != null)
					return (Guid)ViewState["__repTemplatesSelected"];

				return Guid.Empty;
			}
			set
			{
				ViewState["__repTemplatesSelected"] = value;
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

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				BindRepTemplates();

			repTemplates.ItemCommand += new RepeaterCommandEventHandler(repTemplates_ItemCommand);
			BindToolbar();
			lblAddControl.Click += new EventHandler(lblAddControl_Click);

			btnAddAndClose.Text = CHelper.GetResFileString("{IbnFramework.Global:_mc_SaveAndClose}");
			btnAddAndClose.CustomImage = this.ResolveUrl("~/Images/IbnFramework/saveclose.gif");
			btnAddAndClose.Style.Add(HtmlTextWriterStyle.Display, "inline");
			btnAddAndClose.ServerClick += new EventHandler(btnAddAndClose_ServerClick);
			btnAddAndClose.Attributes.Add("class", "floatRight");

			btnDefault.Attributes.Add("onclick", String.Format("if (!confirm('{0}')) return false;", CHelper.GetResFileString("{IbnFramework.Global:_mc_WorkspaceDefault}")));
			btnDefault.Text = CHelper.GetResFileString("{IbnFramework.Global:_mc_SetDefault}");
			btnDefault.CustomImage = this.ResolveUrl("~/Images/IbnFramework/refresh.gif");
			btnDefault.ServerClick += new EventHandler(btnDefault_ServerClick);
		}

		#region btnDefault_ServerClick
		/// <summary>
		/// Handles the ServerClick event of the btnDefault control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnDefault_ServerClick(object sender, EventArgs e)
		{
			if (!IsAdmin)
				CustomPageManager.ResetCustomPage(PageUid, null, Mediachase.IBN.Business.Security.CurrentUser.UserID);
			else
				CustomPageManager.ResetCustomPage(PageUid, ProfileId, UserId);

			//btnCancel.Attributes.Remove("onclick");
			CommandParameters cp = new CommandParameters("MC_Workspace_AddControl");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			this.ForcePostback = true;
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
			CustomPageEntity page = null;
			if (!this.IsAdmin)
				page = CustomPageManager.GetCustomPage(PageUid, null, Mediachase.IBN.Business.Security.CurrentUser.UserID);
			else
				page = CustomPageManager.GetCustomPage(PageUid, ProfileId, UserId);

			// if template is changed we should recalculate control places and save data
			if (page.TemplateId != this.selectedUid)
				TemplateChange(page);
			else // else just save data (to ensure that changes are made for appropriate layer)
				PerformSaveData(page);

			this.ForcePostback = true;

			CommandParameters cp = new CommandParameters("MC_Workspace_EditTemplate");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		} 
		#endregion

		#region TemplateChange
		/// <summary>
		/// Templates the change.
		/// </summary>
		/// <param name="page">The page.</param>
		private void TemplateChange(CustomPageEntity page)
		{
			string userSettings = page.JsonData;

			List<CpInfo> list = UtilHelper.JsonDeserialize<List<CpInfo>>(userSettings);

			WorkspaceTemplateInfo wti = WorkspaceTemplateFactory.GetTemplateInfo(this.selectedUid.ToString());

			if (wti == null)
				throw new NullReferenceException(String.Format("Cant find template: {0}", this.selectedUid));

			List<ColumnInfo> templateColumn = UtilHelper.JsonDeserialize<List<ColumnInfo>>(wti.ColumnInfo);

			if (templateColumn.Count == 0)
				throw new ArgumentException("Template must contain minumum 1 column");

			if (list.Count == 0)
				throw new ArgumentException("Invalid workspace personalize data");

			int counter = 0;
			ArrayList deletedItems = new ArrayList();
			foreach (CpInfo info in list)
			{
				if (!templateColumn.Contains(new ColumnInfo(info.Id)))
				{
					info.Id = templateColumn[0].Id;

					if (counter > 0)
					{
						foreach (CpInfoItem item in info.Items)
						{
							list[0].Items.Add(item);
						}

						deletedItems.Add(info);
					}
				}

				counter++;
			}

			if (deletedItems.Count > 0)
			{
				foreach (CpInfo info in deletedItems)
				{
					list.Remove(info);
				}
			}

			string jsonData = UtilHelper.JsonSerialize(list);
			if (!this.IsAdmin)
				CustomPageManager.UpdateCustomPage(PageUid, jsonData, this.selectedUid, null, Mediachase.IBN.Business.Security.UserID);
			else
				CustomPageManager.UpdateCustomPage(PageUid, jsonData, this.selectedUid, ProfileId, UserId);
		}
		#endregion

		#region PerformSaveData
		/// <summary>
		/// Performs the save data.
		/// </summary>
		/// <param name="page">The page.</param>
		private void PerformSaveData(CustomPageEntity page)
		{
			string userData = page.JsonData;
			Guid templateUid = page.TemplateId;

			List<CpInfo> userList = UtilHelper.JsonDeserialize<List<CpInfo>>(userData.Replace("\\", ""));

			string jsonData = UtilHelper.JsonSerialize(userList);

			//Perform save to storage
			if (!IsAdmin)
				CustomPageManager.UpdateCustomPage(PageUid, jsonData, templateUid, null, Mediachase.IBN.Business.Security.UserID);
			else
				CustomPageManager.UpdateCustomPage(PageUid, jsonData, templateUid, ProfileId, UserId);
		}
		#endregion

		#region lblAddControl_Click
		/// <summary>
		/// Handles the Click event of the lblAddControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void lblAddControl_Click(object sender, EventArgs e)
		{
			Response.Redirect(
				String.Format(CultureInfo.InvariantCulture,
				"~/Apps/WidgetEngine/Pages/AddFramePopup.aspx?PageUid={0}&IsAdmin={1}&ClassName={2}&ObjectId={3}", 
				this.PageUid, 
				this.IsAdmin,
				Request["ClassName"],
				Request["ObjectId"]));
		} 
		#endregion

		#region repTemplates_ItemCommand
		/// <summary>
		/// Handles the ItemCommand event of the repTemplates control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
		void repTemplates_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName == "Click")
			{
				this.selectedUid = new Guid(e.CommandArgument.ToString());
				BindRepTemplates();
			}
		}
		#endregion

		#region BindRepTemplates
		/// <summary>
		/// Binds the rep templates.
		/// </summary>
		private void BindRepTemplates()
		{
			if (this.selectedUid == Guid.Empty)
			{
				CustomPageEntity page = null;
				if (!this.IsAdmin)
					page = CustomPageManager.GetCustomPage(PageUid, null, Mediachase.IBN.Business.Security.CurrentUser.UserID);
				else
					page = CustomPageManager.GetCustomPage(PageUid, ProfileId, UserId);

				this.selectedUid = page.TemplateId;
			}

			repTemplates.DataSource = GenTemplatesDataSource();
			repTemplates.DataBind();

			foreach (RepeaterItem item in repTemplates.Items)
			{
				Button btn = (Button)item.FindControl("btnCommand");
				HtmlGenericControl div = (HtmlGenericControl)item.FindControl("mainItemDiv");

				if (btn != null && div != null)
				{
					if (new Guid(btn.CommandArgument.ToString()) == this.selectedUid)
						div.Attributes.Add("class", "customizeWSTemplateItemSelected");

					div.Attributes.Add("onclick", this.Page.ClientScript.GetPostBackEventReference(btn, btn.CommandArgument));
				}
			}
		}
		#endregion

		#region BindToolbar
		/// <summary>
		/// Binds the toolbar.
		/// </summary>
		private void BindToolbar()
		{
			secTemplate.AddText(CHelper.GetResFileString("{IbnFramework.Global:_mc_TemplateSettings}"));
		}
		#endregion

		#region GenTemplatesDataSource
		/// <summary>
		/// Gens the templates data source.
		/// </summary>
		/// <returns></returns>
		private DataTable GenTemplatesDataSource()
		{
			DataTable retVal = new DataTable();

			retVal.Columns.Add(new DataColumn("Id", typeof(string)));
			retVal.Columns.Add(new DataColumn("Title", typeof(string)));
			retVal.Columns.Add(new DataColumn("Description", typeof(string)));
			retVal.Columns.Add(new DataColumn("ImageUrl", typeof(string)));

			foreach (WorkspaceTemplateInfo wti in WorkspaceTemplateFactory.GetTemplateInfos())
			{
				DataRow row = retVal.NewRow();
				row["Id"] = wti.Uid;
				row["Title"] = CHelper.GetResFileString(wti.Title);
				row["Description"] = CHelper.GetResFileString(wti.Description);
				row["ImageUrl"] = CHelper.GetAbsolutePath(wti.ImageUrl);

				retVal.Rows.Add(row);
			}

			return retVal;
		}
		#endregion
	}
}