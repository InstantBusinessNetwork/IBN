using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data;
using System.Xml.XPath;
using Mediachase.Ibn.XmlTools;
using Mediachase.Ibn.Business.Customization;

namespace Mediachase.Ibn.Web.UI.Administration.Modules
{
	public partial class LeftMenuItems : MCDataBoundControl
	{
		#region ClassName
		public string ClassName
		{
			get
			{
				if (ViewState["ClassName"] != null)
					return (string)ViewState["ClassName"];
				else
					return string.Empty;
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
				if (ViewState["ViewName"] != null)
					return (string)ViewState["ViewName"];
				else
					return string.Empty;
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
				if (ViewState["PlaceName"] != null)
					return (string)ViewState["PlaceName"];
				else
					return string.Empty;
			}
			set
			{
				ViewState["PlaceName"] = value;
			}
		}
		#endregion

		#region ProfileId
		protected int? ProfileId
		{
			get
			{
				int? retval = null;

				if (String.Compare(Request["ClassName"], CustomizationProfileEntity.ClassName, true) == 0
					&& !String.IsNullOrEmpty(Request["ObjectId"]))
					retval = int.Parse(Request["ObjectId"]);
				return retval;
			}
		}
		#endregion

		#region UserId
		protected int? UserId
		{
			get
			{
				int? retval = null;

				if (String.Compare(Request["ClassName"], "Principal", true) == 0
					&& !String.IsNullOrEmpty(Request["ObjectId"]))
					retval = int.Parse(Request["ObjectId"]);
				return retval;
			}
		}
		#endregion

