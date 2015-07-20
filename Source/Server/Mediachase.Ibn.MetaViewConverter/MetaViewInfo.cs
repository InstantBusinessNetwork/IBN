using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Database;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Converter
{
	internal class MetaViewInfo
	{
		private string _metaClassName;
		string[] _availableFieldNames;
		private List<MetaViewPreferenceInfo> _preferences = new List<MetaViewPreferenceInfo>();

		internal MetaViewInfo()
		{
		}

		internal void Load(MetaView metaView)
		{
			_metaClassName = metaView.MetaClassName;
			_availableFieldNames = metaView.AvailableFieldNames;

			List<string> fieldNames = new List<string>(_availableFieldNames);

			mcweb_MetaViewPreferenceRow[] preferenceRows = mcweb_MetaViewPreferenceRow.List(FilterElement.EqualElement("MetaViewName", metaView.Name), FilterElement.IsNullElement("PrincipalId"));
			if(preferenceRows.Length == 0)
			{
				preferenceRows = new mcweb_MetaViewPreferenceRow[] { null };
			}

			foreach (mcweb_MetaViewPreferenceRow preferenceRow in preferenceRows)
			{
				MetaViewPreferenceInfo preferenceInfo = new MetaViewPreferenceInfo();
				preferenceInfo.Load(preferenceRow, fieldNames);
				_preferences.Add(preferenceInfo);
			}
		}

		internal void SaveListViewProfiles()
		{
			foreach (MetaViewPreferenceInfo preferenceInfo in _preferences)
			{
				ListViewProfile profile = new ListViewProfile();

				profile.Id = Guid.NewGuid().ToString("D");
				profile.Name = "{IbnFramework.ListInfo:lvpGeneralView}";
				profile.IsPublic = true;
				profile.FieldSet.AddRange(_availableFieldNames);

				// Build columns list
				foreach (ColumnInfo columnInfo in preferenceInfo.GetColumns())
				{
					profile.ColumnsUI.Add(new ColumnProperties(columnInfo.Name, columnInfo.Width.ToString(CultureInfo.InvariantCulture), string.Empty));
				}

				string placeName = (_metaClassName.EndsWith("_History", StringComparison.Ordinal)) ? "ItemHistoryList" : "EntityList";

				ListViewProfile.SaveSystemProfile(_metaClassName, placeName, -1, profile);
			}
		}
	}
}
