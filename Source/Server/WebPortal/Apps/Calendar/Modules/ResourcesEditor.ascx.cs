using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Events;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Clients;
using Mediachase.IbnNext.TimeTracking;
using System.Resources;
using System.Reflection;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Sql;
using System.Collections.Generic;
using Mediachase.Ibn.Events.Request;

namespace Mediachase.Ibn.Web.UI.Calendar.Modules
{
	public partial class ResourcesEditor : System.Web.UI.UserControl
	{
		private const string _viewName = "CalendarEventResourceSelect";
		private const string _placeName = "CalendarEventView";

		#region _objectId
		private PrimaryKeyId _objectId
		{
			get
			{
				if (Request["ObjectId"] != null)
					return PrimaryKeyId.Parse(Request["ObjectId"]);
				return PrimaryKeyId.Empty;
			}
		}
		#endregion

		#region _realObjectId
		private PrimaryKeyId _realObjectId
		{
			get
			{
				if (Request["RealObjectId"] != null)
				{
					PrimaryKeyId key = PrimaryKeyId.Parse(Request["RealObjectId"]);
					return ((VirtualEventId)key).RealEventId;
				}
				return PrimaryKeyId.Empty;
			}
		}
		#endregion

		#region _workObjectId
		private PrimaryKeyId _workObjectId
		{
			get
			{
				if (_objectId != PrimaryKeyId.Empty)
					return _objectId;
				if (_realObjectId != PrimaryKeyId.Empty)
					return _realObjectId;
				return PrimaryKeyId.Empty;
			}
		}
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strAddResources", Assembly.GetExecutingAssembly());
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", Assembly.GetExecutingAssembly());
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strAddResources", Assembly.GetExecutingAssembly());

		private Mediachase.IBN.Business.UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		private const string _key = "EventResourceSelect_Class";

		protected void Page_Load(object sender, EventArgs e)
		{
			ddFilter.SelectedIndexChanged += new EventHandler(ddFilter_SelectedIndexChanged);

			if (!IsPostBack)
				BindDropDowns();

			grdMain.ClassName = ddFilter.SelectedValue;
			grdMain.ViewName = _viewName;
			grdMain.PlaceName = _placeName;
			grdMain.ProfileName = "CalendarEventResourceList";
			grdMain.ShowCheckboxes = true;
			grdMain.PageSize = -1;

			ctrlGridEventUpdater.ClassName = ddFilter.SelectedValue;
			ctrlGridEventUpdater.ViewName = _viewName;
			ctrlGridEventUpdater.PlaceName = _placeName;
			ctrlGridEventUpdater.GridId = grdMain.GridClientContainerId;
			ctrlGridEventUpdater.GridActionMode = Mediachase.UI.Web.Apps.MetaUI.Grid.MetaGridServerEventAction.Mode.ListViewUI;

			grdMain_Selected.ClassName = "CalendarEventResource";
			grdMain_Selected.ViewName = _viewName;
			grdMain_Selected.PlaceName = _placeName;
			grdMain_Selected.ShowCheckboxes = true;
			_pc[grdMain_Selected.GetPropertyKey(MCGrid.SortingPropertyKey)] = "Name";


			ctrlGridEventUpdater_Selected.ClassName = "CalendarEventResource";
			ctrlGridEventUpdater_Selected.ViewName = _viewName;
			ctrlGridEventUpdater_Selected.PlaceName = _placeName;
			ctrlGridEventUpdater_Selected.GridId = grdMain_Selected.GridClientContainerId;

			if (!IsPostBack)
			{
				BindClassFilter();
				GetObjectData();
			}

			BindDataGrid(!Page.IsPostBack);
			BindDataGridSelected(!Page.IsPostBack);

			BindButtons();
		}

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			btnClear.Visible = !String.IsNullOrEmpty(tbSearchString.Text);

			if (CHelper.NeedToBindGrid())
			{
				BindDataGrid(true);
				BindDataGridSelected(true);
			}

			base.OnPreRender(e);
		}
		#endregion