		protected UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.ClientScript.IsClientScriptBlockRegistered("grid.css"))
			{
				string cssLink = String.Format(CultureInfo.InvariantCulture,
					"<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />",
					Mediachase.Ibn.Web.UI.WebControls.McScriptLoader.Current.GetScriptUrl("~/Styles/IbnFramework/grid.css", this.Page));
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "grid.css", cssLink);
			}

			if (Page.IsPostBack)
				BindDataGrid(false);
		}

		#region DataBind
		public override void DataBind()
		{
			grdMain.ClassName = ClassName;
			grdMain.ViewName = ViewName;
			grdMain.PlaceName = PlaceName;
			pc[grdMain.GetPropertyKey(MCGrid.PageSizePropertyKey)] = "-1";

			ctrlGridEventUpdater.ClassName = ClassName;
			ctrlGridEventUpdater.ViewName = ViewName;
			ctrlGridEventUpdater.PlaceName = PlaceName;
			ctrlGridEventUpdater.GridId = grdMain.GridClientContainerId;

			MainMetaToolbar.ClassName = ClassName;
			MainMetaToolbar.ViewName = ViewName;
			MainMetaToolbar.PlaceName = PlaceName;
			MainMetaToolbar.DataBind();

			BindDataGrid(true);
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid(bool dataBind)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(string)));
			dt.Columns.Add(new DataColumn("FullId", typeof(string)));
			dt.Columns.Add(new DataColumn("Title", typeof(string)));
			dt.Columns.Add(new DataColumn("Order", typeof(string)));
			dt.Columns.Add(new DataColumn("Hidden", typeof(bool)));
			dt.Columns.Add(new DataColumn("HiddenParent", typeof(bool)));
			dt.Columns.Add(new DataColumn("HiddenLayer", typeof(string)));
			dt.Columns.Add(new DataColumn("Added", typeof(bool)));
			dt.Columns.Add(new DataColumn("AddedLayer", typeof(string)));
			dt.Columns.Add(new DataColumn("Changed", typeof(bool)));
			dt.Columns.Add(new DataColumn("ChangedLayer", typeof(string)));
			dt.Columns.Add(new DataColumn("Odd", typeof(bool)));

			bool odd = true;

			string profileString = ProfileId.HasValue ? ProfileId.Value.ToString() : String.Empty;
			string principalString = UserId.HasValue ? UserId.Value.ToString() : String.Empty;
			if (UserId.HasValue)
				profileString = ProfileManager.GetProfileIdByUser(UserId.Value).ToString();

			IXPathNavigable navigable;
			Selector selector = new Selector(string.Empty, string.Empty, string.Empty, profileString, principalString);
			using (DisableDataCacheScope scope = new DisableDataCacheScope())
			{
				navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetCustomizationXml(null, 
					Mediachase.Ibn.XmlTools.StructureType.Navigation, selector);
			}
			XPathNavigator tabsNode = navigable.CreateNavigator().SelectSingleNode("Navigation/Tabs");
			foreach (XPathNavigator subItem in tabsNode.SelectChildren("Tab", string.Empty))
			{
				bool hidden;
				bool.TryParse(subItem.GetAttribute("hidden", string.Empty), out hidden);

				// don't show items, hidden at the above layer
				string hiddenLayer = subItem.GetAttribute("hiddenLayer", string.Empty);
				if (hidden && ProfileId.HasValue && hiddenLayer == NavigationManager.CustomizationLayerGlobal)
					continue;
				if (hidden && UserId.HasValue && (hiddenLayer == NavigationManager.CustomizationLayerGlobal || hiddenLayer == NavigationManager.CustomizationLayerProfile))
					continue;

				string id = subItem.GetAttribute("id", string.Empty);

				DataRow dr = dt.NewRow();
				dr["Id"] = subItem.GetAttribute("id", string.Empty);
				dr["FullId"] = id;
				dr["Title"] = UtilHelper.GetResFileString(subItem.GetAttribute("text", string.Empty));
				dr["Order"] = subItem.GetAttribute("order", string.Empty);

				dr["Hidden"] = hidden;
				dr["HiddenParent"] = false;

				bool changed;
				bool.TryParse(subItem.GetAttribute("changed", string.Empty), out changed);
				dr["Changed"] = changed;
				dr["Odd"] = odd;

				string addedString = subItem.GetAttribute("added", string.Empty);
				if (String.IsNullOrEmpty(addedString))
					dr["Added"] = false;
				else
					dr["Added"] = bool.Parse(addedString);

				dr["HiddenLayer"] = subItem.GetAttribute("hiddenLayer", string.Empty);
				dr["ChangedLayer"] = subItem.GetAttribute("changedLayer", string.Empty);
				dr["AddedLayer"] = subItem.GetAttribute("addedLayer", string.Empty);

				dt.Rows.Add(dr);

//				if (!hidden)
				ProcessChildren(dt, subItem, id, hidden, 0, odd);

				odd = !odd;
			}

			grdMain.DataSource = dt.DefaultView;

			if (dataBind)
				grdMain.DataBind();
		}

		#region ProcessChildren
		private void ProcessChildren(DataTable dt, XPathNavigator linkItem, string path, bool hiddenParent, int level, bool odd)
		{
			level++;

			foreach (XPathNavigator subItem in linkItem.SelectChildren(string.Empty, string.Empty))
			{
				bool hidden;
				bool.TryParse(subItem.GetAttribute("hidden", string.Empty), out hidden);

				// don't show items, hidden at the above layer
				string hiddenLayer = subItem.GetAttribute("hiddenLayer", string.Empty);
				if (hidden && ProfileId.HasValue && hiddenLayer == NavigationManager.CustomizationLayerGlobal)
					continue;
				if (hidden && UserId.HasValue && (hiddenLayer == NavigationManager.CustomizationLayerGlobal || hiddenLayer == NavigationManager.CustomizationLayerProfile))
					continue;

				string id = subItem.GetAttribute("id", string.Empty);
				string fullId = String.Concat(path, "/", id);

				DataRow dr = dt.NewRow();
				dr["Id"] = id;
				dr["FullId"] = fullId;
				dr["Title"] = String.Format(CultureInfo.InvariantCulture,
					"<span style=\"padding-left:{0}px;\">{1}</span>",
					level * 20,
					UtilHelper.GetResFileString(subItem.GetAttribute("text", string.Empty)));

				dr["Order"] = subItem.GetAttribute("order", string.Empty);
				dr["Hidden"] = hidden;
				dr["HiddenParent"] = hiddenParent;

				bool changed;
				bool.TryParse(subItem.GetAttribute("changed", string.Empty), out changed);
				dr["Changed"] = changed;
				dr["Odd"] = odd;

				string addedString = subItem.GetAttribute("added", string.Empty);
				if (String.IsNullOrEmpty(addedString))
					dr["Added"] = false;
				else
					dr["Added"] = bool.Parse(addedString);

				dr["HiddenLayer"] = subItem.GetAttribute("hiddenLayer", string.Empty);
				dr["ChangedLayer"] = subItem.GetAttribute("changedLayer", string.Empty);
				dr["AddedLayer"] = subItem.GetAttribute("addedLayer", string.Empty);

				dt.Rows.Add(dr);

//				if (!hidden && !hiddenParent)
					ProcessChildren(dt, subItem, fullId, hidden || hiddenParent, level, odd);
			}
		}
		#endregion
		#endregion

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			if (CHelper.NeedToBindGrid())
			{
				BindDataGrid(true);
			}

			base.OnPreRender(e);
		}
		#endregion
	}
}