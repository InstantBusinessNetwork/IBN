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
	///		Summary description for CommentAdd.
	/// </summary>
	public partial  class CommentAdd : System.Web.UI.UserControl
	{
		//protected System.Web.UI.WebControls.Label lblTextTitle;

		#region ProjID
		private int ProjID
		{
			get
			{
				try
				{
					if(Request["ProjectID"] != null)
						return int.Parse(Request["ProjectID"]);
					else
						return -1;
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
					if(Request["TaskID"] != null)
						return int.Parse(Request["TaskID"]);
					else
						return -1;
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
					if(Request["ToDoID"] != null)
						return int.Parse(Request["ToDoID"]);
					else
						return -1;
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
					if(Request["EventID"] != null)
						return int.Parse(Request["EventID"]);
					else
						return -1;
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
					if(Request["IncidentID"] != null)
						return int.Parse(Request["IncidentID"]);
					else
						return -1;
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
					if(Request["DocumentID"] != null)
						return int.Parse(Request["DocumentID"]);
					else
						return -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region DiscussionId
		private int DiscussionId
		{
			get
			{
				try
				{
					if(Request["DiscussionId"] != null)
						return int.Parse(Request["DiscussionId"]);
					else
						return -1;
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
				if(ProjID>0)return "Project";
				if(TaskID>0)return "Task";
				if(ToDoID>0)return "ToDo";
				if(EventID>0)return "Event";
				if(IncidentID>0)return "Incident";
				if(DocumentID>0)return "Document";
				return "";
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnCancel.Attributes.Add("onclick", "window.close();");
			btnSave.Attributes.Add("onclick","DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			fckEditor.Language = Security.CurrentUser.Culture;
			fckEditor.EnableSsl = Request.IsSecureConnection;
			fckEditor.SslUrl = ResolveUrl("~/Common/Empty.html");

			if(!Page.IsPostBack)
			{
				ApplyLocalization();
			}

			if (DiscussionId>0)
			{
				using (IDataReader rdr = Project.GetDiscussion(DiscussionId))
				{
					if (rdr.Read())
					{
						if ((int)rdr["CreatorId"] != Security.CurrentUser.UserID)
							throw new AccessWillBeDeniedException();
						fckEditor.Text = rdr["Text"].ToString();
					}
				}
			}
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Common.Resources.strCommentAdd", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
//			lblTextTitle.Text = LocRM.GetString("Text");
			btnSave.Text = LocRM.GetString("tbNewCommentSave");
			btnCancel.Text = LocRM.GetString("tbNewCommentCancel");
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

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			if (String.IsNullOrEmpty(fckEditor.Text))
				return;

			switch(sType)
			{
				case "Project":
					Project.AddDiscussion(ProjID, fckEditor.Text);
					break;
				case "Task":
					Task.AddDiscussion(TaskID, fckEditor.Text);
					break;
				case "ToDo":
					ToDo.AddDiscussion(ToDoID, fckEditor.Text);
					break;
				case "Event":
					CalendarEntry.AddDiscussion(EventID, fckEditor.Text);
					break;
				case "Incident":
					Incident.AddDiscussion(IncidentID, fckEditor.Text);
					break;
				case "Document":
					Document.AddDiscussion(DocumentID, fckEditor.Text);
					break;
			}
			if (DiscussionId>0)
			{
				Project.UpdateDiscussion(DiscussionId, fckEditor.Text);
			}

			Mediachase.UI.Web.Util.CommonHelper.CloseItAndRefresh(Response);
		}
		#endregion
	}
}
