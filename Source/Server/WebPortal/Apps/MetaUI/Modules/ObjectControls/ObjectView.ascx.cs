using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;

using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Web.UI.Common.Design;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaUI.Modules.ObjectControls
{
	public partial class ObjectView : System.Web.UI.UserControl
	{
		private object _bindObject = null;

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
		protected int ObjectId
		{
			get
			{
				try
				{
					return int.Parse(MetaViewGroupUtil.GetIdFromUniqueKey(Request["ObjectId"]));
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region SectionHeader
		public BlockHeader2 SectionHeader
		{
			get
			{
				return MainBlockHeader;
			}
		}
		#endregion

		#region BindSectionHeader
		public bool BindSectionHeader
		{
			get
			{
				if (ViewState["BindSectionHeader"] != null)
					return (bool)ViewState["BindSectionHeader"];
				else
					return true;
			}
			set
			{
				ViewState["BindSectionHeader"] = value;
			}
		}
		#endregion

		#region PlaceName
		public string PlaceName
		{
			get
			{
				if (ViewState["PlaceName"] != null)
					return (string)ViewState["PlaceName"];
				else
					return "";
			}
			set
			{
				ViewState["PlaceName"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			CHelper.AddToContext("ClassName", ClassName);
			CHelper.AddToContext("ObjectId", ObjectId);
			//CHelper.AddToContext(NavigationBlock.KeyContextMenu, "class_" + ClassName);
			//CHelper.AddToContext(NavigationBlock.KeyContextMenuTitle, CommonHelper.GetResFileString(mc.FriendlyName));

			this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
			xmlStruct.InnerDataBind += new XmlFormBuilder.InnerDataBindEventHandler(xmlStruct_InnerDataBind);

			if (!Page.IsPostBack)
			{
				xmlStruct.ClassName = ClassName;
				xmlStruct.LayoutType = LayoutType.ObjectView;
				if (!String.IsNullOrEmpty(PlaceName))
					xmlStruct.PlaceName = PlaceName;
				xmlStruct.LayoutMode = LayoutMode.WithTabs;

				if (_bindObject == null)
					_bindObject = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName(ClassName), ObjectId);

				xmlStruct.CheckVisibleTab = _bindObject;

				xmlStruct.DataBind();
			}
		}

		void xmlStruct_InnerDataBind(object sender, EventArgs e)
		{
			MakeDataBind(this);
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (CHelper.NeedToDataBind())
			{
				if (_bindObject == null)
					_bindObject = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName(ClassName), ObjectId);

				xmlStruct.CheckVisibleTab = _bindObject;

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
				_bindObject = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName(ClassName), ObjectId);
			MakeDataBindColl(_ctrl.Controls, _bindObject);

			if (BindSectionHeader)
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
			MainBlockHeader.Title = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(ClassName).FriendlyName);
			this.Page.Title = MainBlockHeader.Title;
		}
		#endregion
	}
}