namespace ControlPlaceApplication
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
	using Mediachase.UI.Web.Modules;

	/// <summary>
	///		Summary description for BlockMenuHeader.
	/// </summary>
	public partial  class BlockMenuHeader : System.Web.UI.UserControl
	{


		ArrayList _menuitems = new ArrayList();

		#region MainDivClientID
		public string MainDivClientID
		{
			get
			{
				return divBMenu.ClientID;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{

		}

		public void AddBlockMenu(string sUniqueId, ArrayList BMItems)
		{
			_menuitems.Add(new BlockMenu(String.Empty, sUniqueId, BMItems));
		}

		public void ClearBlockMenu()
		{
			_menuitems.Clear();
		}

		public void AddBlockMenuItem(string sMenuUniqueId, string sPicture, string sText, string sClientUrl, string sNavigateUrl)
		{
			foreach (BlockMenu li in _menuitems)
			{
				if(li._id==sMenuUniqueId)
					li._blockMenuItemList.Add(new BlockMenuItem(sPicture, sText, sClientUrl, sNavigateUrl));
			}
		}

		public void InsertBlockMenuItem(int Position, string sMenuUniqueId, string sPicture, string sText, string sClientUrl, string sNavigateUrl)
		{
			foreach (BlockMenu li in _menuitems)
			{
				if(li._id==sMenuUniqueId)
					li._blockMenuItemList.Insert(Position, new BlockMenuItem(sPicture, sText, sClientUrl, sNavigateUrl));
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			foreach (BlockMenu li in _menuitems)
			{
				foreach(BlockMenuItem bmi in li._blockMenuItemList)
				{
					DrawBlockMenuItem(bmi._pic, bmi._text, bmi._clienturl, bmi._navurl);
				}
			}

			base.Render(writer);
		}

		private void DrawBlockMenuItem(string sPicture, string sText, string sClientUrl, string sNavigateUrl)
		{
			HtmlTableRow _tr = new HtmlTableRow();
			_tr.Attributes.Add("onmouseover", "this.className='ibn-rowMenuHoverStyle';this.cells[0].className=''");
			_tr.Attributes.Add("onmouseout", "this.className='ibn-rowMenuStyle';this.cells[0].className='ibn-cellLeftMenu'");
			HtmlTableCell _tdImg = new HtmlTableCell();
			HtmlTableCell _td = new HtmlTableCell();
			_tdImg.Attributes.Add("class","ibn-cellLeftMenu");
			_tdImg.Width = "22px";
			_tdImg.Height = "21px";
			_tdImg.VAlign = "top";
			_tdImg.InnerHtml = sPicture;
			_tr.Cells.Add(_tdImg);
			_td.InnerHtml = sText;
			string sUrl = "javascript:";//try{";
			if(sClientUrl!="")
				sUrl += sClientUrl + ";";
			if(sNavigateUrl!="")
				sUrl += "window.location.href='"+sNavigateUrl+"';";//}catch(e){}";
			else
				sUrl += "";//}catch(e){}";
			_td.Attributes.Add("style", "cursor:default;");
			_tdImg.Attributes.Add("onclick", sUrl);
			_td.Attributes.Add("onclick", sUrl);
			_tr.Cells.Add(_td);
			tblBMenu.Rows.Add(_tr);
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
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}

	internal class BlockMenu
	{
		public string _text;
		public string _id;
		public ArrayList _blockMenuItemList;

		public BlockMenu(string text, string id, ArrayList ItemsList)
		{
			_text = text;
			_id = id;
			_blockMenuItemList = ItemsList;
		}
	}

	internal class BlockMenuItem
	{
		public string _pic;
		public string _text;
		public string _clienturl;
		public string _navurl;
		public BlockMenuItem(string Picture, string Text, string ClientUrl, string NavigateUrl)
		{
			_pic = Picture;
			_text = Text;
			_clienturl = ClientUrl;
			_navurl = NavigateUrl;
		}
	}
}