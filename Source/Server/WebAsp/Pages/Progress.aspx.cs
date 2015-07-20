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
using Mediachase.FileUploader.Web;

namespace Mediachase.Ibn.WebAsp.Pages
{
	/// <summary>
	/// Summary description for Progress.
	/// </summary>
	public partial class Progress : System.Web.UI.Page
	{
		private int percents;
		
		protected string Percents
		{
			get
			{
				if (percents>0)
					return percents.ToString()+ "% of Uploading Completed";
				else
					return "Wait for Uploading Starts";
			}
		}

		protected string PercentsAbs
		{
			get
			{
				return percents.ToString()+ "%";
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
			cbClose.Text = "Close this box when upload completes";
			if (ID!=null)
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
				lblTransferred.Text = (Received/1024).ToString();
				if (Total > 0) 
				{		
					percents = (int)((float)Received / (float)Total * 100);
					lblTransferred.Text = "Uploaded " + (Received/1024).ToString() +" Kb of " + (Total/1024).ToString() + " Kb";
				}
				else
				{
					lblTransferred.Text = "Wait for Uploading Starts";
					percents = 0;
				}

				if (PostCompleted)
				{
					if (Total != Received)
					{
						lblTransferred.Text = "Uploading Failed";
						dClose.Visible = true;
					}
					else
					{
						lblTransferred.Text = "Uploading Completed Sucessfully";
						dClose.Visible = true;
						if (cbClose.Checked) dWinClose.Visible = true;
					}
				}
				else
				{
					body.Attributes.Add("onload","setTimeout('refresh()', 2000)");
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
