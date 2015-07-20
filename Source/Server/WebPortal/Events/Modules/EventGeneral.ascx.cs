namespace Mediachase.UI.Web.Events.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for EventGeneral.
	/// </summary>
	public partial  class EventGeneral : System.Web.UI.UserControl
	{


    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strEventGeneral", typeof(EventGeneral).Assembly);

		#region EventID
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
					throw new Exception("Invalid Event ID");
				}
			}
		}
		#endregion

		#region SharedID
		private int SharedID
		{
			get 
			{
				return CommonHelper.GetRequestInteger(Request, "SharedID", -1);
			}
		}
		#endregion

		#region Page_Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (SharedID > 0 || !CalendarEntry.ShowAcceptDeny(EventID))
				ctrlQT.Visible = false;

			if (Security.CurrentUser.IsExternal)
				ctrlEP.Visible = false;
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
	}
}
