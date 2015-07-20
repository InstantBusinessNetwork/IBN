using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.Modules
{
	public partial class ModifyResources : System.Web.UI.UserControl
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

		private const string _placeName = "AddResources";

		#region _objectId
		private int _objectId
		{
			get
			{
				if (Request["ObjectId"] != null)
					return int.Parse(Request["ObjectId"]);
				return -1;
			}
		}
		#endregion

		#region _objectTypeId
		private int _objectTypeId
		{
			get
			{
				if (Request["ObjectTypeId"] != null)
					return int.Parse(Request["ObjectTypeId"]);
				return -1;
			}
		}
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strAddResources", Assembly.GetExecutingAssembly());
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", Assembly.GetExecutingAssembly());
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strAddResources", Assembly.GetExecutingAssembly());

		protected void Page_Load(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(_className))
				throw new Exception("ClassName is required!");

			grdMain.ClassName = _className;
			grdMain.ViewName = _viewName;
			grdMain.PlaceName = _placeName;
			grdMain.ShowCheckboxes = true;

			ctrlGridEventUpdater.ClassName = _className;
			ctrlGridEventUpdater.ViewName = _viewName;
			ctrlGridEventUpdater.PlaceName = _placeName;
			ctrlGridEventUpdater.GridId = grdMain.GridClientContainerId;

			grdMain_Selected.ClassName = _className;
			grdMain_Selected.ViewName = _viewName;
			grdMain_Selected.PlaceName = _placeName + "_Selected";
			grdMain_Selected.ShowCheckboxes = true;
			
			ctrlGridEventUpdater_Selected.ClassName = _className;
			ctrlGridEventUpdater_Selected.ViewName = _viewName;
			ctrlGridEventUpdater_Selected.PlaceName = _placeName + "_Selected";
			ctrlGridEventUpdater_Selected.GridId = grdMain_Selected.GridClientContainerId;

			if (!IsPostBack)
			{
				BindGroups();
				GetObjectData();
			}

			BindDataGrid(!Page.IsPostBack);
			BindDataGridSelected(!Page.IsPostBack);

			BindButtons();

			cbMustBeConfirmed.Text = "&nbsp;" + LocRM.GetString("MustBeConfirmed");
			cbCanManage.Text = "&nbsp;" + LocRM3.GetString("CanManage");
			btnAddGroup.Text = LocRM.GetString("AddGroup");
		}

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			btnClear.Visible = !String.IsNullOrEmpty(tbSearchString.Text);
			object obj = AssemblyUtil.LoadObject("Mediachase.Ibn.Web.UI.ProjectManagement.CommandHandlers.AddGroupEnableHandler, Mediachase.UI.Web");
			btnAddGroup.Visible = (int.Parse(ddGroups.SelectedValue) > 0) && !IsGroupInTable() && ((ICommandEnableHandler)obj).IsEnable(this, null);
			
			if (CHelper.NeedToBindGrid())
			{
				BindDataGrid(true);
				BindDataGridSelected(true);
			}

			ArrayList userList = new ArrayList();
			DataTable dt = (DataTable)ViewState["Resources"];
			foreach (DataRow row in dt.Rows)
			{
				int principalId = (int)row["UserId"];
				if (Mediachase.IBN.Business.User.IsGroup(principalId))
				{
					using (IDataReader reader = SecureGroup.GetListAllUsersInGroup(principalId, false))
					{
						while (reader.Read())
						{
							if (!userList.Contains((int)reader["UserId"]))
							{
								userList.Add((int)reader["UserId"]);
							}
						}
					}
				}
				else
				{
					if (!userList.Contains(principalId))
					{
						userList.Add(principalId);
					}
				}
			}

			string users = String.Empty;
			foreach (int userId in userList)
			{
				if (!String.IsNullOrEmpty(users))
					users += ",";

				users += userId.ToString();
			}

			if (userList.Count > 0)
			{
				hlResUtil.Text = String.Format(CultureInfo.InvariantCulture,
						"<img alt='' src='{0}'/> {1}",
						Page.ResolveUrl("~/Layouts/Images/ResUtil.png"),
						LocRM2.GetString("Utilization"));
				hlResUtil.NavigateUrl = String.Format(CultureInfo.InvariantCulture,
						"javascript:OpenPopUpNoScrollWindow('{0}?users={1}&amp;ObjectId={2}&amp;ObjectTypeId={3}',750,300)",
						Page.ResolveUrl("~/Common/ResourceUtilGraphForObject.aspx"),
						users,
						_objectId,
						_objectTypeId);
			}
			hlResUtil.Visible = (userList.Count > 0);
			base.OnPreRender(e);
		}
		#endregion

		#region BindButtons
		private void BindButtons()
		{
			btnSave.Text = GetGlobalResourceObject("IbnFramework.Common", "btnOK").ToString();
			btnSave.CustomImage = this.Page.ResolveUrl("~/Layouts/images/saveitem.gif");
			btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
			btnCancel.Text = GetGlobalResourceObject("IbnFramework.Common", "btnCancel").ToString();
			btnCancel.CustomImage = this.Page.ResolveUrl("~/Layouts/images/cancel.gif");
			if (Request["closeFramePopup"] != null)
				btnCancel.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{}}", Request["closeFramePopup"]));
			else
				btnCancel.Visible = false;
		}
		#endregion

		#region GetObjectData
		private void GetObjectData()
		{
			cbCanManage.Visible = false;
			DataTable dt = new DataTable();
			switch (_objectTypeId)
			{
				case (int)ObjectTypes.ToDo:
					dt = Mediachase.IBN.Business.ToDo.GetListResourcesDataTable(_objectId);
					break;
				case (int)ObjectTypes.Task:
					dt = Mediachase.IBN.Business.Task.GetListResourcesDataTable(_objectId);
					cbCanManage.Visible = true;
					break;
				default:
					break;
			}
			ViewState["Resources"] = dt;
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			int ProjectId = -1;

			if (_objectTypeId == (int)ObjectTypes.ToDo)
				using (IDataReader reader = Mediachase.IBN.Business.ToDo.GetToDo(_objectId, false))
				{
					if (reader.Read())
						if (reader["ProjectId"] != DBNull.Value)
							ProjectId = (int)reader["ProjectId"];
				}
			else if (_objectTypeId == (int)ObjectTypes.Task)
				ProjectId = Task.GetProject(_objectId);

			if (ProjectId > 0)
			{
				int pID = -ProjectId;
				ddGroups.Items.Add(new ListItem(LocRM.GetString("ProjectTeam"), pID.ToString()));
			}
			DataTable dt = SecureGroup.GetListGroupsAsTreeDataTable();
			foreach (DataRow row in dt.Rows)
			{
				ddGroups.Items.Add(new ListItem(CHelper.GetResFileString(row["GroupName"].ToString()), row["GroupId"].ToString()));
			}
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid(bool dataBind)
		{
			string sSearch = tbSearchString.Text.Trim();
			int iGroupId = int.Parse(ddGroups.SelectedValue);
			DataTable dtSource = new DataTable();
			switch (_objectTypeId)
			{
				case (int)ObjectTypes.ToDo:
				case (int)ObjectTypes.Task:
					if (!String.IsNullOrEmpty(sSearch))
						dtSource = User.GetListUsersBySubstringDataTable(sSearch);
					else if (iGroupId > 0)
						dtSource = SecureGroup.GetListActiveUsersInGroupDataTable(iGroupId);
					else
						dtSource = Project.GetListTeamMemberNamesWithManagerDataTable(-iGroupId);
					break;
				default:
					break;
			}

			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();
			foreach (DataRow dr in dt.Rows)
			{
				DataRow[] drMas = dtSource.Select("UserId = " + (int)dr["UserId"]);
				if (drMas.Length > 0)
					dtSource.Rows.Remove(drMas[0]);
			}

			grdMain.DataSource = dtSource.DefaultView;

			if (dataBind)
				grdMain.DataBind();
		}
		#endregion

		#region BindDataGridSelected
		private void BindDataGridSelected(bool dataBind)
		{
			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();
			grdMain_Selected.DataSource = dt.DefaultView;

			if (dataBind)
				grdMain_Selected.DataBind();
		} 
		#endregion

		#region Inner Events
		protected void btnClear_Click(object sender, ImageClickEventArgs e)
		{
			SyncDT();
			tbSearchString.Text = "";
			CommonEventPart();
		}

		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			SyncDT();
			CommonEventPart();
		}

		private void CommonEventPart()
		{
			upTop.Update();
			grdMainPanel.Update();
			CHelper.RequireBindGrid();
		}

		protected void ddGroups_ChangeGroup(object sender, System.EventArgs e)
		{
			SyncDT();
			tbSearchString.Text = "";
			CommonEventPart();
		}
		#endregion

		#region lbAdd_Click
		protected void lbAdd_Click(object sender, EventArgs e)
		{
			SyncDT();
			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();
			string[] mas = MCGrid.GetCheckedCollection(this.Page, grdMain.ID);
			if (mas.Length == 0)
				mas = hdnValue.Value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string s in mas)
			{
				DataRow dr = dt.NewRow();
				dr["UserId"] = int.Parse(s);
				dr["MustBeConfirmed"] = cbMustBeConfirmed.Checked;
				dr["ResponsePending"] = true;
				if(_objectTypeId == (int)ObjectTypes.Task)
					dr["CanManage"] = cbCanManage.Checked;
				dt.Rows.Add(dr);
			}
			ViewState["Resources"] = dt;

			grdMainSelectedPanel.Update();
			CommonEventPart();
			hdnValue.Value = String.Empty;
		}
		#endregion

		#region lbAddGroup_Click
		protected void lbAddGroup_Click(object sender, EventArgs e)
		{
			SyncDT();
			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();
			if (ddGroups.SelectedItem != null && int.Parse(ddGroups.SelectedValue) > 0 && dt.Select("UserId=" + ddGroups.SelectedValue).Length == 0)
			{
				DataRow dr = dt.NewRow();
				dr["UserId"] = int.Parse(ddGroups.SelectedValue);
				dr["MustBeConfirmed"] = false;
				dt.Rows.Add(dr);
			}
			ViewState["Resources"] = dt;

			grdMainSelectedPanel.Update();
			CommonEventPart();
			hdnValue.Value = String.Empty;
		}
		#endregion

		#region lbDelete_Click
		protected void lbDelete_Click(object sender, EventArgs e)
		{
			SyncDT();
			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();
			string[] mas = MCGrid.GetCheckedCollection(this.Page, grdMain_Selected.ID);
			if (mas.Length == 0)
				mas = hdnValue.Value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string s in mas)
			{
				DataRow[] dr = dt.Select("UserId = " + s);
				if (dr.Length > 0)
					dt.Rows.Remove(dr[0]);
			}
			ViewState["Resources"] = dt;

			grdMainSelectedPanel.Update();
			CommonEventPart();
			hdnValue.Value = String.Empty;
		}
		#endregion

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, EventArgs e)
		{
			SyncDT();
			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();
			switch (_objectTypeId)
			{
				case (int)ObjectTypes.ToDo:
					Mediachase.IBN.Business.ToDo2.UpdateResources(_objectId, dt);
					break;
				case (int)ObjectTypes.Task:
					Mediachase.IBN.Business.Task2.UpdateResources(_objectId, dt);
					break;
				default:
					break;
			}
			if (Request["ReturnCommand"] != null)
			{
				CommandParameters cp = new CommandParameters(Request["ReturnCommand"]);
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, String.Empty);
		}
		#endregion

		#region SyncDT
		private void SyncDT()
		{
			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();
			foreach (GridViewRow gvr in grdMain_Selected.InnerGrid.Rows)
			{
				foreach (TableCell tc in gvr.Cells)
				{
					if (tc.Controls.Count > 0)
					{
						CheckBox cbCM = (CheckBox)tc.Controls[0].FindControl("cbItemCanManage");
						if (cbCM != null && cbCM.Visible)
						{
							string sid = gvr.Attributes[Mediachase.UI.Web.Apps.MetaUI.Grid.IbnGridView.primaryKeyIdAttr];
							if (!String.IsNullOrEmpty(sid))
							{
								DataRow[] mas = dt.Select("UserId = " + sid);
								if (mas.Length > 0)
									mas[0]["CanManage"] = cbCM.Checked;
							}
						}
					}
				}
			}
			ViewState["Resources"] = dt;
		}
		#endregion


		#region IsGroupInTable
		private bool IsGroupInTable()
		{
			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();
			DataRow[] dr = dt.Select("UserId = " + int.Parse(ddGroups.SelectedValue));
			return (dr.Length > 0);
		} 
		#endregion
	}
}