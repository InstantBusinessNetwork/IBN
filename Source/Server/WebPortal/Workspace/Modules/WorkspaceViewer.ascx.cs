using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Globalization;
using System.Resources;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Business.Customization;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Workspace.Modules
{
	public partial class WorkspaceViewer : System.Web.UI.UserControl
	{
		UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strCustomizer", typeof(Workspaceview2).Assembly);

		#region prop: PageUid
		/// <summary>
		/// Gets or sets the page uid.
		/// </summary>
		/// <value>The page uid.</value>
		public string PageUid
		{
			get
			{
				if (IsAdmin)
				{
					if (Request["PageUid"] != null)
						return Request["PageUid"];
					else
						return "WsMain";
				}
				else
				{
					if (ViewState["_PageUid"] == null)
						return string.Empty;

					return ViewState["_PageUid"].ToString();
				}
			}
			set
			{
				ViewState["_PageUid"] = value;
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
				if (ViewState["_IsAdmin"] == null)
					return false;

				return Convert.ToBoolean(ViewState["_IsAdmin"].ToString(), CultureInfo.InvariantCulture);
			}
			set
			{
				ViewState["_IsAdmin"] = value;
			}
		} 
		#endregion
		
		#region LoadScripts
		/// <summary>
		/// Loads the scripts.
		/// </summary>
		void LoadScripts()
		{
			ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference(McScriptLoader.Current.GetScriptUrl("~/Scripts/IbnFramework/Portal.js", this.Page)));
			ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference(McScriptLoader.Current.GetScriptUrl("~/Scripts/IbnFramework/PortalColumn.js", this.Page)));
			ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference(McScriptLoader.Current.GetScriptUrl("~/Scripts/IbnFramework/Portlet.js", this.Page)));
			ScriptManager.GetCurrent(this.Page).ScriptMode = ScriptMode.Release;

			ScriptManager.GetCurrent(this.Page).EnablePageMethods = true;
		}
		#endregion

		#region ProfileId
		protected int ProfileId
		{
			get
			{
				int retval = -1;
				if (!String.IsNullOrEmpty(Request["ObjectId"]))
					retval = int.Parse(Request["ObjectId"], CultureInfo.InvariantCulture);
				return retval;
			}
		}
		#endregion

		#region OnInit
		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			
			string keyData = string.Format("WS_Personalize_Data_{0}", this.PageUid);
			string keyTemplate = string.Format("WS_Personalize_Template_{0}", this.PageUid);

			if (!this.IsAdmin)
			{
				#region UserBind
				if (pc[keyTemplate] == null || pc[keyData] == null)
				{
					//todo: geyg constant info from page provider
					using (IDataReader r = Mediachase.IBN.Business.Common.GetWorkspaceSettings(DashboardPageProviderBase.GetPageWorkspaceUid(this.PageUid), ProfileManager.GetProfileIdByUser()))
					{
						if (r.Read())
						{
							pc[keyData] = r["JsonData"].ToString().Replace("\\", "");
							pc[keyTemplate] = r["TemplateUid"].ToString();
						}
					}
				}

				cpManager.DataSource = pc[keyTemplate]; 
				#endregion
			}
			else
			{
				#region AdminBind
				string _uid = DashboardPageProviderBase.GetPageWorkspaceUid(this.PageUid);
				if (!String.IsNullOrEmpty(_uid))
				{
					using (IDataReader reader = Mediachase.IBN.Business.Common.GetWorkspaceSettings(_uid, ProfileId))
					{
						if (reader.Read())
						{
							cpManager.DataSource = reader["TemplateUid"].ToString();
						}
					}
				}
				#endregion

				CHelper.AddToContext("pageUid", this.PageUid);
			}

			cpManager.PageUid = this.PageUid;
			cpManagerExtender.PageUid = this.PageUid;
			cpManagerExtender.IsAdmin = this.IsAdmin;
			cpManager.IsAdmin = this.IsAdmin;
			cpManager.DataBind();
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
			Control divContainer = CommandManager.GetCurrent(this.Page).Parent.FindControl(CommandManager.GetCurrent(this.Page).ContainerId);
			if (divContainer != null)
				cpManagerExtender.ContainerId = divContainer.ClientID;

			CommandParameters cp = new CommandParameters("MC_Workspace_PropertyPage");
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add("controlUid", "%controlUid%");
			cp.CommandArguments = dic;

			cpManagerExtender.PropertyPageCommand = CommandManager.GetCurrent(this.Page).AddCommand(string.Empty, string.Empty, "Workspace", cp);
			cpManagerExtender.DeleteMessage = CHelper.GetResFileString("{IbnFramework.Global:_mc_RemoveBlock}");

			LoadScripts();
		} 
		#endregion
	}
}