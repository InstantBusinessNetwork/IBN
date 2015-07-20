namespace Mediachase.Ibn.Web.UI.StateMachine.Modules.MetaClassViewControls
{
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
	using Mediachase.Ibn.Data.Meta;
	using Mediachase.Ibn.Data.Meta.Management;

	using Mediachase.Ibn.Data.Services;
	using Mediachase.Ibn.Web.UI.Controls.Util;
	using Mediachase.Ibn.Web.UI.WebControls;

	public partial class Transitions : MCDataBoundControl
	{
		protected readonly string className = "ClassName";
		protected readonly string fromStateColumn = "FromState";
		protected readonly string dialogWidth = "500";
		protected readonly string dialogHeight = "300";
		protected readonly string editColumn = "EditColumn";
		protected readonly string deleteColumn = "DeleteColumn";
		protected readonly string idColumn = "IdColumn";
		protected readonly double percentsForStates = 75.0;
		protected StateMachine sm = null;

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
				if (!BusinessObjectServiceManager.IsServiceInstalled(mc, StateMachineService.ServiceName) || StateMachineManager.GetAvailableStateMachines(mc).Length == 0)
				{
					this.Visible = false;
					return;
				}

				BindStateMachine();
			}
		}
		#endregion

		#region CheckVisibility
		public override bool CheckVisibility(object dataItem)
		{
			if (dataItem is MetaClass)
			{
				MetaClass cls = (MetaClass)dataItem;
				return BusinessObjectServiceManager.IsServiceInstalled(cls, StateMachineService.ServiceName) && (StateMachineManager.GetAvailableStateMachines(cls).Length > 0);
			}
			else
			{
				return base.CheckVisibility(dataItem);
			}
		}
		#endregion

		#region BindStateMachine
		private void BindStateMachine()
		{
			string savedValue = ddlStateMachine.SelectedValue;

			ddlStateMachine.Items.Clear();

			StateMachine[] smList = StateMachineManager.GetAvailableStateMachines(mc);

			foreach (StateMachine sm in smList)
			{
				ListItem li = new ListItem(CHelper.GetResFileString(sm.Name), sm.PrimaryKeyId.ToString());
				ddlStateMachine.Items.Add(li);
			}

			if (savedValue != null)
				CHelper.SafeSelect(ddlStateMachine, savedValue);

			GenerateStructure();
		}
		#endregion

		#region GenerateStructure
		private void GenerateStructure()
		{
			if (grdMain.Columns.Count > 0)
				grdMain.Columns.Clear();

			BoundField field = new BoundField();
			field.DataField = fromStateColumn;
			//			field.ItemStyle.CssClass = "ibn-vh";
			field.HtmlEncode = false;
			grdMain.Columns.Add(field);

			sm = StateMachineManager.GetStateMachine(mc, int.Parse(ddlStateMachine.SelectedValue));
			foreach (State state in sm.States)
			{
				MetaObject mo = StateMachineManager.GetState(mc, state.Name);

				field = new BoundField();
				field.DataField = state.Name;
				field.HeaderText = CHelper.GetResFileString(mo.Properties["FriendlyName"].Value.ToString());
				field.HeaderStyle.CssClass = "ibn-vh";
				field.HtmlEncode = false;
				field.HeaderStyle.Width = Unit.Percentage(percentsForStates / sm.States.Count);
				grdMain.Columns.Add(field);
			}

			// Edit
			field = new BoundField();
			field.DataField = editColumn;
			field.HeaderText = String.Empty;
			field.HtmlEncode = false;
			field.HeaderStyle.Width = Unit.Pixel(25);
			field.ItemStyle.Width = Unit.Pixel(25);
			grdMain.Columns.Add(field);

			string[] dataKeyNames = { idColumn };
			grdMain.DataKeyNames = dataKeyNames;

			BindGrid();
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			if (sm == null)
				sm = StateMachineManager.GetStateMachine(mc, int.Parse(ddlStateMachine.SelectedValue));

			// DataTable definition
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add(idColumn, typeof(string));
			dt.Columns.Add(fromStateColumn, typeof(string));

			foreach (State state in sm.States)
			{
				dt.Columns.Add(state.Name, typeof(string));
			}
			dt.Columns.Add(editColumn, typeof(string));
			dt.Columns.Add(deleteColumn, typeof(string));

			// Fill data
			DataRow dr;

			foreach (State fromState in sm.States)
			{
				MetaObject mo = StateMachineManager.GetState(mc, fromState.Name);

				dr = dt.NewRow();

				dr[idColumn] = fromState.Name;
				dr[fromStateColumn] = CHelper.GetResFileString(mo.Properties["FriendlyName"].Value.ToString());

				foreach (State toState in sm.States)
				{
					if (fromState.Name == toState.Name)
					{
						continue;
					}

					StateTransition st = sm.FindTransition(fromState, toState);
					if (st != null)
					{
						dr[toState.Name] = String.Format(CultureInfo.InvariantCulture,
							"{0} {1}",
							CHelper.GetPermissionImage((int)Security.Rights.Allow),
							CHelper.GetResFileString(st.Name));
					}
					else
					{
						dr[toState.Name] = CHelper.GetPermissionImage((int)Security.Rights.Forbid);
					}
				}

				string url = String.Format("javascript:ShowWizard(&quot;{6}?ClassName={0}&btn={1}&SMId={2}&FromState={3}&quot;, {4}, {5});",
					mc.Name, Page.ClientScript.GetPostBackEventReference(btnRefresh, ""),
					ddlStateMachine.SelectedValue, fromState.Name, dialogWidth, dialogHeight,
					ResolveClientUrl("~/Apps/StateMachine/Pages/Admin/TransitionEdit.aspx"));
				dr[editColumn] = String.Format("<a href=\"{0}\"><img src=\"{1}\" title=\"{2}\" width=\"16\" height=\"16\" border=\"0\" /></a>", url, CHelper.GetAbsolutePath("/Images/IbnFramework/edit.gif"), GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Edit").ToString());

				dt.Rows.Add(dr);
			}

			grdMain.DataSource = dt;
			grdMain.DataBind();

			for (int i = 0; i < grdMain.Rows.Count; i++)
			{
				grdMain.Rows[i].Cells[i + 1].BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
			}
		}
		#endregion

		#region btnRefresh_Click
		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			BindGrid();
		}
		#endregion

		#region ddlStateMachine_SelectedIndexChanged
		protected void ddlStateMachine_SelectedIndexChanged(object sender, EventArgs e)
		{
			GenerateStructure();
		}
		#endregion

	}
}