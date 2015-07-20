using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Web.UI;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Util;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.CommandHandlers
{
	public class AddToFavoritesEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;
			NameValueCollection qs = ((Control)Sender).Page.Request.QueryString;
			if (!String.IsNullOrEmpty(qs["ClassName"]) && !String.IsNullOrEmpty(qs["ObjectId"]))
			{
				string objectUid = qs["ObjectId"];
				ObjectTypes objectType = CommonHelper.GetObjectTypeByClassName(qs["ClassName"]);
				if (objectType != ObjectTypes.UNDEFINED)
				{
					retval = !Mediachase.IBN.Business.Common.CheckFavoritesByUid(new Guid(objectUid), objectType);
				}
			}

			return retval;
		}
		#endregion
	}
}
