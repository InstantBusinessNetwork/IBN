using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for BlockHeader.
	/// </summary>
	public partial  class BlockHeader : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlTableCell tdCollapse;

		#region Public Properties: Title
		private string title = "";
		public string Title {
			set {
				title = value;
				lblBlockTitle.Text = title;
				if (title == "")
					lblBlockTitle.Visible = false;
				else
					lblBlockTitle.Visible = true;
			}
			get {
				return title;
			}
		}

		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindData();
		}

		public void AddLink(string text, string url)
		{
			HtmlAnchor link = new HtmlAnchor();
			link.InnerHtml = text;
			link.HRef = url;
			link.Attributes.Add("class", "ibn-toolbar");

			HtmlTableCell cell =  new HtmlTableCell();
			cell.Style["white-space"] = "nowrap";
			cell.Style["padding-right"] = "5px";
			cell.Controls.Add(link);

			if (title == "")
				trMain.Cells.Insert(trMain.Cells.Count-1, cell);
			else
				trMain.Cells.Insert(trMain.Cells.Count, cell);
		}

		public void AddLinkTop(string text, string url)
		{
			HtmlAnchor link = new HtmlAnchor();
			link.InnerHtml = text;
			link.HRef = url;
			link.Target = "top";
			link.Attributes.Add("class", "ibn-toolbar");

			HtmlTableCell cell = new HtmlTableCell();
			cell.Style["white-space"] = "nowrap";
			cell.Style["padding-right"] = "5px";
			cell.Controls.Add(link);

			trMain.Cells.Insert(trMain.Cells.Count, cell);
		}

		public void AddLinkAt(int position, string text, string url)
		{
			HtmlAnchor link = new HtmlAnchor();
			link.InnerHtml = text;
			link.HRef = url;
			link.Attributes.Add("class", "ibn-toolbar");

			HtmlTableCell cell =  new HtmlTableCell();
			cell.Style["white-space"] = "nowrap";
			cell.Style["padding-left"] = "5px";
			cell.Controls.Add(link);

			trMain.Cells.Insert(position, cell);
		}

		public void AddTextAt(int position, string text)
		{
			HtmlTableCell cell =  new HtmlTableCell();
			cell.Style["white-space"] = "nowrap";
			cell.Style["padding-left"] = "5px";

			cell.InnerHtml = text;

			trMain.Cells.Insert(position, cell);
		}

		public void AddSeparator()
		{
			HtmlTableCell cell =  new HtmlTableCell();
			cell.Attributes.Add("class", "ibn-separator");
			cell.InnerText = "|";
			if (title == "")
				trMain.Cells.Insert(trMain.Cells.Count-1, cell);
			else
				trMain.Cells.Insert(trMain.Cells.Count, cell);
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

		#region Data Binding
		private void BindData()
		{
			lblBlockTitle.Text = title;
		}
		#endregion 
	}
}
