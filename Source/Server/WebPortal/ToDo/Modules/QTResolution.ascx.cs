namespace Mediachase.UI.Web.ToDo.Modules
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
	///		Summary description for QTResolution.
	/// </summary>
	public partial  class QTResolution : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM;

		#region ToDoID
		private int ToDoID
		{
			get 
			{
				try
				{
					return int.Parse(Request["ToDoID"]);
				}
				catch
				{
					throw new AccessDeniedException();
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
      LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strQuickTracking", typeof(QTResolution).Assembly);
			BindToolBar();
			if (!IsPostBack) 
			{
				
			}
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindSavedValues();
		}

		private void BindToolBar()
		{
			tbView.AddText(LocRM.GetString("IncidentTracking"));
		}

		private void BindSavedValues()
		{
			using(IDataReader rdr = ToDo.GetToDo(ToDoID))
			{
				if (rdr.Read())
				{
					if (rdr["IncidentId"]!=DBNull.Value)
					{
						int incidentid = (int)rdr["IncidentId"];
						using(IDataReader rdr1 = Incident.GetIncident(incidentid,false))
						{
							if (rdr1.Read())
							{
								if (rdr1["Resolution"]!=DBNull.Value)
									tbResolution.Text = (string)rdr1["Resolution"];
								if (rdr1["Workaround"]!=DBNull.Value)
									tbWorkaround.Text = (string)rdr1["Workaround"];
							}
						}
					}
				}
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		protected void btnCompleteToDo_ServerClick(object sender, System.EventArgs e)
		{
			using(IDataReader rdr = ToDo.GetToDo(ToDoID))
			{
				if (rdr.Read())
				{
					if (rdr["IncidentId"]!=DBNull.Value)
					{
						//TODO
						//Issue2.UpdateResolutionInfo((int)rdr["IncidentId"],tbResolution.Text,tbWorkaround.Text);
					}
				}
			}
		}
	}
}
