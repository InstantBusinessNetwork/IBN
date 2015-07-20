using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Core.Business;
using Mediachase.IBN.Business.Documents;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.DocumentManagement.Modules
{
	public partial class VersionEdit : System.Web.UI.UserControl
	{
		#region FormName
		protected string FormName
		{
			get
			{
				if (ViewState["_formName"] != null)
					return ViewState["_formName"].ToString();
				else
					return FormController.BaseFormType;
			}
			set
			{
				ViewState["_formName"] = value;
			}
		}
		#endregion

		#region DocumentId
		protected PrimaryKeyId DocumentId
		{
			get
			{
				if (Request["DocumentId"] == null)
					return PrimaryKeyId.Empty;
				PrimaryKeyId value;
				string s = MetaViewGroupUtil.GetIdFromUniqueKey(Request["DocumentId"]);
				if (PrimaryKeyId.TryParse(s, out value))
					return value;
				else
					return PrimaryKeyId.Empty;
			}
		}
		#endregion

		#region ObjectId
		protected PrimaryKeyId ObjectId
		{
			get
			{
				if (Request["ObjectId"] == null)
					return PrimaryKeyId.Empty;
				PrimaryKeyId value;
				string s = MetaViewGroupUtil.GetIdFromUniqueKey(Request["ObjectId"]);
				if (PrimaryKeyId.TryParse(s, out value))
					return value;
				else
					return PrimaryKeyId.Empty;
			}
		}
		#endregion

		private object _bindObject = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (ObjectId != PrimaryKeyId.Empty)
				CHelper.AddToContext("ObjectId", ObjectId);

			if (!Page.IsPostBack)
			{
				if (ObjectId == PrimaryKeyId.Empty)
					FormName = FormController.CreateFormType;
				else
					FormName = FormController.BaseFormType;

				if (_bindObject == null)
				{
					if (ObjectId != PrimaryKeyId.Empty)
						_bindObject = BusinessManager.Load(DocumentContentVersionEntity.GetAssignedMetaClassName(), 
							ObjectId);
					else
						_bindObject = BusinessManager.InitializeEntity(DocumentContentVersionEntity.GetAssignedMetaClassName());
				}

				formView.FormName = FormName;
				formView.DataItem = _bindObject;
				formView.DataBind();
			}

			btnSave.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Save").ToString();
			btnCancel.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Cancel").ToString();
			btnCancel.OnClientClick = Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true);
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (CHelper.NeedToDataBind())
			{
				if (_bindObject == null)
				{
					if (ObjectId != PrimaryKeyId.Empty)
						_bindObject = BusinessManager.Load(DocumentContentVersionEntity.GetAssignedMetaClassName(),
							ObjectId);
					else
						_bindObject = BusinessManager.InitializeEntity(DocumentContentVersionEntity.GetAssignedMetaClassName());
				}

				formView.FormName = FormName;
				formView.DataItem = _bindObject;
				formView.DataBind();
			}
		}
		#endregion

		#region Save Method
		protected void btnSave_Click(object sender, EventArgs e)
		{
			this.Page.Validate();
			if (!this.Page.IsValid)
				return;

			if (ObjectId != PrimaryKeyId.Empty)
				_bindObject = BusinessManager.Load(DocumentContentVersionEntity.GetAssignedMetaClassName(),
					ObjectId);
			else
				_bindObject = BusinessManager.InitializeEntity(DocumentContentVersionEntity.GetAssignedMetaClassName());

			if (_bindObject != null)
			{
				ProcessCollection(this.Page.Controls, (EntityObject)_bindObject);

				PrimaryKeyId objectId = ObjectId;

				if (ObjectId != PrimaryKeyId.Empty)
					BusinessManager.Update((EntityObject)_bindObject);
				else
				{	
					((DocumentContentVersionEntity)_bindObject).OwnerDocumentId = DocumentId;
					((DocumentContentVersionEntity)_bindObject).Name = ((DocumentContentVersionEntity)_bindObject).File.Name;
					objectId = BusinessManager.Create((EntityObject)_bindObject);
				}

				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, String.Empty);			
			}
		}

		private void ProcessCollection(ControlCollection _coll, EntityObject _obj)
		{
			foreach (Control c in _coll)
			{
				ProcessControl(c, _obj);
				if (c.Controls.Count > 0)
					ProcessCollection(c.Controls, _obj);
			}
		}

		private void ProcessControl(Control c, EntityObject _obj)
		{
			IEditControl editControl = c as IEditControl;
			if (editControl != null)
			{
				string fieldName = editControl.FieldName;

				#region MyRegion
				string ownFieldName = fieldName;
				string aggrFieldName = String.Empty;
				string aggrClassName = String.Empty;
				MetaField ownField = null;
				MetaField aggrField = null;
				MetaClass ownClass = MetaDataWrapper.GetMetaClassByName(_obj.MetaClassName);
				if (ownFieldName.Contains("."))
				{
					string[] mas = ownFieldName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
					if (mas.Length > 1)
					{
						ownFieldName = mas[0];
						aggrFieldName = mas[1];

						ownField = MetaDataWrapper.GetMetaFieldByName(ownClass, ownFieldName);
						aggrClassName = ownField.Attributes.GetValue<string>(McDataTypeAttribute.AggregationMetaClassName);
						aggrField = MetaDataWrapper.GetMetaFieldByName(aggrClassName, aggrFieldName);
					}
				}
				if (ownField == null)
				{
					ownField = ownClass.Fields[ownFieldName];
					if (ownField == null)
						ownField = ownClass.CardOwner.Fields[ownFieldName];
				}
				#endregion

				object eValue = editControl.Value;

				bool makeChange = true;

				MetaField field = (aggrField == null) ? ownField : aggrField;
				if (!field.IsNullable && eValue == null)
					makeChange = false;

				if (makeChange)
				{
					if (aggrField == null)
						_obj[ownFieldName] = eValue;
					else
					{
						EntityObject aggrObj = null;
						if (_obj[ownFieldName] != null)
							aggrObj = (EntityObject)_obj[ownFieldName];
						else
							aggrObj = BusinessManager.InitializeEntity(DocumentContentVersionEntity.GetAssignedMetaClassName());
						aggrObj[aggrFieldName] = eValue;
					}
				}
			}
		}
		#endregion
	}
}