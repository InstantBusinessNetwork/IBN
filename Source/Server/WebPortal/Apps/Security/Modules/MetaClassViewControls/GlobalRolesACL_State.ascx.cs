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
	public partial class GlobalRolesACL_State : MCDataBoundControl
	{
		protected readonly string className = "ClassName";
		protected readonly string principalColumn = "Principal";
		protected readonly string editColumn = "EditColumn";
		protected readonly string resetColumn = "ResetColumn";
		protected readonly string idColumn = "IdColumn";
		protected readonly string isInheritedColumn = "IsInherited";
		protected readonly string dialogWidth = "500";
		protected readonly string dialogHeight = "300";
		protected readonly double percentsForRights = 75.0;
		protected readonly int isInheritedColumnNumber = 2;

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
				if (!CheckVisibility(mc))
				{
					this.Visible = false;
					return;
				}

				BindStateMachine();
				GenerateStructure();
				BindState();
			}
		}
		#endregion

		#region CheckVisibility
		public override bool CheckVisibility(object dataItem)
		{
			if (dataItem is MetaClass)
			{
				MetaClass cls = (MetaClass)dataItem;
				return Mediachase.Ibn.Data.Services.Security.IsInstalled(cls) 
					&& BusinessObjectServiceManager.IsServiceInstalled(cls, StateMachineService.ServiceName) 
					&& (StateMachineManager.GetAvailableStateMachines(cls).Length > 0)
					&& Mediachase.Ibn.Data.Services.Security.GetGlobalAcl(cls.Name).Length > 0;
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

			CustomTableRow[] classRights = CustomTableRow.List(SqlContext.Current.Database.Tables[Mediachase.Ibn.Data.Services.Security.BaseRightsTableName],
				FilterElement.EqualElement("ClassOnly", 1));

			foreach (SecurityRight right in Mediachase.Ibn.Data.Services.Security.GetMetaClassRights(mc.Name))
			{
				// Don't show class rights (ex. Create)
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

			// Reset
			ButtonField btn = new ButtonField();
			btn.ButtonType = ButtonType.Link;
			btn.Text = String.Format(CultureInfo.InvariantCulture,
				"<img src='{0}' width='16' height='16' border='0' title='{1}' />",
				ResolveUrl("~/Images/IbnFramework/Undo.png"),
				GetGlobalResourceObject("IbnFramework.Security", "ResetToBase").ToString());
			btn.HeaderStyle.Width = Unit.Pixel(25);
			btn.ItemStyle.Width = Unit.Pixel(25);
			btn.CommandName = "Reset";
			MainGrid.Columns.Add(btn);

			string[] dataKeyNames = { idColumn };
			MainGrid.DataKeyNames = dataKeyNames;
		}
		#endregion

		#region BindStateMachine
		private void BindStateMachine()
		{
			string savedValue = StateMachineList.SelectedValue;

			StateMachineList.Items.Clear();

			Mediachase.Ibn.Data.Services.StateMachine[] smList = StateMachineManager.GetAvailableStateMachines(mc);
			foreach (Mediachase.Ibn.Data.Services.StateMachine sm in smList)
			{
				ListItem li = new ListItem(CHelper.GetResFileString(sm.Name), sm.PrimaryKeyId.ToString());
				StateMachineList.Items.Add(li);
			}

			if (savedValue != null)
				CHelper.SafeSelect(StateMachineList, savedValue);
		}
		#endregion

		#region BindState
		private void BindState()
		{
			string savedValue = StateList.SelectedValue;

			StateList.Items.Clear();

			int stateMachineId = int.Parse(StateMachineList.SelectedValue);
			Mediachase.Ibn.Data.Services.StateMachine sm = new Mediachase.Ibn.Data.Services.StateMachine(mc, stateMachineId);
			foreach (State state in sm.States)
			{
				MetaObject mo = StateMachineManager.GetState(mc, state.Name);

				ListItem li = new ListItem(CHelper.GetResFileString(mo.Properties["FriendlyName"].Value.ToString()), mo.PrimaryKeyId.ToString());
				StateList.Items.Add(li);
			}

			if (savedValue != null)
				CHelper.SafeSelect(StateList, savedValue);

			BindGrid();
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
			dt.Columns.Add(isInheritedColumn, typeof(bool));
			foreach (SecurityRight right in Mediachase.Ibn.Data.Services.Security.GetMetaClassRights(mc.Name))
			{
				dt.Columns.Add(right.RightName, typeof(string));
			}
			dt.Columns.Add(editColumn, typeof(string));
			dt.Columns.Add(resetColumn, typeof(string));

			// Fill data
			DataRow dr;

			foreach (MetaObject mo in Mediachase.Ibn.Data.Services.Security.GetGlobalAcl(mc.Name))
			{
				int principalId = (PrimaryKeyId)mo.Properties["PrincipalId"].Value;

				dr = dt.NewRow();
				dr[idColumn] = mo.PrimaryKeyId.Value;
				dr[principalColumn] = CHelper.GetUserName(principalId);

				bool isInhereted = false;
				MetaObject obj = StateMachineUtil.GetGlobalAclStateItem(mc.Name, mo.PrimaryKeyId.Value, int.Parse(StateMachineList.SelectedValue), int.Parse(StateList.SelectedValue));
				if (obj == null)
				{
					obj = mo;
					isInhereted = true;
				}

				dr[isInheritedColumn] = isInhereted;

				for (int i = 1; i < MainGrid.Columns.Count - 2; i++)
				{
					BoundField rightsField = MainGrid.Columns[i] as BoundField;
					if (rightsField != null)
					{
						string fieldName = rightsField.DataField;
						dr[fieldName] = CHelper.GetPermissionImage((int)obj.Properties[fieldName].Value, isInhereted);
					}
				}

				string url = String.Format(CultureInfo.InvariantCulture, 
						"javascript:ShowWizard(&quot;{7}?ClassName={0}&btn={1}&PrincipalId={2}&SmId={3}&StateId={4}&quot;, {5}, {6});", 
						mc.Name, Page.ClientScript.GetPostBackEventReference(RefreshButton, ""), 
						principalId, StateMachineList.SelectedValue, StateList.SelectedValue, 
						dialogWidth, dialogHeight,
						ResolveClientUrl("~/Apps/Security/Pages/Admin/GlobalRoleAclStateEdit.aspx"));
				dr[editColumn] = String.Format(CultureInfo.InvariantCulture,
					"<a href=\"{0}\"><img src=\"{1}\" title=\"{2}\" width=\"16\" height=\"16\" border=\"0\" /></a>", url, ResolveUrl("~/Images/IbnFramework/edit.gif"), GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Edit").ToString());

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

		#region StateMachineList_SelectedIndexChanged
		protected void StateMachineList_SelectedIndexChanged(object sender, EventArgs e)
		{
			GenerateStructure();
			BindState();
		}
		#endregion

		#region StateList_SelectedIndexChanged
		protected void StateList_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindGrid();
		}
		#endregion

		#region MainGrid_RowCommand
		protected void MainGrid_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e == null)
				throw new ArgumentNullException("e");

			if (e.CommandName == "Reset")
			{
				int globalAclId = int.Parse(MainGrid.DataKeys[int.Parse(e.CommandArgument.ToString(), CultureInfo.InvariantCulture)].Value.ToString());
				StateMachineUtil.UnregisterGlobalRole(mc.Name, globalAclId, int.Parse(StateMachineList.SelectedValue, CultureInfo.InvariantCulture), int.Parse(StateList.SelectedValue, CultureInfo.InvariantCulture));
			}
			CHelper.AddToContext("RebindPage", "true");
		}
		#endregion

		#region MainGrid_RowDataBound
		protected void MainGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e == null)
				throw new ArgumentNullException("e");

			// if isInhereted=true then we hide the reset button
			object item = e.Row.DataItem;
			if (item != null && ((System.Data.DataRowView)item).Row.ItemArray[isInheritedColumnNumber] != DBNull.Value
				&& (bool)((System.Data.DataRowView)item).Row.ItemArray[isInheritedColumnNumber])
			{
				WebControl ctrl = (WebControl)e.Row.Cells[e.Row.Cells.Count - 1].Controls[0];
				if (ctrl != null)
				{
					ctrl.Visible = false;
				}
			}
		}
		#endregion
	}
}