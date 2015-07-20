using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail;
using System.Text;

namespace Mediachase.UI.Web.Admin
{
	/// <summary>
	/// Summary description for EmailListImportExport.
	/// </summary>
	public partial class EmailListExport : System.Web.UI.Page
	{

		#region Prorerties

		public string ListType
		{
			get
			{
				if (Request["listtype"] != null)
					return Request["listtype"].ToString();
				return string.Empty;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.ContentType = "text/plain";
			HttpContext.Current.Response.Clear();
			HttpContext.Current.Response.Charset = "utf-8";
			HttpContext.Current.Response.AddHeader("Content-Type", "application/octet-stream");
			HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + ListType + "List.txt");
			StringBuilder body = new StringBuilder();
			if (ListType == "White")
			{
				foreach (WhiteListItem wli in WhiteListItem.List(""))
				{
					body.AppendLine(wli.From);
				}
			}
			if (ListType == "Black")
			{
				foreach (BlackListItem bli in BlackListItem.List(""))
				{
					body.AppendLine(bli.From);
				}
			}
			HttpContext.Current.Response.Write(body);
			HttpContext.Current.Response.End();
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
