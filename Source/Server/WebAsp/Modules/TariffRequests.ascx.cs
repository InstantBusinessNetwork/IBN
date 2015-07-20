using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using System.Reflection;

namespace Mediachase.Ibn.WebAsp.Modules
{
	public partial class TariffRequests : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.Tariffs", Assembly.GetExecutingAssembly());
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
				BindDG();
			BindToolbar();
		}
		private void BindDG()
		{
			dgTariffReqs.Columns[0].HeaderText = LocRM.GetString("CompanyName");
			dgTariffReqs.Columns[1].HeaderText = LocRM.GetString("TariffName");
			dgTariffReqs.Columns[2].HeaderText = LocRM.GetString("Description");
			dgTariffReqs.Columns[3].HeaderText = LocRM.GetString("Created");

			dgTariffReqs.DataSource = Tariff.GetTariffRequests(0);
			dgTariffReqs.DataBind();

			foreach (DataGridItem dgi in dgTariffReqs.Items)
			{
				ImageButton ibDelete = (ImageButton)dgi.FindControl("ibDelete");
				if (ibDelete != null)
					ibDelete.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("TariffRequestWarning") + "')");
			}
		}

		private void BindToolbar()
		{
			dgTariffReqs.DeleteCommand += new DataGridCommandEventHandler(dgTariffReqs_DeleteCommand);
			secHeader.Title = LocRM.GetString("TariffRequests");
			secHeader.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' /> {1}",
				this.Page.ResolveClientUrl("~/Layouts/Images/cancel.gif"), LocRM.GetString("TariffsBlock")),
				this.Page.ResolveUrl("~/Pages/ASPHome.aspx?Tab=4"));
		}

		void dgTariffReqs_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int id = int.Parse(e.CommandArgument.ToString());
			Tariff.DeleteTariffRequest(id);
			Response.Redirect("~/Pages/TariffRequests.aspx");
		}

	}
}