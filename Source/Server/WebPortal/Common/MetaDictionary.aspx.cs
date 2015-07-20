using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using System.Reflection;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Common
{
	/// <summary>
	/// Summary description for MetaDictionary.
	/// </summary>
	public partial class MetaDictionary : System.Web.UI.Page
	{
		protected System.Web.UI.HtmlControls.HtmlTable tblDictItems;

		protected ResourceManager LocRMDict = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strManageDictionaries", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		private MetaField _field;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			_field = MetaField.Load(GetFieldID());

			ApplyLocalization();
			if (!Page.IsPostBack)
				BindDG();
		}

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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgDic.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_cancel);
			this.dgDic.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_edit);
			this.dgDic.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_update);
			this.dgDic.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_delete);
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnAddNewItem.Text = LocRMDict.GetString("AddNewItem");

			secItemsHeader.Title = LocRMDict.GetString("DictItems");

			secItemsHeader.AddLink("<img alt='' src='../Layouts/Images/Sort-Ascending.png'/> " + LocRMDict.GetString("tSortAsc"), Page.ClientScript.GetPostBackClientHyperlink(lbSortAsc, ""));
			secItemsHeader.AddLink("<img alt='' src='../Layouts/Images/Sort-Descending.png'/> " + LocRMDict.GetString("tSortDesc"), Page.ClientScript.GetPostBackClientHyperlink(lbSortDesc, ""));
			secItemsHeader.AddLink("<img alt='' src='../Layouts/Images/newitem.gif'/> " + LocRMDict.GetString("AddNewItem"), Page.ClientScript.GetPostBackClientHyperlink(btnAddNewItem, ""));
			secItemsHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRMDict.GetString("Exit"), "javascript:FuncSave();");
		}
		#endregion

		#region GetFieldID
		private int GetFieldID()
		{
			try
			{
				return int.Parse(Request["ID"]);
			}
			catch
			{
				return -1;
			}
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			dgDic.Columns[1].HeaderText = LocRMDict.GetString("Name");

			dgDic.DataSource = _field.Dictionary;
			dgDic.DataBind();

			foreach (DataGridItem dgi in dgDic.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRMDict.GetString("Warning") + "')");

				RequiredFieldValidator rf = (RequiredFieldValidator)dgi.FindControl("rfName");
				if (rf != null)
					rf.ErrorMessage = LocRMDict.GetString("Required");

				DropDownList ddl = (DropDownList)dgi.FindControl("ddIndex");
				if (ddl != null)
				{
					for (int i = 1; i <= _field.Dictionary.Count; i++)
						ddl.Items.Add(new ListItem(i.ToString(), i.ToString()));
					ddl.SelectedValue = (dgDic.EditItemIndex + 1).ToString();
				}
			}
		}
		#endregion

		#region btnAddNewItem_Click
		protected void btnAddNewItem_Click(object sender, System.EventArgs e)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Index", typeof(int)));
			dt.Columns.Add(new DataColumn("Value", typeof(string)));

			DataRow row;
			foreach (MetaDictionaryItem item in _field.Dictionary)
			{
				row = dt.NewRow();
				row["Id"] = item.Id;
				row["Value"] = item.Value;
				row["Index"] = item.Index;
				dt.Rows.Add(row);
			}

			row = dt.NewRow();
			row["Id"] = -1;
			row["Value"] = "";
			row["Index"] = -1;
			dt.Rows.Add(row);

			dgDic.EditItemIndex = dt.Rows.Count - 1;
			dgDic.DataSource = dt.DefaultView;
			dgDic.DataBind();

			foreach (DataGridItem dgi in dgDic.Items)
			{
				DropDownList ddl = (DropDownList)dgi.FindControl("ddIndex");
				if (ddl != null)
				{
					for (int i = 1; i <= dt.Rows.Count; i++)
						ddl.Items.Add(new ListItem(i.ToString(), i.ToString()));
					ddl.SelectedValue = dt.Rows.Count.ToString();
				}
			}
		}
		#endregion

		#region SortingDictionary
		protected void lbSortAsc_Click(object sender, System.EventArgs e)
		{
			_field.Dictionary.SortByValue(true);
			BindDG();
		}

		protected void lbSortDesc_Click(object sender, System.EventArgs e)
		{
			_field.Dictionary.SortByValue(false);
			BindDG();
		}
		#endregion

		#region DataGrid Event Handlers
		private void dg_delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int itemId = int.Parse(e.Item.Cells[0].Text);
			_field.Dictionary.Delete(itemId);

			dgDic.EditItemIndex = -1;
			BindDG();
		}

		private void dg_edit(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgDic.EditItemIndex = e.Item.ItemIndex;
			BindDG();
		}

		private void dg_cancel(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgDic.EditItemIndex = -1;
			BindDG();
		}

		private void dg_update(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int itemId = int.Parse(e.Item.Cells[0].Text);
			TextBox tb = (TextBox)e.Item.FindControl("tbName");
			DropDownList ddl = (DropDownList)e.Item.FindControl("ddIndex");
			if (tb != null && ddl != null)
			{
				if (itemId > 0)
					_field.Dictionary.Update(itemId, int.Parse(ddl.SelectedValue) - 1, tb.Text);
				else
					_field.Dictionary.Insert(int.Parse(ddl.SelectedValue) - 1, tb.Text);
			}

			dgDic.EditItemIndex = -1;
			BindDG();
		}
		#endregion

		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				"try {window.opener.document.forms[0].submit();}" +
				"catch (e){} window.close();", true);
		}

	}
}
