using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Globalization;
using Mediachase.IBN.Business.WidgetEngine;

namespace Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Modules
{
	public partial class PropertyPageContainer : UserControl, INamingContainer
	{
		#region prop: CancelElementId
		public string CancelElementId
		{
			get
			{
				return btnCancel.ClientID;
			}
		}
		#endregion

		#region prop: SaveElementId
		public string SaveElementId
		{
			get
			{
				return btnSaveReal.ClientID;
			}
		}
		#endregion

		#region prop: SaveCommand
		public string SaveCommand
		{
			get
			{
				return this.Page.ClientScript.GetPostBackEventReference(btnSaveReal, "");
			}
		}
		#endregion

		#region prop: PropertyControlUid
		/// <summary>
		/// Gets or sets the property control uid.
		/// </summary>
		/// <value>The property control uid.</value>
		public string PropertyControlUid
		{
			get
			{
				if (Request["propertyControlUid"] != null)
					return Request["propertyControlUid"].ToString();

				if (ViewState["__PropertyControlUid"] != null)
					return ViewState["__PropertyControlUid"].ToString();

				return string.Empty;
			}
			set
			{
				ViewState["__PropertyControlUid"] = value;
			}
		}
		#endregion

		#region prop: ProfileId
		protected int? ProfileId
		{
			get
			{
				int? retval = null;

				if (Request["ProfileId"] == null)
					throw new ArgumentNullException("ProfileId @ PropertyPageContainer.ascx");

				if (Request["ProfileId"] != "null")
					retval = Convert.ToInt32(Request["ProfileId"], CultureInfo.InvariantCulture);

				return retval;
			}
		}
		#endregion

		#region prop: UserId
		protected int? UserId
		{
			get
			{
				int? retval = null;

				if (Request["UserId"] == null)
					throw new ArgumentNullException("UserId @ PropertyPageContainer.ascx");

				if (Request["UserId"] != "null")
					retval = Convert.ToInt32(Request["UserId"], CultureInfo.InvariantCulture);

				return retval;
			}
		}
		#endregion

		#region prop: PageUid
		/// <summary>
		/// Gets or sets the page uid.
		/// </summary>
		/// <value>The page uid.</value>
		public string PageUid
		{
			get
			{
				if (Request["PageUid"] == null)
					throw new ArgumentNullException("PageUid @ PropertyPageContainer");

				return Request["PageUid"];
			}
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
			this.EnsureChildControls();
			btnSaveReal.Click += new EventHandler(btnSaveReal_Click);

			if (Request["closeFramePopup"] != null)
				btnCancel.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{}} return false;", Request["closeFramePopup"]));

			ApplyLocalization();
		} 
		#endregion

		#region Page_Init
		/// <summary>
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			if (GetCurrent(this.Page) != null)
			{
				throw new InvalidOperationException("PropertyPageContainer must be one per page");
			}
			this.Page.Items[typeof(PropertyPageContainer)] = this;

		} 
		#endregion

		#region GetCurrent
		/// <summary>
		/// Gets the current.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <returns></returns>
		public static PropertyPageContainer GetCurrent(Page page)
		{
			if (page == null)
			{
				throw new ArgumentNullException("page");
			}
			return (page.Items[typeof(PropertyPageContainer)] as PropertyPageContainer);

		} 
		#endregion

		#region HideBottomButtons
		/// <summary>
		/// Hides the bottom buttons.
		/// </summary>
		public void HideBottomButtons()
		{
			DockBottom.Height = 0;
			DockBottom.Visible = false;
		} 
		#endregion

		#region btnSaveReal_Click
		/// <summary>
		/// Handles the Click event of the btnSaveReal control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnSaveReal_Click(object sender, EventArgs e)
		{
			//TODO:
			foreach (Control c in mainContainer.Controls)
			{
				if (c is IPropertyPageControl)
				{
					(c as IPropertyPageControl).Save();
				}
			}

			CommandParameters cp = new CommandParameters("MC_Workspace_PropertyPage");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		} 
		#endregion

		#region CreateChildControls
		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			if (this.PropertyControlUid != string.Empty)
			{
				HttpContext.Current.Items[ControlProperties._pageUidKey] = this.PageUid;
				HttpContext.Current.Items[ControlProperties._profileUidKey] = this.ProfileId;
				HttpContext.Current.Items[ControlProperties._userUidKey] = this.UserId;

				Control c = DynamicControlFactory.CreatePropertyPage(this.Page, this.PropertyControlUid);
				c.ID = String.Format("wrapControl{0}", this.PropertyControlUid.Replace("-", string.Empty));
				mainContainer.Controls.Add(c);
			}
		} 
		#endregion

		#region ApplyLocalization
		/// <summary>
		/// Applies the localization.
		/// </summary>
		void ApplyLocalization()
		{
			btnSaveReal.Text = CHelper.GetResFileString("{IbnFramework.Global:_mc_Save}");
			btnCancel.Value = CHelper.GetResFileString("{IbnFramework.Global:_mc_Cancel}");
		} 
		#endregion
	}
}