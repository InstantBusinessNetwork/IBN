namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;

	/// <summary>
	///		Summary description for IncidentGeneral.
	/// </summary>
	public partial class IncidentGeneral : System.Web.UI.UserControl
	{

		#region IncidentId
		private int IncidentId
		{
			get 
			{
				if(Request["IncidentId"]!=null && Request["IncidentId"]!="")
					return int.Parse(Request["IncidentId"]);
				else
					return -1;
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

		private void Page_PreRender(object sender, EventArgs e)
		{
//			bool canAddResBox = false;
//			bool canAddToDoBox = false;
//
//			using(IDataReader reader = Incident.GetIncident(IncidentId))
//			{
//				if(reader.Read())
//				{
//					int IssBoxId = (int)reader["IncidentBoxId"];
//					IncidentBoxDocument ibd = IncidentBoxDocument.Load(IssBoxId);
//					canAddResBox = ibd.GeneralBlock.AllowAddResources;
//					canAddToDoBox = ibd.GeneralBlock.AllowAddToDo;
//				}
//			}
//
//			if(!canAddResBox)
//				ucIncidentResources.Visible = false;
//			if(!canAddToDoBox)
//				ucIncidentsToDo.Visible = false;

			if (Security.CurrentUser.IsExternal)
			{
				ucIncidentRecipients.Visible = false;
				ucIncidentResources.Visible = false;
				ucIncidentsToDo.Visible = false;
				ucRelatedIncidents.Visible = false;
			}
			if (!Configuration.TimeTrackingModule)
				ucTimeTracking.Visible = false;


			tdMain.Visible = (ucIncidentResources.Visible || ucIncidentRecipients.Visible || ucIncidentsToDo.Visible || ucRelatedIncidents.Visible || ucTimeTracking.Visible);
		}
	}
}
