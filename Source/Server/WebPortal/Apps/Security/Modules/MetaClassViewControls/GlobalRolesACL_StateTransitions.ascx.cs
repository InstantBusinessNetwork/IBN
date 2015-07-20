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
using Mediachase.Ibn.Core;

using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Security.Modules.MetaClassViewControls
{
	public partial class GlobalRolesACL_StateTransitions : MCDataBoundControl
	{
		protected readonly string className = "ClassName";
		protected readonly string principalColumn = "Principal";
		protected readonly string editColumn = "EditColumn";
		protected readonly string dialogWidth = "500";
		protected readonly string dialogHeight = "300";
		protected readonly double percentsForTransition = 75.0;
		protected Mediachase.Ibn.Data.Services.StateMachine sm = null;

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

		#region BindStateMachine
		private void BindStateMachine()
		{
			string savedValue = ddlStateMachine.SelectedValue;

			ddlStateMachine.Items.Clear();

			Mediachase.Ibn.Data.Services.StateMachine[] smList = StateMachineManager.GetAvailableStateMachines(mc);

			foreach (Mediachase.Ibn.Data.Services.StateMachine sm in smList)
			{
				ListItem li = new ListItem(CHelper.GetResFileString(sm.Name), sm.PrimaryKeyId.ToString());
				ddlStateMachine.Items.Add(li);
			}

			if (savedValue != null)
				CHelper.SafeSelect(ddlStateMachine, savedValue);

			BindState();
		}
		#endregion

		#region BindState
		private void BindState()
		{
			string savedValue = ddlState.SelectedValue;

			ddlState.Items.Clear();

			int stateMachineId = int.Parse(ddlStateMachine.SelectedValue);

			sm = new Mediachase.Ibn.Data.Services.StateMachine(mc, stateMachineId);

			grdMain.Visible = true;
			if (sm.States.Count == 0)
			{
				grdMain.Visible = false;
				return;
			}

			foreach (State state in sm.States)
			{
				MetaObject mo = StateMachineManager.GetState(mc, state.Name);

				ListItem li = new ListItem(CHelper.GetResFileString(mo.Properties["FriendlyName"].Value.ToString()), mo.PrimaryKeyId.Value.ToString());
				ddlState.Items.Add(li);
			}

			if (savedValue != null)
				CHelper.SafeSelect(ddlState, savedValue);

			GenerateStructure();
		}
		#endregion

		#region GenerateStructure
		private void GenerateStructure()
		{
			if (sm == null)
				sm = new Mediachase.Ibn.Data.Services.StateMachine(mc, int.Parse(ddlStateMachine.SelectedValue));

			if (grdMain.Columns.Count > 0)
				grdMain.Columns.Clear();

			BoundField field = new BoundField();
			field.DataField = principalColumn;
			field.HeaderText = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Principal").ToString();
			field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
			field.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
			field.HeaderStyle.CssClass = "ibn-vh";
			field.HtmlEncode = false;
			grdMain.Columns.Add(field);

			int transitionCounter = 0;
			MetaObject mo = StateMachineManager.GetState(mc, int.Parse(ddlState.SelectedValue));
			string fromState = mo.Properties["Name"].Value.ToString();
			foreach (State state in sm.States)
			{
				StateTransition st = sm.FindTransition(fromState, state.Name);
				if (st != null)
				{
					transitionCounter++;
					field = new BoundField();
					field.DataField = state.Name;
					field.HeaderText = CHelper.GetResFileString(st.Name);
					field.HeaderStyle.CssClass = "thCenter";
					field.HtmlEncode = false;
					grdMain.Columns.Add(field);
				}
			}

			grdMain.Visible = true;
			if (transitionCounter == 0)
			{
				grdMain.Visible = false;
				return;
			}

			for (int i = 1; i <= transitionCounter; i++)
			{
				grdMain.Columns[i].HeaderStyle.Width = Unit.Percentage(percentsForTransition / transitionCounter);
			}

			// Edit
			field = new BoundField();
			field.DataField = editColumn;
			field.HeaderText = String.Empty;
			field.HtmlEncode = false;
			field.HeaderStyle.Width = Unit.Pixel(25);
			field.ItemStyle.Width = Unit.Pixel(25);
			grdMain.Columns.Add(field);

			BindGrid();
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			if (sm == null)
				sm = new Mediachase.Ibn.Data.Services.StateMachine(mc, int.Parse(ddlStateMachine.SelectedValue));

			// DataTable structure
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add(principalColumn, typeof(string));

			MetaObject moState = StateMachineManager.GetState(mc, int.Parse(ddlState.SelectedValue));
			string fromState = moState.Properties["Name"].Value.ToString();

			foreach (State state in sm.States)
			{
				StateTransition st = sm.FindTransition(fromState, state.Name);
				if (st != null)
					dt.Columns.Add(state.Name, typeof(string));
			}
			dt.Columns.Add(editColumn, typeof(string));

			// Fill data
			DataRow dr;

			foreach (MetaObject mo in Mediachase.Ibn.Data.Services.Security.GetGlobalAcl(mc.Name))
			{
				int principalId = (PrimaryKeyId)mo.Properties["PrincipalId"].Value;

				dr = dt.NewRow();
				dr[principalColumn] = CHelper.GetUserName(principalId);

				MetaObject obj = StateMachineUtil.GetGlobalAclStateItem(mc.Name, mo.PrimaryKeyId.Value, int.Parse(ddlStateMachine.SelectedValue), int.Parse(ddlState.SelectedValue));

				for (int i = 1; i < grdMain.Columns.Count - 1; i++)
				{
					BoundField rightsField = grdMain.Columns[i] as BoundField;
					if (rightsField != null)
					{
						string fieldName = rightsField.DataField;
						if (obj != null)
							dr[fieldName] = CHelper.GetPermissionImage((int)obj.Properties[fieldName].Value);
						else
							dr[fieldName] = CHelper.GetPermissionImage((int)Mediachase.Ibn.Data.Services.Security.Rights.None);
					}
				}

				string url = String.Format("javascript:ShowWizard(&quot;{7}?ClassName={0}&btn={1}&PrincipalId={2}&SmId={3}&StateId={4}&quot;, {5}, {6});", 
					mc.Name, Page.ClientScript.GetPostBackEventReference(btnRefresh, ""), 
					principalId, ddlStateMachine.SelectedValue, ddlState.SelectedValue, 
					dialogWidth, dialogHeight,
					ResolveClientUrl("~/Apps/Security/Pages/Admin/GlobalRoleAclStateTransitionEdit.aspx"));
				dr[editColumn] = String.Format("<a href=\"{0}\"><img src=\"{1}\" title=\"{2}\" width=\"16\" height=\"16\" border=\"0\" /></a>", url, ResolveUrl("~/Images/IbnFramework/edit.gif"), GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Edit").ToString());

				dt.Rows.Add(dr);
			}

			grdMain.DataSource = dt;
			grdMain.DataBind();
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
			BindState();
		}
		#endregion

		#region ddlState_SelectedIndexChanged
		protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
		{
			GenerateStructure();
		}
		#endregion
	}
}