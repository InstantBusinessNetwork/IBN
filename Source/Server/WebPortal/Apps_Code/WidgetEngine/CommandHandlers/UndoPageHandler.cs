using System;
using System.Collections.Generic;
using System.Web;

using Mediachase.Ibn.Business.Customization;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.WidgetEngine.CommandHandlers
{
	public class UndoPageHandler : ICommand
	{
		#region ICommand Members
		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				CustomPageEntity page = (CustomPageEntity)BusinessManager.Load(CustomPageEntity.ClassName, PrimaryKeyId.Parse(cp.CommandArguments["primaryKeyId"]));
				if (page != null)
				{
					HttpRequest request = HttpContext.Current.Request;

					// ProfileId
					int? profileId = null;
					if (String.Compare(request["ClassName"], CustomizationProfileEntity.ClassName, true) == 0
						&& !String.IsNullOrEmpty(request["ObjectId"]))
					{
						profileId = int.Parse(request["ObjectId"]);
					}

					// UserId
					int? userId = null;
					if (String.Compare(request["ClassName"], "Principal", true) == 0
						&& !String.IsNullOrEmpty(request["ObjectId"]))
					{
						userId = int.Parse(request["ObjectId"]);
					}

					CustomPageManager.ResetCustomPage(page.Uid, profileId, userId);
				}

			}
		}
		#endregion
	}
}
