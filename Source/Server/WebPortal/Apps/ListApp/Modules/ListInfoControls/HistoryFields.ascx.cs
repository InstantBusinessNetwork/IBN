using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Lists;

namespace Mediachase.Ibn.Web.UI.ListApp.Modules.ListInfoControls
{
	public partial class HistoryFields : UserControl
	{
		protected readonly string deleteCommand = "Dlt";

		#region ClassName
		public string ClassName
		{
			get
			{
				return (string)ViewState["ClassName"];
			}
			set
			{
				ViewState["ClassName"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			CommandManager.GetCurrent(this.Page).AddCommand("", "", "ListInfoView", "MC_ListApp_HistoryProfileEdit");
			CommandManager.GetCurrent(this.Page).AddCommand("", "", "ListInfoView", "MC_ListApp_NeedToDataBind");
			CommandManager.GetCurrent(this.Page).AddCommand("", "", "ListInfoView", "MC_ListApp_AddHistoryFieldFrame");
		}

		#region DataBind
		public override void DataBind()
		{
			if (!ListManager.IsHistoryActivated(ClassName))
				return;
			if (!String.IsNullOrEmpty(ClassName))
			{
				BindBlockHeader();
				BindGrid();
			}
		}
		#endregion

		#region BindBlockHeader
		private void BindBlockHeader()
		{
			// Check, that there are fields left
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(ClassName);
			HistoryMetaClassInfo historyInfo = HistoryManager.GetInfo(mc);
			Collection<string> selectedFields = historyInfo.SelectedFields;
			bool fieldsExist = false;
			foreach (MetaField mf in mc.Fields)
			{
				if (HistoryManager.IsSupportedField(mf) && !selectedFields.Contains(mf.Name))
				{
					fieldsExist = true;
					break;
				}
			}

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp;

			if (fieldsExist)
			{
				string id = String.Empty;
				ListViewProfile[] mas = ListViewProfile.GetSystemProfiles(HistoryManager.GetHistoryMetaClassName(ClassName), "ItemHistoryList");
				if (mas.Length == 0)
					id = CHelper.GetHistorySystemListViewProfile(HistoryManager.GetHistoryMetaClassName(ClassName), "ItemHistoryList");
				else
					id = mas[0].Id;

				cp = new CommandParameters("MC_ListApp_HistoryProfileEdit");
				cp.CommandArguments = new Dictionary<string, string>();
				cp.AddCommandArgument("ClassName", HistoryManager.GetHistoryMetaClassName(ClassName));
				cp.AddCommandArgument("uid", id);
				string cmd = cm.AddCommand("", "", "ListInfoView", cp);
				MainBlockHeader.AddRightLink(
					GetGlobalResourceObject("IbnFramework.ListInfo", "HistoryView").ToString(),
					String.Format("javascript:{{{0}}};", cmd));

				cp = new CommandParameters("MC_ListApp_AddHistoryFieldFrame", new Dictionary<string, string>());
				cp.AddCommandArgument("ClassName", ClassName);
				string command = cm.AddCommand("", "", "ListInfoView", cp);

				MainBlockHeader.AddRightLink(
					GetGlobalResourceObject("IbnFramework.ListInfo", "AddField").ToString(),
					String.Format("javascript:{{{0}}};", command));
			}
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(ClassName);
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Id", typeof(string));
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("Type", typeof(string));

			HistoryMetaClassInfo historyInfo = HistoryManager.GetInfo(mc);
			Collection<string> selectedFields = historyInfo.SelectedFields;

			foreach (string fieldName in selectedFields)
			{
				MetaField mf = mc.Fields[fieldName];
				if (mf == null)
					continue;

				DataRow row = dt.NewRow();
				row["Id"] = fieldName;
				row["Name"] = CHelper.GetMetaFieldName(mf);
				row["Type"] = CHelper.GetMcDataTypeName(mf.GetOriginalMetaType().McDataType);

				dt.Rows.Add(row);
			}

			MainGrid.DataSource = dt;
			MainGrid.DataBind();

			foreach (GridViewRow row in MainGrid.Rows)
			{
				ImageButton ib = (ImageButton)row.FindControl("DeleteButton");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Delete").ToString() + "?')");
			}
		}
		#endregion

		#region MainGrid_RowCommand
		protected void MainGrid_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == deleteCommand && !String.IsNullOrEmpty(ClassName))
			{
				string fieldName = e.CommandArgument.ToString();
				MetaClass mc = MetaDataWrapper.GetMetaClassByName(ClassName);

				HistoryMetaClassInfo historyInfo = HistoryManager.GetInfo(mc);
				historyInfo.SelectedFields.Remove(fieldName);
				HistoryManager.SetInfo(mc, historyInfo);

				CHelper.RequireDataBind();
			}
		}
		#endregion
	}
}