using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.XPath;

using McBusiness = Mediachase.IBN.Business;
using Mediachase.Ibn.XmlTools;
using Mediachase.UI.Web.Util;
using System.Globalization;

namespace Mediachase.UI.Web.Public.Modules
{
	/// <summary>
	///		Summary description for Download1.
	/// </summary>
	public partial class Download1 : System.Web.UI.UserControl
	{
		public ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Public.Resources.strDownload", typeof(Download1).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.repCategories.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(this.repCat_DataBound);

			if (!IsPostBack)
				BindCategoriesRepeater();

			BindToolbar();
		}

		private static IXPathNavigable GetXml()
		{
			return Mediachase.Ibn.XmlTools.XmlBuilder.GetXml("Download", null
				, new Selector(Thread.CurrentThread.CurrentUICulture.Name)
				//, new Selector(McBusiness.Configuration.DefaultLocale)
			);
		}

		private void BindCategoriesRepeater()
		{
			IXPathNavigable navigable = GetXml();
			XPathNavigator linkItem = navigable.CreateNavigator().SelectSingleNode("Download");

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("CategoryId", typeof(string)));
			dt.Columns.Add(new DataColumn("Title", typeof(string)));
			dt.Columns.Add(new DataColumn("Description", typeof(string)));
			DataRow dr;
			foreach (XPathNavigator subItem in linkItem.SelectChildren(string.Empty, string.Empty))
			{
				dr = dt.NewRow();
				dr["CategoryId"] = subItem.GetAttribute("id", string.Empty);
				dr["Title"] = subItem.GetAttribute("title", string.Empty);
				dr["Description"] = String.Empty;
				dt.Rows.Add(dr);
			}
			repCategories.DataSource = dt.DefaultView;
			repCategories.DataBind();
		}

		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("Header");
		}

		#region GetFileSize
		protected string GetFileSize(string FileName)
		{
			long lSize = 0;
			try
			{
				string sPath = "~/Download/" + FileName;
				FileInfo fi = new FileInfo(Server.MapPath(sPath));
				lSize = fi.Length;
			}
			catch { }
			return CommonHelper.ByteSizeToStr((int)lSize);
		}
		#endregion

		private void repCat_DataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
		{
			IXPathNavigable navigable = GetXml();

			string categoryId = DataBinder.Eval(e.Item.DataItem, "CategoryId").ToString();
			XPathNavigator links = navigable.CreateNavigator().SelectSingleNode(string.Format("Download/Category[@id='{0}']", categoryId));
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("ProductID", typeof(string)));
			dt.Columns.Add(new DataColumn("ProductTitle", typeof(string)));
			dt.Columns.Add(new DataColumn("Description", typeof(string)));
			dt.Columns.Add(new DataColumn("FileName", typeof(string)));
			dt.Columns.Add(new DataColumn("Build", typeof(string)));
			dt.Columns.Add(new DataColumn("Date", typeof(DateTime)));
			DataRow dr;
			foreach (XPathNavigator subItem in links.SelectChildren(string.Empty, string.Empty))
			{
				dr = dt.NewRow();
				dr["ProductID"] = subItem.GetAttribute("id", string.Empty);
				dr["ProductTitle"] = subItem.GetAttribute("title", string.Empty);
				dr["Description"] = subItem.GetAttribute("description", string.Empty);
				dr["FileName"] = subItem.GetAttribute("file", string.Empty);
				dr["Build"] = subItem.GetAttribute("build", string.Empty);
				dr["Date"] = DateTime.Parse(subItem.GetAttribute("date", string.Empty));
				dt.Rows.Add(dr);
			}
			DataGrid dgProduct = (DataGrid)e.Item.FindControl("dgProduct");
			if (dgProduct != null)
			{
				//ArrayList alExLangs = new ArrayList();
				//using (IDataReader reader = McBusiness.Common.GetListLanguages())
				//{
				//    while (reader.Read())
				//        alExLangs.Add(reader["Locale"].ToString());
				//}

				//int i = 0;
				//while (i < dt.Rows.Count)
				//{
				//    if ((dt.Rows[i]["Build"] != DBNull.Value && (int)dt.Rows[i]["Build"] == -1)
				//        || !alExLangs.Contains(dt.Rows[i]["Locale"]))
				//        dt.Rows.RemoveAt(i);
				//    else
				//        i++;
				//}

				dgProduct.DataSource = dt.DefaultView;
				dgProduct.DataBind();
			}
		}
	}
}
