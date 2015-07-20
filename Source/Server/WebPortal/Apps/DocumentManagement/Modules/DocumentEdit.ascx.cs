using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core;
using Mediachase.IBN.Business.Documents;
using System.Globalization;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.DocumentManagement.Modules
{
	public partial class DocumentEdit : System.Web.UI.UserControl
	{
		#region ClassName
		protected string ClassName
		{
			get
			{
				if (ViewState["_className"] != null)
					return ViewState["_className"].ToString();
				else
					return String.Empty;
			}
			set
			{
				ViewState["_className"] = value;
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

		#region Back
		protected string Back
		{
			get
			{
				if (Request["Back"] != null)
					return Request["Back"].ToLower(CultureInfo.InvariantCulture);
				else
					return String.Empty;
			}
		}
		#endregion

		private object _bindObject = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (ObjectId != PrimaryKeyId.Empty)
			{
				CHelper.AddToContext("ObjectId", ObjectId);
				trDocType.Visible = false;
			}

			if (!Page.IsPostBack)
			{
				if (ObjectId == PrimaryKeyId.Empty)
				{
					BindCards();
					ClassName = ddType.SelectedValue;
				}
				else
					ClassName = DocumentEntity.GetAssignedMetaClassName();

				CHelper.AddToContext("ClassName", ClassName);
				
				if (_bindObject == null)
				{
					if (ObjectId != PrimaryKeyId.Empty)
						_bindObject = BusinessManager.Load(ClassName, ObjectId);
					else
						_bindObject = BusinessManager.InitializeEntity(ClassName);
				}

				formView.DataItem = _bindObject;
				formView.DataBind();
			}
			else
				CHelper.AddToContext("ClassName", ClassName);

			this.Page.Title = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(ClassName).FriendlyName);
		}

		#region BindCards
		private void BindCards()
		{
			ddType.Items.Clear();
			EntityObject[] mas = BusinessManager.List(DocumentTypeEntity.GetAssignedMetaClassName(), new FilterElementCollection().ToArray());
			foreach (EntityObject eo in mas)
			{
				MetaClass mc = MetaDataWrapper.GetMetaClassByName(eo.Properties["Name"].Value.ToString());
				ddType.Items.Add(new ListItem(CHelper.GetResFileString(mc.FriendlyName), eo.Properties["Name"].Value.ToString()));
			}

			BindTemplates();
		} 
		#endregion

		#region BindTemplates
		private void BindTemplates()
		{
			ddTemplate.Items.Clear();
			ddTemplate.Items.Add(new ListItem(String.Empty, PrimaryKeyId.Empty.ToString()));

			FilterElement fe = FilterElement.EqualElement("Name", ddType.SelectedValue);
			FilterElementCollection fec = new FilterElementCollection();
			fec.Add(fe);

			EntityObject[] masTypes = BusinessManager.List(DocumentTypeEntity.GetAssignedMetaClassName(), fec.ToArray());
			if (masTypes.Length > 0)
			{
				FilterElement fe1 = FilterElement.EqualElement("DocumentTypeId", masTypes[0].PrimaryKeyId.Value);
				FilterElementCollection fec1 = new FilterElementCollection();
				fec1.Add(fe1);
				EntityObject[] mas = BusinessManager.List(DocumentTemplateEntity.GetAssignedMetaClassName(), fec1.ToArray());
				foreach (EntityObject eo in mas)
				{
					ddTemplate.Items.Add(new ListItem(CHelper.GetResFileString(eo["Name"].ToString()), eo.PrimaryKeyId.Value.ToString()));
				}
			}
		}
		#endregion

		#region ddType_SelectedIndexChanged
		protected void ddType_SelectedIndexChanged(object sender, EventArgs e)
		{
			ClassName = ddType.SelectedValue;
			CHelper.AddToContext("ClassName", ClassName);

			BindTemplates();

			_bindObject = BusinessManager.InitializeEntity(ClassName);
			formView.DataItem = _bindObject;
			formView.DataBind();
		} 
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			BindToolbar();

			if (CHelper.NeedToDataBind())
			{
				if (_bindObject == null)
				{
					if (ObjectId != PrimaryKeyId.Empty)
						_bindObject = BusinessManager.Load(ClassName, ObjectId);
					else
						_bindObject = BusinessManager.InitializeEntity(ClassName);
				}

				formView.DataItem = _bindObject;
				formView.DataBind();
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			MainBlockHeader.Title = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(ClassName).FriendlyName);

			MainBlockHeader.ClearLinks();

			//if (!String.IsNullOrEmpty(this.Page.Request["ViewName"]))
			//    MainBlockHeader.AddLink("Back To List", CHelper.GetLinkMetaViewShow(Request["ViewName"]));

			//SecurityService ss = ((BusinessObject)_bindObject).GetService<SecurityService>();
			//if (!(ss != null && !ss.CheckUserRight("Write")))
			//{
			MainBlockHeader.AddLink(
				"~/Images/IbnFramework/saveitem.gif",
				CHelper.GetResFileString("{IbnFramework.Global:_mc_Save}"),
				Page.ClientScript.GetPostBackClientHyperlink(btnSave, "", true)
				);

			string backLink;
			if (Back == "view" && ObjectId != PrimaryKeyId.Empty)
				backLink = CHelper.GetLinkEntityView(ClassName, ObjectId.ToString());
			else
				backLink = CHelper.GetLinkEntityList(ClassName);
			MainBlockHeader.AddLink(
				"~/Images/IbnFramework/cancel.gif",
				CHelper.GetResFileString("{IbnFramework.Global:_mc_Cancel}"),
				backLink);
			//}

			//if (Mode.ToLower() == "popup")
			//    MainBlockHeader.AddLink("~/Images/IbnFramework/close.gif", GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Close").ToString(), "javascript:window.close();");
		}
		#endregion

		#region Save Method
		protected void btnSave_Click(object sender, EventArgs e)
		{
			this.Page.Validate();
			if (!this.Page.IsValid)
				return;

			if (ObjectId != PrimaryKeyId.Empty)
				_bindObject = BusinessManager.Load(ClassName, ObjectId);
			else
				_bindObject = BusinessManager.InitializeEntity(ClassName);

			if (_bindObject != null)
			{
				ProcessCollection(this.Page.Controls, (EntityObject)_bindObject);

				PrimaryKeyId objectId = ObjectId;

				if (ObjectId != PrimaryKeyId.Empty)
					BusinessManager.Update((EntityObject)_bindObject);
				else
				{
					CreateRequest request = new CreateRequest((EntityObject)_bindObject);
					if (PrimaryKeyId.Parse(ddTemplate.SelectedValue) != PrimaryKeyId.Empty)
						request.Parameters.Add(DocumentRequestParameters.Create_DocumentTemplatedId, PrimaryKeyId.Parse(ddTemplate.SelectedValue));
					CreateResponse response = (CreateResponse)BusinessManager.Execute(request);
					objectId = response.PrimaryKeyId;
				}

				Response.Redirect(String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName={0}&ObjectId={1}", DocumentEntity.GetAssignedMetaClassName(), objectId), true);
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
					if(ownField == null)
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
							aggrObj = BusinessManager.InitializeEntity(ClassName);
						aggrObj[aggrFieldName] = eValue;
					}
				}
			}
		}
		#endregion
	}
}