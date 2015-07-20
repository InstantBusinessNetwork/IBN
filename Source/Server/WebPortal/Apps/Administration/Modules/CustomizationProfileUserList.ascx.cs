using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Data.Sql;

namespace Mediachase.Ibn.Web.UI.Administration.Modules
{
	public partial class CustomizationProfileUserList : MCDataBoundControl
	{
		#region ObjectId
		protected int ObjectId
		{
			get
			{
				return int.Parse(Request.QueryString["ObjectId"]);
			}
		}
		#endregion

		private const string ClassName = "Principal";
		private const string ViewName = "ProfileView";
		private const string PlaceName = "EntityView";
		private const string ProfileName = "UsersByProfile";
		private const string BridgeClassName = "CustomizationProfileUser";
		private const string Filter1FieldName = "ProfileId";
		private const string Filter2FieldName = "PrincipalId";

		private const string _httpContextClassNameKey = "ReferenceNN_ClassName";
		private const string _httpContextBridgeClassNameKey = "ReferenceNN_BridgeClassName";
		private const string _httpContextFilter1FieldNameKey = "ReferenceNN_Field1Name";
		private const string _httpContextFilter1FieldValueKey = "ReferenceNN_Field1Value";
		private const string _httpContextFilter2FieldNameKey = "ReferenceNN_Field2Name";

		protected UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		protected void Page_Load(object sender, EventArgs e)
		{
			CHelper.AddToContext(_httpContextBridgeClassNameKey, BridgeClassName);
			CHelper.AddToContext(_httpContextClassNameKey, ClassName);
			CHelper.AddToContext(_httpContextFilter1FieldNameKey, Filter1FieldName);
			CHelper.AddToContext(_httpContextFilter1FieldValueKey, ObjectId);
			CHelper.AddToContext(_httpContextFilter2FieldNameKey, Filter2FieldName);

			grdMain.ShowCheckboxes = (ObjectId > 0);

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
		}

		protected override void OnPreRender(EventArgs e)
		{
			mainDiv.Style.Add(HtmlTextWriterStyle.Padding, "5px");
			base.OnPreRender(e);
		}

		#region DataBind
		public override void DataBind()
		{
			CHelper.AddToContext(_httpContextBridgeClassNameKey, BridgeClassName);
			CHelper.AddToContext(_httpContextClassNameKey, ClassName);
			CHelper.AddToContext(_httpContextFilter1FieldNameKey, Filter1FieldName);
			CHelper.AddToContext(_httpContextFilter1FieldValueKey, ObjectId);
			CHelper.AddToContext(_httpContextFilter2FieldNameKey, Filter2FieldName);

			MetaClass mc = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(ClassName);
			
			grdMain.ClassName = ClassName;
			grdMain.ViewName = ViewName;
			grdMain.PlaceName = PlaceName;
			grdMain.ProfileName = ProfileName;
			grdMain.ShowCheckboxes = (ObjectId > 0);

			MetaClass bridgeClass = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(BridgeClassName);
			string pkName = SqlContext.Current.Database.Tables[mc.DataSource.PrimaryTable].PrimaryKey.Name;
			string bridgeTableName = bridgeClass.DataSource.PrimaryTable;
			FilterElementCollection fec = new FilterElementCollection();
			FilterElement fe;

			if (ObjectId < 0)	// default profile
			{
				fe = new FilterElement(pkName, FilterElementType.In,
					String.Format("(SELECT [{0}] FROM cls_Principal WHERE Card = 'User' AND Activity = 3 AND [{0}] NOT IN (SELECT [{0}] FROM [{1}]))",
						Filter2FieldName, bridgeTableName));
			}
			else 
			{
				fe = new FilterElement(pkName, FilterElementType.In,
					String.Format("(SELECT [{0}] FROM [{1}] WHERE [{2}]={3})",
						Filter2FieldName, bridgeTableName, Filter1FieldName, ObjectId));
			}
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
				PrimaryKeyId objId = PrimaryKeyId.Parse(ObjectId.ToString());
				foreach (string item in mas)
				{
					PrimaryKeyId id = PrimaryKeyId.Parse(MetaViewGroupUtil.GetIdFromUniqueKey(item));
					string className = MetaViewGroupUtil.GetMetaTypeFromUniqueKey(item);

					if (!String.IsNullOrEmpty(className) && className != MetaViewGroupUtil.keyValueNotDefined)
					{
						FilterElementCollection fec = new FilterElementCollection();
						fec.Add(FilterElement.EqualElement(Filter1FieldName, PrimaryKeyId.Parse(ObjectId.ToString())));
						fec.Add(FilterElement.EqualElement(Filter2FieldName, id));
						EntityObject[] list = BusinessManager.List(BridgeClassName, fec.ToArray());
						if (list.Length == 0)
						{
							EntityObject eo = BusinessManager.InitializeEntity(BridgeClassName);
							eo[Filter1FieldName] = PrimaryKeyId.Parse(ObjectId.ToString());
							eo[Filter2FieldName] = id;
							BusinessManager.Create(eo);
						}
					}
					else if (id != PrimaryKeyId.Empty)
					{
						EntityObject eo = BusinessManager.InitializeEntity(BridgeClassName);
						eo[Filter1FieldName] = PrimaryKeyId.Parse(ObjectId.ToString());
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