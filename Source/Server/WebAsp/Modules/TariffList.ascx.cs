using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using System.Reflection;

namespace Mediachase.Ibn.WebAsp.Modules
{
	public partial class TariffList : System.Web.UI.UserControl
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
			dgTariffs.Columns[0].HeaderText = LocRM.GetString("TariffName");
			dgTariffs.Columns[1].HeaderText = LocRM.GetString("TariffTypeName");
			dgTariffs.Columns[2].HeaderText = LocRM.GetString("TariffCurrency");
			dgTariffs.Columns[3].HeaderText = LocRM.GetString("MaxHdd");
			dgTariffs.Columns[4].HeaderText = LocRM.GetString("MaxUsers");
			dgTariffs.Columns[5].HeaderText = LocRM.GetString("MaxExternalUsers");
			dgTariffs.Columns[6].HeaderText = LocRM.GetString("MonthlyCost");

			dgTariffs.DataSource = Tariff.GetTariff(0, 0);
			dgTariffs.DataBind();

			foreach (DataGridItem dgi in dgTariffs.Items)
			{
				ImageButton ibDelete = (ImageButton)dgi.FindControl("ibDelete");
				if (ibDelete != null)
					ibDelete.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("TariffWarning") + "')");
			}
		}

		private void BindToolbar()
		{
			dgTariffs.DeleteCommand += new DataGridCommandEventHandler(dgTariffs_DeleteCommand);
			secHeader.Title = LocRM.GetString("TariffsList");
			secHeader.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' /> {1}",
				this.Page.ResolveClientUrl("~/Layouts/Images/newitem.gif"), LocRM.GetString("TariffCreate")),
				this.Page.ResolveUrl("~/Pages/TariffEdit.aspx"));
			secHeader.AddSeparator();
			secHeader.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' /> {1}",
				this.Page.ResolveClientUrl("~/Layouts/Images/cancel.gif"), LocRM.GetString("TariffsBlock")),
				this.Page.ResolveUrl("~/Pages/ASPHome.aspx?Tab=4"));
		}

		void dgTariffs_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int id = int.Parse(e.CommandArgument.ToString());
			Tariff.DeleteTariff(id);
			Response.Redirect("~/Pages/TariffList.aspx");
		}
	}
}