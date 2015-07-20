using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;
using System.Collections.Generic;

namespace Mediachase.Ibn.Web.UI
{
	public partial class ListSelectControl : System.Web.UI.UserControl
	{
		#region _className
		private string _className
		{
			get { return (ViewState["_className"] == null ? "" : ViewState["_className"].ToString()); }
			set { ViewState["_className"] = value; }
		}
		#endregion

		#region SelectPopupID
		public string SelectPopupID
		{
			get { return (ViewState["_selectPopupID"] == null ? "" : ViewState["_selectPopupID"].ToString()); }
			set { ViewState["_selectPopupID"] = value; }
		}
		#endregion

		#region Width
		public string Width
		{
			get { return (ViewState["__width"] == null ? "250px" : ViewState["__width"].ToString()); }
			set { ViewState["__width"] = value; }
		}
		#endregion

		private string _objectIds
		{
			get { return (ViewState["__objectIds"] == null ? "" : ViewState["__objectIds"].ToString()); }
			set { ViewState["__objectIds"] = value; hfValue.Value = value.ToString(); }
		}

		#region ObjectIds
		public List<int> ObjectIds
		{
			get
			{
				List<int> newList = new List<int>();
				if (ViewState["__objectIds"] != null)
				{
					string[] sMas = hfValue.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string s in sMas)
						newList.Add(int.Parse(s));
				}
				return newList;
			}
			set
			{
				_objectIds = "";
				foreach (int i in value)
					_objectIds += i + ","; 
			}
		}
		#endregion

		#region AddObjectId
		private void AddObjectId(int id)
		{
			List<int> list = new List<int>(ObjectIds);
			if (!list.Contains(id))
				list.Add(id);
			ObjectIds = list;
		} 
		#endregion

		#region RemoveObjectId
		private void RemoveObjectId(int id)
		{
			List<int> list = new List<int>(ObjectIds);
			if (list.Contains(id))
				list.Remove(id);
			ObjectIds = list;
		} 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(SelectPopupID))
				throw new Exception("SelectPopupID is required!");

			lblSearch.Text = String.Format("<img alt='' align='absmiddle' src='{0}' style='cursor:pointer;' />",
				CHelper.GetAbsolutePath("/images/IbnFramework/plus.png"));
			divSearch.Attributes.Add("onclick",
				String.Format("javascript:MC_SELECT_POPUPS['{0}'].openSelectPopup(event, '{1}');",
					GetPopupClientID(this.Page), hfValue.ClientID));

			txtName.Style.Add("width", Width);
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (Page.IsPostBack)
			{
				if (Request["__EVENTTARGET"] == hfValue.ClientID)
				{
					try
					{
						AddObjectId(int.Parse(Request["__EVENTARGUMENT"]));
					}
					catch { }
				}
				if (Request["__EVENTTARGET"] == hfClear.ClientID)
				{
					try
					{
						RemoveObjectId(int.Parse(Request["__EVENTARGUMENT"]));
					}
					catch { }
				}
			}
			BindObjects();
		}

		#region BindObjects
		private void BindObjects()
		{
			if (!String.IsNullOrEmpty(_className) && ObjectIds.Count > 0)
			{
				txtName.InnerHtml = "";
				foreach (int i in ObjectIds)
				{
					BusinessObject bo = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName(_className), i);
					txtName.InnerHtml += String.Format("<div style='height:18px;vertical-align:middle;border-bottom:1px solid #dedede;padding:2px;'><span style='float:left;'>{0}</span><span style='float:right;cursor:pointer;padding-right:4px;' onclick=\"{2}\"><img alt='' align='absmiddle' src='{1}' style='cursor:pointer;' /></span></div>", 
						bo.Properties[bo.GetMetaType().TitleFieldName].Value.ToString(),
						CHelper.GetAbsolutePath("/images/IbnFramework/delete.gif"),
						String.Format("__doPostBack('{0}', '{1}')", hfClear.ClientID, i.ToString()));
				}
			}
			else
				txtName.InnerHtml = "&nbsp;";
		}
		#endregion

		#region GetPopupClientID
		private string GetPopupClientID(Page _page)
		{
			SelectPopup sp = null;
			sp = GetSelectPopupFromCollection(_page.Controls);
			if (sp != null)
			{
				_className = sp.ClassName;
				return sp.ClientID;
			}
			return "";
		}

		private SelectPopup GetSelectPopupFromCollection(ControlCollection coll)
		{
			SelectPopup retVal = null;
			foreach (Control c in coll)
			{
				if (c is SelectPopup && c.ID == SelectPopupID)
				{
					retVal = (SelectPopup)c;
					break;

				}
				else
				{
					retVal = GetSelectPopupFromCollection(c.Controls);
					if (retVal != null)
						break;
				}
			}
			return retVal;
		}
		#endregion
	}
}