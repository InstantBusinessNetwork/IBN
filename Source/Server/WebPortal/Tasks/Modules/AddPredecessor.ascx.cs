namespace Mediachase.UI.Web.Tasks.Modules
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
	using System.Globalization;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for AddPredecessor.
	/// </summary>
	public partial  class AddPredecessor : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strPredecessors", typeof(AddPredecessor).Assembly);

		#region TaskID
		private int TaskID
		{
			get
			{
				try
				{
					return int.Parse(Request["BaseTaskID"]);
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
			//btnCancel.Attributes.Add("onclick","DisableButtons(this);");
			btnSave.Attributes.Add("onclick","DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			BindToolbar();
			if (!IsPostBack)
			{
				BindDefaultValues();
			}
			if (Request["closeFramePopup"] != null)
				btnCancel.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{}} return false;", Request["closeFramePopup"]));
		}

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			tbH.Text= "0";
			tbMin.Text = "0";

			ddGroups.Items.Clear();

			ddGroups.DataTextField = "FullTitle";
			ddGroups.DataValueField = "TaskId";
			ddGroups.DataSource = Task.GetListVacantPredecessors(TaskID);
			ddGroups.DataBind();
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			btnSave.Text = LocRM.GetString("IAdd");
			btnCancel.Text = LocRM.GetString("Cancel");
		}
		#endregion

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			if (ddGroups.SelectedItem!=null)
			{
				int PTaskID = int.Parse(ddGroups.SelectedItem.Value);

				int lag;
				if (tbH.Text.Trim().StartsWith("-"))
					lag = int.Parse(tbH.Text) * 60 - int.Parse(tbMin.Text);
				else
					lag = int.Parse(tbH.Text) * 60 + int.Parse(tbMin.Text);
				Task.CreatePredecessor(PTaskID,TaskID,lag);
			}
			//Response.Redirect("../Tasks/TaskView.aspx?TaskID=" + TaskID);
			CommandParameters cp = new CommandParameters("MC_PM_TaskRedirect");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion
	}
}
