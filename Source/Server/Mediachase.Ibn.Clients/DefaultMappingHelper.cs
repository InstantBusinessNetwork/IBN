using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core.Business.Mapping;
using System.Data;
using System.Resources;
using System.Web;
using System.Threading;
using System.Globalization;

namespace Mediachase.Ibn.Clients
{
	/// <summary>
	/// Represens logic creation default mapping document by saved pattern or metaField friendly names
	/// </summary>
	internal static class DefaultMappingHelper
	{
		private class FieldMathResult
		{
			public FieldMathResult(string origPattern, string comparePattern, string tag)
			{
				this.OrigPattern = origPattern;
				this.ComparePattern = comparePattern;
				this.Tag = tag;
			}
			public string Tag = string.Empty;
			private string OrigPattern = string.Empty;
			private string ComparePattern = string.Empty;
			public int Weight
			{
				get
				{
					int matchIndex = OrigPattern.IndexOf(ComparePattern);
					return matchIndex + (OrigPattern.Length - ComparePattern.Length - matchIndex);

				}
			}
		}

		/// <summary>
		/// Creates the maping by friendly name comparison.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <param name="dstClass">The DST class.</param>
		/// <param name="builder">The builder.</param>
		public static void CreateMapingByFriendlyNameComparison(DataColumn column, MetaClass dstClass, 
																MappingElementBuilder builder)
		{
			if (column == null)
				throw new ArgumentNullException("column");
			if (dstClass == null)
				throw new ArgumentNullException("dstClass");
			if (builder == null)
				throw new ArgumentNullException("builder");

			FieldMathResult matchResult = CreateMapingByFriendlyNameComparison(column, dstClass, string.Empty, null, builder);
			if (matchResult != null)
			{
				builder.AssignCopyValueRule(column.ColumnName, matchResult.Tag);
			}
		}

		/// <summary>
		/// Creates the default name of the mapping by.
		/// </summary>
		private static FieldMathResult CreateMapingByFriendlyNameComparison(DataColumn column, MetaClass dstClass,
																		   string prefix, FieldMathResult prevMatchResult, 
																		   MappingElementBuilder builder)
		{
			FieldMathResult retVal = prevMatchResult;
			foreach (MetaField metaField in dstClass.Fields)
			{
				string mappedFieldName = prefix + metaField.Name;
				// Skip always maped metaField
				if (builder.GetRuleByMetaField(mappedFieldName) != null)
					continue;
				// Skip Back Aggregation References
				if (metaField.Attributes.ContainsKey(McDataTypeAttribute.AggregationMark))
					continue;
				//Skip referenses
				if (metaField.IsReference)
					continue;

				// Process Aggregation
				if (metaField.IsAggregation)
				{
					// Find Aggr meta class
					MetaClass aggrMetaClass = DataContext.Current.GetMetaClass((string)metaField.Attributes[McDataTypeAttribute.AggregationMetaClassName]);
					// Find rules
					retVal = CreateMapingByFriendlyNameComparison(column, aggrMetaClass, metaField.Name + ".", retVal, builder);
					continue;
				}

				//Получаем имя в русской локализации
				string enName = GetWebResourceString(metaField.FriendlyName, CultureInfo.GetCultureInfo("en-US"));

				//получаем имя в английской локализации
				string ruName = GetWebResourceString(metaField.FriendlyName, CultureInfo.GetCultureInfo("ru-RU"));
				//Find best match for pattern
				foreach(string searchPattern in new string[] { enName, ruName })
				{
					if (column.ColumnName.Contains(searchPattern))
					{
						FieldMathResult newResult = new FieldMathResult(column.ColumnName, searchPattern, mappedFieldName);
						if (retVal != null)
						{
							retVal = retVal.Weight < newResult.Weight ? retVal : newResult;
						}
						else
						{
							retVal = newResult;
						}
						break;
					}
				}
			}

			return retVal;
		}


