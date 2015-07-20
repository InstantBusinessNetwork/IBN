using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Sql;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.Modules
{
	public partial class EntityListReferenceNN : MCDataBoundControl
	{
		#region ClassName
		public string ClassName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["ClassName"] != null)
					retval = (string)ViewState["ClassName"];
				return retval;
			}
			set
			{
				ViewState["ClassName"] = value;
			}
		}
		#endregion

		#region ViewName
		public string ViewName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["ViewName"] != null)
					retval = (string)ViewState["ViewName"];
				return retval;
			}
			set
			{
				ViewState["ViewName"] = value;
			}
		}
		#endregion

		#region PlaceName
		public string PlaceName
		{
			get
			{
				string retval = "ReferenceNN";
				if (ViewState["PlaceName"] != null)
					retval = (string)ViewState["PlaceName"];
				return retval;
			}
			set
			{
				ViewState["PlaceName"] = value;
			}
		}
		#endregion

		#region ProfileName
		public string ProfileName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["ProfileName"] != null)
					retval = (string)ViewState["ProfileName"];
				return retval;
			}
			set
			{
				ViewState["ProfileName"] = value;
			}
		}
		#endregion

		#region BridgeClassName
		public string BridgeClassName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["BridgeClassName"] != null)
					retval = (string)ViewState["BridgeClassName"];
				return retval;
			}
			set
			{
				ViewState["BridgeClassName"] = value;
			}
		}
		#endregion

		#region Filter1FieldName
		public string Filter1FieldName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["Filter1FieldName"] != null)
					retval = (string)ViewState["Filter1FieldName"];
				return retval;
			}
			set
			{
				ViewState["Filter1FieldName"] = value;
			}
		}
		#endregion

		#region Filter2FieldName
		public string Filter2FieldName
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["Filter2FieldName"] != null)
					retval = (string)ViewState["Filter2FieldName"];
				return retval;
			}
			set
			{
				ViewState["Filter2FieldName"] = value;
			}
		}
		#endregion

		#region ShowToolbar
		public bool ShowToolbar
		{
			get
			{
				return ToolbarRow.Visible;
			}
			set
			{
				ToolbarRow.Visible = value;
			}
		}
		#endregion

		#region DoPadding
		public bool DoPadding
		{
			get
			{
				if (ViewState["__doPadding"] == null)
					ViewState["__doPadding"] = true;
				return (bool)ViewState["__doPadding"];
			}
			set
			{
				ViewState["__doPadding"] = value;
			}
		}
		#endregion

		#region ShowCheckBoxes
		public bool ShowCheckBoxes
		{
			get
			{
				if (ViewState["__showCheckBoxes"] == null)
					ViewState["__showCheckBoxes"] = true;
				return (bool)ViewState["__showCheckBoxes"];
			}
			set
			{
				ViewState["__showCheckBoxes"] = value;
			}
		}
		#endregion

		private const string _httpContextClassNameKey = "ReferenceNN_ClassName";
		private const string _httpContextBridgeClassNameKey = "ReferenceNN_BridgeClassName";
		private const string _httpContextFilter1FieldNameKey = "ReferenceNN_Field1Name";
		private const string _httpContextFilter1FieldValueKey = "ReferenceNN_Field1Value";
		private const string _httpContextFilter2FieldNameKey = "ReferenceNN_Field2Name";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(BridgeClassName))
				CHelper.AddToContext(_httpContextBridgeClassNameKey, BridgeClassName);
			if (!String.IsNullOrEmpty(ClassName))
				CHelper.AddToContext(_httpContextClassNameKey, ClassName);
			if (!String.IsNullOrEmpty(Filter1FieldName))
				CHelper.AddToContext(_httpContextFilter1FieldNameKey, Filter1FieldName);
			if (!String.IsNullOrEmpty(Request["ObjectId"]))
				CHelper.AddToContext(_httpContextFilter1FieldValueKey, Request["ObjectId"]);
			if (!String.IsNullOrEmpty(Filter2FieldName))
				CHelper.AddToContext(_httpContextFilter2FieldNameKey, Filter2FieldName);
			if (!Page.ClientScript.IsClientScriptBlockRegistered("grid.css"))
			{
				string cssLink = String.Format(CultureInfo.InvariantCulture,
					"<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />",
					Mediachase.Ibn.Web.UI.WebControls.McScriptLoader.Current.GetScriptUrl("~/Styles/IbnFramework/grid.css", this.Page));
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "grid.css", cssLink);
			}

			StringBuilder sb = new StringBuilder();
			sb.Append("function SelectItems_Refresh(params){");
			sb.Append("var obj = Sys.Serialization.JavaScriptSerializer.deserialize(params);");
			sb.Append("if(obj && obj.CommandArguments && obj.CommandArguments.SelectedValue)");
			sb.AppendFormat("__doPostBack('{0}', obj.CommandArguments.SelectedValue);", lbAddItems.UniqueID);
			sb.Append("}");

			ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString("N"),
				sb.ToString(), true);

			CommandManager.GetCurrent(this.Page).AddCommand(ClassName, ViewName, PlaceName, "MC_MUI_RefN_NeedToDataBind");
			if (ListManager.MetaClassIsList(ClassName))
				CommandManager.GetCurrent(this.Page).AddCommand(ClassName, String.Empty, "EntityList", "MC_ListApp_ViewItem");
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (DoPadding)
				mainDiv.Style.Add(HtmlTextWriterStyle.Padding, "5px");
			base.OnPreRender(e);
		}

		#region DataBind
		public override void DataBind()
		{
			CHelper.AddToContext(_httpContextBridgeClassNameKey, BridgeClassName);
			CHelper.AddToContext(_httpContextClassNameKey, ClassName);
			CHelper.AddToContext(_httpContextFilter1FieldNameKey, Filter1FieldName);
			CHelper.AddToContext(_httpContextFilter1FieldValueKey, Request["ObjectId"]);
			CHelper.AddToContext(_httpContextFilter2FieldNameKey, Filter2FieldName);

			MetaClass mc = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(ClassName);

			if (String.IsNullOrEmpty(ProfileName))
			{
				ListViewProfile[] list = ListViewProfile.GetSystemProfiles(ClassName, PlaceName);
				if (list.Length == 0)
				{
					list = ListViewProfile.GetSystemProfiles(ClassName, "EntityList");
					if (list.Length == 0)
					{
						list = ListViewProfile.GetSystemProfiles(ClassName, String.Empty);
						if (list.Length == 0)
						{
							ListViewProfile lvp = new ListViewProfile();
							lvp.Id = Guid.NewGuid().ToString();
							lvp.IsPublic = true;
							lvp.IsSystem = true;
							lvp.Name = CHelper.GetResFileString(mc.FriendlyName);
							lvp.ColumnsUI.Add(new ColumnProperties(mc.TitleFieldName, "150px", String.Empty));
							lvp.FieldSet.Add(mc.TitleFieldName);
							lvp.Filters = new FilterElementCollection();
							ListViewProfile.SaveSystemProfile(ClassName, PlaceName, Mediachase.Ibn.Data.Services.Security.CurrentUserId, lvp);
							list = ListViewProfile.GetSystemProfiles(ClassName, String.Empty);
						}
					}
				}
				ProfileName = list[0].Id;
			}
			grdMain.ClassName = ClassName;
			grdMain.ViewName = ViewName;
			grdMain.PlaceName = PlaceName;
			grdMain.ProfileName = ProfileName;
			grdMain.ShowCheckboxes = ShowCheckBoxes;

			MetaClass bridgeClass = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(BridgeClassName);
			string pkName = SqlContext.Current.Database.Tables[mc.DataSource.PrimaryTable].PrimaryKey.Name;
			string bridgeTableName = bridgeClass.DataSource.PrimaryTable;
			FilterElementCollection fec = new FilterElementCollection();
			FilterElement fe = new FilterElement(pkName, FilterElementType.In,
				String.Format("(SELECT [{0}] FROM [{1}] WHERE [{2}]='{3}')",
							Filter2FieldName, bridgeTableName, Filter1FieldName, Request["ObjectId"]));
			//FilterElement fe = new FilterElement(String.Empty, FilterElementType.Custom, 
			//    String.Format("[t01].[{0}] IN (SELECT [{1}] FROM [{2}] WHERE [{3}]='{4}')",
			//                pkName, Filter2FieldName, bridgeTableName, Filter1FieldName, Request["ObjectId"]));
			fec.Add(fe);
			grdMain.AddFilters = fec;
			grdMain.DataBind();

			ctrlGridEventUpdater.ClassName = ClassName;
			ctrlGridEventUpdater.ViewName = ViewName;
			ctrlGridEventUpdater.PlaceName = PlaceName;
			ctrlGridEventUpdater.GridId = grdMain.GridClientContainerId;

			MainMetaToolbar.ClassName = ClassName;
			MainMetaToolbar.ViewName = ViewName;
			MainMetaToolbar.PlaceName = PlaceName;
			MainMetaToolbar.DataBind();

		}
		#endregion

		#region lbAddItems_Click
		protected void lbAddItems_Click(object sender, EventArgs e)
		{
			string s = Request["__EVENTARGUMENT"];
			if (!String.IsNullOrEmpty(s))
			{
				string[] mas = s.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				PrimaryKeyId objId = PrimaryKeyId.Parse(Request["ObjectId"]);
				foreach (string item in mas)
				{
					PrimaryKeyId id = PrimaryKeyId.Parse(MetaViewGroupUtil.GetIdFromUniqueKey(item));
					string className = MetaViewGroupUtil.GetMetaTypeFromUniqueKey(item);

					if (!String.IsNullOrEmpty(className) && className != MetaViewGroupUtil.keyValueNotDefined)
					{
						FilterElementCollection fec = new FilterElementCollection();
						fec.Add(FilterElement.EqualElement(Filter1FieldName, PrimaryKeyId.Parse(Request["ObjectId"])));
						fec.Add(FilterElement.EqualElement(Filter2FieldName, id));
						EntityObject[] list = BusinessManager.List(BridgeClassName, fec.ToArray());
						if (list.Length == 0)
						{
							EntityObject eo = BusinessManager.InitializeEntity(BridgeClassName);
							eo[Filter1FieldName] = PrimaryKeyId.Parse(Request["ObjectId"]);
							eo[Filter2FieldName] = id;
							BusinessManager.Create(eo);
						}
					}
					else if (id != PrimaryKeyId.Empty)
					{
						EntityObject eo = BusinessManager.InitializeEntity(BridgeClassName);
						eo[Filter1FieldName] = PrimaryKeyId.Parse(Request["ObjectId"]);
						eo[Filter2FieldName] = id;
						BusinessManager.Create(eo);
					}
				}
				CHelper.RequireDataBind();
			}
		}
		#endregion
	}
}
