using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.XmlTools;
using Mediachase.Net.WebDavServer;
using System.IO;
using System.Xml;

namespace Mediachase.IBN.Business.WidgetEngine
{
	public sealed class DynamicCategoryInfoLoader
	{
		public const string ControlsDir = "Controls";
		public const string ConfigDir = "Categories";

		public static DynamicCategoryInfo[] Load()
		{
			List<DynamicCategoryInfo> list = new List<DynamicCategoryInfo>();

			//string structureVirtualPath
			//string structurePath = HostingEnvironment.MapPath(structureVirtualPath);
			FileDescriptor[] files = FileResolver.GetFiles(ControlsDir + Path.DirectorySeparatorChar + ConfigDir, "*.xml");
			foreach (FileDescriptor file in files)
			{
				string controlDir = Path.GetDirectoryName(file.FilePath);

				if (!string.IsNullOrEmpty(controlDir))
				{
					string configsDir = Path.DirectorySeparatorChar + ConfigDir;
					string tempControlDirString = controlDir.EndsWith(Convert.ToString(Path.DirectorySeparatorChar)) ? controlDir.Substring(0, controlDir.Length - 1) : controlDir;
					if (controlDir.EndsWith(configsDir, StringComparison.OrdinalIgnoreCase))
						controlDir = controlDir.Substring(0, controlDir.LastIndexOf(configsDir));
				}

				XmlDocument doc = new XmlDocument();
				doc.Load(file.FilePath);

				foreach (XmlNode node in doc.DocumentElement.SelectNodes("DynamicCategoryInfo"))
				{
					DynamicCategoryInfo dci = McXmlSerializer.GetObject<DynamicCategoryInfo>(node.OuterXml); //McXmlSerializer.GetObjectFromFile<DynamicCategoryInfo>(file.FilePath);

					if (string.IsNullOrEmpty(dci.Uid) || list.Contains(dci))
						throw new ArgumentNullException("DynamicCategoryInfo must have unique <Uid>");

					list.Add(dci);
				}

				list.Sort(DynamicCategoryInfoComparasion);
			}

			return list.ToArray();

		}

		#region DynamicCategoryInfoComparasion
		/// <summary>
		/// Dynamics the category info comparasion.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns></returns>
		private static int DynamicCategoryInfoComparasion(DynamicCategoryInfo x, DynamicCategoryInfo y)
		{
			if (x.Weight < y.Weight)
				return -1;
			else if (x.Weight == y.Weight)
				return 0;
			else return 1;
		} 
		#endregion
	}
}
