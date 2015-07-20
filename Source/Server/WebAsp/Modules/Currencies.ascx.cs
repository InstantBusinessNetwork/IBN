using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using System.Reflection;

namespace Mediachase.Ibn.WebAsp.Modules
{
	public partial class Currencies : System.Web.UI.UserControl
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
			dgCurrencies.Columns[0].HeaderText = LocRM.GetString("CurrencyName");
			dgCurrencies.Columns[1].HeaderText = LocRM.GetString("CurrencySymbol");

			dgCurrencies.DataSource = Tariff.GetCurrency(0);
			dgCurrencies.DataBind();

			foreach (DataGridItem dgi in dgCurrencies.Items)
			{
				ImageButton ibDelete = (ImageButton)dgi.FindControl("ibDelete");
				if (ibDelete != null)
					ibDelete.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("CurrencyWarning") + "')");
			}
		}

		private void BindToolbar()
		{
			dgCurrencies.DeleteCommand += new DataGridCommandEventHandler(dgCurrencies_DeleteCommand);
			secHeader.Title = LocRM.GetString("Currencies");
			secHeader.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' /> {1}",
				this.Page.ResolveClientUrl("~/Layouts/Images/newitem.gif"), LocRM.GetString("CurrencyCreate")),
				this.Page.ResolveUrl("~/Pages/CurrencyEdit.aspx"));
			secHeader.AddSeparator();
			secHeader.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' /> {1}",
				this.Page.ResolveClientUrl("~/Layouts/Images/cancel.gif"), LocRM.GetString("TariffsBlock")),
				this.Page.ResolveUrl("~/Pages/ASPHome.aspx?Tab=4"));
		}

		void dgCurrencies_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int id = int.Parse(e.CommandArgument.ToString());
			Tariff.DeleteCurrency(id);
			Response.Redirect("~/Pages/Currencies.aspx");
		}
	}
}