using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Resources;
using System.Reflection;
using Mediachase.Ibn;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.ToDo.Modules
{
	public partial class TodoSuccessors : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strPredecessors", Assembly.GetExecutingAssembly());
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strToDoView", Assembly.GetExecutingAssembly());

		#region BaseToDoID
		protected int BaseToDoID
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

		protected void Page_Load(object sender, EventArgs e)
		{
			dgSuccessors.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_delete);

			if (!Mediachase.IBN.Business.ToDo.CanUpdate(BaseToDoID))
			{
				this.Visible = false;
				return;
			}

			this.Visible = true;

			if (!IsPostBack)
				BindDG();
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolbar();

			if (CHelper.NeedToBindGrid())
				BindDG();

			if (dgSuccessors.Items.Count == 0)
				this.Visible = false;
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("tTitleS"));

			/*
			using (IDataReader rdr = Mediachase.IBN.Business.ToDo.GetListVacantSuccessors(BaseToDoID))
			{
				if (rdr.Read())
				{
					CommandManager cm = CommandManager.GetCurrent(this.Page);
					CommandParameters cp = new CommandParameters("MC_PM_AddSuccTodo");
					string cmd = cm.AddCommand("ToDo", "", "ToDoView", cp);
					cmd = cmd.Replace("\"", "&quot;");
					secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/task_sucessors.gif'/> " + LocRM.GetString("AddSuc"),
						String.Format("javascript:{{{0}}}", cmd));
				}
			}*/

			// Add Successors
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters("MC_ToDo_AddSucc");
			string cmd = cm.AddCommand("ToDo", "", "ToDoView", cp);
			cmd = cmd.Replace("\"", "&quot;");
			secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/task_sucessors.gif'/> " + LocRM2.GetString("AddSuccessors"),
				"javascript:" + cmd);

			secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/task_create.gif'/> " + LocRM2.GetString("Create"),
				ResolveClientUrl("~/ToDo/ToDoEdit.aspx?PredID=" + BaseToDoID));
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			dgSuccessors.Columns[2].HeaderText = LocRM.GetString("Title");
			dgSuccessors.Columns[3].HeaderText = LocRM.GetString("StartDate");

			dgSuccessors.DataSource = Mediachase.IBN.Business.ToDo.GetListSuccessorsDetails(BaseToDoID);
			dgSuccessors.DataBind();

			foreach (DataGridItem dgi in dgSuccessors.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("WarningS") + "')");
			}
		}
		#endregion

		#region dg_delete
		private void dg_delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int LinkID = int.Parse(e.Item.Cells[1].Text);

			Mediachase.IBN.Business.ToDo.DeleteToDoLink(LinkID);

			Response.Redirect("../ToDo/ToDoView.aspx?ToDoID=" + BaseToDoID, true);
		}
		#endregion
	}
}