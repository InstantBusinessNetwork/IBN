using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Hosting;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.XmlTools;
using Mediachase.Ibn;
using System.Collections;


namespace Mediachase.IBN.Business.WidgetEngine
{
	public sealed class DynamicControlInfoLoader
	{
		public const string ControlsDir = "Controls";
		public const string ConfigDir = "Configs";

		private DynamicControlInfoLoader()
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
		public static DynamicControlInfo[] Load()
		{
			List<DynamicControlInfo> list = new List<DynamicControlInfo>();

			//string structureVirtualPath
			//string structurePath = HostingEnvironment.MapPath(structureVirtualPath);
			FileDescriptor[] files = FileResolver.GetFiles(ControlsDir + Path.DirectorySeparatorChar + "Configs", "*.xml");
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

				DynamicControlInfo dci = McXmlSerializer.GetObjectFromFile<DynamicControlInfo>(file.FilePath);

				if (string.IsNullOrEmpty(dci.Uid))
					dci.Uid = Path.GetFileNameWithoutExtension(file.FilePath);
				dci.AdapterPath = MakeVirtualPath(controlDir, dci.AdapterPath);
				dci.IconPath = dci.IconPath;
				dci.Path = dci.Path; //MakeVirtualPath(structurePath, structureVirtualPath, controlDir, dci.Path);
				dci.PropertyPagePath = dci.PropertyPagePath;//MakeVirtualPath(structurePath, structureVirtualPath, controlDir, dci.PropertyPagePath);
				dci.LargeThumbnail = dci.LargeThumbnail;
				dci.SmallThumbnail = dci.SmallThumbnail;

				list.Add(dci);
			}

			return list.ToArray();
		}

		private static string MakeVirtualPath(string physicalDir, string relativePath)
		{
			string virtualPath = relativePath;

			if (!string.IsNullOrEmpty(virtualPath))
			{
				virtualPath = string.Concat(physicalDir, Path.DirectorySeparatorChar, relativePath);
				virtualPath = virtualPath.Replace(GlobalContext.Current.ModulesDirectoryPath, GlobalContext.Current.ModulesVirtualPath);
				virtualPath = virtualPath.Replace(Path.DirectorySeparatorChar, '/');
			}

			return virtualPath;
		}

		#region GetAllGroups
		private static List<string> _categories = null;
		/// <summary>
		/// Gets all groups.
		/// </summary>
		/// <returns></returns>
		public static List<string> GetAllGroups()
		{
			if (_categories != null)
				return _categories;

			_categories = new List<string>();

			foreach (DynamicControlInfo dci in DynamicControlInfoLoader.Load())
			{
				if (_categories.Contains(dci.Category))
					continue;

				_categories.Add(dci.Category);
			}

			return _categories;
		} 
		#endregion

		#endregion
	}

}
