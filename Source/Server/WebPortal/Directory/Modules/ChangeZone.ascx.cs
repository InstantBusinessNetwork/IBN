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
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for ChangeZone.
	/// </summary>
	public partial class ChangeZone : System.Web.UI.UserControl
	{
		ResourceManager LocRM;


		private int TimeOffset
		{
			get
			{
				try
				{
					return int.Parse(Request["TimeOffset"]);
				}
				catch
				{
					return 0;
				}
			}
		}


		protected void Page_Load(object sender, System.EventArgs e)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strChangeZone", typeof(ChangeZone).Assembly);
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			BindToolbar();
			if (!IsPostBack)
			{
				lblText.Text = LocRM.GetString("Message");
				ddZones.DataSource = User.GetListTimeZone();
				ddZones.DataTextField = "DisplayName";
				ddZones.DataValueField = "TimeZoneId";
				ddZones.DataBind();
			}

			//int TZId = User.GetTimeZoneByBias(TimeOffset);
			ListItem lItem = ddZones.Items.FindByValue(Security.CurrentUser.TimeZoneId.ToString());
			if (lItem != null)
			{
				ddZones.ClearSelection();
				lItem.Selected = true;
			}
		}

		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tTitle");
			btnSave.Text = LocRM.GetString("Ok");
			btnCancel.Text = LocRM.GetString("Cancel");
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
			int TimeZoneId = int.Parse(ddZones.SelectedItem.Value);
			//int TimeOffset = User.GetCurrentBias(TimeZoneId);
			User.UpdateTimeZoneId(Security.CurrentUser.UserID, TimeZoneId);
			User.UpdateTimeOffsetLatest(Security.CurrentUser.UserID, TimeOffset);

			string url = Request.QueryString["ReturnUrl"];
			if (String.IsNullOrEmpty(url) || url.ToLower().Contains("/apps/shell/pages/default.aspx"))
			{
				RedirectTop(ResolveClientUrl("~/Workspace/default.aspx?BTab=Workspace"));
			}
			else
			{
				RedirectTop(url);
			}
		}

		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			//User.UpdateTimeOffsetLatest(Security.CurrentUser.UserID, TimeOffset);

			string url = Request.QueryString["ReturnUrl"];
			if (String.IsNullOrEmpty(url) || url.ToLower().Contains("/apps/shell/pages/default.aspx"))
			{
				Response.Redirect("~/Workspace/default.aspx?BTab=Workspace");
			}
			else
			{
				Response.Redirect(Request["ReturnUrl"]);
			}
		}

		#region RedirectTop
		private void RedirectTop(string url)
		{
			Response.Clear();
			Response.Write(
				"<script type=\"text/javascript\">" +
				"window.top.location.href = \"" + url + "\"" +
				"</script>"
				);
			Response.End();
		}
		#endregion
	}
}
