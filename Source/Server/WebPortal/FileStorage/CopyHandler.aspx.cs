using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.FileStorage
{
	/// <summary>
	/// Summary description for CopyHandler.
	/// </summary>
	public partial class CopyHandler : System.Web.UI.Page
	{


		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strLibraryAdvanced", typeof(CopyHandler).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			if (!Page.IsPostBack && Request["Files"] != null)
			{
				UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
				int iCount = 10;
				if (pc["ClipboardItemsCount"] != null)
					iCount = int.Parse(pc["ClipboardItemsCount"].ToString());

				ArrayList alFiles = new ArrayList();
				string sFiles = Request["Files"];
				while (sFiles.Length > 0)
				{
					alFiles.Add(sFiles.Substring(0, sFiles.IndexOf(",")));
					sFiles = sFiles.Remove(0, sFiles.IndexOf(",") + 1);
				}
				foreach (string sFileId in alFiles)
				{
					int FileId = int.Parse(sFileId);
					string sNewFileClip = "";
					if (pc["ClipboardFiles"] != null)
						sNewFileClip = pc["ClipboardFiles"].ToString();
					sNewFileClip = WorkWithClipboard(iCount, sFileId + "|" + sNewFileClip);
					pc["ClipboardFiles"] = sNewFileClip;
				}

				lblResult.Text = String.Format(LocRM.GetString("tWereAdded"), alFiles.Count);
			}
		}

		private string WorkWithClipboard(int iCount, string sClip)
		{
			int pCount = 0;
			string sCheck = sClip;
			int[] iArray = new int[iCount + 1];
			ArrayList aList = new ArrayList();
			while (sCheck.Length > 0)
			{
				if (sCheck.IndexOf("|") >= 0)
				{
					int iObj = int.Parse(sCheck.Substring(0, sCheck.IndexOf("|")));
					if (!aList.Contains(iObj))
					{
						aList.Add(iObj);
						iArray[pCount++] = iObj;
					}
					sCheck = sCheck.Substring(sCheck.IndexOf("|") + 1);
				}
				if (pCount >= iCount)
					break;
			}
			sClip = "";
			for (int i = 0; i < pCount; i++)
				sClip += iArray[i].ToString() + "|";
			return sClip;
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
