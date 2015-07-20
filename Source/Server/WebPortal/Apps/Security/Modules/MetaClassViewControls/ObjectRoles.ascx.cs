using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Sql;
using Mediachase.Ibn.Core;

using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Security.Modules.MetaClassViewControls
{
	public partial class ObjectRoles : MCDataBoundControl
	{
		protected readonly string className = "ClassName";
		protected readonly string roleColumn = "RoleName";
		protected readonly string editColumn = "EditColumn";
		protected readonly string idColumn = "IdColumn";
		protected readonly string classColumn = "ClassName";
		protected readonly string dialogWidth = "500";
		protected readonly string dialogHeight = "205";
		protected readonly double percentsForRights = 75.0;
		protected readonly int classColumnNumber = 2;

		#region DataItem
		public override object DataItem
		{
			get
			{
				return base.DataItem;
			}
			set
			{
				if (value is MetaClass)
					mc = (MetaClass)value;

				base.DataItem = value;
			}
		}
		#endregion

		#region MetaClass mc
		private MetaClass _mc = null;
		private MetaClass mc
		{
			get
			{
				if (_mc == null)
				{
					if (ViewState[className] != null)
						_mc = MetaDataWrapper.GetMetaClassByName(ViewState[className].ToString());
				}
				return _mc;
			}
			set
			{
				ViewState[className] = value.Name;
				_mc = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region DataBind
		public override void DataBind()
		{
			if (mc != null)
			{
				GenerateStructure();
				BindGrid();
			}
		}
		#endregion

		#region CheckVisibility
		public override bool CheckVisibility(object dataItem)
		{
			if (dataItem is MetaClass)
			{
				return Mediachase.Ibn.Data.Services.Security.IsInstalled((MetaClass)dataItem);
			}
			else
			{
				return base.CheckVisibility(dataItem);
			}
		}
		#endregion

		#region GenerateStructure
		private void GenerateStructure()
		{
			if (MainGrid.Columns.Count > 0)
				MainGrid.Columns.Clear();

			BoundField field = new BoundField();
			field.DataField = roleColumn;
			field.HeaderText = GetGlobalResourceObject("IbnFramework.Security", "Role").ToString();
			field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
			field.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
			field.HeaderStyle.CssClass = "ibn-vh";
			MainGrid.Columns.Add(field);

			int rightsCounter = 0;

			this.Visible = true;

			CustomTableRow[] classRights = CustomTableRow.List(SqlContext.Current.Database.Tables[Mediachase.Ibn.Data.Services.Security.BaseRightsTableName],
				FilterElement.EqualElement("ClassOnly", 1));

			try
			{
				foreach (SecurityRight right in Mediachase.Ibn.Data.Services.Security.GetMetaClassRights(mc.Name))
				{
					// Check for Class Right (ex. Create)
					bool isClassRight = false;
					string rightUid = right.BaseRightUid.ToString();
					foreach (CustomTableRow r in classRights)
					{
						if (r["BaseRightUid"].ToString() == rightUid)
						{
							isClassRight = true;
							break;
						}
					}
					if (isClassRight)
						continue;

					rightsCounter++;
					field = new BoundField();
					field.DataField = right.RightName;
					field.HeaderText = CHelper.GetResFileString(right.FriendlyName);
					field.HeaderStyle.CssClass = "thCenter";
					field.HtmlEncode = false;
					MainGrid.Columns.Add(field);
				}
			}
			catch
			{
				this.Visible = false;
				return;
			}

			for (int i = 1; i <= rightsCounter; i++)
			{
				MainGrid.Columns[i].HeaderStyle.Width = Unit.Percentage(percentsForRights / rightsCounter);
			}

			// Edit
			field = new BoundField();
			field.DataField = editColumn;
			field.HeaderText = String.Empty;
			field.HtmlEncode = false;
			field.HeaderStyle.Width = Unit.Pixel(25);
			field.ItemStyle.Width = Unit.Pixel(25);
			MainGrid.Columns.Add(field);

			string[] dataKeyNames = { idColumn };
			MainGrid.DataKeyNames = dataKeyNames;
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			if (!this.Visible)
				return;

			// DataTable structure
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add(idColumn, typeof(string));
			dt.Columns.Add(roleColumn, typeof(string));
			dt.Columns.Add(classColumn, typeof(string));

			CustomTableRow[] classRights = CustomTableRow.List(SqlContext.Current.Database.Tables[Mediachase.Ibn.Data.Services.Security.BaseRightsTableName],
				FilterElement.EqualElement("ClassOnly", 1));

			foreach (SecurityRight right in Mediachase.Ibn.Data.Services.Security.GetMetaClassRights(mc.Name))
			{
				// Check for Class Right (ex. Create)
				bool isClassRight = false;
				string rightUid = right.BaseRightUid.ToString();
				foreach (CustomTableRow r in classRights)
				{
					if (r["BaseRightUid"].ToString() == rightUid)
					{
						isClassRight = true;
						break;
					}
				}
				if (isClassRight)
					continue;

				dt.Columns.Add(right.RightName, typeof(string));
			}
			dt.Columns.Add(editColumn, typeof(string));

			// Fill data
			DataRow dr;
			foreach (MetaObject mo in Mediachase.Ibn.Data.Services.RoleManager.GetObjectRoleItems(mc.Name))
			{
				string roleName = mo.Properties["RoleName"].Value.ToString();

				dr = dt.NewRow();
				dr[idColumn] = mo.PrimaryKeyId;
				dr[roleColumn] = CHelper.GetResFileString(mo.Properties["FriendlyName"].Value.ToString());
				if (mo.Properties["ClassName"].Value != null)
				{
					string className = (string)mo.Properties["ClassName"].Value;
					if (className.Contains("::"))
					{
						string[] s = new string[] { "::" };
						className = className.Split(s, StringSplitOptions.None)[0];
					}
					dr[classColumn] = className;
					dr[roleColumn] += String.Format(CultureInfo.InvariantCulture,
						" ({0})", 
						CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(className).PluralName));
				}

				for (int i = 1; i < MainGrid.Columns.Count - 1; i++)
				{
					BoundField rightsField = MainGrid.Columns[i] as BoundField;
					if (rightsField != null)
					{
						string fieldName = rightsField.DataField;
						dr[fieldName] = CHelper.GetPermissionImage((int)mo.Properties[fieldName].Value);
					}
				}

				string url = String.Format(CultureInfo.InvariantCulture,
					"javascript:ShowWizard(&quot;{5}?ClassName={0}&btn={1}&Role={2}&quot;, {3}, {4});", 
					mc.Name, 
					Page.ClientScript.GetPostBackEventReference(RefreshButton, ""), 
					roleName, 
					dialogWidth, dialogHeight,
					ResolveUrl("~/Apps/Security/Pages/Admin/ObjectRoleEdit.aspx"));
				dr[editColumn] = String.Format(CultureInfo.InvariantCulture, 
					"<a href=\"{0}\"><img src=\"{1}\" title=\"{2}\" width=\"16\" height=\"16\" border=\"0\" /></a>", 
					url, 
					ResolveUrl("~/Images/IbnFramework/edit.gif"), 
					GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Edit").ToString());

				dt.Rows.Add(dr);
			}

			MainGrid.DataSource = dt;
			MainGrid.DataBind();
		}
		#endregion

		#region RefreshButton_Click
		protected void RefreshButton_Click(object sender, EventArgs e)
		{
			CHelper.AddToContext("RebindPage", "true");
		}
		#endregion
	}
}