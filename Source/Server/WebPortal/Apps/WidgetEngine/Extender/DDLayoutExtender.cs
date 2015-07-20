using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Data;


namespace Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Extender
{
	[TargetControlType(typeof(IbnControlPlaceManager))]
	public class DDLayoutExtender : ExtenderControl
	{

		#region DeleteMessage
		/// <summary>
		/// Gets or sets the delete message.
		/// </summary>
		/// <value>The delete message.</value>
		[Bindable(true)]
		public string DeleteMessage
		{
			get
			{
				if (this.ViewState["__DeleteMessage"] != null)
					return this.ViewState["__DeleteMessage"].ToString();

				return string.Empty;
			}
			set
			{
				this.ViewState["__DeleteMessage"] = value;
			}
		}
		#endregion

		#region AddTemplateClientId
		/// <summary>
		/// Gets or sets the add template client id.
		/// </summary>
		/// <value>The add template client id.</value>
		private string AddTemplateClientId
		{
			get
			{
				if (this.ViewState["__AddTemplateClientId"] != null)
					return this.ViewState["__AddTemplateClientId"].ToString();

				return string.Empty;
			}
			set
			{
				this.ViewState["__AddTemplateClientId"] = value;
			}
		}
		#endregion

		#region PageUid
		/// <summary>
		/// Gets or sets the workspace page uid.
		/// </summary>
		/// <value>The ws page uid.</value>
		public Guid PageUid
		{
			get
			{
				if (this.ViewState["__PageUid"] != null)
					return (Guid)this.ViewState["__PageUid"];

				return Guid.Empty;
			}
			set
			{
				this.ViewState["__PageUid"] = value;
			}
		}
		#endregion

		#region IsAdmin

		/// <summary>
		/// Gets or sets a value indicating whether this instance is admin.
		/// </summary>
		/// <value><c>true</c> if this instance is admin; otherwise, <c>false</c>.</value>
		public bool IsAdmin
		{
			get
			{
				if (this.ViewState["__IsAdmin"] != null)
					return Convert.ToBoolean(this.ViewState["__IsAdmin"].ToString(), CultureInfo.InvariantCulture);

				return false;
			}
			set
			{
				this.ViewState["__IsAdmin"] = value;
			}
		}
		#endregion

		#region ContainerId
		/// <summary>
		/// Gets or sets the container id.
		/// </summary>
		/// <value>The container id.</value>
		public string ContainerId
		{
			get
			{
				if (this.ViewState["__ContainerId"] != null)
					return this.ViewState["__ContainerId"].ToString();

				return string.Empty;
			}
			set
			{
				this.ViewState["__ContainerId"] = value;
			}
		}
		#endregion

		#region PropertyPageCommand
		[Bindable(true)]
		public string PropertyPageCommand
		{
			get
			{
				if (this.ViewState["__PropertyPageCommand"] != null)
					return this.ViewState["__PropertyPageCommand"].ToString();

				return string.Empty;
			}
			set
			{
				this.ViewState["__PropertyPageCommand"] = value;
			}
		}
		#endregion

		#region CreateChildControls
		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			UserControl c = (UserControl)this.Page.LoadControl("~/Apps/WidgetEngine/Modules/AddTemplate.ascx");

			c.ID = "ctrlAddTemplate";
			c.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, "none");
			((Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Modules.AddTemplate)c).IsAdmin = this.IsAdmin;
			//c.Attributes.Add("wsPageUid", this.PageUid);

			if (c != null)
			{
				this.Controls.Add(c);
			}

			this.AddTemplateClientId = c.ClientID;
		}
		#endregion

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.EnsureChildControls();
		}

		#region implementation ExtenderControl
		protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors(Control targetControl)
		{
			ScriptControlDescriptor descriptor = new ScriptControlDescriptor("Ibn.DDLayoutExtender", targetControl.ClientID);

			descriptor.AddProperty("jsonItems", ((IbnControlPlaceManager)targetControl).JsonItems.Replace(" ", string.Empty));
			//descriptor.AddProperty("popupElementId", this.UpdateId);
			if (!String.IsNullOrEmpty(this.ContainerId))
				descriptor.AddElementProperty("popupElement", this.ContainerId);

			//descriptor.AddProperty("cancelId", this.CancelElementId);
			//descriptor.AddProperty("saveId", this.SaveElementId);
			//descriptor.AddProperty("saveCommand", this.SaveCommand);
			descriptor.AddProperty("deleteMsg", this.DeleteMessage);
			descriptor.AddProperty("propertyCommand", this.PropertyPageCommand);

			descriptor.AddProperty("addElementContainer", this.AddTemplateClientId);
			descriptor.AddProperty("wsPageUid", this.PageUid);
			descriptor.AddProperty("contextKey", UtilHelper.JsonSerialize(new LayoutContextKey(this.PageUid, this.IsAdmin)));

			return new ScriptDescriptor[] { descriptor };
		}

		protected override IEnumerable<ScriptReference> GetScriptReferences()
		{
			ScriptReference reference = new ScriptReference();

			reference.Path = McScriptLoader.Current.GetScriptUrl("~/Scripts/IbnFramework/DDLayoutExtender.js", this.Page);

			return new ScriptReference[] { reference };
		}
		#endregion
	}
}
