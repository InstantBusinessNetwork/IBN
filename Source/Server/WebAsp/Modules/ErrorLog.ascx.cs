using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;

namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for ErrorLog.
	/// </summary>
	public partial  class ErrorLog : System.Web.UI.UserControl
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!Page.IsPostBack)BinddgErrors();
			// Put user code to initialize the page here
		}

		private void BinddgErrors()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("ErrorID");
			dt.Columns.Add("CreationTime",typeof(DateTime));

			string path = (Server.MapPath("Log/Error/"));
			DirectoryInfo dir = new DirectoryInfo(path);
			foreach (FileInfo fileinfo in dir.GetFiles())
			{
				DataRow dr = dt.NewRow();
				dr["ErrorID"] = "<a href='Log/Error/"+fileinfo.Name+"'>" + fileinfo.Name + "</a>";
				dr["CreationTime"] = fileinfo.CreationTime;
				dt.Rows.Add(dr);
			}
			
			DataView dv = dt.DefaultView;
			dv.Sort = "CreationTime DESC";

			dgErrors.DataSource = dv;
			dgErrors.DataBind();
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

		}
		#endregion
	}
}
