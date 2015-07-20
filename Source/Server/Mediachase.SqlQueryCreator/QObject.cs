using System;

namespace Mediachase.SQLQueryCreator
{
	/// <summary>
	/// Summary description for QObject.
	/// </summary>
	public abstract class QObject
	{
		private FieldCollection _Fields = new FieldCollection();
		private DictionaryCollection _Dic = new DictionaryCollection();

		public string OwnerTable { get; protected set; }

		public QObject()
		{
			LoadScheme();
		}

		protected virtual void LoadScheme()
		{
		}

		public virtual FilterCondition[] GetWhereExtensions()
		{
			return null;
		}

		public virtual QField[] GetFieldExtensions(QField Field)
		{
			return null;
		}

		public virtual QDictionary GetDictionary(QField FieldValue)
		{
			return _Dic[FieldValue];
		}

		public virtual QObject[] GetExtensions()
		{
			return null;
		}

		public QField KeyField
		{
			get
			{
				foreach(QField	fieldItem in this.Fields)
				{
					if(fieldItem.IsKey)
						return fieldItem;
				}
				return null;
			}
		}

		public FieldCollection Fields
		{
			get
			{
				return _Fields;
			}
		}

		public DictionaryCollection Dictionary
		{
			get
			{
				return _Dic;
			}
		}
	}
}
