namespace Mediachase.UI.Web.Documents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.UI.Web.Util;
	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Web.UI;

	/// <summary>
	///		Summary description for DocumentInfo2.
	/// </summary>
	public partial class DocumentInfo2 : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Documents.Resources.strDocuments", typeof(DocumentInfo2).Assembly);

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

		#region BindValues
		private void BindValues()
		{
			if (DocumentId != 0)
			{
				using (IDataReader rdr = Document.GetDocument(DocumentId))
				{
					///  DocumentId, ProjectId, ProjectTitle, CreatorId, ManagerId, 
					///  Title, Description, CreationDate, PriorityId, PriorityName, 
					///  StatusId, StatusName, IsCompleted, ReasonId, StateId, ProjectCode
					if (rdr.Read())
					{
						string sTitle = "";
						if (rdr["ProjectId"] != DBNull.Value)
						{
							string projectPostfix = CHelper.GetProjectNumPostfix((int)rdr["ProjectId"], (string)rdr["ProjectCode"]);
							if (Project.CanRead((int)rdr["ProjectId"]))
								sTitle += String.Format("<a href='../Projects/ProjectView.aspx?ProjectId={0}' title='{1}'>{2}{3}</a> \\ ", 
									(int)rdr["ProjectId"],
									LocRM.GetString("Project"),
									rdr["ProjectTitle"].ToString(),
									projectPostfix
									);
							else
								sTitle += String.Format("<span title='{0}'>{1}{2}</span>\\ ",
									LocRM.GetString("Project"), 
									rdr["ProjectTitle"].ToString(),
									projectPostfix);
						}
						sTitle += String.Format("{0} (#{1})", rdr["Title"].ToString(), rdr["DocumentId"].ToString());
						lblTitle.Text = sTitle;

						lblState.ForeColor = Util.CommonHelper.GetStateColor((int)rdr["StateId"]);
						lblState.Text = rdr["StateName"].ToString();
						if ((int)rdr["StateId"] == (int)ObjectStates.Active || (int)rdr["StateId"] == (int)ObjectStates.Overdue)
							lblState.Text += String.Format(": {0}", rdr["StatusName"].ToString());

						lblPriority.Text = rdr["PriorityName"].ToString() + " " + LocRM.GetString("Priority").ToLower();
						lblPriority.ForeColor = Util.CommonHelper.GetPriorityColor((int)rdr["PriorityId"]);
						lblPriority.Visible = PortalConfig.CommonDocumentAllowViewPriorityField;

						if (rdr["ManagerId"] != DBNull.Value)
							lblManager.Text = CommonHelper.GetUserStatus((int)rdr["ManagerId"]);

						if (rdr["Description"] != DBNull.Value)
						{
							string txt = CommonHelper.parsetext(rdr["Description"].ToString(), false);
							if (PortalConfig.ShortInfoDescriptionLength > 0 && txt.Length > PortalConfig.ShortInfoDescriptionLength)
								txt = txt.Substring(0, PortalConfig.ShortInfoDescriptionLength) + "...";
							lblDescription.Text = txt;
						}
					}
				}
			}
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			BindValues();
		}
		#endregion
	}
}
