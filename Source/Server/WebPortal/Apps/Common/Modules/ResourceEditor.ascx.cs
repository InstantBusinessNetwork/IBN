using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Globalization;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.Apps.Common.Modules
{
	public partial class ResourceEditor : System.Web.UI.UserControl
	{
		private readonly string _imgFailedUrlDefault = "~/Images/IbnFramework/icon_outcome_failed16.gif";
		private readonly string _imgSuccessUrlDefault = "~/Images/IbnFramework/success_icon16.gif";
		private readonly string _imgLoadingUrlDefault = "~/Images/IbnFramework/loading_icon16.gif";
		private readonly string _imgHelpUrlDefault = "~/Images/IbnFramework/help16.png";
		private readonly string _defaultResourceKey = "{Resource:Key}";
		private readonly string _emptyTextDefault = "{IbnFramework.Global:ResourceEdit}";
		private readonly int _defaultWidth = 100;

		#region prop: Text
		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		public string Text
		{
			get
			{
				return tbValue.Text;
			}
			set
			{
				tbValue.Text = value;
			}
		}
		#endregion

		#region prop: ContextKey
		/// <summary>
		/// Gets or sets the context key.
		/// </summary>
		/// <value>The context key.</value>
		public ResourceEditorContextKey ContextKey
		{
			get
			{
				if (ViewState["_ContextKey"] == null)
					return new ResourceEditorContextKey();

				return (ResourceEditorContextKey)ViewState["_ContextKey"];
			}
			set
			{
				ViewState["_ContextKey"] = value;
			}
		}
		#endregion

		#region prop: CloseTextBoxTimeout
		/// <summary>
		/// Gets or sets the close text box timeout.
		/// </summary>
		/// <value>The close text box timeout.</value>
		public int CloseTextBoxTimeout
		{
			get
			{
				if (ViewState["_CloseTextBoxTimeout"] == null)
					return 0;

				return Convert.ToInt32(ViewState["_CloseTextBoxTimeout"].ToString(), CultureInfo.InvariantCulture);
			}
			set
			{
				ViewState["_CloseTextBoxTimeout"] = value;
			}
		}
		#endregion

		#region prop: CheckValueTimeout
		/// <summary>
		/// Gets or sets the check value timeout.
		/// </summary>
		/// <value>The check value timeout.</value>
		public int CheckValueTimeout
		{
			get
			{
				if (ViewState["_CheckValueTimeout"] == null)
					return 0;

				return Convert.ToInt32(ViewState["_CheckValueTimeout"].ToString(), CultureInfo.InvariantCulture);
			}
			set
			{
				ViewState["_CheckValueTimeout"] = value;
			}
		}
		#endregion

		#region prop: WebServiceUrl
		/// <summary>
		/// Gets or sets the web service URL.
		/// </summary>
		/// <value>The web service URL.</value>
		public string WebServiceUrl
		{
			get
			{
				if (ViewState["_WebServiceUrl"] == null)
					return null;

				return ViewState["_WebServiceUrl"].ToString();
			}
			set
			{
				ViewState["_WebServiceUrl"] = value;
			}
		}
		#endregion

		#region prop: ImgFailedUrl
		/// <summary>
		/// Gets or sets the web service URL.
		/// </summary>
		/// <value>The web service URL.</value>
		public string ImgFailedUrl
		{
			get
			{
				if (ViewState["_ImgFailedUrl"] == null)
					return _imgFailedUrlDefault;

				return ViewState["_ImgFailedUrl"].ToString();
			}
			set
			{
				ViewState["_ImgFailedUrl"] = value;
			}
		}
		#endregion

		#region prop: ImgHelpUrl
		/// <summary>
		/// Gets or sets the img help URL.
		/// </summary>
		/// <value>The img help URL.</value>
		public string ImgHelpUrl
		{
			get
			{
				if (ViewState["_ImgHelpUrl"] == null)
					return _imgHelpUrlDefault;

				return ViewState["_ImgHelpUrl"].ToString();
			}
			set
			{
				ViewState["_ImgHelpUrl"] = value;
			}
		}
		#endregion

		#region prop: ImgLoadingUrl
		/// <summary>
		/// Gets or sets the img loading URL.
		/// </summary>
		/// <value>The img loading URL.</value>
		public string ImgLoadingUrl
		{
			get
			{
				if (ViewState["_ImgLoadingUrl"] == null)
					return _imgLoadingUrlDefault;

				return ViewState["_ImgLoadingUrl"].ToString();
			}
			set
			{
				ViewState["_ImgLoadingUrl"] = value;
			}
		}
		#endregion

		#region prop: ImgSuccessUrl

		/// <summary>
		/// Gets or sets the img success URL.
		/// </summary>
		/// <value>The img success URL.</value>
		public string ImgSuccessUrl
		{
			get
			{
				if (ViewState["_ImgSuccessUrl"] == null)
					return _imgSuccessUrlDefault;

				return ViewState["_ImgSuccessUrl"].ToString();
			}
			set
			{
				ViewState["_ImgSuccessUrl"] = value;
			}
		}
		#endregion

		#region prop: Tooltip

		/// <summary>
		/// Gets or sets the tooltip.
		/// </summary>
		/// <value>The tooltip.</value>
		public string Tooltip
		{
			get
			{
				if (ViewState["_Tooltip"] == null)
					return string.Empty;

				return ViewState["_Tooltip"].ToString();
			}
			set
			{
				ViewState["_Tooltip"] = value;
			}
		}
		#endregion

		#region prop: CssClassLabel

		/// <summary>
		/// Gets or sets the tooltip.
		/// </summary>
		/// <value>The tooltip.</value>
		public string CssClassLabel
		{
			get
			{
				if (ViewState["_CssClassLabel"] == null)
					return string.Empty;

				return ViewState["_CssClassLabel"].ToString();
			}
			set
			{
				ViewState["_CssClassLabel"] = value;
			}
		}
		#endregion

		#region prop: DefaultResourceKey

		/// <summary>
		/// Gets or sets the default resource key.
		/// </summary>
		/// <value>The default resource key.</value>
		public string DefaultResourceKey
		{
			get
			{
				if (ViewState["_DefaultResourceKey"] == null)
					return _defaultResourceKey;

				return ViewState["_DefaultResourceKey"].ToString();
			}
			set
			{
				ViewState["_DefaultResourceKey"] = value;
			}
		}
		#endregion

		#region prop: Width

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		/// <value>The width.</value>
		public int Width
		{
			get
			{
				if (ViewState["_Width"] == null)
					return _defaultWidth;

				return Convert.ToInt32(ViewState["_Width"].ToString(), CultureInfo.InvariantCulture);
			}
			set
			{
				ViewState["_Width"] = value;
			}
		}
		#endregion

		#region prop: Mode

		/// <summary>
		/// Gets or sets the mode.
		/// </summary>
		/// <value>The mode.</value>
		public ResourceEditorMode Mode
		{
			get
			{
				return tbValueExt.Mode;
			}
			set
			{
				tbValueExt.Mode = value;
			}
		}
		#endregion

		#region prop: CssClassFailed

		/// <summary>
		/// Gets or sets the CSS class failed.
		/// </summary>
		/// <value>The CSS class failed.</value>
		public string CssClassFailed
		{
			get
			{
				return tbValueExt.CssClassFailed;
			}
			set
			{
				tbValueExt.CssClassFailed = value;
			}
		}
		#endregion

		#region prop: CssClassSuccess

		/// <summary>
		/// Gets or sets the CSS class success.
		/// </summary>
		/// <value>The CSS class success.</value>
		public string CssClassSuccess
		{
			get
			{
				return tbValueExt.CssClassSuccess;
			}
			set
			{
				tbValueExt.CssClassSuccess = value;
			}
		}
		#endregion

		#region prop: EmptyText
		/// <summary>
		/// Gets or sets the empty text.
		/// </summary>
		/// <value>The empty text.</value>
		public string EmptyText
		{
			get
			{
				if (ViewState["_EmptyText"] == null)
					return CHelper.GetResFileString(_emptyTextDefault);

				return ViewState["_EmptyText"].ToString();
			}
			set
			{
				ViewState["_EmptyText"] = value;
			}
		}
		#endregion

		#region prop: CanBeEmpty

		/// <summary>
		/// Gets or sets a value indicating whether this instance can be empty.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance can be empty; otherwise, <c>false</c>.
		/// </value>
		public bool CanBeEmpty
		{
			get
			{
				if (ViewState["_CanBeEmpty"] == null)
					return true;
				return Convert.ToBoolean(ViewState["_CanBeEmpty"].ToString(), CultureInfo.InvariantCulture);
			}
			set
			{
				ViewState["_CanBeEmpty"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			fieldText.Enabled = this.CanBeEmpty;

			tbValueExt.DefaultResourceKey = this.DefaultResourceKey;
			tbValueExt.WebServiceUrl = ResolveUrl(this.WebServiceUrl);
			tbValueExt.ContextKey = this.ContextKey;
			tbValueExt.EmptyText = this.EmptyText;

			tbValueExt.ImgSuccess = imgSuccess.ClientID;
			tbValueExt.ImgLoading = imgLoading.ClientID;
			tbValueExt.ImgFailed = imgFailed.ClientID;
			tbValueExt.ImgHelp = imgHelp.ClientID;

			tbValueExt.CheckValueTimeout = this.CheckValueTimeout;
			tbValueExt.CloseTextBoxTimeout = this.CloseTextBoxTimeout;
			tbValueExt.CssClassLabel = this.CssClassLabel;

			tbValue.Width = Unit.Parse(string.Format("{0}px", this.Width), CultureInfo.InvariantCulture);

			imgFailed.ImageUrl = this.ImgFailedUrl;
			imgSuccess.ImageUrl = this.ImgSuccessUrl;
			imgLoading.ImageUrl = this.ImgLoadingUrl;
			imgHelp.ImageUrl = this.ImgHelpUrl;

			if (!String.IsNullOrEmpty(this.Tooltip))
				imgHelp.ToolTip = this.Tooltip;
		}

	}
}