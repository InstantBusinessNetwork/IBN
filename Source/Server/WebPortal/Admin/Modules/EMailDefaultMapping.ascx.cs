namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using System.Reflection;

	/// <summary>
	///		Summary description for EMailDefaultMapping.
	/// </summary>
	public partial class EMailDefaultMapping : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		#region IssBoxId
		protected int IssBoxId
		{
			get
			{
				if(Request["IssBoxId"]!=null)
					return int.Parse(Request["IssBoxId"]);
				else
					return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!Page.IsPostBack)
				BindList();
			string controlName = EMailIncidentMappingHandler.Load(int.Parse(ddSource.SelectedValue)).UserControl;
			System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
			phMap.Controls.Add(control);
		}

		private void BindList()
		{
			ddSource.Items.Clear();
			EMailIncidentMappingHandler[] list = EMailIncidentMappingHandler.List();
			foreach(EMailIncidentMappingHandler imh in list)
				ddSource.Items.Add(new ListItem(imh.Name, imh.EMailIncidentMappingId.ToString()));
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
			this.ddSource.SelectedIndexChanged += new EventHandler(ddSource_SelectedIndexChanged);
		}
		#endregion

		private void ddSource_SelectedIndexChanged(object sender, EventArgs e)
		{
			phMap.Controls.Clear();
			string controlName = EMailIncidentMappingHandler.Load(int.Parse(ddSource.SelectedValue)).UserControl;
			System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
			phMap.Controls.Add(control);
		}
	}
}
