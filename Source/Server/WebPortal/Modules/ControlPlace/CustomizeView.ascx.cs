namespace ControlPlaceApplication
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for CustomizeView.
	/// </summary>
	public partial class CustomizeView : System.Web.UI.UserControl
	{


		#region Public Properties: Title
		private string title = "";
		public string Title 
		{
			set 
			{
				title = value;
				lbTitle.Text = title;
			}
			get 
			{
				return title;
			}
		}
		#endregion

		#region Public Properties: DropMenuHtml
		private string dropmenuhtml = "";
		public string DropMenuHtml 
		{
			set 
			{
				dropmenuhtml = value;
				tdDropMenu.InnerHtml = value;
			}
			get 
			{
				return dropmenuhtml;
			}
		}
		#endregion

		#region Public Properties: Description
		private string description = "";
		public string Description 
		{
			set 
			{
				description = value;
				tdMainContent.InnerHtml = value;
			}
			get 
			{
				return description;
			}
		}
		#endregion

		#region Public Function: Control
		public void Control(System.Web.UI.Control InnerControl)
		{
			tdMainContent.Controls.Add(InnerControl);
		}
		#endregion

		#region Path_Img
		private string path_img = "";
		public string Path_Img
		{
			set
			{
				path_img = value;
			}
			get
			{
				return path_img;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
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

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if(dropmenuhtml == "")
				tdDropMenu.Visible = false;
			else
				tdDropMenu.Visible = true;
		}
	}
}
