using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using Mediachase.Database;
using System.Data;

namespace Mediachase.Ibn.Configuration
{
	internal class LanguageInfo : ILanguageInfo
	{
		public int LanguageId { get; internal set; }
		public string Locale { get; internal set; }
		public string FriendlyName { get; internal set; }

		internal LanguageInfo()
		{
		}

		#region internal static LanguageInfo[] List(string path)
		internal static LanguageInfo[] List(string path)
		{
			List<LanguageInfo> list = new List<LanguageInfo>();

			XmlDocument doc = new XmlDocument();
			doc.Load(path);

			foreach (XmlNode node in doc.SelectNodes("/dictionaries/dictionary[@name='LANGUAGES']/item"))
			{
				LanguageInfo item = new LanguageInfo();
				item.Load(node);
				list.Add(item);
			}

			return list.ToArray();
		}
		#endregion

		internal void Load(DBHelper dbHelper)
		{
			using (IDataReader reader = dbHelper.RunTextDataReader("SELECT [LanguageId],[Locale],[FriendlyName] FROM [LANGUAGES] WHERE [IsDefault]=1"))
			{
				if (reader.Read())
				{
					LanguageId = (int)reader[0];
					Locale = reader[1].ToString();
					FriendlyName = reader[2].ToString();
				}
			}
		}


		private void Load(XmlNode language)
		{
			LanguageId = int.Parse(language.Attributes["LanguageId"].Value, CultureInfo.InvariantCulture);
			Locale = language.Attributes["Locale"].Value;
			FriendlyName = language.Attributes["FriendlyName"].Value;
		}
	}
}
