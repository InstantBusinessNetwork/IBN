using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using System.Reflection;

namespace Mediachase.Ibn.WebAsp.Modules
{
	public partial class Payments : System.Web.UI.UserControl
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
			dgPayments.Columns[0].HeaderText = LocRM.GetString("CompanyName");
			dgPayments.Columns[1].HeaderText = LocRM.GetString("PaymentDate");
			dgPayments.Columns[2].HeaderText = LocRM.GetString("Amount");
			dgPayments.Columns[3].HeaderText = LocRM.GetString("Bonus");
			dgPayments.Columns[4].HeaderText = LocRM.GetString("OrderNo");

			dgPayments.DataSource = Tariff.GetPayment(0, Guid.Empty);
			dgPayments.DataBind();

			foreach (DataGridItem dgi in dgPayments.Items)
			{
				ImageButton ibDelete = (ImageButton)dgi.FindControl("ibDelete");
				if (ibDelete != null)
					ibDelete.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("PaymentWarning") + "')");

				ImageButton ibUndo = (ImageButton)dgi.FindControl("ibUndo");
				if (ibUndo != null)
					ibUndo.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("PaymentWarningUndo") + "')");
			}
		}

		private void BindToolbar()
		{
			dgPayments.DeleteCommand += new DataGridCommandEventHandler(dgPayments_DeleteCommand);
			secHeader.Title = LocRM.GetString("Payments");
			secHeader.AddLink(String.Format("<img alt='' src='{0}' /> {1}",
				this.Page.ResolveClientUrl("~/Layouts/Images/newitem.gif"), LocRM.GetString("PaymentCreate")),
				this.Page.ResolveUrl("~/Pages/PaymentCreate.aspx"));
			secHeader.AddSeparator();
			secHeader.AddLink(String.Format("<img alt='' src='{0}' /> {1}",
				this.Page.ResolveClientUrl("~/Layouts/Images/cancel.gif"), LocRM.GetString("TariffsBlock")),
				this.Page.ResolveUrl("~/Pages/ASPHome.aspx?Tab=4"));
		}

		protected void dgPayments_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			dgPayments.CurrentPageIndex = e.NewPageIndex;
			BindDG();
		}

		void dgPayments_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int id = int.Parse(e.CommandArgument.ToString());
			Tariff.DeletePayment(id);
			Response.Redirect("~/Pages/Payments.aspx");
		}

		protected void dgPayments_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			int id = int.Parse(e.CommandArgument.ToString());
			Tariff.UndoPayment(id);
			Response.Redirect("~/Pages/Payments.aspx");
		}

	}
}