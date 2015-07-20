using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Modules
{
	public partial class WsViewer : System.Web.UI.UserControl
	{
		UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strCustomizer", typeof(WsViewer).Assembly);

		#region prop: PageUid
		/// <summary>
		/// Gets or sets the page uid.
		/// </summary>
		/// <value>The page uid.</value>
		public Guid PageUid
		{
			get
			{
				if (IsAdmin)
				{
					return new Guid(Request["PageUid"]);
				}
				else
				{
					if (ViewState["_PageUid"] != null)
						return (Guid)ViewState["_PageUid"];
					else
						return new Guid(Request["PageUid"]);
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

				if (!this.IsAdmin)
				{
					return Mediachase.IBN.Business.Security.UserID;
				}
				else
				{
					HttpRequest request = HttpContext.Current.Request;
					if (String.Compare(request["ClassName"], "Principal", true) == 0
						&& !String.IsNullOrEmpty(request["ObjectId"]))
						retval = int.Parse(request["ObjectId"]);
				}

				return retval;
			}
		}
		#endregion

		#region LoadScripts
		/// <summary>
		/// Loads the scripts.
		/// </summary>
		void LoadScripts()
		{
			ScriptManager.GetCurrent(this.Page).ScriptMode = ScriptMode.Release;

			ScriptManager.GetCurrent(this.Page).EnablePageMethods = true;
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

			string valueData = string.Empty;
			string valueTemplate = string.Empty;

			CustomPageEntity page = null;
			if (!this.IsAdmin)
			{
				page = CustomPageManager.GetCustomPage(PageUid, null, (PrimaryKeyId)Mediachase.IBN.Business.Security.CurrentUser.UserID);
			}
			else
			{
				page = CustomPageManager.GetCustomPage(PageUid, ProfileId, UserId);
//				CHelper.AddToContext("pageUid", this.PageUid);
			}

			cpManager.DataSource = page.TemplateId.ToString();

			cpManager.PageUid = this.PageUid;
			cpManagerExtender.PageUid = this.PageUid;
			cpManagerExtender.IsAdmin = this.IsAdmin;
			LayoutContextKey key = new LayoutContextKey(this.PageUid, this.IsAdmin);
			if (IsAdmin)
			{
				key.ProfileId = ProfileId;
				key.UserId = UserId;
			}

			cpManagerExtender.ContextKey = key;
			cpManager.IsAdmin = this.IsAdmin;
			if (!IsPostBack)
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
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");
			//ClientScript.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString("N"), string.Format("function run_MC_Workspace_EditTemplate() {{ eval('{0}'); }}", cmd2), true);

			Control divContainer = CommandManager.GetCurrent(this.Page).Parent.FindControl(CommandManager.GetCurrent(this.Page).ContainerId);
			if (divContainer != null)
				cpManagerExtender.ContainerId = divContainer.ClientID;

			CommandParameters cp = new CommandParameters("MC_Workspace_PropertyPage");
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add("controlUid", "%controlUid%");
			
			//dvs: new feature for ControlProperties provider [2009-06-17]
			dic.Add("PageUid", this.PageUid.ToString());
			
			if (this.ProfileId.HasValue)
				dic.Add("ProfileId", this.ProfileId.Value.ToString());
			else
				dic.Add("ProfileId", "null");

			if (this.UserId.HasValue)
				dic.Add("UserId", this.UserId.Value.ToString());
			else
				dic.Add("UserId", "null");

			cp.CommandArguments = dic;

			cpManagerExtender.PropertyPageCommand = CommandManager.GetCurrent(this.Page).AddCommand(string.Empty, string.Empty, "Workspace", cp);
			cpManagerExtender.DeleteMessage = CHelper.GetResFileString("{IbnFramework.Global:_mc_RemoveBlock}");

			LoadScripts();
		}
		#endregion
	}
}