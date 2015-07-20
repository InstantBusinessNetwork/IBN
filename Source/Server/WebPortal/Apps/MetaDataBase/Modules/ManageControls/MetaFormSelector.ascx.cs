using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Core.Layout;
using Mediachase.Ibn.Lists;
using System.Collections.Generic;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Modules.ManageControls
{
	public partial class MetaFormSelector : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		#region .prop MetaForms
		public List<FormDocument> MetaForms
		{
			get
			{
				List<FormDocument> retVal = new List<FormDocument>();
				foreach (DataGridItem dgi in grdMain.Items)
				{
					if (!dgi.Cells[1].Text.Equals("true"))
						continue;

					string className = dgi.Cells[0].Text;
					CheckBox cb = (CheckBox)dgi.FindControl("cbIsAdd");
					if (cb != null && cb.Checked)
					{
						FormDocument fd = new FormDocument();
						fd.MetaClassName = className;
						fd.Name = grdMain.DataKeys[dgi.ItemIndex].ToString();
						retVal.Add(fd);
					}
				}
				return retVal;
			}
		} 
		#endregion

		#region .prop MetaViews
		[Obsolete]
		public List<string> MetaViews
		{
			get
			{
				List<string> retVal = new List<string>();
				foreach (DataGridItem dgi in grdMain.Items)
				{
					if (dgi.Cells[1].Text.Equals("true"))
						continue;

					CheckBox cb = (CheckBox)dgi.FindControl("cbIsAdd");
					if (cb != null && cb.Checked)
					{
						string sName = grdMain.DataKeys[dgi.ItemIndex].ToString();
						retVal.Add(sName);
					}
				}
				return retVal;
			}
		}
		#endregion

		#region .prop ListProfiles
		public List<string> ListProfiles
		{
			get
			{
				List<string> retVal = new List<string>();
				foreach (DataGridItem dgi in grdMain.Items)
				{
					if (dgi.Cells[1].Text.Equals("true"))
						continue;

					CheckBox cb = (CheckBox)dgi.FindControl("cbIsAdd");
					if (cb != null && cb.Checked)
					{
						string sName = grdMain.DataKeys[dgi.ItemIndex].ToString();
						retVal.Add(sName);
					}
				}
				return retVal;
			}
		}
		#endregion

		#region .prop Count
		public int Count
		{
			get
			{
				return grdMain.Items.Count;
			}
		}
		#endregion

		#region BindData
		public void BindData(string metaClassName)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("DisplayName", typeof(string)));
			dt.Columns.Add(new DataColumn("ClassName", typeof(string)));
			dt.Columns.Add(new DataColumn("IsForm", typeof(string)));

			bool isBaseFormAdded = false;
			bool isGeneralViewFormAdded = false;
			bool isShortViewFormAdded = false;

			// ListApp Fix: we don't use ShortViewForm for lists
			bool isList = ListManager.MetaClassIsList(metaClassName);
			if (isList)
				isShortViewFormAdded = true;

			string[] forms = MetaUIManager.GetMetaFormList(metaClassName);
			foreach (string name in forms)
			{
				if ((name == FormController.CreateFormType || name == FormController.ShortViewFormType) && isList)
					continue;
				AddRow(dt, metaClassName, name, String.Empty, true, false);

				if (name == FormController.BaseFormType)
					isBaseFormAdded = true;
				if (name == FormController.GeneralViewFormType)
					isGeneralViewFormAdded = true;
				if (name == FormController.ShortViewFormType)
					isShortViewFormAdded = true;
			}

			// Edit Form
			if (!isBaseFormAdded)
				AddRow(dt, metaClassName, FormController.BaseFormType, String.Empty, true, false);

			// View Form
			if (!isGeneralViewFormAdded)
				AddRow(dt, metaClassName, FormController.GeneralViewFormType, String.Empty, true, false);

			// Short View Form
			if (!isShortViewFormAdded)
				AddRow(dt, metaClassName, FormController.ShortViewFormType, String.Empty, true, false);

			if (ListManager.IsHistoryActivated(metaClassName))
			{
				string[] formsHistory = MetaUIManager.GetMetaFormList(HistoryManager.GetHistoryMetaClassName(metaClassName));
				foreach (string name in formsHistory)
				{
					if (name != FormController.GeneralViewHistoryFormType && isList)
						continue;
					AddRow(dt, HistoryManager.GetHistoryMetaClassName(metaClassName), name, String.Empty, true, true);
				}
			}

			ListViewProfile[] list = ListViewProfile.GetSystemProfiles(metaClassName, "EntityList");
			foreach (ListViewProfile lvp in list)
				AddRow(dt, metaClassName, lvp.Id, lvp.Name, false, false);
			//string[] views = MetaUIManager.GetMetaViewList(metaClassName);
			//foreach (string name in views)
			//    AddRow(dt, metaClassName, name, false, false);

			grdMain.DataKeyField = "Name";
			grdMain.DataSource = dt.DefaultView;
			grdMain.DataBind();
		}

		private void AddRow(DataTable dt, string metaClassName, string name, string friendlyName, bool isForm, bool isHistory)
		{
			DataRow dr = dt.NewRow();
			dr["Name"] = name;
			dr["ClassName"] = metaClassName;
			dr["IsForm"] = isForm.ToString().ToLower();
			string sName = String.Empty;
			if(isForm)
				sName = String.Format(" {0}{1}",
					CHelper.GetFormName(name),
					((name != FormController.BaseFormType && name != FormController.CreateFormType && name != FormController.GeneralViewFormType && name != FormController.ShortViewFormType && name != FormController.GeneralViewHistoryFormType) ? " (" + GetGlobalResourceObject("IbnFramework.MetaForm", "FormName").ToString() + ")" : ""));
			else
				sName = String.Format(" {0}{1}",
					GetGlobalResourceObject("IbnFramework.ListInfo", "ListView").ToString(),
					" (" + CHelper.GetResFileString(friendlyName) + ")");
			
			dr["DisplayName"] = sName;
			dt.Rows.Add(dr);
		}
		#endregion
	}
}