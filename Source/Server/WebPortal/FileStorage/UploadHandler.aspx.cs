using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.FileStorage
{
	/// <summary>
	/// Summary description for UploadHandler.
	/// </summary>
	public partial class UploadHandler : System.Web.UI.Page
	{
		#region External Query Properties
		public string ExtId
		{
			get
			{
				return Request["ExtId"];
			}
		}
		#endregion

		#region Request Variables
		private int FolderId
		{
			get
			{
				if (Request["FolderId"] != null)
					return int.Parse(Request["FolderId"]);
				else
					return -1;
			}
		}

		private string ContainerKey
		{
			get
			{
				if (Request["ContainerKey"] != null)
					return Request["ContainerKey"];
				else
					return "";
			}
		}

		private string ContainerName
		{
			get
			{
				if (Request["ContainerName"] != null)
					return Request["ContainerName"];
				else
					return "";
			}
		}
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strAddDoc", Assembly.GetExecutingAssembly());

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");

			if (this.IsPostBack)
			{
				string _containerName = (ContainerName != "") ? ContainerName : "FileLibrary";
				string _containerKey = (ContainerKey != "") ? ContainerKey : hidCFUKey.Value;
				if (Mediachase.IBN.Business.Security.CurrentUser == null)
					CheckExternal(_containerKey);
				int _folderId = (FolderId > 0) ? FolderId : int.Parse(hidFFUId.Value);
				Mediachase.IBN.Business.ControlSystem.BaseIbnContainer bic = Mediachase.IBN.Business.ControlSystem.BaseIbnContainer.Create(_containerName, _containerKey);
				Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");
				string sFileName = hidFName.Value;
				//win fix
				if (sFileName.LastIndexOf("\\") >= 0)
					sFileName = sFileName.Substring(sFileName.LastIndexOf("\\") + 1);
				//linux fix 
				if (sFileName.LastIndexOf("/") >= 0)
					sFileName = sFileName.Substring(sFileName.LastIndexOf("/") + 1);
				fs.SaveFile(_folderId, sFileName, textDescription.Text, McFileUp.PostedFile.InputStream);
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

		#region CheckExternal()
		private void CheckExternal(string _containerKey)
		{
			if (!String.IsNullOrEmpty(ExtId))
			{
				try
				{
					string[] mas = _containerKey.Split('_');
					int ObjectTypeId = -1;
					switch (mas[0].ToLower())
					{
						case "taskid":
							ObjectTypeId = (int)ObjectTypes.Task;
							break;
						case "todoid":
							ObjectTypeId = (int)ObjectTypes.ToDo;
							break;
						case "eventid":
							ObjectTypeId = (int)ObjectTypes.CalendarEntry;
							break;
						case "documentid":
						case "documentvers":
							ObjectTypeId = (int)ObjectTypes.Document;
							break;
						case "incidentid":
							ObjectTypeId = (int)ObjectTypes.Issue;
							break;
						default:
							break;
					}
					if (Mediachase.IBN.Business.User.CheckUserIdByExternalGate(ObjectTypeId, int.Parse(mas[1]), int.Parse(ExtId)))
					{
						UserLight userLight = UserLight.Load(int.Parse(ExtId));
						HttpContext context = HttpContext.Current;
						context.Items.Add("userlight", userLight);
					}
					else
						throw new AccessDeniedException();
				}
				catch
				{
					throw new AccessDeniedException();
				}
			}
		}
		#endregion
	}
}
