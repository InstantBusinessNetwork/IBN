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
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.ToDo.Modules
{
	public partial class TodoPredecessors : System.Web.UI.UserControl
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
			dgPredecessors.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_delete);

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

			if (dgPredecessors.Items.Count == 0)
				this.Visible = false;
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("tTitle"));

			/*
			using (IDataReader rdr = Mediachase.IBN.Business.ToDo.GetListVacantPredecessors(BaseToDoID))
			{
				if (rdr.Read())
				{
					CommandManager cm = CommandManager.GetCurrent(this.Page);
					CommandParameters cp = new CommandParameters("MC_PM_AddPredTodo");
					string cmd = cm.AddCommand("ToDo", "", "ToDoView", cp);
					cmd = cmd.Replace("\"", "&quot;");
					secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/task_predecessors.gif'/> " + LocRM.GetString("Add"),
						String.Format("javascript:{{{0}}}", cmd));
				}
			}
			 */

			// Add Predecessors
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters("MC_ToDo_AddPred");
			string cmd = cm.AddCommand("ToDo", "", "ToDoView", cp);
			cmd = cmd.Replace("\"", "&quot;");
			secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/task_predecessors.gif'/> " + LocRM2.GetString("AddPredecessors"),
				"javascript:" + cmd);

			secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/task_create.gif'/> " + LocRM2.GetString("Create"),
				ResolveClientUrl("~/ToDo/ToDoEdit.aspx?SuccID=" + BaseToDoID));
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			dgPredecessors.Columns[2].HeaderText = LocRM.GetString("Title");
			dgPredecessors.Columns[3].HeaderText = LocRM.GetString("EndDate");

			dgPredecessors.DataSource = Mediachase.IBN.Business.ToDo.GetListPredecessorsDetails(BaseToDoID);
			dgPredecessors.DataBind();

			foreach (DataGridItem dgi in dgPredecessors.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");
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