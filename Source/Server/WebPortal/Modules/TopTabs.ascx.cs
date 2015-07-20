namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Text;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for TopTabs.
	/// </summary>

	#region class: TopTabsEventArgs
	public sealed class TopTabsEventArgs
	{
		#region prop: ServerStorageValue
		private string serverStorageValue;

		public string ServerStorageValue
		{
			get { return serverStorageValue; }
			set { serverStorageValue = value; }
		}
		#endregion

		public TopTabsEventArgs(string serverStorageValue)
		{
			this.serverStorageValue = serverStorageValue;
		}
	}
	#endregion

	public partial  class TopTabs : UserControl
	{

		private ArrayList TabItems = new ArrayList();

		#region TabsCount
		public int TabsCount
		{
			get
			{
				return TabItems.Count;
			}
		}
		#endregion

		#region BaseUrl
		string _BaseUrl = "#";
		public string BaseUrl
		{
			get
			{
				return _BaseUrl;
			}
			set
			{
				_BaseUrl = value;
			}
		}
		#endregion

		#region TabWidth
		string _TabWidth = "120";
		public string TabWidth
		{
			get
			{
				return ((int)(int.Parse(_TabWidth) - 10)).ToString();
			}
			set
			{
				_TabWidth = value;
			}
		}
		#endregion

		#region IsLeftLine
		private bool _isleftLine = true;
		public bool IsLeftLine
		{
			get
			{
				return _isleftLine;
			}
			set
			{
				_isleftLine = value;
			}
		}
		#endregion

		#region IsLeftGap
		private bool _isLeftGap = true;
		public bool IsLeftGap
		{
			get
			{
				return _isLeftGap;
			}
			set
			{
				_isLeftGap = value;
			}
		}
		#endregion

		#region IsDesignMode
		private bool _isDesignMode = false;
		public bool IsDesignMode
		{
			get
			{
				return _isDesignMode;
			}
			set
			{
				_isDesignMode = value;
				_isPostMode = value;
			}
		}
		#endregion

		#region IsSelectable
		private bool _isSelectable = false;
		public bool IsSelectable
		{
			get
			{
				return _isSelectable;
			}
			set
			{
				_isSelectable = value;
			}
		}
		#endregion

		#region IsPostMode
		private bool _isPostMode = false;
		public bool IsPostMode
		{
			get
			{
				return _isPostMode;
			}
			set
			{
				_isPostMode = value;
			}
		}
		#endregion

		#region UseTabWidth
		private bool _useTabWidth = true;
		public bool UseTabWidth
		{
			get
			{
				return _useTabWidth;
			}
			set
			{
				_useTabWidth = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/TopTabs.css");

			this.lbSelectTab.Click += new EventHandler(lbSelectTab_Click);
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (IsLeftGap)
			{
				HtmlTableCell htcgap1 = new HtmlTableCell();
				htcgap1.Attributes.Add("class", "ibn-stylebox tabGap");
				htcgap1.InnerHtml = "&nbsp;";

				HtmlTableCell htcgap2 = new HtmlTableCell();
				htcgap2.Attributes.Add("class", "ibn-stylebox tabGap1");
				htcgap2.InnerHtml = "&nbsp;";
				if (!_isleftLine)
				{
					htcgap1.Style.Add("border-left", "0px;");
					htcgap2.Style.Add("border-left", "0px;");
				}
				trTabs.Cells.Add(htcgap1);
				trTabs1.Cells.Add(htcgap2);
			}

			for (int i = 0; i < TabItems.Count; i++)
			{
				TabItem ti = (TabItem)TabItems[i];
				string index = "";
				if (ti.key == "")
					index = i.ToString();
				else
					index = ti.key;

				string sPost = this.Page.ClientScript.GetPostBackEventReference(lbSelectTab, index);

				HtmlTableCell cell = new HtmlTableCell();

				if (UseTabWidth)
				{
					cell.Style.Add("min-width", _TabWidth + "px");

					if (Request.Browser.Browser.IndexOf("IE") >= 0 && Request.Browser.MajorVersion <= 7)
					{
						cell.Style.Add("width", _TabWidth + "px");
						cell.NoWrap = true;
					}
				}

				HtmlGenericControl content = new HtmlGenericControl("span");
				content.Style.Add("white-space", "nowrap");

				cell.Controls.Add(content);

				if (ti.selected)
				{
					cell.Attributes.Add("class", "ibn-stylebox tabSelected");

					if (IsPostMode)
					{
						if (_isSelectable)
							content.InnerHtml = "&nbsp;<a class='tabTLink' href=\"javascript:" + sPost + "\">" + ti.text + "</a>&nbsp;";
						else
						{
							content.InnerHtml = "&nbsp;" + ti.text + "&nbsp;";
							if (_isDesignMode)
								cell.Style.Add("border", "2px solid #8DD485");
						}
					}
					else
						content.InnerHtml = "&nbsp;" + ti.text + "&nbsp;";
				}
				else
				{
					cell.Attributes.Add("class", "ibn-stylebox ibn-navframe tab");

					string tab = "?Tab=";
					if (ti.link.IndexOf("?") >= 0)
						tab = "&amp;Tab=";

					if (IsPostMode)
						content.InnerHtml = "&nbsp;<a class='tabTLink' href=\"javascript:" + sPost + "\">" + ti.text + "</a>&nbsp;";
					else
						content.InnerHtml = "&nbsp;<a class='tabTLink' href='" + ti.link + tab + index + "'>" + ti.text + "</a>&nbsp;";
				}

				HtmlTableCell htcgap = new HtmlTableCell();
				htcgap.Attributes.Add("class", "ibn-stylebox tabGap");
				htcgap.InnerHtml = "&nbsp;";

				if (ti.row == 1)
				{
					trTabs.Cells.Add(cell);
					trTabs.Cells.Add(htcgap);
				}
				else
				{
					trTabs1.Cells.Add(cell);
					trTabs1.Cells.Add(htcgap);
				}
			}

			// last cell (row 1)
			HtmlTableCell htcendgap = new HtmlTableCell();
			htcendgap.Attributes.Add("class", "ibn-stylebox tabGap");
			htcendgap.Style.Add(HtmlTextWriterStyle.Width, "100%");
			htcendgap.Style.Add(HtmlTextWriterStyle.TextAlign, "right");
			htcendgap.Style.Add(HtmlTextWriterStyle.PaddingRight, "10px");

			TabItem selectedTab = GetSelectedItem();
			if (selectedTab != null && !Mediachase.IBN.Business.Security.CurrentUser.IsExternal)
			{
				string tab = "?Tab=";
				if (selectedTab.link.IndexOf("?") >= 0)
					tab = "&amp;Tab=";

				htcendgap.InnerHtml = String.Format(CultureInfo.InvariantCulture,
					"<a href='javascript:copy(\"{0}{1}{2}\")' class='text'>{3}</a>",
					GetFullPath(selectedTab.link),
					tab,
					selectedTab.key,
					GetGlobalResourceObject("IbnFramework.Common", "CopyLink").ToString());
			}
			else
			{
				htcendgap.InnerHtml = "&nbsp;";
			}

			// last cell (row 2)
			HtmlTableCell htcendgap1 = new HtmlTableCell();
			htcendgap1.Attributes.Add("class", "ibn-stylebox tabGap2");
			htcendgap1.Style.Add("width", "100%");
			htcendgap1.InnerHtml = "&nbsp;";
			if (!_isleftLine)
			{
				htcendgap.Style.Add("border-right", "0px;");
				htcendgap1.Style.Add("border-right", "0px;");
			}
			trTabs.Cells.Add(htcendgap);
			trTabs1.Cells.Add(htcendgap1);
			if (trTabs1.Cells.Count <= 2)
				tbl2.Visible = false;
			else
				tbl2.Visible = true;
		}
		#endregion

		#region AddTab
		public void AddTab(string title, string link, string key)
		{
			TabItems.Add(new TabItem(title, link, false, key));
		}

		public void AddTab(string title, string key)
		{
			string link = Request.Url.AbsolutePath;
			int indexer = 0;
			for (int i = 0; i < Request.QueryString.Count; i++)
			{
				if (Request.QueryString.Keys[i] != "Tab"
				  && Request.QueryString.Keys[i] != "SubTab"
				  && Request.QueryString.Keys[i] != "Assign")
				{
					if (indexer == 0)
						link += "?";
					else
						link += "&amp;";

					link += Request.QueryString.Keys[i] + "=" + Request.QueryString[i];
					indexer++;
				}
			}

			TabItems.Add(new TabItem(title, link, false, key));
		}

		public void AddTab(string title, string key, int row)
		{
			string link = Request.Url.AbsolutePath;
			int indexer = 0;
			for (int i = 0; i < Request.QueryString.Count; i++)
			{
				if (Request.QueryString.Keys[i] != "Tab" && Request.QueryString.Keys[i] != "SubTab")
				{
					if (indexer == 0) link += "?";
					else link += "&";
					link += Request.QueryString.Keys[i] + "=" + Request.QueryString[i];
					indexer++;
				}
			}

			TabItems.Add(new TabItem(title, link, false, key, row));
		}

		public void AddTab(string title)
		{
			TabItems.Add(new TabItem(title, BaseUrl, false));
		}
		#endregion

		#region SelectItem
		public void SelectItem(int index)
		{
			if (index >= 0 && index < TabItems.Count)
				((TabItem)TabItems[index]).selected = true;
		}

		public void SelectItem(string key)
		{
			ClearSelection();
			bool find = false;
			foreach (TabItem ti in TabItems)
			{
				if (ti.key == key)
				{
					ti.selected = true;
					find = true;
				}
			}
			if (!find && !IsDesignMode)
			{
				SelectItem(0);
			}
		}
		#endregion

		#region ClearSelection
		private void ClearSelection()
		{
			foreach (TabItem ti in TabItems)
				if (ti.selected == true)
					ti.selected = false;
		}
		#endregion

		#region GetItemLink
		public string GetItemLink(string key)
		{
			TabItem _ti = null;
			foreach (TabItem ti in TabItems)
				if (ti.key == key)
					_ti = ti;
			if (_ti != null)
			{
				string tab = "?Tab=";

				if (_ti.link.IndexOf("?") >= 0)
					tab = "&Tab=";
				string index = _ti.key;
				return _ti.link + tab + index;

			}
			else
				return "#";
		}
		#endregion

		#region GetSelectedItem
		private TabItem GetSelectedItem()
		{
			TabItem retval = null;
			foreach (TabItem ti in TabItems)
			{
				if (ti.selected)
				{
					retval = ti;
					break;
				}
			}
			return retval;
		}
		#endregion

		#region lbSelectTab_Click
		protected void lbSelectTab_Click(object sender, EventArgs e)
		{
			string arg = Request["__EVENTARGUMENT"];
			TopTabsEventArgs args = new TopTabsEventArgs(arg);
			this.OnTabChange(sender, args);
		}
		#endregion

		#region event: OnTabChange
		public delegate void TabChanged(Object sender, TopTabsEventArgs e);

		public event TabChanged TabChange;

		protected virtual void OnTabChange(Object sender, TopTabsEventArgs e)
		{
			if (TabChange != null)
			{
				TabChange(this, e);
			}
		}
		#endregion

		#region GetFullPath
		private string GetFullPath(string xs_Path)
		{
			string UrlScheme = System.Configuration.ConfigurationManager.AppSettings["UrlScheme"];

			StringBuilder builder = new StringBuilder();
			if (UrlScheme != null)
				builder.Append(UrlScheme);
			else
				builder.Append(HttpContext.Current.Request.Url.Scheme);
			builder.Append("://");

			builder.Append(HttpContext.Current.Request.Url.Authority);

			if (builder[builder.Length - 1] != '/')
				builder.Append("/");

			if (xs_Path != string.Empty)
			{
				if (xs_Path[0] == '/')
					xs_Path = xs_Path.Substring(1, xs_Path.Length - 1);
				builder.Append(xs_Path);
			}
			return builder.ToString();
		}
		#endregion
	}

	#region class TabItem
	public class TabItem
	{
		public string text;
		public string link;
		public bool selected;
		public string key;
		public int row;

		public TabItem(string _text, string _link, bool _selected)
		{
			text = _text;
			link = _link;
			selected = _selected;
			key = "";
			row = 1;
		}

		public TabItem(string _text, string _link, bool _selected, string _key)
		{
			text = _text;
			link = _link;
			selected = _selected;
			key = _key;
			row = 1;
		}

		public TabItem(string _text, string _link, bool _selected, string _key, int _row)
		{
			text = _text;
			link = _link;
			selected = _selected;
			key = _key;
			row = _row;
		}
	}
	#endregion
}
