using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Web.UI.Common.Design;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Clients;
using Mediachase.UI.Web.Util;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.Modules
{
	public partial class EntityView : System.Web.UI.UserControl
	{
		private object _bindObject = null;

		#region ClassName
		protected string ClassName
		{
			get
			{
				//if (ViewState["_className"] != null)
				//    return ViewState["_className"].ToString();
				//else
					return Request["ClassName"];
			}
			//set
			//{
			//    ViewState["_className"] = value;
			//}
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

		#region PlaceName
		public string PlaceName
		{
			get
			{
				if (ViewState["PlaceName"] != null)
					return (string)ViewState["PlaceName"];
				else
					return String.Empty;
			}
			set
			{
				ViewState["PlaceName"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			// O.R.: IBN 4.7 fix - check that partner can see the client
			if (ClassName == OrganizationEntity.GetAssignedMetaClassName())
			{
				if (!CommonHelper.CanViewOrganization(ObjectId))
					throw new AccessDeniedException();
			}

			if (ClassName == ContactEntity.GetAssignedMetaClassName())
			{
				if (!CommonHelper.CanViewContact(ObjectId))
					throw new AccessDeniedException();
			}
			//

			CHelper.AddToContext("ClassName", ClassName);
			CHelper.AddToContext("ObjectId", ObjectId);

			this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
			xmlStruct.InnerDataBind += new XmlFormBuilder.InnerDataBindEventHandler(xmlStruct_InnerDataBind);

			if (!Page.IsPostBack)
			{
				// Config/Layout/[LayoutType].[ClassName].[PlaceName].xml
				xmlStruct.LayoutType = LayoutType.EntityView;
				if (!String.IsNullOrEmpty(PlaceName))
					xmlStruct.PlaceName = PlaceName;
				xmlStruct.LayoutMode = LayoutMode.LeftMenu;

				// O.R.[2008-11-02]: Ibn 4.7 hack.
				// По-нормальному надо вводить на уровне класса атрибут ViewLayoutMode.
				// При создании/редактировании метакласса определять способ отображения -
				// с табами или с левым меню
				if (ClassName == "CustomizationProfile" || ClassName == "WorkflowInstance")
					xmlStruct.LayoutMode = LayoutMode.WithTabs;

				if (_bindObject == null)
				{
					try
					{
						_bindObject = BusinessManager.Load(ClassName, ObjectId);
					}
					catch
					{
						Response.Redirect("~/Common/NotExistingId.aspx?ClassName=" + ClassName, true);
					}
				}

				//if (ClassName != ((EntityObject)_bindObject).MetaClassName)
				//    ClassName = ((EntityObject)_bindObject).MetaClassName;

				xmlStruct.ClassName = ClassName;
				xmlStruct.CheckVisibleTab = _bindObject;

				xmlStruct.DataBind();

				Mediachase.IBN.Business.Common.AddEntityHistory(ClassName, ObjectId);
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
					_bindObject = BusinessManager.Load(ClassName, ObjectId);

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
				_bindObject = BusinessManager.Load(ClassName, ObjectId);
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
	}
}