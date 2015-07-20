using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus.Common;


namespace Mediachase.MetaDataPlus.Import
{
	[Serializable]
	public class RuleAttributeDictionary : Dictionary<string, string>
	{
		internal RuleAttributeDictionary()
		{
		}

		protected RuleAttributeDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	/// <summary>
	/// Summary description for Rule.
	/// </summary>
	[Serializable]
	public class Rule
	{
		private int _id = -1;
		private List<RuleItem> _items = new List<RuleItem>();
		private RuleAttributeDictionary _attributes = new RuleAttributeDictionary();

		internal string _innerClassName;

		public RuleItem this[int index]
		{
			get
			{
				return (RuleItem)_items[index];
			}
		}

		public RuleItem this[string destColumnName]
		{
			get
			{
				foreach (RuleItem item in _items)
				{
					if (item.DestColumnName == destColumnName)
						return item;
				}
				return null;
			}
		}

		public RuleAttributeDictionary Attribute
		{
			get
			{
				return _attributes;
			}
		}

		public int Count
		{
			get
			{
				return _items.Count;
			}
		}

		public string ClassName
		{
			get
			{
				return
					_innerClassName;
			}
		}

		public RuleItem GetBySrcColumnName(string srcColumnName)
		{
			foreach (RuleItem item in _items)
			{
				if (item.SrcColumnName == srcColumnName)
					return item;
			}
			return null;
		}

		public void Add(RuleItem item)
		{
			_items.Add(item);
		}

		public void RemoveAt(int index)
		{
			_items.RemoveAt(index);
		}

		public Rule(MetaClass metaClass)
		{
			if (metaClass == null)
				throw new ArgumentNullException("metaClass");

			_innerClassName = metaClass.Name;
		}

		public Rule(string className)
		{
			_innerClassName = className;
		}

		public Rule()
		{
		}

		public static Rule Load(int ruleId)
		{
			Rule retVal = null;

			using (IDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaRuleById"),
					   new SqlParameter("@RuleId", ruleId)))
			{
				if (reader.Read())
				{
					byte[] data = (byte[])reader["Data"];

					retVal = Deserialize(data);

					retVal._id = (int)reader["RuleId"];
				}
				reader.Close();
			}
			return retVal;
		}

		public static Rule[] GetList(int metaClassId)
		{
			List<Rule> list = new List<Rule>();

			using (IDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaRuleByMetaClassId"), new SqlParameter("@MetaClassId", metaClassId)))
			{
				while (reader.Read())
				{
					byte[] data = (byte[])reader["Data"];
					int ruleId = (int)reader["RuleId"];

					Rule rule = Deserialize(data);
					rule._id = ruleId;

					list.Add(rule);
				}
				reader.Close();
			}
			return list.ToArray();
		}

		public static Rule[] GetList(MetaClass metaClass)
		{
			return (metaClass != null) ? GetList(metaClass.Id) : null;
		}

		public static Rule[] GetList(string metaClassName)
		{
			return GetList(MetaClass.Load(metaClassName));
		}

		public static void Delete(int ruleId)
		{
			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_DeleteMetaRule"), new SqlParameter("@RuleId", ruleId));
		}

		public static void Save(Rule rule)
		{
			if (rule == null)
				throw new ArgumentNullException("rule");

			byte[] data = Serialize(rule);
			int MetaClassId = MetaClass.Load(rule._innerClassName).Id;

			SqlParameter retVal = new SqlParameter("@RetVal", SqlDbType.Int, 4);
			retVal.Direction = ParameterDirection.Output;

			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaRule"),
										new SqlParameter("@RuleId", rule._id),
										new SqlParameter("@MetaClassId", MetaClassId),
										new SqlParameter("@Data", data),
										retVal);

			rule._id = (int)retVal.Value;
		}

		public void Save()
		{
			Rule.Save(this);
		}

		public void Delete()
		{
			Rule.Delete(_id);
		}

		public static byte[] Serialize(Rule rule)
		{
			IFormatter formatter = new BinaryFormatter();

			using (MemoryStream stream = new MemoryStream(1024))
			{
				formatter.Serialize((Stream)stream, rule);
				stream.Capacity = (int)stream.Length;

				return stream.GetBuffer();
			}
		}

