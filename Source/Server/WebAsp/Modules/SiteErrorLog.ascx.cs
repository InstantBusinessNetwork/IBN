using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Configuration;

namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for SiteErrorLog.
	/// </summary>
	public partial  class SiteErrorLog : System.Web.UI.UserControl
	{

		#region CompanyUid
		protected Guid CompanyUid
		{
			get
			{
				try
				{
					return new Guid(Request["id"].ToString());
				}
				catch(Exception)
				{
					return Guid.Empty;
				}
			}
		}
		#endregion

		#region FileName
		private string FileName
		{
			get
			{
				return Request["fileName"];
			}
		}
		#endregion

		#region Back
		private string Back
		{
			get
			{
				return (string)Request["back"];
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			lblError.Text = "";

			if (!IsPostBack)
			{
				if (FileName != null)	// view error
				{
					trTable.Visible = false;
					BindError();
				}
				else // errors by company
				{
					trLabel.Visible = false;
					BinddgErrors();
				}

			}

			BindToolbar();
		}

		#region BindToolbar
		private void BindToolbar()
		{
			string sDomain = Configurator.Create().GetCompanyInfo(CompanyUid.ToString()).Host;
			if (FileName == null)
			{
				secH.Title = sDomain;
				secH.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> Manage Sites","../Pages/Sites.aspx");
			}
			else
			{
				secH.Title = "Error View";
				if (Back != "reports")
					secH.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> Error Log","../Pages/SiteErrorLog.aspx?id="+CompanyUid.ToString());
				else
					secH.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> Back To Reports","../Pages/ASPHome.aspx?Tab=1");
			}
		}
		#endregion

		#region BinddgErrors
		private void BinddgErrors()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("ErrorID");
			dt.Columns.Add("CreationTime",typeof(DateTime));

			IConfigurator config = Configurator.Create();
			ICompanyInfo company = config.GetCompanyInfo(CompanyUid.ToString());
			string installPath = config.InstallPath;
			string companyCodePath = Path.Combine(installPath, company.CodePath);
			string errorsPath = Path.Combine(companyCodePath, @"Web\Portal\Admin\Log\Error");

			DirectoryInfo dir = new DirectoryInfo(errorsPath);
			foreach (FileInfo fileinfo in dir.GetFiles())
			{
				string ErrorLink = fileinfo.Name;

				string ext = "";
				if (ErrorLink.IndexOf(".aspx") >= 0)
					ext = ".aspx";
				else if (ErrorLink.IndexOf(".html") >= 0)
					ext = ".html";
				else
					continue;

				string PureName = ErrorLink.Substring(0, ErrorLink.IndexOf(ext));
				string ErrorID = PureName.Substring(PureName.LastIndexOf("_") + 1);

				DataRow dr = dt.NewRow();
				dr["ErrorID"] = "<a href='SiteErrorLog.aspx?id=" + CompanyUid.ToString() + "&fileName=" + PureName + "'>" + ErrorID + "</a>";
				dr["CreationTime"] = fileinfo.CreationTime;
				dt.Rows.Add(dr);
			}
			
			DataView dv = dt.DefaultView;
			dv.Sort = "CreationTime DESC";

			dgErrors.DataSource = dv;
			dgErrors.DataBind();
		}
		#endregion

		#region BindError
		private void BindError()
		{
			IConfigurator config = Configurator.Create();
			ICompanyInfo company = config.GetCompanyInfo(CompanyUid.ToString());
			string installPath = config.InstallPath;
			string companyCodePath = Path.Combine(installPath, company.CodePath);
			string errorsPath = Path.Combine(companyCodePath, @"Web\Portal\Admin\Log\Error");

			string FilePath = Path.Combine(errorsPath, FileName + ".html");
			if(!File.Exists(FilePath))
				FilePath = FilePath.Replace(".html", ".aspx");
			if (!File.Exists(FilePath))
				return;

			StreamReader sr = File.OpenText(FilePath);
			string sTemp = sr.ReadToEnd();
			sr.Close();
			lblError.Text = sTemp.Replace("../../../Admin/errorlog.aspx","../Pages/SiteErrorLog.aspx?id="+CompanyUid.ToString());
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
