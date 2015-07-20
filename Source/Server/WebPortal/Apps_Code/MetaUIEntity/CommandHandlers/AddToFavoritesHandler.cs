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
	public class AddToFavoritesHandler : ICommand
	{
		#region ICommand Members
		public void Invoke(object Sender, object Element)
		{
			NameValueCollection qs = ((Control)Sender).Page.Request.QueryString;
			if (!String.IsNullOrEmpty(qs["ClassName"]) && !String.IsNullOrEmpty(qs["ObjectId"]))
			{
				string objectUid = qs["ObjectId"];
				ObjectTypes objectType = CommonHelper.GetObjectTypeByClassName(qs["ClassName"]);
				if (objectType != ObjectTypes.UNDEFINED)
				{
					Mediachase.IBN.Business.Common.AddFavoritesByUid(new Guid(objectUid), objectType);
				}
			}
		}
		#endregion
	}
}
