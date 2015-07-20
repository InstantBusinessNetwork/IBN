using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.Apps.WidgetEngine
{
	#region class: WsButton
	public class WsButton
	{
		#region prop: ClientId
		private string _clientId;
		public string ClientId
		{
			get { return _clientId; }
			set { _clientId = value; }
		}
		#endregion

		#region prop: ButtonType
		private string _buttonType;
		public string ButtonType
		{
			get { return _buttonType; }
			set { _buttonType = value; }
		}
		#endregion

		#region prop: IsSystem
		private bool _isSystem;
		/// <summary>
		/// Gets or sets a value indicating whether this instance is system.
		/// If true, visible even in read only mode
		/// </summary>
		/// <value><c>true</c> if this instance is system; otherwise, <c>false</c>.</value>
		public bool IsSystem
		{
			get { return _isSystem; }
			set { _isSystem = value; }
		}
		#endregion

		#region .ctor
		public WsButton()
		{
		}

		public WsButton(string ClientId)
			: this()
		{
			this.ClientId = ClientId;
		}

		public WsButton(string ClientId, string ButtonType)
			: this(ClientId)
		{
			this.ButtonType = ButtonType;
		}

		public WsButton(string ClientId, string ButtonType, bool IsSystem)
			: this(ClientId, ButtonType)
		{
			this.IsSystem = IsSystem;
		}
		#endregion
	} 
	#endregion

	public class IbnControlPlace : CompositeDataBoundControl
	{

		#region prop: controlsJson
		private string controlsJson
		{
			get
			{
				if (ViewState["__controlsJson"] != null)
					return ViewState["__controlsJson"].ToString();

				return string.Empty;
			}
			set
			{
				ViewState["__controlsJson"] = value;
			}
		} 
		#endregion

		#region prop: WidthPercentage
		public int WidthPercentage
		{
			get
			{
				if (ViewState["_WidthPercentage"] != null)
					return Convert.ToInt32(ViewState["_WidthPercentage"].ToString());

				return 50;
			}
			set { ViewState["_WidthPercentage"] = value; }
		}
		#endregion

		#region prop: ControlPlaceId
		public string ControlPlaceId
		{
			get
			{
				if (ViewState["__ControlPlaceId"] != null)
					return ViewState["__ControlPlaceId"].ToString();

				return string.Empty;
			}
			set
			{
				ViewState["__ControlPlaceId"] = value;
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
				if (ViewState["_IsAdmin"] != null)
					return Convert.ToBoolean(ViewState["_IsAdmin"].ToString(), CultureInfo.InvariantCulture);

				return false;
			}
			set
			{
				ViewState["_IsAdmin"] = value;
			}
		}
		#endregion

		#region prop: PageUid
		/// <summary>
		/// Gets or sets the page uid.
		/// </summary>
		/// <value>The page uid.</value>
		public Guid PageUid
		{
			get
			{
				if (ViewState["_PageUid"] != null)
					return (Guid)ViewState["_PageUid"];

				return Guid.Empty;
			}
			set
			{
				ViewState["_PageUid"] = value;
			}
		}
		#endregion

		#region bindControl
		/// <summary>
		/// Binds the control.
		/// </summary>
		/// <param name="dci">The dci.</param>
		/// <returns></returns>
		private string bindControl(DynamicControlInfo dci, string collapsed, int counter, string instanseUid, ref string sourceInfo)
		{
			StringBuilder sb = new StringBuilder();
			Control ctrl;

			if (dci != null)
				ctrl = DynamicControlFactory.Create(this.Page, dci.Uid);
			else
				ctrl = this.Page.LoadControl("~/" + dci.Uid.Replace("..\\", string.Empty).Replace("\\", "/"));

			//esli kontrol udalili, a v preferensah on ostalsa to nichego ne delaem
			if (ctrl == null)
				return string.Empty;

			HtmlGenericControl divContainer = new HtmlGenericControl("div"); 
			HtmlGenericControl divHeader = new HtmlGenericControl("div");

			bool isAdmin = IbnControlPlaceManager.GetCurrent(this.Page).IsAdmin;
			bool checkPersonalization = Mediachase.Ibn.Business.Customization.ProfileManager.CheckPersonalization();

			#region Image ExpandCollapse
			ImageButton imgOpen = new ImageButton();
			imgOpen.ID = String.Format("imgOpen_{0}", instanseUid/*counter*/);
			if (!Convert.ToBoolean(collapsed, CultureInfo.InvariantCulture))
			{
				imgOpen.ImageUrl = this.ResolveUrl("~/Images/IbnFramework/btn_up.gif");
				imgOpen.Attributes.Add("changeUrl", this.ResolveUrl("~/Images/IbnFramework/btn_down.gif"));
			}
			else
			{
				imgOpen.ImageUrl = this.ResolveUrl("~/Images/IbnFramework/btn_down.gif");
				imgOpen.Attributes.Add("changeUrl", this.ResolveUrl("~/Images/IbnFramework/btn_up.gif"));
			}
			imgOpen.Attributes.Add("class", "IbnHeaderWidgetButton");

			if (!(isAdmin || checkPersonalization))
			{
				imgOpen.Style.Add("right", "5px");
			}
			else
			{
				imgOpen.Style.Add("right", "25px");
			}
			imgOpen.OnClientClick = "return false;";
			imgOpen.Visible = true;// (isAdmin || checkPersonalization);
			#endregion

			#region Image Close
			ImageButton imgClose = new ImageButton();
			imgClose.ID = String.Format("imgClose_{0}", instanseUid/*counter*/);
			imgClose.ImageUrl = this.ResolveUrl("~/Images/IbnFramework/btn_close.gif");
			imgClose.Visible = (isAdmin || checkPersonalization);
			imgClose.Attributes.Add("class", "IbnHeaderWidgetButton");
			imgClose.Style.Add("right", "5px");
			imgClose.OnClientClick = "return false;";
			#endregion

			#region Image PropertyPage
			ImageButton imgProperty = new ImageButton();
			imgProperty.ID = String.Format("imgProperty_{0}", instanseUid/*counter*/);
			imgProperty.ImageUrl = this.ResolveUrl("~/Images/IbnFramework/btn_prop.gif");
			imgProperty.Attributes.Add("class", "IbnHeaderWidgetButton");
			imgProperty.Style.Add("right", "45px");
			imgProperty.OnClientClick = "return false;";
			imgProperty.Visible = (isAdmin || checkPersonalization);
			#endregion

			#region Label Title
			Label lblTitle = new Label();
			lblTitle.CssClass = "x-panel-header IbnHeaderWidgetButton";
			lblTitle.Style.Add("left", "2px");
			lblTitle.Style.Add("top", "1px");
			lblTitle.Style.Add("right", "75px");
			lblTitle.Style.Add(HtmlTextWriterStyle.BorderWidth, "0px");
			#endregion

			List<WsButton> buttonList = new List<WsButton>();

			if ((isAdmin || checkPersonalization))
			{
				divHeader.Attributes.Add("class", "IbnWidgetHeader");
			}
			else
			{
				divHeader.Attributes.Add("class", "IbnWidgetHeaderNoCursor");
			}

			divHeader.Attributes.Add("dragObj", "0");

			ctrl.ID = String.Format("wrapControl{0}_{1}", dci.Uid.Replace("-", ""), instanseUid);
			IbnWidgetContainer c = new IbnWidgetContainer(ctrl, Convert.ToBoolean(collapsed, CultureInfo.InvariantCulture));
			c.ID = String.Format("id{0}{1}", dci.Uid.Replace("-", ""), instanseUid/*counter*/);

			divContainer.Controls.Add(divHeader);
			divContainer.Controls.Add(c);
			//this.Controls.Add(divHeader);
			this.Controls.Add(divContainer); 

			c.DataBind();

			sourceInfo += String.Format("{0}^{1}^{2}:", dci.Uid, collapsed, instanseUid); // _uid + ":";

			if (dci != null)
			{
				divHeader.Controls.Add(imgClose);
				divHeader.Controls.Add(imgOpen);
				divHeader.Controls.Add(lblTitle);
				buttonList.Add(new WsButton(imgClose.ClientID, "close", false));
				buttonList.Add(new WsButton(imgOpen.ClientID, "expand", true));
				if (ControlProperties.Provider.GetValue(ctrl.ID, ControlProperties._titleKey) != null)
				{
					lblTitle.Text = CHelper.GetResFileString(ControlProperties.Provider.GetValue(ctrl.ID, ControlProperties._titleKey).ToString());
				}
				else
				{
					lblTitle.Text = CHelper.GetResFileString(dci.Title);
				}

				if (collapsed == "true" && ControlProperties.Provider.GetValue(ctrl.ID, ControlProperties._countKey) != null)
				{
					lblTitle.Text += String.Format(" ({0})", ControlProperties.Provider.GetValue(ctrl.ID, ControlProperties._countKey).ToString());
				}

				if (!string.IsNullOrEmpty(dci.PropertyPagePath) || !string.IsNullOrEmpty(dci.PropertyPageType))
				{
					divHeader.Controls.Add(imgProperty);
					buttonList.Add(new WsButton(imgProperty.ClientID, "property", false));
					sb.AppendFormat("{{ title: '{2}', tools: layoutExtender_tools2, contentEl: '{0}', id:'{4}_{1}', collapsed:{3}, buttons:{5} }},", c.ClientID, instanseUid, CHelper.GetResFileString(dci.Title), collapsed, dci.Uid, UtilHelper.JsonSerialize<List<WsButton>>(buttonList));
				}
				else
				{
					sb.AppendFormat("{{ title: '{2}', tools: layoutExtender_tools, contentEl: '{0}', id:'{4}_{1}', collapsed:{3}, buttons:{5} }},", c.ClientID, instanseUid, CHelper.GetResFileString(dci.Title), collapsed, dci.Uid, UtilHelper.JsonSerialize<List<WsButton>>(buttonList));
					//if (Convert.ToBoolean(collapsed, CultureInfo.InvariantCulture))
					//    c.Style.Add(HtmlTextWriterStyle.Display, "none");
				}
			}
			else
			{
				sb.AppendFormat("{{ title: '', tools: layoutExtender_tools, contentEl: '{0}', id:'{3}_{1}', collapsed:{2} }},", c.ClientID, instanseUid/*dci.Uid.Replace("..\\", string.Empty).Replace("\\", "/")*/, collapsed, dci.Uid.Replace("..\\", string.Empty).Replace("\\", "/"));
			}

			return sb.ToString();
		} 
		#endregion

		#region CreateChildControls
		/// <summary>
		/// When overridden in an abstract class, creates the control hierarchy that is used to render the composite data-bound control based on the values from the specified data source.
		/// </summary>
		/// <param name="dataSource">An <see cref="T:System.Collections.IEnumerable"></see> that contains the values to bind to the control.</param>
		/// <param name="dataBinding">true to indicate that the <see cref="M:System.Web.UI.WebControls.CompositeDataBoundControl.CreateChildControls(System.Collections.IEnumerable,System.Boolean)"></see> is called during data binding; otherwise, false.</param>
		/// <returns>
		/// The number of items created by the <see cref="M:System.Web.UI.WebControls.CompositeDataBoundControl.CreateChildControls(System.Collections.IEnumerable,System.Boolean)"></see>.
		/// </returns>
		protected override int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
		{
			int counter = 0;
			controlsJson = string.Empty;

			string _sourceInfo = string.Empty;

			if (dataBinding)
			{
				foreach (string val in dataSource)
				{
					if (val == string.Empty || val.Split('^').Length != 3)
						continue;

					string _uid = val.Split('^')[0];
					string _collapsed = val.Split('^')[1].ToLowerInvariant();
					string _instanseUid = val.Split('^')[2].ToLowerInvariant();
					//if (string.IsNullOrEmpty(_instanseUid))
					//    _instanseUid = Guid.NewGuid().ToString("N");
					//TO DO: test InstanseUid

					counter++;

					DynamicControlInfo dci = DynamicControlFactory.GetControlInfo(_uid);
					
					//fix when user has deleted controls
					if (dci == null)
						continue;

					if (!(this.IsAdmin || Mediachase.Ibn.Business.Customization.ProfileManager.CheckPersonalization()))
					{
						List<CpInfo> list = new List<CpInfo>();
						UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
						if (pc["userCollapseExpand_" + this.PageUid.ToString("N")] != null)
						{
							list = UtilHelper.JsonDeserialize<List<CpInfo>>(pc["userCollapseExpand_" + this.PageUid.ToString("N")]);
							string _tmpCollapsed = FindCollapseInfo(list, _instanseUid);
							if (_tmpCollapsed != string.Empty)
								_collapsed = _tmpCollapsed;
						}
					}

					controlsJson += this.bindControl(dci, _collapsed, counter, _instanseUid, ref _sourceInfo);
				}

				if (_sourceInfo.Length > 0)
					_sourceInfo = _sourceInfo.Remove(_sourceInfo.Length - 1);

				this.ViewState["_sourceInfo"] = _sourceInfo;

			}
			else
			{
				if (this.ViewState["_sourceInfo"] == null)
					throw new ArgumentNullException("SourceInfo");

				_sourceInfo = this.ViewState["_sourceInfo"].ToString();

				if (_sourceInfo.Length == 0)
					return 0;

				foreach (string val in _sourceInfo.Split(':'))
				{
					if (val == string.Empty || val.Split('^').Length != 3)
						continue;

					string _uid = val.Split('^')[0];
					string _collapsed = val.Split('^')[1].ToLowerInvariant();
					string _instanseUid = val.Split('^')[2].ToLowerInvariant();

					counter++;

					DynamicControlInfo dci = DynamicControlFactory.GetControlInfo(_uid);
					
					if (!(this.IsAdmin || Mediachase.Ibn.Business.Customization.ProfileManager.CheckPersonalization()))
					{
						List<CpInfo> list = new List<CpInfo>();
						UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
						if (pc["userCollapseExpand_" + this.PageUid.ToString("N")] != null)
						{
							list = UtilHelper.JsonDeserialize<List<CpInfo>>(pc["userCollapseExpand_" + this.PageUid.ToString("N")]);
							string _tmpCollapsed = FindCollapseInfo(list, _instanseUid);
							if (_tmpCollapsed != string.Empty)
								_collapsed = _tmpCollapsed;
						}
					}

					controlsJson += this.bindControl(dci, _collapsed, counter, _instanseUid, ref _sourceInfo);
				}

			}

			if (controlsJson.Length > 0)
				controlsJson = controlsJson.Remove(controlsJson.Length - 1);

			return counter;
		} 
		#endregion

		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			IbnControlPlaceManager.GetCurrent(this.Page).ControlPlaces.Add(this);
			base.OnInit(e);			
			//this.EnsureChildControls();
		} 
		#endregion

		#region FindCollapseInfo
		/// <summary>
		/// Finds the collapse info.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="instanseUid">The instanse uid.</param>
		/// <returns></returns>
		private string FindCollapseInfo(List<CpInfo> list, string instanseUid)
		{
			if (String.IsNullOrEmpty(instanseUid) || list == null)
				return string.Empty;
			foreach (CpInfo ci in list)
			{
				foreach (CpInfoItem item in ci.Items)
				{
					if (item.InstanseUid == instanseUid)
						return item.Collapsed;
				}
			}

			return string.Empty;
		} 
		#endregion

		#region GetItemsJson
		/// <summary>
		/// Gets the items json.
		/// </summary>
		/// <returns></returns>
		public string GetItemsJson()
		{
			StringBuilder sb = new StringBuilder();

			if (this.Page.Request.Browser.Browser.Contains("IE"))
			{
				if (this.WidthPercentage < 100)
					sb.AppendFormat("{{ columnWidth:.{0}, style:'padding:0px 0px 0px 0px', id: '{1}', clientId: '{2}' ", this.WidthPercentage, this.ID, this.ClientID);
				else
					sb.AppendFormat("{{ columnWidth:1, style:'padding:5px 0px 5px 10px', id: '{0}', clientId: '{1}' ", this.ID, this.ClientID);
			}
			else
			{
				if (this.WidthPercentage < 100)
					sb.AppendFormat("{{ columnWidth:.{0}, style:'padding:5px 0px 5px 10px', id: '{1}', clientId: '{2}' ", this.WidthPercentage, this.ID, this.ClientID);
				else
					sb.AppendFormat("{{ columnWidth:1, style:'padding:5px 0px 5px 10px', id: '{0}', clientId: '{1}' ", this.ID, this.ClientID);
			}

			if (controlsJson != string.Empty)
				sb.AppendFormat(", items: [{0}]", controlsJson);
			sb.Append("}");

			return sb.ToString();
		} 
		#endregion

		#region OnLoad
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			//this.EnsureChildControls();
		} 
		#endregion

		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute("id", this.ClientID);
			writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.WidthPercentage + "%");
			writer.AddStyleAttribute("float", "left");
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "columnContainer");
			writer.RenderBeginTag("div");


			base.RenderChildren(writer);

			writer.RenderEndTag();
		}

	}
}
