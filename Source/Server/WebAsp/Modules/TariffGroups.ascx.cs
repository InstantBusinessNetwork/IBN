using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using System.Reflection;

namespace Mediachase.Ibn.WebAsp.Modules
{
	public partial class TariffGroups : System.Web.UI.UserControl
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
			dgGroups.Columns[0].HeaderText = LocRM.GetString("TariffTypeName");
			dgGroups.Columns[1].HeaderText = LocRM.GetString("TariffIsActive");

			dgGroups.DataSource = Tariff.GetTariffType(0);
			dgGroups.DataBind();

			foreach (DataGridItem dgi in dgGroups.Items)
			{
				ImageButton ibDelete = (ImageButton)dgi.FindControl("ibDelete");
				if (ibDelete != null)
					ibDelete.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("TariffTypeWarning") + "')");
			}
		}

		private void BindToolbar()
		{
			dgGroups.DeleteCommand += new DataGridCommandEventHandler(dgGroups_DeleteCommand);
			secHeader.Title = LocRM.GetString("TariffGroups");
			secHeader.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' /> {1}",
				this.Page.ResolveClientUrl("~/Layouts/Images/newitem.gif"), LocRM.GetString("TariffGroupCreate")),
				this.Page.ResolveUrl("~/Pages/TariffGroupEdit.aspx"));
			secHeader.AddSeparator();
			secHeader.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' /> {1}",
				this.Page.ResolveClientUrl("~/Layouts/Images/cancel.gif"), LocRM.GetString("TariffsBlock")), 
				this.Page.ResolveUrl("~/Pages/ASPHome.aspx?Tab=4"));
		}

		void dgGroups_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int id = int.Parse(e.CommandArgument.ToString());
			Tariff.DeleteTariffType(id);
			Response.Redirect("~/Pages/TariffGroups.aspx");
		}
	}
}