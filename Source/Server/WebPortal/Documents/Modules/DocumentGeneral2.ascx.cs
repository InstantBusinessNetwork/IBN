namespace Mediachase.UI.Web.Documents.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;

	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.UI.Web.Modules;

	/// <summary>
	///		Summary description for DocumentGeneral.
	/// </summary>
	public partial class DocumentGeneral2 : System.Web.UI.UserControl
	{
		#region Html Vars
		#endregion

		public ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Documents.Resources.strDocuments", typeof(DocumentGeneral2).Assembly);

		#region DocumentId
		private int DocumentId
		{
			get
			{
				try
				{
					return int.Parse(Request["DocumentId"]);
				}
				catch
				{
					throw new AccessDeniedException();
				}
			}
		}
		#endregion

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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			if (!Document.CanViewToDoList(DocumentId))
				ToDoList.Visible = false;
			else
				ToDoList.Visible = true;
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindValues();
			if (Security.CurrentUser.IsExternal)
			{
				ToDoList.Visible = false;
				ctrlDR.Visible = false;
			}
		}
		#endregion

	}
}
