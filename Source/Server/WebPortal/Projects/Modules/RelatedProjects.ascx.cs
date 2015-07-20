namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;

	using ComponentArt.Web.UI;
	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.UI.Web.Modules;
	using Mediachase.Ibn.Web.UI.WebControls;
	using Mediachase.Ibn.Web.UI;

	/// <summary>
	///		Summary description for RelatedProjects.
	/// </summary>
	public partial class RelatedProjects : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(RelatedProjects).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(RelatedProjects).Assembly);

		#region ProjectId
		protected int ProjectId
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectId"]);
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

			this.Visible = (dgRelatedPrjs.Items.Count > 0);
		}

		#region BindDG
		private void BindDG()
		{
			dgRelatedPrjs.Columns[1].HeaderText = LocRM.GetString("title");
			dgRelatedPrjs.Columns[2].HeaderText = LocRM.GetString("manager");

			dgRelatedPrjs.DataSource = Project.GetListProjectRelationsDataTable(ProjectId).DefaultView;
			dgRelatedPrjs.DataBind();

			foreach (DataGridItem dgi in dgRelatedPrjs.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
				{
					ib.ToolTip = LocRM.GetString("Delete");
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning") + "')");
				}
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("tRelProjects"));

			if (Project.CanUpdate(ProjectId))
			{
				ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
				topMenuItem.Text = /*"<img border='0' src='../Layouts/Images/downbtn.gif' width='9px' height='5px' align='absmiddle'/>&nbsp;" + */LocRM2.GetString("Actions");
				topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
				topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
				topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
				topMenuItem.LookId = "TopItemLook";

				ComponentArt.Web.UI.MenuItem subItem;

				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters();
				string cmd = String.Empty;

				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/xp-paste.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				cp = new CommandParameters("MC_PM_RelatedPrjClipboard");
				cmd = cm.AddCommand("Project", "", "ProjectView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd;
				subItem.Text = LocRM2.GetString("tPastePrjFromClipboard");
				topMenuItem.Items.Add(subItem);

				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/relprojects.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				cp = new CommandParameters("MC_PM_RelatedPrj");
				cmd = cm.AddCommand("Project", "", "ProjectView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd;
				subItem.Text = LocRM2.GetString("AddRelated");
				topMenuItem.Items.Add(subItem);

				secHeader.ActionsMenu.Items.Add(topMenuItem);
			}
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

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgRelatedPrjs.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_delete);
			this.btnAddRelatedPrj.Click += new EventHandler(btnAddRelatedPrj_Click);
		}
		#endregion

		#region dg_delete
		private void dg_delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int RelProjectId = int.Parse(e.Item.Cells[0].Text);
			Project2.DeleteRelation(ProjectId, RelProjectId);
			Response.Redirect("../Projects/ProjectView.aspx?ProjectId=" + ProjectId);
		}
		#endregion

		void btnAddRelatedPrj_Click(object sender, EventArgs e)
		{
			string param = Request["__EVENTARGUMENT"];
			if (!String.IsNullOrEmpty(param))
			{
				string[] mas = param.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				if (mas.Length < 2 || !mas[0].Equals("3"))
					return;
				int iRelId = int.Parse(mas[1]);
				Project2.AddRelation(ProjectId, iRelId);
				BindDG();
			}
		}
	}
}
