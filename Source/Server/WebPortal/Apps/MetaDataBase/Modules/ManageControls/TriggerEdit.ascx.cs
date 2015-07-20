using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class TriggerEdit : System.Web.UI.UserControl
	{

		protected string labelColumnWidth = "120px";
		protected int iLabelColumnWidth = 120;

		#region RefreshButton
		public string RefreshButton
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["btn"] != null)
					retval = Request.QueryString["btn"];
				return retval;
			}
		}
		#endregion

		#region ClassName
		public string ClassName
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["ClassName"] != null)
					retval = Request.QueryString["ClassName"];
				return retval;
			}
		}
		#endregion

		#region TriggerName
		public string TriggerName
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["TriggerName"] != null)
					retval = Request.QueryString["TriggerName"];
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				FillDropDowns();
				BindData();
			}
			else
			{
				GenerateConditionStructure(ddlCondition.SelectedValue);
				GenerateActionStructure(ddlAction.SelectedValue);
			}
		}

		#region FillDropDowns
		private void FillDropDowns()
		{
			foreach (TriggerMethodInfo info in TriggerManager.ConditionMethods)
			{
				ddlCondition.Items.Add(new ListItem(info.Name));
			}

			foreach (TriggerMethodInfo info in TriggerManager.ActionMethods)
			{
				ddlAction.Items.Add(new ListItem(info.Name));
			}
		}
		#endregion

		#region GenerateConditionStructure
		private void GenerateConditionStructure(string condition)
		{
			if (tblCondition.Rows.Count > 0)
			{
				tblCondition.Rows.Clear();
			}

			TriggerMethodInfo info = TriggerManager.GetConditionMethod(condition);
			ParameterInfo[] paramList = info.InParameters;
			int i = 0;
			foreach (ParameterInfo param in paramList)
			{
				i++;

				TableRow tr = new TableRow();
				tblCondition.Rows.Add(tr);

				TableCell td1 = new TableCell();
				td1.Width = Unit.Pixel(iLabelColumnWidth - 2);
				tr.Cells.Add(td1);

				Label lbl = new Label();
				lbl.Text = param.Name + ":";
				td1.Controls.Add(lbl);

				TableCell td2 = new TableCell();
				tr.Cells.Add(td2);

				TextBox txt = new TextBox();
				txt.ID = String.Format("txtCond{0}", i);
				txt.Width = Unit.Percentage(100);
				td2.Controls.Add(txt);
			}
		}
		#endregion

		#region GenerateActionStructure
		private void GenerateActionStructure(string action)
		{
			if (String.IsNullOrEmpty(action))
				return;
			
			if (tblCondition.Rows.Count > 0)
			{
				tblAction.Rows.Clear();
			}

			TriggerMethodInfo info = TriggerManager.GetActionMethod(action);
			ParameterInfo[] paramList = info.InParameters;
			int i = 0;
			foreach (ParameterInfo param in paramList)
			{
				i++;

				TableRow tr = new TableRow();
				tblAction.Rows.Add(tr);

				TableCell td1 = new TableCell();
				td1.Width = Unit.Pixel(iLabelColumnWidth - 2);
				tr.Cells.Add(td1);

				Label lbl = new Label();
				lbl.Text = param.Name + ":";
				td1.Controls.Add(lbl);

				TableCell td2 = new TableCell();
				tr.Cells.Add(td2);

				TextBox txt = new TextBox();
				txt.ID = String.Format("txtAct{0}", i);
				txt.Width = Unit.Percentage(100);
				td2.Controls.Add(txt);
			}
		}
		#endregion

		#region BindData
		private void BindData()
		{
			List<string> conditionInParameters = new List<string>();
			List<string> actionInParameters = new List<string>();
			if (!String.IsNullOrEmpty(TriggerName))
			{
				Trigger trigger = TriggerManager.GetTrigger(ClassName, TriggerName);

				txtName.Text = TriggerName;
				txtName.Enabled = false;

				txtDescription.Text = trigger.Description;
				chkInsert.Checked = trigger.Create;
				chkUpdate.Checked = trigger.Update;
				chkDelete.Checked = trigger.Delete;
				CHelper.SafeSelect(ddlCondition, trigger.GetConditionMethod().Name);
				CHelper.SafeSelect(ddlAction, trigger.GetActionMethod().Name);

				conditionInParameters = trigger.ConditionInParameters;
				actionInParameters = trigger.ActionInParameters;
			}

			GenerateConditionStructure(ddlCondition.SelectedValue);
			GenerateActionStructure(ddlAction.SelectedValue);

			for (int i = 0; i < tblCondition.Rows.Count; i++)
			{
				if (conditionInParameters.Count < i + 1)
					break;

				TextBox txt = (TextBox)tblCondition.Rows[i].Cells[1].Controls[0];
				txt.Text = conditionInParameters[i];
			}

			for (int i = 0; i < tblAction.Rows.Count; i++)
			{
				if (actionInParameters.Count < i + 1)
					break;

				TextBox txt = (TextBox)tblAction.Rows[i].Cells[1].Controls[0];
				txt.Text = actionInParameters[i];
			}
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			using (MetaClassManagerEditScope editScope = DataContext.Current.MetaModel.BeginEdit())
			{
				Trigger trigger;
				if (TriggerName != String.Empty)
				{
					trigger = TriggerManager.GetTrigger(ClassName, TriggerName);
				}
				else
				{
					trigger = new Trigger();
					trigger.Name = txtName.Text.Trim();
				}

				trigger.ActionName = ddlAction.SelectedValue;
				trigger.ConditionName = ddlCondition.SelectedValue;
				trigger.Create = chkInsert.Checked;
				trigger.Delete = chkDelete.Checked;
				trigger.Update = chkUpdate.Checked;
				trigger.Description = txtDescription.Text;

				List<string> conditionInParameters = new List<string>();
				for (int i = 0; i < tblCondition.Rows.Count; i++)
				{
					TextBox txt = (TextBox)tblCondition.Rows[i].Cells[1].Controls[0];
					conditionInParameters.Add(txt.Text.Trim());
				}
				trigger.ConditionInParameters = conditionInParameters;

				List<string> actionInParameters = new List<string>();
				for (int i = 0; i < tblAction.Rows.Count; i++)
				{
					TextBox txt = (TextBox)tblAction.Rows[i].Cells[1].Controls[0];
					actionInParameters.Add(txt.Text.Trim());
				}
				trigger.ActionInParameters = actionInParameters;

				if (TriggerName == String.Empty)
					TriggerManager.AddTrigger(ClassName, trigger);

				editScope.SaveChanges();
			}

			// Closing window
			if (RefreshButton == String.Empty)
			{
				CHelper.CloseItAndRefresh(Response);
			}
			else  // Dialog Mode
			{
				CHelper.CloseItAndRefresh(Response, RefreshButton);
			}
		}
		#endregion

		#region ddlCondition_SelectedIndexChanged
		protected void ddlCondition_SelectedIndexChanged(object sender, EventArgs e)
		{
			GenerateConditionStructure(ddlCondition.SelectedValue);
		}
		#endregion

		#region ddlAction_SelectedIndexChanged
		protected void ddlAction_SelectedIndexChanged(object sender, EventArgs e)
		{
			GenerateActionStructure(ddlAction.SelectedValue);
		}
		#endregion

		[TriggerCondition("ConditionName", "ConditionDescription")]
		public static bool IsStateChanged2(string param1, string param2, string param3)
		{
			return true;
		}
	}
}