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
using Mediachase.IBN.Business;


namespace Mediachase.IBN.UI.Web.FileLibrary
{
	/// <summary>
	/// Summary description for FileDownload.
	/// </summary>
	public partial class FileDownload : System.Web.UI.Page
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		protected void btnSubmit_Click(object sender, System.EventArgs e)
		{
			if(this.IsPostBack)
			{
				try
				{
					string sUserLight = "userlight";
		
					// check user's name and password here
					UserLight currentUser = Security.GetUser(Login.Value, Password.Value);
					if(currentUser==null)
						throw new HttpException(405,null);	

					// Security Addon [3/2/2004]
					UserLight retUser = null;
					if(HttpContext.Current.Items.Contains(sUserLight))
					{
						retUser = (UserLight)HttpContext.Current.Items[sUserLight];
						HttpContext.Current.Items.Remove(sUserLight);
					}
					HttpContext.Current.Items.Add(sUserLight, currentUser);
					// End Security Addon [3/2/2004]

					int AssetVersionId	= Int32.Parse(VersionID.Value);

					// New Folder System Addon [12/27/2005]
					string ContainerName = "FileLibrary";
					string ContainerKey = "Workspace";
					
					Mediachase.IBN.Business.ControlSystem.BaseIbnContainer bic = Mediachase.IBN.Business.ControlSystem.BaseIbnContainer.Create(ContainerName, ContainerKey);
					Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

					fs.LoadFile(AssetVersionId, Response.OutputStream);

					// EndNew Folder System Addon [12/27/2005] 

					/*
					int FolderId		= Int32.Parse(FolderID.Value);
					using (IDataReader assetVersion = Asset.GetAssetVersion(FolderId,ObjectTypes.Folder,AssetVersionId))
					{
						if(assetVersion.Read())
						{
							string	fileUrl	= (string)assetVersion["URL"];
							int		assetId = (int)assetVersion["AssetId"];

							Asset.IncDownloadCount(assetId);

							if((bool)assetVersion["IsInternal"])
							{
								DSFile.DownloadFile(fileUrl,Response);
							}
							else
							{
								Response.Redirect(fileUrl);
							}
						}
					}
					*/

					// Security Addon [3/2/2004]
					HttpContext.Current.Items.Remove(sUserLight);
					HttpContext.Current.Items.Add(sUserLight, retUser);
					// End Security Addon [3/2/2004]
				}
				catch
				{
					throw new HttpException(405,null);	
				}
				Response.End();
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
