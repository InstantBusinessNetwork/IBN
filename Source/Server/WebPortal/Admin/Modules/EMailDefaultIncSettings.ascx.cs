namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using System.Reflection;

	/// <summary>
	///		Summary description for EMailDefaultIncSettings.
	/// </summary>
	public partial class EMailDefaultIncSettings : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				BindData();
				BindValues();
			}
			BindToolbars();
			imbSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			imbCancel.CustomImage = this.Page.ResolveUrl("~/layouts/images/cancel.gif");
		}

		#region BindData
		private void BindData()
		{
			ucTaskTime.Value = DateTime.MinValue;

			ddType.DataSource = Incident.GetListIncidentTypes();
			ddType.DataTextField = "TypeName";
			ddType.DataValueField = "TypeId";
			ddType.DataBind();

			ddSeverity.DataSource = Incident.GetListIncidentSeverity();
			ddSeverity.DataTextField = "SeverityName";
			ddSeverity.DataValueField = "SeverityId";
			ddSeverity.DataBind();

			ddPriority.DataSource = Incident.GetListPriorities();
			ddPriority.DataTextField = "PriorityName";
			ddPriority.DataValueField = "PriorityId";
			ddPriority.DataBind();

			ddProject.DataSource = Incident.GetListProjects();
			ddProject.DataTextField = "Title";
			ddProject.DataValueField = "ProjectId";
			ddProject.DataBind();
			ddProject.Items.Insert(0, new ListItem(LocRM.GetString("tNotSet"), "0"));

			ddCreator.DataSource = SecureGroup.GetListAllActiveUsersInGroup((int)InternalSecureGroups.Intranet, false);
			ddCreator.DataTextField = "DisplayName";
			ddCreator.DataValueField = "UserId";
			ddCreator.DataBind();

			ddManager.DataSource = SecureGroup.GetListAllActiveUsersInGroup((int)InternalSecureGroups.Intranet, false);
			ddManager.DataTextField = "DisplayName";
			ddManager.DataValueField = "UserId";
			ddManager.DataBind();
			ddManager.Items.Insert(0, new ListItem(
				Util.CommonHelper.GetGroupPureName((int)InternalSecureGroups.HelpDeskManager),
				((int)InternalSecureGroups.HelpDeskManager).ToString()
				));
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			DefaultIncidentField dif = DefaultIncidentField.Load();
			Util.CommonHelper.SafeSelect(ddCreator, dif.CreatorId.ToString());
			Util.CommonHelper.SafeSelect(ddManager, dif.ManagerId.ToString());
			Util.CommonHelper.SafeSelect(ddPriority, dif.PriorityId.ToString());
			Util.CommonHelper.SafeSelect(ddProject, dif.ProjectId.ToString());
			Util.CommonHelper.SafeSelect(ddSeverity, dif.SeverityId.ToString());
			Util.CommonHelper.SafeSelect(ddType, dif.TypeId.ToString());
			ucTaskTime.Value = DateTime.MinValue.AddMinutes(dif.TaskTime);
		}
		#endregion

		#region BindToolbars
		private void BindToolbars()
		{
			imbSave.Text = LocRM.GetString("tSave");
			imbCancel.Text = LocRM.GetString("tCancel");
			secHeader.Title = LocRM.GetString("tDefIncSettings");
		}
		#endregion

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
			this.imbSave.ServerClick += new EventHandler(imbSave_ServerClick);
			this.imbCancel.ServerClick += new EventHandler(imbCancel_ServerClick);
		}
		#endregion

		private void imbCancel_ServerClick(object sender, EventArgs e)
		{
			Response.Redirect("~/Admin/EMailPop3Boxes.aspx");
		}

		private void imbSave_ServerClick(object sender, EventArgs e)
		{
			DefaultIncidentField dif = DefaultIncidentField.Load();
			dif.CreatorId = int.Parse(ddCreator.SelectedValue);
			dif.ManagerId = int.Parse(ddManager.SelectedValue);
			dif.TypeId = int.Parse(ddType.SelectedValue);
			dif.SeverityId = int.Parse(ddSeverity.SelectedValue);
			dif.PriorityId = int.Parse(ddPriority.SelectedValue);
			dif.ProjectId = int.Parse(ddProject.SelectedValue);
			TimeSpan ts = new TimeSpan(ucTaskTime.Value.Ticks);
			dif.TaskTime = (int)ts.TotalMinutes;
			DefaultIncidentField.Update(dif);
			Response.Redirect("~/Admin/EMailPop3Boxes.aspx");
		}
	}
}