		public static Rule Deserialize(byte[] data)
		{
			IFormatter formatter = new BinaryFormatter();
			Rule retVal = null;

			using (Stream stream = new MemoryStream(data))
			{
				retVal = (Rule)formatter.Deserialize(stream);
			}
			return retVal;
		}

		public static void XmlSerialize(Rule rule, string fileName)
		{
			XmlTextWriter writer = new XmlTextWriter(fileName, System.Text.Encoding.GetEncoding(1251));
			writer.WriteStartDocument();
			writer.WriteStartElement("Rule");
			writer.WriteStartElement("Id");
			writer.WriteString(rule._id.ToString(CultureInfo.InvariantCulture));
			writer.WriteEndElement();
			writer.WriteStartElement("InnerClassName");
			writer.WriteString(rule._innerClassName);
			writer.WriteEndElement();

			foreach (KeyValuePair<string, string> item in rule._attributes)
			{
				writer.WriteStartElement((string)item.Key);
				writer.WriteAttributeString("auto", "yes");
				writer.WriteString((string)item.Value);
				writer.WriteEndElement();
			}

			foreach (RuleItem item in rule._items)
			{
				writer.WriteStartElement("RuleItem");
				writer.WriteStartElement("SourColumnName");
				writer.WriteString(item.SrcColumnName);
				writer.WriteEndElement();
				writer.WriteStartElement("SourColumnType");
				writer.WriteString(item.SrcColumnType.ToString());
				writer.WriteEndElement();
				writer.WriteStartElement("DestColumnName");
				writer.WriteString(item.DestColumnName);
				writer.WriteEndElement();
				writer.WriteStartElement("DestColumnType");
				writer.WriteString(item.DestColumnType.ToString());
				writer.WriteEndElement();
				writer.WriteStartElement("FillType");
				writer.WriteString(item.FillType.ToString());
				writer.WriteEndElement();
				writer.WriteStartElement("CustomValue");
				writer.WriteString(item.CustomValue);
				writer.WriteEndElement();
				writer.WriteStartElement("DestColumnSystem");
				writer.WriteString(item.DestColumnSystem.ToString());
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteEndDocument();

			writer.Close();
		}

		public static Rule XmlDeserialize(string fileName)
		{
			Rule rule = new Rule();

			XmlDocument doc = new XmlDocument();
			doc.PreserveWhitespace = true;
			doc.Load(fileName);

			XmlNode ruleNode = doc.SelectSingleNode("Rule");

			rule._id = Int32.Parse(ruleNode.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
			rule._innerClassName = ruleNode.SelectSingleNode("InnerClassName").InnerText;

			for (int i = 0; i < ruleNode.ChildNodes.Count; i++)
			{
				if (ruleNode.ChildNodes[i].Attributes != null && ruleNode.ChildNodes[i].Attributes["auto"] != null)
				{
					rule._attributes.Add(ruleNode.ChildNodes[i].Name, ruleNode.ChildNodes[i].InnerText);
				}
			}

			XmlNodeList itemNodes = ruleNode.SelectNodes("RuleItem");
			foreach (XmlNode itemNode in itemNodes)
			{
				string sourColName = itemNode.SelectSingleNode("SourColumnName").InnerText;
				Type sourColType = Type.GetType(itemNode.SelectSingleNode("SourColumnType").InnerText);
				string destColName = itemNode.SelectSingleNode("DestColumnName").InnerText;
				MetaDataType destColType = (MetaDataType)Enum.Parse(typeof(MetaDataType), itemNode.SelectSingleNode("DestColumnType").InnerText);
				bool destColSystem = Boolean.Parse(itemNode.SelectSingleNode("DestColumnSystem").InnerText);
				FillTypes fillType = (FillTypes)Enum.Parse(typeof(FillTypes), itemNode.SelectSingleNode("FillType").InnerText);
				string customValue = itemNode.SelectSingleNode("CustomValue").InnerText;

				rule.Add(new RuleItem(sourColName, sourColType, destColName, destColType, destColSystem, fillType, customValue));
			}

			return rule;
		}
	}
}
