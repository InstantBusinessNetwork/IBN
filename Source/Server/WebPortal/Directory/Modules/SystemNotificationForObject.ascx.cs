namespace Mediachase.UI.Web.Directory.Modules
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
	using Mediachase.Ibn.Lists;
	using System.Reflection;

	/// <summary>
	///		Summary description for SystemNotificationForObject.
	/// </summary>
	public partial class SystemNotificationForObject : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMessageTemplates", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(SystemNotificationForObject).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		#region ObjectTypeId
		private int objectTypeId = -1;
		private int ObjectTypeId
		{
			get 
			{
				if (objectTypeId < 0 && !String.IsNullOrEmpty(Request["ObjectTypeId"]))
					objectTypeId = int.Parse(Request["ObjectTypeId"]);
				return objectTypeId;
			}
			set
			{
				objectTypeId = value;
			}
		}
		#endregion

		#region ObjectId
		private int objectId = -1;
		private int ObjectId
		{
			get
			{
				if (objectId < 0 && !String.IsNullOrEmpty(Request["ObjectId"]))
					objectId = int.Parse(Request["ObjectId"]);
				return objectId;
			}
			set
			{
				objectId = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// List patch
			if (!String.IsNullOrEmpty(Request["ClassName"]))
			{
				string className = Request["ClassName"];
				ListInfo li = ListManager.GetListInfoByMetaClassName(className);
				ObjectTypeId = (int)ObjectTypes.List;
				ObjectId = li.PrimaryKeyId.Value;
			}
			//

			if (!IsPostBack)
				BindLabels();

			DefineDGStructure();

			secHeader.Title = LocRM2.GetString("tEventNotification");
			secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("Back"), 
				Util.CommonHelper.GetObjectLink(ObjectTypeId, ObjectId));
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
			grdMain.EditCommand +=new DataGridCommandEventHandler(grdMain_EditCommand);
			grdMain.CancelCommand +=new DataGridCommandEventHandler(grdMain_CancelCommand);
			grdMain.UpdateCommand +=new DataGridCommandEventHandler(grdMain_UpdateCommand);
			grdMain.ItemCommand +=new DataGridCommandEventHandler(grdMain_ItemCommand);
		}
		#endregion

		#region BindLabels
		private void BindLabels()
		{
			lblObjectType.Text = Util.CommonHelper.GetObjectTypeName(ObjectTypeId);
			lnkObject.Text = Util.CommonHelper.GetObjectLinkAndTitle(ObjectTypeId, ObjectId);
		}
		#endregion

		#region DefineDGStructure
		private void DefineDGStructure()
		{
			DataTable source = SystemEvents.GetPersonalSubscriptionsForObjectDT(ObjectTypeId, ObjectId);

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
			titleCol.ReadOnly = true;
			grdMain.Columns.Add(titleCol);

			foreach (DataColumn roleCol in source.Columns)
			{
				if (!roleCol.ColumnName.ToLower().StartsWith("role_"))
					continue;

				TemplateColumn imageCol = new TemplateColumn();
				imageCol.HeaderStyle.Width = Unit.Pixel(75);
				imageCol.ItemStyle.Width = Unit.Pixel(75);
				imageCol.ItemTemplate = new DataGridImageTemplate(roleCol.ColumnName);
				imageCol.EditItemTemplate = new DataGridCheckBoxTemplate(roleCol.ColumnName);
				imageCol.HeaderStyle.CssClass = "ibn-vh3";
				imageCol.ItemStyle.CssClass = "ibn-vb2";
				imageCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
				imageCol.HeaderText = LocRM.GetString(roleCol.ColumnName + "_singular");
				grdMain.Columns.Add(imageCol);
			}				

			TemplateColumn buttonCol = new TemplateColumn();
			buttonCol.HeaderStyle.Width = Unit.Pixel(50);
			buttonCol.ItemStyle.Width = Unit.Pixel(50);
			buttonCol.ItemTemplate = new DataGridActionsTemplate(LocRM.GetString("RestoreDefaults"), LocRM.GetString("Edit"));
			buttonCol.EditItemTemplate = new DataGridEditActionsTemplate(LocRM.GetString("Save"), LocRM.GetString("Cancel"));
			buttonCol.HeaderStyle.CssClass = "ibn-vh3";
			buttonCol.ItemStyle.CssClass = "ibn-vb2";
			buttonCol.HeaderText = "";
			grdMain.Columns.Add(buttonCol);

			BindData(source);
		}
		#endregion

		#region BindData
		private void BindData()
		{
			BindData(SystemEvents.GetPersonalSubscriptionsForObjectDT(ObjectTypeId, ObjectId));
		}

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

		#region grdMain_EditCommand
		private void grdMain_EditCommand(object source, DataGridCommandEventArgs e)
		{
			grdMain.EditItemIndex = e.Item.ItemIndex;
			BindData();
		}
		#endregion

		#region grdMain_UpdateCommand
		private void grdMain_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			int EventTypeId = int.Parse(e.CommandArgument.ToString());

			ArrayList selectedValues = new ArrayList();
			for (int i = 2; i < e.Item.Cells.Count; i++)
			{
				TableCell cell = e.Item.Cells[i];

				foreach (Control ctrl in cell.Controls)
				{
					CheckBox chk = ctrl as CheckBox;
					if (chk != null && chk.Checked)
						selectedValues.Add(int.Parse(chk.ID.Substring(5)));
				}
			}

			SystemEvents.AddPersonalSubscriptionForObject(EventTypeId, ObjectId, selectedValues);

			grdMain.EditItemIndex = -1;
			BindData();
		}
		#endregion

		#region grdMain_CancelCommand
		private void grdMain_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			grdMain.EditItemIndex = -1;
			BindData();
		}
		#endregion

		#region grdMain_ItemCommand
		private void grdMain_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Restore")
			{
				int EventTypeId = int.Parse(e.CommandArgument.ToString());
				SystemEvents.DeletePersonalSubscriptionForObject(EventTypeId, ObjectId);
			}

			BindData();
		}
		#endregion

		#region Class DataGridImageTemplate
		protected class DataGridImageTemplate : ITemplate
		{
			private string roleId;

			public DataGridImageTemplate(string RoleId)
			{
				roleId = RoleId;
			}

			public void InstantiateIn(System.Web.UI.Control container)
			{
				System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
				img.Width = Unit.Pixel(16);
				img.Height = Unit.Pixel(16);
				img.ImageUrl = "~/layouts/images/accept.gif";
				img.ID = roleId;
				img.DataBinding +=new EventHandler(img_DataBinding);
				container.Controls.Add(img);
			}

			private void img_DataBinding(object sender, EventArgs e)
			{
				System.Web.UI.WebControls.Image img = (System.Web.UI.WebControls.Image)sender;
				DataGridItem container = (DataGridItem)img.NamingContainer;
				img.Visible = (bool)DataBinder.Eval(container.DataItem, img.ID);
				if (!(bool)DataBinder.Eval(container.DataItem, "IsDefault"))
				{
					if ((bool)DataBinder.Eval(container.DataItem, img.ID))	// if checked
						img.ImageUrl = "~/layouts/images/accept_1.gif";
					else
						img.ImageUrl = "~/layouts/images/deny_1.gif";
				}
			}
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
				cb.DataBinding +=new EventHandler(cb_DataBinding);
				container.Controls.Add(cb);
			}

			private void cb_DataBinding(object sender, EventArgs e)
			{
				CheckBox cb = (CheckBox)sender;
				DataGridItem container = (DataGridItem)cb.NamingContainer;
				cb.Checked = (bool)DataBinder.Eval(container.DataItem, cb.ID);
				//				cb.Enabled = !(bool)DataBinder.Eval(container.DataItem, "IsDefault");
			}
		}
		#endregion

		#region Class DataGridActionsTemplate
		protected class DataGridActionsTemplate : ITemplate
		{
			private string restoreTitle;
			private string editTitle;

			public DataGridActionsTemplate(string RestoreTitle, string EditTitle)
			{
				restoreTitle = RestoreTitle;
				editTitle = EditTitle;
			}

			public void InstantiateIn(System.Web.UI.Control container)
			{
				// Edit
				ImageButton btn1 = new ImageButton();
				btn1.BorderWidth = Unit.Pixel(0);
				btn1.Width = Unit.Pixel(16);
				btn1.Height = Unit.Pixel(16);
				btn1.ImageUrl = "~/layouts/images/edit.gif";
				btn1.CommandName =	"Edit";
				btn1.ToolTip = editTitle;
				container.Controls.Add(btn1);

				LiteralControl lc1 = new LiteralControl("&nbsp;");
				container.Controls.Add(lc1);

				// Restore defaults
				ImageButton btn2 = new ImageButton();
				btn2.BorderWidth = Unit.Pixel(0);
				btn2.Width = Unit.Pixel(16);
				btn2.Height = Unit.Pixel(16);
				btn2.ImageUrl = "~/layouts/images/import.gif";
				btn2.CommandName =	"Restore";
				btn2.ToolTip = restoreTitle;
				btn2.DataBinding += new EventHandler(btn2_DataBinding);
				container.Controls.Add(btn2);
			}

			private void btn2_DataBinding(object sender, EventArgs e)
			{
				ImageButton btn = (ImageButton)sender;
				DataGridItem container = (DataGridItem)btn.NamingContainer;
				btn.CommandArgument = DataBinder.Eval(container.DataItem, "EventTypeId").ToString();
				btn.Visible = !(bool)DataBinder.Eval(container.DataItem, "IsDefault");
			}
		}
		#endregion

		#region Class DataGridEditActionsTemplate
		protected class DataGridEditActionsTemplate : ITemplate
		{
			private string saveTitle;
			private string cancelTitle;

			public DataGridEditActionsTemplate(string SaveTitle, string CancelTitle)
			{
				saveTitle = SaveTitle;
				cancelTitle = CancelTitle;
			}

			public void InstantiateIn(System.Web.UI.Control container)
			{
				// Save
				ImageButton btn1 = new ImageButton();
				btn1.BorderWidth = Unit.Pixel(0);
				btn1.Width = Unit.Pixel(16);
				btn1.Height = Unit.Pixel(16);
				btn1.ImageUrl = "~/layouts/images/Saveitem.gif";
				btn1.CommandName =	"Update";
				btn1.ToolTip = saveTitle;
				btn1.DataBinding += new EventHandler(btn1_DataBinding);
				container.Controls.Add(btn1);

				LiteralControl lc1 = new LiteralControl("&nbsp;");
				container.Controls.Add(lc1);
				
				// Cancel
				ImageButton btn2 = new ImageButton();
				btn2.BorderWidth = Unit.Pixel(0);
				btn2.Width = Unit.Pixel(16);
				btn2.Height = Unit.Pixel(16);
				btn2.ImageUrl = "~/layouts/images/cancel.GIF";
				btn2.CommandName =	"Cancel";
				btn2.ToolTip = cancelTitle;
				container.Controls.Add(btn2);
			}

			private void btn1_DataBinding(object sender, EventArgs e)
			{
				ImageButton btn = (ImageButton)sender;
				DataGridItem container = (DataGridItem)btn.NamingContainer;
				btn.CommandArgument = DataBinder.Eval(container.DataItem, "EventTypeId").ToString();
			}
		}
		#endregion
	}
}
