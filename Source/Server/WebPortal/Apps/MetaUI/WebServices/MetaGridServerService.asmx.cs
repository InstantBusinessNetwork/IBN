using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Apps.MetaUI.Grid;

namespace Mediachase.UI.Web.Apps.MetaUI.WebServices
{
	/// <summary>
	/// Summary description for MetaGridServerService
	/// </summary>
	[WebService(Namespace = "http://ibn47.medichasae.ru/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	[ScriptService]
	public class MetaGridServerService : System.Web.Services.WebService
	{
		private McMetaViewPreference mvPref;

		#region SaveMetaViewPreferences
		private void SaveMetaViewPreferences()
		{
			if (mvPref == null)
				throw new ArgumentException("mvPref is null @ SaveMetaViewPreferences");

			Mediachase.Ibn.Core.UserMetaViewPreference.Save(Mediachase.Ibn.Data.Services.Security.CurrentUserId, mvPref);
		}
		#endregion

		#region LoadMetaViewPreference
		private void LoadMetaViewPreference(string ContextKey)
		{
			MetaView view = GetViewByName(ContextKey);
			mvPref = Mediachase.Ibn.Core.UserMetaViewPreference.Load(view, Mediachase.Ibn.Data.Services.Security.CurrentUserId);
		}
		#endregion

		#region GetViewByName
		/// <summary>
		/// Gets the name of the view by.
		/// </summary>
		/// <param name="ContextKey">The context key.</param>
		/// <returns></returns>
		public MetaView GetViewByName(string ContextKey)
		{
			MetaView retVal = null;

			retVal = Mediachase.Ibn.Data.DataContext.Current.MetaModel.MetaViews[ContextKey];

			if (retVal == null)
			{
				throw new ArgumentException(String.Format("Cant find view with name: {0}", ContextKey));
			}

			return retVal;
		}
		#endregion

		[WebMethod]
		public void ColumnResize(string ContextKey, int ColumnIndex, int NewSize)
		{
			MetaGridContextKey gridContextKey = UtilHelper.JsonDeserialize<MetaGridContextKey>(ContextKey);

			LoadMetaViewPreference(gridContextKey.ViewName);
			mvPref.SetMetaFieldWidth(ColumnIndex - gridContextKey.CustomColumnsCount, NewSize);
			SaveMetaViewPreferences();
		}
	}
}
