using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.XmlTools;

namespace Mediachase.Ibn.Web.UI.EventService
{
	public class EventCode
	{
		private const string _eventType = "EventType";

		public static string GetEventControlVirtualPath(string virtualStructureDir, MetaObject eventObject, string className)
		{
			string structureDir = HostingEnvironment.MapPath(virtualStructureDir);
			string controlPath = GetEventControlPath(eventObject, className);
			if (controlPath != null)
			{
				controlPath = controlPath.Replace(structureDir, virtualStructureDir);
				controlPath = controlPath.Replace(Path.DirectorySeparatorChar, '/');
			}
			return controlPath;
		}

		public static string GetEventControlPath(MetaObject eventObject, string className)
		{
			string result = null;

			string eventTypeName = (string)eventObject.Properties[_eventType].Value;
			FileDescriptor[] files = FileResolver.GetFiles("EventControls", "*.ascx", new Selector(eventTypeName, className));
			if (files.Length > 0)
			{
				result = files[files.Length - 1].FilePath;
			}

			return result;
		}
	}
}
