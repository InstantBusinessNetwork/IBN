namespace Mediachase.UI.Web.Common
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	
	using Mediachase.IBN.Business;

	/// <summary>
	///		Summary description for Comments.
	/// </summary>
	public partial class Comments : System.Web.UI.UserControl
	{
		protected UserLightPropertyCollection pcCurrentUser;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Common.Resources.strComments", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		#region ProjID
		private int ProjID
		{
			get
			{
				try
				{
					return Request["ProjectID"] != null ? int.Parse(Request["ProjectID"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region TaskID
		private int TaskID
		{
			get
			{
				try
				{
					return Request["TaskID"] != null ? int.Parse(Request["TaskID"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region EventID
		private int EventID
		{
			get
			{
				try
				{
					return Request["EventID"] != null ? int.Parse(Request["EventID"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region ToDoID
		private int ToDoID
		{
			get
			{
				try
				{
					return Request["ToDoID"] != null ? int.Parse(Request["ToDoID"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region IncidentID
		private int IncidentID
		{
			get
			{
				try
				{
					return Request["IncidentID"] != null ? int.Parse(Request["IncidentID"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region DocumentID
		private int DocumentID
		{
			get
			{
				try
				{
					return Request["DocumentID"] != null ? int.Parse(Request["DocumentID"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region sType
		private string sType
		{
			get
			{
				if (ProjID > 0) return "Project";
				if (TaskID > 0) return "Task";
				if (ToDoID > 0) return "ToDo";
				if (EventID > 0) return "Event";
				if (IncidentID > 0) return "Incident";
				if (DocumentID > 0) return "Document";
				return "";
			}
		}
		#endregion

		#region SharedID
		private int SharedID
		{
			get
			{
				try
				{
					return Request["SharedId"] != null ? int.Parse(Request["SharedId"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			pcCurrentUser = Security.CurrentUser.Properties;
			if (pcCurrentUser["c_SortColumn"] == null)
				pcCurrentUser["c_SortColumn"] = "CreationDate DESC";
			BindToolbar();
			BindDataGrid();
		}

		#region BindToolbar
		private void BindToolbar()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Common.Resources.strComments", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

			string commentaddlink = "../Common/CommentAdd.aspx";
			string sSize = "520,270";
			if (Security.CurrentUser.IsExternal)
			{
				commentaddlink = "../External/ExternalCommentAdd.aspx";
				sSize = "800,600";
			}

			if (SharedID > 0) commentaddlink += "?SharedId=" + SharedID + "&";
			else
				commentaddlink += "?";

			string link = "<img alt='' src='../Layouts/Images/icons/comments.gif'/> " + LocRM.GetString("tbCommentsAdd");
			switch (sType)
			{
				case "Project":
					secHeader.AddRightLink(link, "javascript:OpenWindow('" + commentaddlink + "ProjectID=" + ProjID + "'," + sSize + ",false);");
					break;
				case "Task":
					secHeader.AddRightLink(link, "javascript:OpenWindow('" + commentaddlink + "TaskID=" + TaskID + "'," + sSize + ",false);");
					break;
				case "ToDo":
					secHeader.AddRightLink(link, "javascript:OpenWindow('" + commentaddlink + "ToDoID=" + ToDoID + "'," + sSize + ",false);");
					break;
				case "Event":
					secHeader.AddRightLink(link, "javascript:OpenWindow('" + commentaddlink + "EventID=" + EventID + "'," + sSize + ",false);");
					break;
				case "Incident":
					secHeader.AddRightLink(link, "javascript:OpenWindow('" + commentaddlink + "IncidentID=" + IncidentID + "'," + sSize + ",false);");
					break;
				case "Document":
					secHeader.AddRightLink(link, "javascript:OpenWindow('" + commentaddlink + "DocumentID=" + DocumentID + "'," + sSize + ",false);");
					break;
			}
		}
		#endregion

		#region BindDataGrid

		private void BindDataGrid()
		{
			dgComments.Columns[1].HeaderText = LocRM.GetString("Text");
			dgComments.Columns[2].HeaderText = LocRM.GetString("CreatedBy");
			dgComments.Columns[3].HeaderText = LocRM.GetString("CreationDate");
			

			foreach (DataGridColumn dgc in dgComments.Columns)
			{
				if (dgc.SortExpression == pcCurrentUser["c_SortColumn"].ToString())
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='../layouts/images/upbtnF.jpg'/>";
				else if (dgc.SortExpression + " DESC" == pcCurrentUser["c_SortColumn"].ToString())
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='../layouts/images/downbtnF.jpg'/>";
			}

			DataTable dt = new DataTable();
			switch (sType)
			{
				case "Project":
					{
						dt = Project.GetListDiscussionsDataTable(ProjID);
						break;
					}
				case "Task":
					{
						dt = Task.GetListDiscussionsDataTable(TaskID);
						break;
					}
				case "ToDo":
					{
						dt = ToDo.GetListDiscussionsDataTable(ToDoID);
						break;
					}
				case "Event":
					{
						dt = CalendarEntry.GetListDiscussionsDataTable(EventID);
						break;
					}
				case "Incident":
					{
						dt = Incident.GetListDiscussionsDataTable(IncidentID);
						break;
					}
				case "Document":
					{
						dt = Document.GetListDiscussionsDataTable(DocumentID);
						break;
					}
				default:
					{
						break;
					}
			}

			DataView dv = dt.DefaultView;
			try
			{
				dv.Sort = pcCurrentUser["c_SortColumn"];
			}
			catch
			{
				pcCurrentUser["c_SortColumn"] = "CreationDate DESC";
				dv.Sort = pcCurrentUser["c_SortColumn"];
			}
			dgComments.DataSource = dt.DefaultView;

			if (pcCurrentUser["c_PageSize"] != null)
				dgComments.PageSize = int.Parse(pcCurrentUser["c_PageSize"]);


			if (pcCurrentUser["c_Page"] != null)
			{
				int pageindex = int.Parse(pcCurrentUser["c_Page"]);
				int ppi = dt.Rows.Count / dgComments.PageSize;
				if (dt.Rows.Count % dgComments.PageSize == 0)
					ppi = ppi - 1;

				if (pageindex <= ppi)
					dgComments.CurrentPageIndex = pageindex;
				else dgComments.CurrentPageIndex = 0;
			}

			dgComments.DataBind();
			foreach (DataGridItem dgi in dgComments.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning") + "')");
			}
		}
		#endregion

		protected string GetEditLink(int DiscussionId)
		{
			string sPath = (Security.CurrentUser.IsExternal) ? "../External/ExternalCommentAdd.aspx" : "../Common/CommentAdd.aspx";
			return String.Format("javascript:OpenWindow('{0}?DiscussionId={1}', {2}, false);",
			  sPath, DiscussionId, (Security.CurrentUser.IsExternal) ? "800,600" : "520,270");
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgComments.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgComments_PageIndexChanged);
			this.dgComments.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dgComments_SortCommand);
			this.dgComments.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dgComments_PageSizeChange);
			this.dgComments.DeleteCommand += new DataGridCommandEventHandler(dgComments_DeleteCommand);
		}
		#endregion

		#region DG Events
		private void dgComments_PageSizeChange(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pcCurrentUser["c_PageSize"] = e.NewPageSize.ToString();
			BindDataGrid();
		}

		private void dgComments_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			pcCurrentUser["c_Page"] = e.NewPageIndex.ToString();
			BindDataGrid();
		}

		private void dgComments_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if (pcCurrentUser["c_SortColumn"] != null && pcCurrentUser["c_SortColumn"].ToString() == (string)e.SortExpression)
				pcCurrentUser["c_SortColumn"] = (string)e.SortExpression + " DESC";
			else
				pcCurrentUser["c_SortColumn"] = (string)e.SortExpression;

			BindDataGrid();
		}

		private void dgComments_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int sid = int.Parse(e.Item.Cells[0].Text);
			Project.DeleteDiscussion(sid);
			BindDataGrid();
			//or response
		}
		#endregion
	}
}
