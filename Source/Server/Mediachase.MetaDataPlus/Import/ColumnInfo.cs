using System;

using Mediachase.MetaDataPlus.Configurator;


namespace Mediachase.MetaDataPlus.Import
{
	/// <summary>
	/// Summary description for ColumnInfo.
	/// </summary>
	public class ColumnInfo
	{
		private MetaField _field;
		private bool _isSystemDictionary;
		private FillTypes _fillType;

		public FillTypes SupportedFillType { get { return _fillType; } }

		public MetaField Field { get { return _field; } }

		public string FieldName { get { return _field.Name; } }

		public string FieldFriendlyName { get { return _field.FriendlyName; } }

		public bool IsSystemDictionary { get { return _isSystemDictionary; } }

		public ColumnInfo()
		{
			_fillType = FillTypes.None;
		}
		public ColumnInfo(MetaField field, FillTypes fillType)
		{
			_field = field;
			_fillType = fillType;
		}
		public ColumnInfo(MetaField field, FillTypes fillType, bool isSystemDictionary)
		{
			if (field == null)
				throw new ArgumentNullException("field");

			_field = field;
			_fillType = fillType;
			_isSystemDictionary = isSystemDictionary & field.IsSystem;
		}
	}
}
