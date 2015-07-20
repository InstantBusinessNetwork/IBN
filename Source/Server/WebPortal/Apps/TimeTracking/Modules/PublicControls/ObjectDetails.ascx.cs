using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Services;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.UI.Web.Apps.MetaUI.Grid;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls
{
	public partial class ObjectDetails : System.Web.UI.UserControl
	{
		#region ClassName
		public string ClassName
		{
			get
			{
				if (ViewState["__className"] == null)
					ViewState["__className"] = "";
				return ViewState["__className"].ToString();
			}
			set
			{
				ViewState["__className"] = value;
			}
		}
		#endregion

		#region ObjectId
		public int ObjectId
		{
			get
			{
				if (ViewState["__objectId"] == null)
					ViewState["__objectId"] = -1;
				return (int)ViewState["__objectId"];
			}
			set
			{
				ViewState["__objectId"] = value;
			}
		}
		#endregion

		private BusinessObject _bindObject = null;

		public HiddenField UpdateElement
		{
			get { return ctrlUpdate; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			ctrlUpdate.ValueChanged += new EventHandler(ctrlUpdate_ValueChanged);
			this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);

			divNoDetails.InnerHtml = String.Format("<span style='margin:10px;'>{0}</span>", CHelper.GetResFileString("{IbnFramework.Global:_mc_NoSelectedItems}"));
			xmlStruct.InnerDataBind += new XmlFormBuilder.InnerDataBindEventHandler(xmlStruct_InnerDataBind);
		}

		void ctrlUpdate_ValueChanged(object sender, EventArgs e)
		{
			xmlStruct.Controls.Clear();
			string[] mas = ctrlUpdate.Value.Split(new char[] { '^' }, StringSplitOptions.RemoveEmptyEntries);
			if (mas.Length > 0)
			{
				ClassName = mas[0].Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1];				

				if (ClassName == MetaViewGroupUtil.keyValueNotDefined)
				{
					CHelper.UpdateParentPanel(this);
					return;
				}
				ObjectId = Convert.ToInt32(mas[0].Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0]);

				_bindObject = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName(ClassName), ObjectId);

				if (_bindObject != null && _bindObject.GetCardMetaType() != null)
					ClassName = _bindObject.GetCardMetaType().Name;

				xmlStruct.ClassName = ClassName;
				xmlStruct.LayoutType = LayoutType.ObjectView;
				xmlStruct.PlaceName = "TTGridDetails";
				xmlStruct.LayoutMode = LayoutMode.WithTabs;
				xmlStruct.TabBlockCssClass = "ibn-stylebox-light";
				xmlStruct.TabLeftGap = false;
				xmlStruct.SelectTabByPostback = true;
				xmlStruct.CheckVisibleTab = _bindObject;

				xmlStruct.DataBind();
			}
			CHelper.UpdateParentPanel(this);
		}

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			object needtoclearselector = CHelper.GetFromContext("NeedToClearSelector");
			if (needtoclearselector != null && needtoclearselector.ToString().ToLower() == "true")
			{
				ctrlUpdate.Value = "";
				xmlStruct.Controls.Clear();
				CHelper.UpdateParentPanel(this);
			}

			if (CHelper.NeedToDataBind())
			{
				xmlStruct.DataBind();
				CHelper.UpdateParentPanel(this);
			}
			base.OnPreRender(e);

			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("<link rel='stylesheet' type='text/css' href='{0}' />",
					CHelper.GetAbsolutePath("/styles/IbnFramework/FrameworkUtilTopTabs.css")));
		}
		#endregion

		#region Page_PreRenderComplete
		void Page_PreRenderComplete(object sender, EventArgs e)
		{
			if (xmlStruct.Controls.Count == 0)
				divNoDetails.Visible = true;
			else
				divNoDetails.Visible = false;

			CHelper.RequireDataBind(false);

			object needtoclearselector = CHelper.GetFromContext("NeedToClearSelector");
			if (needtoclearselector != null && needtoclearselector.ToString().ToLower() == "true")
				CHelper.RemoveFromContext("NeedToClearSelector");
		}
		#endregion

		#region xmlStruct_InnerDataBind
		void xmlStruct_InnerDataBind(object sender, EventArgs e)
		{
			MakeDataBind(this);
		} 
		#endregion

		#region OnBubbleEvent
		protected override bool OnBubbleEvent(object source, EventArgs args)
		{
			bool handled = false;
			SaveCommandArgs se = args as SaveCommandArgs;
			if (se != null)
			{
				SaveObject();
				handled = true;
			}
			return handled;
		}
		#endregion

		#region MakeDataBind
		public void MakeDataBind(Control _ctrl)
		{
            if (ClassName == string.Empty && _bindObject == null)
                return;

			_bindObject = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName(ClassName), ObjectId);

			MakeDataBindColl(_ctrl.Controls, _bindObject);
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

		#region SaveObject
		private void SaveObject()
		{
			if (ObjectId > 0)
			{
				_bindObject = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName(ClassName), ObjectId);

				if (_bindObject != null)
				{
					ProcessCollection(this.Page.Controls, _bindObject);
					_bindObject.Save();

					CHelper.UpdateParentPanel(this);
				}
			}
		}

		private void ProcessCollection(ControlCollection _coll, MetaObject mo)
		{
			foreach (Control c in _coll)
			{
				ProcessControl(c, mo);
				if (c.Controls.Count > 0)
					ProcessCollection(c.Controls, mo);
			}
		}

		private void ProcessControl(Control c, MetaObject mo)
		{
			IEditControl fc = c as IEditControl;
			if (fc != null && !fc.ReadOnly)
			{
				string fieldName = fc.FieldName;
				if (fieldName.ToLower() == "title" && mo.Properties[fieldName] == null)
				{
					fieldName = mo.GetMetaType().TitleFieldName;
				}
				mo.Properties[fieldName].Value = fc.Value;
			}
		} 
		#endregion
	}

	public class SaveCommandArgs : EventArgs
	{
		public SaveCommandArgs()
		{
		}
	}
}