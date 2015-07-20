using System;
using System.Data;
using System.IO;

using Mediachase.Ibn;
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Common
{
	/// <summary>
	/// Summary description for GroupLogo.
	/// </summary>
	public partial class GroupLogo : System.Web.UI.Page
	{
		private int IMGroupID
		{
			get
			{
				return int.Parse(Request["GroupID"]);
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{

			Response.ContentType = "image/jpeg";

			int bufferSize = 100;
			byte[] outbyte = new byte[bufferSize];
			long retVal;
			long startIndex = 0;

			using (IDataReader reader = IMGroup.GetBinaryClientLogo(IMGroupID))
			{
				if (reader.Read())
				{
					startIndex = 0;
					retVal = reader.GetBytes(0, startIndex, outbyte, 0, bufferSize);

					if (retVal == 0)
					{
						Response.Redirect(GlobalResourceManager.Strings["IMGroupLogoUrl"], true);
					}

					Stream stream = Response.OutputStream;
					while (retVal == bufferSize)
					{
						stream.Write(outbyte, 0, bufferSize);
						startIndex += bufferSize;
						retVal = reader.GetBytes(0, startIndex, outbyte, 0, bufferSize);
					}
					stream.Write(outbyte, 0, System.Convert.ToInt32(retVal));
				}
				else
				{
					Response.Redirect(GlobalResourceManager.Strings["IMGroupLogoUrl"], true);
				}

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
