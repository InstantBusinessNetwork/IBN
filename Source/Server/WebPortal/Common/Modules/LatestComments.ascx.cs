namespace Mediachase.UI.Web.Common.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;

	/// <summary>
	///		Summary description for LatestComments.
	/// </summary>
	public partial class LatestComments : System.Web.UI.UserControl
	{
		protected Mediachase.UI.Web.Modules.BlockHeader tbCommentsExternal;
		protected Mediachase.UI.Web.Modules.TopTabs oTopTabs;

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
					return Request["IncidentId"] != null ? int.Parse(Request["IncidentId"]) : -1;
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
					return Request["DocumentId"] != null ? int.Parse(Request["DocumentId"]) : -1;
				}
				catch
				{
					return -1;
				}
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

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			BindValues();
			BindToolbar();
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strLatestComments", typeof(LatestComments).Assembly);
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strLatestComments", typeof(LatestComments).Assembly);
			//tbComments.Title = LocRM.GetString("tbLatestComments");
			tbComments.AddText(LocRM.GetString("tbLatestComments"));

			string viewAllLink = "";
			ITopTabs tt = null;
			if (this.Parent.Parent.Parent is ITopTabs)
				tt = this.Parent.Parent.Parent as ITopTabs;
			else if (this.Parent.Parent is ITopTabs)
				tt = this.Parent.Parent as ITopTabs;
			if (tt != null)
			{
				oTopTabs = (TopTabs)tt.GetTopTabs();
				viewAllLink = oTopTabs.GetItemLink("Discussions");
			}

			string commentAddLink = "../Common/CommentAdd.aspx?";
			string sSize = "520,270";
			if (Security.CurrentUser.IsExternal)
			{
				commentAddLink = "../External/ExternalCommentAdd.aspx?";
				sSize = "800,600";
			}


			string AddLink = "#";
			switch (sType)
			{
				case "Project":
					{
						AddLink = "javascript:OpenWindow('" + commentAddLink + "ProjectID=" + ProjID + "'," + sSize + ",false);";
						break;
					}
				case "Task":
					{
						AddLink = "javascript:OpenWindow('" + commentAddLink + "TaskID=" + TaskID + "'," + sSize + ",false);";
						break;
					}
				case "ToDo":
					{
						AddLink = "javascript:OpenWindow('" + commentAddLink + "ToDoID=" + ToDoID + "'," + sSize + ",false);";
						break;
					}
				case "Event":
					{
						AddLink = "javascript:OpenWindow('" + commentAddLink + "EventID=" + EventID + "'," + sSize + ",false);";
						break;
					}
				case "Incident":
					{
						AddLink = "javascript:OpenWindow('" + commentAddLink + "IncidentID=" + IncidentID + "'," + sSize + ",false);";
						break;
					}
				case "Document":
					{
						AddLink = "javascript:OpenWindow('" + commentAddLink + "DocumentID=" + DocumentID + "'," + sSize + ",false);";
						break;
					}
				default:
					{
						break;
					}
			}

			if (SharedID > 0)
				AddLink += "&SharedId=" + SharedID;

			if (dgComments.Items.Count == 0)
			{
				this.Visible = false;
				if (tbCommentsExternal != null)
				{
					if (sType != "Incident")
					{
						tbCommentsExternal.AddLink("<img alt='' src='../Layouts/Images/icons/comments.gif' border='0' width='16' height='16' align='absmiddle' title='" + LocRM.GetString("tbAddL") + "'>", AddLink);
						tbCommentsExternal.AddSeparator();
					}
				}
			}
			else
			{
				this.Visible = true;
				tbComments.AddRightLink("<img alt='' src='../Layouts/Images/icon-search.gif'/> " + LocRM.GetString("tbView"), HttpUtility.HtmlAttributeEncode(viewAllLink));
				tbComments.AddRightLink("<img alt='' src='../Layouts/Images/icons/comments.gif'/> " + LocRM.GetString("tbAdd"), HttpUtility.HtmlAttributeEncode(AddLink));
			}
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
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
			dv.Sort = "CreationDate DESC";
			dgComments.DataSource = dv;
			dgComments.DataBind();

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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
