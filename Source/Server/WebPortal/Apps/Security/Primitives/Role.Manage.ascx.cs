using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;

using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.Security.Primitives
{
	public partial class Role_Manage : System.Web.UI.UserControl, IManageControl
	{
		string principalType = "PrincipalType";		// Enum

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IManageControl Members
		public string GetDefaultValue(bool AllowNulls)
		{
			return null;
		}

		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get 
			{
				List<int> lst = new List<int>();

				foreach (DataGridItem dgi in MainGrid.Items)
				{
					foreach (Control control in dgi.Cells[1].Controls)
					{
						if (control is CheckBox)
						{
							CheckBox checkBox = (CheckBox)control;
							if (checkBox.Checked)
							{
								lst.Add(int.Parse(dgi.Cells[0].Text, CultureInfo.InvariantCulture));
							}
						}
					}
				}

				Mediachase.Ibn.Data.Meta.Management.AttributeCollection attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				attr.Add(RoleTypeAttribute.PermittedPrincipalTypes, lst.ToArray());
				return attr;
			}
		}

		public void BindData(MetaClass mc, string fieldType)
		{
			FillItems(true);
		}

		public void BindData(MetaField mf)
		{
			FillItems(false);

			object value = mf.Attributes[RoleTypeAttribute.PermittedPrincipalTypes];
			if (value != null)
			{
				List<int> lst = new List<int>((int[])value);

				foreach (DataGridItem dgi in MainGrid.Items)
				{
					foreach (Control control in dgi.Cells[1].Controls)
					{
						if (control is CheckBox)
						{
							CheckBox checkBox = (CheckBox)control;
							int itemId = int.Parse(dgi.Cells[0].Text);

							if (lst.Contains(itemId))
								checkBox.Checked = true;
						}
					}
				}
			}
		}
		#endregion

		#region FillItems
		private void FillItems(bool checkUser)
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Id", typeof(int));
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("Checked", typeof(bool));

			foreach (MetaEnumItem item in MetaEnum.GetItems(DataContext.Current.MetaModel.RegisteredTypes[principalType]))
			{
				// Ibn 4.7 fix: use only Users and Groups
				if (item.Handle != (int)PrincipalTypes.User && item.Handle != (int)PrincipalTypes.Department)
					continue;

				DataRow row = dt.NewRow();
				row["Id"] = item.Handle;

				// Ibn 4.7 fix
				if (item.Handle == (int)PrincipalTypes.Department)
					row["Name"] = GetGlobalResourceObject("IbnFramework.Global", "Group").ToString();
				else
					row["Name"] = CHelper.GetResFileString(item.Name);

				if (checkUser && item.Handle == (int)PrincipalTypes.User)
					row["Checked"] = true;
				else
					row["Checked"] = false;
				dt.Rows.Add(row);
			}

			MainGrid.DataSource = dt.DefaultView;
			MainGrid.DataBind();
		}
		#endregion
	}
}