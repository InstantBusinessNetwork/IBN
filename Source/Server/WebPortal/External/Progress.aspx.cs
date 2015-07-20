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
using System.Resources;
using Mediachase.FileUploader.Web;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.External
{
	/// <summary>
	/// Summary description for Progress.
	/// </summary>
	public partial class Progress : System.Web.UI.Page
	{
		private int percents;

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strFileLibrary", typeof(Progress).Assembly);

		protected string Percents
		{
			get
			{
				if (percents > 0)
					return percents.ToString() + LocRM.GetString("tUplCompl");
				else
					return LocRM.GetString("tWait");
			}
		}

		protected string PercentsAbs
		{
			get
			{
				return percents.ToString() + "%";
			}
		}

		protected new string ID
		{
			get
			{
				return Request["ID"];
			}
		}



		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");

			cbClose.Text = LocRM.GetString("tClose");
			if (ID != null)
			{
				dClose.Visible = false;
				Guid progressUid = new Guid(ID);
				UploadProgressInfo upi = UploadProgress.Provider.GetInfo(progressUid);

				int Received = 0;
				int Total = 0;
				bool PostCompleted = false;

				if (upi != null)
				{
					Received = upi.BytesReceived;
					Total = upi.BytesTotal;
					PostCompleted = upi.Result == UploadResult.Succeeded;
				}
				lblTransferred.Text = (Received / 1024).ToString();
				if (Total > 0)
				{
					percents = (int)((float)Received / (float)Total * 100);
					lblTransferred.Text = LocRM.GetString("tUploaded") + " " + (Received / 1024).ToString() + " Kb " + LocRM.GetString("tof") + " " + (Total / 1024).ToString() + " Kb";
				}
				else
				{
					lblTransferred.Text = LocRM.GetString("tWait");
					percents = 0;
				}

				if (PostCompleted)
				{
					if (Total != Received)
					{
						lblTransferred.Text = LocRM.GetString("tUplFail");
						dClose.Visible = true;
					}
					else
					{
						lblTransferred.Text = LocRM.GetString("tUplComplSucces");
						dClose.Visible = true;
						if (cbClose.Checked) dWinClose.Visible = true;
					}
				}
				else
				{
					body.Attributes.Add("onload", "setTimeout('refresh()', 2000)");
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
