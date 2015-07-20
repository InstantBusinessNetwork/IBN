using System;
using System.Data;
using System.Collections;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core.Layout;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Core;

namespace Mediachase.Ibn.Web.UI.MetaUI
{
	public partial class FormItemEdit : System.Web.UI.UserControl
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

		#region add
		private bool _add
		{
			get
			{
				if (Request["add"] != null && Request["add"] == "1")
					return true;
				return false;
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

		#region FormItemData
		private FormItem FormItemData
		{
			get
			{
				if (ViewState[this.ID + "_FormItemData"] == null)
					return null;
				return (FormItem)ViewState[this.ID + "_FormItemData"];
			}
			set
			{
				ViewState[this.ID + "_FormItemData"] = value;
			}
		}
		#endregion

		#region FormSectionData
		private FormSection FormSectionData
		{
			get
			{
				if (ViewState[this.ID + "_FormSectionData"] == null)
					return null;
				return (FormSection)ViewState[this.ID + "_FormSectionData"];
			}
			set
			{
				ViewState[this.ID + "_FormSectionData"] = value;
			}
		}
		#endregion

		#region propertyPath
		private string _propertyPath
		{
			get
			{
				if (ViewState[this.ID + "__propertyPath"] == null)
					return String.Empty;
				return (string)ViewState[this.ID + "__propertyPath"];
			}
			set
			{
				ViewState[this.ID + "__propertyPath"] = value;
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

				int rowSpan = 3;

				if (!String.IsNullOrEmpty(itemUid) && itemUid != "0")
				{
					FormController fc = new FormController(FormDocumentData);
					FormItemData = fc.GetSTLItemByUid(new Guid(itemUid));
					if (_add) // create
					{
						if (FormItemData != null)
							FormSectionData = fc.GetSectionForFormItem(FormItemData);
						else
							FormSectionData = fc.GetSectionByUid(new Guid(itemUid));
						if (FormSectionData == null)
							throw new Exception("FormSection is not defined.");

						#region Create
						lblTitle.Text = GetGlobalResourceObject("IbnFramework.MetaForm", "AddField").ToString();
						lblComments.Text = GetGlobalResourceObject("IbnFramework.MetaForm", "AddFieldComment").ToString();
						txtTitle.Text = "";
						txtLabelWidth.Text = "120";
						TabIndexText.Text = "0";

						rbDefault.Checked = true;
						this.Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString("N"),
							"ModifyTxt(0);", true);

						lblControl.Visible = false;
						ddControl.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.MetaForm", FormController.MetaPrimitiveControlType).ToString(), FormController.MetaPrimitiveControlType));

						BindPropertiesControl(ddControl.SelectedValue);
						#endregion
					}
					else	//edit
					{
						if (FormItemData != null)
						{
							#region Edit
							lblTitle.Text = GetGlobalResourceObject("IbnFramework.MetaForm", "EditField").ToString();
							lblComments.Text = GetGlobalResourceObject("IbnFramework.MetaForm", "EditFieldComment").ToString();

							rowSpan = fc.GetMaxRowSpan(FormItemData);

							ddControl.Visible = false;
							#endregion
						}
						else
							throw new Exception("FormItem is not defined.");
					}
				}
				else
					throw new Exception("There is no selected item on the form!");

				for (int i = 1; i <= rowSpan; i++)
					ddRows.Items.Add(new ListItem(i.ToString(), i.ToString()));

				if (!_add && FormItemData != null)
					BindValues();
			}
			btnSave.InnerText = GetGlobalResourceObject("IbnFramework.MetaForm", "Save").ToString();
			btnCancel.InnerText = GetGlobalResourceObject("IbnFramework.MetaForm", "Cancel").ToString();
			rbNone.Text = GetGlobalResourceObject("IbnFramework.MetaForm", "HideLabel").ToString();
			rbDefault.Text = GetGlobalResourceObject("IbnFramework.MetaForm", "SystemLabel").ToString();
			rbCustom.Text = GetGlobalResourceObject("IbnFramework.MetaForm", "CustomLabel").ToString();
			//TEMP/
			divSelector.Visible = false;
		}

		#region BindValues
		private void BindValues()
		{
			FormLabel lbl = null;
			foreach (FormLabel temp in FormItemData.Labels)
				if (temp.Code.ToLower().Equals(Thread.CurrentThread.CurrentUICulture.Name.ToLower()))
					lbl = temp;

			if (lbl != null)
			{
				if (lbl.Title == "[MC_DefaultLabel]" || !FormItemData.ShowLabel)
				{
					#region Define Classes & Fields
					string ownName = FormItemData.Control.Source;
					string aggrName = String.Empty;
					MetaField fieldOwn = null;
					MetaClass classOwn = MetaDataWrapper.GetMetaClassByName(FormDocumentData.MetaClassName);
					MetaField fieldAggr = null;
					MetaClass classAggr = null;
					if (ownName.Contains("."))	//aggregation
					{
						string[] mas = ownName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
						ownName = mas[0];
						aggrName = mas[1];
						if (!classOwn.Fields.Contains(ownName))
							classOwn = classOwn.CardOwner;
						fieldOwn = classOwn.Fields[ownName];
						if (fieldOwn.IsAggregation)
						{
							string aggrClassName = fieldOwn.Attributes[McDataTypeAttribute.AggregationMetaClassName].ToString();
							classAggr = MetaDataWrapper.GetMetaClassByName(aggrClassName);
							if (!classAggr.Fields.Contains(aggrName))
								classAggr = classAggr.CardOwner;
							fieldAggr = classAggr.Fields[aggrName];
						}
					}
					else
					{
						if (!classOwn.Fields.Contains(ownName))
							classOwn = classOwn.CardOwner;
						fieldOwn = classOwn.Fields[ownName];
					}
					#endregion

					string text = String.Empty;
					if (fieldAggr != null)
						text = String.Format("{0} - {1}",
												CHelper.GetResFileString(fieldOwn.FriendlyName),
												CHelper.GetResFileString(fieldAggr.FriendlyName)
											);
					else
						text = CHelper.GetResFileString(fieldOwn.FriendlyName);
					
					txtTitle.Text = CHelper.GetResFileString(text) + ":";
				}
				else
					txtTitle.Text = lbl.Title;

				if (lbl.Title == "[MC_DefaultLabel]")
					rbDefault.Checked = true;
			}
			if (!rbDefault.Checked)
			{
				if (FormItemData.ShowLabel)
					rbCustom.Checked = true;
				else
					rbNone.Checked = true;
			}
			this.Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("ModifyTxt({0});", rbCustom.Checked ? "1" : "0"), true);

			txtLabelWidth.Text = Unit.Parse(FormItemData.LabelWidth).Value.ToString();
			TabIndexText.Text = FormItemData.TabIndex.ToString();

			ddRows.SelectedValue = FormItemData.RowSpan.ToString();
			switch (FormItemData.ColSpan)
			{
				case 1:
					rb1.Checked = true;
					break;
				case 2:
					rb2.Checked = true;
					break;
				default:
					rb1.Disabled = true;
					rb2.Disabled = true;

					rb1.Checked = false;
					break;
			}
			FormController fc = new FormController(FormDocumentData);
			if (!fc.CanChangeColspan(FormItemData))
			{
				rb1.Disabled = true;
				rb2.Disabled = true;
			}

			lblControl.Text = String.Format("&lt;{0}&gt;", GetGlobalResourceObject("IbnFramework.MetaForm", "NoControl").ToString());
			if (FormItemData.Control != null && !String.IsNullOrEmpty(FormItemData.Control.Type))
				lblControl.Text = CHelper.GetResFileString(String.Format("{{IbnFramework.MetaForm:{0}}}", FormItemData.Control.Type));

			BindPropertiesControl(FormItemData.Control == null ? "" : FormItemData.Control.Type);
		}
		#endregion

