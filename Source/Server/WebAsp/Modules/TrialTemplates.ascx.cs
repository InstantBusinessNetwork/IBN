using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for TrialTemplates.
	/// </summary>
	public partial  class TrialTemplates : System.Web.UI.UserControl
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolBar();
			if (!Page.IsPostBack)
			{
				BindData();
			}
		}

		#region BindToolBar()
		private void BindToolBar()
		{
			secHeader.Title = "Manage Notification Templates";
			secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> Default Page","../Pages/ASPHome.aspx");
		}
		#endregion

		#region BindData()
		private void BindData()
		{
			DataTable table = new DataTable();
			Hashtable templates = new Hashtable();

			table.Columns.Add(new DataColumn("TemplateName", typeof(string)));
			table.Columns.Add(new DataColumn("Subject", typeof(string)));
			table.Columns.Add(new DataColumn("Attachments", typeof(string)));
			table.Columns.Add(new DataColumn("ActionEdit", typeof(string)));
			
//			templates.Add("UserActivation","Mediachase IBN Trial - Instructions for Activation.");
			templates.Add("UserActivated","Mediachase IBN Trial – Getting Started.");
			templates.Add("UserAfterOneDayIM", "Additional Tips and Information for Your IBN Trial.");
			templates.Add("UserAfterOneWeek","Additional Tips and Information for Your IBN Trial.");
			templates.Add("UserOneWeekBefore","IBN Trial - One Week Left.");
			templates.Add("UserOneDayBefore","IBN Trial - Trial Period has Completed.");

			templates.Add("Client1DayZeroBalance", "Mediachase IBN Notification - Balance is too low.");
			templates.Add("Client3DayZeroBalance", "Mediachase IBN Notification - Balance is too low.");
			templates.Add("Client7DayZeroBalance", "Mediachase IBN Notification - Balance is too low.");
			templates.Add("ClientZeroBalance", "Mediachase IBN Notification - Negative Balance.");
			templates.Add("ClientBalanceUp", "Mediachase IBN Notification - Payment was received.");

			BindGrid(templates, table, dgCustomer);

			templates.Clear();

//			templates.Add("TrialNewRequest","Mediachase IBN Trial - New Trial Request.");
			templates.Add("TrialActivated","IBN Trial Alert – Customer has Activated Trial.");
			templates.Add("TrialOneDayBefore","IBN Trial - Trial Period has Completed.");
			templates.Add("TrialDeactivated","IBN Trial - Account Deactivated.");

			templates.Add("OperatorCompanyDeactivatedDayBefore", "Mediachase IBN Billable Company - Balance is too low.");
			templates.Add("OperatorTariffRequest", "Mediachase IBN Billable Company - Request for new Tariff.");
			templates.Add("OperatorCompanyDeactivated", "Mediachase IBN Billable Company - Customer Account has been Deactivated.");

			BindGrid(templates, table, dgTrial);
		}
		#endregion

		#region BindGrid()
		private void BindGrid(Hashtable templates, DataTable table, DataGrid grid)
		{
			DataRow dr;

			table.Rows.Clear();

			foreach(string skey in templates.Keys)
			{
				dr = table.NewRow();
				dr["TemplateName"] = skey;
				string path = Server.MapPath("~/email/en-US/"+skey+"/");
				StringBuilder sb = new StringBuilder();
				if(Directory.Exists(path))
				{
					foreach(FileInfo fi in (new DirectoryInfo(path)).GetFiles())
					{
						sb.Append(fi.Name+"<br>");
					}
				}
				dr["Attachments"] = sb.ToString();

				string sTemp;
				using(StreamReader sr = File.OpenText(Server.MapPath("~/email/en-US/"+skey+".htm")))
				{
					sTemp = sr.ReadToEnd();
				}
				int i1 = sTemp.ToLower().IndexOf("<title>") + 7;
				int i2 = sTemp.ToLower().IndexOf("</title>");
				string sSubj = sTemp.Substring(i1, i2-i1);
				if(sSubj!="")
					dr["Subject"] = sSubj;
				else
					dr["Subject"] = templates[skey].ToString();
				dr["ActionEdit"] = String.Format("<a href=\"../Pages/TemplateEdit.aspx?TemplateName="+skey+"\" title=\"{0}\"> <img alt='' src='../Layouts/Images/edit.gif'/></a>", "Edit Template");
				table.Rows.Add(dr);

				dr = table.NewRow();
			}

			DataView dv = table.DefaultView;
			dv.Sort = "TemplateName";
			grid.DataSource = dv;
			grid.DataBind();
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