		/// <summary>
		/// Creates the default by mapping element.
		/// </summary>
		public static void CreateMapingByPatternComparision(DataTable srcData, MetaClass dstMetaClass, MappingElementBuilder builder)
		{
			if (srcData == null)
				throw new ArgumentNullException("srcData");
			if (dstMetaClass == null)
				throw new ArgumentNullException("dstMetaClass");
			if (builder == null)
				throw new ArgumentNullException("builder");

			//MappingDocument enDefaultMappingDoc = MappingDocument.LoadFromXml(VCardType.LocRM.GetString("DefaultMapping",
			//                                                                   CultureInfo.GetCultureInfo("en-US")));
			MappingDocument enDefaultMappingDoc = MappingDocument.LoadFromXml(GetWebResourceString("{IbnFramework.OutlookMappingPattern:Outlook2007}",
																			 CultureInfo.GetCultureInfo("en-US")));

			//MappingDocument ruDefaultMappingDoc = MappingDocument.LoadFromXml(VCardType.LocRM.GetString("DefaultMapping",
			//                                                                  CultureInfo.GetCultureInfo("ru-RU")));
			MappingDocument ruDefaultMappingDoc = MappingDocument.LoadFromXml(GetWebResourceString("{IbnFramework.OutlookMappingPattern:Outlook2007}",
																			  CultureInfo.GetCultureInfo("ru-RU")));

			MappingElement srcEl = null;
			MappingElement dstEl = null;
			//Recognize source lang and type
			DataColumnCollection dataColl = srcData.Columns;

			//Recognize language pattern
			if (enDefaultMappingDoc != null && enDefaultMappingDoc.Count != 0)
			{
				if (DataColumnSourceIsMatch(dataColl, enDefaultMappingDoc[0]))
				{
					srcEl = enDefaultMappingDoc[0];
					if (FieldDestinationIsMatch(dstMetaClass.Fields, enDefaultMappingDoc[0]))
					{
						dstEl = enDefaultMappingDoc[0];
					}
				}
			}

			if (ruDefaultMappingDoc != null && ruDefaultMappingDoc.Count != 0)
			{
				if (DataColumnSourceIsMatch(dataColl, ruDefaultMappingDoc[0]))
				{
					srcEl = ruDefaultMappingDoc[0];
					if (FieldDestinationIsMatch(dstMetaClass.Fields, ruDefaultMappingDoc[0]))
					{
						dstEl = ruDefaultMappingDoc[0];
					}
				}
			}

			//Pattern found, build mapping by pattern
			if (srcEl != null && dstEl != null)
			{
				for (int i = 0; i < srcEl.Count; i++)
				{
					builder.AssignCopyValueRule(srcEl[i].ColumnName, dstEl[i].FieldName);
				}
			}
			else
			{
				//Pattern not found, build mapping by field friendly names comparison
				foreach (DataColumn dataCol in srcData.Columns)
				{
					CreateMapingByFriendlyNameComparison(dataCol, dstMetaClass, builder);
				}
			}
		}


		private static bool FieldDestinationIsMatch(MetaFieldCollection metaFields, MappingElement mapEl)
		{
			bool retVal = false;
			foreach (MappingRule mapRule in mapEl)
			{
				foreach (MetaField field in metaFields)
				{
					if (mapRule.FieldName == field.Name)
					{
						retVal = true;
						break;
					}
				}

				if (retVal == false)
					break;

			}
			return retVal;
		}

		private static bool  DataColumnSourceIsMatch(DataColumnCollection dataColl, MappingElement mapEl)
		{
			bool retVal = false;
			foreach (MappingRule mapRule in mapEl)
			{
				foreach (DataColumn column in dataColl)
				{
					retVal = mapRule.ColumnName == column.ColumnName;
					if (retVal)
					{
						break;
					}
				}

				if (!retVal)
					break;
			}

			return retVal;
		}

		#region GetWebResourceString
		/// <summary>
		/// Gets the web resource string.
		/// </summary>
		/// <param name="template">The template.</param>
		/// <returns></returns>
		public static string GetWebResourceString(string template)
		{
			return GetWebResourceString(template, Thread.CurrentThread.CurrentCulture);
		}
		#endregion
		#region public static string GetWebResourceString(string template, CultureInfo culture)
		/// <summary>
		/// Gets the web resource string.
		/// </summary>
		/// <param name="template">The template.</param>
		/// <param name="culture">The culture.</param>
		/// <returns></returns>
		public static string GetWebResourceString(string template, CultureInfo culture)
		{
			string retVal = template;
			if (!string.IsNullOrEmpty(template))
			{
				int begin = template.IndexOf("{");
				int end = template.IndexOf("}");
				if (begin >= 0 && end >= begin)
				{
					string oldValue = template.Substring(begin, end - begin + 1);
					string classKey = "Global";
					string resourceKey = oldValue.Substring(1, oldValue.Length - 2);
					int separator = resourceKey.IndexOf(":");
					if (separator >= 0)
					{
						classKey = resourceKey.Substring(0, separator);
						resourceKey = resourceKey.Substring(separator + 1);
					}
					try
					{
						string resourceValue = HttpContext.GetGlobalResourceObject(classKey, resourceKey, culture) as string;
						if (resourceValue != null)
							retVal = template.Replace(oldValue, resourceValue);
					}
					catch (MissingManifestResourceException)
					{
					}
				}
			}
			return retVal;
		}

		#endregion
	}
}
