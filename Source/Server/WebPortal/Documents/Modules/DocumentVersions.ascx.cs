namespace Mediachase.UI.Web.Documents.Modules
{
	using System;
	using System.Text;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.ControlSystem;
	using Mediachase.UI.Web.Util;
	using Mediachase.IBN.Business.WebDAV.Common;

	public partial class DocumentVersions : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Documents.Resources.strDocuments", typeof(DocumentVersions).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strViewDocument", typeof(DocumentVersions).Assembly);
		
		private BaseIbnContainer _bic;
		private Mediachase.IBN.Business.ControlSystem.FileStorage _fs;
		protected string ContainerKey = "";
		protected string ContainerName
		{
			get
			{
				return "FileLibrary";
			}
		}

		#region DocumentId
		private int _documentId = -1;
		protected int DocumentId
		{
			get
			{
				if (_documentId < 0)
				{
					_documentId = 0;
					if (Request.QueryString["DocumentId"] != null)
					{
						try
						{
							_documentId = int.Parse(Request.QueryString["DocumentId"]);
						}
						catch { }
					}
				}
				return _documentId;
			}
			set
			{
				_documentId = value;
			}
		}
		#endregion

		#region ToDoId
		private int _todoId = -1;
		protected int ToDoId
		{
			get
			{
				if (_todoId < 0)
				{
					_todoId = 0;
					if (Request.QueryString["ToDoId"] != null)
					{
						try
						{
							_todoId = int.Parse(Request.QueryString["ToDoId"]);
						}
						catch { }
					}
				}
				return _todoId;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			GetDocumentId();

			ApplyLocalization();
			BindToolbar();

			//if (!IsPostBack)
				BindData();
		}

		#region GetDocumentId
		private void GetDocumentId()
		{
			if (DocumentId <= 0 && ToDoId > 0)
			{
				using (IDataReader reader = Mediachase.IBN.Business.ToDo.GetToDo(ToDoId, false))
				{
					if (reader.Read())
					{
						if (reader["DocumentId"] != DBNull.Value)
							DocumentId = (int)reader["DocumentId"];
					}
				}
			}
			ContainerKey = "DocumentVers_" + DocumentId.ToString();
			_bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
			_fs = (FileStorage)_bic.LoadControl("FileStorage");
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			grdMain.Columns[0].HeaderText = LocRM2.GetString("tVerNum");
		}
		#endregion

		#region BindToolbar()
		private void BindToolbar()
		{
			tbInfo.AddText(LocRM.GetString("tVesions"));
			if (Document.CanAddVersion(DocumentId))
				tbInfo.AddRightLink("<img alt='' src='../Layouts/Images/FileTypes/common.gif'/> " + LocRM.GetString("tAddVersion"), "javascript:ShowWizard('" + ResolveUrl("~/FileStorage/FileUpload.aspx") + "?ParentFolderId=" + _fs.Root.Id + "&ContainerKey=" + ContainerKey + "&ContainerName=" + ContainerName + "', 470, 250);");
		}
		#endregion

		#region BindData
		private void BindData()
		{
			if (Security.CurrentUser.IsExternal)
				grdMain.Columns[3].Visible = false;

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Icon", typeof(string)));
			dt.Columns.Add(new DataColumn("Title", typeof(string)));
			dt.Columns.Add(new DataColumn("Description", typeof(string)));
			dt.Columns.Add(new DataColumn("Size", typeof(string)));
			dt.Columns.Add(new DataColumn("Created", typeof(DateTime)));
			dt.Columns.Add(new DataColumn("Version", typeof(string)));
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			DataRow dr;
			FileInfo[] fMas = _fs.GetFiles(_fs.Root.Id);
			foreach (FileInfo fi in fMas)
			{
				dr = dt.NewRow();
				dr["Icon"] = ResolveUrl("~/Common/ContentIcon.aspx?IconID=" + fi.FileBinaryContentTypeId);

				string sLink = "";
				if (fi.FileBinaryContentType.ToLower().IndexOf("url") >= 0)
					sLink = Util.CommonHelper.GetLinkText(_fs, fi);
				if (sLink == "")
					sLink = Util.CommonHelper.GetAbsoluteDownloadFilePath(fi.Id, fi.Name, ContainerName, ContainerKey);

				string sNameLocked = CommonHelper.GetLockerText(sLink);

				dr["Title"] = String.Format("<a href=\"{0}\"{2}>{1}</a> {3}",
					sLink, fi.Name,
					Mediachase.IBN.Business.Common.OpenInNewWindow(fi.FileBinaryContentType) ? " target='_blank'" : "",
					sNameLocked);

				dr["Description"] = fi.Description;
				dr["Created"] = fi.Created;
				dr["Size"] = FormatBytes((long)fi.Length);
				dr["Id"] = fi.Id;
				dt.Rows.Add(dr);
			}
			DataView dv = dt.DefaultView;
			dv.Sort = "Created DESC";
			int i = dv.Count;
			foreach (DataRowView drv in dv)
			{
				drv["Version"] = i.ToString();
				i--;
			}
			grdMain.DataSource = dv;
			grdMain.DataBind();

			foreach (DataGridItem dgi in grdMain.Items)
			{
				HyperLink hl = (HyperLink)dgi.FindControl("ibPublish");
				if (hl != null)
					hl.ToolTip = LocRM.GetString("Publish");
			}
		}
		#endregion

		#region Help Strings
		string FormatBytes(long bytes)
		{
			const double ONE_KB = 1024;
			const double ONE_MB = ONE_KB * 1024;
			const double ONE_GB = ONE_MB * 1024;
			const double ONE_TB = ONE_GB * 1024;
			const double ONE_PB = ONE_TB * 1024;
			const double ONE_EB = ONE_PB * 1024;
			const double ONE_ZB = ONE_EB * 1024;
			const double ONE_YB = ONE_ZB * 1024;

			if ((double)bytes <= 999)
				return bytes.ToString() + " bytes";
			else if ((double)bytes <= ONE_KB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_KB) + " KB";
			else if ((double)bytes <= ONE_MB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_MB) + " MB";
			else if ((double)bytes <= ONE_GB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_GB) + " GB";
			else if ((double)bytes <= ONE_TB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_TB) + " TB";
			else if ((double)bytes <= ONE_PB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_PB) + " PB";
			else if ((double)bytes <= ONE_EB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_EB) + " EB";
			else if ((double)bytes <= ONE_ZB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_ZB) + " ZB";
			else
				return ThreeNonZeroDigits((double)bytes / ONE_YB) + " YB";
		}

		string ThreeNonZeroDigits(double value)
		{
			if (value >= 100)
				return ((int)value).ToString();
			else if (value >= 10)
				return value.ToString("0.0");
			else
				return value.ToString("0.00");
		}
		#endregion

		#region CanUpdateDelete
		protected bool CanUpdateDelete()
		{
			if (ViewState["CanUpdateDelete"] == null)
				ViewState["CanUpdateDelete"] = Document.CanUpdate(DocumentId);
			return (bool)ViewState["CanUpdateDelete"];
		}
		#endregion
	}
}
