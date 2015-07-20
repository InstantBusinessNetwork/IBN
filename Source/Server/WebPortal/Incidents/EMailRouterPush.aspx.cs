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
using Mediachase.IBN.Business.EMail;
using Mediachase.IBN.Business.SpreadSheet;
using System.Text;
using Mediachase.IBN.Business;
using Mediachase.Net.Mail;

namespace Mediachase.UI.Web.Incidents
{
    /// <summary>
    /// Summary description for EMailRouterPush.
    /// </summary>
    public partial class EMailRouterPush : System.Web.UI.Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Put user code to initialize the page here
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

        protected void routeEMailBoxesButton_Click(object sender, System.EventArgs e)
		{
//			IncidentInfo info = new IncidentInfo();
//
//			info.EMailBox = 13;
//			info.MailSenderEmail = "zhukoo@mail.ru";
//			info.Title = "Test";
//			info.Description = "";
//
//			IncidentBox box = IncidentBoxRule.Evaluate(info);

//			IncidentInfo incidentInfo = new IncidentInfo();
//
//			incidentInfo.Title = "SUper Хакер!";
//
//			incidentInfo.GeneralCategories.Add(1);
//			incidentInfo.GeneralCategories.Add(75);
//			incidentInfo.GeneralCategories.Add(57);
//
//			IncidentBox incidentBox = IncidentBoxRule.Evaluate(incidentInfo); 

//			SpreadSheetView view = ProjectSpreadSheet.LoadView(335,0,2006,2006);
//			ExpressionParser parser = new ExpressionParser();
//			double result1 = parser.Parse("[2006-Q1-01:NetSales]",view);
			
			//Mediachase.IBN.Business.EMail.EMailRouterScheduleClientInvoker.Invoke();
			//SmtpClient box = SmtpClientUtility.CreateSmtpClient("HelpDeskEmailBox:1");


			// Check Open State
			// Chech New State
			// Check Control state

			//int IncidentId = 8290;

			//IncidentBoxDocument document = IncidentBoxDocument.Load(Incident.GetIncidentBox(IncidentId));
			//XIbnHeaderCommand.SetIncidentState(nodeType, document, IncidentId, ThreadNodeId, cmdObjectStates);

			//XIbnHeaderCommand.ChangeResponsible(IncidentId, 65); // Not Set -1, Group = -2

            //bool AllowOverride = Mediachase.IBN.Business.SmtpSettings.AllowOverride();
            //bool IsCustom = Mediachase.IBN.Business.SmtpSettings.IsCustom();

            //Mediachase.IBN.Business.SmtpSettings settings = Mediachase.IBN.Business.SmtpSettings.GetDefault();

            //string EmlMessage = "Date: Wed, 14 Mar 2007 10:04:56 +0200" + "\r\n" +
            //                    "From: Oleg Zhuk <zhuk@mediachase.com>" + "\r\n" +
            //                    "Organization: Mediachase"+ "\r\n" +
            //                    "User-Agent: Thunderbird 1.5.0.10 (Windows/20070221)"+ "\r\n" +
            //                    "MIME-Version: 1.0"+ "\r\n" +
            //                    "To: Oleg Zhuk <zhukoo@mail.ru>"+ "\r\n" +
            //                    "Subject: Test"+ "\r\n" +
            //                    "Content-Type: text/plain; charset=UTF-8; format=flowed"+ "\r\n" +
            //                    "Content-Transfer-Encoding: 8bit"+ "\r\n" +
            //                    ""+"\r\n"+ 
            //                    "Test.\r\n";

            //SmtpClient.SendMessage("zhuk@mediachase.com", "zhukoo@mail.ru", "Test", Encoding.Default.GetBytes(EmlMessage));
		}
    }
}
