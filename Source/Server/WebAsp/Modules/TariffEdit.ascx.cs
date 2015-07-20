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
	public partial class TariffEdit : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.Tariffs", Assembly.GetExecutingAssembly());
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
				BindValues();
			BindToolbar();
		}

		private void BindLists()
		{
			ddType.Items.Clear();
			ddType.DataSource = Tariff.GetTariffType(0);
			ddType.DataTextField = "TypeName";
			ddType.DataValueField = "TypeId";
			ddType.DataBind();

			ddCurrency.Items.Clear();
			ddCurrency.DataSource = Tariff.GetCurrency(0);
			ddCurrency.DataTextField = "CurrencyName";
			ddCurrency.DataValueField = "CurrencyId";
			ddCurrency.DataBind();

			txtMaxHdd.Text = "0";
			txtMaxUsers.Text = "0";
			txtMaxExtUsers.Text = "0";
			txtMonthlyCost.Text = ((decimal)0).ToString("f");
		}

		private void BindValues()
		{
			BindLists();
			if(Request["TariffId"] != null)
				using (IDataReader reader = Tariff.GetTariff(int.Parse(Request["TariffId"]), 0))
				{
					if (reader.Read())
					{
						txtName.Text = reader["TariffName"].ToString();
						if (reader["Description"] != DBNull.Value)
							txtDescription.Text = reader["Description"].ToString();

						SafeSelect(ddType, reader["TypeId"].ToString());
						SafeSelect(ddCurrency, reader["CurrencyId"].ToString());

						if (reader["MaxHdd"] != DBNull.Value)
							txtMaxHdd.Text = reader["MaxHdd"].ToString();
						if (reader["MaxUsers"] != DBNull.Value)
							txtMaxUsers.Text = reader["MaxUsers"].ToString();
						if (reader["MaxExternalUsers"] != DBNull.Value)
							txtMaxExtUsers.Text = reader["MaxExternalUsers"].ToString();
						if (reader["MonthlyCost"] != DBNull.Value)
							txtMonthlyCost.Text = ((decimal)reader["MonthlyCost"]).ToString("f");
					}
				}
		}

		private void BindToolbar()
		{
			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
			btnSave.Click += new EventHandler(btnSave_Click);
			btnCancel.Click += new EventHandler(btnCancel_Click);

			if (Request["TariffId"] != null)
				secHeader.Title = LocRM.GetString("TariffEdit");
			else
				secHeader.Title = LocRM.GetString("TariffCreate");
			secHeader.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' /> {1}",
				this.Page.ResolveClientUrl("~/Layouts/Images/cancel.gif"), LocRM.GetString("TariffsList")),
				this.Page.ResolveUrl("~/Pages/TariffList.aspx"));
		}

		void btnCancel_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Pages/TariffList.aspx");
		}

		void btnSave_Click(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;
			if (Request["TariffId"] == null)
				Tariff.AddTariff(txtName.Text, txtDescription.Text, int.Parse(ddType.SelectedValue),
					int.Parse(ddCurrency.SelectedValue), decimal.Parse(txtMonthlyCost.Text), 
					int.Parse(txtMaxHdd.Text), int.Parse(txtMaxUsers.Text), int.Parse(txtMaxExtUsers.Text));
			else
				Tariff.UpdateTariff(int.Parse(Request["TariffId"]), txtName.Text, txtDescription.Text, int.Parse(ddType.SelectedValue),
					int.Parse(ddCurrency.SelectedValue), decimal.Parse(txtMonthlyCost.Text), 
					int.Parse(txtMaxHdd.Text), int.Parse(txtMaxUsers.Text), int.Parse(txtMaxExtUsers.Text));
			Response.Redirect("~/Pages/TariffList.aspx");
		}

		#region SafeSelect
		public static void SafeSelect(ListControl ddl, string val)
		{
			ListItem li = ddl.Items.FindByValue(val);
			if (li != null)
			{
				ddl.ClearSelection();
				li.Selected = true;
			}
		}
		#endregion
	}
}