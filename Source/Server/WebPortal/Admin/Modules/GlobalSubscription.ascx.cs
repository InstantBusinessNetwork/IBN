namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using System.Resources;
	using System.Globalization;
	using Mediachase.IBN.Business;
	using System.Reflection;

	/// <summary>
	///		Summary description for GlobalSubscription.
	/// </summary>
	public partial class GlobalSubscription : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMessageTemplates", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(GlobalSubscription).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(GlobalSubscription).Assembly);
		protected ResourceManager LocRM4 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				BindTypes();
				if (pc["GlobalSubscription_ObjectType"] == null)
					pc["GlobalSubscription_ObjectType"] = ((int)ObjectTypes.Project).ToString();

				Util.CommonHelper.SafeSelect(ddType, pc["GlobalSubscription_ObjectType"]);
			}

			DefineDGStructure();

			secHeader.Title = LocRM2.GetString("tEventNotification");
			secHeader.AddLink("<img alt='' src='" + this.Page.ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM4.GetString("tRoutingWorkflow"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin4"));
			btnSave.Text = LocRM3.GetString("tbsave_save");
			btnCancel.Text = LocRM3.GetString("tbsave_cancel");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnCancel.CustomImage = this.Page.ResolveUrl("~/layouts/images/deny.gif");
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			ddType.SelectedIndexChanged += new EventHandler(ddType_SelectedIndexChanged);
			btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
			btnCancel.ServerClick += new EventHandler(btnCancel_ServerClick);
		}
		#endregion

		#region BindTypes
		private void BindTypes()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Text", typeof(string)));
			dt.Columns.Add(new DataColumn("Value", typeof(int)));
			if (Configuration.ProjectManagementEnabled)
			{
				AddRow(dt, LocRM.GetString("Project"), (int)ObjectTypes.Project);
				AddRow(dt, LocRM.GetString("Task"), (int)ObjectTypes.Task);
			}
			AddRow(dt, LocRM.GetString("Todo"), (int)ObjectTypes.ToDo);
			AddRow(dt, LocRM.GetString("CalendarEntry"), (int)ObjectTypes.CalendarEntry);
			if (Configuration.HelpDeskEnabled)
				AddRow(dt, LocRM.GetString("Issue"), (int)ObjectTypes.Issue);
			AddRow(dt, LocRM.GetString("Document"), (int)ObjectTypes.Document);
			AddRow(dt, LocRM.GetString("List"), (int)ObjectTypes.List);
			//AddRow(dt, LocRM.GetString("IssueRequest"), (int)ObjectTypes.IssueRequest);
			AddRow(dt, LocRM.GetString("User"), (int)ObjectTypes.User);
			if (Configuration.WorkflowModule)
				AddRow(dt, LocRM.GetString("Assignment"), (int)ObjectTypes.Assignment);

			DataView dv = dt.DefaultView;
			dv.Sort = "Text";
			ddType.DataSource = dv;
			ddType.DataTextField = "Text";
			ddType.DataValueField = "Value";
			ddType.DataBind();
		}
		private void AddRow(DataTable dt, string Text, int Value)
		{
			DataRow dr = dt.NewRow();
			dr["Text"] = Text;
			dr["Value"] = Value;
			dt.Rows.Add(dr);
		}
		#endregion

		#region DefineDGStructure
		private void DefineDGStructure()
		{
			int ObjectTypeId = int.Parse(pc["GlobalSubscription_ObjectType"]);
			DataTable source = SystemEvents.GetGlobalSubscriptionsDT(ObjectTypeId);

			// Data Grid definition
			grdMain.Columns.Clear();

			BoundColumn idCol = new BoundColumn();
			idCol.DataField = "EventTypeId";
			idCol.Visible = false;
			grdMain.Columns.Add(idCol);

			BoundColumn titleCol = new BoundColumn();
			titleCol.DataField = "Title";
			titleCol.HeaderStyle.CssClass = "ibn-vh2";
			titleCol.ItemStyle.CssClass = "ibn-vb2";
			titleCol.HeaderText = LocRM.GetString("SystemEvent");
			grdMain.Columns.Add(titleCol);

			foreach (DataColumn roleCol in source.Columns)
			{
				if (!roleCol.ColumnName.ToLower().StartsWith("role_"))
					continue;

				TemplateColumn checkBoxCol = new TemplateColumn();
				checkBoxCol.HeaderStyle.Width = Unit.Pixel(75);
				checkBoxCol.ItemStyle.Width = Unit.Pixel(75);
				checkBoxCol.ItemTemplate = new DataGridCheckBoxTemplate(roleCol.ColumnName);
				checkBoxCol.HeaderStyle.CssClass = "ibn-vh2";
				checkBoxCol.ItemStyle.CssClass = "ibn-vb2";
				checkBoxCol.HeaderText = LocRM.GetString(roleCol.ColumnName);
				grdMain.Columns.Add(checkBoxCol);
			}
			BindData(source);
		}
		#endregion

		#region BindData
		private void BindData(DataTable source)
		{
			foreach (DataRow dr in source.Rows)
			{
				string s = SystemEvents.GetSystemEventName(dr["Title"].ToString().Trim());
				if (s != null)
					dr["Title"] = s;
			}
			DataView dv = source.DefaultView;
			dv.Sort = "Title";
			grdMain.DataSource = dv;
			grdMain.DataBind();
		}
		#endregion

		#region ddType_SelectedIndexChanged
		private void ddType_SelectedIndexChanged(object sender, EventArgs e)
		{
			pc["GlobalSubscription_ObjectType"] = ddType.SelectedValue;

			DefineDGStructure();
		}
		#endregion

		#region btnSave_ServerClick
		private void btnSave_ServerClick(object sender, EventArgs e)
		{
			int ObjectTypeId = int.Parse(pc["GlobalSubscription_ObjectType"]);

			// —формируем массив выбранных элементов, представив их в виде строки EventTypeId_ObjectRoleId
			ArrayList selectedValues = new ArrayList();

			foreach (DataGridItem item in grdMain.Items)
			{
				string sEventTypeId = item.Cells[0].Text;

				for (int i = 2; i < item.Cells.Count; i++)
				{
					TableCell cell = item.Cells[i];

					foreach (Control ctrl in cell.Controls)
					{
						CheckBox chk = ctrl as CheckBox;
						if (chk != null && chk.Checked)
							selectedValues.Add(String.Format("{0}_{1}", sEventTypeId, chk.ID.Substring(5)));
					}
				}
			}

			SystemEvents.AddGlobalSubscriptions(ObjectTypeId, selectedValues);
		}
		#endregion

		#region btnCancel_ServerClick
		private void btnCancel_ServerClick(object sender, EventArgs e)
		{
			int ObjectTypeId = int.Parse(pc["GlobalSubscription_ObjectType"]);
			BindData(SystemEvents.GetGlobalSubscriptionsDT(ObjectTypeId));
		}
		#endregion

		#region Class DataGridCheckBoxTemplate
		protected class DataGridCheckBoxTemplate : ITemplate
		{
			private string roleId;

			public DataGridCheckBoxTemplate(string RoleId)
			{
				roleId = RoleId;
			}

			public void InstantiateIn(System.Web.UI.Control container)
			{
				CheckBox cb = new CheckBox();
				cb.ID = roleId;
				cb.DataBinding += new EventHandler(cb_DataBinding);
				container.Controls.Add(cb);
			}

			private void cb_DataBinding(object sender, EventArgs e)
			{
				CheckBox cb = (CheckBox)sender;
				DataGridItem container = (DataGridItem)cb.NamingContainer;
				cb.Checked = (bool)DataBinder.Eval(container.DataItem, cb.ID);
			}
		}
		#endregion
	}
}
