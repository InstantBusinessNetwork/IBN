using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.IBN.Business.WidgetEngine
{
	public static class WorkspaceTemplateFactory
	{
		private static ArrayList _templates;

		private static void Initialize()
		{
			if (_templates == null || WorkspaceTemplateLoader.HasChanged)
			{
				lock (typeof(WorkspaceTemplateFactory))
				{
					if (_templates == null || WorkspaceTemplateLoader.HasChanged)
					{
						_templates = ArrayList.Synchronized(new ArrayList());

						_templates.AddRange(WorkspaceTemplateLoader.Load());
					}
				}
			}
		}

		/// <summary>
		/// Creates the control virtual path.
		/// </summary>
		/// <param name="controlPath">The control path.</param>
		/// <returns></returns>
		private static string CreateControlVirtualPath(string controlPath)
		{
			return controlPath;
		}

		/// <summary>
		/// Gets the control info.
		/// </summary>
		/// <param name="controlUid">The control uid.</param>
		/// <returns></returns>
		public static WorkspaceTemplateInfo GetTemplateInfo(string templateUid)
		{
			if (templateUid == null)
				throw new ArgumentNullException("controlUid");

			Initialize();

			foreach (WorkspaceTemplateInfo info in _templates)
			{
				if (info.Uid.ToUpperInvariant() == templateUid.ToUpperInvariant())
					return info;
			}

			return null;
		}

		/// <summary>
		/// Gets the Template infos.
		/// </summary>
		/// <returns></returns>
		public static WorkspaceTemplateInfo[] GetTemplateInfos()
		{
			Initialize();
			return (WorkspaceTemplateInfo[])_templates.ToArray(typeof(WorkspaceTemplateInfo));
		}

	}
}
