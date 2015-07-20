using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;
using System.Xml;

namespace Mediachase.Ibn
{
	public class GlobalResourceManager
	{
		private static bool _initialized;
		private static object _lockObject = new object();
		private static Dictionary<string, string> _strings = new Dictionary<string, string>();
		private static Dictionary<string, bool> _options = new Dictionary<string, bool>();

		public static Dictionary<string, string> Strings { get { return _strings; } }
		public static Dictionary<string, bool> Options { get { return _options; } }

		public static void Initialize(string resourceFilePath)
		{
			if (resourceFilePath == null)
				throw new ArgumentNullException("resourceFilePath");
			if (resourceFilePath.Length == 0)
				throw new ArgumentException("Path must not be empty.", "resourceFilePath");

			if (!_initialized)
			{
				lock (_lockObject)
				{
					if (!_initialized)
					{
						Load(resourceFilePath);
						_initialized = true;
					}
				}
			}
		}

		private static void Load(string resourceFilePath)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(resourceFilePath);

			_strings.Clear();
			_options.Clear();

			foreach (KeyValuePair<string, string> pair in GetItems(doc, "/GlobalResources/String"))
				_strings.Add(pair.Key, pair.Value);

			foreach (KeyValuePair<string, string> pair in GetItems(doc, "/GlobalResources/Boolean"))
				_options.Add(pair.Key, bool.Parse(pair.Value));
		}

		private static Dictionary<string, string> GetItems(XmlDocument document, string query)
		{
			Dictionary<string, string> items = new Dictionary<string, string>();

			foreach (XmlNode node in document.SelectNodes(query))
				items.Add(node.Attributes["name"].Value, node.InnerText);

			return items;
		}
	}
}
