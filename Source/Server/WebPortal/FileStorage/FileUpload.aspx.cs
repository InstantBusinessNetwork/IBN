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

namespace Mediachase.UI.Web.FileStorage
{
	/// <summary>
	/// Summary description for FileUpload.
	/// </summary>
	public partial class FileUpload : System.Web.UI.Page
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strAddDoc", typeof(FileUpload).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
      pT.Title = LocRM.GetString("tAddDoc");
		}
	}
}
