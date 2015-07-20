namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.ControlSystem;
	using ComponentArt.Web.UI;
	using System.Reflection;

	/// <summary>
	///		Summary description for DirectoryTreeView.
	/// </summary>
	public partial class DirectoryTreeView : System.Web.UI.UserControl
	{

		protected System.Web.UI.HtmlControls.HtmlInputHidden txtDisFolderId;

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		BaseIbnContainer bic;
		Mediachase.IBN.Business.ControlSystem.FileStorage fs;


		#region ContainerKey
		public string ContainerKey
		{
			set
			{
				txtContainerKey.Value = value;
			}
			get
			{
				return txtContainerKey.Value;
			}
		}
		#endregion

		#region FolderId
		public int FolderId
		{
			set
			{
				txtFolderId.Value = "";
				try
				{
					txtFolderId.Value = value.ToString();
				}
				catch{}
			}
			get
			{
				int retVal = -1;
				try
				{
					retVal = int.Parse(txtFolderId.Value);
				}
				catch{}
				return retVal;
			}
		}
		#endregion

		#region Folder
		public string Folder
		{
			set
			{
				txtFolder.Text = value;
			}
			get
			{
				return txtFolder.Text;
			}
		}
		#endregion

		#region Height
		public Unit Height
		{
			set
			{
				TreeView1.Height = value;
			}
			get
			{
				return TreeView1.Height;
			}
		}
		#endregion

		#region Width
		public Unit Width
		{
			set
			{
				TreeView1.Width = value;
				txtFolder.Width = value;
			}
			get
			{
				return TreeView1.Width;
			}
		}
		#endregion

		#region ParentNodeImageUrl
		public string ParentNodeImageUrl
		{
			set
			{
				TreeView1.ParentNodeImageUrl = value;
			}
			get
			{
				return TreeView1.ParentNodeImageUrl;
			}
		}
		#endregion

		#region LeafNodeImageUrl
		public string LeafNodeImageUrl
		{
			set
			{
				TreeView1.LeafNodeImageUrl = value;
			}
			get
			{
				return TreeView1.LeafNodeImageUrl;
			}
		}
		#endregion

		#region DisFolderId
		private int disFolderId = -1;
		public int DisFolderId
		{
			set
			{
				disFolderId = value;
			}
			get
			{
				return disFolderId;
			}
		}
		#endregion

		#region RequiredFieldSettings
		public bool RequiredField
		{
			set 
			{
				cvFolder.Enabled = value;
			}
			get
			{
				return cvFolder.Enabled;
			}
		}

		public string ReqValidatorError
		{
			get
			{
				return cvFolder.ErrorMessage;
			}
			set
			{
				cvFolder.ErrorMessage = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			if(!IsPostBack)
				BindDefaultValues();
			BindData();
			BindTree();
		}

		#region BindDefaultValues
		void BindDefaultValues()
		{
			txtFolder.Text = "";

			int iFolderId = -1;
			try{iFolderId = int.Parse(txtFolderId.Value);}
			catch{return;}

			string ContainerKey = txtContainerKey.Value;
			//if(ContainerKey == "")
			//	return;

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
			{
				DirectoryInfo di = fs.GetDirectory(iFolderId);
				if(di != null)
					name = di.Name;
				else
				{
					ContainerKey = "";
					FolderId = -1;
					return;
				}
			}
			//	name = fs.GetDirectory(iFolderId).Name;

			System.IO.StringWriter writer = new System.IO.StringWriter();
			Server.HtmlDecode(name, writer);
			string decoded = writer.ToString();
			txtFolder.Text = decoded;
		}
		#endregion

		#region BindData
		private void BindData()
		{
			string ContainerKey = "Workspace";
			bic = BaseIbnContainer.Create("FileLibrary", ContainerKey);
			fs = (FileStorage)bic.LoadControl("FileStorage");

			TreeViewNode rootNode = new TreeViewNode();
			rootNode.Text = LocRM.GetString("tRoot");

			ProcessFolderNode(rootNode, ContainerKey, fs.Root.Id, LocRM.GetString("tRoot"), false);

			TreeView1.Nodes.Add(rootNode);
		}
		#endregion

		#region BindTree
		private void BindTree()
		{
			TreeView1.MultipleSelectEnabled = false;
			TreeView1.DragAndDropEnabled = false; 
			TreeView1.KeyboardEnabled = true; 
			TreeView1.NodeEditingEnabled = false; 
			TreeView1.KeyboardCutCopyPasteEnabled = false; 
			TreeView1.ShowLines = true;
			TreeView1.DisplayMargin = false;

			TreeViewNode rootNode = new TreeViewNode();
			rootNode.Text = "Projects";
			rootNode.Selectable = false;
			rootNode.CssClass = "TreeNodeGray";
			rootNode.HoverCssClass = "TreeNodeGray";

			DataTable dt = Project.GetListActiveProjectsByUserDataTable();// ActiveProjectsByUserOnlyDataTable();
			foreach (DataRow dr in dt.Rows)
			{
				string ContainerKey = Mediachase.IBN.Business.UserRoleHelper.CreateProjectContainerKey((int)dr["ProjectId"]);
				bic = BaseIbnContainer.Create("FileLibrary", ContainerKey);
				fs = (FileStorage)bic.LoadControl("FileStorage");
				ProcessFolderNode(rootNode, ContainerKey, fs.Root.Id, dr["Title"].ToString(), true);
			}
			
			TreeView1.Nodes.Add(rootNode);
			TreeView1.ExpandAll();
		}
		#endregion

		#region ProcessFolderNode
		private void ProcessFolderNode(TreeViewNode CurTreeNode, string ContainerKey, int iFolder, string RootName, bool RenderRoot)
		{
			string name = "";
			string folder = "";

			bool isRoot = false;

			if(iFolder==fs.Root.Id)
			{
				name += RootName;
				isRoot = true;
			}
			else
				name += fs.GetDirectory(iFolder).Name;

			folder = iFolder.ToString();

			bool fl = fs.CanUserWrite(iFolder);

			DirectoryInfo[] _di = fs.GetDirectories(iFolder);

			System.IO.StringWriter writer = new System.IO.StringWriter();
			Server.HtmlDecode(name, writer);
			string decoded = writer.ToString();

			TreeViewNode childNode;

			if(isRoot && !RenderRoot)
				childNode = CurTreeNode;
			else
			{
				childNode = new TreeViewNode();
				childNode.Text = decoded;
			}
			if(iFolder==DisFolderId)
			{
				childNode.Selectable = false;
				childNode.CssClass = "TreeNodeGray";
				childNode.HoverCssClass = "TreeNodeGray";
			}
			if(fl)
				childNode.ClientSideCommand = "_dtSV('" + decoded + "','" + ContainerKey + "','" + folder + "');";
			else
			{
				childNode.Selectable = false;
				childNode.CssClass = "TreeNodeGray";
				childNode.HoverCssClass = "TreeNodeGray";
			}

			if(Mediachase.IBN.Business.Common.IsPop3Folder(iFolder))
			{
				//childNode.ImageUrl="../layouts/images/folder2.gif";
				childNode.ImageUrl="../layouts/images/folder_mailbox.gif";
			}


			if(ContainerKey==txtContainerKey.Value && iFolder.ToString()==txtFolderId.Value)
				TreeView1.SelectedNode = childNode;

			foreach(DirectoryInfo di in _di)
				ProcessFolderNode(childNode, ContainerKey, di.Id, RootName, true);

			if(!(isRoot && !RenderRoot))
				CurTreeNode.Nodes.Add(childNode);

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
			this.cvFolder.ServerValidate += new ServerValidateEventHandler(cvFolder_ServerValidate);
		}
		#endregion

		private void cvFolder_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if(txtFolderId.Value == string.Empty || txtFolderId.Value == "-1")
				args.IsValid = false;
			else
				args.IsValid = true;
		}
	}
}
