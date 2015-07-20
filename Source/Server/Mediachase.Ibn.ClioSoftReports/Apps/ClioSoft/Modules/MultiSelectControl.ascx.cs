using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Apps.ClioSoft.Modules
{
	public partial class MultiSelectControl : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.ClioSoftReports.App_GlobalResources.Report", typeof(MultiSelectControl).Assembly);

		#region event: OnValueChange
		public delegate void ValueChanged(Object sender, EventArgs e);

		public event ValueChanged ValueChange;

		protected virtual void OnValueChange(Object sender, EventArgs e)
		{
			if (ValueChange != null)
			{
				ValueChange(this, e);
			}
		}
		#endregion

		#region Width
		public string Width
		{
			set
			{
				MainTable.Width = value;
			}
			get
			{
				return MainTable.Width;
			}
		}
		#endregion

		#region Value
		private string[] _value;
		public string[] Value
		{
			set
			{
				_value = SetCheckedItems(value);

				SetText();
			}
			get
			{
				if (_value == null)
					_value = GetCheckedItems();
				return _value;
			}
		}
		#endregion

		#region DataValueField
		public string DataValueField
		{
			set
			{
				ViewState["DataValueField"] = value;
			}
			get
			{
				string retval = "Key";
				if (ViewState["DataValueField"] != null)
					retval = ViewState["DataValueField"].ToString();
				return retval;
			}
		}
		#endregion

		#region DataTextField
		public string DataTextField
		{
			set
			{
				ViewState["DataTextField"] = value;
			}
			get
			{
				string retval = "Value";
				if (ViewState["DataTextField"] != null)
					retval = ViewState["DataTextField"].ToString();
				return retval;
			}
		}
		#endregion

		#region DataSource
		public object DataSource
		{
			set
			{
				MainGrid.DataSource = value;
			}
		}
		#endregion

		#region AnyText
		public string AnyText
		{
			set
			{
				ViewState["AnyText"] = value;
			}
			get
			{
				string retval = LocRM.GetString("AnyHe");
				if (ViewState["AnyText"] != null)
					retval = ViewState["AnyText"].ToString();
				return retval;
			}
		}
		#endregion

		#region NotSetText
		public string NotSetText
		{
			set
			{
				ViewState["NotSetText"] = value;
			}
			get
			{
				string retval = LocRM.GetString("NotSetHe");
				if (ViewState["NotSetText"] != null)
					retval = ViewState["NotSetText"].ToString();
				return retval;
			}
		}
		#endregion

		#region Items
		public DataGridItemCollection Items
		{
			get
			{
				return MainGrid.Items;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				ApplyLocalization();
			}
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			ChangeLabel.Text = String.Format(
				CultureInfo.InvariantCulture,
				"<img class='btndown2' border='0' src='{0}' />",
				ResolveClientUrl("~/Layouts/Images/downbtn.gif"));

			AllItemsCheckBox.Text = LocRM.GetString("SelectAll");

			SaveButton.Text = LocRM.GetString("OK");
		}
		#endregion

		#region GetCheckedItems
		private string[] GetCheckedItems()
		{
			List<string> lst = new List<string>();
			if (AllItemsCheckBox.Checked)
			{
				lst.Add("0");
			}
			else
			{
				foreach (DataGridItem dgi in MainGrid.Items)
				{
					foreach (Control control in dgi.Cells[1].Controls)
					{
						if (control is CheckBox)
						{
							CheckBox checkBox = (CheckBox)control;
							if (checkBox.Checked)
							{
								lst.Add(dgi.Cells[0].Text);
							}
						}
					}
				}
			}
			return lst.ToArray();
		}
		#endregion

		#region SetCheckedItems
		private string[] SetCheckedItems(string[] value)
		{
			List<string> lstNew = new List<string>();	// return value

			List<string> lst = new List<string>(value);
			if (lst.Count == 0)
			{
				AllItemsCheckBox.Checked = false;
			}
			else if (lst.Count == 1 && lst[0] == "0")	// 0 indicates "All checked"
			{
				lstNew.Add("0");

				AllItemsCheckBox.Checked = true;
				foreach (DataGridItem dgi in MainGrid.Items)
				{
					foreach (Control control in dgi.Cells[1].Controls)
					{
						CheckBox checkBox = control as CheckBox;
						if (checkBox != null)
							checkBox.Checked = true;
					}
				}
			}
			else
			{
				AllItemsCheckBox.Checked = false;
				foreach (DataGridItem dgi in MainGrid.Items)
				{
					foreach (Control control in dgi.Cells[1].Controls)
					{
						CheckBox checkBox = control as CheckBox;
						if (checkBox != null)
						{
							string itemId = dgi.Cells[0].Text;

							if (lst.Contains(itemId))
							{
								checkBox.Checked = true;
								lstNew.Add(itemId);
							}
						}
					}
				}
			}
			return lstNew.ToArray();
		}
		#endregion

		#region GetAllCheckedItems
		public string[] GetAllCheckedItems()
		{
			List<string> lst = new List<string>();
			foreach (DataGridItem dgi in MainGrid.Items)
			{
				foreach (Control control in dgi.Cells[1].Controls)
				{
					if (control is CheckBox)
					{
						CheckBox checkBox = (CheckBox)control;
						if (checkBox.Checked)
						{
							lst.Add(dgi.Cells[0].Text);
						}
					}
				}
			}
			return lst.ToArray();
		}
		#endregion

		#region SetText
		private void SetText()
		{
			if (AllItemsCheckBox.Checked)
			{
				SelectedLabel.Text = AnyText;
			}
			else
			{
				string text = String.Empty;
				foreach (DataGridItem dgi in MainGrid.Items)
				{
					foreach (Control control in dgi.Cells[1].Controls)
					{
						CheckBox checkBox = control as CheckBox;
						if (checkBox != null && checkBox.Checked)
						{
							if (text != String.Empty)
								text += "; ";
							text += checkBox.Text;
						}
					}
				}

				SelectedLabel.Text = (text != String.Empty) ? text : NotSetText;
			}
		}
		#endregion

		#region DataBind
		public override void DataBind()
		{
			((BoundColumn)MainGrid.Columns[0]).DataField = DataValueField;

			string[] savedValue = Value;
			MainGrid.DataBind();
			Value = savedValue;
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			// .css
			if (!Page.ClientScript.IsClientScriptBlockRegistered("objectDD_css"))
			{
				Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "objectDD_css",
					String.Format("<link type='text/css' rel='stylesheet' href='{0}' />", ResolveClientUrl("~/styles/IbnFramework/objectDD.css")), false);
			}

			// .js
			ClientScript.RegisterClientScriptInclude(this.Page, this.Page.GetType(),
				"MultiSelectControl_js", ResolveClientUrl("~/Apps/ClioSoft/scripts/MultiSelectControl.js"));

			// javascript object
			string registerControl = String.Format("msc_Collection['{0}'] = new msc_Object(\"{0}\", \"{1}\", \"{2}\", \"{3}\");",
				this.ClientID,
				MainTable.ClientID,
				DropDownDiv.ClientID,
				AllItemsCheckBox.ClientID);
			ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString(),
				registerControl, true);

			// onclick
			MainTable.Attributes.Add("onclick", String.Format("msc_Collection['{0}'].msc_ShowHideDropDown(event);", this.ClientID));
			AllItemsCheckBox.Attributes.Add("onclick", String.Format("msc_Collection['{0}'].msc_CheckAll(event);", this.ClientID));

			foreach (DataGridItem dgi in MainGrid.Items)
			{
				CheckBox cb = (CheckBox)dgi.FindControl("chkItem");
				if (cb != null)
					cb.Attributes.Add("onclick", String.Format("msc_Collection['{0}'].msc_UncheckAllIfNeed(this, event);", this.ClientID));
			}

			// Selected Label text
			if (String.IsNullOrEmpty(SelectedLabel.Text))
				SelectedLabel.Text = NotSetText;
		}
		#endregion

		#region SaveButton_Click
		protected void SaveButton_Click(object sender, EventArgs e)
		{
			SetText();
			this.OnValueChange(sender, e);
		}
		#endregion
	}
}