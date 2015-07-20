using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using System.Reflection;

namespace Mediachase.Ibn.WebAsp.Modules
{
	public partial class PaymentCreate : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.Tariffs", Assembly.GetExecutingAssembly());
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
				BindValues();
			BindToolbar();
		}

		private void BindValues()
		{
			DataView dv = CManage.GetCompaniesDataTable().DefaultView;
			dv.Sort = "Company_Name";
			ddCompanies.DataSource = dv;
			ddCompanies.DataTextField = "Company_Name";
			ddCompanies.DataValueField = "Company_Uid";
			ddCompanies.DataBind();
			txtAmount.Text = "0";
			txtBonus.Text = "0";
			PaymentDate.Text = DateTime.Today.ToShortDateString();
		}

		private void BindToolbar()
		{
			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
			btnSave.Click += new EventHandler(btnSave_Click);
			btnCancel.Click += new EventHandler(btnCancel_Click);

			secHeader.Title = LocRM.GetString("PaymentCreate");
			secHeader.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' /> {1}",
				this.Page.ResolveClientUrl("~/Layouts/Images/cancel.gif"), LocRM.GetString("Payments")),
				this.Page.ResolveUrl("~/Pages/Payments.aspx"));
		}

		void btnCancel_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Pages/Payments.aspx");
		}

		void btnSave_Click(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			DateTime dt = DateTime.Parse(PaymentDate.Text);
			Tariff.AddPayment(new Guid(ddCompanies.SelectedValue), dt, decimal.Parse(txtAmount.Text), decimal.Parse(txtBonus.Text), OrderNo.Text.Trim(), false);
			Response.Redirect("~/Pages/Payments.aspx");
		}
	}
}