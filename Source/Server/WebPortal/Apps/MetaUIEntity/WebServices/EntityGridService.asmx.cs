using System;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Web.Script.Services;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Apps.MetaUIEntity.WebServices
{
	/// <summary>
	/// Summary description for EntityGridService
	/// </summary>
	[WebService(Namespace = "http://ibn47.mediachase.ru/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	[ScriptService]
	public class EntityGridService : System.Web.Services.WebService
	{
		[WebMethod]
		public void ColumnResize(string ContextKey, int ColumnIndex, int NewSize)
		{
			EntityGridContextKey gridContextKey = UtilHelper.JsonDeserialize<EntityGridContextKey>(ContextKey);
			Mediachase.IBN.Business.UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
			pc[String.Format("{0}_{1}_{2}_{3}", gridContextKey.ClassName, gridContextKey.ProfileName, gridContextKey.PlaceName, (ColumnIndex - gridContextKey.CustomColumnsCount).ToString())] = NewSize.ToString();
		}
	}
}
