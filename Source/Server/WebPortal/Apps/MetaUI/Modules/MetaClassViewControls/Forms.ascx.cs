using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Database;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core.Layout;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Lists;
using System.Text;
using System.IO;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaUI.Modules.MetaClassViewControls
{
	public partial class Forms : MCDataBoundControl
	{
		protected readonly string className = "ClassName";
		protected readonly string deleteCommand = "Dlt";
		protected readonly string resetCommand = "Rst";
		protected readonly string recreateCommand = "Recreate";
		protected readonly string dialogWidth = "1000";
		protected readonly string dialogHeight = "700";
		protected readonly string historyPostfix = "_History";

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
		private MetaClass _mc;
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
			ScriptManager sm1 = ScriptManager.GetCurrent(this.Page);
			sm1.RegisterPostBackControl(btnRefresh);
		}

		#region DataBind
		public override void DataBind()
		{
			if (mc != null)
			{
				lnkNew.Text = GetGlobalResourceObject("IbnFramework.MetaForm", "NewForm").ToString();
				lnkNew.NavigateUrl = String.Format("javascript:ShowWizard(\"{4}?class={0}&btn={1}\", {2}, {3});", 
					mc.Name, btnRefresh.UniqueID, 350, 460,
					ResolveUrl("~/Apps/MetaUI/Pages/Public/FormDocumentEdit.aspx"));

				BindGrid();
			}
		}
		#endregion

		#region CheckVisibility
		public override bool CheckVisibility(object dataItem)
		{
			if (dataItem is MetaClass)
			{
				MetaClass mc = (MetaClass)dataItem;
				ListInfo li = ListManager.GetListInfoByMetaClassName(mc.Name);
				if (li != null && li.IsTemplate)
					return false;
			}
			return base.CheckVisibility(dataItem);
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Id", typeof(string));
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("EditLink", typeof(string));
			dt.Columns.Add("CanDelete", typeof(bool));
			dt.Columns.Add("CanReset", typeof(bool));
			dt.Columns.Add("PublicFormName", typeof(string));

			bool isBaseFormAdded = false;
			bool isGeneralViewFormAdded = false;
			bool isShortViewFormAdded = false;

			//ListApp Fix: we don't use ShortViewForm for lists
			bool isList = ListManager.MetaClassIsList(mc.Name);
			if (isList)
				isShortViewFormAdded = true;

			FormDocument[] mas;

			#region Forms
			mas = FormDocument.GetFormDocuments(mc.Name);
			foreach (FormDocument fd in mas)
			{
				DataRow row = dt.NewRow();
				row["Id"] = fd.Name;
				string name = "";
				if (MetaUIManager.MetaUITypeIsSystem(fd.MetaClassName, fd.MetaUITypeId) ||
						fd.MetaUITypeId == FormController.BaseFormType ||
						fd.MetaUITypeId == FormController.CreateFormType ||
						fd.MetaUITypeId == FormController.GeneralViewFormType ||
						fd.MetaUITypeId == FormController.GeneralViewHistoryFormType ||
						fd.MetaUITypeId == FormController.ShortViewFormType)
					name = CHelper.GetFormName(fd.MetaUITypeId);
				else
					name = String.Format("{0} ({1})", CHelper.GetFormName(fd.Name), CHelper.GetFormName(fd.MetaUITypeId));

				row["PublicFormName"] = "-1";
				
				//ListApp Fix
				if (ListManager.MetaClassIsList(mc.Name) && fd.MetaUITypeId == FormController.PublicEditFormType)
					row["PublicFormName"] = fd.Name;
				
				row["Name"] = name;
				row["EditLink"] = String.Format("javascript:OpenSizableWindow(\"{5}?ClassName={0}&btn={1}&FormName={2}\", {3}, {4});",
					mc.Name, btnRefresh.UniqueID,
					fd.Name, dialogWidth, dialogHeight,
					ResolveClientUrl("~/Apps/MetaUI/Pages/Admin/CustomizeObjectView2.aspx"));
				if (MetaUIManager.MetaUITypeIsSystem(fd.MetaClassName, fd.MetaUITypeId))
				{
					row["CanDelete"] = false;
					row["CanReset"] = !isList;
				}
				else
				{
					row["CanDelete"] = true;
					row["CanReset"] = false;
				}

				if (fd.MetaUITypeId == FormController.BaseFormType)
					isBaseFormAdded = true;
				if (fd.MetaUITypeId == FormController.GeneralViewFormType)
					isGeneralViewFormAdded = true;
				if (fd.MetaUITypeId == FormController.ShortViewFormType)
					isShortViewFormAdded = true;

				dt.Rows.Add(row);
			}

			// Edit Form
			if (!isBaseFormAdded)
			{
				DataRow row = dt.NewRow();
				string formType = FormController.BaseFormType;
				row["Id"] = formType;
				row["Name"] = CHelper.GetFormName(formType);
				row["PublicFormName"] = "-1";
				row["EditLink"] = String.Format("javascript:OpenSizableWindow(\"{5}?ClassName={0}&btn={1}&FormName={2}\", {3}, {4});",
					mc.Name, btnRefresh.UniqueID,
					formType, dialogWidth, dialogHeight,
					ResolveClientUrl("~/Apps/MetaUI/Pages/Admin/CustomizeObjectView2.aspx"));
				row["CanDelete"] = false;
				row["CanReset"] = false;
				dt.Rows.Add(row);
			}

			// View Form
			if (!isGeneralViewFormAdded)
			{
				DataRow row = dt.NewRow();
				string formType = FormController.GeneralViewFormType;
				row["Id"] = formType;
				row["Name"] = CHelper.GetFormName(formType);
				row["PublicFormName"] = "-1";
				row["EditLink"] = String.Format("javascript:OpenSizableWindow(\"{5}?ClassName={0}&btn={1}&FormName={2}\", {3}, {4});",
					mc.Name, btnRefresh.UniqueID,
					formType, dialogWidth, dialogHeight,
					ResolveClientUrl("~/Apps/MetaUI/Pages/Admin/CustomizeObjectView2.aspx"));
				row["CanDelete"] = false;
				row["CanReset"] = false;
				dt.Rows.Add(row);
			}

			// Short View Form
			if (!isShortViewFormAdded)
			{
				DataRow row = dt.NewRow();
				string formType = FormController.ShortViewFormType;
				row["Id"] = formType;
				row["Name"] = CHelper.GetFormName(formType);
				row["PublicFormName"] = "-1";
				row["EditLink"] = String.Format("javascript:OpenSizableWindow(\"{5}?ClassName={0}&btn={1}&FormName={2}\", {3}, {4});",
					mc.Name, btnRefresh.UniqueID,
					formType, dialogWidth, dialogHeight,
					ResolveClientUrl("~/Apps/MetaUI/Pages/Admin/CustomizeObjectView2.aspx"));
				row["CanDelete"] = false;
				row["CanReset"] = false;
				dt.Rows.Add(row);
			}
			#endregion

			#region History Forms
			string historyClassName = HistoryManager.GetHistoryMetaClassName(mc.Name);
			mas = FormDocument.GetFormDocuments(historyClassName);
			foreach (FormDocument fd in mas)
			{
				DataRow row = dt.NewRow();
				row["Id"] = String.Format(CultureInfo.InvariantCulture, "{0}{1}", fd.Name, historyPostfix);
				string name = "";
				if (MetaUIManager.MetaUITypeIsSystem(fd.MetaClassName, fd.MetaUITypeId) ||
						fd.MetaUITypeId == FormController.BaseFormType || 
						fd.MetaUITypeId == FormController.CreateFormType ||
						fd.MetaUITypeId == FormController.GeneralViewFormType ||
						fd.MetaUITypeId == FormController.GeneralViewHistoryFormType ||
						fd.MetaUITypeId == FormController.ShortViewFormType)
					name = CHelper.GetFormName(fd.MetaUITypeId);
				else
					name = String.Format("{0} ({1})", CHelper.GetFormName(fd.Name), CHelper.GetFormName(fd.MetaUITypeId));

				row["PublicFormName"] = "-1";

				row["Name"] = name;
				row["EditLink"] = String.Format("javascript:OpenSizableWindow(\"{5}?ClassName={0}&btn={1}&FormName={2}\", {3}, {4});",
					historyClassName, btnRefresh.UniqueID,
					fd.Name, dialogWidth, dialogHeight,
					ResolveClientUrl("~/Apps/MetaUI/Pages/Admin/CustomizeObjectView2.aspx"));

				if (MetaUIManager.MetaUITypeIsSystem(fd.MetaClassName, fd.MetaUITypeId))
					row["CanDelete"] = false;
				else
					row["CanDelete"] = true;
				row["CanReset"] = false;

				dt.Rows.Add(row);
			}
			#endregion

			DataView dv = dt.DefaultView;
			dv.Sort = "Name";

			grdMain.DataSource = dv;
			grdMain.DataBind();

			foreach (DataGridItem row in grdMain.Items)
			{
				ImageButton ib;
				ib = (ImageButton)row.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Delete").ToString() + "?')");

				ib = (ImageButton)row.FindControl("ibReset");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "ResetToDefault").ToString() + "?')");

				ib = (ImageButton)row.FindControl("ibRecreate");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("IbnFramework.MetaForm", "RecreateFormWarning").ToString() + "')");
			}
		}
		#endregion

		#region grdMain_RowDataBound
		protected void grdMain_RowDataBound(object sender, DataGridItemEventArgs e)
		{
			//ListApp Fix

			TextBox tb = (TextBox)e.Item.FindControl("txtLink");
			if (tb == null)
				return;

			if (!String.IsNullOrEmpty(e.Item.Cells[0].Text) && e.Item.Cells[0].Text != "-1")
			{
				tb.Visible = true;
				string fName = e.Item.Cells[0].Text;
				string sPath = ResolveUrl("~/Public/PublicListItemAdd.aspx");
				string ss = "ClassName=" + mc.Name + "&FormName=" + fName;
				ss = HttpUtility.UrlEncode(ss);
				string sc = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(ss));
				sPath += "?uid=" + sc;
				tb.Text = String.Format("<iframe src='{0}' style='border: 0' width='100%' height='100%' frameborder='0' scrolling='auto'></iframe>", sPath);
				tb.Style.Add("margin-top", "10px");
			}
			else
				tb.Visible = false;
		}
		#endregion

		#region grdMain_RowCommand
		protected void grdMain_RowCommand(object source, DataGridCommandEventArgs e)
		{
			if (e == null)
				return;

			if (e.CommandName == deleteCommand)
			{
				string id = e.CommandArgument.ToString();
				FormDocument fd = FormDocument.Load(mc.Name, id);
				fd.Delete();
				if (id == FormController.GeneralViewFormType || id == FormController.GeneralViewHistoryFormType)
					MetaDataWrapper.RemoveClassAttribute(mc, "HasCompositePage");
			}
			else if (e.CommandName == resetCommand)
			{
				string id = e.CommandArgument.ToString();
				FormDocument fd = FormDocument.Load(mc.Name, id);
				fd.Delete();
			}
			else if (e.CommandName == recreateCommand)
			{
				string id = e.CommandArgument.ToString();

				FormDocument fd = FormDocument.Load(mc.Name, id);

				if (fd != null)
				{
					fd = FormController.ReCreateFormDocument(mc.Name, fd.Name);

					//ListApp Fix
					if (ListManager.MetaClassIsList(mc.Name) && fd.FormTable.Rows[0].Cells[0].Sections.Count > 0)
					{
						fd.FormTable.Rows[0].Cells[0].Sections[0].BorderType = (int)BorderType.None;
						fd.FormTable.Rows[0].Cells[0].Sections[0].ShowLabel = false;
					}
				}
			}

			CHelper.RequireDataBind();
		}
		#endregion

		#region btnRefresh_Click
		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			CHelper.RequireDataBind();

			string param = Request.Params.Get("__EVENTARGUMENT");
			if (String.IsNullOrEmpty(param))
				return;

			FormDocument FormDocumentData = (FormDocument)Session[param];
			FormDocumentData.Save();

			if (FormDocumentData.MetaUITypeId == FormController.GeneralViewFormType || FormDocumentData.MetaUITypeId == FormController.GeneralViewHistoryFormType)
				MetaDataWrapper.AddClassAttribute(MetaDataWrapper.GetMetaClassByName(FormDocumentData.MetaClassName), "HasCompositePage", true);
		}
		#endregion
	}
}