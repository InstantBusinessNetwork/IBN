using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using System.Reflection;
using System.Data;

namespace Mediachase.Ibn.WebAsp.Modules
{
	public partial class CurrencyEdit : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.Tariffs", Assembly.GetExecutingAssembly());
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack && Request["CurrencyId"] != null)
				BindValues();
			BindToolbar();
		}

		private void BindValues()
		{
			using (IDataReader reader = Tariff.GetCurrency(int.Parse(Request["CurrencyId"])))
			{
				if (reader.Read())
				{
					txtName.Text = reader["CurrencyName"].ToString();
					txtSymbol.Text = reader["Symbol"].ToString();
				}
			}
		}

		private void BindToolbar()
		{
			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
			btnSave.Click += new EventHandler(btnSave_Click);
			btnCancel.Click += new EventHandler(btnCancel_Click);

			if (Request["CurrencyId"] != null)
				secHeader.Title = LocRM.GetString("CurrencyEdit");
			else
				secHeader.Title = LocRM.GetString("CurrencyCreate");
			secHeader.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' /> {1}",
				this.Page.ResolveClientUrl("~/Layouts/Images/cancel.gif"), LocRM.GetString("Currencies")),
				this.Page.ResolveUrl("~/Pages/Currencies.aspx"));
		}

		void btnCancel_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Pages/Currencies.aspx");
		}

		void btnSave_Click(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;
			if (Request["CurrencyId"] == null)
				Tariff.AddCurrency(txtName.Text, txtSymbol.Text);
			else
				Tariff.UpdateCurrency(int.Parse(Request["CurrencyId"]), txtName.Text, txtSymbol.Text);
			Response.Redirect("~/Pages/Currencies.aspx");
		}
	}
}