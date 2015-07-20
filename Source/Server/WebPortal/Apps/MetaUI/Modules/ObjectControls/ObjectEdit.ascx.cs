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


namespace Mediachase.Ibn.Web.UI.MetaUI.Modules.ObjectControls
{
	public partial class ObjectEdit : System.Web.UI.UserControl
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
					_bindObject = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName(ClassName), ObjectId);
				else
					_bindObject = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName(ClassName));
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

			SecurityService ss = ((BusinessObject)_bindObject).GetService<SecurityService>();
			if (!(ss != null && !ss.CheckUserRight("Write")))
			{
				MainBlockHeader.AddLink(
					"~/Images/IbnFramework/SAVEITEM.GIF",
					CHelper.GetResFileString("{IbnFramework.Global:_mc_Save}"),
					Page.ClientScript.GetPostBackClientHyperlink(btnSave, "", true)
					);
			}

			if (Mode.ToLower() == "popup")
				MainBlockHeader.AddLink("~/Images/IbnFramework/close.gif", GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Close").ToString(), "javascript:window.close();");
		}
		#endregion

		#region Save Method
		protected void btnSave_Click(object sender, EventArgs e)
		{
			this.Page.Validate();
			if (!this.Page.IsValid)
				return;

			if (ObjectId != PrimaryKeyId.Empty)
				_bindObject = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName(ClassName), ObjectId);
			else
				_bindObject = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName(ClassName));

			if (_bindObject != null)
			{
				ProcessCollection(this.Page.Controls, (BusinessObject)_bindObject);

				((BusinessObject)_bindObject).Save();

				PrimaryKeyId objectId = ((BusinessObject)_bindObject).PrimaryKeyId.Value;

				if (Mode.ToLower() == "popup")
				{
					string param = "";
					if (!String.IsNullOrEmpty(CommandName))
					{
						CommandParameters cp = new CommandParameters(CommandName);
						param = cp.ToString();
					}
					Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, param);
				}
				else
				{
					Response.Redirect(CHelper.GetLinkObjectView_Edit(ClassName, objectId.ToString()));
				}
			}
		}

		private void ProcessCollection(ControlCollection _coll, BusinessObject _obj)
		{
			foreach (Control c in _coll)
			{
				ProcessControl(c, _obj);
				if (c.Controls.Count > 0)
					ProcessCollection(c.Controls, _obj);
			}
		}

		private void ProcessControl(Control c, BusinessObject _obj)
		{
			IEditControl editControl = c as IEditControl;
			if (editControl != null)
			{
				string fieldName = editControl.FieldName;

				string ownFieldName = fieldName;
				string aggrFieldName = String.Empty;
				MetaField ownField = null;
				MetaField aggrField = null;
				if (ownFieldName.Contains("."))
				{
					string[] mas = ownFieldName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
					if (mas.Length > 1)
					{
						ownFieldName = mas[0];
						aggrFieldName = mas[1];
						string aggrClassName = _obj.Properties[ownFieldName].GetMetaType().Attributes[McDataTypeAttribute.AggregationMetaClassName].ToString();
						aggrField = MetaDataWrapper.GetMetaFieldByName(aggrClassName, aggrFieldName);
					}
				}
				ownField = _obj.Properties[ownFieldName].GetMetaType();

				object eValue = editControl.Value;

				bool makeChange = true;

				MetaField field = (aggrField == null) ? ownField : aggrField;
				if ((!field.IsNullable && eValue == null) ||
					_obj.Properties[ownFieldName].IsReadOnly )
					makeChange = false;

				if (makeChange)
				{
					if (aggrField == null)
						_obj[ownFieldName] = eValue;
					else
					{
						//make aggregation
						MetaObject aggrObj = (MetaObject)_obj[ownFieldName];
						aggrObj[aggrFieldName] = eValue;
					}
				}
			}

			BaseServiceEditControl bsc = c as BaseServiceEditControl;
			if (bsc != null)
			{
				bsc.Save(_obj);
			}
		}
		#endregion
	}
}