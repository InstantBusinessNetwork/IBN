namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using System.Threading;
	using Mediachase.Ibn.Web.UI;

	/// <summary>
	///		Summary description for BlockHeader.
	/// </summary>
	public partial class BlockHeader : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlTableCell tdCollapse;

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

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindData();
		}

		#region AddLink
		public void AddLink(string text, string url)
		{
			items.Add(new LinkItem1(text, url));
		}
		#endregion

		#region AddLinkAt
		public void AddLinkAt(int position, string text, string url)
		{
			items.Insert(position, new LinkItem1(text, url));
		}
		#endregion

		#region AddImageLink
		public void AddImageLink(string ImageUrl, string text, string url)
		{
			items.Add(new LinkItem1("<img align='absmiddle' border='0' src='" + ImageUrl + "' />&nbsp;" + text, url));
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

		// Oleg Rylin: Render заменен на PreRender [6/22/2006]
		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
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
				cell.Style["padding-right"] = "5";
				cell.NoWrap = true;
				//	cell.Attributes.Add("class", "ibn-toolbar");
				cell.Controls.Add(link);
				if (title == "")
					trMain.Cells.Insert(trMain.Cells.Count - 1, cell);
				else
					trMain.Cells.Insert(trMain.Cells.Count, cell);
			}
		}
		#endregion
	}
}
