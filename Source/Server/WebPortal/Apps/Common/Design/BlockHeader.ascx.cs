using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;

namespace Mediachase.Ibn.Web.UI
{
	public partial class BlockHeader : System.Web.UI.UserControl
	{
		ArrayList items = new ArrayList();

		#region Public Properties: Title
		private string title = "";
		public string Title
		{
			set
			{
				title = value;
				lblBlockTitle.Text = title;
				if (title == "")
					lblBlockTitle.Visible = false;
				else
					lblBlockTitle.Visible = true;
			}
			get
			{
				return title;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			BindData();
		}

		#region PreRender
		protected override void OnPreRender(EventArgs e)
		{
			bool setseparator = false;
			foreach (LinkItem1 li in items)
			{
				if (setseparator)
				{
					AddSeparatorInternal();
				}
				else setseparator = true;

				HtmlAnchor link = new HtmlAnchor();
				link.InnerHtml = li._text;
				link.HRef = li._url;
				link.Attributes.Add("class", "ibn-toolbar");

				HtmlTableCell cell = new HtmlTableCell();
				cell.Style.Add("padding-right", "5");
				cell.NoWrap = true;
				cell.Controls.Add(link);
				if (title == "")
					trMain.Cells.Insert(trMain.Cells.Count - 1, cell);
				else
					trMain.Cells.Insert(trMain.Cells.Count, cell);
			}
		}
		#endregion

		#region BindData
		private void BindData()
		{
			lblBlockTitle.Text = title;
		}
		#endregion

		#region AddLink
		public void AddLink(string text, string url)
		{
			items.Add(new LinkItem1(text, url));
		}
		#endregion

		#region AddImageLink
		public void AddImageLink(string ImageUrl, string text, string url)
		{
			items.Add(new LinkItem1("<img align='absmiddle' border='0' src='" + ImageUrl + "' />&nbsp;" + text, url));
		}
		#endregion

		#region AddThemedImageLink
		public void AddThemedImageLink(string ImageUrl, string text, string url)
		{
			items.Add(new LinkItem1("<img align='absmiddle' border='0' src='" + CHelper.GetAbsolutePath(ImageUrl) + "' />&nbsp;" + text, url));
		}
		#endregion

		#region AddLinkAt
		public void AddLinkAt(int position, string text, string url)
		{
			items.Insert(position, new LinkItem1(text, url));
		}
		#endregion

		#region AddSeparatorInternal()
		public void AddSeparatorInternal()
		{
			HtmlTableCell cell = new HtmlTableCell();
			cell.Attributes.Add("class", "ibn-separator");
			cell.InnerText = "|";
			if (title == "")
				trMain.Cells.Insert(trMain.Cells.Count - 1, cell);
			else
				trMain.Cells.Insert(trMain.Cells.Count, cell);
		}
		#endregion

		#region AddSeparator
		public void AddSeparator()
		{
		}
		#endregion

		#region AddSeparatorAt
		public void AddSeparatorAt(int position)
		{
		}
		#endregion
	}

	public interface IGetHTML
	{
		string GetHTML();
	}

	public class LinkItem1 : IGetHTML
	{
		public string _text;
		public string _url;

		public LinkItem1(string text, string url)
		{
			_text = text;
			_url = url;
		}

		#region Implementation of IGetHTML
		public string GetHTML()
		{
			return String.Format("<a href='{0}'>{1}</a>", _url, _text);
		}
		#endregion
	}
}