using System;
using System.Data;

using Mediachase.MetaDataPlus.Configurator;


namespace Mediachase.MetaDataPlus.Import
{
	/// <summary>
	/// Summary description for RuleItem.
	/// </summary>
	[Serializable]
	public class RuleItem
	{
		string _srcColumnName;
		Type _srcColumnType;
		string _destColumnName;
		MetaDataType _destColumnType;
		bool _destColumnSystem;
		FillTypes _fillType;
		string _customValue;

		public string SrcColumnName
		{
			get
			{
				return _srcColumnName;
			}
			set
			{
				_srcColumnName = value;
			}
		}

		public Type SrcColumnType
		{
			get
			{
				return _srcColumnType;
			}
			set
			{
				_srcColumnType = value;
			}
		}

		public string DestColumnName
		{
			get
			{
				return _destColumnName;
			}
			set
			{
				_destColumnName = value;
			}
		}

		public MetaDataType DestColumnType
		{
			get
			{
				return _destColumnType;
			}
			set
			{
				_destColumnType = value;
			}
		}

		public bool DestColumnSystem
		{
			get
			{
				return _destColumnSystem;
			}
			set
			{
				_destColumnSystem = value;
			}
		}

		public FillTypes FillType
		{
			get
			{
				return _fillType;
			}
			set
			{
				_fillType = value;
			}
		}

		public string CustomValue
		{
			get
			{
				return _customValue;
			}
			set
			{
				_customValue = value;
			}
		}

		public RuleItem(string srcColumnName, Type srcColumnType)
			: this(srcColumnName, srcColumnType, "", MetaDataType.Variant, false, FillTypes.None, "")
		{
		}

		public RuleItem(string destColumnName, MetaDataType destColumnType, bool destColumnSystem, FillTypes fillType, string customValue)
			: this("", typeof(string), destColumnName, destColumnType, destColumnSystem, fillType, customValue)
		{
		}

		public RuleItem(string srcColumnName, Type srcColumnType, string destColumnName, MetaDataType destColumnType, bool destColumnSystem, FillTypes fillType)
			: this(srcColumnName, srcColumnType, destColumnName, destColumnType, destColumnSystem, fillType, "")
		{
		}

		public RuleItem(MetaField destColumn, FillTypes fillType)
			: this("", typeof(string), destColumn, fillType, "")
		{
		}

		public RuleItem(string srcColumnName, Type srcColumnType, MetaField destColumn, FillTypes fillType)
			: this(srcColumnName, srcColumnType, destColumn, fillType, "")
		{
		}

		public RuleItem(string srcColumnName, Type srcColumnType, MetaField destColumn, FillTypes fillType, string customValue)
		{
			if (destColumn == null)
				throw new ArgumentNullException("destColumn");

			_srcColumnName = srcColumnName;
			_srcColumnType = srcColumnType;
			_destColumnName = destColumn.Name;
			_destColumnType = destColumn.DataType;
			_destColumnSystem = destColumn.IsSystem;
			_fillType = fillType;
			_customValue = customValue;
		}

		public RuleItem(string srcColumnName, Type srcColumnType, string destColumnName, MetaDataType destColumnType, bool destColumnSystem, FillTypes fillType, string customValue)
		{
			_srcColumnName = srcColumnName;
			_srcColumnType = srcColumnType;
			_destColumnName = destColumnName;
			_destColumnType = destColumnType;
			_destColumnSystem = destColumnSystem;
			_fillType = fillType;
			_customValue = customValue;
		}

	}
}
