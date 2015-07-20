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

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.Modules
{
	public partial class EntityEdit : System.Web.UI.UserControl
	{
		#region ClassName
		protected string ClassName
		{
			get
			{
				return Request["ClassName"];
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

		#region ContainerFieldName
		protected string ContainerFieldName
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["ContainerFieldName"] != null)
					retval = Request.QueryString["ContainerFieldName"];
				return retval;
			}
		}
		#endregion

		#region ContainerId
		protected PrimaryKeyId ContainerId
		{
			get
			{
				PrimaryKeyId retval = PrimaryKeyId.Empty;
				if (Request.QueryString["ContainerId"] != null)
					retval = PrimaryKeyId.Parse(Request.QueryString["ContainerId"]);
				return retval;
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
			CHelper.AddToContext("ClassName", ClassName);
			if (ObjectId != PrimaryKeyId.Empty)
				CHelper.AddToContext("ObjectId", ObjectId);

			this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
			xmlStruct.InnerDataBind += new XmlFormBuilder.InnerDataBindEventHandler(xmlStruct_InnerDataBind);

			if (!Page.IsPostBack)
			{
				xmlStruct.ClassName = ClassName;
				xmlStruct.LayoutType = LayoutType.ObjectEdit;

				xmlStruct.DataBind();
			}

			this.Page.Title = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(ClassName).FriendlyName);
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
					_bindObject = BusinessManager.Load(ClassName, ObjectId);
				else
					_bindObject = BusinessManager.InitializeEntity(ClassName);
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
				else if (c is BaseServiceEditControl)
				{
					BaseServiceEditControl bscontrol = (BaseServiceEditControl)c;
					bscontrol.DataItem = _obj;
					bscontrol.DataBind();
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
				Page.ClientScript.GetPostBackClientHyperlink(btnSave, String.Empty, true)
				);

			string backLink;
			if (Back == "view" && ObjectId != PrimaryKeyId.Empty)
				backLink = CHelper.GetLinkEntityView(ClassName, ObjectId.ToString());
			else
			{
				if (ClassName == Mediachase.Ibn.Events.CalendarEventEntity.ClassName)
					backLink = "~/Apps/Calendar/Pages/Calendar.aspx";
				else
					backLink = CHelper.GetLinkEntityList(ClassName);
			}
			MainBlockHeader.AddLink(
				"~/Images/IbnFramework/cancel.gif",
				CHelper.GetResFileString("{IbnFramework.Global:_mc_Cancel}"),
				backLink);

//			if (Back
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

				// Save container id
				if (!String.IsNullOrEmpty(ContainerFieldName)
					&& ((EntityObject)_bindObject).Properties[ContainerFieldName] != null
					&& ContainerId != PrimaryKeyId.Empty)
				{
					((EntityObject)_bindObject)[ContainerFieldName] = ContainerId;
				}

				PrimaryKeyId objectId = ObjectId;

				if (ObjectId != PrimaryKeyId.Empty)
					BusinessManager.Update((EntityObject)_bindObject);
				else
					objectId = BusinessManager.Create((EntityObject)_bindObject);

				if (Mode.ToLower() == "popup")
				{
					string param = String.Empty;
					if (!String.IsNullOrEmpty(CommandName))
					{
						CommandParameters cp = new CommandParameters(CommandName);
						param = cp.ToString();
					}
					Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, param);
				}
				else
				{
					if (Back == "view" || ObjectId == PrimaryKeyId.Empty)
						Response.Redirect(String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName={0}&ObjectId={1}", ClassName, objectId), true);
					else
					{
						if (ClassName == Mediachase.Ibn.Events.CalendarEventEntity.ClassName)
							Response.Redirect("~/Apps/Calendar/Pages/Calendar.aspx", true);
						else
							Response.Redirect(String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName={0}", ClassName), true);
					}
				}
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

			//BaseServiceEditControl bsc = c as BaseServiceEditControl;
			//if (bsc != null)
			//{
			//    bsc.Save(_obj);
			//}
		}
		#endregion
	}
}