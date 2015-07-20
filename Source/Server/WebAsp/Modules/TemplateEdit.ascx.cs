using System;
using System.Data;
using System.Text;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Configuration;


namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for TemplateEdit.
	/// </summary>
	public partial  class TemplateEdit : System.Web.UI.UserControl
	{

		#region PublicStrings
		public string TemplateName
		{
			get
			{
				if(Request["TemplateName"]!=null)
					return Request["TemplateName"].ToString();
				else
					return "";
			}
		}

		private string CurrentLocale
		{
			get
			{
				return ddLanguage.SelectedItem.Value;
			}
		}

		public string Subject
		{
			get
			{
				if(TemplateName!="")
				{
					Hashtable htc = new Hashtable();
					htc.Add("UserActivation","Mediachase IBN Trial - Instructions for Activation.");
					htc.Add("UserActivated","Mediachase IBN Trial – Getting Started.");
					htc.Add("UserAfterOneWeek","Additional Tips and Information for Your IBN Trial.");
					htc.Add("UserOneWeekBefore","IBN Trial - One Week Left.");
					htc.Add("UserOneDayBefore","IBN Trial - Trial Period has Completed.");
					htc.Add("TrialNewRequest","Mediachase IBN Trial - New Trial Request.");
					htc.Add("TrialActivated","IBN Trial Alert – Customer has Activated Trial.");
					htc.Add("TrialOneDayBefore","IBN Trial - Trial Period has Completed.");
					htc.Add("TrialDeactivated","IBN Trial - Account Deactivated.");

					htc.Add("Client1DayZeroBalance", "Mediachase IBN Notification - Balance is too low.");
					htc.Add("Client3DayZeroBalance", "Mediachase IBN Notification - Balance is too low.");
					htc.Add("Client7DayZeroBalance", "Mediachase IBN Notification - Balance is too low.");
					htc.Add("ClientZeroBalance", "Mediachase IBN Notification - Negative Balance.");
					htc.Add("ClientBalanceUp", "Mediachase IBN Notification - Payment was received.");
					htc.Add("OperatorCompanyDeactivatedDayBefore", "Mediachase IBN Billable Company - Balance is too low.");
					htc.Add("OperatorTariffRequest", "Mediachase IBN Billable Company - Request for new Tariff.");
					htc.Add("OperatorCompanyDeactivated", "Mediachase IBN Billable Company - Customer Account has been Deactivated.");

					string sPath = "~/email/"+CurrentLocale+"/"+TemplateName+".htm";
					StreamReader sr = File.OpenText(Server.MapPath(sPath));
					string sTemp = sr.ReadToEnd();
					sr.Close();
					int i1 = sTemp.IndexOf("<TITLE>", StringComparison.InvariantCultureIgnoreCase) + 7;
					int i2 = sTemp.IndexOf("</TITLE>", StringComparison.InvariantCultureIgnoreCase);
					string sSubj = sTemp.Substring(i1, i2-i1);
					if(sSubj!="")
						return sSubj;
					else
						return htc[TemplateName].ToString();
				}
				else
					return "";
			}
		}
		#endregion
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolBar();
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Page.ClientScript.RegisterOnSubmitStatement(this.GetType(), "submit_trap","trapsubmit();");
			if (!Page.IsPostBack)
			{
				BindLanguages();
				BindValues();
			}

			fckEditor.Language = ddLanguage.SelectedValue;
			fckEditor.ToolbarBackgroundImage = false;
			fckEditor.EnableSsl = Request.IsSecureConnection;
			fckEditor.SslUrl = this.Page.ResolveUrl("~/Pages/Empty.html");
		}

		#region AllBinds
		private void BindToolBar()
		{
			btnSave.Text = "Save";
			btnCancel.Text = "Cancel";
			secHeader.Title = "Edit Notification Template";

			txtSubject.Text = Subject;
		}

		private void BindLanguages()
		{
			ddLanguage.DataSource = Configurator.Create().ListLanguages();
			ddLanguage.DataTextField="FriendlyName";
			ddLanguage.DataValueField="Locale";
			ddLanguage.DataBind();
		}

		private void BindValues()
		{
			String sPath = "~/email/"+CurrentLocale+"/"+TemplateName+".htm";
			StreamReader sr = File.OpenText(Server.MapPath(sPath));
			string sTemp = sr.ReadToEnd();
			sr.Close();
			fckEditor.Text = sTemp;

			#region BindSupportHashtables
			Hashtable htTrialActivated = new Hashtable();
			htTrialActivated.Add("Domain","Domain");
			htTrialActivated.Add("CompanyName","Company Name");
			htTrialActivated.Add("ContactName","Contact Name");
			htTrialActivated.Add("ContactPhone","Contact Phone");
			htTrialActivated.Add("ContactEmail","Contact e-Mail");
			htTrialActivated.Add("Login","Login");
			htTrialActivated.Add("Password","Password");
			htTrialActivated.Add("TrialPeriod","Trial Period");
			htTrialActivated.Add("TrialUsers","Trial Users");
			htTrialActivated.Add("TrialDiskSpace", "Trial Database Size");
			htTrialActivated.Add("EndDate","End Date");
			htTrialActivated.Add("PortalLink", "Link to Portal");
			
			Hashtable htTrialDeactivated = new Hashtable();
			htTrialDeactivated.Add("Domain","Domain");
			htTrialDeactivated.Add("CompanyName","Company Name");
			htTrialDeactivated.Add("ContactName","Contact Name");
			htTrialDeactivated.Add("ContactPhone","Contact Phone");
			htTrialDeactivated.Add("ContactEmail","Contact e-Mail");
			htTrialDeactivated.Add("EndDate","End Date");

			Hashtable htTrialNewRequest = new Hashtable();
			htTrialNewRequest.Add("Domain","Domain");
			htTrialNewRequest.Add("CompanyName","Company Name");
			htTrialNewRequest.Add("Login","Login");
			htTrialNewRequest.Add("Password","Password");
			htTrialNewRequest.Add("FirstName","First Name");
			htTrialNewRequest.Add("LastName","Last Name");
			htTrialNewRequest.Add("Phone","Phone");
			htTrialNewRequest.Add("Email","e-Mail");
			htTrialNewRequest.Add("SizeOfGroup","Size Of Group");
			htTrialNewRequest.Add("Country","Country");
//			htTrialNewRequest.Add("TimeZone","Time Zone");
			htTrialNewRequest.Add("RecellerTitle","Receller Title");
			htTrialNewRequest.Add("RecellerId","Receller Id");
			htTrialNewRequest.Add("RequestID","Request ID");
			htTrialNewRequest.Add("RequestGUID","Request GUID");
			htTrialNewRequest.Add("Description","Description");
//			htTrialNewRequest.Add("UseIM","Use IM");
			htTrialNewRequest.Add("XML","XML");

			Hashtable htTrialOneDayBefore = new Hashtable();
			htTrialOneDayBefore.Add("Domain","Domain");
			htTrialOneDayBefore.Add("CompanyName","Company Name");
			htTrialOneDayBefore.Add("ContactName","Contact Name");
			htTrialOneDayBefore.Add("ContactPhone","Contact Phone");
			htTrialOneDayBefore.Add("ContactEmail","Contact e-Mail");
			htTrialOneDayBefore.Add("EndDate","End Date");
			htTrialOneDayBefore.Add("PortalLink", "Link to Portal");

			Hashtable htUserActivated = new Hashtable();
			htUserActivated.Add("Domain","Domain");
			htUserActivated.Add("CompanyName","Company Name");
			htUserActivated.Add("ContactName","Contact Name");
			htUserActivated.Add("ContactPhone","Contact Phone");
			htUserActivated.Add("ContactEmail","Contact e-Mail");
			htUserActivated.Add("Login","Login");
			htUserActivated.Add("Password","Password");
			htUserActivated.Add("TrialPeriod","Trial Period");
			htUserActivated.Add("TrialUsers","Trial Users");
			htUserActivated.Add("TrialDiskSpace", "Trial Database Size");
			htUserActivated.Add("EndDate","End Date");
			htUserActivated.Add("PortalLink", "Portal Link");

			Hashtable htUserActivation = new Hashtable();
			htUserActivation.Add("Domain","Domain");
			htUserActivation.Add("CompanyName","Company Name");
			htUserActivation.Add("Login","Login");
			htUserActivation.Add("Password","Password");
			htUserActivation.Add("FirstName","First Name");
			htUserActivation.Add("LastName","Last Name");
			htUserActivation.Add("Phone","Phone");
			htUserActivation.Add("Email","e-Mail");
			htUserActivation.Add("TrialPeriod","Trial Period");
			htUserActivation.Add("TrialUsers","Trial Users");
			htUserActivation.Add("TrialDiskSpace", "Trial Database Size");
			htUserActivation.Add("SizeOfGroup","Size Of Group");
			htUserActivation.Add("Country","Country");
//			htUserActivation.Add("TimeZone","Time Zone");
			htUserActivation.Add("RecellerTitle","Receller Title");
			htUserActivation.Add("RecellerId","Receller Id");
			htUserActivation.Add("RequestID","Request ID");
			htUserActivation.Add("RequestGUID","Request GUID");
			htUserActivation.Add("Description","Description");
//			htUserActivation.Add("UseIM","Use IM");
			htUserActivation.Add("XML","XML");
			htUserActivation.Add("Locale","Locale");
			htUserActivation.Add("PortalLink", "Portal Link");

			Hashtable htUserAfterOneDayIM = new Hashtable();
			htUserAfterOneDayIM.Add("Domain", "Domain");
			htUserAfterOneDayIM.Add("PortalLink", "Portal Link");
			htUserAfterOneDayIM.Add("CompanyName", "Company Name");
			htUserAfterOneDayIM.Add("ContactName", "Contact Name");
			htUserAfterOneDayIM.Add("ContactPhone", "Contact Phone");
			htUserAfterOneDayIM.Add("ContactEmail", "Contact e-Mail");
			htUserAfterOneDayIM.Add("EndDate", "End Date");
			htUserAfterOneDayIM.Add("Name", "Name");
			htUserAfterOneDayIM.Add("Login", "Login");
//			htUserAfterOneDayIM.Add("Password", "Password");

			Hashtable htUserAfterOneWeek = new Hashtable();
			htUserAfterOneWeek.Add("Domain","Domain");
			htUserAfterOneWeek.Add("CompanyName","Company Name");
			htUserAfterOneWeek.Add("ContactName","Contact Name");
			htUserAfterOneWeek.Add("ContactPhone","Contact Phone");
			htUserAfterOneWeek.Add("ContactEmail","Contact e-Mail");
			htUserAfterOneWeek.Add("EndDate","End Date");
			htUserAfterOneWeek.Add("PortalLink", "Portal Link");

			Hashtable htUserOneDayBefore = new Hashtable();
			htUserOneDayBefore.Add("Domain","Domain");
			htUserOneDayBefore.Add("CompanyName","Company Name");
			htUserOneDayBefore.Add("ContactName","Contact Name");
			htUserOneDayBefore.Add("ContactPhone","Contact Phone");
			htUserOneDayBefore.Add("ContactEmail","Contact e-Mail");
			htUserOneDayBefore.Add("EndDate","End Date");
			htUserOneDayBefore.Add("PortalLink", "Portal Link");

			Hashtable htUserOneWeekBefore = new Hashtable();
			htUserOneWeekBefore.Add("Domain","Domain");
			htUserOneWeekBefore.Add("CompanyName","Company Name");
			htUserOneWeekBefore.Add("ContactName","Contact Name");
			htUserOneWeekBefore.Add("ContactPhone","Contact Phone");
			htUserOneWeekBefore.Add("ContactEmail","Contact e-Mail");
			htUserOneWeekBefore.Add("EndDate","End Date");
			htUserOneWeekBefore.Add("PortalLink", "Portal Link");

			Hashtable htOperatorCompanyDeactivated = new Hashtable();
			htOperatorCompanyDeactivated.Add("Domain", "Domain");
			htOperatorCompanyDeactivated.Add("PortalLink", "Portal Link");
			htOperatorCompanyDeactivated.Add("CompanyName", "Company Name");
			htOperatorCompanyDeactivated.Add("ContactName", "Contact Name");
			htOperatorCompanyDeactivated.Add("ContactPhone", "Contact Phone");
			htOperatorCompanyDeactivated.Add("ContactEmail", "Contact e-Mail");
			htOperatorCompanyDeactivated.Add("Tariff", "Tariff");

			Hashtable htOperatorCompanyDeactivatedDayBefore = new Hashtable();
			htOperatorCompanyDeactivatedDayBefore.Add("Domain", "Domain");
			htOperatorCompanyDeactivatedDayBefore.Add("PortalLink", "Portal Link");
			htOperatorCompanyDeactivatedDayBefore.Add("CompanyName", "Company Name");
			htOperatorCompanyDeactivatedDayBefore.Add("ContactName", "Contact Name");
			htOperatorCompanyDeactivatedDayBefore.Add("ContactPhone", "Contact Phone");
			htOperatorCompanyDeactivatedDayBefore.Add("ContactEmail", "Contact e-Mail");
			htOperatorCompanyDeactivatedDayBefore.Add("Tariff", "Tariff");
			htOperatorCompanyDeactivatedDayBefore.Add("Balance", "Balance");
			htOperatorCompanyDeactivatedDayBefore.Add("CurrencySymbol", "Currency Symbol");

			Hashtable htClient1DayZeroBalance = new Hashtable();
			htClient1DayZeroBalance.Add("Domain", "Domain");
			htClient1DayZeroBalance.Add("PortalLink", "Portal Link");
			htClient1DayZeroBalance.Add("Tariff", "Tariff");
			htClient1DayZeroBalance.Add("Balance", "Balance");
			htClient1DayZeroBalance.Add("CurrencySymbol", "Currency Symbol");
			htClient1DayZeroBalance.Add("Credit", "Credit");
			htClient1DayZeroBalance.Add("DaysBeforeEnd", "Days Before End");

			Hashtable htClientBalanceUp = new Hashtable();
			htClientBalanceUp.Add("Domain", "Domain");
			htClientBalanceUp.Add("PortalLink", "Portal Link");
			htClientBalanceUp.Add("Tariff", "Tariff");
			htClientBalanceUp.Add("Balance", "Balance");
			htClientBalanceUp.Add("CurrencySymbol", "Currency Symbol");
			htClientBalanceUp.Add("Amount", "Amount");
			htClientBalanceUp.Add("Bonus", "Bonus");
			htClientBalanceUp.Add("Total", "Total (Amount + Bonus)");
			htClientBalanceUp.Add("Credit", "Credit");
			htClientBalanceUp.Add("DaysBeforeEnd", "Days Before End");

			Hashtable htMain = new Hashtable();
			htMain.Add("TrialActivated", htTrialActivated);
			htMain.Add("TrialDeactivated", htTrialDeactivated);
			htMain.Add("TrialNewRequest", htTrialNewRequest);
			htMain.Add("TrialOneDayBefore", htTrialOneDayBefore);
			htMain.Add("UserActivated", htUserActivated);
			htMain.Add("UserActivation", htUserActivation);
			htMain.Add("UserAfterOneDayIM", htUserAfterOneDayIM);
			htMain.Add("UserAfterOneWeek", htUserAfterOneWeek);
			htMain.Add("UserOneDayBefore", htUserOneDayBefore);
			htMain.Add("UserOneWeekBefore", htUserOneWeekBefore);

			htMain.Add("OperatorCompanyDeactivatedDayBefore", htOperatorCompanyDeactivatedDayBefore);
			htMain.Add("OperatorTariffRequest", htOperatorCompanyDeactivatedDayBefore);
			htMain.Add("OperatorCompanyDeactivated", htOperatorCompanyDeactivated);
			htMain.Add("ClientBalanceUp", htClientBalanceUp);
			htMain.Add("Client1DayZeroBalance", htClient1DayZeroBalance);
			htMain.Add("Client3DayZeroBalance", htClient1DayZeroBalance);
			htMain.Add("Client7DayZeroBalance", htClient1DayZeroBalance);
			htMain.Add("ClientZeroBalance", htClient1DayZeroBalance);
			#endregion

			DataTable dt = new DataTable();
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("Description", typeof(string));
			DataRow dr;

			Hashtable htTemp = (Hashtable)htMain[TemplateName];
			
			foreach(string skey in htTemp.Keys)
			{
				dr = dt.NewRow();
				dr["Name"] = skey;
				dr["Description"] = htTemp[skey].ToString();
				dt.Rows.Add(dr);
			}
			DataView dv = dt.DefaultView;
			dv.Sort = "Name";
			dlSysVar.DataSource = dv;
			dlSysVar.DataBind();
			
			BindFiles();
		}

		private void BindFiles()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("ActionDelete", typeof(string));

			string path = Server.MapPath("~/email/"+CurrentLocale+"/"+TemplateName+"/");
			if(Directory.Exists(path))
			{
				DataRow dr;
				foreach(FileInfo _fi in (new DirectoryInfo(path)).GetFiles())
				{
					dr = dt.NewRow();
					dr["Name"] = _fi.Name;
					dr["ActionDelete"] = String.Format("<a href=\"javascript:DeleteFile('"+_fi.Name+"')\" title=\"{0}\"> <img alt='' src='../Layouts/Images/delete.gif'/>", "Delete Template");
					dt.Rows.Add(dr);
				}
			}

			dgFiles.DataSource = dt.DefaultView;
			dgFiles.DataBind();
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
			this.btnSubmit.Click += new System.Web.UI.ImageClickEventHandler(this.btnSubmit_Click);

		}
		#endregion

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			SavePostedFile();

			string sTemp = fckEditor.Text;
			string sSubj = txtSubject.Text;
			if(sSubj=="")
				sSubj = Subject;
			string sHeader="<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" ><HTML><HEAD><TITLE>"+sSubj+"</TITLE><META NAME=\"GENERATOR\" Content=\"Microsoft Visual Studio 7.0\"></HEAD><BODY>";
			string sFooter="</BODY></HTML>";
			String sPath = "~/email/"+CurrentLocale+"/"+TemplateName+".htm";
			StreamWriter sw = File.CreateText(Server.MapPath(sPath));
			sw.Write(sHeader+sTemp+sFooter);
			sw.Close();
			Response.Redirect("TrialTemplates.aspx");
		}

		protected void btnDelete_Click(object sender, System.EventArgs e)
		{
			String sPathDir = "~/email/"+CurrentLocale+"/"+TemplateName+"/";
			DirectoryInfo dir = new DirectoryInfo(Server.MapPath(sPathDir));
			FileInfo[] fi = dir.GetFiles(hdnFileName.Value);
			if(fi.Length>0)
				fi[0].Delete();
			BindFiles();
		}

		protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Pages/TrialTemplates.aspx", true);
		}

		private void btnSubmit_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if(SavePostedFile())
				BindFiles();
		}

		protected void ddLanguage_Change(object sender, System.EventArgs e)
		{
			BindValues();
		}

		private bool SavePostedFile()
		{
			bool ret = false;
			if(fAssetFile.PostedFile!=null && fAssetFile.PostedFile.ContentLength>0)
			{
				string FileName = fAssetFile.PostedFile.FileName;
				byte[] HeaderPic = null;
				HeaderPic = new byte[fAssetFile.PostedFile.ContentLength];
				fAssetFile.PostedFile.InputStream.Read(HeaderPic,0,fAssetFile.PostedFile.ContentLength);
				string path = Server.MapPath("~/email/"+CurrentLocale+"/"+TemplateName+"/" + FileName);
				CreatePath(path);
				using(FileStream fs = File.Create(path))
				{
					fs.Write(HeaderPic,0,HeaderPic.Length);
				}

				ret = true;
			}
			return ret;
		}

		#region CreatePath
		static void CreatePath(string path)
		{
			string p = path, s = "";
			int i;
			while(true)
			{
				i = p.IndexOf('\\');
				if(i < 0)
					break;
				s += p.Substring(0, i+1);
				System.IO.Directory.CreateDirectory(s);
				p = p.Substring(i+1);
			}
		}
		#endregion
	}
}
