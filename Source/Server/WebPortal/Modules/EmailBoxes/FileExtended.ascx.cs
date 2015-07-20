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
	using Mediachase.IBN.Business.ControlSystem;
	using ComponentArt.Web.UI;
	using System.Reflection;

	/// <summary>
	///		Summary description for FileExtended.
	/// </summary>
	public partial class FileExtended : System.Web.UI.UserControl, IPersistPop3MessageHandlerStorage
	{

		/*
		protected System.Web.UI.WebControls.TextBox txtFolder;
		protected System.Web.UI.HtmlControls.HtmlInputHidden txtContainerKey;
		protected System.Web.UI.HtmlControls.HtmlInputHidden txtFolderId;
		protected ComponentArt.Web.UI.TreeView TreeView1;
		*/
		//protected System.Web.UI.WebControls.CustomValidator cvFolder;


		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			ApplyLocalization();
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			cbSaveMessageBodyAsEml.Text=LocRM.GetString("tSaveMessageBodyEml");
			cbSaveMessageBodyAsMsg.Text=LocRM.GetString("tSaveMessageBodyMsg");
			cbSaveMessageBodyAsMht.Text=LocRM.GetString("tSaveMessageBodyMht");
			cbAutoDelete.Text=LocRM.GetString("tAutoDelete");
			cbUseExternal.Text=LocRM.GetString("tUseExternal");

			txtFolderPattern.Attributes.Add("onchange","VerifyPattern('" + txtFolderPattern.ClientID + "');");
		}
		#endregion

		/*
		#region cvFolder_ServerValidate
		private void cvFolder_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			//if(txtFolderId.Value == string.Empty)
			if(dtv.FolderId == -1)
				args.IsValid = false;
			else
				args.IsValid = true;
		}
		#endregion
		*/

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
			//this.cvFolder.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cvFolder_ServerValidate);
		}
		#endregion

		#region IPersistPop3MessageHandlerStorage Members

		void IPersistPop3MessageHandlerStorage.Load(Pop3Box box)
		{
			cbSaveMessageBodyAsEml.Checked=box.Parameters["SaveMessageBodyAsEml"]=="1";
			cbSaveMessageBodyAsMsg.Checked=box.Parameters["SaveMessageBodyAsMsg"]=="1";
			cbSaveMessageBodyAsMht.Checked=box.Parameters["SaveMessageBodyAsMht"]=="1";
			cbUseExternal.Checked=box.Parameters["OnlyExternalSenders"]=="1";
			cbAutoDelete.Checked=box.Parameters["AutoKillForUnknown"]=="1";

			//txtFolderId.Value = "";
			int iFolderId = -1;
			if(box.Parameters["FolderId"] != null)
			{
				try{iFolderId = int.Parse(box.Parameters["FolderId"]);}
				catch{}
				/*
				if(iFolderId != -1)
					txtFolderId.Value = iFolderId.ToString();
				*/
			}

			dtv.FolderId=iFolderId;

			/*
			txtContainerKey.Value = "";
			txtFolder.Text = "";
			*/
			string ContainerKey = "";
			if(box.Parameters["ContainerKey"] != null)
			{
				ContainerKey = box.Parameters["ContainerKey"];
				/*
				txtContainerKey.Value = ContainerKey;

				bic = BaseIbnContainer.Create("FileLibrary", ContainerKey);
				fs = (FileStorage)bic.LoadControl("FileStorage");

				string name = "unknown";
				if(iFolderId==fs.Root.Id)
				{
					if(ContainerKey.LastIndexOf("_") > 0)
					{
						int ProjectId = -1;
						try
						{
							ProjectId = int.Parse(ContainerKey.Substring(ContainerKey.LastIndexOf("_")+1));
						}
						catch{}
						if(ProjectId != -1)
						{
							using(IDataReader reader = Project.GetProject(ProjectId))
							{
								reader.Read();
								if(reader["Title"] != null)
									name = reader["Title"].ToString();
							}
						}
					}
					else
						name = LocRM.GetString("tRoot");
				}
				else
					name = fs.GetDirectory(iFolderId).Name;

				System.IO.StringWriter writer = new System.IO.StringWriter();
				Server.HtmlDecode(name, writer);
				string decoded = writer.ToString();
				txtFolder.Text = decoded;
				*/
			}

			dtv.ContainerKey=ContainerKey;

			if(box.Parameters["FolderPattern"] != null)
				txtFolderPattern.Text = box.Parameters["FolderPattern"];
			else txtFolderPattern.Text = "";
		}

		void IPersistPop3MessageHandlerStorage.Save(Pop3Box box)
		{
			box.Parameters["SaveMessageBodyAsEml"] = cbSaveMessageBodyAsEml.Checked?"1":"0";
			box.Parameters["SaveMessageBodyAsMsg"] = cbSaveMessageBodyAsMsg.Checked?"1":"0";
			box.Parameters["SaveMessageBodyAsMht"] = cbSaveMessageBodyAsMht.Checked?"1":"0";
			box.Parameters["OnlyExternalSenders"] = cbUseExternal.Checked?"1":"0";
			box.Parameters["AutoKillForUnknown"] = cbAutoDelete.Checked?"1":"0";

			string ContainerKey = "";
			int iFolderId = -1;
			/*
			ContainerKey = txtContainerKey.Value;
			try
			{
				iFolderId = int.Parse(txtFolderId.Value);
			}
			catch{}
			*/

			iFolderId = dtv.FolderId;
			ContainerKey = dtv.ContainerKey;

			if(iFolderId != -1)
				box.Parameters["FolderId"] = iFolderId.ToString();
			if(ContainerKey != "")
				box.Parameters["ContainerKey"] = ContainerKey;

			box.Parameters["FolderPattern"] = txtFolderPattern.Text;
		}

		#endregion
	}
}
