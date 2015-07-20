using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Globalization;

namespace Mediachase.Ibn.Configuration
{
	internal sealed class UpdateInfo : IUpdateInfo
	{
		private const string ConstUpdateFileName = "update.xml";

		public int Version { get; internal set; }
		public DateTime ReleaseDate { get; internal set; }
		public bool UpdatesCommonComponents { get; internal set; }
		public bool RequiresCommonComponentsUpdate { get; internal set; }
		public string Changes { get; internal set; }
		public string Warnings { get; internal set; }

		private UpdateInfo()
		{
		}

		#region internal static int[] List(string path)
		internal static int[] List(string path)
		{
			List<int> list = new List<int>();

			DirectoryInfo directory = new DirectoryInfo(path);
			if (directory.Exists)
			{
				foreach (DirectoryInfo updateDirectory in directory.GetDirectories())
				{
					FileInfo updateFile = new FileInfo(Path.Combine(updateDirectory.FullName, ConstUpdateFileName));
					if (updateFile.Exists)
					{
						int version;
						if (int.TryParse(updateDirectory.Name, NumberStyles.Integer, CultureInfo.InvariantCulture, out version))
							list.Add(version);
					}
				}
			}

			return list.ToArray();
		}
		#endregion

		#region internal static UpdateInfo[] LoadExtendedInfo(string updateDirectory)
		internal static UpdateInfo[] LoadExtendedInfo(string updateDirectory)
		{
			List<UpdateInfo> list = new List<UpdateInfo>();

			string filePath = Path.Combine(updateDirectory, ConstUpdateFileName);
			XmlDocument doc = new XmlDocument();
			doc.Load(filePath);

			foreach (XmlNode update in doc.SelectNodes("/Updates/Update"))
			{
				UpdateInfo item = new UpdateInfo();
				item.Load(update);
				list.Add(item);
			}

			list.Sort(CompareByVersion);

			return list.ToArray();
		}
		#endregion


		private void Load(XmlNode update)
		{
			Version = int.Parse(update.Attributes["version"].Value, CultureInfo.InvariantCulture);
			ReleaseDate = DateTime.Parse(update.Attributes["date"].Value, CultureInfo.InvariantCulture);
			UpdatesCommonComponents = bool.Parse(update.Attributes["updatesCommonComponents"].Value);
			RequiresCommonComponentsUpdate = bool.Parse(update.Attributes["requiresCommonComponentsUpdate"].Value);
			Changes = XmlHelper.GetChildNodeText(update, "Changes");
			Warnings = XmlHelper.GetChildNodeText(update, "Warnings");
		}

		private static int CompareByVersion(UpdateInfo x, UpdateInfo y)
		{
			int result;

			if (x == null)
			{
				if (y == null)
					result = 0;
				else
					result = -1;
			}
			else
			{
				if (y == null)
					result = 1;
				else
				{
					result = x.Version.CompareTo(y.Version);
				}
			}

			return result * -1;
		}
	}
}
