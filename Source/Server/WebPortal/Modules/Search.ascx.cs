namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Text;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Resources;
	using System.Globalization;
	using Mediachase.IBN.Business.ControlSystem;
	using Mediachase.Ibn.Lists;
	using Mediachase.Ibn.Core.Business;
	using Mediachase.Ibn.Clients;
	using Mediachase.Ibn.Web.UI;
	using Mediachase.Ibn.Data;
	using Mediachase.IBN.Business.WebDAV.Common;

	/// <summary>
	///		Summary description for Search.
	/// </summary>
	public partial class Search : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strActive", typeof(Search).Assembly);
		private UserLightPropertyCollection pc =  Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindSepAndPanels();
			if (!IsPostBack)
			{
				if (pc["Search_String"] != null)
					tbSearchstr.Text = pc["Search_String"];
				BindInVisibility();
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
			this.btnSearch.Click +=new System.Web.UI.ImageClickEventHandler(btnSearch_Click);
		}
		#endregion

		#region BindSepAndPanels
		private void BindSepAndPanels()
		{
			Sep1.ControlledPanel = Pan1;
			Sep2.ControlledPanel = Pan2;
			Sep4.ControlledPanel = Pan4;
			Sep5.ControlledPanel = Pan5;
			Sep6.ControlledPanel = Pan6;
			Sep7.ControlledPanel = Pan7;
			Sep8.ControlledPanel = Pan8;
			Sep9.ControlledPanel = Pan9;
			Sep10.ControlledPanel = Pan10;
			Sep11.ControlledPanel = Pan11;
			Sep12.ControlledPanel = Pan12;

			Sep1.PCValue = "Search_sep1";
			Sep2.PCValue = "Search_sep2";
			Sep4.PCValue = "Search_sep4";
			Sep5.PCValue = "Search_sep5";
			Sep6.PCValue = "Search_sep6";
			Sep7.PCValue = "Search_sep7";
			Sep8.PCValue = "Search_sep8";
			Sep9.PCValue = "Search_sep9";
			Sep10.PCValue = "Search_sep10";
			Sep11.PCValue = "Search_sep11";
			Sep12.PCValue = "Search_sep12";
		}
		#endregion

		#region BindVisibility
		private void BindVisibility()
		{
			// TODO: вытащить видимость блоков из pc
			Sep1.Visible = true;
			Pan1.Visible = true;

			Sep2.Visible = true;
			Pan2.Visible = true;

			Sep4.Visible = true;
			Pan4.Visible = true;

			Sep5.Visible = true;
			Pan5.Visible = true;

			Sep6.Visible = true;
			Pan6.Visible = true;

			Sep7.Visible = true;
			Pan7.Visible = true;

			Sep8.Visible = true;
			Pan8.Visible = true;

			Sep9.Visible = true;
			Pan9.Visible = true;

			Sep10.Visible = true;
			Pan10.Visible = true;

			Sep11.Visible = true;
			Pan11.Visible = true;

			Sep12.Visible = true;
			Pan12.Visible = true;
		}
		#endregion

		#region BindInVisibility
		private void BindInVisibility()
		{
			Sep1.Visible = false;
			Pan1.Visible = false;

			Sep2.Visible = false;
			Pan2.Visible = false;

			Sep4.Visible = false;
			Pan4.Visible = false;

			Sep5.Visible = false;
			Pan5.Visible = false;

			Sep6.Visible = false;
			Pan6.Visible = false;

			Sep7.Visible = false;
			Pan7.Visible = false;

			Sep8.Visible = false;
			Pan8.Visible = false;

			Sep9.Visible = false;
			Pan9.Visible = false;

			Sep10.Visible = false;
			Pan10.Visible = false;

			Sep11.Visible = false;
			Pan11.Visible = false;

			Sep12.Visible = false;
			Pan12.Visible = false;
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			if (pc["Search_String"] != null && pc["Search_String"] != "")
			{
				string Keyword = HttpUtility.HtmlEncode(pc["Search_String"]);

				if (Sep1.Visible)
					BindDGProjects(Keyword);

				if (Sep2.Visible)
					BindDGTasks(Keyword);

				if (Sep4.Visible)
					BindDGEvents(Keyword);

				if (Sep5.Visible)
					BindDGIncidents(Keyword);

				if (Sep6.Visible)
					BindDGFiles(Keyword);

				if (Sep7.Visible)
					BindDGLists(Keyword);

				if (Sep8.Visible)
					BindDGUsers(Keyword);

				if (Sep9.Visible)
					BindDGGroups(Keyword);

				if (Sep10.Visible)
					BindDGDocuments(Keyword);

				if (Sep11.Visible)
					BindDGOrganizations(Keyword);

				if (Sep12.Visible)
					BindDGContacts(Keyword);
			}
			else
				BindInVisibility();
		}
		#endregion

		#region BindDGProjects
		private void BindDGProjects(string Keyword)
		{
			///	 ProjectId, ManagerId, Title, Description, StatusId, PercentCompleted, 
			///	 StartDate, FinishDate, TargetFinishDate, ActualFinishDate
			dgProjects.DataSource = Project.GetListProjectsByKeyword(Keyword);
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
		private void BindDGTasks(string Keyword)
		{
			///		ItemId, Title, Description, ManagerId, IsCompleted, IsToDo, CompletionTypeId,
			///		StartDate, FinishDate, ReasonId
			dgTasks.DataSource = ToDo.GetListToDoAndTasksByKeyword(Keyword);
			dgTasks.DataBind();

			int RowCount = dgTasks.Items.Count;
			if (RowCount == 0)
			{
				Sep2.Visible = false;
				Pan2.Visible = false;
			}
			else
			{
				Sep2.Title = String.Format("{0} ({1})", LocRM.GetString("ToDoTasks"), RowCount);
			}
		}
		#endregion

		#region BindDGEvents
		private void BindDGEvents(string Keyword)
		{
			// EventId, Title, Description, Location, TypeId, StartDate, FinishDate
			dgEvents.DataSource = CalendarEntry.GetListEventsByKeyword(Keyword);
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
		private void BindDGIncidents(string Keyword)
		{
			/// IncidentId, CreatorId, ManagerId, Title, Description, PriorityId, PriorityName, 
			/// CreationDate
			int TicketUid = Mediachase.IBN.Business.EMail.TicketUidUtil.GetThreadId(Keyword);
			if(TicketUid!=-1)
				Keyword = TicketUid.ToString();

			dgIncidents.DataSource = Incident.GetListIncidentsByKeyword(Keyword);
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

		#region BindDGFiles
		private void BindDGFiles(string Keyword)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Title", typeof(string)));
			dt.Columns.Add(new DataColumn("_Href", typeof(string)));
			dt.Columns.Add(new DataColumn("_New", typeof(int)));
			dt.Columns.Add(new DataColumn("Location", typeof(string)));
			DataRow dr;

			FileInfo[] _fi = Mediachase.IBN.Business.ControlSystem.FileStorage.SearchFiles(Security.CurrentUser.UserID, -1, -1, Keyword, -1, DateTime.MinValue, DateTime.MinValue, -1, -1);
			foreach (FileInfo fi in _fi)
			{
				dr = dt.NewRow();
				dr["Id"] = fi.Id;
				dr["Title"] = Util.CommonHelper.GetShortFileName(fi.Name, 40);
				string sLink = "";
				if (fi.FileBinaryContentType.ToLower().IndexOf("url") >= 0)
					sLink = GetLinkText(fi);
				if (sLink == "")
				  //sLink = Util.CommonHelper.GetAbsolutePath(Mediachase.IBN.Business.ControlSystem.WebDavFileUserTicket.GetDownloadPath(fi.Id, fi.Name));
					sLink = WebDavUrlBuilder.GetFileStorageWebDavUrl(fi, true);
				dr["_Href"] = sLink;
				dr["_New"] = Common.OpenInNewWindow(fi.FileBinaryContentType) ? 1 : 0;


				string containerKey = fi.ContainerKey;
				BaseIbnContainer bic = BaseIbnContainer.Create("FileLibrary", containerKey);
				Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");
				string parentName = "";
				string parentLink = "";
				Util.CommonHelper.GetParentContainer(containerKey, fi.ParentDirectory, out parentName, out parentLink);
				dr["Location"] = parentName;

				dt.Rows.Add(dr);
			}

			dgFiles.DataSource = dt.DefaultView;
			dgFiles.DataBind();

			int RowCount = dgFiles.Items.Count;
			if (RowCount == 0)
			{
				Sep6.Visible = false;
				Pan6.Visible = false;
			}
			else
			{
				Sep6.Title = String.Format("{0} ({1})", LocRM.GetString("Files"), RowCount);
			}
		}
		#endregion

		#region BindDGLists
		private void BindDGLists(string Keyword)
		{
			dgLists.DataSource = ListManager.GetLists(Keyword);
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

		#region BindDGUsers
		private void BindDGUsers(string Keyword)
		{
			// UserId, FirstName, LastName, Email, Login, IsExternal, IsPending
			dgUsers.DataSource = User.GetListUsersBySubstring(Keyword);
			dgUsers.DataBind();

			int RowCount = dgUsers.Items.Count;
			if (RowCount == 0)
			{
				Sep8.Visible = false;
				Pan8.Visible = false;
			}
			else
			{
				Sep8.Title = String.Format("{0} ({1})", LocRM.GetString("Users"), RowCount);
			}
		}
		#endregion

		#region BindDGGroups
		private void BindDGGroups(string Keyword)
		{
			// GroupId, GroupName, HasChildren
			dgGroups.DataSource = SecureGroup.GetListGroupsBySubstring(Keyword);
			dgGroups.DataBind();

			int RowCount = dgGroups.Items.Count;
			if (RowCount == 0)
			{
				Sep9.Visible = false;
				Pan9.Visible = false;
			}
			else
			{
				Sep9.Title = String.Format("{0} ({1})", LocRM.GetString("Groups"), RowCount);
			}
		}
		#endregion

		#region BindDGDocuments
		private void BindDGDocuments(string Keyword)
		{
			/// DocumentId, CreatorId, ManagerId, Title, Description, PriorityId, PriorityName, 
			/// StatusId, StatusName, CreationDate
			dgDocuments.DataSource = Document.GetListDocumentsByKeyword(Keyword);
			dgDocuments.DataBind();

			int RowCount = dgDocuments.Items.Count;
			if (RowCount == 0)
			{
				Sep10.Visible = false;
				Pan10.Visible = false;
			}
			else
			{
				Sep10.Title = String.Format("{0} ({1})", LocRM.GetString("Documents"), RowCount);
			}
		}
		#endregion

		#region BindDGOrganizations
		private void BindDGOrganizations(string keyword)
		{
			FilterElement fe = CHelper.GetSearchFilterElementByKeyword(keyword, OrganizationEntity.GetAssignedMetaClassName());
			FilterElementCollection fec = new FilterElementCollection();
			fec.Add(fe);

			dgOrganizations.DataSource = BusinessManager.List(OrganizationEntity.GetAssignedMetaClassName(), fec.ToArray());
			dgOrganizations.DataBind();

			int RowCount = dgOrganizations.Items.Count;
			if (RowCount == 0)
			{ 
				Sep11.Visible = false;
				Pan11.Visible = false;
			}
			else
			{
				Sep11.Title = String.Format("{0} ({1})", LocRM.GetString("Organizations"), RowCount);
			}
		}
		#endregion

		#region BindDGContacts
		private void BindDGContacts(string keyword)
		{
			FilterElement fe = CHelper.GetSearchFilterElementByKeyword(keyword, ContactEntity.GetAssignedMetaClassName());
			FilterElementCollection fec = new FilterElementCollection();
			fec.Add(fe);


			dgContacts.DataSource = BusinessManager.List(ContactEntity.GetAssignedMetaClassName(), fec.ToArray());
			dgContacts.DataBind();

			int RowCount = dgContacts.Items.Count;
			if (RowCount == 0)
			{
				Sep12.Visible = false;
				Pan12.Visible = false;
			}
			else
			{
				Sep12.Title = String.Format("{0} ({1})", LocRM.GetString("Contacts"), RowCount);
			}
		}
		#endregion

		#region btnSearch_Click
		private void btnSearch_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			BindVisibility();
			if (tbSearchstr.Text.Trim().Length > 0)
				pc["Search_String"] = tbSearchstr.Text.Trim();
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			if(Page.IsPostBack)
				BindDG();
		}
		#endregion

		#region GetLinkText
		private string GetLinkText(FileInfo fi)
		{
			System.IO.MemoryStream memStream = new System.IO.MemoryStream();
			Mediachase.IBN.Business.ControlSystem.FileStorage.LightLoadFile(fi, memStream);
			memStream.Position = 0;
			System.IO.StreamReader reader = new System.IO.StreamReader(memStream, Encoding.Unicode);
			string data = reader.ReadLine();
			string sLink = "";
			while (data != null)
			{
				if (data.IndexOf("URL=") >= 0)
				{
					sLink = data.Substring(data.IndexOf("URL=") + 4);
					break;
				}
				data = reader.ReadLine();
			}
			if (sLink != "")
			{
				if (!sLink.StartsWith("http://") && !sLink.StartsWith("\\"))
					sLink = "http://" + sLink;
				if (sLink.StartsWith("\\"))
				{
					sLink = sLink.Replace(@"\", "/");
					sLink = "file:" + sLink;
				}
			}
			return sLink;
		}
		#endregion
	}
}
