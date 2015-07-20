using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Core;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls
{
	public partial class Info : System.Web.UI.UserControl
	{
		#region ClassName
		public string ClassName
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["primaryKeyId"] != null)
				{
					retval = MetaViewGroupUtil.GetMetaTypeFromUniqueKey(Request.QueryString["primaryKeyId"]);
				}
				return retval;
			}
		}
		#endregion

		#region ObjectId
		public int ObjectId
		{
			get
			{
				int retval = -1;
				if (Request.QueryString["primaryKeyId"] != null)
				{
					retval = int.Parse(MetaViewGroupUtil.GetIdFromUniqueKey(Request.QueryString["primaryKeyId"]));
				}
				return retval;
			}
		}
		#endregion

		private BusinessObject _bindObject = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			CHelper.AddToContext("ClassName", ClassName);
			CHelper.AddToContext("ObjectId", ObjectId);

			xmlStruct.InnerDataBind += new XmlFormBuilder.InnerDataBindEventHandler(xmlStruct_InnerDataBind);

			if (!IsPostBack)
			{
				BindData();
			}
		}

		#region BindData
		private void BindData()
		{
			xmlStruct.ClassName = ClassName;
			xmlStruct.LayoutType = LayoutType.ObjectView;
			xmlStruct.PlaceName = "TTGridDetails";
			xmlStruct.LayoutMode = LayoutMode.WithTabs;
			xmlStruct.TabBlockCssClass = "ibn-stylebox-light";
			xmlStruct.TabLeftGap = false;
			xmlStruct.SelectTabByPostback = true;

			if (_bindObject == null)
				_bindObject = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName(ClassName), ObjectId);
			xmlStruct.CheckVisibleTab = _bindObject;

			xmlStruct.DataBind();
		}
		#endregion

		#region xmlStruct_InnerDataBind
		void xmlStruct_InnerDataBind(object sender, EventArgs e)
		{
			MakeDataBind(this);
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

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			if (CHelper.NeedToDataBind())
			{
				xmlStruct.DataBind();
			}
			base.OnPreRender(e);

			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("<link rel='stylesheet' type='text/css' href='{0}' />",
					ResolveClientUrl("~/styles/IbnFramework/Forum.css")));

/*			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("<link rel='stylesheet' type='text/css' href='{0}' />",
					ResolveClientUrl("~/styles/IbnFramework/FrameworkUtilTopTabs.css")));
 */ 
		}
		#endregion
	}
}