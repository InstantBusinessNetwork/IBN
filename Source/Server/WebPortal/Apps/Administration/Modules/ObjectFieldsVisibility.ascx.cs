using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Clients;
using System.Collections;

namespace Mediachase.Ibn.Web.UI.Administration.Modules
{
	public partial class ObjectFieldsVisibility : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			divWarning.Visible = false;
			upWarning.Update();
			ddObjectType.SelectedIndexChanged += new EventHandler(ddObjectType_SelectedIndexChanged);
			dgMain.CancelCommand += new DataGridCommandEventHandler(dgMain_CancelCommand);
			dgMain.EditCommand += new DataGridCommandEventHandler(dgMain_EditCommand);
			dgMain.UpdateCommand += new DataGridCommandEventHandler(dgMain_UpdateCommand);
			dgMain.ItemDataBound += new DataGridItemEventHandler(dgMain_ItemDataBound);

			if (!Page.IsPostBack)
			{
				BindTypes();
				BindToolbar();
			}
			ApllyLocalization();
			Mediachase.Ibn.Web.UI.MetaUIEntity.Modules.EntityDropDown.RegisterClientScript(this.Page, "", "", "", "MC_MUI_EntityDD"); 
		}

		#region ApllyLocalization
		private void ApllyLocalization()
		{
			btnSave.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Save").ToString();
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnCancel.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Cancel").ToString();
		} 
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			BlockHeaderMain.Title = GetGlobalResourceObject("IbnFramework.Admin", "ObjectFieldsVisibility").ToString();
			BlockHeaderMain.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' /> {1}",
					this.Page.ResolveClientUrl("~/Layouts/Images/cancel.gif"), GetGlobalResourceObject("IbnShell.Navigation", "tInfoAppearance").ToString()),
				this.Page.ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin1"));
		}
		#endregion

		#region BindTypes
		private void BindTypes()
		{
			List<ListItem> listItems = new List<ListItem>();
			listItems.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Admin", "ObjectType_CalendarEntry").ToString(), "CalendarEntry"));
			listItems.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Admin", "ObjectType_Document").ToString(), "Document"));
			listItems.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Admin", "ObjectType_Incident").ToString(), "Incident"));
			listItems.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Admin", "ObjectType_Task").ToString(), "Task"));
			listItems.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Admin", "ObjectType_ToDo").ToString(), "ToDo"));
			listItems.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Admin", "ObjectType_Project").ToString(), "Project"));
			listItems.Sort(delegate(ListItem x, ListItem y) { return x.Text.CompareTo(y.Text); });
			ddObjectType.Items.Clear();
			ddObjectType.Items.AddRange(listItems.ToArray());

			BindSavedSettings(ddObjectType.SelectedValue);
		} 
		#endregion

		#region BindSavedSettings
		private void BindSavedSettings(string sid)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("FieldId", typeof(string));
			dt.Columns.Add("FieldName", typeof(string));
			dt.Columns.Add("DefaultValue", typeof(string));
			dt.Columns.Add("AllowEdit", typeof(bool));
			dt.Columns.Add("AllowView", typeof(bool));

			#region Fields
			switch (sid)
			{
				case "CalendarEntry":
					AddRow(dt, "client", GetGlobalResourceObject("IbnFramework.Admin", "Field_Client").ToString(), PortalConfig.CEntryAllowEditClientField, PortalConfig.CEntryAllowViewClientField);
					AddRow(dt, "generalcategories", GetGlobalResourceObject("IbnFramework.Admin", "Field_GenCats").ToString(), PortalConfig.CEntryAllowEditGeneralCategoriesField, PortalConfig.CEntryAllowViewGeneralCategoriesField);
					AddRow(dt, "priority", GetGlobalResourceObject("IbnFramework.Admin", "Field_Priority").ToString(), PortalConfig.CEntryAllowEditPriorityField, PortalConfig.CEntryAllowViewPriorityField);
					AddRow(dt, "attachment", GetGlobalResourceObject("IbnFramework.Admin", "Field_Attachment").ToString(), PortalConfig.CEntryAllowEditAttachmentField, false);
					break;
				case "Document":
					AddRow(dt, "client", GetGlobalResourceObject("IbnFramework.Admin", "Field_Client").ToString(), PortalConfig.DocumentAllowEditClientField, PortalConfig.DocumentAllowViewClientField);
					AddRow(dt, "generalcategories", GetGlobalResourceObject("IbnFramework.Admin", "Field_GenCats").ToString(), PortalConfig.DocumentAllowEditGeneralCategoriesField, PortalConfig.DocumentAllowViewGeneralCategoriesField);
					AddRow(dt, "priority", GetGlobalResourceObject("IbnFramework.Admin", "Field_Priority").ToString(), PortalConfig.DocumentAllowEditPriorityField, PortalConfig.DocumentAllowViewPriorityField);
					AddRow(dt, "tasktime", GetGlobalResourceObject("IbnFramework.Admin", "Field_TaskTime").ToString(), PortalConfig.DocumentAllowEditTaskTimeField, PortalConfig.DocumentAllowViewTaskTimeField);
					AddRow(dt, "attachment", GetGlobalResourceObject("IbnFramework.Admin", "Field_Attachment").ToString(), PortalConfig.DocumentAllowEditAttachmentField, false);
					break;
				case "Incident":
					AddRow(dt, "client", GetGlobalResourceObject("IbnFramework.Admin", "Field_Client").ToString(), PortalConfig.IncidentAllowEditClientField, PortalConfig.IncidentAllowViewClientField);
					AddRow(dt, "generalcategories", GetGlobalResourceObject("IbnFramework.Admin", "Field_GenCats").ToString(), PortalConfig.IncidentAllowEditGeneralCategoriesField, PortalConfig.IncidentAllowViewGeneralCategoriesField);
					AddRow(dt, "priority", GetGlobalResourceObject("IbnFramework.Admin", "Field_Priority").ToString(), PortalConfig.IncidentAllowEditPriorityField, PortalConfig.IncidentAllowViewPriorityField);
					AddRow(dt, "tasktime", GetGlobalResourceObject("IbnFramework.Admin", "Field_TaskTime").ToString(), PortalConfig.IncidentAllowEditTaskTimeField, PortalConfig.IncidentAllowViewTaskTimeField);
					AddRow(dt, "incidentcategories", GetGlobalResourceObject("IbnFramework.Admin", "Field_IncCats").ToString(), PortalConfig.IncidentAllowEditIncidentCategoriesField, PortalConfig.IncidentAllowViewIncidentCategoriesField);
					AddRow(dt, "expectedassigndeadline", GetGlobalResourceObject("IbnFramework.Admin", "Field_ExpAssDead").ToString(), PortalConfig.IncidentAllowEditExpAssDeadlineField, PortalConfig.IncidentAllowViewExpAssDeadlineField);
					AddRow(dt, "expectedreplydeadline", GetGlobalResourceObject("IbnFramework.Admin", "Field_ExpRepDead").ToString(), PortalConfig.IncidentAllowEditExpReplyDeadlineField, PortalConfig.IncidentAllowViewExpReplyDeadlineField);
					AddRow(dt, "expectedresolutiondeadline", GetGlobalResourceObject("IbnFramework.Admin", "Field_ExpResolDead").ToString(), PortalConfig.IncidentAllowEditExpResolutionDeadlineField, PortalConfig.IncidentAllowViewExpResolutionDeadlineField);
					AddRow(dt, "incidenttype", GetGlobalResourceObject("IbnFramework.Admin", "Field_Type").ToString(), PortalConfig.IncidentAllowEditTypeField, PortalConfig.IncidentAllowViewTypeField);
					AddRow(dt, "severity", GetGlobalResourceObject("IbnFramework.Admin", "Field_Severity").ToString(), PortalConfig.IncidentAllowEditSeverityField, PortalConfig.IncidentAllowViewSeverityField);
					AddRow(dt, "attachment", GetGlobalResourceObject("IbnFramework.Admin", "Field_Attachment").ToString(), PortalConfig.IncidentAllowEditAttachmentField, false);
					break;
				case "Task":
					AddRow(dt, "generalcategories", GetGlobalResourceObject("IbnFramework.Admin", "Field_GenCats").ToString(), PortalConfig.TaskAllowEditGeneralCategoriesField, PortalConfig.TaskAllowViewGeneralCategoriesField);
					AddRow(dt, "priority", GetGlobalResourceObject("IbnFramework.Admin", "Field_Priority").ToString(), PortalConfig.TaskAllowEditPriorityField, PortalConfig.TaskAllowViewPriorityField);
					AddRow(dt, "tasktime", GetGlobalResourceObject("IbnFramework.Admin", "Field_TaskTime").ToString(), PortalConfig.TaskAllowEditTaskTimeField, PortalConfig.TaskAllowViewTaskTimeField);
					AddRow(dt, "activationtype", GetGlobalResourceObject("IbnFramework.Admin", "Field_ActType").ToString(), PortalConfig.TaskAllowEditActivationTypeField, PortalConfig.TaskAllowViewActivationTypeField);
					AddRow(dt, "completiontype", GetGlobalResourceObject("IbnFramework.Admin", "Field_CompType").ToString(), PortalConfig.TaskAllowEditCompletionTypeField, PortalConfig.TaskAllowViewCompletionTypeField);
					AddRow(dt, "mustbeconfirmbymanager", GetGlobalResourceObject("IbnFramework.Admin", "Field_MustBeConfirm").ToString(), PortalConfig.TaskAllowEditMustConfirmField, PortalConfig.TaskAllowViewMustConfirmField);
					AddRow(dt, "attachment", GetGlobalResourceObject("IbnFramework.Admin", "Field_Attachment").ToString(), PortalConfig.TaskAllowEditAttachmentField, false);
					break;
				case "ToDo":
					AddRow(dt, "client", GetGlobalResourceObject("IbnFramework.Admin", "Field_Client").ToString(), PortalConfig.ToDoAllowEditClientField, PortalConfig.ToDoAllowViewClientField);
					AddRow(dt, "generalcategories", GetGlobalResourceObject("IbnFramework.Admin", "Field_GenCats").ToString(), PortalConfig.ToDoAllowEditGeneralCategoriesField, PortalConfig.ToDoAllowViewGeneralCategoriesField);
					AddRow(dt, "priority", GetGlobalResourceObject("IbnFramework.Admin", "Field_Priority").ToString(), PortalConfig.ToDoAllowEditPriorityField, PortalConfig.ToDoAllowViewPriorityField);
					AddRow(dt, "tasktime", GetGlobalResourceObject("IbnFramework.Admin", "Field_TaskTime").ToString(), PortalConfig.ToDoAllowEditTaskTimeField, PortalConfig.ToDoAllowViewTaskTimeField);
					AddRow(dt, "activationtype", GetGlobalResourceObject("IbnFramework.Admin", "Field_ActType").ToString(), PortalConfig.ToDoAllowEditActivationTypeField, PortalConfig.ToDoAllowViewActivationTypeField);
					AddRow(dt, "completiontype", GetGlobalResourceObject("IbnFramework.Admin", "Field_CompType").ToString(), PortalConfig.ToDoAllowEditCompletionTypeField, PortalConfig.ToDoAllowViewCompletionTypeField);
					AddRow(dt, "mustbeconfirmbymanager", GetGlobalResourceObject("IbnFramework.Admin", "Field_MustBeConfirm").ToString(), PortalConfig.ToDoAllowEditMustConfirmField, PortalConfig.ToDoAllowViewMustConfirmField);
					AddRow(dt, "attachment", GetGlobalResourceObject("IbnFramework.Admin", "Field_Attachment").ToString(), PortalConfig.ToDoAllowEditAttachmentField, false);
					break;
				case "Project":
					AddRow(dt, "client", GetGlobalResourceObject("IbnFramework.Admin", "Field_Client").ToString(), PortalConfig.ProjectAllowEditClientField, PortalConfig.ProjectAllowViewClientField);
					AddRow(dt, "generalcategories", GetGlobalResourceObject("IbnFramework.Admin", "Field_GenCats").ToString(), PortalConfig.ProjectAllowEditGeneralCategoriesField, PortalConfig.ProjectAllowViewGeneralCategoriesField);
					AddRow(dt, "priority", GetGlobalResourceObject("IbnFramework.Admin", "Field_Priority").ToString(), PortalConfig.ProjectAllowEditPriorityField, PortalConfig.ProjectAllowViewPriorityField);
					AddRow(dt, "projectcategories", GetGlobalResourceObject("IbnFramework.Admin", "Field_PrgCats").ToString(), PortalConfig.ProjectAllowEditProjectCategoriesField, PortalConfig.ProjectAllowViewProjectCategoriesField);
					AddRow(dt, "goals", GetGlobalResourceObject("IbnFramework.Admin", "Field_Goals").ToString(), PortalConfig.ProjectAllowEditScopeField, PortalConfig.ProjectAllowViewScopeField);
					AddRow(dt, "deliverables", GetGlobalResourceObject("IbnFramework.Admin", "Field_Deliverables").ToString(), PortalConfig.ProjectAllowEditDeliverablesField, PortalConfig.ProjectAllowViewDeliverablesField);
					AddRow(dt, "scope", GetGlobalResourceObject("IbnFramework.Admin", "Field_Scope").ToString(), PortalConfig.ProjectAllowEditScopeField, PortalConfig.ProjectAllowViewScopeField);
					AddRow(dt, "currency", GetGlobalResourceObject("IbnFramework.Admin", "Field_Currency").ToString(), PortalConfig.ProjectAllowEditCurrencyField, PortalConfig.ProjectAllowViewCurrencyField);
					break;
				default:
					break;
			}
			#endregion

			ViewState["SchemaTable"] = dt;

			BindGrid();
		}

		private void AddRow(DataTable dt, string sid, string name, bool allowedit, bool allowview)
		{
			DataRow dr = dt.NewRow();
			dr["FieldId"] = sid;
			dr["FieldName"] = name;
			dr["DefaultValue"] = GetDefaultValue(ddObjectType.SelectedValue, sid);
			dr["AllowEdit"] = allowedit;
			dr["AllowView"] = allowview;
			dt.Rows.Add(dr);
		} 
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			DataTable dt = ((DataTable)ViewState["SchemaTable"]).Copy();
			DataView dv = dt.DefaultView;
			dv.Sort = "FieldName";

			dgMain.Columns[0].HeaderText = GetGlobalResourceObject("IbnFramework.Admin", "tField").ToString();
			dgMain.Columns[1].HeaderText = GetGlobalResourceObject("IbnFramework.Admin", "tEdit").ToString();
			dgMain.Columns[2].HeaderText = GetGlobalResourceObject("IbnFramework.Admin", "tView").ToString();
			dgMain.Columns[3].HeaderText = GetGlobalResourceObject("IbnFramework.Admin", "tDefaultValue").ToString();

			dgMain.DataKeyField = "FieldId";
			dgMain.DataSource = dv;
			dgMain.DataBind();

			#region DefaultValueEditing
			foreach (DataGridItem dgi in dgMain.Items)
			{
				CheckBox cb = (CheckBox)dgi.FindControl("cbDefault");
				DropDownList ddl = (DropDownList)dgi.FindControl("ddDefault");
				ListBox lb = (ListBox)dgi.FindControl("lbDefault");
				Mediachase.Ibn.Web.UI.MetaUIEntity.Modules.EntityDropDown client = (Mediachase.Ibn.Web.UI.MetaUIEntity.Modules.EntityDropDown)dgi.FindControl("clientDefault");
				Mediachase.UI.Web.Modules.TimeControl time = (Mediachase.UI.Web.Modules.TimeControl)dgi.FindControl("timeDefault");
				TextBox tb = (TextBox)dgi.FindControl("tbDefault");
				if (cb != null && ddl != null && lb != null && client != null)
				{
					string fieldId = dgMain.DataKeys[dgi.ItemIndex].ToString();
					DataRow[] mas = dt.Select("FieldId = '" + fieldId + "'");
					switch (fieldId)
					{
						case "mustbeconfirmbymanager":
							#region mustbeconfirmbymanager
							cb.Visible = true;
							cb.Text = " " + GetGlobalResourceObject("IbnFramework.Admin", "TrueValue").ToString();
							string defConfirm = mas[0]["DefaultValue"].ToString();
							cb.Checked = bool.Parse(defConfirm); 
							#endregion
							break;
						case "priority":
							#region priority
							ddl.Visible = true;
							ddl.DataTextField = "PriorityName";
							ddl.DataValueField = "PriorityId";
							ddl.DataSource = Mediachase.IBN.Business.Document.GetListPriorities();
							ddl.DataBind();
							string defPr = mas[0]["DefaultValue"].ToString();
							CHelper.SafeSelect(ddl, defPr); 
							#endregion
							break;
						case "activationtype":
							#region activationtype
							ddl.Visible = true;
							ddl.DataTextField = "ActivationTypeName";
							ddl.DataValueField = "ActivationTypeId";
							ddl.DataSource = Task.GetListActivationTypes();
							ddl.DataBind();
							string defActT = mas[0]["DefaultValue"].ToString();
							CHelper.SafeSelect(ddl, defActT);  
							#endregion
							break;
						case "completiontype":
							#region completiontype
							ddl.Visible = true;
							ddl.DataTextField = "CompletionTypeName";
							ddl.DataValueField = "CompletionTypeId";
							ddl.DataSource = Task.GetListCompletionTypes();
							ddl.DataBind();
							string defCompT = mas[0]["DefaultValue"].ToString();
							CHelper.SafeSelect(ddl, defCompT);  
							#endregion 
							break;
						case "incidenttype":
							#region incidenttype
							ddl.Visible = true;
							ddl.DataTextField = "TypeName";
							ddl.DataValueField = "TypeId";
							ddl.DataSource = Mediachase.IBN.Business.Incident.GetListIncidentTypes();
							ddl.DataBind();
							string defIncType = mas[0]["DefaultValue"].ToString();
							CHelper.SafeSelect(ddl, defIncType); 
							#endregion 
							break;
						case "severity":
							#region severity
							ddl.Visible = true;
							ddl.DataTextField = "SeverityName";
							ddl.DataValueField = "SeverityId";
							ddl.DataSource = Mediachase.IBN.Business.Incident.GetListIncidentSeverity();
							ddl.DataBind();
							string defIncSev = mas[0]["DefaultValue"].ToString();
							CHelper.SafeSelect(ddl, defIncSev);  
							#endregion
							break;
						case "generalcategories":
							#region generalcategories
							lb.Visible = true;
							lb.DataTextField = "CategoryName";
							lb.DataValueField = "CategoryId";
							lb.DataSource = Mediachase.IBN.Business.Document.GetListCategoriesAll();
							lb.DataBind();
							ArrayList listGC = Mediachase.IBN.Business.Common.StringToArrayList(mas[0]["DefaultValue"].ToString());
							foreach (int i in listGC)
								CHelper.SafeMultipleSelect(lb, i.ToString());
							#endregion
							break;
						case "incidentcategories":
							#region incidentcategories
							lb.Visible = true;
							lb.DataTextField = "CategoryName";
							lb.DataValueField = "CategoryId";
							lb.DataSource = Mediachase.IBN.Business.Incident.GetListIncidentCategories();
							lb.DataBind();
							ArrayList listIC = Mediachase.IBN.Business.Common.StringToArrayList(mas[0]["DefaultValue"].ToString());
							foreach (int i in listIC)
								CHelper.SafeMultipleSelect(lb, i.ToString()); 
							#endregion
							break;
						case "client":
							#region client
							client.Visible = true;
							PrimaryKeyId org_id = PrimaryKeyId.Empty;
							PrimaryKeyId contact_id = PrimaryKeyId.Empty;
							Mediachase.IBN.Business.Common.GetDefaultClient(mas[0]["DefaultValue"].ToString(),
								out contact_id, out org_id);
							if (contact_id != PrimaryKeyId.Empty)
							{
								client.ObjectType = ContactEntity.GetAssignedMetaClassName();
								client.ObjectId = contact_id;
							}
							else if (org_id != PrimaryKeyId.Empty)
							{
								client.ObjectType = OrganizationEntity.GetAssignedMetaClassName();
								client.ObjectId = org_id;
							}
							else
								client.ObjectId = PrimaryKeyId.Empty; 
							#endregion
							break;
						case "tasktime":
						case "expectedassigndeadline":
						case "expectedreplydeadline":
						case "expectedresolutiondeadline":
							#region time
							time.Visible = true;
							time.ReInit(); 
							time.Value = DateTime.MinValue;
							string defTime = mas[0]["DefaultValue"].ToString();
							if (!String.IsNullOrEmpty(defTime))
								time.Value = DateTime.MinValue.AddMinutes(int.Parse(defTime));
							#endregion
							break;
						case "projectcategories":
							#region projectcategories
							lb.Visible = true;
							lb.DataTextField = "CategoryName";
							lb.DataValueField = "CategoryId";
							lb.DataSource = Mediachase.IBN.Business.Project.GetListProjectCategories();
							lb.DataBind();
							ArrayList listPC = Mediachase.IBN.Business.Common.StringToArrayList(mas[0]["DefaultValue"].ToString());
							foreach (int i in listPC)
								CHelper.SafeMultipleSelect(lb, i.ToString());
							#endregion
							break;
						case "goals":
						case "deliverables":
						case "scope":
							#region text
							tb.Visible = true;
							tb.Text = mas[0]["DefaultValue"].ToString();
							#endregion
							break;
						case "currency":
							#region currency
							ddl.Visible = true;
							ddl.DataTextField = "CurrencySymbol";
							ddl.DataValueField = "CurrencyId";
							ddl.DataSource = Mediachase.IBN.Business.Project.GetListCurrency();
							ddl.DataBind();
							string defCur = mas[0]["DefaultValue"].ToString();
							CHelper.SafeSelect(ddl, defCur);
							#endregion
							break;
						default:
							break;
					}
				}
			}
			#endregion
		} 
		#endregion

		#region CheckChanges
		private void CheckChanges()
		{
			CheckChanges(true);
		}

		private void CheckChanges(bool withDefault)
		{
			DataTable dt = ((DataTable)ViewState["SchemaTable"]).Copy();
			foreach (DataGridItem dgi in dgMain.Items)
			{
				string fieldId = dgMain.DataKeys[dgi.ItemIndex].ToString();
				DataRow[] mas = dt.Select("FieldId='" + fieldId + "'");
				CheckBox cbEdit = (CheckBox)dgi.FindControl("cbAllowEdit");
				CheckBox cbView = (CheckBox)dgi.FindControl("cbAllowView");
				if (mas.Length > 0 && cbEdit != null && cbView != null)
				{
					mas[0]["AllowEdit"] = cbEdit.Checked;
					mas[0]["AllowView"] = cbView.Checked;
				}
				if (withDefault)
				{
					//client
					Mediachase.Ibn.Web.UI.MetaUIEntity.Modules.EntityDropDown client = (Mediachase.Ibn.Web.UI.MetaUIEntity.Modules.EntityDropDown)dgi.FindControl("clientDefault");
					if (client != null && client.Visible)
					{
						if (client.ObjectId != PrimaryKeyId.Empty)
							mas[0]["DefaultValue"] = String.Format("{0}_{1}", client.ObjectType, client.ObjectId.ToString());
						else
							mas[0]["DefaultValue"] = String.Empty;
					}
					//ListBox
					ListBox lb = (ListBox)dgi.FindControl("lbDefault");
					if (lb != null && lb.Visible)
					{
						List<string> list = new List<string>();
						foreach (ListItem li in lb.Items)
							if (li.Selected)
								list.Add(li.Value);
						mas[0]["DefaultValue"] = String.Join(",", list.ToArray());
					}
					//DropDown
					DropDownList ddl = (DropDownList)dgi.FindControl("ddDefault");
					if (ddl != null && ddl.Visible)
						mas[0]["DefaultValue"] = ddl.SelectedValue;
					//CheckBox
					CheckBox cb = (CheckBox)dgi.FindControl("cbDefault");
					if (cb != null && cb.Visible)
						mas[0]["DefaultValue"] = cb.Checked.ToString();
					//Time
					Mediachase.UI.Web.Modules.TimeControl time = (Mediachase.UI.Web.Modules.TimeControl)dgi.FindControl("timeDefault");
					if (time != null && time.Visible)
					{
						TimeSpan ts = new TimeSpan(time.Value.Ticks);
						int minutes = (int)ts.TotalMinutes;
						mas[0]["DefaultValue"] = minutes.ToString();
					}
					// TextBox
					TextBox tb = (TextBox)dgi.FindControl("tbDefault");
					if (tb != null && tb.Visible)
						mas[0]["DefaultValue"] = tb.Text.Trim();
				}
			}
			ViewState["SchemaTable"] = dt;
		} 
		#endregion

		#region ItemDataBound - Visibility
		void dgMain_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.DataItem != null && e.Item.DataItem is DataRowView)
			{
				DataRowView row = (DataRowView)e.Item.DataItem;
				if (String.Compare(row.Row["FieldId"].ToString(), "attachment", true) == 0)
				{
					CheckBox cbView = (CheckBox)e.Item.FindControl("cbAllowView");
					if (cbView != null)
						cbView.Visible = false;
				}
			}
		}
		#endregion

		#region Edit-Update-Cancel
		void dgMain_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			CheckChanges();
			dgMain.EditItemIndex = -1;
			BindGrid();
		}

		void dgMain_EditCommand(object source, DataGridCommandEventArgs e)
		{
			CheckChanges();
			dgMain.EditItemIndex = e.Item.ItemIndex;
			BindGrid();
		}

		void dgMain_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			CheckChanges(false);
			dgMain.EditItemIndex = -1;
			BindGrid();
		} 
		#endregion

		#region ddObjectType_SelectedIndexChanged
		void ddObjectType_SelectedIndexChanged(object sender, EventArgs e)
		{
			dgMain.EditItemIndex = -1;
			BindSavedSettings(ddObjectType.SelectedValue);
		} 
		#endregion

		#region Save-Cancel
		protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin1");
		}

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			dgMain.EditItemIndex = -1;

			CheckChanges(false);

			DataTable dt = ((DataTable)ViewState["SchemaTable"]).Copy();
			switch (ddObjectType.SelectedValue)
			{
				case "CalendarEntry":
					#region CalendarEntry
					foreach (DataRow dr in dt.Rows)
					{
						switch (dr["FieldId"].ToString())
						{
							case "client":
								if(PortalConfig.CEntryAllowEditClientField != (bool)dr["AllowEdit"])
									PortalConfig.CEntryAllowEditClientField = (bool)dr["AllowEdit"];
								if(PortalConfig.CEntryAllowViewClientField != (bool)dr["AllowView"])
									PortalConfig.CEntryAllowViewClientField = (bool)dr["AllowView"];
								if (PortalConfig.CEntryDefaultValueClientField != dr["DefaultValue"].ToString())
									PortalConfig.CEntryDefaultValueClientField = dr["DefaultValue"].ToString();
								break;
							case "generalcategories":
								if(PortalConfig.CEntryAllowEditGeneralCategoriesField != (bool)dr["AllowEdit"])
									PortalConfig.CEntryAllowEditGeneralCategoriesField = (bool)dr["AllowEdit"];
								if(PortalConfig.CEntryAllowViewGeneralCategoriesField != (bool)dr["AllowView"])
									PortalConfig.CEntryAllowViewGeneralCategoriesField = (bool)dr["AllowView"];
								if (PortalConfig.CEntryDefaultValueGeneralCategoriesField != dr["DefaultValue"].ToString())
									PortalConfig.CEntryDefaultValueGeneralCategoriesField = dr["DefaultValue"].ToString();
								break;
							case "priority":
								if(PortalConfig.CEntryAllowEditPriorityField != (bool)dr["AllowEdit"])									
									PortalConfig.CEntryAllowEditPriorityField = (bool)dr["AllowEdit"];
								if(PortalConfig.CEntryAllowViewPriorityField != (bool)dr["AllowView"])
									PortalConfig.CEntryAllowViewPriorityField = (bool)dr["AllowView"];
								if (PortalConfig.CEntryDefaultValuePriorityField != dr["DefaultValue"].ToString())
									PortalConfig.CEntryDefaultValuePriorityField = dr["DefaultValue"].ToString();
								break;
							case "attachment":
								if(PortalConfig.CEntryAllowEditAttachmentField != (bool)dr["AllowEdit"])
									PortalConfig.CEntryAllowEditAttachmentField = (bool)dr["AllowEdit"];
								break;
							default:
								break;
						}
					} 
					#endregion
					break;
				case "Document":
					#region Document
					foreach (DataRow dr in dt.Rows)
					{
						switch (dr["FieldId"].ToString())
						{
							case "client":
								if (PortalConfig.DocumentAllowEditClientField != (bool)dr["AllowEdit"])
									PortalConfig.DocumentAllowEditClientField = (bool)dr["AllowEdit"];
								if (PortalConfig.DocumentAllowViewClientField != (bool)dr["AllowView"])
									PortalConfig.DocumentAllowViewClientField = (bool)dr["AllowView"];
								if (PortalConfig.DocumentDefaultValueClientField != dr["DefaultValue"].ToString())
									PortalConfig.DocumentDefaultValueClientField = dr["DefaultValue"].ToString();
								break;
							case "generalcategories":
								if (PortalConfig.DocumentAllowEditGeneralCategoriesField != (bool)dr["AllowEdit"])
									PortalConfig.DocumentAllowEditGeneralCategoriesField = (bool)dr["AllowEdit"];
								if (PortalConfig.DocumentAllowViewGeneralCategoriesField != (bool)dr["AllowView"])
									PortalConfig.DocumentAllowViewGeneralCategoriesField = (bool)dr["AllowView"];
								if (PortalConfig.DocumentDefaultValueGeneralCategoriesField != dr["DefaultValue"].ToString())
									PortalConfig.DocumentDefaultValueGeneralCategoriesField = dr["DefaultValue"].ToString();
								break;
							case "priority":
								if (PortalConfig.DocumentAllowEditPriorityField != (bool)dr["AllowEdit"])
									PortalConfig.DocumentAllowEditPriorityField = (bool)dr["AllowEdit"];
								if (PortalConfig.DocumentAllowViewPriorityField != (bool)dr["AllowView"])
									PortalConfig.DocumentAllowViewPriorityField = (bool)dr["AllowView"];
								if (PortalConfig.DocumentDefaultValuePriorityField != dr["DefaultValue"].ToString())
									PortalConfig.DocumentDefaultValuePriorityField = dr["DefaultValue"].ToString();
								break;
							case "tasktime":
								if (PortalConfig.DocumentAllowEditTaskTimeField != (bool)dr["AllowEdit"])
									PortalConfig.DocumentAllowEditTaskTimeField = (bool)dr["AllowEdit"];
								if (PortalConfig.DocumentAllowViewTaskTimeField != (bool)dr["AllowView"])
									PortalConfig.DocumentAllowViewTaskTimeField = (bool)dr["AllowView"];
								if (PortalConfig.DocumentDefaultValueTaskTimeField != dr["DefaultValue"].ToString())
									PortalConfig.DocumentDefaultValueTaskTimeField = dr["DefaultValue"].ToString();
								break;
							case "attachment":
								if (PortalConfig.DocumentAllowEditAttachmentField != (bool)dr["AllowEdit"])
									PortalConfig.DocumentAllowEditAttachmentField = (bool)dr["AllowEdit"];
								break;
							default:
								break;
						}
					} 
					#endregion
					break;
				case "Incident":
					#region Incident
					foreach (DataRow dr in dt.Rows)
					{
						switch (dr["FieldId"].ToString())
						{
							case "client":
								if (PortalConfig.IncidentAllowEditClientField != (bool)dr["AllowEdit"])
									PortalConfig.IncidentAllowEditClientField = (bool)dr["AllowEdit"];
								if (PortalConfig.IncidentAllowViewClientField != (bool)dr["AllowView"])
									PortalConfig.IncidentAllowViewClientField = (bool)dr["AllowView"];
								if (PortalConfig.IncidentDefaultValueClientField != dr["DefaultValue"].ToString())
									PortalConfig.IncidentDefaultValueClientField = dr["DefaultValue"].ToString();
								break;
							case "generalcategories":
								if (PortalConfig.IncidentAllowEditGeneralCategoriesField != (bool)dr["AllowEdit"])
									PortalConfig.IncidentAllowEditGeneralCategoriesField = (bool)dr["AllowEdit"];
								if (PortalConfig.IncidentAllowViewGeneralCategoriesField != (bool)dr["AllowView"])
									PortalConfig.IncidentAllowViewGeneralCategoriesField = (bool)dr["AllowView"];
								if (PortalConfig.IncidentDefaultValueGeneralCategoriesField != dr["DefaultValue"].ToString())
									PortalConfig.IncidentDefaultValueGeneralCategoriesField = dr["DefaultValue"].ToString();
								break;
							case "priority":
								if (PortalConfig.IncidentAllowEditPriorityField != (bool)dr["AllowEdit"])
									PortalConfig.IncidentAllowEditPriorityField = (bool)dr["AllowEdit"];
								if (PortalConfig.IncidentAllowViewPriorityField != (bool)dr["AllowView"])
									PortalConfig.IncidentAllowViewPriorityField = (bool)dr["AllowView"];
								if (PortalConfig.IncidentDefaultValuePriorityField != dr["DefaultValue"].ToString())
									PortalConfig.IncidentDefaultValuePriorityField = dr["DefaultValue"].ToString();
								break;
							case "tasktime":
								if (PortalConfig.IncidentAllowEditTaskTimeField != (bool)dr["AllowEdit"])
									PortalConfig.IncidentAllowEditTaskTimeField = (bool)dr["AllowEdit"];
								if (PortalConfig.IncidentAllowViewTaskTimeField != (bool)dr["AllowView"])
									PortalConfig.IncidentAllowViewTaskTimeField = (bool)dr["AllowView"];
								if (PortalConfig.IncidentDefaultValueTaskTimeField != dr["DefaultValue"].ToString())
									PortalConfig.IncidentDefaultValueTaskTimeField = dr["DefaultValue"].ToString();
								break;
							case "attachment":
								if (PortalConfig.IncidentAllowEditAttachmentField != (bool)dr["AllowEdit"])
									PortalConfig.IncidentAllowEditAttachmentField = (bool)dr["AllowEdit"];
								break;
							case "incidentcategories":
								if (PortalConfig.IncidentAllowEditIncidentCategoriesField != (bool)dr["AllowEdit"])
									PortalConfig.IncidentAllowEditIncidentCategoriesField = (bool)dr["AllowEdit"];
								if (PortalConfig.IncidentAllowViewIncidentCategoriesField != (bool)dr["AllowView"])
									PortalConfig.IncidentAllowViewIncidentCategoriesField = (bool)dr["AllowView"];
								if (PortalConfig.IncidentDefaultValueIncidentCategoriesField != dr["DefaultValue"].ToString())
									PortalConfig.IncidentDefaultValueIncidentCategoriesField = dr["DefaultValue"].ToString();
								break;
							case "expectedassigndeadline":
								if (PortalConfig.IncidentAllowEditExpAssDeadlineField != (bool)dr["AllowEdit"])
									PortalConfig.IncidentAllowEditExpAssDeadlineField = (bool)dr["AllowEdit"];
								if (PortalConfig.IncidentAllowViewExpAssDeadlineField != (bool)dr["AllowView"])
									PortalConfig.IncidentAllowViewExpAssDeadlineField = (bool)dr["AllowView"];
								if (PortalConfig.IncidentDefaultValueExpAssDeadlineField != dr["DefaultValue"].ToString())
									PortalConfig.IncidentDefaultValueExpAssDeadlineField = dr["DefaultValue"].ToString();
								break;
							case "expectedreplydeadline":
								if (PortalConfig.IncidentAllowEditExpReplyDeadlineField != (bool)dr["AllowEdit"])
									PortalConfig.IncidentAllowEditExpReplyDeadlineField = (bool)dr["AllowEdit"];
								if (PortalConfig.IncidentAllowViewExpReplyDeadlineField != (bool)dr["AllowView"])
									PortalConfig.IncidentAllowViewExpReplyDeadlineField = (bool)dr["AllowView"];
								if (PortalConfig.IncidentDefaultValueExpReplyDeadlineField != dr["DefaultValue"].ToString())
									PortalConfig.IncidentDefaultValueExpReplyDeadlineField = dr["DefaultValue"].ToString();
								break;
							case "expectedresolutiondeadline":
								if (PortalConfig.IncidentAllowEditExpResolutionDeadlineField != (bool)dr["AllowEdit"])
									PortalConfig.IncidentAllowEditExpResolutionDeadlineField = (bool)dr["AllowEdit"];
								if (PortalConfig.IncidentAllowViewExpResolutionDeadlineField != (bool)dr["AllowView"])
									PortalConfig.IncidentAllowViewExpResolutionDeadlineField = (bool)dr["AllowView"];
								if (PortalConfig.IncidentDefaultValueExpResolutionDeadlineField != dr["DefaultValue"].ToString())
									PortalConfig.IncidentDefaultValueExpResolutionDeadlineField = dr["DefaultValue"].ToString();
								break;
							case "incidenttype":
								if (PortalConfig.IncidentAllowEditTypeField != (bool)dr["AllowEdit"])
									PortalConfig.IncidentAllowEditTypeField = (bool)dr["AllowEdit"];
								if (PortalConfig.IncidentAllowViewTypeField != (bool)dr["AllowView"])
									PortalConfig.IncidentAllowViewTypeField = (bool)dr["AllowView"];
								if (PortalConfig.IncidentDefaultValueTypeField != dr["DefaultValue"].ToString())
									PortalConfig.IncidentDefaultValueTypeField = dr["DefaultValue"].ToString();
								break;
							case "severity":
								if (PortalConfig.IncidentAllowEditSeverityField != (bool)dr["AllowEdit"])
									PortalConfig.IncidentAllowEditSeverityField = (bool)dr["AllowEdit"];
								if (PortalConfig.IncidentAllowViewSeverityField != (bool)dr["AllowView"])
									PortalConfig.IncidentAllowViewSeverityField = (bool)dr["AllowView"];
								if (PortalConfig.IncidentDefaultValueSeverityField != dr["DefaultValue"].ToString())
									PortalConfig.IncidentDefaultValueSeverityField = dr["DefaultValue"].ToString();
								break;
							default:
								break;
						}
					} 
					#endregion
					break;
				case "Task":
					#region Task
					foreach (DataRow dr in dt.Rows)
					{
						switch (dr["FieldId"].ToString())
						{
							case "generalcategories":
								if (PortalConfig.TaskAllowEditGeneralCategoriesField != (bool)dr["AllowEdit"])
									PortalConfig.TaskAllowEditGeneralCategoriesField = (bool)dr["AllowEdit"];
								if (PortalConfig.TaskAllowViewGeneralCategoriesField != (bool)dr["AllowView"])
									PortalConfig.TaskAllowViewGeneralCategoriesField = (bool)dr["AllowView"];
								if (PortalConfig.TaskDefaultValueGeneralCategoriesField != dr["DefaultValue"].ToString())
									PortalConfig.TaskDefaultValueGeneralCategoriesField = dr["DefaultValue"].ToString();
								break;
							case "priority":
								if (PortalConfig.TaskAllowEditPriorityField != (bool)dr["AllowEdit"])
									PortalConfig.TaskAllowEditPriorityField = (bool)dr["AllowEdit"];
								if (PortalConfig.TaskAllowViewPriorityField != (bool)dr["AllowView"])
									PortalConfig.TaskAllowViewPriorityField = (bool)dr["AllowView"];
								if (PortalConfig.TaskDefaultValuePriorityField != dr["DefaultValue"].ToString())
									PortalConfig.TaskDefaultValuePriorityField = dr["DefaultValue"].ToString();
								break;
							case "tasktime":
								if (PortalConfig.TaskAllowEditTaskTimeField != (bool)dr["AllowEdit"])
									PortalConfig.TaskAllowEditTaskTimeField = (bool)dr["AllowEdit"];
								if (PortalConfig.TaskAllowViewTaskTimeField != (bool)dr["AllowView"])
									PortalConfig.TaskAllowViewTaskTimeField = (bool)dr["AllowView"];
								if (PortalConfig.TaskDefaultValueTaskTimeField != dr["DefaultValue"].ToString())
									PortalConfig.TaskDefaultValueTaskTimeField = dr["DefaultValue"].ToString();
								break;
							case "attachment":
								if (PortalConfig.TaskAllowEditAttachmentField != (bool)dr["AllowEdit"])
									PortalConfig.TaskAllowEditAttachmentField = (bool)dr["AllowEdit"];
								break;
							case "activationtype":
								if (PortalConfig.TaskAllowEditActivationTypeField != (bool)dr["AllowEdit"])
									PortalConfig.TaskAllowEditActivationTypeField = (bool)dr["AllowEdit"];
								if (PortalConfig.TaskAllowViewActivationTypeField != (bool)dr["AllowView"])
									PortalConfig.TaskAllowViewActivationTypeField = (bool)dr["AllowView"];
								if (PortalConfig.TaskDefaultValueActivationTypeField != dr["DefaultValue"].ToString())
									PortalConfig.TaskDefaultValueActivationTypeField = dr["DefaultValue"].ToString();
								break;
							case "completiontype":
								if (PortalConfig.TaskAllowEditCompletionTypeField != (bool)dr["AllowEdit"])
									PortalConfig.TaskAllowEditCompletionTypeField = (bool)dr["AllowEdit"];
								if (PortalConfig.TaskAllowViewCompletionTypeField != (bool)dr["AllowView"])
									PortalConfig.TaskAllowViewCompletionTypeField = (bool)dr["AllowView"];
								if (PortalConfig.TaskDefaultValueCompetionTypeField != dr["DefaultValue"].ToString())
									PortalConfig.TaskDefaultValueCompetionTypeField = dr["DefaultValue"].ToString();
								break;
							case "mustbeconfirmbymanager":
								if (PortalConfig.TaskAllowEditMustConfirmField != (bool)dr["AllowEdit"])
									PortalConfig.TaskAllowEditMustConfirmField = (bool)dr["AllowEdit"];
								if (PortalConfig.TaskAllowViewMustConfirmField != (bool)dr["AllowView"])
									PortalConfig.TaskAllowViewMustConfirmField = (bool)dr["AllowView"];
								if (PortalConfig.TaskDefaultValueMustConfirmField != dr["DefaultValue"].ToString())
									PortalConfig.TaskDefaultValueMustConfirmField = dr["DefaultValue"].ToString();
								break;
							default:
								break;
						}
					} 
					#endregion
					break;
				case "ToDo":
					#region ToDo
					foreach (DataRow dr in dt.Rows)
					{
						switch (dr["FieldId"].ToString())
						{
							case "client":
								if (PortalConfig.ToDoAllowEditClientField != (bool)dr["AllowEdit"])
									PortalConfig.ToDoAllowEditClientField = (bool)dr["AllowEdit"];
								if (PortalConfig.ToDoAllowViewClientField != (bool)dr["AllowView"])
									PortalConfig.ToDoAllowViewClientField = (bool)dr["AllowView"];
								if (PortalConfig.ToDoDefaultValueClientField != dr["DefaultValue"].ToString())
									PortalConfig.ToDoDefaultValueClientField = dr["DefaultValue"].ToString();
								break;
							case "generalcategories":
								if (PortalConfig.ToDoAllowEditGeneralCategoriesField != (bool)dr["AllowEdit"])
									PortalConfig.ToDoAllowEditGeneralCategoriesField = (bool)dr["AllowEdit"];
								if (PortalConfig.ToDoAllowViewGeneralCategoriesField != (bool)dr["AllowView"])
									PortalConfig.ToDoAllowViewGeneralCategoriesField = (bool)dr["AllowView"];
								if (PortalConfig.ToDoDefaultValueGeneralCategoriesField != dr["DefaultValue"].ToString())
									PortalConfig.ToDoDefaultValueGeneralCategoriesField = dr["DefaultValue"].ToString();
								break;
							case "priority":
								if (PortalConfig.ToDoAllowEditPriorityField != (bool)dr["AllowEdit"])
									PortalConfig.ToDoAllowEditPriorityField = (bool)dr["AllowEdit"];
								if (PortalConfig.ToDoAllowViewPriorityField != (bool)dr["AllowView"])
									PortalConfig.ToDoAllowViewPriorityField = (bool)dr["AllowView"];
								if (PortalConfig.ToDoDefaultValuePriorityField != dr["DefaultValue"].ToString())
									PortalConfig.ToDoDefaultValuePriorityField = dr["DefaultValue"].ToString();
								break;
							case "tasktime":
								if (PortalConfig.ToDoAllowEditTaskTimeField != (bool)dr["AllowEdit"])
									PortalConfig.ToDoAllowEditTaskTimeField = (bool)dr["AllowEdit"];
								if (PortalConfig.ToDoAllowViewTaskTimeField != (bool)dr["AllowView"])
									PortalConfig.ToDoAllowViewTaskTimeField = (bool)dr["AllowView"];
								if (PortalConfig.ToDoDefaultValueTaskTimeField != dr["DefaultValue"].ToString())
									PortalConfig.ToDoDefaultValueTaskTimeField = dr["DefaultValue"].ToString();
								break;
							case "attachment":
								if (PortalConfig.ToDoAllowEditAttachmentField != (bool)dr["AllowEdit"])
									PortalConfig.ToDoAllowEditAttachmentField = (bool)dr["AllowEdit"];
								break;
							case "activationtype":
								if (PortalConfig.ToDoAllowEditActivationTypeField != (bool)dr["AllowEdit"])
									PortalConfig.ToDoAllowEditActivationTypeField = (bool)dr["AllowEdit"];
								if (PortalConfig.ToDoAllowViewActivationTypeField != (bool)dr["AllowView"])
									PortalConfig.ToDoAllowViewActivationTypeField = (bool)dr["AllowView"];
								if (PortalConfig.ToDoDefaultValueActivationTypeField != dr["DefaultValue"].ToString())
									PortalConfig.ToDoDefaultValueActivationTypeField = dr["DefaultValue"].ToString();
								break;
							case "completiontype":
								if (PortalConfig.ToDoAllowEditCompletionTypeField != (bool)dr["AllowEdit"])
									PortalConfig.ToDoAllowEditCompletionTypeField = (bool)dr["AllowEdit"];
								if (PortalConfig.ToDoAllowViewCompletionTypeField != (bool)dr["AllowView"])
									PortalConfig.ToDoAllowViewCompletionTypeField = (bool)dr["AllowView"];
								if (PortalConfig.ToDoDefaultValueCompetionTypeField != dr["DefaultValue"].ToString())
									PortalConfig.ToDoDefaultValueCompetionTypeField = dr["DefaultValue"].ToString();
								break;
							case "mustbeconfirmbymanager":
								if (PortalConfig.ToDoAllowEditMustConfirmField != (bool)dr["AllowEdit"])
									PortalConfig.ToDoAllowEditMustConfirmField = (bool)dr["AllowEdit"];
								if (PortalConfig.ToDoAllowViewMustConfirmField != (bool)dr["AllowView"])
									PortalConfig.ToDoAllowViewMustConfirmField = (bool)dr["AllowView"];
								if (PortalConfig.ToDoDefaultValueMustConfirmField != dr["DefaultValue"].ToString())
									PortalConfig.ToDoDefaultValueMustConfirmField = dr["DefaultValue"].ToString();
								break;
							default:
								break;
						}
					} 
					#endregion
					break;
				case "Project":
					#region Project
					foreach (DataRow dr in dt.Rows)
					{
						switch (dr["FieldId"].ToString())
						{
							case "client":
								if (PortalConfig.ProjectAllowEditClientField != (bool)dr["AllowEdit"])
									PortalConfig.ProjectAllowEditClientField = (bool)dr["AllowEdit"];
								if (PortalConfig.ProjectAllowViewClientField != (bool)dr["AllowView"])
									PortalConfig.ProjectAllowViewClientField = (bool)dr["AllowView"];
								if (PortalConfig.ProjectDefaultValueClientField != dr["DefaultValue"].ToString())
									PortalConfig.ProjectDefaultValueClientField = dr["DefaultValue"].ToString();
								break;
							case "generalcategories":
								if (PortalConfig.ProjectAllowEditGeneralCategoriesField != (bool)dr["AllowEdit"])
									PortalConfig.ProjectAllowEditGeneralCategoriesField = (bool)dr["AllowEdit"];
								if (PortalConfig.ProjectAllowViewGeneralCategoriesField != (bool)dr["AllowView"])
									PortalConfig.ProjectAllowViewGeneralCategoriesField = (bool)dr["AllowView"];
								if (PortalConfig.ProjectDefaultValueGeneralCategoriesField != dr["DefaultValue"].ToString())
									PortalConfig.ProjectDefaultValueGeneralCategoriesField = dr["DefaultValue"].ToString();
								break;
							case "priority":
								if (PortalConfig.ProjectAllowEditPriorityField != (bool)dr["AllowEdit"])
									PortalConfig.ProjectAllowEditPriorityField = (bool)dr["AllowEdit"];
								if (PortalConfig.ProjectAllowViewPriorityField != (bool)dr["AllowView"])
									PortalConfig.ProjectAllowViewPriorityField = (bool)dr["AllowView"];
								if (PortalConfig.ProjectDefaultValuePriorityField != dr["DefaultValue"].ToString())
									PortalConfig.ProjectDefaultValuePriorityField = dr["DefaultValue"].ToString();
								break;
							case "projectcategories":
								if (PortalConfig.ProjectAllowEditProjectCategoriesField != (bool)dr["AllowEdit"])
									PortalConfig.ProjectAllowEditProjectCategoriesField = (bool)dr["AllowEdit"];
								if (PortalConfig.ProjectAllowViewProjectCategoriesField != (bool)dr["AllowView"])
									PortalConfig.ProjectAllowViewProjectCategoriesField = (bool)dr["AllowView"];
								if (PortalConfig.ProjectDefaultValueProjectCategoriesField != dr["DefaultValue"].ToString())
									PortalConfig.ProjectDefaultValueProjectCategoriesField = dr["DefaultValue"].ToString();
								break;
							case "goals":
								if (PortalConfig.ProjectAllowEditGoalsField != (bool)dr["AllowEdit"])
									PortalConfig.ProjectAllowEditGoalsField = (bool)dr["AllowEdit"];
								if (PortalConfig.ProjectAllowViewGoalsField != (bool)dr["AllowView"])
									PortalConfig.ProjectAllowViewGoalsField = (bool)dr["AllowView"];
								if (PortalConfig.ProjectDefaultValueGoalsField != dr["DefaultValue"].ToString())
									PortalConfig.ProjectDefaultValueGoalsField = dr["DefaultValue"].ToString();
								break;
							case "deliverables":
								if (PortalConfig.ProjectAllowEditDeliverablesField != (bool)dr["AllowEdit"])
									PortalConfig.ProjectAllowEditDeliverablesField = (bool)dr["AllowEdit"];
								if (PortalConfig.ProjectAllowViewDeliverablesField != (bool)dr["AllowView"])
									PortalConfig.ProjectAllowViewDeliverablesField = (bool)dr["AllowView"];
								if (PortalConfig.ProjectDefaultValueDeliverablesField != dr["DefaultValue"].ToString())
									PortalConfig.ProjectDefaultValueDeliverablesField = dr["DefaultValue"].ToString();
								break;
							case "scope":
								if (PortalConfig.ProjectAllowEditScopeField != (bool)dr["AllowEdit"])
									PortalConfig.ProjectAllowEditScopeField = (bool)dr["AllowEdit"];
								if (PortalConfig.ProjectAllowViewScopeField != (bool)dr["AllowView"])
									PortalConfig.ProjectAllowViewScopeField = (bool)dr["AllowView"];
								if (PortalConfig.ProjectDefaultValueScopeField != dr["DefaultValue"].ToString())
									PortalConfig.ProjectDefaultValueScopeField = dr["DefaultValue"].ToString();
								break;
							case "currency":
								if (PortalConfig.ProjectAllowEditCurrencyField != (bool)dr["AllowEdit"])
									PortalConfig.ProjectAllowEditCurrencyField = (bool)dr["AllowEdit"];
								if (PortalConfig.ProjectAllowViewCurrencyField != (bool)dr["AllowView"])
									PortalConfig.ProjectAllowViewCurrencyField = (bool)dr["AllowView"];
								if (PortalConfig.ProjectDefaultValueCurrencyField != dr["DefaultValue"].ToString())
									PortalConfig.ProjectDefaultValueCurrencyField = dr["DefaultValue"].ToString();
								break;
							default:
								break;
						}
					}
					#endregion
					break;
				default:
					break;
			}

			divWarning.Visible = true;
			upWarning.Update();

			Mediachase.Ibn.Web.UI.WebControls.ClientScript.RegisterStartupScript(this.Page,
				this.Page.GetType(), Guid.NewGuid().ToString("N"), "LoadFunc();", true);
			//Response.Redirect("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin1");
		} 
		#endregion

		#region GetDefaultValue
		private string GetDefaultValue(string objectType, string sid)
		{
			switch (objectType)
			{
				case "CalendarEntry":
					#region CalendarEntry
					switch (sid)
					{
						case "client":
							return PortalConfig.CEntryDefaultValueClientField;
						case "generalcategories":
							return PortalConfig.CEntryDefaultValueGeneralCategoriesField;
						case "priority":
							return PortalConfig.CEntryDefaultValuePriorityField;
						default:
							break;
					}
					#endregion
					break;
				case "Document":
					#region Document
					switch (sid)
					{
						case "client":
							return PortalConfig.DocumentDefaultValueClientField;
						case "generalcategories":
							return PortalConfig.DocumentDefaultValueGeneralCategoriesField;
						case "priority":
							return PortalConfig.DocumentDefaultValuePriorityField;
						case "tasktime":
							return PortalConfig.DocumentDefaultValueTaskTimeField;
						default:
							break;
					}
					#endregion
					break;
				case "Incident":
					#region Incident
					switch (sid)
					{
						case "client":
							return PortalConfig.IncidentDefaultValueClientField;
						case "generalcategories":
							return PortalConfig.IncidentDefaultValueGeneralCategoriesField;
						case "priority":
							return PortalConfig.IncidentDefaultValuePriorityField;
						case "tasktime":
							return PortalConfig.IncidentDefaultValueTaskTimeField;
						case "incidentcategories":
							return PortalConfig.IncidentDefaultValueIncidentCategoriesField;
						case "expectedassigndeadline":
							return PortalConfig.IncidentDefaultValueExpAssDeadlineField;
						case "expectedreplydeadline":
							return PortalConfig.IncidentDefaultValueExpReplyDeadlineField;
						case "expectedresolutiondeadline":
							return PortalConfig.IncidentDefaultValueExpResolutionDeadlineField;
						case "incidenttype":
							return PortalConfig.IncidentDefaultValueTypeField;
						case "severity":
							return PortalConfig.IncidentDefaultValueSeverityField;
						default:
							break;
					}
					#endregion
					break;
				case "Task":
					#region Task
					switch (sid)
					{
						case "generalcategories":
							return PortalConfig.TaskDefaultValueGeneralCategoriesField;
						case "priority":
							return PortalConfig.TaskDefaultValuePriorityField;
						case "tasktime":
							return PortalConfig.TaskDefaultValueTaskTimeField;
						case "activationtype":
							return PortalConfig.TaskDefaultValueActivationTypeField;
						case "completiontype":
							return PortalConfig.TaskDefaultValueCompetionTypeField;
						case "mustbeconfirmbymanager":
							return PortalConfig.TaskDefaultValueMustConfirmField;
						default:
							break;
					}
					#endregion
					break;
				case "ToDo":
					#region ToDo
					switch (sid)
					{
						case "client":
							return PortalConfig.ToDoDefaultValueClientField;
						case "generalcategories":
							return PortalConfig.ToDoDefaultValueGeneralCategoriesField;
						case "priority":
							return PortalConfig.ToDoDefaultValuePriorityField;
						case "tasktime":
							return PortalConfig.ToDoDefaultValueTaskTimeField;
						case "activationtype":
							return PortalConfig.ToDoDefaultValueActivationTypeField;
						case "completiontype":
							return PortalConfig.ToDoDefaultValueCompetionTypeField;
						case "mustbeconfirmbymanager":
							return PortalConfig.ToDoDefaultValueMustConfirmField;
						default:
							break;
					}
					#endregion
					break;
				case "Project":
					#region Project
					switch (sid)
					{
						case "client":
							return PortalConfig.ProjectDefaultValueClientField;
						case "generalcategories":
							return PortalConfig.ProjectDefaultValueGeneralCategoriesField;
						case "priority":
							return PortalConfig.ProjectDefaultValuePriorityField;
						case "projectcategories":
							return PortalConfig.ProjectDefaultValueProjectCategoriesField;
						case "goals":
							return PortalConfig.ProjectDefaultValueGoalsField;
						case "deliverables":
							return PortalConfig.ProjectDefaultValueDeliverablesField;
						case "scope":
							return PortalConfig.ProjectDefaultValueScopeField;
						case "currency":
							return PortalConfig.ProjectDefaultValueCurrencyField;
						default:
							break;
					}
					#endregion
					break;
				default:
					break;
			}
			return String.Empty;
		} 
		#endregion

		#region GetFriendlyDefaultValue
		protected string GetFriendlyDefaultValue(string sid, string value)
		{
			switch (sid)
			{
				case "client":
					return GetClientName(value);
				case "generalcategories":
					return GetGeneralCategories(value);
				case "priority":
					if (!String.IsNullOrEmpty(value))
						return Mediachase.IBN.Business.Common.GetPriority(int.Parse(value));
					return String.Empty;
				case "tasktime":
				case "expectedassigndeadline":
				case "expectedreplydeadline":
				case "expectedresolutiondeadline":
					if (!String.IsNullOrEmpty(value))
						return Mediachase.UI.Web.Util.CommonHelper.GetHours(int.Parse(value));
					break;
				case "activationtype":
					if (!String.IsNullOrEmpty(value))
						return Mediachase.IBN.Business.Common.GetActivationType(int.Parse(value));
					return String.Empty;
				case "completiontype":
					if (!String.IsNullOrEmpty(value))
						return Mediachase.IBN.Business.Common.GetCompletionType(int.Parse(value));
					return String.Empty;
				case "mustbeconfirmbymanager":
					return GetBool(value);
				case "incidentcategories":
					return GetIncidentCategories(value);
				case "incidenttype":
					if (!String.IsNullOrEmpty(value))
						return Mediachase.IBN.Business.Common.GetIncidentType(int.Parse(value));
					return String.Empty;
				case "severity":
					if (!String.IsNullOrEmpty(value))
						return Mediachase.IBN.Business.Common.GetIncidentSeverity(int.Parse(value));
					return String.Empty;
				case "projectcategories":
					return GetProjectCategories(value);
				case "goals":
				case "deliverables":
				case "scope":
					return value;
				case "currency":
					if (!String.IsNullOrEmpty(value))
						return Mediachase.IBN.Business.Common.GetCurrency(int.Parse(value));
					return String.Empty;
				default:
					break;
			}
			return String.Empty;
		} 
		#endregion

		#region GetClientName
		private string GetClientName(string value)
		{
			PrimaryKeyId org_id = PrimaryKeyId.Empty;
			PrimaryKeyId contact_id = PrimaryKeyId.Empty;
			Mediachase.IBN.Business.Common.GetDefaultClient(value, out contact_id, out org_id);
			if (contact_id != PrimaryKeyId.Empty)
				return CHelper.GetEntityTitleHtml(ContactEntity.GetAssignedMetaClassName(), contact_id);
			else if (org_id != PrimaryKeyId.Empty)
				return CHelper.GetEntityTitleHtml(OrganizationEntity.GetAssignedMetaClassName(), org_id);
			return String.Format("<img align='absmiddle' border='0' src='{0}' />&nbsp;{1}",
					this.Page.ResolveUrl("~/layouts/images/not_set.png"),
					GetGlobalResourceObject("IbnFramework.Admin", "tObjectNotSet").ToString());
		} 
		#endregion

		#region GetGeneralCategories
		private string GetGeneralCategories(string value)
		{
			ArrayList list = Mediachase.IBN.Business.Common.StringToArrayList(value);
			string values = String.Empty;
			foreach (int i in list)
				values += Mediachase.IBN.Business.Common.GetGeneralCategory(i) + "<br />";
			return values;
		}
		#endregion

		#region GetIncidentCategories
		private string GetIncidentCategories(string value)
		{
			ArrayList list = Mediachase.IBN.Business.Common.StringToArrayList(value);
			string values = String.Empty;
			foreach (int i in list)
				values += Mediachase.IBN.Business.Common.GetIncidentCategory(i) + "<br />";
			return values;
		}
		#endregion

		#region GetProjectCategories
		private string GetProjectCategories(string value)
		{
			ArrayList list = Mediachase.IBN.Business.Common.StringToArrayList(value);
			string values = String.Empty;
			foreach (int i in list)
				values += Mediachase.IBN.Business.Common.GetProjectCategory(i) + "<br />";
			return values;
		}
		#endregion

		#region GetBool
		private string GetBool(string value)
		{
			if (!String.IsNullOrEmpty(value))
			{
				if (bool.Parse(value))
					return GetGlobalResourceObject("IbnFramework.Admin", "TrueValue").ToString();
				else
					return GetGlobalResourceObject("IbnFramework.Admin", "FalseValue").ToString();
			}
			return String.Empty;
		}
		#endregion
	}
}