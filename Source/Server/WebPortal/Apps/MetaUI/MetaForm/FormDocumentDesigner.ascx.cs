using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Layout;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaUI
{
	public partial class FormDocumentDesigner : System.Web.UI.UserControl
	{
		#region MetaClassName
		public string MetaClassName
		{
			get
			{
				if (ViewState[this.ID + "_MetaClassName"] == null)
					ViewState[this.ID + "_MetaClassName"] = (Request["ClassName"] != null) ? Request["ClassName"] : "";
				return (string)ViewState[this.ID + "_MetaClassName"];
			}
			set
			{
				ViewState[this.ID + "_MetaClassName"] = value;
			}
		}
		#endregion

		#region FormName
		public string FormName
		{
			get
			{
				if (ViewState[this.ID + "_FormName"] == null)
					ViewState[this.ID + "_FormName"] = (Request["FormName"] != null) ? Request["FormName"] : "";
				return (string)ViewState[this.ID + "_FormName"];
			}
			set
			{
				ViewState[this.ID + "_FormName"] = value;
			}
		}
		#endregion

		#region FormDocumentData
		public FormDocument FormDocumentData
		{
			get
			{
				if (ViewState[this.ID + "_FormDocumentData"] == null)
					return null;
				return (FormDocument)ViewState[this.ID + "_FormDocumentData"];
			}
			set
			{
				ViewState[this.ID + "_FormDocumentData"] = value;
			}
		}
		#endregion

		#region CanAddNewForm
		public bool CanAddNewForm
		{
			get
			{
				if (ViewState[this.ID + "_CanAddNewForm"] == null)
					ViewState[this.ID + "_CanAddNewForm"] = false;
				return (bool)ViewState[this.ID + "_CanAddNewForm"];
			}
			set
			{
				ViewState[this.ID + "_CanAddNewForm"] = value;
			}
		}
		#endregion

		private string scriptNewForm = "<a href='#' onclick=\"javascript:OpenFDDPopUp('{0}', 350, 460, {2});\">{1}</a>";
		private string scriptNewSection = "<a href='#' onclick=\"javascript:OpenFDDPopUp('{0}', 350, 520, {2});\">{1}</a>";
		private string scriptNewItem = "<a href='#' onclick=\"javascript:OpenFDDPopUp('{0}', 350, 550, {2});\">{1}</a>";

		protected void Page_Load(object sender, EventArgs e)
		{
			MainMetaToolbar.ClassName = "";
			MainMetaToolbar.ViewName = "";
			MainMetaToolbar.PlaceName = "FormDocumentDesigner";

			topMetaBar.ClassName = "";
			topMetaBar.ViewName = "";
			topMetaBar.PlaceName = "FormDocumentDesignerTop";

			if (!Page.IsPostBack)
			{
				if (!CanAddNewForm && (String.IsNullOrEmpty(MetaClassName) || String.IsNullOrEmpty(FormName)))
					throw new Exception("MetaClassName and FormName are required properties!");
				//CreateDocument();
				BindDD();

				// Ibn47 Lists
				if (ListManager.MetaClassIsList(MetaClassName))
					TableLabel.Text = GetGlobalResourceObject("IbnFramework.ListInfo", "List").ToString();
			}
			tdRight.Visible = false;
		}

		#region BindDD
		private void BindDD()
		{
			if (CanAddNewForm)
			{
				//Dictionary<int, string> dic = Mediachase.Ibn.Data.Meta.Management.SqlSerialization.MetaClassId.GetIds();
				//List<string> list = new List<string>(dic.Values);
				List<string> list = new List<string>();
				foreach (MetaClass mc in DataContext.Current.MetaModel.MetaClasses)
					list.Add(mc.Name);
				list.Sort();

				ddClasses.DataSource = list;
				ddClasses.DataBind();

				if (!String.IsNullOrEmpty(MetaClassName))
					CHelper.SafeSelect(ddClasses, MetaClassName);

				MetaClassName = ddClasses.SelectedValue;
			}
			else
				lblTempClassName.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(MetaClassName).FriendlyName);

			lblTableName.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(MetaClassName).FriendlyName);

			ddClasses.Visible = CanAddNewForm;
			lblTempClassName.Visible = !CanAddNewForm;

			BindForms();
		}
		#endregion

		#region BindForms
		private void BindForms()
		{
			string className = MetaClassName;

			if (CanAddNewForm)
			{
				ddFormName.Items.Clear();
				FormDocument[] mas = FormDocument.GetFormDocuments(className);
				foreach (FormDocument fd in mas)
					ddFormName.Items.Add(new ListItem(CHelper.GetFormName(fd.Name), fd.Name));

				if (!String.IsNullOrEmpty(FormName))
					CHelper.SafeSelect(ddFormName, FormName);

				FormName = (ddFormName.SelectedItem != null) ? ddFormName.SelectedValue : "";
			}
			else
				lblTempFormName.Text = FormName;

			ddFormName.Visible = CanAddNewForm;
			lblTempFormName.Visible = !CanAddNewForm;

			BindRenderer();
		}
		#endregion

		#region BindRenderer
		private void BindRenderer()
		{
			FormDocumentData = null;
			try
			{
				FormDocumentData = FormDocument.Load(MetaClassName, FormName);

				if (FormDocumentData == null)
				{
					FormDocumentData = FormController.ReCreateFormDocument(MetaClassName, FormName);
				}

				if (MetaUIManager.MetaUITypeIsSystem(FormDocumentData.MetaClassName, FormDocumentData.MetaUITypeId)
					|| FormName == FormDocumentData.MetaUITypeId)
					lblFormName.Text = CHelper.GetFormName(FormName);
				else
					lblFormName.Text = String.Format("{0} ({1})", CHelper.GetFormName(FormName), CHelper.GetFormName(FormDocumentData.MetaUITypeId));
			}
			catch { }
			BindRendererInner();
		}
		#endregion

		#region BindRendererInner
		private void BindRendererInner()
		{
			fRenderer.FormDocumentData = FormDocumentData;
			fRenderer.DataBind();

			if (FormDocumentData == null)
			{
				ddFormName.Enabled = false;
				tblMove.Visible = false;
				tblLinks.Visible = false;
			}
			else
			{
				if (ddFormName.Visible)
					ddFormName.Enabled = true;
				tblMove.Visible = true;
				tblLinks.Visible = true;
			}
		}
		#endregion

		protected override void OnPreRender(EventArgs e)
		{
			ApplyActions();
			//txtXml.Text = FormDocumentData.GetFormTableXml();
			base.OnPreRender(e);
		}

		#region ApplyActions
		private void ApplyActions()
		{
			imgLeft.ImageUrl = CHelper.GetAbsolutePath("Images/IbnFramework/Arrows/Left.gif");
			imgUp.ImageUrl = CHelper.GetAbsolutePath("Images/IbnFramework/Arrows/Up.gif");
			imgRight.ImageUrl = CHelper.GetAbsolutePath("Images/IbnFramework/Arrows/Right.gif");
			imgDown.ImageUrl = CHelper.GetAbsolutePath("Images/IbnFramework/Arrows/Down.gif");

			imgLeft.Style.Add("cursor", "pointer");
			imgUp.Style.Add("cursor", "pointer");
			imgRight.Style.Add("cursor", "pointer");
			imgDown.Style.Add("cursor", "pointer");

			string clientID = fRenderer.TableContainer;
			imgLeft.Attributes.Add("onclick", String.Format("__doPostBack('{1}', $find('{0}').getSelection());", clientID, lbLeft.UniqueID));
			imgUp.Attributes.Add("onclick", String.Format("__doPostBack('{1}', $find('{0}').getSelection());", clientID, lbTop.UniqueID));
			imgRight.Attributes.Add("onclick", String.Format("__doPostBack('{1}', $find('{0}').getSelection());", clientID, lbRight.UniqueID));
			imgDown.Attributes.Add("onclick", String.Format("__doPostBack('{1}', $find('{0}').getSelection());", clientID, lbDown.UniqueID));

			lbRemoveField.Attributes.Add("onclick", String.Format("if(confirm('{2}')){{__doPostBack('{1}', $find('{0}').getSelection());}} else return false;", clientID, lbRemoveField.UniqueID, GetGlobalResourceObject("IbnFramework.MetaForm", "WarningField").ToString()));
			lbRemoveSection.Attributes.Add("onclick", String.Format("if(confirm('{2}')){{__doPostBack('{1}', $find('{0}').getSelection());}} else return false;", clientID, lbRemoveSection.UniqueID, GetGlobalResourceObject("IbnFramework.MetaForm", "WarningSection").ToString()));

			if (CanAddNewForm)
				lblNewForm.Text = String.Format(scriptNewForm,
					String.Format("{2}?btn={0}&class={1}",
						lbNewForm.UniqueID, ddClasses.SelectedItem != null ? ddClasses.SelectedValue : "",
						CHelper.GetAbsolutePath("/Apps/MetaUI/Pages/Public/FormDocumentEdit.aspx")),
					GetGlobalResourceObject("IbnFramework.MetaForm", "AddForm").ToString(),
					"null");

			string uid = Guid.NewGuid().ToString("N");
			hFieldKey.Value = uid;
			Session[uid] = FormDocumentData;
			lblEditForm.Text = String.Format(scriptNewForm,
				String.Format("{0}?uid={1}&btn={2}",
					CHelper.GetAbsolutePath("/Apps/MetaUI/Pages/Public/FormDocumentEdit.aspx"),
					uid, lbNewForm.UniqueID),
				GetGlobalResourceObject("IbnFramework.MetaForm", "EditForm").ToString(),
				"null");

			lblNewSection.Text = String.Format(scriptNewSection,
				String.Format("{0}?uid={1}&btn={2}",
					CHelper.GetAbsolutePath("/Apps/MetaUI/Pages/Public/FormSectionEdit.aspx"),
					uid, lbAddSection.UniqueID),
				GetGlobalResourceObject("IbnFramework.MetaForm", "AddSection").ToString(),
				"null");

			lblEditSection.Text = String.Format(scriptNewSection,
				String.Format("{0}?uid={1}&btn={2}",
					CHelper.GetAbsolutePath("/Apps/MetaUI/Pages/Public/FormSectionEdit.aspx"),
					uid, lbAddSection.UniqueID),
				GetGlobalResourceObject("IbnFramework.MetaForm", "EditSection").ToString(),
				"'" + clientID + "'");

			lblAddField.Text = String.Format(scriptNewItem,
				String.Format("{0}?add=1&uid={1}&btn={2}",
					CHelper.GetAbsolutePath("/Apps/MetaUI/Pages/Public/FormItemEdit.aspx"),
					uid, lbAddSection.UniqueID),
				GetGlobalResourceObject("IbnFramework.MetaForm", "AddField").ToString(),
				"'" + clientID + "'");

			lblEditField.Text = String.Format(scriptNewItem,
				String.Format("{0}?uid={1}&btn={2}",
					CHelper.GetAbsolutePath("/Apps/MetaUI/Pages/Public/FormItemEdit.aspx"),
					uid, lbAddSection.UniqueID),
				GetGlobalResourceObject("IbnFramework.MetaForm", "EditField").ToString(),
				"'" + clientID + "'");
		}
		#endregion

		#region lbDown_Click
		protected void lbDown_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			Guid uid = Guid.Empty;
			try
			{
				uid = new Guid(param);
			}
			catch
			{
				return;
			}

			FormController fController = new FormController(FormDocumentData);
			FormSection itemSection = fController.GetSectionByUid(uid);
			if (itemSection != null)
				fController.MoveSectionDown(uid);
			else
			{
				FormItem item = fController.GetSTLItemByUid(uid);
				if (item != null)
					fController.MoveFormItemDown(uid);
			}

			BindRendererInner();
		}
		#endregion

		#region lbRight_Click
		protected void lbRight_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			Guid uid = Guid.Empty;
			try
			{
				uid = new Guid(param);
			}
			catch
			{
				return;
			}

			FormController fController = new FormController(FormDocumentData);
			FormSection itemSection = fController.GetSectionByUid(uid);
			if (itemSection != null)
				fController.MoveSectionRight(uid);
			else
			{
				FormItem item = fController.GetSTLItemByUid(uid);
				if (item != null)
					fController.MoveFormItemRight(uid);
			}

			BindRendererInner();
		}
		#endregion

		#region lbTop_Click
		protected void lbTop_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			Guid uid = Guid.Empty;
			try
			{
				uid = new Guid(param);
			}
			catch
			{
				return;
			}

			FormController fController = new FormController(FormDocumentData);
			FormSection itemSection = fController.GetSectionByUid(uid);
			if (itemSection != null)
				fController.MoveSectionUp(uid);
			else
			{
				FormItem item = fController.GetSTLItemByUid(uid);
				if (item != null)
					fController.MoveFormItemUp(uid);
			}

			BindRendererInner();
		}
		#endregion

		#region lbLeft_Click
		protected void lbLeft_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			Guid uid = Guid.Empty;
			try
			{
				uid = new Guid(param);
			}
			catch
			{
				return;
			}

			FormController fController = new FormController(FormDocumentData);
			FormSection itemSection = fController.GetSectionByUid(uid);
			if (itemSection != null)
				fController.MoveSectionLeft(uid);
			else
			{
				FormItem item = fController.GetSTLItemByUid(uid);
				if (item != null)
					fController.MoveFormItemLeft(uid);
			}

			BindRendererInner();
		}
		#endregion

		#region Add/Edit Form
		protected void lbNewForm_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			if (String.IsNullOrEmpty(param))
				return;

			FormDocumentData = (FormDocument)Session[param];

			MetaClassName = FormDocumentData.MetaClassName;
			FormName = FormDocumentData.Name;

			fRenderer.FormDocumentData = FormDocumentData;
			fRenderer.DataBind();

			#region Visibility Elements
			if (CanAddNewForm)
			{
				bool changeForms = (ddClasses.SelectedItem == null || ddClasses.SelectedValue != MetaClassName);

				CHelper.SafeSelect(ddClasses, MetaClassName);
				if (ddClasses.SelectedItem == null || ddClasses.SelectedValue != MetaClassName)
				{
					ddClasses.Visible = false;
					lblTempClassName.Visible = true;
					lblTempClassName.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(MetaClassName).FriendlyName);
				}
				else
				{
					ddClasses.Visible = true;
					lblTempClassName.Visible = false;
				}

				if (changeForms)
				{
					ddFormName.Visible = false;
					lblTempFormName.Visible = true;
					lblTempFormName.Text = CHelper.GetFormName(FormName);
				}
				else
				{
					CHelper.SafeSelect(ddFormName, FormName);
					if (ddFormName.SelectedItem == null || ddFormName.SelectedValue != FormName)
					{
						ddFormName.Visible = false;
						lblTempFormName.Visible = true;
						lblTempFormName.Text = CHelper.GetFormName(FormName);
					}
					else
					{
						ddFormName.Visible = true;
						lblTempFormName.Visible = false;
					}
				}
			}
			else
			{
				lblTempClassName.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(MetaClassName).FriendlyName);
				lblTempFormName.Text = CHelper.GetFormName(FormName);
			}

			lblTableName.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(MetaClassName).FriendlyName);
			lblFormName.Text = CHelper.GetFormName(FormName);

			if (FormDocumentData == null)
			{
				ddFormName.Enabled = false;
				tblMove.Visible = false;
				tblLinks.Visible = false;
			}
			else
			{
				ddFormName.Enabled = true;
				tblMove.Visible = true;
				tblLinks.Visible = true;

				if (MetaUIManager.MetaUITypeIsSystem(FormDocumentData.MetaClassName, FormDocumentData.MetaUITypeId))
					lblFormName.Text = CHelper.GetFormName(FormDocumentData.Name);
				else
					lblFormName.Text = String.Format("{0} ({1})", CHelper.GetFormName(FormDocumentData.Name), CHelper.GetFormName(FormDocumentData.MetaUITypeId));
			}
			#endregion
		}
		#endregion

		#region Add/Edit Section
		protected void lbAddSection_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			if (String.IsNullOrEmpty(param))
				return;

			FormDocumentData = (FormDocument)Session[param];

			BindRendererInner();
		}
		#endregion

		#region RemoveSection
		protected void lbRemoveSection_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			Guid uid = Guid.Empty;
			try
			{
				uid = new Guid(param);
			}
			catch
			{
				return;
			}
			FormController fController = new FormController(FormDocumentData);
			fController.RemoveSection(uid);

			BindRendererInner();
		}
		#endregion

		#region RemoveField
		protected void lbRemoveField_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			Guid uid = Guid.Empty;
			try
			{
				uid = new Guid(param);
			}
			catch
			{
				return;
			}
			FormController fController = new FormController(FormDocumentData);
			fController.RemoveFormItem(uid);

			BindRendererInner();
		}
		#endregion

		#region Save
		protected void lbSave_Click(object sender, EventArgs e)
		{
			FormDocumentData.Save();
			MetaClassName = FormDocumentData.MetaClassName;
			FormName = FormDocumentData.Name;

			BindDD();
		}

		protected void lbSaveClose_Click(object sender, EventArgs e)
		{
			FormDocumentData.Save();

			string retVal = "try {window.opener.location.href=window.opener.location.href;} catch (e) {;}setTimeout('window.close();', 500);";
			ClientScript.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString("N"), retVal, true);
		}

		protected void lbReCreate_Click(object sender, EventArgs e)
		{
			FormDocumentData = FormController.ReCreateFormDocument(MetaClassName, FormName);

			//ListApp Fix
			if (ListManager.MetaClassIsList(MetaClassName) && FormDocumentData.FormTable.Rows[0].Cells[0].Sections.Count > 0)
			{
				FormDocumentData.FormTable.Rows[0].Cells[0].Sections[0].BorderType = (int)BorderType.None;
				FormDocumentData.FormTable.Rows[0].Cells[0].Sections[0].ShowLabel = false;
			}

			BindRendererInner();
		}
		#endregion

		#region Change Class
		protected void ddClasses_SelectedIndexChanged(object sender, EventArgs e)
		{
			MetaClassName = ddClasses.SelectedValue;
			BindForms();

			lblTableName.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(MetaClassName).FriendlyName);
		}
		#endregion

		#region Change Form
		protected void ddFormName_SelectedIndexChanged(object sender, EventArgs e)
		{
			FormName = ddFormName.SelectedValue;
			BindRenderer();
		}
		#endregion

		//#region SaveXml
		//protected void btnSave2_Click(object sender, EventArgs e)
		//{
		//    FormDocumentData.LoadFormTableFromXml(txtXml.Text);
		//    fRenderer.FormDocumentData = FormDocumentData;
		//    fRenderer.DataBind();
		//}
		//#endregion


		#region CreateDocument
		private void CreateDocument()
		{
			FormTable tL = new FormTable();
			tL.Columns = "50%;*";
			tL.Width = "100%";
			tL.CellPadding = 5;

			FormRow row1 = new FormRow();
			FormCell cell11 = new FormCell();
			cell11.ColSpan = 2;
			cell11.Name = "cell_11";
			row1.Cells.Add(cell11);
			tL.Rows.Add(row1);

			FormRow row2 = new FormRow();
			FormCell cell21 = new FormCell();
			FormCell cell22 = new FormCell();
			cell21.ColSpan = 1;
			cell22.ColSpan = 1;
			cell21.Name = "cell_21";
			cell22.Name = "cell_22";
			row2.Cells.Add(cell21);
			row2.Cells.Add(cell22);
			tL.Rows.Add(row2);

			FormRow row3 = new FormRow();
			FormCell cell31 = new FormCell();
			cell31.ColSpan = 2;
			cell31.Name = "cell_31";
			row3.Cells.Add(cell31);
			tL.Rows.Add(row3);

			FormSection sec1 = new FormSection();
			sec1.BorderType = 1;
			sec1.ItemIndex = 1;
			sec1.ShowLabel = true;
			sec1.Uid = "dd6acdd98240403984e561399d33d9a9";
			sec1.Labels.Add(new FormLabel("Sec1", Thread.CurrentThread.CurrentUICulture.Name));
			cell11.Sections.Add(sec1);

			FormSection sec2 = new FormSection();
			sec2.BorderType = 0;
			sec2.ItemIndex = 1;
			sec2.ShowLabel = true;
			sec2.Uid = "886cb9a3aae34e68ac8ef234d0ce8ce2";
			sec2.Labels.Add(new FormLabel("Sec2", Thread.CurrentThread.CurrentUICulture.Name));
			cell21.Sections.Add(sec2);

			FormSection sec3 = new FormSection();
			sec3.BorderType = 0;
			sec3.ItemIndex = 1;
			sec3.ShowLabel = true;
			sec3.Uid = "bb6acbb98240403784e561397b33d7a7";
			sec3.Labels.Add(new FormLabel("Sec3", Thread.CurrentThread.CurrentUICulture.Name));
			cell22.Sections.Add(sec3);

			FormSection sec4 = new FormSection();
			sec4.BorderType = 0;
			sec4.ItemIndex = 1;
			sec4.ShowLabel = true;
			sec4.Uid = Guid.NewGuid().ToString("N");
			sec4.Labels.Add(new FormLabel("Sec4", Thread.CurrentThread.CurrentUICulture.Name));
			cell31.Sections.Add(sec4);

			FormControl ctrl1 = new FormControl(FormController.SmartTableLayoutType);
			ctrl1.Columns = "50%;*";
			ctrl1.Width = "100%";
			ctrl1.CellPadding = 5;

			FormItem item1 = new FormItem();
			item1.LabelWidth = "120px";
			item1.ShowLabel = true;
			item1.Labels.Add(new FormLabel("test label1:", Thread.CurrentThread.CurrentUICulture.Name));
			item1.Uid = "9b3c4642e59b405faa2a1f38559a06cc";
			item1.RowIndex = 1;
			item1.CellIndex = 1;
			item1.RowSpan = 1;
			item1.ColSpan = 2;
			FormControl item1c = new FormControl(FormController.MetaPrimitiveControlType);
			item1c.Uid = Guid.NewGuid().ToString("N");
			item1c.Source = "Title";
			item1.Control = item1c;
			ctrl1.Items.Add(item1);

			FormItem item2 = new FormItem();
			item2.LabelWidth = "120px";
			item2.ShowLabel = true;
			item2.Labels.Add(new FormLabel("test label2:", Thread.CurrentThread.CurrentUICulture.Name));
			item2.Uid = "cafa725a31b74ad6a069e3b6446d89c0";
			item2.RowIndex = 2;
			item2.CellIndex = 1;
			item2.RowSpan = 1;
			item2.ColSpan = 1;
			FormControl item2c = new FormControl(FormController.MetaPrimitiveControlType);
			item2c.Uid = Guid.NewGuid().ToString("N");
			item2c.Source = "Priority";
			item2.Control = item2c;
			ctrl1.Items.Add(item2);

			FormItem item3 = new FormItem();
			item3.LabelWidth = "120px";
			item3.ShowLabel = true;
			item3.Labels.Add(new FormLabel("test label3:", Thread.CurrentThread.CurrentUICulture.Name));
			item3.Uid = "8d36be893c3f4cf1b5295be6853eb246";
			item3.RowIndex = 3;
			item3.CellIndex = 1;
			item3.RowSpan = 1;
			item3.ColSpan = 1;
			FormControl item3c = new FormControl(FormController.MetaPrimitiveControlType);
			item3c.Uid = Guid.NewGuid().ToString("N");
			item3c.Source = "Created";
			item3.Control = item3c;
			ctrl1.Items.Add(item3);

			FormItem item4 = new FormItem();
			item4.LabelWidth = "120px";
			item4.ShowLabel = true;
			item4.Labels.Add(new FormLabel("test label4:", Thread.CurrentThread.CurrentUICulture.Name));
			item4.Uid = "a5d87024cbf849cea5d273d77a292e6f";
			item4.RowIndex = 2;
			item4.CellIndex = 2;
			item4.RowSpan = 2;
			item4.ColSpan = 1;
			FormControl item4c = new FormControl(FormController.MetaPrimitiveControlType);
			item4c.Uid = Guid.NewGuid().ToString("N");
			item4c.Source = "Description";
			item4.Control = item4c;
			ctrl1.Items.Add(item4);

			sec1.Control = ctrl1;

			//FormDocument fd = new FormDocument();
			FormDocument fd = FormDocument.Load("Task", "[MC_BaseForm]");
			fd.MetaClassName = "Task";
			fd.Name = "[MC_BaseForm]";
			fd.FormTable = tL;
			fd.Save();
		}
		#endregion
	}
}
