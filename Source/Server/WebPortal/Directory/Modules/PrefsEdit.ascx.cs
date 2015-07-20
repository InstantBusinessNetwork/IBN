namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Globalization;
	using Mediachase.IBN.Business;

	/// <summary>
	///		Summary description for PrefsEdit.
	/// </summary>
	public partial class PrefsEdit : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Button btnCreateSave;

		public ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strPrefsEdit", typeof(PrefsEdit).Assembly);

		private int UserID
		{
			get
			{
				try
				{
					return int.Parse(Request["UserID"]);
				}
				catch
				{
					return Security.CurrentUser.UserID;
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			BindToolbars();
			if (!IsPostBack)
				ctlEditPrefs.LoadPrefs();
		}

		private void BindToolbars()
		{
			tbEditPrefs.Title = LocRM.GetString("tbTitle");

			btnSave.Text = LocRM.GetString("tbSave");
			btnCancel.Text = LocRM.GetString("tbCancel");
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


		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			bool isRefresh = ctlEditPrefs.SavePrefs();
			if (UserID == Security.CurrentUser.UserID && isRefresh)
			{
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
							"<script>window.top.location.href='../Directory/UserView.aspx?UserID=" + UserID.ToString() + "';</script>");
				//				UserLightPropertyCollection pc = Security.CurrentUser.Properties;
				//				pc["AllRefresh"]="1";
			}
			else
				Response.Redirect("../Directory/UserView.aspx?UserID=" + UserID.ToString());
		}

		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			Response.Redirect("../Directory/UserView.aspx?UserID=" + UserID);
		}
	}
}
