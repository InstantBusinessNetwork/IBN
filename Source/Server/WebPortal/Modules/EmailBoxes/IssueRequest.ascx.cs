namespace Mediachase.UI.Web.Modules.EmailBoxes
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.Pop3;
	using System.Reflection;

	/// <summary>
	///		Summary description for IssueRequest.
	/// </summary>
	public partial class IssueRequest : System.Web.UI.UserControl, IPersistPop3MessageHandlerStorage
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here

			ApplyLocalization();

//			Pop3Box currentBox = Pop3Manager.Current.SelectedPop3Box;
//			if(!IsPostBack)
//			{
//				cbAutoApprove.Checked=currentBox.Parameters["AutoApproveForKnown"]=="1";
//				cbUseExternal.Checked=currentBox.Parameters["OnlyExternalSenders"]=="1";
//				cbAutoDelete.Checked=currentBox.Parameters["AutoKillForUnknown"]=="1";
//			}
//			else
//			{
//				//postback
//				currentBox.Parameters["AutoApproveForKnown"] = this.Request.Form[cbAutoApprove.UniqueID]=="on"?"1":"0";//cbAutoApprove.Checked?"1":"0";
//				currentBox.Parameters["OnlyExternalSenders"] = this.Request.Form[cbUseExternal.UniqueID]=="on"?"1":"0";
//				currentBox.Parameters["AutoKillForUnknown"] = this.Request.Form[cbAutoDelete.UniqueID]=="on"?"1":"0";
//				//currentBox.Parameters["OnlyExternalSenders"] = cbUseExternal.Checked?"1":"0";
//			}
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			cbAutoApprove.Text=LocRM.GetString("tAutoApprove");
			cbAutoDelete.Text=LocRM.GetString("tAutoDelete");
			cbUseExternal.Text=LocRM.GetString("tUseExternal");
			cbSaveMessageBodyAsEml.Text=LocRM.GetString("tSaveMessageBodyEml");
			cbSaveMessageBodyAsMsg.Text=LocRM.GetString("tSaveMessageBodyMsg");
			cbSaveMessageBodyAsMht.Text=LocRM.GetString("tSaveMessageBodyMhtPrev");
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
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region IPersistPop3MessageHandlerStorage Members

		void IPersistPop3MessageHandlerStorage.Load(Pop3Box box)
		{
			cbAutoApprove.Checked=box.Parameters["AutoApproveForKnown"]=="1";
			cbUseExternal.Checked=box.Parameters["OnlyExternalSenders"]=="1";
			cbAutoDelete.Checked=box.Parameters["AutoKillForUnknown"]=="1";
			cbSaveMessageBodyAsEml.Checked=box.Parameters["SaveMessageBodyAsEml"]=="1";
			cbSaveMessageBodyAsMsg.Checked=box.Parameters["SaveMessageBodyAsMsg"]=="1";
			if(box.Parameters["SaveMessageBodyAsMht"] != null)
				cbSaveMessageBodyAsMht.Checked=box.Parameters["SaveMessageBodyAsMht"]=="1";
			else cbSaveMessageBodyAsMht.Checked = true;
		}

		void IPersistPop3MessageHandlerStorage.Save(Pop3Box box)
		{
			box.Parameters["AutoApproveForKnown"] = cbAutoApprove.Checked?"1":"0";//cbAutoApprove.Checked?"1":"0";
			box.Parameters["OnlyExternalSenders"] = cbUseExternal.Checked?"1":"0";
			box.Parameters["AutoKillForUnknown"] = cbAutoDelete.Checked?"1":"0";
			box.Parameters["SaveMessageBodyAsEml"] = cbSaveMessageBodyAsEml.Checked?"1":"0";
			box.Parameters["SaveMessageBodyAsMsg"] = cbSaveMessageBodyAsMsg.Checked?"1":"0";
			box.Parameters["SaveMessageBodyAsMht"] = cbSaveMessageBodyAsMht.Checked?"1":"0";
		}

		#endregion
	}
}
