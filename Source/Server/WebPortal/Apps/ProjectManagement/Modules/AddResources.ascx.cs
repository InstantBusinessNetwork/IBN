using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Resources;
using System.Reflection;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.Modules
{
	public partial class AddResources : System.Web.UI.UserControl
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

			//инициализируем тулбар
			MainMetaToolbar.ClassName = _className;
			MainMetaToolbar.ViewName = _viewName;
			MainMetaToolbar.PlaceName = _placeName;

			if (!IsPostBack)
			{
				BindGroups();
				GetObjectData();
			}

			BindDataGrid(!Page.IsPostBack);

			if(!Page.IsPostBack)
				BinddgMemebers();

			BindButtons();
			this.dgMembers.DeleteCommand += new DataGridCommandEventHandler(dgMembers_DeleteCommand);
		}

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			btnClear.Visible = !String.IsNullOrEmpty(tbSearchString.Text);

			if (CHelper.NeedToBindGrid())
				BindDataGrid(true);

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
			DataTable dt = new DataTable();
			switch (_objectTypeId)
			{
				case (int)ObjectTypes.ToDo:
					dt = Mediachase.IBN.Business.ToDo.GetListResourcesDataTable(_objectId);
					break;
				default:
					break;
			}
			ViewState["Resources"] = dt;

			//using (IDataReader rdr = Mediachase.IBN.Business.ToDo.GetToDo(ToDoID))
			//{
			//    if (rdr.Read())
			//    {
			//        int compltype = (int)rdr["CompletionTypeId"];
			//        if (compltype == (int)Mediachase.IBN.Business.CompletionType.All) btnAddGroup.Visible = false;
			//    }
			//}
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			int ProjectId = -1;

			if(_objectTypeId == (int)ObjectTypes.ToDo)
				using (IDataReader reader = Mediachase.IBN.Business.ToDo.GetToDo(_objectId, false))
				{
					if (reader.Read())
						if (reader["ProjectId"] != DBNull.Value)
							ProjectId = (int)reader["ProjectId"];
				}
			else if (_objectTypeId == (int)ObjectTypes.Task)
				using (IDataReader reader = Mediachase.IBN.Business.Task.GetTask(_objectId, false))
				{
					if (reader.Read())
						if (reader["ProjectId"] != DBNull.Value)
							ProjectId = (int)reader["ProjectId"];
				}

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

		#region BinddgMemebers
		private void BinddgMemebers()
		{
			dgMembers.Columns[1].HeaderText = LocRM.GetString("Name");
			dgMembers.Columns[2].HeaderText = LocRM.GetString("Status");

			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();
			dgMembers.DataSource = dt.DefaultView;
			dgMembers.DataBind();

			foreach (DataGridItem dgi in dgMembers.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				ib.Attributes.Add("onclick", "if(confirm('" + LocRM.GetString("Warning") + "')) {DisableButtons(this); return true;} else return false;");
				ib.ToolTip = LocRM.GetString("Delete");
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
					if(!String.IsNullOrEmpty(sSearch))
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

		#region Inner Events
		protected void btnClear_Click(object sender, ImageClickEventArgs e)
		{
			tbSearchString.Text = "";
			CommonEventPart();
		}

		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
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
			tbSearchString.Text = "";
			CommonEventPart();
		}
		#endregion

		#region lbAdd_Click
		protected void lbAdd_Click(object sender, EventArgs e)
		{
			AddChecked(false);

			BinddgMemebers();
			upRight.Update();
			CommonEventPart();
			hdnValue.Value = String.Empty;
		}
		#endregion

		#region lbAddWithConfirm_Click
		protected void lbAddWithConfirm_Click(object sender, EventArgs e)
		{
			AddChecked(true);

			BinddgMemebers();
			upRight.Update();
			CommonEventPart();
			hdnValue.Value = String.Empty;
		}
		#endregion

		#region AddChecked
		private void AddChecked(bool mustBeConfirmed)
		{
			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();
			string[] mas = MCGrid.GetCheckedCollection(this.Page, grdMain.ID);
			if (mas.Length == 0)
				mas = hdnValue.Value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string s in mas)
			{
				DataRow dr = dt.NewRow();
				dr["UserId"] = int.Parse(s);
				dr["MustBeConfirmed"] = mustBeConfirmed;
				dr["ResponsePending"] = true;
				dt.Rows.Add(dr);
			}
			ViewState["Resources"] = dt;
		} 
		#endregion

		#region lbAddGroup_Click
		protected void lbAddGroup_Click(object sender, EventArgs e)
		{
			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();
			if (ddGroups.SelectedItem != null && int.Parse(ddGroups.SelectedValue) > 0 && dt.Select("UserId=" + ddGroups.SelectedValue).Length == 0)
			{
				DataRow dr = dt.NewRow();
				dr["UserId"] = int.Parse(ddGroups.SelectedValue);
				dr["MustBeConfirmed"] = false;
				dt.Rows.Add(dr);
			}
			ViewState["Resources"] = dt;

			BinddgMemebers();
			upRight.Update();
			CommonEventPart();
			hdnValue.Value = String.Empty;
		}
		#endregion

		#region dgMembers_DeleteCommand
		private void dgMembers_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int iUserId = int.Parse(e.Item.Cells[0].Text);
			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();
			DataRow[] dr = dt.Select("UserId = " + iUserId);
			if (dr.Length > 0)
				dt.Rows.Remove(dr[0]);
			ViewState["Resources"] = dt;

			upRight.Update();
			BinddgMemebers();
			CommonEventPart();
		} 
		#endregion

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, EventArgs e)
		{
			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();
			switch (_objectTypeId)
			{
				case (int)ObjectTypes.ToDo:
					Mediachase.IBN.Business.ToDo2.UpdateResources(_objectId, dt);
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

		#region GetLink
		protected string GetLink(int PID, bool IsGroup)
		{
			if (IsGroup)
				return Mediachase.UI.Web.Util.CommonHelper.GetGroupLinkUL(PID);
			else
				return Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL(PID);
		}
		#endregion

		#region GetStatus
		protected string GetStatus(object _mbc, object _rp, object _ic)
		{
			bool mbc = false;
			if (_mbc != DBNull.Value)
				mbc = (bool)_mbc;

			bool rp = false;
			if (_rp != DBNull.Value)
				rp = (bool)_rp;

			bool ic = false;
			if (_ic != DBNull.Value)
				ic = (bool)_ic;

			if (!mbc) return "";
			else
				if (rp) return LocRM.GetString("Waiting");
				else
					if (ic) return LocRM.GetString("Accepted");
					else return LocRM.GetString("Denied");
		}
		#endregion
	}
}