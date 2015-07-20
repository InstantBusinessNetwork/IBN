using System;
using System.Collections.Generic;
using System.Web;

using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Business.Customization;

namespace Mediachase.Ibn.Web.UI.WidgetEngine.CommandHandlers
{
	public class UndoPageEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				int createdLevel = (int)CustomPageLevel.Disk;
				if (cp.CommandArguments.ContainsKey(CustomPageNormalizationPlugin.FieldNameCreatedLevel))
					createdLevel = int.Parse(cp.CommandArguments[CustomPageNormalizationPlugin.FieldNameCreatedLevel]);

				int modifiedLevel = (int)CustomPageLevel.Disk;
				if (cp.CommandArguments.ContainsKey(CustomPageNormalizationPlugin.FieldNameModifiedLevel))
					modifiedLevel = int.Parse(cp.CommandArguments[CustomPageNormalizationPlugin.FieldNameModifiedLevel]);

				int currentLevel = (int)CustomPageLevel.Global;
				HttpRequest request = HttpContext.Current.Request;

				// Profile level
				if (String.Compare(request["ClassName"], CustomizationProfileEntity.ClassName, true) == 0 && !String.IsNullOrEmpty(request["ObjectId"]))
					currentLevel = (int)CustomPageLevel.Profile;
				// User level
				else if (String.Compare(request["ClassName"], "Principal", true) == 0  && !String.IsNullOrEmpty(request["ObjectId"]))
					currentLevel = (int)CustomPageLevel.User;

				if (currentLevel == modifiedLevel && currentLevel > createdLevel)
					retval = true;
			}
			return retval;
		}
		#endregion
	}
}
