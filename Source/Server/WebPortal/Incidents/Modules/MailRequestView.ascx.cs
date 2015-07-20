namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.UI.Web.Util;
	using System.Resources;
	using System.IO;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.ControlSystem;

	/// <summary>
	///		Summary description for MailRequestView.
	/// </summary>
	public partial class MailRequestView : System.Web.UI.UserControl
	{

    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strMailIncidentsList", typeof(MailRequestView).Assembly);

		public string iSrc = String.Empty;

		#region Pop3BoxId
		protected int Pop3BoxId
		{
			get
			{
				if (ViewState["Pop3BoxId"] != null)
					return (int)ViewState["Pop3BoxId"];
				else
					return 0;
			}
			set
			{
				ViewState["Pop3BoxId"] = value;
			}
		}
		#endregion

		#region RequestId
		protected int RequestId
		{
			get
			{
				try
				{
					return int.Parse(Request["RequestId"]);
				}
				catch
				{
					throw new Exception("Invalid RequestId ID!!!");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				BindValues();
				BindFiles();
			}

			BindToolbar();
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
			btnApprove.ServerClick +=new EventHandler(btnApprove_ServerClick);
			btnApproveCrete.ServerClick +=new EventHandler(btnApproveCrete_ServerClick);
			btnDelete.ServerClick +=new EventHandler(btnDelete_ServerClick);
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tbTitle");
			secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("Back"),"../Incidents/default.aspx?BTab=MailIncidents&MailBoxId="+Pop3BoxId);

			secFiles.Title = LocRM.GetString("tbFileTitle");

			secTracking.Title  = LocRM.GetString("tbTrackingTitle");

			btnApproveCrete.Text = LocRM.GetString("ApproveCreate");
			btnDelete.Text = LocRM.GetString("Delete");
			btnApprove.Text = LocRM.GetString("Approve");
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			ddlPriority.DataSource = Incident.GetListPriorities();
			ddlPriority.DataTextField =	"PriorityName";	
			ddlPriority.DataValueField="PriorityId";
			ddlPriority.DataBind();

			ddlProjects.DataSource = Incident.GetListProjects();
			ddlProjects.DataTextField = "Title";
			ddlProjects.DataValueField = "ProjectId";
			ddlProjects.DataBind();
			ListItem liNew = new ListItem(LocRM.GetString("ProjectNotSet"),"-1");
			ddlProjects.Items.Insert(0,liNew);
			ddlProjects.DataSource = null;
			ddlProjects.DataBind();

			ClearMHTCache(Server.MapPath(CommonHelper.ChartPath));

			///	 Pop3MailRequestId, Sender, SenderIbnUserId, FirstName, LastName, Subject, InnerText, 
			///  Priority, PriorityName, Pop3BoxId, Received, MhtFileId, SenderType, Pop3BoxName
			using(IDataReader rdr = IssueRequest.Get(RequestId))
			{
				if (rdr.Read())
				{
					txtSubject.Text = rdr["Subject"].ToString();
					txtDescription.Text = rdr["InnerText"].ToString();

					Pop3BoxId = (int)rdr["Pop3BoxId"];
					CommonHelper.SafeSelect(ddlPriority, rdr["Priority"].ToString());

					if (rdr["SenderIbnUserId"] != DBNull.Value)
					{
						lblSender.Text = CommonHelper.GetUserStatus((int)rdr["SenderIbnUserId"]);

						lblText.Text = LocRM.GetString("ADText");
						btnApproveCrete.Visible = false;
					}
					else 
					{
						if (rdr["FirstName"].ToString() != String.Empty || rdr["LastName"].ToString() != String.Empty)
							lblSender.Text = String.Format("<a href='mailto:{0}'>{1} {2}</a> ({3})", rdr["Sender"].ToString(), rdr["LastName"].ToString(), rdr["FirstName"].ToString(), LocRM.GetString("Unknown"));
						else
							lblSender.Text = String.Format("<a href='mailto:{0}'>{0}</a> ({1})", rdr["Sender"].ToString(), LocRM.GetString("Unknown"));

						lblText.Text = LocRM.GetString("ADTextExternal");
					}
					

					lblReceived.Text = ((DateTime)rdr["Received"]).ToShortDateString();

					lnkMailBox.Text = rdr["Pop3BoxName"].ToString();
					lnkMailBox.NavigateUrl = String.Format("~/Incidents/default.aspx?BTab=MailIncidents&MailBoxId={0}", rdr["Pop3BoxId"].ToString());
					
					int mhtFileId = -1;
					if (rdr["MhtFileId"] != DBNull.Value)
						mhtFileId = (int)rdr["MhtFileId"];

					if (mhtFileId > 0)
					{
						string wwwpath = CommonHelper.ChartPath + Guid.NewGuid().ToString()+".mht";

						Mediachase.IBN.Business.ControlSystem.FileStorage fileStorage = IssueRequest.GetFileStorage(RequestId);

						using (Stream sw = File.Create(Server.MapPath(wwwpath)))
						{
							fileStorage.LoadFile(mhtFileId, sw);
						}

						iSrc = ResolveUrl(wwwpath);
					}
					else
					{
						trMHT.Visible = false;
					}
				}
				else
				{
					Response.Redirect("~/common/NotExistingId.aspx?IncidentId="+ RequestId);
				}
			}
		}
		#endregion

		#region ClearMHTCache
		private void ClearMHTCache(string _MHTPath)
		{
			System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(_MHTPath);
			System.IO.FileInfo[] fi = dir.GetFiles("*.mht");
			foreach(System.IO.FileInfo _fi in fi)
			{
				if (_fi.CreationTime<DateTime.Now.AddMinutes(-60))
					_fi.Delete();
			}
		}
		#endregion

		#region BindFiles
		private void BindFiles()
		{

			dgFiles.Columns[1].HeaderText = LocRM.GetString("Title");
			dgFiles.Columns[2].HeaderText = LocRM.GetString("Size");
			DataTable dt = new DataTable();
			DataRow dr;

			dt.Columns.Add(new DataColumn("FileId", typeof(int)));
			dt.Columns.Add(new DataColumn("FileName", typeof(string)));
			dt.Columns.Add(new DataColumn("Icon", typeof(string)));				
			dt.Columns.Add(new DataColumn("Size", typeof(string)));

			Mediachase.IBN.Business.ControlSystem.FileStorage fileStorage = IssueRequest.GetFileStorage(RequestId);
			foreach (Mediachase.IBN.Business.ControlSystem.FileInfo fi in fileStorage.GetFiles(fileStorage.Root))
			{
				dr = dt.NewRow();

				dr["FileId"] = fi.Id;
				dr["Icon"] = "../Common/ContentIcon.aspx?IconID=" + fi.FileBinaryContentTypeId;
				dr["FileName"] = String.Format("<a href=\"../FileStorage/DownloadFile.aspx?Id={0}&CName=Pop3MailRequest&CKey=Pop3MailRequestId_{1}\">{2}</a>", fi.Id, RequestId, fi.Name);
				dr["Size"] = Mediachase.UI.Web.Util.CommonHelper.ByteSizeToStr(fi.Length);

				dt.Rows.Add(dr);
			}
			
			dgFiles.DataSource = new DataView(dt);
			dgFiles.DataBind();

/*			foreach (DataGridItem dgi in dgFiles.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib!=null)
					ib.Attributes.Add("onclick","return confirm('"+ LocRM.GetString("Warning") +"')");
			}
*/
		}
		#endregion

		#region Approve()
		private void Approve(bool createExternalUser)
		{
			int projectId = int.Parse(ddlProjects.SelectedValue);

			IssueRequest.Update(RequestId, txtSubject.Text, txtDescription.Text, int.Parse(ddlPriority.SelectedValue));
			int NewId = -1;//IssueRequest.Approve(RequestId, projectId, createExternalUser);

			Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../Incidents/IncidentView.aspx?IncidentId=" + NewId, Response);
		}
		#endregion

		#region Button Handlers
		private void btnApprove_ServerClick(object sender, EventArgs e)
		{
			Approve(false);
		}

		private void btnApproveCrete_ServerClick(object sender, EventArgs e)
		{
			Approve(true);
		}

		private void btnDelete_ServerClick(object sender, EventArgs e)
		{
			IssueRequest.Delete(RequestId);
			Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../Incidents/default.aspx?BTab=MailIncidents&MailBoxId="+Pop3BoxId, Response);
		}
		#endregion
	}
}
