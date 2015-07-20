using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Events;

namespace Mediachase.Ibn.Web.UI.Calendar.Modules
{
	public partial class EventEdit : System.Web.UI.UserControl
	{
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
				{
					EntityObject eo = BusinessManager.Load(CalendarEventEntity.ClassName, value);
					if (eo != null)
					{
						CalendarEventEntity ceo = eo as CalendarEventEntity;
						if (ceo.CalendarEventExceptionId.HasValue)
							return value;
					}
					value = ((VirtualEventId)value).RealEventId;
					return value;
				}
				else
					return PrimaryKeyId.Empty;
			}
		}
		#endregion

		#region Mode
		protected string Mode
		{
			get
			{
				if (Request["Mode"] != null)
					return Request["Mode"];
				else
					return String.Empty;
			}
		}
		#endregion

		#region CommandName
		protected string CommandName
		{
			get
			{
				if (Request["CommandName"] != null)
					return Request["CommandName"];
				else
					return String.Empty;
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
			CHelper.AddToContext("ClassName", CalendarEventEntity.ClassName);
			if (ObjectId != PrimaryKeyId.Empty)
				CHelper.AddToContext("ObjectId", ObjectId);

			this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
			xmlStruct.InnerDataBind += new XmlFormBuilder.InnerDataBindEventHandler(xmlStruct_InnerDataBind);

			if (!Page.IsPostBack)
			{
				xmlStruct.ClassName = CalendarEventEntity.ClassName;
				xmlStruct.LayoutType = LayoutType.ObjectEdit;

				xmlStruct.DataBind();
			}

			this.Page.Title = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(CalendarEventEntity.ClassName).FriendlyName);
		}

		#region xmlStruct_InnerDataBind
		void xmlStruct_InnerDataBind(object sender, EventArgs e)
		{
			MakeDataBind(this);
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (CHelper.NeedToDataBind())
			{
				xmlStruct.DataBind();
			}
		}
		#endregion

		#region Page_PreRenderComplete
		void Page_PreRenderComplete(object sender, EventArgs e)
		{
			CHelper.RequireDataBind(false);
		}
		#endregion

		#region MakeDataBind
		public void MakeDataBind(Control _ctrl)
		{
			if (_bindObject == null)
			{
				if (ObjectId != PrimaryKeyId.Empty)
					_bindObject = BusinessManager.Load(CalendarEventEntity.ClassName, ObjectId);
				else
					_bindObject = BusinessManager.InitializeEntity(CalendarEventEntity.ClassName);
			}

			MakeDataBindColl(_ctrl.Controls, _bindObject);

			BindToolbar();
		}

		private static void MakeDataBindColl(ControlCollection _coll, object _obj)
		{
			foreach (Control c in _coll)
			{
				if (c is MCDataBoundControl)
				{
					MCDataBoundControl dbcontrol = (MCDataBoundControl)c;
					dbcontrol.DataItem = _obj;
					dbcontrol.DataBind();
					continue;
				}
				else if (c.Controls.Count > 0)
					MakeDataBindColl(c.Controls, _obj);
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			if (Request["ObjectId"] != ObjectId.ToString())
				MainBlockHeader.Title = String.Format("{0} - {1}",
					CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(CalendarEventEntity.ClassName).FriendlyName),
					GetGlobalResourceObject("IbnFramework.Calendar", "EditSeries").ToString());
			else
				MainBlockHeader.Title = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(CalendarEventEntity.ClassName).FriendlyName);

			MainBlockHeader.ClearLinks();

			MainBlockHeader.AddLink(
				"~/Images/IbnFramework/saveitem.gif",
				CHelper.GetResFileString("{IbnFramework.Global:_mc_Save}"),
				Page.ClientScript.GetPostBackClientHyperlink(btnSave, String.Empty, true)
				);

			string backLink;
			if (Back == "view" && ObjectId != PrimaryKeyId.Empty)
				backLink = CHelper.GetLinkEntityView(CalendarEventEntity.ClassName, Request["ObjectId"]);
			else
				backLink = CHelper.GetLinkEntityList(CalendarEventEntity.ClassName);
			MainBlockHeader.AddLink(
				"~/Images/IbnFramework/cancel.gif",
				CHelper.GetResFileString("{IbnFramework.Global:_mc_Cancel}"),
				backLink);
		}
		#endregion

		#region Save Method
		protected void btnSave_Click(object sender, EventArgs e)
		{
			this.Page.Validate();
			if (!this.Page.IsValid)
				return;

			if (ObjectId != PrimaryKeyId.Empty)
				_bindObject = BusinessManager.Load(CalendarEventEntity.ClassName, ObjectId);
			else
				_bindObject = BusinessManager.InitializeEntity(CalendarEventEntity.ClassName);

			if (_bindObject != null)
			{
				ProcessCollection(this.Page.Controls, (EntityObject)_bindObject);

				PrimaryKeyId objectId = ObjectId;

				if (ObjectId != PrimaryKeyId.Empty)
					BusinessManager.Update((EntityObject)_bindObject);
				else
					objectId = BusinessManager.Create((EntityObject)_bindObject);

				if (Back == "view" || ObjectId == PrimaryKeyId.Empty)
					Response.Redirect(String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName={0}&ObjectId={1}", CalendarEventEntity.ClassName, objectId), true);
				else
					Response.Redirect(String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName={0}", CalendarEventEntity.ClassName), true);
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
							aggrObj = BusinessManager.InitializeEntity(aggrClassName);
						aggrObj[aggrFieldName] = eValue;
					}
				}
			}
		}
		#endregion
	}
}