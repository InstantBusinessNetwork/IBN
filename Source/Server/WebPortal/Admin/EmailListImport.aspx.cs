using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Resources;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail;
using System.Reflection;
using Mediachase.Ibn.Web.UI.WebControls;


namespace Mediachase.UI.Web.Admin
{
	/// <summary>
	/// Summary description for EmailListImport.
	/// </summary>
	public partial class EmailListImport : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));


		public string ListType
		{
			get
			{
				if(Request["listtype"]!=null)
					return Request["listtype"].ToString();
				return string.Empty;
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/dialog.css");

			ApplyLocalization();
			btnSave.CustomImage = this.ResolveUrl("~/Layouts/Images/saveitem.gif");
		}

		private void ApplyLocalization()
		{
			btnSave.Text = LocRM.GetString("tImportList");
			secHeader.Title = LocRM.GetString("tImportList");
			secHeader.AddLink(
				"<img alt='' src='" + ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tClose"),
				"javascript:window.close();");
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
			this.btnSave.ServerClick+=new EventHandler(btnSave_ServerClick);
		}
		#endregion

		private void btnSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			if (mcImportFile.PostedFile!=null && mcImportFile.PostedFile.ContentLength>0)
			{
				string sText = string.Empty;
				using(StreamReader _reader = new StreamReader(mcImportFile.PostedFile.InputStream))
				{
					sText = _reader.ReadToEnd();
				}

				string regex = "([0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*@(([0-9a-zA-Z])+([-\\w]*[0-9a-zA-Z])*\\.)+[a-zA-Z]" +
					"{2,9})";
				System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline) 
					| System.Text.RegularExpressions.RegexOptions.IgnoreCase);
				System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(regex, options);

				foreach(Match	item in reg.Matches(sText))
				{
					if(ListType=="Black" && !BlackListItem.Contains(item.Value))
					{
						BlackListItem.Create(item.Value);
					}
					if(ListType=="White" && !WhiteListItem.Contains(item.Value))
					{
						WhiteListItem.Create(item.Value);
					}
				}

			}
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				"try {window.opener.location.href=window.opener.location.href;}" +
				"catch (e){} window.close();", true);
		}
	}
}
