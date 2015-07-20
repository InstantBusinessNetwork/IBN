namespace Mediachase.UI.Web.Documents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;

	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Globalization;

	/// <summary>
	///		Summary description for DocumentInfo.
	/// </summary>
	public partial class DocumentInfo3 : System.Web.UI.UserControl
	{

		public ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Documents.Resources.strDocuments", typeof(DocumentInfo3).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.TimeTracking.Resources.strTimeTrackingInfo", typeof(DocumentInfo3).Assembly);

		#region DocumentId
		private int DocumentId
		{
			get
			{
				try
				{
					return int.Parse(Request["DocumentId"]);
				}
				catch
				{
					throw new Exception("Ivalid Document ID!!!");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();
			if(!Page.IsPostBack)
			{
				BindValues();
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
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{

			tbInfo.AddText(LocRM.GetString("tbDocumentInfo"));
			if(Document.CanUpdate(DocumentId))
				tbInfo.AddRightLink("<img alt='' src='../Layouts/Images/icons/document_edit.gif'/> " + LocRM.GetString("tbViewEdit"),"../Documents/DocumentEdit.aspx?DocumentID=" + DocumentId + "&Back=View");
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			if(DocumentId != 0)
			{
				bool canViewFinances = Document.CanViewFinances(DocumentId);
				using (IDataReader reader = Document.GetDocument(DocumentId))
				{
					if (reader.Read())
					{
						lblDocumentId.Text = reader["DocumentId"].ToString();
						lblTitle.Text = (string)reader["Title"];
						if (Configuration.ProjectManagementEnabled)
						{
							if (reader["ProjectId"] != DBNull.Value)
							{
								string sProjectId = reader["ProjectId"].ToString();
								if (sProjectId != "")
								{
									if (!Project.CanRead(int.Parse(sProjectId)) || Security.CurrentUser.IsExternal)
										lblProject.Text = reader["ProjectTitle"].ToString();
									else
										lblProject.Text = "<a href='../Projects/ProjectView.aspx?ProjectID=" + int.Parse(sProjectId) + "'>" + reader["ProjectTitle"].ToString() + "</a>";
								}
							}
						}
						else
						{
							tdPrjLabel.Visible = false;
							tdPrjName.Visible = false;
							tdCats.ColSpan = 3;
						}
						lblPriority.Text = reader["PriorityName"].ToString();
						if (reader["Description"] != DBNull.Value)
							lblDescription.Text = CommonHelper.parsetext(reader["Description"].ToString(), false);
						if (reader["ManagerId"] != DBNull.Value)
							lblManager.Text = CommonHelper.GetUserStatus((int)reader["ManagerId"]);
						lblCreated.Text = CommonHelper.GetUserStatus((int)reader["CreatorId"]) + "&nbsp;&nbsp;" + ((DateTime)reader["CreationDate"]).ToShortDateString() + " " + ((DateTime)reader["CreationDate"]).ToShortTimeString();

						lblClient.Text = Util.CommonHelper.GetClientLink(this.Page, reader["OrgUid"], reader["ContactUid"], reader["ClientName"]);

						lblTaskTime.Text = Util.CommonHelper.GetHours((int)reader["TaskTime"]);
						if (canViewFinances)
						{
							SpentTimeLabel.Text = String.Format(CultureInfo.InvariantCulture,
								"{0} / {1}:",
								LocRM3.GetString("spentTime"),
								LocRM3.GetString("approvedTime"));

							lblSpentTime.Text = String.Format(CultureInfo.InvariantCulture,
								"{0} / {1}",
								Util.CommonHelper.GetHours((int)reader["TotalMinutes"]),
								Util.CommonHelper.GetHours((int)reader["TotalApproved"]));
						}
					}
					else
					{
						Response.Redirect("../Common/NotExistingID.aspx?DocumentID=1");
					}
				}

				string sCategories = string.Empty;
				using (IDataReader reader = Document.GetListCategories(DocumentId))
				{
					while (reader.Read())
					{
						if (sCategories != "")
							sCategories += ", ";
						sCategories += (string)reader["CategoryName"];
					}
					lblCategories.Text = sCategories;		
				}

				tdPriority.Visible = tdPriority2.Visible = PortalConfig.CommonDocumentAllowViewPriorityField;
				tdClient.Visible = tdClient2.Visible = PortalConfig.CommonDocumentAllowViewClientField;
				trPriorityClient.Visible = tdPriority.Visible || tdClient.Visible;
				tdCategories.Visible = tdCats.Visible = PortalConfig.CommonDocumentAllowViewGeneralCategoriesField;
				tdTaskTime.Visible = tdTaskTime2.Visible = PortalConfig.CommonDocumentAllowViewTaskTimeField;
			}
		}
		#endregion
	}
}