		#region BindPropertiesControl
		private void BindPropertiesControl(string type)
		{
			Control control = null;
			switch (type)
			{
				case FormController.MetaPrimitiveControlType:
					_propertyPath = "~/Apps/MetaUI/Modules/MetaFormControls/SmartTableLayoutItemProperties.ascx";
					control = this.Page.LoadControl(_propertyPath);
					control.ID = "propCtrl";
					phProperties.Controls.Clear();
					phProperties.Controls.Add(control);
					SmartTableLayoutItemProperties sp = (SmartTableLayoutItemProperties)control;
					if (!_add && FormItemData != null && FormItemData.Control != null)
					{
						#region Define Classes & Fields
						string ownName = FormItemData.Control.Source;
						string aggrName = String.Empty;
						MetaField fieldOwn = null;
						MetaClass classOwn = MetaDataWrapper.GetMetaClassByName(FormDocumentData.MetaClassName);
						MetaField fieldAggr = null;
						MetaClass classAggr = null;
						if (ownName.Contains("."))	//aggregation
						{
							string[] mas = ownName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
							ownName = mas[0];
							aggrName = mas[1];
							if (!classOwn.Fields.Contains(ownName))
								classOwn = classOwn.CardOwner;
							fieldOwn = classOwn.Fields[ownName];
							if (fieldOwn.IsAggregation)
							{
								string aggrClassName = fieldOwn.Attributes[McDataTypeAttribute.AggregationMetaClassName].ToString();
								classAggr = MetaDataWrapper.GetMetaClassByName(aggrClassName);
								if (!classAggr.Fields.Contains(aggrName))
									classAggr = classAggr.CardOwner;
								fieldAggr = classAggr.Fields[aggrName];
							}
						}
						else
						{
							if (!classOwn.Fields.Contains(ownName))
								classOwn = classOwn.CardOwner;
							fieldOwn = classOwn.Fields[ownName];
						}
						#endregion

						string text = String.Empty;
						if (fieldAggr != null)
							text = String.Format("{0} - {1}",
													CHelper.GetResFileString(fieldOwn.FriendlyName),
													CHelper.GetResFileString(fieldAggr.FriendlyName)
												);
						else
							text = CHelper.GetResFileString(fieldOwn.FriendlyName);
						sp.Source = text;
						sp.ReadOnly = FormItemData.Control.ReadOnly;
					}
					if (_add && FormSectionData != null)
						sp.MetaClassName = FormDocumentData.MetaClassName;
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
			if (!_add && FormItemData != null) //edit
			{
				FormItemData = fc.GetSTLItemByUid(new Guid(itemUid));

				FormItemData.ShowLabel = !rbNone.Checked;

				FormLabel lbl = null;
				foreach (FormLabel temp in FormItemData.Labels)
				{
					if (temp.Code.ToLower().Equals(Thread.CurrentThread.CurrentUICulture.Name.ToLower()))
						lbl = temp;
				}
				if (lbl == null)
				{
					lbl = new FormLabel();
					lbl.Code = Thread.CurrentThread.CurrentUICulture.Name.ToLower();
					FormItemData.Labels.Add(lbl);
				}
				if (rbDefault.Checked)
					lbl.Title = FormController.DefaultLabelValue;
				else
					lbl.Title = txtTitle.Text;

				if (!String.IsNullOrEmpty(txtLabelWidth.Text))
					FormItemData.LabelWidth = Unit.Pixel(int.Parse(txtLabelWidth.Text)).ToString();

				short tabIndex = 0;
				if (!String.IsNullOrEmpty(TabIndexText.Text.Trim()))
					tabIndex = short.Parse(TabIndexText.Text.Trim());
				FormItemData.TabIndex = tabIndex;

				FormItemData.RowSpan = int.Parse(ddRows.SelectedValue);
				if (!rb1.Disabled && rb1.Checked)
					FormItemData.ColSpan = 1;
				if (!rb2.Disabled && rb2.Checked)
					FormItemData.ColSpan = 2;

				foreach (Control c in phProperties.Controls)
				{
					if (c is SmartTableLayoutItemProperties)
					{
						SmartTableLayoutItemProperties sp = (SmartTableLayoutItemProperties)c;
						//FormItemData.Control.Source = sp.Source;
						FormItemData.Control.ReadOnly = sp.ReadOnly;
					}
				}
			}
			else if (_add && FormSectionData != null)
			{
				FormSectionData = fc.GetSectionByUid(new Guid(FormSectionData.Uid));

				if (ddControl.SelectedValue.Equals(FormController.MetaPrimitiveControlType))
				{
					string source = String.Empty;
					bool readOnly = false;
					foreach (Control c in phProperties.Controls)
					{
						if (c is SmartTableLayoutItemProperties)
						{
							SmartTableLayoutItemProperties sp = (SmartTableLayoutItemProperties)c;
							source = sp.Source;
							readOnly = sp.ReadOnly;
						}
					}
					int colSpan = 1;
					if (!rb1.Disabled && rb1.Checked)
						colSpan = 1;
					if (!rb2.Disabled && rb2.Checked)
						colSpan = 2;

					string label = String.Empty;
					if (rbDefault.Checked)
						label = FormController.DefaultLabelValue;
					else if (rbCustom.Checked)
						label = txtTitle.Text;

					short tabIndex = 0;
					if (!String.IsNullOrEmpty(TabIndexText.Text.Trim()))
						tabIndex = short.Parse(TabIndexText.Text.Trim());
					
					FormItem newItem = FormController.CreateFormItemPrimitive(-1, -1,
						int.Parse(ddRows.SelectedValue), colSpan, !rbNone.Checked,
						label, Unit.Pixel(int.Parse(txtLabelWidth.Text)), tabIndex, source, readOnly);

					fc.AddFormItem(newItem, new Guid(FormSectionData.Uid));
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