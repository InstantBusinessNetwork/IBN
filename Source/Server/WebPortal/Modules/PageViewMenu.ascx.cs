namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Threading;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	using System.Xml;

	using ComponentArt.Web.UI;
	using Mediachase.Ibn.Web.UI.WebControls;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for PageViewMenu.
	/// </summary>
	public partial class PageViewMenu : UserControl
	{


		public ComponentArt.Web.UI.Menu ActionsMenu
		{
			get
			{
				return AcMenu;
			}
		}

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

		private string menuXML = "";
		public string MenuXML
		{
			set
			{
				menuXML = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			CommonHelper.SafeRegisterStyle(Page, "~/Styles/IbnFramework/mcBlockMenu.css");

			lblBlockTitle.Text = title;
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if(menuXML!="")
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(menuXML);
				XmlNodeList xmlTopItems = doc.SelectNodes("Menus/Menu");
				ComponentArt.Web.UI.MenuItem newItem;
				foreach(XmlNode xmlTopItem in xmlTopItems)
				{
					newItem = new ComponentArt.Web.UI.MenuItem();
					string sTitle = "";
					if(xmlTopItem.Attributes["PictureUrl"]!=null)
					{
						string sUrl = xmlTopItem.Attributes["PictureUrl"].Value;
						if(sUrl.IndexOf("~")==0)
							sUrl = Request.ApplicationPath + "/" + sUrl.Substring(2);
						sTitle = "<img border='0' src='"+sUrl+"' align='absmiddle'/>&nbsp;";
					}
					newItem.Text = sTitle + xmlTopItem.Attributes["Name"].Value;
					newItem.LookId = "TopItemLook";
					if(xmlTopItem.HasChildNodes)
						BindChildNodes(xmlTopItem, newItem);
					AcMenu.Items.Add(newItem);
				}
			}
/*			else
				AcMenu.Visible = false;
*/
			base.Render(writer);
		}

		private void BindChildNodes(XmlNode xmlNode, ComponentArt.Web.UI.MenuItem MenuItem)
		{
			ComponentArt.Web.UI.MenuItem subItem;
			foreach(XmlNode xmlItem in xmlNode.ChildNodes)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				if(xmlItem.Attributes["PictureUrl"]!=null)
				{
					string sUrl = xmlItem.Attributes["PictureUrl"].Value;
					subItem.Look.LeftIconUrl = sUrl;
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
				}
				if(xmlItem.Attributes["NavigateUrl"]!=null)
				{
					string sUrl = xmlItem.Attributes["NavigateUrl"].Value;
					if(sUrl.IndexOf("IncidentId=")>=0)
						sUrl += Request["IncidentId"].ToString();
					subItem.NavigateUrl = sUrl;
				}
				if(xmlItem.Attributes["ClientCommand"]!=null)
				{
					subItem.ClientSideCommand = xmlItem.Attributes["ClientCommand"].Value;
				}
				subItem.Text = xmlItem.Attributes["Name"].Value;
				if(xmlItem.HasChildNodes)
					BindChildNodes(xmlItem, subItem);
				MenuItem.Items.Add(subItem);
			}
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
}
