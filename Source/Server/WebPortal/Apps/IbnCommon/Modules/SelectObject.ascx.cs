using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Globalization;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.IbnCommon.Modules
{
	public partial class SelectObject : System.Web.UI.UserControl
	{
		#region _className
		private string _className
		{
			get
			{
				if (Request["ClassName"] != null)
					return Request["ClassName"];
				return String.Empty;
			}
		}
		#endregion

		#region _viewName
		private string _viewName
		{
			get
			{
				if (Request["ViewName"] != null)
					return Request["ViewName"];
				return String.Empty;
			}
		}
		#endregion

		private const string _placeName = "SelectObject";

		private const string _filterOrganization = "Organization";
		private const string _filterContact = "Contact";
		private const string _filterAll = "All";
		private bool _afterSearch = false;

		#region _isMultipleSelect
		private bool _isMultipleSelect
		{
			get
			{
				if (Request["MultipleSelect"] != null && Request["MultipleSelect"] == "1")
					return true;
				else
					return false;
			}
		} 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(_className))
				throw new Exception("ClassName is required!");

			ddFilter.SelectedIndexChanged += new EventHandler(ddFilter_SelectedIndexChanged);

			grdMain.ClassName = _className;
			grdMain.ViewName = _viewName;
			grdMain.PlaceName = _placeName;
			grdMain.ShowCheckboxes = _isMultipleSelect;

			ctrlGridEventUpdater.ClassName = _className;
			ctrlGridEventUpdater.ViewName = _viewName;
			ctrlGridEventUpdater.PlaceName = _placeName;
			ctrlGridEventUpdater.GridId = grdMain.GridClientContainerId;

			BindFilters();

			if (!IsPostBack)
				BindDropDowns();

			BindDataGrid(!Page.IsPostBack);

			if (!Page.IsPostBack)
				BindLabels();

			BindButtons();
		}

		#region ddFilter_SelectedIndexChanged
		/// <summary>
		/// Handles the SelectedIndexChanged event of the ddFilter control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ddFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindDataGrid(true);
			grdMainPanel.Update();
		} 
		#endregion

		#region BindFilters
		private void BindFilters()
		{
			switch (_className)
			{
				case "Client":
					cbShowActive.Checked = false;
					cbShowActive.Visible = false;
					ddFilter.Visible = true;
					break;
				case "Contact":
					ddFilter.Visible = true;
					break;
				case "User":
					cbShowActive.Checked = false;
					cbShowActive.Visible = false;
					ddFilter.Visible = false;
					break;
				default:
					ddFilter.Visible = false;
					break;
			}
		} 
		#endregion

		#region BindDropDowns
		/// <summary>
		/// Binds the drop downs.
		/// </summary>
		private void BindDropDowns()
		{
			if (ddFilter.Visible)
			{
				//ddFilter.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Global:_mc_All}"), _filterAll)); раскомментарить если нужны все записи
				ddFilter.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Common:Contact}"), _filterContact));
				ddFilter.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Common:Organization}"), _filterOrganization));
			}
		} 
		#endregion

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			btnClear.Visible = !String.IsNullOrEmpty(tbSearchString.Text);

			if (CHelper.NeedToBindGrid())
				BindDataGrid(true);

			if (String.IsNullOrEmpty(tbSearchString.Text))
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
							"<script type=\"text/javascript\">" +
							"setTimeout(\"StartFocusElement('" + tbSearchString.ClientID + "')\", 0);</script>");

			base.OnPreRender(e);
		}
		#endregion

		#region BindButtons
		private void BindButtons()
		{
			btnSave.Text = GetGlobalResourceObject("IbnFramework.Common", "btnOK").ToString();
			btnSave.CustomImage = this.Page.ResolveUrl("~/Layouts/images/saveitem.gif");
			btnCancel.Text = GetGlobalResourceObject("IbnFramework.Common", "btnCancel").ToString();
			btnCancel.CustomImage = this.Page.ResolveUrl("~/Layouts/images/cancel.gif");
			if (Request["closeFramePopup"] != null)
				btnCancel.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{}}", Request["closeFramePopup"]));
			else
				btnCancel.Visible = false;
		} 
		#endregion

		#region BindLabels
		private void BindLabels()
		{
			cbShowActive.Text = " " + GetGlobalResourceObject("IbnFramework.Common", "OnlyActiveObjects").ToString();
		} 
		#endregion

		#region BindDataGrid
		private void BindDataGrid(bool dataBind)
		{
			string sSearch = tbSearchString.Text.Trim();
			DataView dv = new DataView();
			switch (_className)
			{
				case "User":
					dv = User.GetListActiveDataTable(sSearch).DefaultView;
					break; 
				case "Incident":
					dv = Incident.GetListIncidentsByKeywordDataTable(sSearch).DefaultView;
					string issRowFilter = "";

					//exclude related issues
					if (Request["exclude"] != null)
					{
						string str = Request["exclude"] + ",";
						DataTable dt = Incident.GetListIncidentRelationsDataTable(int.Parse(Request["ObjectId"]));
						foreach (DataRow dr in dt.Rows)
							str += dr["IncidentId"].ToString() + ",";
						issRowFilter += GetExcludeFilter(str, "IncidentId");
					}
					if (cbShowActive.Checked)
						issRowFilter += String.Format("{3}(StateId={0} OR StateId={1} OR StateId={2})",
							((int)Mediachase.IBN.Business.ObjectStates.Active).ToString(),
							((int)Mediachase.IBN.Business.ObjectStates.ReOpen).ToString(),
							((int)Mediachase.IBN.Business.ObjectStates.Upcoming).ToString(),
							(issRowFilter.Length > 0 ? " AND " : ""));

					dv.RowFilter = issRowFilter;
					break;
				case "Project":
					dv = Project.GetListProjectsByKeywordDataTable(sSearch).DefaultView;
					string prjRowFilter = "";

					//exclude related projects
					if (Request["exclude"] != null)
					{
						string str = Request["exclude"] + ",";
						DataTable dt = Project.GetListProjectRelationsDataTable(int.Parse(Request["ObjectId"]));
						foreach (DataRow dr in dt.Rows)
							str += dr["ProjectId"].ToString() + ",";
						prjRowFilter += GetExcludeFilter(str, "ProjectId");
					}
					if (cbShowActive.Checked)
						prjRowFilter += String.Format("{3}(StatusId={0} OR StatusId={1} OR StatusId={2})",
							((int)Mediachase.IBN.Business.Project.ProjectStatus.Active).ToString(),
							((int)Mediachase.IBN.Business.Project.ProjectStatus.AtRisk).ToString(),
							((int)Mediachase.IBN.Business.Project.ProjectStatus.Pending).ToString(),
							(prjRowFilter.Length > 0 ? " AND " : ""));

					dv.RowFilter = prjRowFilter;
					break;
				case "ToDo":
					dv = ToDo.GetListManagedToDoDataTable(cbShowActive.Checked, sSearch).DefaultView;
					string todoRowFilter = "";
					if (Request["exclude"] != null)
						todoRowFilter += "ToDoId <>" + Request["exclude"];

					dv.RowFilter = todoRowFilter;
					break;
				default:
					break;
			}

			if (_afterSearch && dv.Count == 1)
				OnSelectMethod(dv.Table.Rows[0][grdMain.PrimaryKeyIdField].ToString());

			grdMain.DataSource = dv;

			if (dataBind)
				grdMain.DataBind();
		}
		#endregion

		#region InnerEvents
		protected void btnClear_Click(object sender, ImageClickEventArgs e)
		{
			tbSearchString.Text = "";
			CommonEventPart();
		}

		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			CommonEventPart();
			_afterSearch = true;
		}

		protected void cbShowActive_CheckedChanged(object sender, EventArgs e)
		{
			CommonEventPart();
		}

		private void CommonEventPart()
		{
			upTop.Update();
			grdMainPanel.Update();
			CHelper.RequireBindGrid();
		} 
		#endregion

		#region lbSave_Click
		protected void lbSave_Click(object sender, EventArgs e)
		{
			btnSave.Disabled = true;
			btnCancel.Disabled = true;

			OnSelectMethod(hdnValue.Value);
		}
		#endregion

		#region OnSelectMethod
		private void OnSelectMethod(string values)
		{
			CommandParameters cp = new CommandParameters();
			switch (_className)
			{
				case "Incident":
					cp.CommandName = "MC_HDM_RelatedIss";
					break;
				case "Project":
					cp.CommandName = "MC_PM_RelatedPrj";
					break;
				default:
					break;
			}

			if (Request["ReturnCommand"] != null)
				cp.CommandName = Request["ReturnCommand"];
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("ClassName", _className);
			if (Request["ObjectId"] != null)
				cp.AddCommandArgument("ObjectId", Request["ObjectId"]);
			if (Request["GridId"] != null)
				cp.AddCommandArgument("GridId", Request["GridId"]);
			cp.AddCommandArgument("SelectedValue", values);

			if (_className == "User")
				cp.AddCommandArgument("SelectedHtml", Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL(int.Parse(values)));

			if (!String.IsNullOrEmpty(Request["SelectCtrlId"]))
			{
				string[] mas = values.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
				if (mas.Length > 0)
				{
					cp.AddCommandArgument("SelectObjectId", mas[0]);
					cp.AddCommandArgument("SelectCtrlId", Request["SelectCtrlId"]);
					switch (_className)
					{
						case "Project":
							cp.AddCommandArgument("SelectObjectTypeId", "3");
							cp.AddCommandArgument("Html", CommonHelper.GetObjectHTMLTitle(3, int.Parse(mas[0])));
							break;
						case "User":
							cp.AddCommandArgument("SelectObjectTypeId", "1");
							cp.AddCommandArgument("Html", CommonHelper.GetObjectHTMLTitle(1, int.Parse(mas[0])));
							break;
						default:
							break;
					}

				}
			}

			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion

		#region GetExcludeFilter
		private string GetExcludeFilter(string ids, string primaryKeyName)
		{
			string retVal = String.Empty;
			string[] mas = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			if (mas.Length > 0)
				for (int i = 0; i < mas.Length; i++)
				{
					if (i > 0)
						retVal += " AND ";
					retVal += String.Format("({1} <> {0})", mas[i], primaryKeyName);
				}
			return retVal;
		} 
		#endregion
	}
}