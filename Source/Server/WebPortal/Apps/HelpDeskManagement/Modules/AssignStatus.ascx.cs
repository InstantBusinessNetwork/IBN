using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules
{
	public partial class AssignStatus : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			divErrors.Visible = false;
			btnSave.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Save").ToString();
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
			btnCancel.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Cancel").ToString();
			btnCancel.CustomImage = this.Page.ResolveUrl("~/layouts/images/cancel.gif");

			btnClose.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Close").ToString();
			btnClose.CustomImage = this.Page.ResolveUrl("~/layouts/images/cancel.gif");
			btnClose.ServerClick += new EventHandler(btnClose_ServerClick);

			if (Request["closeFramePopup"] != null)
				btnCancel.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{;}}return false;", Request["closeFramePopup"]));

			if (!Page.IsPostBack)
			{
				BindStatuses();
				ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString("N"),
					"GetIds();", true);
			}
		}

		private void BindStatuses()
		{
			ddStatus.Items.Clear();
			DataTable dt = Incident.GetListIncidentStatesDataTable();
			DataView dv = dt.DefaultView;
			dv.RowFilter = "StateId <> 1 AND StateId <> 3";
			ddStatus.DataSource = dv;
			ddStatus.DataTextField = "StateName";
			ddStatus.DataValueField = "StateId";
			ddStatus.DataBind();
			ddStatus.Items.Insert(0, new ListItem("", "0"));
		}

		void btnSave_ServerClick(object sender, EventArgs e)
		{
			string values = hfValues.Value;
			if (!String.IsNullOrEmpty(values))
			{
				int state_id = int.Parse(ddStatus.SelectedValue);
				if (state_id > 0)
				{
					string sMessage = txtComment.Text;
					sMessage = Mediachase.UI.Web.Util.CommonHelper.parsetext_br(sMessage);
					ArrayList errors = new ArrayList();
					string[] elems = values.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string s in elems)
					{
						int id = Convert.ToInt32(s, CultureInfo.InvariantCulture);
						try
						{
							Issue2.UpdateQuickTracking(id, sMessage, state_id);
						}
						catch
						{
							errors.Add(id);
						}
					}

					if (errors.Count > 0)
					{
						divErrors.Visible = true;
						tblMain.Visible = false;
						ShowErrors(errors);
					}
					else
						CloseThis();
				}
				else
					CloseThis();
			}
			else
				CloseThis();
		}

		private void ShowErrors(ArrayList list)
		{
			string sIncs = String.Empty;
			foreach (int id in list)
			{
				sIncs += "<div style='padding-left:5px;'>-&nbsp;&nbsp;" + Incident.GetTitle(id) + "</div>";
			}
			lblResult.Text = String.Format(GetGlobalResourceObject("IbnFramework.Incident", "StatusNotChanged").ToString(), ddStatus.SelectedItem.Text, sIncs);
		}

		void btnClose_ServerClick(object sender, EventArgs e)
		{
			CloseThis();
		}

		private void CloseThis()
		{
			CHelper.RequireBindGrid();
			if (!String.IsNullOrEmpty(Request["commandName"]))
			{
				string commandName = Request["commandName"];
				CommandParameters cp = new CommandParameters(commandName);
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, String.Empty);
		}
	}
}