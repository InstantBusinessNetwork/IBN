using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Sync.Core;

namespace Mediachase.ClientOutlook.Configuration
{
    delegate object ConversionDelegate(object toConvert);

    static public class Type2ConfigPropType
    {
        static private Dictionary<Type, Dictionary<Type, ConversionDelegate>> _conversionTable;

        static Type2ConfigPropType()
        {
            _conversionTable = new Dictionary<Type, Dictionary<Type, ConversionDelegate>>();
            //Add string conversion
            Dictionary<Type, ConversionDelegate> mcString2DataTypeConv = 
                                                new Dictionary<Type, ConversionDelegate>();
            mcString2DataTypeConv.Add(typeof(string), String2String);
            mcString2DataTypeConv.Add(typeof(bool), String2Boolean);
            mcString2DataTypeConv.Add(typeof(int), String2Integer);
            mcString2DataTypeConv.Add(typeof(DateTime), String2DateTime);
			mcString2DataTypeConv.Add(typeof(long), String2Long);

            _conversionTable.Add(typeof(String), mcString2DataTypeConv);
                       

            //Add Boolean conversation
            Dictionary<Type, ConversionDelegate> mcBoolean2DataTypeConv =
                                           new Dictionary<Type, ConversionDelegate>();

            mcBoolean2DataTypeConv.Add(typeof(bool), Boolean2Boolean);
            mcBoolean2DataTypeConv.Add(typeof(int), Boolean2Integer);
            mcBoolean2DataTypeConv.Add(typeof(string), Boolean2String);

            _conversionTable.Add(typeof(Boolean), mcBoolean2DataTypeConv);

         
            //Add integer conversation
            Dictionary<Type, ConversionDelegate> mcInt2DataTypeConv =
                                        new Dictionary<Type, ConversionDelegate>();
            mcInt2DataTypeConv.Add(typeof(int), Int2Integer);
            mcInt2DataTypeConv.Add(typeof(string), Int2String);
            mcInt2DataTypeConv.Add(typeof(bool), Int2Boolean);

            _conversionTable.Add(typeof(int), mcInt2DataTypeConv);

            //Add DateTime conversation
            Dictionary<Type, ConversionDelegate> mcDateTime2DataTypeConv =
                                        new Dictionary<Type, ConversionDelegate>();
			mcDateTime2DataTypeConv.Add(typeof(DateTime), DateTime2DateTime);
            mcDateTime2DataTypeConv.Add(typeof(string), DateTime2String);

            _conversionTable.Add(typeof(DateTime), mcDateTime2DataTypeConv);

			//Add Long conversion
			Dictionary<Type, ConversionDelegate> mcLong2DataTypeConv =
									  new Dictionary<Type, ConversionDelegate>();
			 mcLong2DataTypeConv.Add(typeof(long), Long2Long);
			 mcLong2DataTypeConv.Add(typeof(string), Long2String);

			 _conversionTable.Add(typeof(long), mcLong2DataTypeConv);


        }

    
        /// <summary>
        /// Converts the type.
        /// </summary>
        /// <returns></returns>
        public static object ConvertType2Type(Type mcType, object obj)
        {
            object retVal = null;
             Dictionary<Type, ConversionDelegate> TypeConv;

             if ((obj == null) || (obj == DBNull.Value))
                 return retVal;

             if (_conversionTable.TryGetValue(obj.GetType(), out TypeConv))
             {
                 ConversionDelegate convDelegate;
                 if (TypeConv.TryGetValue(mcType, out convDelegate))
                 {
                     retVal = convDelegate(obj);
                 }
             }
             else
             {
                 String errMsg = String.Format("conversion from [{0}] to [{1}] not found",
                                                                      obj.GetType().ToString(), mcType.ToString());
                 DebugAssistant.Log(errMsg);
             }
            return retVal;
       }

       #region Int Conversion
        private static object Int2Integer(object obj)
        {
            return obj;
        }
        private static object Int2String(object obj)
        {
            return obj.ToString();
        }
        private static object Int2Boolean(object obj)
        {
            return (((uint)obj) > 0);
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

        private static object Boolean2String(object obj)
        {
            return obj.ToString();
        }

        #endregion
       #region String Conversion

   
        private static object String2DateTime(Object obj)
        {
            return Convert.ToDateTime(obj);
        }
        private static object String2Boolean(Object obj)
        {
            return Convert.ToBoolean(obj);
        }
        private static object String2Integer(Object obj)
        {
            return Convert.ToInt32(obj);
        }
        private static object String2String(Object obj)
        {
            return obj;
        }
		private static object String2Long(Object obj)
		{
			return Convert.ToInt64(obj);
		}
        #endregion
       #region DateTime Conversion
        private static object DateTime2DateTime(Object obj)
        {
            return obj;
        }
        private static object DateTime2String(Object obj)
        {
            return Convert.ToString(obj);
        }
        #endregion
	   #region Long Conversion
		private static object Long2Long(Object obj)
		{
			return obj;
		}
		private static object Long2String(Object obj)
		{
			return Convert.ToString(obj);
		}
	   #endregion

      
        private static object DBNULL2NULL(Object obj)
        {
            return obj;
        }

    }
}
