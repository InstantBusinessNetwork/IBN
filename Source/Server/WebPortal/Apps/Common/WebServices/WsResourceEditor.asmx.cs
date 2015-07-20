using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Threading;
using Mediachase.Ibn.Web.UI;
using System.Web.Script.Services;

namespace Mediachase.UI.Web.Apps.Common.WebServices
{
	/// <summary>
	/// Summary description for WsResourceEditor
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[ScriptService]
	public class WsResourceEditor : System.Web.Services.WebService
	{

		[WebMethod]
		public string CheckResource(string value, string contextKey)
		{
			return CHelper.GetResFileString(value);
		}
	}
}
