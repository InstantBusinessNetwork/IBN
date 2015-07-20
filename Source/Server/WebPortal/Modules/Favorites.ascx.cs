namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Resources;
	using System.Globalization;

	/// <summary>
	///		Summary description for Favorites.
	/// </summary>
	public partial class Favorites : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strActive", typeof(Favorites).Assembly);
		private UserLightPropertyCollection pc =  Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindSepAndPanels();
			if (!IsPostBack)
			{
				BindVisibility();
			}
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
			this.dgProjects.DeleteCommand +=new DataGridCommandEventHandler(dgProjects_DeleteCommand);
			this.dgEvents.DeleteCommand +=new DataGridCommandEventHandler(dgEvents_DeleteCommand);
			this.dgIncidents.DeleteCommand +=new DataGridCommandEventHandler(dgIncidents_DeleteCommand);
			this.dgLists.DeleteCommand +=new DataGridCommandEventHandler(dgLists_DeleteCommand);
			this.dgTasks.DeleteCommand +=new DataGridCommandEventHandler(dgTasks_DeleteCommand);
			this.dgToDo.DeleteCommand +=new DataGridCommandEventHandler(dgToDo_DeleteCommand);
			this.dgDocuments.DeleteCommand +=new DataGridCommandEventHandler(dgDocuments_DeleteCommand);
			this.dgReports.DeleteCommand += new DataGridCommandEventHandler(dgReports_DeleteCommand);
			this.dgUsers.DeleteCommand += new DataGridCommandEventHandler(dgUsers_DeleteCommand);
			this.dgOrganizations.DeleteCommand += new DataGridCommandEventHandler(dgOrganizations_DeleteCommand);
			this.dgContacts.DeleteCommand += new DataGridCommandEventHandler(dgContacts_DeleteCommand);
			this.dgCalendarEvent.DeleteCommand += new DataGridCommandEventHandler(dgCalendarEvent_DeleteCommand);
		}
		#endregion

		#region BindSepAndPanels
		private void BindSepAndPanels()
		{
			Sep1.ControlledPanel = Pan1;
			Sep2.ControlledPanel = Pan2;
			Sep3.ControlledPanel = Pan3;
			Sep4.ControlledPanel = Pan4;
			Sep5.ControlledPanel = Pan5;
			Sep7.ControlledPanel = Pan7;
			Sep8.ControlledPanel = Pan8;
			Sep9.ControlledPanel = Pan9;
			Sep10.ControlledPanel = Pan10;
			Sep21.ControlledPanel = Pan21;
			Sep22.ControlledPanel = Pan22;
			Sep29.ControlledPanel = Pan29;

			Sep1.PCValue = "Favorites_sep1";
			Sep2.PCValue = "Favorites_sep2";
			Sep3.PCValue = "Favorites_sep3";
			Sep4.PCValue = "Favorites_sep4";
			Sep5.PCValue = "Favorites_sep5";
			Sep7.PCValue = "Favorites_sep7";
			Sep8.PCValue = "Favorites_sep8";
			Sep9.PCValue = "Favorites_sep9";
			Sep10.PCValue = "Favorites_sep10";
			Sep21.PCValue = "Favorites_sep21";
			Sep22.PCValue = "Favorites_sep22";
			Sep29.PCValue = "Favorites_sep29";
		}
		#endregion

		#region BindVisibility
		private void BindVisibility()
		{
			// TODO: вытащить видимость блоков из pc
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			BindDG();
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			if (Sep1.Visible)
				BindDGProjects();

			if (Sep2.Visible)
				BindDGTasks();

			if (Sep3.Visible)
				BindDGToDo();

			if (Sep4.Visible)
				BindDGEvents();

			if (Sep5.Visible)
				BindDGIncidents();

			if (Sep7.Visible)
				BindDGLists();

			if (Sep8.Visible)
				BindDGDocuments();

			if (Sep9.Visible)
				BindDGReports();

			if (Sep10.Visible)
				BindDGUsers();

			if (Sep21.Visible)
				BindDGOrganizations();

			if (Sep22.Visible)
				BindDGContacts();

			if (Sep29.Visible)
				BindDGCalendarEvents();
		}
		#endregion

		#region BindDGProjects
		private void BindDGProjects()
		{
			dgProjects.DataSource = Project.GetListFavoritesDT();
			dgProjects.DataBind();

			int RowCount = dgProjects.Items.Count;
			if (RowCount == 0)
			{
				Sep1.Visible = false;
				Pan1.Visible = false;
			}
			else
			{
				Sep1.Title = String.Format("{0} ({1})", LocRM.GetString("Projects"), RowCount);
			}
		}
		#endregion

		#region BindDGTasks
		private void BindDGTasks()
		{
			dgTasks.DataSource = Task.GetListFavoritesDT();
			dgTasks.DataBind();

			int RowCount = dgTasks.Items.Count;
			if (RowCount == 0)
			{
				Sep2.Visible = false;
				Pan2.Visible = false;
			}
			else
			{
				Sep2.Title = String.Format("{0} ({1})", LocRM.GetString("Tasks"), RowCount);
			}
		}
		#endregion

		#region BindDGToDo
		private void BindDGToDo()
		{
			dgToDo.DataSource = Mediachase.IBN.Business.ToDo.GetListFavoritesDT();
			dgToDo.DataBind();

			int RowCount = dgToDo.Items.Count;
			if (RowCount == 0)
			{
				Sep3.Visible = false;
				Pan3.Visible = false;
			}
			else
			{
				Sep3.Title = String.Format("{0} ({1})", LocRM.GetString("ToDos"), RowCount);
			}
		}
		#endregion

		#region BindDGEvents
		private void BindDGEvents()
		{
			dgEvents.DataSource = CalendarEntry.GetListFavoritesDT();
			dgEvents.DataBind();

			int RowCount = dgEvents.Items.Count;
			if (RowCount == 0)
			{
				Sep4.Visible = false;
				Pan4.Visible = false;
			}
			else
			{
				Sep4.Title = String.Format("{0} ({1})", LocRM.GetString("CalendarEntries"), RowCount);
			}
		}
		#endregion

		#region BindDGIncidents
		private void BindDGIncidents()
		{
			dgIncidents.DataSource = Incident.GetListFavoritesDT();
			dgIncidents.DataBind();

			int RowCount = dgIncidents.Items.Count;
			if (RowCount == 0)
			{
				Sep5.Visible = false;
				Pan5.Visible = false;
			}
			else
			{
				Sep5.Title = String.Format("{0} ({1})", LocRM.GetString("Incidents"), RowCount);
			}
		}
		#endregion

		#region BindDGLists
		private void BindDGLists()
		{
			dgLists.DataSource = ListInfoBus.GetListFavoritesDT();
			dgLists.DataBind();

			int RowCount = dgLists.Items.Count;
			if (RowCount == 0)
			{
				Sep7.Visible = false;
				Pan7.Visible = false;
			}
			else
			{
				Sep7.Title = String.Format("{0} ({1})", LocRM.GetString("Lists"), RowCount);
			}
		}
		#endregion

		#region BindDGDocuments
		private void BindDGDocuments()
		{
			dgDocuments.DataSource = Document.GetListFavoritesDT();
			dgDocuments.DataBind();

			int RowCount = dgDocuments.Items.Count;
			if (RowCount == 0)
			{
				Sep8.Visible = false;
				Pan8.Visible = false;
			}
			else
			{
				Sep8.Title = String.Format("{0} ({1})", LocRM.GetString("Documents"), RowCount);
			}
		}
		#endregion

		#region BindDGReports
		private void BindDGReports()
		{
			dgReports.DataSource = Common.GetListFavoritesDT(ObjectTypes.Report);
			dgReports.DataBind();

			int RowCount = dgReports.Items.Count;
			if (RowCount == 0)
			{
				Sep9.Visible = false;
				Pan9.Visible = false;
			}
			else
			{
				Sep9.Title = String.Format("{0} ({1})", LocRM.GetString("Reports"), RowCount);
			}
		}
		#endregion

		#region BindDGUsers
		private void BindDGUsers()
		{
			dgUsers.DataSource = Common.GetListFavoritesDT(ObjectTypes.User);
			dgUsers.DataBind();

			int RowCount = dgUsers.Items.Count;
			if (RowCount == 0)
			{
				Sep10.Visible = false;
				Pan10.Visible = false;
			}
			else
			{
				Sep10.Title = String.Format("{0} ({1})", LocRM.GetString("Users"), RowCount);
			}
		}
		#endregion

		#region BindDGOrganizations
		private void BindDGOrganizations()
		{
			dgOrganizations.DataSource = Common.GetListFavoritesDT(ObjectTypes.Organization);
			dgOrganizations.DataBind();

			int RowCount = dgOrganizations.Items.Count;
			if (RowCount == 0)
			{
				Sep21.Visible = false;
				Pan21.Visible = false;
			}
			else
			{
				Sep21.Title = String.Format("{0} ({1})", LocRM.GetString("Organizations"), RowCount);
			}
		}
		#endregion

		#region BindDGContacts
		private void BindDGContacts()
		{
			dgContacts.DataSource = Common.GetListFavoritesDT(ObjectTypes.Contact);
			dgContacts.DataBind();

			int RowCount = dgContacts.Items.Count;
			if (RowCount == 0)
			{
				Sep22.Visible = false;
				Pan22.Visible = false;
			}
			else
			{
				Sep22.Title = String.Format("{0} ({1})", LocRM.GetString("Contacts"), RowCount);
			}
		}
		#endregion

		#region BindDGCalendarEvents
		private void BindDGCalendarEvents()
		{
			dgCalendarEvent.DataSource = Common.GetListFavoritesDT(ObjectTypes.CalendarEvent);
			dgCalendarEvent.DataBind();

			int RowCount = dgCalendarEvent.Items.Count;
			if (RowCount == 0)
			{
				Sep29.Visible = false;
				Pan29.Visible = false;
			}
			else
			{
				Sep29.Title = String.Format("{0} ({1})", LocRM.GetString("CalendarEvents"), RowCount);
			}
		}
		#endregion

		#region Delete event handlers
		private void dgProjects_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int ObjectId = int.Parse(e.CommandArgument.ToString());
			Project.DeleteFavorites(ObjectId);
		}

		private void dgEvents_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int ObjectId = int.Parse(e.CommandArgument.ToString());
			CalendarEntry.DeleteFavorites(ObjectId);
		}

		private void dgIncidents_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int ObjectId = int.Parse(e.CommandArgument.ToString());
			Incident.DeleteFavorites(ObjectId);
		}

		private void dgLists_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int ObjectId = int.Parse(e.CommandArgument.ToString());
			ListInfoBus.DeleteFavorites(ObjectId);
		}

		private void dgTasks_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int ObjectId = int.Parse(e.CommandArgument.ToString());
			Task.DeleteFavorites(ObjectId);
		}

		private void dgToDo_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int ObjectId = int.Parse(e.CommandArgument.ToString());
			ToDo.DeleteFavorites(ObjectId);
		}

		private void dgDocuments_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int ObjectId = int.Parse(e.CommandArgument.ToString());
			Document.DeleteFavorites(ObjectId);
		}

		void dgReports_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int objectId = int.Parse(e.CommandArgument.ToString());
			Common.DeleteFavorites(objectId, ObjectTypes.Report);
		}

		void dgUsers_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int objectId = int.Parse(e.CommandArgument.ToString());
			Common.DeleteFavorites(objectId, ObjectTypes.User);
		}

		void dgOrganizations_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			string objectUid = e.CommandArgument.ToString();
			Common.DeleteFavoritesByUid(new Guid(objectUid), ObjectTypes.Organization);
		}

		void dgContacts_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			string objectUid = e.CommandArgument.ToString();
			Common.DeleteFavoritesByUid(new Guid(objectUid), ObjectTypes.Contact);
		}

		void dgCalendarEvent_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			string objectUid = e.CommandArgument.ToString();
			Common.DeleteFavoritesByUid(new Guid(objectUid), ObjectTypes.CalendarEvent);
		}
		#endregion
	}
}
