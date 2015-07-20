using System;
using System.Data;

using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Apps.Shell.Modules
{
	public partial class ActiveActivities : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Grid.css");
			BindDataGrid(!IsPostBack);
		}

		void BindDataGrid(bool dataBind)
		{
			DataTable dt = Mediachase.IBN.Business.ToDo.GetListActiveToDoAndTasksByUserOnlyDataTable();
			
			DataView dv = dt.DefaultView;

			ctrlGrid.DataSource = dv;

			divNoObjects.Visible = false;
			if (dv.Count == 0)
			{
				ctrlGrid.Visible = false;
				lblNoObjects.Text = String.Format("{0} <a href='{1}'>{2}</a>",
					GetGlobalResourceObject("IbnFramework.Global", "NoToDos").ToString(),
					this.Page.ResolveUrl("~/ToDo/ToDoEdit.aspx"),
						GetGlobalResourceObject("IbnFramework.Global", "CreateToDo").ToString());
				divNoObjects.Visible = true;
			}
		}
	}
}