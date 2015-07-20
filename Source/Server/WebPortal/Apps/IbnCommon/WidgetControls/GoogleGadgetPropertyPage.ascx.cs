using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Modules;

namespace Mediachase.UI.Web.Apps.IbnCommon.WidgetControls
{
	public partial class GoogleGadgetPropertyPage : System.Web.UI.UserControl
	{
		#region prop: currentId
		/// <summary>
		/// Gets the current id.
		/// </summary>
		/// <value>The current id.</value>
		private string currentId
		{
			get
			{
				if (ControlProperties.Provider.GetValue(this.ID, "PageSource") == null)
					return string.Empty;

				return ControlProperties.Provider.GetValue(this.ID, "PageSource").ToString();
			}
		} 
		#endregion

		#region prop: SearchKeyword
		/// <summary>
		/// Gets or sets the search keyword.
		/// </summary>
		/// <value>The search keyword.</value>
		public string SearchKeyword
		{
			get
			{
				if (ViewState["_SearchKeyword"] == null)
					return string.Empty;

				return ViewState["_SearchKeyword"].ToString();
			}
			set
			{
				ViewState["_SearchKeyword"] = value;
			}
		}
		#endregion

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			PropertyPageContainer ppc = PropertyPageContainer.GetCurrent(Page);
			if (ppc != null)
			{
				ppc.HideBottomButtons();
			}
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				ctrlGrid.DataBind();

			RegisterStyles();

			ctrlEventAction.GridId = ctrlGrid.GridClientContainerId;
			HttpContext.Current.Session["ControlId"] = this.ID;

			btnSearch.Click += new ImageClickEventHandler(btnSearch_Click);
			btnClear.Click += new ImageClickEventHandler(btnClear_Click);
		}

		#region btnClear_Click
		/// <summary>
		/// Handles the Click event of the btnClear control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
		void btnClear_Click(object sender, ImageClickEventArgs e)
		{
			this.SearchKeyword = string.Empty;
			ctrlGrid.SearchKeyword = this.SearchKeyword;
			tbSearch.Text = string.Empty;
			ctrlGrid.DataBind();
		} 
		#endregion

		#region btnSearch_Click
		/// <summary>
		/// Handles the Click event of the btnSearch control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
		void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			this.SearchKeyword = tbSearch.Text;
			ctrlGrid.SearchKeyword = this.SearchKeyword;
			ctrlGrid.DataBind();
		} 
		#endregion

		#region CurrentGadget
		/// <summary>
		/// Currents the gadget.
		/// </summary>
		/// <returns></returns>
		public string CurrentGadget()
		{
			if (currentId != string.Empty)
			{
				GoogleGadgetEntity gge = (GoogleGadgetEntity)BusinessManager.Load("GoogleGadget", PrimaryKeyId.Parse(currentId));
				if (gge != null)
					return CHelper.GetResFileString(gge.Title);
			}

			return CHelper.GetResFileString("{IbnFramework.WidgetEngine:_mc_DefaultGadget}");
		} 
		#endregion

		#region RegisterStyles
		/// <summary>
		/// Registers the styles.
		/// </summary>
		void RegisterStyles()
		{
			Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), Guid.NewGuid().ToString(),
				String.Format("<link type='text/css' rel='stylesheet' href='{0}' />", ResolveClientUrl("~/styles/IbnFramework/grid.css")));

		} 
		#endregion
	}
}