		#region BindDropDowns
		private void BindDropDowns()
		{
			ddFilter.Items.Clear();
			ddFilter.Items.Add(new ListItem(CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName("Principal").FriendlyName), Principal.GetAssignedMetaClass().Name));
			ddFilter.Items.Add(new ListItem(CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName("Organization").FriendlyName), OrganizationEntity.GetAssignedMetaClassName()));
			ddFilter.Items.Add(new ListItem(CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName("Contact").FriendlyName), ContactEntity.GetAssignedMetaClassName()));

			if (_pc[_key] == null)
				_pc[_key] = "Principal";
			CHelper.SafeSelect(ddFilter, _pc[_key]);
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
			btnCancel.Attributes.Add("onclick", CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true));
		}
		#endregion

		#region BindClassFilter
		private void BindClassFilter()
		{
			switch (ddFilter.SelectedValue)
			{
				case "Principal":
					break;
				default:
					break;
			}
		}
		#endregion

		#region GetObjectData
		private void GetObjectData()
		{
			FilterElementCollection fec = new FilterElementCollection();
			FilterElement fe = FilterElement.EqualElement("EventId", _workObjectId);
			fec.Add(fe);
			EntityObject[] list = BusinessManager.List(CalendarEventResourceEntity.ClassName, fec.ToArray());

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(string)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			DataRow dr;
			foreach (EntityObject eo in list)
			{
				dr = dt.NewRow();
				CalendarEventResourceEntity cero = (CalendarEventResourceEntity)eo;
				if (cero.ContactId.HasValue)
				{
					dr["Id"] = String.Format("{0}::{1}",
						cero.ContactId.Value.ToString(),
						ContactEntity.GetAssignedMetaClassName());
					dr["Name"] = cero.Contact;
				}
				else if (cero.OrganizationId.HasValue)
				{
					dr["Id"] = String.Format("{0}::{1}",
						cero.OrganizationId.Value.ToString(),
						OrganizationEntity.GetAssignedMetaClassName());
					dr["Name"] = cero.Organization;
				}
				else if (cero.PrincipalId.HasValue)
				{
					dr["Id"] = String.Format("{0}::{1}",
						cero.PrincipalId.Value.ToString(),
						Principal.GetAssignedMetaClass().Name);
					dr["Name"] = cero.Principal;
				}
				else
				{
					dr["Id"] = String.Format("{0}::0", cero.Email);
					dr["Name"] = cero.Email;
				}
				dt.Rows.Add(dr);
			}

			ViewState["Resources"] = dt;
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid(bool dataBind)
		{
			grdMain.SearchKeyword = tbSearchString.Text.Trim();

			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();
			FilterElementCollection fec = new FilterElementCollection();
			foreach (DataRow dr in dt.Rows)
			{
				string[] elem = dr["Id"].ToString().Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
				if (elem[1] != "0")
				{
					MetaClass mcEl = MetaDataWrapper.GetMetaClassByName(elem[1]);
					if (mcEl.IsCard)
						elem[1] = mcEl.CardOwner.Name;
				}
				if (elem[1] == ddFilter.SelectedValue)
				{
					MetaClass mc = MetaDataWrapper.GetMetaClassByName(ddFilter.SelectedValue);
					FilterElement fe = FilterElement.NotEqualElement(
						SqlContext.Current.Database.Tables[mc.DataSource.PrimaryTable].PrimaryKey.Name,
						PrimaryKeyId.Parse(elem[0]));
					fec.Add(fe);
				}
			}
			grdMain.AddFilters = fec;

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

		#region ddFilter_SelectedIndexChanged
		/// <summary>
		/// Handles the SelectedIndexChanged event of the ddFilter control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ddFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			grdMain.ClassName = ddFilter.SelectedValue;
			ctrlGridEventUpdater.ClassName = ddFilter.SelectedValue;

			_pc[_key] = ddFilter.SelectedValue;

			BindDataGrid(true);
			grdMainPanel.Update();
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
			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();

			if (!String.IsNullOrEmpty(txtEMail.Text.Trim()))
			{
				DataRow[] rows = dt.Select(String.Format("Id='{0}'", String.Format("{0}::0", txtEMail.Text.Trim())));
				if (rows.Length == 0)
				{
					DataRow dr = dt.NewRow();
					dr["Id"] = String.Format("{0}::0", txtEMail.Text.Trim());
					dr["Name"] = txtEMail.Text.Trim();
					dt.Rows.Add(dr);
				}
				txtEMail.Text = String.Empty;
			}
			else
			{
				string[] mas = EntityGrid.GetCheckedCollection(this.Page, grdMain.ID);
				if (mas.Length == 0)
					mas = hdnValue.Value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string s in mas)
				{
					DataRow[] rows = dt.Select(String.Format("Id='{0}'", s));
					if (rows.Length > 0)
						continue;
					DataRow dr = dt.NewRow();
					if (ddFilter.SelectedValue == ContactEntity.GetAssignedMetaClassName())
					{
						dr["Id"] = s;
						dr["Name"] = CHelper.GetEntityTitle(ContactEntity.GetAssignedMetaClassName(), PrimaryKeyId.Parse(s.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0]));
					}
					else if (ddFilter.SelectedValue == OrganizationEntity.GetAssignedMetaClassName())
					{
						dr["Id"] = s;
						dr["Name"] = CHelper.GetEntityTitle(OrganizationEntity.GetAssignedMetaClassName(), PrimaryKeyId.Parse(s.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0]));
					}
					else if (ddFilter.SelectedValue == Principal.GetAssignedMetaClass().Name)
					{
						dr["Id"] = s;
						dr["Name"] = CHelper.GetEntityTitle(Principal.GetAssignedMetaClass().Name, PrimaryKeyId.Parse(s.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0]));
					}
					dt.Rows.Add(dr);
				}
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
			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();
			string[] mas = MCGrid.GetCheckedCollection(this.Page, grdMain_Selected.ID);
			if (mas.Length == 0)
				mas = hdnValue.Value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string s in mas)
			{
				DataRow[] dr = dt.Select(String.Format("Id='{0}'", s));
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
			DataTable dt = ((DataTable)ViewState["Resources"]).Copy();

			List<CalendarEventResourceEntity> list = new List<CalendarEventResourceEntity>();
			foreach (DataRow dr in dt.Rows)
			{
				CalendarEventResourceEntity cero = BusinessManager.InitializeEntity<CalendarEventResourceEntity>(CalendarEventResourceEntity.ClassName);
				string[] elem = dr["Id"].ToString().Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
				if (elem[1] != "0")
				{
					MetaClass mcEl = MetaDataWrapper.GetMetaClassByName(elem[1]);
					if (mcEl.IsCard)
						elem[1] = mcEl.CardOwner.Name;
				}

				if (elem[1] == "0")
					cero.Email = elem[0];
				else if (elem[1] == Principal.GetAssignedMetaClass().Name)
					cero.PrincipalId = PrimaryKeyId.Parse(elem[0]);
				else if (elem[1] == ContactEntity.GetAssignedMetaClassName())
					cero.ContactId = PrimaryKeyId.Parse(elem[0]);
				else if (elem[1] == OrganizationEntity.GetAssignedMetaClassName())
					cero.OrganizationId = PrimaryKeyId.Parse(elem[0]);

				cero.Name = dr["Name"].ToString();
				cero.Status = (int)eResourceStatus.NotResponded;
				list.Add(cero);
			}

			CalendarEventEntity ceo = (CalendarEventEntity)BusinessManager.Load(CalendarEventEntity.ClassName, _workObjectId);
			CalendarEventUpdateResourcesRequest req = new CalendarEventUpdateResourcesRequest(ceo, list.ToArray());
			BusinessManager.Execute(req);

			if (Request["CommandName"] != null)
			{
				CommandParameters cp = new CommandParameters(Request["CommandName"]);
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, String.Empty);
		}
		#endregion
	}
}