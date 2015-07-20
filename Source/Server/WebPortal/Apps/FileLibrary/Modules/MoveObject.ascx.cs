using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Resources;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.FileLibrary.Modules
{
	public partial class MoveObject : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strLibraryAdvanced", typeof(MoveObject).Assembly);

		#region Request Variables
		protected string ContainerKey
		{
			get
			{
				if (Request["ContainerKey"] != null)
					return Request["ContainerKey"];
				else
					return "";
			}
		}

		protected string ContainerName
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

		Mediachase.IBN.Business.ControlSystem.FileStorage fs;

		protected void Page_Load(object sender, EventArgs e)
		{
			GetCurrentFolderAndMoveObjects();
			BindElements();
			this.btnMove.ServerClick += new EventHandler(btnMove_ServerClick);

			if (Request["closeFramePopup"] != null)
			{
				this.btnCancel.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{}}", Request["closeFramePopup"]));
			}
		}
		#region GetCurrentFolderAndMoveObjects
		private void GetCurrentFolderAndMoveObjects()
		{
			fs = Mediachase.IBN.Business.ControlSystem.FileStorage.Create(ContainerName, ContainerKey);

			if(Request["ParentFolderId"] != null)
				ctrlDirTree.DisFolderId = int.Parse(Request["ParentFolderId"]);
		}
		#endregion

		#region BindElements
		private void BindElements()
		{
			btnMove.Attributes.Add("onclick", "DisableButtons(this);");
			btnCancel.Attributes.Add("onclick", "window.close();");
			lblMoveTo.Text = LocRM.GetString("MoveTo");
			btnMove.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
			btnMove.CustomImage = this.Page.ResolveUrl("~/layouts/images/upload.gif");
		}
		#endregion

		#region MoveEvent
		private void btnMove_ServerClick(object sender, EventArgs e)
		{
			int iDestFolder = -1;
			try
			{
				iDestFolder = ctrlDirTree.FolderId;
			}
			catch { }
			if (iDestFolder == -1)
			{
				lblNotValid.Visible = true;
				return;
			}

			CommandParameters cp = new CommandParameters("FL_Selected_Move");
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("DestFolderId", iDestFolder.ToString());
			cp.AddCommandArgument("ContainerKey", ContainerKey);
			cp.AddCommandArgument("ContainerName", ContainerName);

			if (Request["GridId"] != null)
			{
				cp.CommandName = "FL_Selected_MoveToFolder";
				cp.AddCommandArgument("GridId", Request["GridId"]);
			}
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion
	}
}