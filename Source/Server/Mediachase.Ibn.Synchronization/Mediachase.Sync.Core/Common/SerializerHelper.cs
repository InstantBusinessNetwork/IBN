using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Mediachase.Sync.Core.Common
{
	public class SerializerHelper
	{
		public static T XmlDeserialize<T>(string value)
		{
			T retVal = default(T);
			System.Xml.Serialization.XmlSerializer xmlsz = 
							new System.Xml.Serialization.XmlSerializer(typeof(T));
			using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(value)))
			{
				retVal = (T)xmlsz.Deserialize(ms);
			}

			return retVal;
		}

		public static string XmlSerialize(object obj)
		{
			string retVal = String.Empty;

			if (obj == null)
				return retVal;

			System.Xml.Serialization.XmlSerializer xmlsz = new System.Xml.Serialization.XmlSerializer(obj.GetType());
			using (MemoryStream ms = new MemoryStream())
			{
				xmlsz.Serialize(ms, obj);
				retVal =  System.Text.Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
			}
			return retVal;
		}

		public static byte[] BinarySerialize(object obj)
		{
			byte[] retVal = null;
			BinaryFormatter formatter = new BinaryFormatter();
			using (MemoryStream memStream = new MemoryStream())
			{
				formatter.Serialize(memStream, obj);
				retVal = memStream.GetBuffer();
			}

			return retVal;
		}
		public static T BinaryDeserialize<T>(byte[] rawData) where T : class
		{
			T retVal = default(T);
			BinaryFormatter formatter = new BinaryFormatter();
			using (MemoryStream memStream = new MemoryStream(rawData))
			{
				retVal = formatter.Deserialize(memStream) as T;
			}

			return retVal;
		}
	}
}
