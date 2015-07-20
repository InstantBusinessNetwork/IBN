using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;

namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for TopTabs.
	/// </summary>
	public partial class TopTabs : System.Web.UI.UserControl
	{

		private ArrayList TabItems = new ArrayList();

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/TopTabs.css");
			// Put user code to initialize the page here
		}

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

		string _TabWidth = "120px";
		public string TabWidth
		{
			get
			{
				return ((int)(int.Parse(_TabWidth) + 10)).ToString();
			}
			set
			{
				_TabWidth = value;
			}

		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			for (int i = 0; i < TabItems.Count; i++)
			{
				TabItem ti = (TabItem)TabItems[i];

				HtmlTableCell cell = new HtmlTableCell();
				trTabs.Cells.Add(cell);

				HtmlGenericControl content = new HtmlGenericControl("div");
				cell.Controls.Add(content);
				content.Style["width"] = _TabWidth;

				if (ti.selected)
				{
					cell.Attributes.Add("class", "ibn-stylebox ibn-navframe tab tabSelected");
					content.InnerHtml = ti.text;
				}
				else
				{
					cell.Attributes.Add("class", "ibn-stylebox ibn-navframe tab");

					string tab = "?Tab=";
					if (ti.link.IndexOf("?") >= 0)
						tab = "&Tab=";
					string index = "";
					if (ti.key == "")
						index = i.ToString();
					else
						index = ti.key;

					content.InnerHtml = "<a href=\"" + ti.link + tab + index + "\">" + ti.text + "</a>";
				}

				HtmlTableCell htcgap = new HtmlTableCell();
				htcgap.Attributes.Add("class", "ibn-stylebox tabGap");
				htcgap.InnerHtml = "&nbsp;";
				trTabs.Cells.Add(htcgap);
			}

			HtmlTableCell htcendgap = new HtmlTableCell();
			htcendgap.Attributes.Add("class", "ibn-stylebox tabGap");
			htcendgap.Style["width"] = "100%";
			htcendgap.InnerHtml = "&nbsp;";
			trTabs.Cells.Add(htcendgap);
		}

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
				if (Request.QueryString.Keys[i] != "Tab" && Request.QueryString.Keys[i] != "Assign")
				{
					if (indexer == 0)
						link += "?";
					else
						link += "&";

					link += Request.QueryString.Keys[i] + "=" + Request.QueryString[i];
					indexer++;
				}
			}

			TabItems.Add(new TabItem(title, link, false, key));
		}

		public void AddTab(string title)
		{
			TabItems.Add(new TabItem(title, BaseUrl, false));
		}

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
			if (!find)
			{
				throw new Exception(key + " tab not found");
			}
		}

		private void ClearSelection()
		{
			foreach (TabItem ti in TabItems)
				if (ti.selected == true)
					ti.selected = false;
		}

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

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
