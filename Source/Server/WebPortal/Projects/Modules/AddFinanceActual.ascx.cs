namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Text;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Web;
	using System.Resources;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.SpreadSheet;
	using Mediachase.UI.Web.Modules;
	using Mediachase.Ibn.Web.UI;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for AddFinanceActual.
	/// </summary>
	public partial class AddFinanceActual : System.Web.UI.UserControl
	{
		#region Html Controls
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectFinances", typeof(AddFinanceActual).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(AddFinanceActual).Assembly);
		#endregion

		#region prop: ObjectId
		public int ObjectId
		{
			get
			{
				if (Request["ObjectId"] == null) return -1;
				return int.Parse(Request["ObjectId"]);
			}
		}
		#endregion

		#region prop: ObjectTypeId
		public int ObjectTypeId
		{
			get
			{
				if (Request["ObjectTypeId"] == null) return -1;
				return int.Parse(Request["ObjectTypeId"]);
			}
		}
		#endregion

		#region prop: ActualFinancesId
		public int ActualFinancesId
		{
			get
			{
				if (Request["ActualFinancesId"] == null) return -1;
				return int.Parse(Request["ActualFinancesId"]);
			}
		}
		#endregion

		#region ApplyLocalization
		void ApplyLocalization()
		{
			btnSave.Text = LocRM2.GetString("tbsave_save");
			btnCancel.Text = LocRM2.GetString("tbsave_cancel");
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();

			btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
			btnCancel.Attributes.Add("onclick", "window.close();");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			if (!IsPostBack)
			{
				BindDropDown();
				if (ActualFinancesId != -1)
					BindValues();
				else
					dtcDate.SelectedDate = DateTime.Now.Date;
			}
		}

		#region Bind: DropDown
		void BindDropDown()
		{
			Mediachase.IBN.Business.SpreadSheet.Row[] mas = ProjectSpreadSheet.GetFactAvailableRows(CommonHelper.GetProjectIdByObjectIdObjectType(ObjectId, ObjectTypeId));
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("Id", typeof(string)));

			foreach (Row row in mas)
			{
				if (!(row.Name == string.Empty && row.ReadOnly))
				{
					DataRow datarow = dt.NewRow();
					datarow["Id"] = row.Id;

					if (!row.HasChildRows && !row.ReadOnly && row.Visibility == RowVisibility.User)
						datarow["Name"] = "--- " + row.Name;
					else
						datarow["Name"] = row.Name;

					dt.Rows.Add(datarow);
				}
			}

			ddAccounts.DataSource = dt;
			ddAccounts.DataTextField = "Name";
			ddAccounts.DataValueField = "Id";
			ddAccounts.DataBind();
		}
		#endregion

		#region Bind: Values
		void BindValues()
		{
			ActualFinances item = ActualFinances.Load(ActualFinancesId);
			dtcDate.SelectedDate = item.Date;
			tbValue.Text = item.Value.ToString();
			txtDescription.Text = item.Comment;
			if (ddAccounts.Items.FindByValue(item.RowId) != null)
				ddAccounts.SelectedValue = item.RowId;
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region btnSave_ServerClick
		private void btnSave_ServerClick(object sender, EventArgs e)
		{
			//int ProjectId, DateTime Date, string RowId, double Value, string Comment
			string value = tbValue.Text.Replace(',', '.');
			if (ActualFinancesId == -1)
			{
				Mediachase.IBN.Business.SpreadSheet.ActualFinances.Create(ObjectId, (ObjectTypes)ObjectTypeId, dtcDate.SelectedDate, ddAccounts.SelectedValue, Convert.ToDouble(value, CultureInfo.InvariantCulture), txtDescription.Text);
			}
			else
			{
				ActualFinances item = ActualFinances.Load(ActualFinancesId);
				item.Date = dtcDate.SelectedDate;
				item.Value = Convert.ToDouble(value, CultureInfo.InvariantCulture);
				item.Comment = txtDescription.Text;
				item.RowId = ddAccounts.SelectedValue;
				ActualFinances.Update(item);
			}

			if (!String.IsNullOrEmpty(Request["btn"]))
				CHelper.CloseItAndRefresh(Response, Request["btn"]);
			else
				CHelper.CloseItAndRefresh(Response);
		}
		#endregion
	}
}
