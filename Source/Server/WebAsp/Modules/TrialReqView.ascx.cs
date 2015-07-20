using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using Mediachase.Ibn.Configuration;


namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for TrialReqView.
	/// </summary>
	public partial class TrialReqView : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.TrialReqEdit", typeof(TrialReqView).Assembly);

		public string sXML = "";
		private int ReqId
		{
			get
			{
				try
				{
					return int.Parse(Request["id"]);
				}
				catch
				{
					return -1;
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				BindToolbar();
				ApplyLocalization();
				BindInfo();
			}
		}

		private void BindToolbar()
		{
			secH.Title = LocRM.GetString("tbViewTitle");
			secH.AddLink("<img alt='' src='../Layouts/Images/edit.gif'/> " + LocRM.GetString("tbEdit"), "../Pages/TrialReqEdit.aspx?id=" + ReqId);
			secH.AddSeparator();
			secH.AddLink("<img alt='' src='../Layouts/Images/delete.gif'/> " + LocRM.GetString("Delete"), "javascript:DeleteRequest()");
			secH.AddSeparator();
			secH.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tbBack"), "../Pages/trialrequests.aspx");
		}

		private void ApplyLocalization()
		{
			lblCompName.Text = LocRM.GetString("tCompName") + ":";
			lblDescr.Text = LocRM.GetString("tDescr") + ":";
			lblDomain.Text = LocRM.GetString("tDomain") + ":";
			lblEMail.Text = LocRM.GetString("tEMail") + ":";
			lblContactName.Text = LocRM.GetString("tContactName") + ":";
			lblLogin.Text = LocRM.GetString("tLogin") + ":";
			lblPassword.Text = LocRM.GetString("tPassword") + ":";
			lblPhone.Text = LocRM.GetString("tPhone") + ":";
			lblCountry.Text = LocRM.GetString("tCountry");
		}

		private void BindInfo()
		{
			if (ReqId > 0)
			{
				string locale = "en-US";
				using (IDataReader reader = DBTrialRequest.Get(ReqId))
				{
					if (reader.Read())
					{
						txtCompName.Text = reader["CompanyName"].ToString();
						txtDescr.Text = reader["Description"].ToString();
						txtDomain.Text = reader["Domain"].ToString();
						txtEMail.Text = reader["Email"].ToString();
						txtContactName.Text = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
						txtLogin.Text = reader["Login"].ToString();
						txtPassword.Text = reader["Password"].ToString();
						txtPhone.Text = reader["Phone"].ToString();
						locale = reader["Locale"].ToString();
						txtCountry.Text = reader["Country"].ToString();

						if (reader["XML"] != DBNull.Value && reader["XML"].ToString() != "")
						{

							string sPath = HttpRuntime.AppDomainAppPath + @"parametres.xslt";
							XmlTextReader xmlRead = new XmlTextReader(reader["XML"].ToString(), XmlNodeType.Document, null);
							XPathDocument doc = new XPathDocument(xmlRead);
							XslCompiledTransform _transform = new XslCompiledTransform();
							_transform.Load(sPath);


							StringWriter sw = new StringWriter();
							XmlTextWriter w = new XmlTextWriter(sw);
							_transform.Transform(doc, w);
							sXML = sw.ToString();
						}
					}
				}

				foreach (ILanguageInfo lang in Configurator.Create().ListLanguages())
				{
					if (lang.Locale == locale)
					{
						lblDefaultLanguage.Text = lang.FriendlyName;
						break;
					}
				}
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		protected void lbDelete_Click(object sender, System.EventArgs e)
		{
			DBTrialRequest.Delete(ReqId);
			Response.Redirect("../pages/trialrequests.aspx");
		}
	}
}
