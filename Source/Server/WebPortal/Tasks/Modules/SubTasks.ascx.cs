namespace Mediachase.UI.Web.Tasks.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;

	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.UI.Web.Modules;
	using Mediachase.Ibn;

	/// <summary>
	///		Summary description for SubTasks.
	/// </summary>
	public partial class SubTasks : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strTaskGeneral", typeof(SubTasks).Assembly);

		#region SummaryTaskID
		protected int SummaryTaskID
		{
			get 
			{
				try 
				{
					return int.Parse(Request["TaskID"]);
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
			if (Security.CurrentUser.IsExternal)
			{
				this.Visible = false;
				return;
			}

			this.Visible = true;
			if (!IsPostBack)
				BindDG();
			BindToolbar();
		}

		#region BindDG
		private void BindDG()
		{
			dgSubTasks.Columns[1].HeaderText = LocRM.GetString("Title");
			dgSubTasks.Columns[2].HeaderText = LocRM.GetString("StartDate");
			dgSubTasks.Columns[3].HeaderText = LocRM.GetString("DueDate");
			dgSubTasks.Columns[4].HeaderText = LocRM.GetString("OverallStatus");

			dgSubTasks.DataSource = Task.GetListChildrenInfo(SummaryTaskID);
			dgSubTasks.DataBind();
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			if (dgSubTasks.Items.Count == 0)
				this.Visible = false;
			secHeader.AddText(LocRM.GetString("tTitleSubTasks"));
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
		}
		#endregion
	}
}
