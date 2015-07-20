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
	public partial class TariffGroupEdit : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.Tariffs", Assembly.GetExecutingAssembly());
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack && Request["TypeId"] != null)
				BindValues();
			BindToolbar();
		}

		private void BindValues()
		{
			using (IDataReader reader = Tariff.GetTariffType(int.Parse(Request["TypeId"])))
			{
				if (reader.Read())
				{
					txtName.Text = reader["TypeName"].ToString();
					cbIsActive.Checked = (bool)reader["IsActive"];
				}
			}
		}

		private void BindToolbar()
		{
			cbIsActive.Text = "&nbsp;" + LocRM.GetString("TariffIsActive");
			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
			btnSave.Click += new EventHandler(btnSave_Click);
			btnCancel.Click += new EventHandler(btnCancel_Click);

			if (Request["TypeId"] != null)
				secHeader.Title = LocRM.GetString("TariffGroupEdit");
			else
				secHeader.Title = LocRM.GetString("TariffGroupCreate");
			secHeader.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' /> {1}",
				this.Page.ResolveClientUrl("~/Layouts/Images/cancel.gif"), LocRM.GetString("TariffGroups")),
				this.Page.ResolveUrl("~/Pages/TariffGroups.aspx"));
		}

		void btnCancel_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Pages/TariffGroups.aspx");
		}

		void btnSave_Click(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;
			if (Request["TypeId"] == null)
				Tariff.AddTariffType(txtName.Text, cbIsActive.Checked);
			else
				Tariff.UpdateTariffType(int.Parse(Request["TypeId"]), txtName.Text, cbIsActive.Checked);
			Response.Redirect("~/Pages/TariffGroups.aspx");
		}
	}
}