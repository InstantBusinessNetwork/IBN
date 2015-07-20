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
using System.Globalization;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.ToDo.Modules
{
	public partial class AddSuccessor : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strPredecessors", Assembly.GetExecutingAssembly());

		#region ToDoID
		private int ToDoID
		{
			get
			{
				try
				{
					return int.Parse(Request["BaseToDoID"]);
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
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
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
			ddGroups.Items.Clear();

			ddGroups.DataTextField = "Title";
			ddGroups.DataValueField = "ToDoId";
			ddGroups.DataSource = Mediachase.IBN.Business.ToDo.GetListVacantSuccessors(ToDoID);
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
			if (ddGroups.SelectedItem != null)
			{
				int PTodoID = int.Parse(ddGroups.SelectedValue);

				Mediachase.IBN.Business.ToDo.CreateToDoLink(ToDoID, PTodoID);
			}
			CommandParameters cp = new CommandParameters("MC_PM_ToDoRedirect");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion
	}
}