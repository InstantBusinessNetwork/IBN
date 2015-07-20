using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Lists.Mapping
{
	delegate object ConversionDelegate(object toConvert);

	static public class Type2McDataType
	{
		private static Dictionary<Type, Dictionary<McDataType, ConversionDelegate>> _conversionTable;

		static Type2McDataType()
		{
			_conversionTable = new Dictionary<Type, Dictionary<McDataType, ConversionDelegate>>();
			//Add string conversion
			Dictionary<McDataType, ConversionDelegate> mcString2DataTypeConv = new Dictionary<McDataType, ConversionDelegate>();
			mcString2DataTypeConv.Add(McDataType.Boolean, String2Boolean);
			mcString2DataTypeConv.Add(McDataType.Integer, String2Integer);
			mcString2DataTypeConv.Add(McDataType.DateTime, String2DateTime);
			mcString2DataTypeConv.Add(McDataType.Double, String2Double);
			mcString2DataTypeConv.Add(McDataType.Guid, String2Guid);
			mcString2DataTypeConv.Add(McDataType.String, String2String);
			mcString2DataTypeConv.Add(McDataType.Currency, String2Decimal);
			mcString2DataTypeConv.Add(McDataType.Decimal, String2Decimal);
			mcString2DataTypeConv.Add(McDataType.Reference, String2Integer);
			mcString2DataTypeConv.Add(McDataType.BackReference, String2Integer);
			mcString2DataTypeConv.Add(McDataType.Card, String2String);
			mcString2DataTypeConv.Add(McDataType.Identifier, String2Integer);
			mcString2DataTypeConv.Add(McDataType.Enum, String2McEnum);

			_conversionTable.Add(typeof(String), mcString2DataTypeConv);

			//Add double conversation
			Dictionary<McDataType, ConversionDelegate> mcDouble2DataTypeConv = new Dictionary<McDataType, ConversionDelegate>();
			mcDouble2DataTypeConv.Add(McDataType.Double, Double2Double);
			mcDouble2DataTypeConv.Add(McDataType.Currency, Double2Currency);
			mcDouble2DataTypeConv.Add(McDataType.String, Double2String);
			mcDouble2DataTypeConv.Add(McDataType.Integer, Double2Integer);
			mcDouble2DataTypeConv.Add(McDataType.Boolean, Double2Boolean);
			mcDouble2DataTypeConv.Add(McDataType.Reference, Double2Integer);
			mcDouble2DataTypeConv.Add(McDataType.Decimal, Double2Decimal);

			_conversionTable.Add(typeof(double), mcDouble2DataTypeConv);

			//Add DateTime conversation
			Dictionary<McDataType, ConversionDelegate> mcDateTime2DataTypeConv = new Dictionary<McDataType, ConversionDelegate>();

			mcDateTime2DataTypeConv.Add(McDataType.DateTime, DateTime2DateTime);
			mcDateTime2DataTypeConv.Add(McDataType.Integer, DateTime2Integer);
			mcDateTime2DataTypeConv.Add(McDataType.Double, DateTime2Double);
			mcDateTime2DataTypeConv.Add(McDataType.String, DateTime2String);

			_conversionTable.Add(typeof(DateTime), mcDateTime2DataTypeConv);

			//Add Boolean conversation
			Dictionary<McDataType, ConversionDelegate> mcBoolean2DataTypeConv = new Dictionary<McDataType, ConversionDelegate>();

			mcBoolean2DataTypeConv.Add(McDataType.Boolean, Boolean2Boolean);
			mcBoolean2DataTypeConv.Add(McDataType.Integer, Boolean2Integer);
			mcBoolean2DataTypeConv.Add(McDataType.Double, Boolean2Double);
			mcBoolean2DataTypeConv.Add(McDataType.String, Boolean2String);
			mcBoolean2DataTypeConv.Add(McDataType.Currency, Boolean2Currency);

			_conversionTable.Add(typeof(Boolean), mcBoolean2DataTypeConv);

			//Add Decimal conversation
			Dictionary<McDataType, ConversionDelegate> mcDecimal2DataTypeConv = new Dictionary<McDataType, ConversionDelegate>();

			mcDecimal2DataTypeConv.Add(McDataType.Currency, Decimal2Currency);
			mcDecimal2DataTypeConv.Add(McDataType.Double, Decimal2Double);
			mcDecimal2DataTypeConv.Add(McDataType.Integer, Decimal2Integer);
			mcDecimal2DataTypeConv.Add(McDataType.String, Decimal2String);
			mcDecimal2DataTypeConv.Add(McDataType.Decimal, Decimal2Currency);

			_conversionTable.Add(typeof(Decimal), mcDecimal2DataTypeConv);

			//Add integer conversation
			Dictionary<McDataType, ConversionDelegate> mcInt2DataTypeConv = new Dictionary<McDataType, ConversionDelegate>();
			mcInt2DataTypeConv.Add(McDataType.Integer, Int2Integer);
			mcInt2DataTypeConv.Add(McDataType.String, Int2String);
			mcInt2DataTypeConv.Add(McDataType.Double, Int2Double);
			mcInt2DataTypeConv.Add(McDataType.Boolean, Int2Boolean);
			mcInt2DataTypeConv.Add(McDataType.Currency, Int2Currency);
			mcInt2DataTypeConv.Add(McDataType.Reference, Int2Integer);
			mcInt2DataTypeConv.Add(McDataType.Enum, Int2Integer);
			mcInt2DataTypeConv.Add(McDataType.File, Int2Integer);

			_conversionTable.Add(typeof(int), mcInt2DataTypeConv);
		}

		public static Type McDataType2ManagedType(McDataType mcDataType)
		{
			Type result = null;

			switch (mcDataType)
			{
				case McDataType.Boolean:
					result = typeof(bool);
					break;
				case McDataType.Currency:
					result = typeof(decimal);
					break;
				case McDataType.DateTime:
					result = typeof(DateTime);
					break;
				case McDataType.Decimal:
					result = typeof(decimal);
					break;
				case McDataType.Double:
					result = typeof(double);
					break;
				case McDataType.Guid:
					result = typeof(Guid);
					break;
				case McDataType.Integer:
					result = typeof(Int32);
					break;
				case McDataType.String:
					result = typeof(string);
					break;
				case McDataType.Enum:
					result = typeof(string);
					break;
			}

			return result;
		}


		/// <summary>
		/// Преобразует значение свойства мета объекта к одному из стандартных типов в соответвии 
		/// с определенными правилами
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static object ConvertMcData2ManagedData(MetaField field, object value)
		{
			if ((value == null) || (value == DBNull.Value))
				return null;

			object retVal = null;
			if (field.IsMultivalueEnum)
			{
				int[] enumValues = value as int[];
				if (enumValues != null)
				{
					retVal = String.Join(",", Array.ConvertAll<int, string>(enumValues,
											delegate(int intId)
											{
												return Mediachase.IBN.Business.Common.GetWebResourceString(MetaEnum.GetFriendlyName(field.GetMetaType(), intId), CultureInfo.CurrentUICulture);
											}));
				}
			}
			else if (field.IsEnum)
				retVal = Mediachase.IBN.Business.Common.GetWebResourceString(MetaEnum.GetFriendlyName(field.GetMetaType(), (int)value), CultureInfo.CurrentUICulture);
			else if (field.GetMetaType().McDataType == McDataType.File)
			{
				FileInfo fileInfo = value as FileInfo;
				if (fileInfo != null)
					retVal = fileInfo.Handle;
			}
			else if (field.IsReference)
			{
				PrimaryKeyId? primaryKey = value as PrimaryKeyId?;
				if (primaryKey != null)
					retVal = primaryKey.ToString();
			}
			else
				retVal = ConvertType2McDataType(field.GetMetaType().McDataType, value);

			return retVal;
		}
		/// <summary>
		/// Converts the type.
		/// </summary>
		/// <returns></returns>
		public static object ConvertType2McDataType(McDataType mcType, object obj)
		{
			Dictionary<McDataType, ConversionDelegate> mcDataTypeConv;

			if ((obj == null) || (obj == DBNull.Value))
				return null;

			if (_conversionTable.TryGetValue(obj.GetType(), out mcDataTypeConv))
			{
				ConversionDelegate convDelegate;
				if (mcDataTypeConv.TryGetValue(mcType, out convDelegate))
					return convDelegate(obj);
			}

			String errMsg = String.Format("conversion from [{0}] to [{1}] not found", obj.GetType().ToString(), mcType.ToString());
			throw new ArgumentException(errMsg);
		}

		#region Int Conversion
		private static object Int2Integer(object obj)
		{
			return obj;
		}

		private static object Int2Double(object obj)
		{
			return Convert.ToDouble(obj);
		}
		private static object Int2String(object obj)
		{
			return obj.ToString();
		}
		private static object Int2Boolean(object obj)
		{
			return (((uint)obj) > 0);
		}
		private static object Int2Currency(object obj)
		{
			return Int2Double(obj);
		}
		#endregion

		#region Decimal Conversion
		private static object Decimal2Currency(object obj)
		{
			return obj;
		}
		private static object Decimal2Double(object obj)
		{
			return Convert.ToDouble(obj);
		}
		private static object Decimal2Integer(object obj)
		{
			return Convert.ToInt32(obj);
		}
		private static object Decimal2String(object obj)
		{
			return obj.ToString();
		}

		#endregion

		#region Boolean Conversion

		private static object Boolean2Boolean(object obj)
		{
			return obj;
		}

		private static object Boolean2Integer(object obj)
		{
			return Convert.ToInt32(obj);
		}

		private static object Boolean2Double(object obj)
		{
			return Convert.ToDouble(obj);
		}

		private static object Boolean2Currency(object obj)
		{
			return Boolean2Double(obj);
		}

		private static object Boolean2String(object obj)
		{
			return obj.ToString();
		}

		#endregion

		#region DateTime Conversion

		private static object DateTime2DateTime(object obj)
		{
			return obj;
		}
		private static object DateTime2Integer(object obj)
		{
			//Always throw invalid cast
			return Convert.ToInt32(obj);
		}
		private static object DateTime2Double(object obj)
		{
			//Always throw invalid cast
			return Convert.ToInt32(obj);
		}
		private static object DateTime2String(object obj)
		{
			return obj.ToString();
		}
		#endregion

		#region Double Conversion
		private static object Double2Boolean(Object obj)
		{
			return ((uint)Double2Integer(obj) > 0);
		}

		private static object Double2Integer(Object obj)
		{
			return Convert.ToInt32(obj);
		}

		private static object Double2String(Object obj)
		{
			return obj.ToString();
		}
		private static object Double2Currency(Object obj)
		{
			return Double2Double(obj);
		}
		private static object Double2Decimal(Object obj)
		{
			return Convert.ToDecimal(obj);
		}
		private static object Double2Double(Object obj)
		{
			return obj;
		}
		#endregion

		#region String Conversion

		private static object String2Boolean(Object obj)
		{
			return Convert.ToBoolean(obj);
		}

		private static object String2Integer(Object obj)
		{
			return Convert.ToInt32(obj);
		}

		private static object String2DateTime(Object obj)
		{
			return Convert.ToDateTime(obj);
		}

		private static object String2Guid(Object obj)
		{
			return new Guid((String)obj);
		}
		private static object String2Decimal(Object obj)
		{
			try
			{
				return Convert.ToDecimal(obj);
			}
			catch
			{
				return Convert.ToDecimal(obj, CultureInfo.InvariantCulture);
			}
		}
		private static object String2Double(Object obj)
		{
			try
			{
				return Convert.ToDouble(obj);
			}
			catch (FormatException)
			{
				return Convert.ToDouble(obj, CultureInfo.InvariantCulture);
			}
		}

		private static object String2String(Object obj)
		{
			return obj;
		}

		private static object String2McEnum(Object obj)
		{
			object retVal = null;

			if (obj != null)
			{
				string[] arrEnumIds = ((string)obj).Split(new char[] { ',' });
				retVal = arrEnumIds;
				if (arrEnumIds.Length == 1)
					retVal = arrEnumIds[0];
			}

			return retVal;
		}

		#endregion

		private static object DBNULL2NULL(Object obj)
		{
			return obj;
		}
	}
}
