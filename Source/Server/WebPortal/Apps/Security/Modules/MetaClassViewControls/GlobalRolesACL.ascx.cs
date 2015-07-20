using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta;

using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Security.Modules.MetaClassViewControls
{
	public partial class GlobalRolesACL : MCDataBoundControl
	{
		protected readonly string className = "ClassName";
		protected readonly string principalColumn = "Principal";
		protected readonly string editColumn = "EditColumn";
		protected readonly string deleteColumn = "DeleteColumn";
		protected readonly string idColumn = "IdColumn";
		protected readonly string dialogWidth = "500";
		protected readonly string dialogHeight = "300";
		protected readonly double percentsForRights = 75.0;

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
				this.Visible = true;
				if (!Mediachase.Ibn.Data.Services.Security.IsInstalled(mc))
				{
					this.Visible = false;
					return;
				}

				NewLink.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewItem").ToString();
				NewLink.NavigateUrl = String.Format(CultureInfo.InvariantCulture,
					"javascript:ShowWizard(\"{4}?ClassName={0}&btn={1}\", {2}, {3});", 
					mc.Name, Page.ClientScript.GetPostBackEventReference(RefreshButton, ""), 
					dialogWidth, dialogHeight,
					ResolveUrl("~/Apps/Security/Pages/Admin/GlobalRoleAclEdit.aspx"));

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
			field.DataField = principalColumn;
			field.HeaderText = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Principal").ToString();
			field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
			field.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
			field.HeaderStyle.CssClass = "ibn-vh";
			field.HtmlEncode = false;
			MainGrid.Columns.Add(field);

			int rightsCounter = 0;

			foreach (SecurityRight right in Mediachase.Ibn.Data.Services.Security.GetMetaClassRights(mc.Name))
			{
				rightsCounter++;
				field = new BoundField();
				field.DataField = right.RightName;
				field.HeaderText = CHelper.GetResFileString(right.FriendlyName);
				field.HeaderStyle.CssClass = "thCenter";
				field.HtmlEncode = false;
				MainGrid.Columns.Add(field);
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

			// Delete
			CommandField del = new CommandField();
			del.ButtonType = ButtonType.Link;
			del.DeleteText = String.Format(CultureInfo.InvariantCulture,
				"<img src='{0}' width='16' height='16' border='0' title='{1}' />", ResolveUrl("~/Images/IbnFramework/delete.gif"), GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Delete").ToString());
			del.HeaderStyle.Width = Unit.Pixel(25);
			del.ItemStyle.Width = Unit.Pixel(25);
			del.ShowDeleteButton = true;
			MainGrid.Columns.Add(del);

			string[] dataKeyNames = {idColumn};
			MainGrid.DataKeyNames = dataKeyNames;
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			// DataTable structure
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add(idColumn, typeof(string));
			dt.Columns.Add(principalColumn, typeof(string));
			foreach (SecurityRight right in Mediachase.Ibn.Data.Services.Security.GetMetaClassRights(mc.Name))
			{
				dt.Columns.Add(right.RightName, typeof(string));
			}
			dt.Columns.Add(editColumn, typeof(string));
			dt.Columns.Add(deleteColumn, typeof(string));

			// Fill data
			DataRow dr;

			foreach (MetaObject mo in Mediachase.Ibn.Data.Services.Security.GetGlobalAcl(mc.Name))
			{
				int principalId = (PrimaryKeyId)mo.Properties["PrincipalId"].Value;

				dr = dt.NewRow();
				dr[idColumn] = mo.PrimaryKeyId;
				dr[principalColumn] = CHelper.GetUserName(principalId);

				for (int i = 1; i < MainGrid.Columns.Count - 2; i++)
				{
					BoundField rightsField = MainGrid.Columns[i] as BoundField;
					if (rightsField != null)
					{
						string fieldName = rightsField.DataField;
						dr[fieldName] = CHelper.GetPermissionImage((int)mo.Properties[fieldName].Value);
					}
				}

				string url = String.Format(CultureInfo.InvariantCulture,
					"javascript:ShowWizard(&quot;{5}?ClassName={0}&btn={1}&PrincipalId={2}&quot;, {3}, {4});", 
					mc.Name, Page.ClientScript.GetPostBackEventReference(RefreshButton, ""), 
					principalId, dialogWidth, dialogHeight,
					ResolveUrl("~/Apps/Security/Pages/Admin/GlobalRoleAclEdit.aspx"));
				dr[editColumn] = String.Format(CultureInfo.InvariantCulture,
					"<a href=\"{0}\"><img src=\"{1}\" title=\"{2}\" width=\"16\" height=\"16\" border=\"0\" /></a>", url, ResolveUrl("~/Images/IbnFramework/edit.gif"), GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Edit").ToString());

				dt.Rows.Add(dr);
			}

			MainGrid.DataSource = dt;
			MainGrid.DataBind();

			foreach (GridViewRow row in MainGrid.Rows)
			{
				if (row.Cells[row.Cells.Count - 1].Controls.Count > 0)
				{
					WebControl ctrl = (WebControl)row.Cells[row.Cells.Count - 1].Controls[0];
					if (ctrl != null)
						ctrl.Attributes.Add("onclick", "if (!confirm('" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Delete").ToString() + "?')) return false;");
				}
			}
		}
		#endregion

		#region MainGrid_RowCommand
		protected void MainGrid_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e == null)
				throw new ArgumentException("e");

			if (e.CommandName == "Delete")
			{
				int objectId = int.Parse(MainGrid.DataKeys[int.Parse(e.CommandArgument.ToString())].Value.ToString());
				MetaObject mo = Mediachase.Ibn.Data.Services.Security.GetGlobalAce(mc.Name, objectId);
				mo.Delete();
			}

			CHelper.AddToContext("RebindPage", "true");
		}
		#endregion

		#region RefreshButton_Click
		protected void RefreshButton_Click(object sender, EventArgs e)
		{
			CHelper.AddToContext("RebindPage", "true");
		}
		#endregion

		#region MainGrid_RowDeleting
		protected void MainGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{

		}
		#endregion
	}
}