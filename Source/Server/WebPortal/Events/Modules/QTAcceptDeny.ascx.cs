namespace Mediachase.UI.Web.Events.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.Ibn;
	using Mediachase.IBN.Business;

	/// <summary>
	///		Summary description for QuickTrackingAcceptDeny.
	/// </summary>
	public partial  class QuickTrackingAcceptDeny : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM;

		private int EventID
		{
			get 
			{
				try
				{
					return int.Parse(Request["EventID"]);
				}
				catch
				{
					throw new AccessDeniedException();
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
      LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strQuickTracking", typeof(QuickTrackingAcceptDeny).Assembly);
			BindToolBar();
			DataBind();
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

		private void BindToolBar()
		{
			tbView.Title = LocRM.GetString("QuickTrackingAcceptDeny");
		}

		protected void btnAccept_ServerClick(object sender, System.EventArgs e)
		{
			CalendarEntry2.AcceptResource(EventID);
      if (!Security.CurrentUser.IsExternal)
			  Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../Events/EventView.aspx?EventId=" + EventID, Response);
      else
        Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../External/ExternalEvent.aspx?EventId=" + EventID, Response);
		}

		protected void btnDecline_ServerClick(object sender, System.EventArgs e)
		{
			CalendarEntry2.DeclineResource(EventID);
      if (!Security.CurrentUser.IsExternal)
        Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../Workspace/default.aspx?BTab=Workspace", Response);
      else
        Response.Redirect("~/External/MissingObject.aspx");
		}
	}
}
