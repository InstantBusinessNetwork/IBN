using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.XmlTools;

namespace Mediachase.IBN.Business.WidgetEngine
{
	public sealed class WorkspaceTemplateLoader
	{
		public const string ControlsDir = "Templates";
		public const string ConfigDir = "Configs";

		private WorkspaceTemplateLoader()
		{
		}

		// TODO:
		public static bool HasChanged
		{
			get
			{
				//return FileResolver.HasChanged;
				return true;
			}
		}

		#region Methods

		/// <summary>
		/// Loads all controls from given directory and its subdirectories.
		/// </summary>
		/// <returns></returns>
		public static WorkspaceTemplateInfo[] Load()
		{
			List<WorkspaceTemplateInfo> list = new List<WorkspaceTemplateInfo>();

			FileDescriptor[] files = FileResolver.GetFiles(ControlsDir + Path.DirectorySeparatorChar + "Configs", "*.xml");
			foreach (FileDescriptor file in files)
			{
				string controlDir = Path.GetDirectoryName(file.FilePath);

				if (!String.IsNullOrEmpty(controlDir))
				{
					string configsDir = Path.DirectorySeparatorChar + ConfigDir;
					string tempControlDirString = controlDir.EndsWith(Convert.ToString(Path.DirectorySeparatorChar)) ? controlDir.Substring(0, controlDir.Length - 1) : controlDir;
					if (controlDir.EndsWith(configsDir, true, CultureInfo.InvariantCulture))
						controlDir = controlDir.Substring(0, controlDir.LastIndexOf(configsDir));
				}

				WorkspaceTemplateInfo wti = McXmlSerializer.GetObjectFromFile<WorkspaceTemplateInfo>(file.FilePath);

				if (string.IsNullOrEmpty(wti.Uid))
					wti.Uid = Path.GetFileNameWithoutExtension(file.FilePath);

				list.Add(wti);
			}

			return list.ToArray();
		}

		#endregion
	}
}
