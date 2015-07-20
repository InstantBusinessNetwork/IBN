using System;
using System.Collections.Generic;
using System.Text;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Database;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Converter
{
	internal class MetaViewPreferenceInfo
	{
		private List<ColumnInfo> _columns = new List<ColumnInfo>();

		public MetaViewPreferenceInfo()
		{
		}

		public void Load(mcweb_MetaViewPreferenceRow preferenceRow, List<string> availableFieldNames)
		{
			_columns.Clear();

			if (preferenceRow != null)
			{
				Dictionary<string, ColumnInfo> columnsByName = new Dictionary<string, ColumnInfo>();

				McMetaViewPreference preference = UserMetaViewPreference.GetPreferenceFromString(preferenceRow.XSMetaViewPreference);
				AttributeCollection attributes = preference.Attributes;

				for (int i = 0; i < attributes.Count; i++)
				{
					string key = attributes.Keys[i];

					string[] parts = key.Split(':');
					if (parts.Length == 2)
					{
						string fieldName = parts[0];
						if (availableFieldNames.Contains(fieldName))
						{
							ColumnInfo column;
							if (columnsByName.ContainsKey(fieldName))
							{
								column = columnsByName[fieldName];
							}
							else
							{
								column = new ColumnInfo(fieldName);
								columnsByName.Add(fieldName, column);
							}

							string attributeName = parts[1];
							object value = attributes[key];

							switch (attributeName)
							{
								case "Index":
									column.Index = (int)value;
									break;
								case "Width":
									column.Width = (int)value;
									break;
							}
						}
					}
				}
				_columns.AddRange(columnsByName.Values);
				// Sort columns by index.
				_columns.Sort();
			}
			else
			{
				foreach (string name in availableFieldNames)
				{
					_columns.Add(new ColumnInfo(name));
				}
			}
		}

		public ColumnInfo[] GetColumns()
		{
			return _columns.ToArray();
		}
	}
}
