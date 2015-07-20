using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Threading;

using Mediachase.Ibn.Core.Layout;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaUI
{
	public partial class FormSectionEdit : System.Web.UI.UserControl
	{
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

		#region uid
		protected string uid
		{
			get
			{
				if (!String.IsNullOrEmpty(Request["uid"]))
					return Request["uid"];
				else
					return "";
			}
		}
		#endregion

		#region itemUid
		protected string itemUid
		{
			get
			{
				if (!String.IsNullOrEmpty(Request["itemUid"]))
					return Request["itemUid"];
				else
					return "";
			}
		}
		#endregion

		#region FormDocumentData
		private FormDocument FormDocumentData
		{
			get
			{
				if (ViewState["_FormDocumentData"] == null)
					return null;
				return (FormDocument)ViewState["_FormDocumentData"];
			}
			set
			{
				ViewState["_FormDocumentData"] = value;
			}
		}
		#endregion

		#region FormSectionData
		private FormSection FormSectionData
		{
			get
			{
				if (ViewState["_FormSectionData"] == null)
					return null;
				return (FormSection)ViewState["_FormSectionData"];
			}
			set
			{
				ViewState["_FormSectionData"] = value;
			}
		}
		#endregion

		#region propertyPath
		private string _propertyPath
		{
			get
			{
				if (ViewState["__propertyPath"] == null)
					return String.Empty;
				return (string)ViewState["__propertyPath"];
			}
			set
			{
				ViewState["__propertyPath"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(_propertyPath))
			{
				Control control = this.Page.LoadControl(_propertyPath);
				control.ID = "propCtrl";
				phProperties.Controls.Add(control);
			}
			if (!Page.IsPostBack)
			{
				if (FormDocumentData == null)
					FormDocumentData = ((FormDocument)Session[uid]).Copy();
				if (FormDocumentData == null)
					throw new Exception("FormDocument is undefined!");

				if (!String.IsNullOrEmpty(itemUid) && itemUid != "0")
				{
					#region Edit
					lblTitle.Text = GetGlobalResourceObject("IbnFramework.MetaForm", "EditSection").ToString();
					lblComments.Text = GetGlobalResourceObject("IbnFramework.MetaForm", "EditSectionComment").ToString();

					FormController fc = new FormController(FormDocumentData);
					FormSectionData = fc.GetSectionByUid(new Guid(itemUid));
					if (FormSectionData == null)
						throw new Exception("FormSection is undefined!");
					ddControl.Visible = false;
					#endregion
				}
				else
				{
					#region Create
					lblTitle.Text = GetGlobalResourceObject("IbnFramework.MetaForm", "AddSection").ToString();
					lblComments.Text = GetGlobalResourceObject("IbnFramework.MetaForm", "AddSectionComment").ToString();
					txtTitle.Text = GetGlobalResourceObject("IbnFramework.MetaForm", "Section1").ToString();
					cbShowBorder.Checked = true;
					cbShowTitle.Checked = true;

					lblControl.Visible = false;
					ddControl.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.MetaForm", FormController.SmartTableLayoutType).ToString(), FormController.SmartTableLayoutType));

					BindPropertiesControl(ddControl.SelectedValue);
					#endregion
				}

				if (FormSectionData != null)
					BindValues();

				cbShowBorder.Text = " " + GetGlobalResourceObject("IbnFramework.MetaForm", "ShowBorderSection").ToString();
				cbShowTitle.Text = " " + GetGlobalResourceObject("IbnFramework.MetaForm", "ShowNameSection").ToString();
			}

			btnSave.InnerText = GetGlobalResourceObject("IbnFramework.MetaForm", "Save").ToString();
			btnCancel.InnerText = GetGlobalResourceObject("IbnFramework.MetaForm", "Cancel").ToString();

			divControl.Visible = false;
		}

		#region BindValues
		private void BindValues()
		{
			FormLabel lbl = null;
			foreach (FormLabel temp in FormSectionData.Labels)
				if (temp.Code.ToLower().Equals(Thread.CurrentThread.CurrentUICulture.Name.ToLower()))
					lbl = temp;
			if (lbl != null)
				txtTitle.Text = lbl.Title;
			cbShowBorder.Checked = (FormSectionData.BorderType > 0);
			cbShowTitle.Checked = FormSectionData.ShowLabel;

			lblControl.Text = String.Format("&lt;{0}&gt;", GetGlobalResourceObject("IbnFramework.MetaForm", "NoControl").ToString());
			if (FormSectionData.Control != null && !String.IsNullOrEmpty(FormSectionData.Control.Type))
				lblControl.Text = CHelper.GetResFileString(String.Format("{{IbnFramework.MetaForm:{0}}}", FormSectionData.Control.Type));

			BindPropertiesControl(FormSectionData.Control == null ? "" : FormSectionData.Control.Type);
		}
		#endregion

		#region BindPropertiesControl
		private void BindPropertiesControl(string type)
		{
			Control control = null;
			switch (type)
			{
				case FormController.SmartTableLayoutType:
					_propertyPath = "~/Apps/MetaUI/Modules/MetaFormControls/SmartTableLayoutProperties.ascx";
					control = this.Page.LoadControl(_propertyPath);
					control.ID = "propCtrl";
					phProperties.Controls.Clear();
					phProperties.Controls.Add(control);
					SmartTableLayoutProperties sp = (SmartTableLayoutProperties)control;
					if (FormSectionData != null && FormSectionData.Control != null)
					{
						sp.Columns = FormSectionData.Control.Columns;
						sp.CellPadding = FormSectionData.Control.CellPadding;
					}
					break;
				default:
					//CUSTOM
					//FormSectionData.Control.Property
					//FormControlProperty fcp = new FormControlProperty();
					//fcp.Data = string
					break;
			}
			if (control == null)
				divProperties.Visible = false;
		}
		#endregion

		protected void ddControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindPropertiesControl(ddControl.SelectedValue);
		}

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			FormController fc = new FormController(FormDocumentData);
			if (!String.IsNullOrEmpty(itemUid)) //edit
			{
				FormSectionData = fc.GetSectionByUid(new Guid(itemUid));

				FormSectionData.BorderType = (cbShowBorder.Checked ? 1 : 0);
				FormSectionData.ShowLabel = cbShowTitle.Checked;
				FormLabel lbl = null;
				foreach (FormLabel temp in FormSectionData.Labels)
				{
					if (temp.Code.ToLower().Equals(Thread.CurrentThread.CurrentUICulture.Name.ToLower()))
						lbl = temp;
				}
				if (lbl == null)
				{
					lbl = new FormLabel();
					lbl.Code = Thread.CurrentThread.CurrentUICulture.Name.ToLower();
					FormSectionData.Labels.Add(lbl);
				}
				lbl.Title = txtTitle.Text;
				foreach (Control c in phProperties.Controls)
				{
					if (c is SmartTableLayoutProperties)
					{
						SmartTableLayoutProperties sp = (SmartTableLayoutProperties)c;
						FormSectionData.Control.CellPadding = sp.CellPadding;
						if (!String.IsNullOrEmpty(sp.Columns))
							FormSectionData.Control.Columns = sp.Columns;
					}
				}
			}
			else //create
			{
				if (ddControl.SelectedValue.Equals(FormController.SmartTableLayoutType))
				{
					int cellPadding = 5;
					string columns = "50%;*";
					foreach (Control c in phProperties.Controls)
					{
						if (c is SmartTableLayoutProperties)
						{
							SmartTableLayoutProperties sp = (SmartTableLayoutProperties)c;
							cellPadding = sp.CellPadding;
							if (!String.IsNullOrEmpty(sp.Columns))
								columns = sp.Columns;
						}
					}
					BorderType bType = (cbShowBorder.Checked ? BorderType.TemplateBorder : BorderType.None);
					FormSection newSection = FormController.CreateSectionSTL(bType, cbShowBorder.Checked, txtTitle.Text, Unit.Percentage(100), columns, cellPadding);
					fc.AddSection(newSection);
				}
			}

			string newUid = Guid.NewGuid().ToString("N");
			Session[newUid] = FormDocumentData;
			CloseAndRefresh(newUid);
		}
		#endregion

		#region CloseAndRefresh
		private void CloseAndRefresh(string formId)
		{
			// OR: Если передали RefreshButton, то обновляем родительское окно через её нажатие
			if (RefreshButton != String.Empty)
			{
				string script = String.Format("try{{window.opener.__doPostBack('{0}', '{1}')}}catch(e){{;}}window.close();", RefreshButton, formId);
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script, true);
			}
			else
			{
				string backRefresh = ";";
				if (!String.IsNullOrEmpty(Request["backRefresh"]))
					backRefresh = "window.opener." + Request["backRefresh"] + "(" + formId + ");";
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "try{" + backRefresh + "}catch(e){;}window.close();", true);
			}
		}
		#endregion
	}
}