using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.ClientManagement.Modules
{
	public partial class RelatedObjects : MCDataBoundControl
	{
		#region ClassFilter
		public string ClassFilter
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["ClassFilter"] != null)
					retval = (string)ViewState["ClassFilter"];
				return retval;
			}
			set
			{
				ViewState["ClassFilter"] = value;
			}
		}
		#endregion

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
				return String.Empty;
			}
		}
		#endregion

		#region PlaceName
		public string PlaceName
		{
			get
			{
				string retval = "ClientView";
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
			grdMain.ChangingMCGridColumnHeader += new ChangingMCGridColumnHeaderEventHandler(grdMain_ChangingMCGridColumnHeader);

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			cm.AddCommand(ClassName, ViewName, PlaceName, "MC_CM_HDM_GroupResponsibilityList");
			if(!String.IsNullOrEmpty(ClassFilter) && !String.IsNullOrEmpty(ClassName))
				BindDataGrid(!Page.IsPostBack);
		}

		#region DataBind
		public override void DataBind()
		{
			grdMain.ClassName = ClassName;
			grdMain.ViewName = ViewName;
			grdMain.PlaceName = PlaceName;
			//grdMain.ShowPaging = false;
			//grdMain.PageSize = -1;
			//grdMain.DataBind();

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

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			//_pc["MCGrid_IssueList_" + _viewName + "_PageIndex"] = grdMain.PageIndex.ToString();

			//если необходимо перебиндить датагрид
			if (CHelper.NeedToBindGrid())
			{
				//биндим датагрид
				BindDataGrid(true);
			}

			base.OnPreRender(e);
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid(bool dataBind)
		{
			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			
			PrimaryKeyId uid = PrimaryKeyId.Empty;
			if (Request["ObjectId"] != null)
				uid = PrimaryKeyId.Parse(Request["ObjectId"]);

			switch (ClassFilter)
			{
				case "Organization":
					orgUid = uid;
					break;
				case "Contact":
					contactUid = uid;
					break;
				default:
					break;
			}

			DataTable dt = new DataTable();
			switch(ClassName)
			{
				case "Incident":
					dt = Incident.GetListIncidentsByFilterDataTable
						(0, 0, 0, 0, 0, orgUid, contactUid,
						0, -1, 0, 0, 0, String.Empty,
						0, 0, false, false);
					break;
				case "Project":
					dt = Project.GetListProjectsByFilterDataTable(String.Empty, 0, 0, 0, -1,
						contactUid, orgUid, 0, DateTime.Now, 0, DateTime.Now, 0, 0, 0, 0, 0, 0, false);
					break;
				case "Document":
					dt = Document.GetListDocumentsByFilterDataTable(0, 0, 0, -1, 0, String.Empty, 0, 
						contactUid, orgUid);
					break;
				case "ToDo":
					dt = ToDo.GetListToDoByFilterDataTable(0, 0, String.Empty, 
						DateTime.Today.AddYears(-30), DateTime.Today,
						orgUid, contactUid);
					break;
				default:
					break;
			}
			DataView dv = dt.DefaultView;

			grdMain.DataSource = dv;

			if (dataBind)
				grdMain.DataBind();
		}
		#endregion

		#region grdMain_ChangingMCGridColumnHeader
		void grdMain_ChangingMCGridColumnHeader(object sender, ChangingMCGridColumnHeaderEventArgs e)
		{
			if (e.FieldName == "PriorityId")
			{
				e.ControlField.HeaderText = string.Format(CultureInfo.InvariantCulture, "<span title='{0}'>!!!</span>", GetGlobalResourceObject("IbnFramework.Project", "Priority").ToString());
				//e.ControlField.HeaderText = String.Format("<img width='16' height='16' src='{0}' title='{1}'>",
				//    this.Page.ResolveClientUrl("~/layouts/images/PriorityHeader.gif"),
				//    GetGlobalResourceObject("IbnFramework.Incident", "Priority").ToString());
			}
		}
		#endregion
	}
}