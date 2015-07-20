namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;

	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.UI.Web.Modules;
	using Mediachase.Ibn.Web.UI.WebControls;
	using Mediachase.Ibn.Web.UI;

	/// <summary>
	///		Summary description for RelatedIncidents.
	/// </summary>
	public partial class RelatedIncidents : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(RelatedIncidents).Assembly);

		#region IncidentId
		protected int IncidentId
		{
			get 
			{
				try 
				{
					return int.Parse(Request["IncidentId"]);
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
			this.Visible = true;
			if (!IsPostBack)
				BindDG();

			BindToolbar();
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (CHelper.NeedToBindGrid())
				BindDG();
			this.Visible = (dgRelatedIss.Items.Count > 0);
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
			this.dgRelatedIss.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_delete);
			this.btnAddRelatedIss.Click += new EventHandler(btnAddRelatedIss_Click);
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			dgRelatedIss.Columns[1].HeaderText = LocRM.GetString("Title");
			dgRelatedIss.Columns[2].HeaderText = LocRM.GetString("Manager");
			if(!Incident.CanUpdate(IncidentId))
				dgRelatedIss.Columns[3].Visible = false;
			dgRelatedIss.DataSource = Incident.GetListIncidentRelationsDataTable(IncidentId).DefaultView;
			dgRelatedIss.DataBind();

			foreach (DataGridItem dgi in dgRelatedIss.Items)
			{
				ImageButton ib=(ImageButton)dgi.FindControl("ibDelete");
				if (ib!=null)
				{
					ib.Attributes.Add("onclick","return confirm('"+ LocRM.GetString("tWarning") +"')");
					ib.ToolTip = LocRM.GetString("Delete");
				}
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("tRelIss"));
			if (Incident.CanUpdate(IncidentId))
			{
				//string sLink = String.Format("javascript:OpenPopUpNoScrollWindow(&quot;../Common/SelectIncident.aspx?btn={0}&exclude={1}&quot;, 640, 480);",
				//    Page.ClientScript.GetPostBackEventReference(btnAddRelatedIss, "xxxtypeid;xxxid"), IncidentId.ToString());
				//secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/relincidents.gif'/> " + LocRM.GetString("tAdd"), sLink);

				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters("MC_HDM_RelatedIss");
				string cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/relincidents.gif'/> " + LocRM.GetString("tAdd"), "javascript:" + cmd);
			}
		}
		#endregion

		void btnAddRelatedIss_Click(object sender, EventArgs e)
		{
			string param = Request["__EVENTARGUMENT"];
			if (!String.IsNullOrEmpty(param))
			{
				string[] mas = param.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				if (mas.Length < 2 || !mas[0].Equals("7"))
					return;
				int iRelId = int.Parse(mas[1]);
				Issue2.AddRelation(IncidentId, iRelId);
				BindDG();
			}
		}

		#region dg_delete
		private void dg_delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int RelIssId = int.Parse(e.Item.Cells[0].Text);
			Issue2.DeleteRelation(IncidentId, RelIssId);
			Response.Redirect("../Incidents/IncidentView.aspx?IncidentId="+IncidentId);
		}
		#endregion
	}
}
