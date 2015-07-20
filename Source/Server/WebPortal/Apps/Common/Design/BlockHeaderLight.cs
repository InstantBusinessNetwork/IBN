using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI
{
	public class BlockHeaderLight : UserControl
	{
		private string outLStr = string.Empty;
		private string outRStr = string.Empty;
		private string divStr = "<td style='color: #444444;font-size: 10pt;'>|</td>";
		private string _title = String.Empty;

		#region Title
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
			}
		} 
		#endregion

		#region HeaderCssClass
		private string _headerCssClass;
		public string HeaderCssClass
		{
			get { return _headerCssClass; }
			set { _headerCssClass = value; }
		}
		#endregion

		#region AddText
		public void AddText(string Text)
		{
			Title = Text;
		}
		#endregion

		#region AddLeftLink
		public void AddLeftLink(string Text, string Url)
		{
			if (outLStr.Length > 0)
				outLStr += divStr;

			Url = Url.Replace("\"", "&quot;");
			outLStr += "<td nowrap='nowrap' style='padding-right:5px; padding-left:5px'><a href=\"" + Url + "\">" + Text + "</a></td>";
		}
		#endregion

		#region AddRightLink
		public void AddRightLink(string Text, string Url)
		{
			if (outRStr.Length > 0)
				outRStr += divStr;

			Url = Url.Replace("\"", "&quot;");
			outRStr += "<td nowrap='nowrap' style='padding-right:5px; padding-left:5px'><a href=\"" + Url + "\">" + Text + "</a></td>";
		}
		#endregion

		#region ClearLeftItems
		public void ClearLeftItems()
		{
			outLStr = string.Empty;
		}
		#endregion

		#region ClearRightItems
		public void ClearRightItems()
		{
			outRStr = string.Empty;
		}
		#endregion

		#region Render
		protected override void Render(HtmlTextWriter writer)
		{
			if (!String.IsNullOrEmpty(Title))
			{
				if (outLStr.Length > 0)
					outLStr += divStr;

				outLStr += "<td nowrap='nowrap' style='padding-right:5px; padding-left:5px; font-weight:bold'>" + Title + "</td>";
			}

			Table mainTable = new Table();
			mainTable.CellSpacing = 0;
			mainTable.CellPadding = 0;
			mainTable.BorderWidth = Unit.Pixel(0);
			mainTable.Width = Unit.Percentage(100);
			TableRow tr = new TableRow();

			TableCell tc1 = new TableCell();
			TableCell tdLeftItems = new TableCell();
			TableCell tc3 = new TableCell();
			TableCell tdRightItems = new TableCell();
			TableCell tc5 = new TableCell();
			tc1.VerticalAlign = VerticalAlign.Bottom;
			HtmlImage imgLeft = new HtmlImage();
			imgLeft.Width = 11;
			imgLeft.Height = 20;
			imgLeft.Src = this.Page.ResolveUrl("~/Images/IbnFramework/leftCorner.GIF");// this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "Mediachase.Ibn.Web.UI.WebControls.BlockHeaderLight.leftCorner.gif");
			tc1.Controls.Add(imgLeft);
			tc3.Attributes.Add("background", this.Page.ResolveUrl("~/Images/IbnFramework/linehz.GIF"));// this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "Mediachase.Ibn.Web.UI.WebControls.BlockHeaderLight.linehz.gif"));
			tc3.Attributes.Add("width", "100%");
			tc3.Style.Add("background-repeat", "repeat-x");
			tc3.Style.Add("background-position-y", "bottom");
			tc5.VerticalAlign = VerticalAlign.Bottom;
			HtmlImage imgRight = new HtmlImage();
			imgRight.Width = 11;
			imgRight.Height = 20;
			imgRight.Src = this.Page.ResolveUrl("~/Images/IbnFramework/rightCorner.GIF");// this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "Mediachase.Ibn.Web.UI.WebControls.BlockHeaderLight.rightCorner.gif");
			tc5.Controls.Add(imgRight);
			tr.Cells.Add(tc1);
			tr.Cells.Add(tdLeftItems);
			tr.Cells.Add(tc3);
			tr.Cells.Add(tdRightItems);
			tr.Cells.Add(tc5);
			mainTable.Rows.Add(tr);

			#region Render Header
			if (outLStr.Length > 0)
			{
				tdLeftItems.Text = String.Format("<table cellpadding='0' cellspacing='0' border='0' {1}><tr>{0}</tr></table>",
					outLStr, String.IsNullOrEmpty(HeaderCssClass) ? "" : "class='" + HeaderCssClass + "'");
				tdLeftItems.Visible = true;
			}
			else
				tdLeftItems.Visible = false;

			if (outRStr.Length > 0)
			{
				tdRightItems.Text = String.Format("<table cellpadding='0' cellspacing='0' border='0' {1}><tr>{0}</tr></table>",
					outRStr, String.IsNullOrEmpty(HeaderCssClass) ? "" : "class='" + HeaderCssClass + "'");
				tdRightItems.Visible = true;
			}
			else
				tdRightItems.Visible = false;
			#endregion

			mainTable.RenderControl(writer);

			base.Render(writer);
		}
		#endregion
	}
}
