using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.Common.Design
{
	public partial class BlockHeader2 : System.Web.UI.UserControl
	{
		#region Property: Title
		public string Title
		{
			set
			{
				lblBlockTitle.Text = value;
			}
			get
			{
				return lblBlockTitle.Text;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
		
		}

		#region AddLink
		public void AddLink(string text, string url)
		{
			if (ViewState["Links"] == null)
				ViewState["Links"] = new Dictionary<string, KeyValuePair<string, string>>();

			((Dictionary<string, KeyValuePair<string, string>>)ViewState["Links"]).Add(text, new KeyValuePair<string, string>(url, String.Empty));
		}

		public void AddLink(string imageUrl, string text, string url)
		{
			if (ViewState["Links"] == null)
				ViewState["Links"] = new Dictionary<string, KeyValuePair<string, string>>();

			((Dictionary<string, KeyValuePair<string, string>>)ViewState["Links"]).Add(text, new KeyValuePair<string, string>(url, imageUrl));
		}
		#endregion

		#region ClearLinks
		public void ClearLinks()
		{
			ViewState["Links"] = null;
		}
		#endregion

		#region PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			bool setSeparator = false;
			if (ViewState["Links"] != null)
			{
				foreach (KeyValuePair<string, KeyValuePair<string, string>> kvp in (Dictionary<string, KeyValuePair<string, string>>)ViewState["Links"])
				{
					if (setSeparator)
					{
						AddSeparatorInternal();
					}
					else
					{
						setSeparator = true;
					}

					HtmlAnchor link = new HtmlAnchor();
					string text;
					if (!String.IsNullOrEmpty(kvp.Value.Value))
					{
						text = String.Format(
							CultureInfo.InvariantCulture,
							"<img align='absmiddle' border='0' width='16px;' height='16px;' src='{0}' />&nbsp;{1}",
							ResolveClientUrl(kvp.Value.Value), kvp.Key);
					}
					else
					{
						text = kvp.Key;
					}

					link.InnerHtml = text;
					link.HRef = kvp.Value.Key;
					link.Attributes.Add("class", "ibn-toolbar");

					HtmlTableCell cell = new HtmlTableCell();
					cell.Style.Add("padding-right", "5px");
					cell.NoWrap = true;
					cell.Controls.Add(link);
					trMain.Cells.Insert(trMain.Cells.Count, cell);
				}
			}
		}
		#endregion

		#region AddSeparatorInternal()
		private void AddSeparatorInternal()
		{
			HtmlTableCell cell = new HtmlTableCell();
			cell.Attributes.Add("class", "ibn-separator");
			cell.InnerText = "|";
			trMain.Cells.Insert(trMain.Cells.Count, cell);
		}
		#endregion
	}
}