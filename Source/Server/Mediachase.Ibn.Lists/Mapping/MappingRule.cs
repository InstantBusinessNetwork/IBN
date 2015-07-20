using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Lists.Mapping
{
	public enum MappingRuleType
	{
		Unknown = 0,
		CopyValue = 1,
		DefaultValue = 2,
		Reference = 3
	}

    /// <summary>
    /// 
    /// </summary>
    /// 
    [Serializable]
    [XmlRoot(ElementName="map")]
    public class MappingRule
    {
		private MappingRuleType _type = MappingRuleType.Unknown;

        private string _mapFieldName;
        private string _mapColumnName;
        private string _mapValue;
        private string _relationTo;

		/// <summary>
		/// Initializes a new instance of the <see cref="MappingRule"/> class.
		/// </summary>
        public MappingRule()
        {

        }

		/// <summary>
		/// Creates the copy value.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns></returns>
		public static MappingRule CreateCopyValue(string columnName, string fieldName)
		{
			return new MappingRule(columnName, fieldName);
		}

		/// <summary>
		/// Creates the default value.
		/// </summary>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static MappingRule CreateDefaultValue(string fieldName, string value)
		{
			return new MappingRule(MappingRuleType.DefaultValue, string.Empty, fieldName, value, string.Empty);
		}

		/// <summary>
		/// Creates the reference.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="relationTo">The relation to.</param>
		/// <returns></returns>
		public static MappingRule CreateReference(string columnName, string fieldName, string relationTo)
		{
			return new MappingRule(MappingRuleType.Reference, columnName, fieldName, string.Empty, relationTo);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MappingRule"/> class.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="fieldName">Name of the field.</param>
        public MappingRule(string columnName, string fieldName)
        {
			_type = MappingRuleType.CopyValue;
            _mapColumnName = columnName;
            _mapFieldName = fieldName;

        }

		/// <summary>
		/// Initializes a new instance of the <see cref="MappingRule"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="value">The value.</param>
		/// <param name="relationTo">The relation to.</param>
        public MappingRule(MappingRuleType type, string columnName, string fieldName, 
			string value, string relationTo)
        {
			_type = type;
            _mapColumnName = columnName;
            _mapFieldName = fieldName;
            _mapValue = value;
        }

		/// <summary>
		/// Gets or sets the name of the field.
		/// </summary>
		/// <value>The name of the field.</value>
        [XmlAttribute(AttributeName="field")]
        public string FieldName
        {
            get { return _mapFieldName; }
            set { _mapFieldName = value; }
        }

		/// <summary>
		/// Gets or sets the name of the column.
		/// </summary>
		/// <value>The name of the column.</value>
        [XmlAttribute(AttributeName="column")]
        public string ColumnName
        {
            get { return _mapColumnName; }
            set { _mapColumnName = value; }
        }

		/// <summary>
		/// Gets or sets the default value.
		/// </summary>
		/// <value>The default value.</value>
        [XmlAttribute(AttributeName="value")]
        public string DefaultValue
        {
            get { return _mapValue; }
            set { _mapValue = value;}
        }

		/// <summary>
		/// Gets or sets the relation to.
		/// </summary>
		/// <value>The relation to.</value>
        [XmlAttribute(AttributeName="relationTo")]
        public string RelationTo
        {
            get { return _relationTo; }
            set {_relationTo = value; }
        }

		/// <summary>
		/// Gets the type of the rule.
		/// </summary>
		/// <value>The type of the rule.</value>
		[XmlAttribute(AttributeName="type")]
		public MappingRuleType RuleType
		{
			get 
			{
				return _type; 
			}
			set
			{
				_type = value;
			}
		}
	
    }
}
