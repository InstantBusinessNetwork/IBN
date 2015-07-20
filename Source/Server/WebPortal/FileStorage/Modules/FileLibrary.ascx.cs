namespace Mediachase.UI.Web.FileStorage.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;

	/// <summary>
	///		Summary description for FileLibrary.
	/// </summary>
	public partial class FileLibrary : System.Web.UI.UserControl, ITopTabs, IToolbarLight
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strFileLibrary", typeof(FileLibrary).Assembly);

		private string pcKey = "WSFS_CurrentTab";

		#region Tab
		private string tab;
		protected string Tab
		{
			get
			{
				if (tab == null)
				{
					tab = "";
					if (Request["Tab"] != null)
						tab = Request["Tab"];
				}
				return tab;
			}
		}
		#endregion

		#region TypeId
		private int TypeId
		{
			get
			{
				if (Request["TypeId"] != null && Request["TypeId"] != String.Empty)
					return int.Parse(Request["TypeId"]);
				else
					return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindTabs();
		}

		#region BindTabs
		private void BindTabs()
		{
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;

			if (Tab != null && (Tab == "0" || Tab == "1" || Tab == "2"))
				pc[pcKey] = Tab;
			else if (pc[pcKey] == null)
				pc[pcKey] = "1";

			if (TypeId < 0)
				ctrlTopTab.AddTab(LocRM.GetString("tFileStorage"), "0");
			else if (pc[pcKey] == "0")
				pc[pcKey] = "1";
			ctrlTopTab.AddTab(LocRM.GetString("tFileList"), "1");
			ctrlTopTab.AddTab(LocRM.GetString("tSearch"), "2");

			string controlName = "";
			string selectedTab = pc[pcKey];

			if (!IsPostBack)
			{
				switch (selectedTab)
				{
					case "0":
						controlName = "../../FileStorage/Modules/FileStorageControl.ascx";
						break;
					case "1":
						controlName = "../../FileStorage/Modules/FilesList.ascx";
						break;
					case "2":
						controlName = "../../FileStorage/Modules/FileSearch.ascx";
						break;
					default:
						break;
				}
				ViewState["controlName"] = controlName;
				ViewState["selectedTab"] = selectedTab;
			}
			else
			{
				controlName = (string)ViewState["controlName"];
				selectedTab = (string)ViewState["selectedTab"];
			}

			System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
			phItems.Controls.Add(control);
			ctrlTopTab.SelectItem(selectedTab);
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

		#region Implementation of ITopTabs
		public Mediachase.UI.Web.Modules.TopTabs GetTopTabs()
		{
			return ctrlTopTab;
		}
		#endregion

		#region Implementation of IToolbarLight
		public Mediachase.UI.Web.Modules.BlockHeaderLightWithMenu GetToolBar()
		{
			return tbLightFS;
		}
		#endregion
	}
}
