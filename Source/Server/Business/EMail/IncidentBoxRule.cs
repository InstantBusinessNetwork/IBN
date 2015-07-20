using System;
using System.Collections;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.EMail;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace Mediachase.IBN.Business.EMail
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	/*					[Key]				[Value]
	 Contains								Aaaa;BBB;CCC;d?*d
						MailSenderEmail
						Title
						Description
						TypeId				1
						PriorityId			2
						SeverityId			3	
						GeneralCategories	1;2;
						IncidentCategories	3;5;4;
						[MDPField]
						
	 RegexMatch								RegExpression string
						MailSenderEmail
						Title
						Description
						[MDPField]
						
	IsEqual									string
						MailSenderEmail
						Title
						Description
						Project
						[MDPField]
						
	Function			FunctionId			"[Title]";"Param2";"Param3"
	
	OrBlock				String.Empty		String.Empty
	AndBlock			String.Empty		String.Empty
	*/
	///////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// 
	/// </summary>
	public enum IncidentBoxRuleType
	{
		Contains = 0,
		RegexMatch = 1,
		IsEqual = 2,
		FromEMailBox = 3,
		OrBlock = 4,
		AndBlock = 5,
		Function = 6,
		NotContains = 7,
		NotIsEqual = 8
	}

	/// <summary>
	/// Summary description for IncidentBoxRule.
	/// </summary>
	public class IncidentBoxRule
	{
		IncidentBoxRuleRow _srcRow = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="IncidentBoxRule"/> class.
		/// </summary>
		/// <param name="row">The row.</param>
		private IncidentBoxRule(IncidentBoxRuleRow row)
		{
			_srcRow = row;
		}

		/// <summary>
		/// Lists the specified incident box id.
		/// </summary>
		/// <param name="IncidentBoxId">The incident box id.</param>
		/// <returns></returns>
		public static IncidentBoxRule[] List(int IncidentBoxId)
		{
			ArrayList retVal = new ArrayList();

			foreach (IncidentBoxRuleRow row in IncidentBoxRuleRow.List(IncidentBoxId))
			{
				retVal.Add(new IncidentBoxRule(row));
			}

			return (IncidentBoxRule[])retVal.ToArray(typeof(IncidentBoxRule));
		}

		/// <summary>
		/// Creates the specified incident box id.
		/// </summary>
		/// <param name="IncidentBoxId">The incident box id.</param>
		/// <param name="OutlineIndex">Index of the outline.</param>
		/// <param name="RuleType">Type of the Rule.</param>
		/// <param name="Key">The key.</param>
		/// <param name="Value">The value.</param>
		/// <returns></returns>
		public static int Create(int IncidentBoxId, int OutlineIndex, IncidentBoxRuleType RuleType, string Key, string Value)
		{
			IncidentBoxRuleRow newRow = new IncidentBoxRuleRow();

			newRow.IncidentBoxId = IncidentBoxId;

			newRow.OutlineIndex = OutlineIndex;
			newRow.OutlineLevel = ".";

			newRow.RuleType = (int)RuleType;
			newRow.Key = Key;
			newRow.Value = Value;

			newRow.Update();

			return newRow.PrimaryKeyId;
		}

		/// <summary>
		/// Creates the child.
		/// </summary>
		/// <param name="IncidentBoxId">The incident box id.</param>
		/// <param name="ParentIncidentBoxRuleId">The parent incident box rule id.</param>
		/// <param name="OutlineIndex">Index of the outline.</param>
		/// <param name="RuleType">Type of the rule.</param>
		/// <param name="Key">The key.</param>
		/// <param name="Value">The value.</param>
		/// <returns></returns>
		public static int CreateChild(int IncidentBoxId, int ParentIncidentBoxRuleId, int OutlineIndex, IncidentBoxRuleType RuleType, string Key, string Value)
		{
			IncidentBoxRuleRow parentRule = new IncidentBoxRuleRow(ParentIncidentBoxRuleId);
			if ((parentRule.RuleType & (int)(IncidentBoxRuleType.AndBlock | IncidentBoxRuleType.OrBlock)) == 0)
				throw new ArgumentException("ParentIncidentBoxRule RuleType Should be either AndBlock or OrBlock.", "ParentIncidentBoxRuleId");

			IncidentBoxRuleRow newRow = new IncidentBoxRuleRow();

			newRow.IncidentBoxId = IncidentBoxId;

			newRow.OutlineIndex = OutlineIndex;
			newRow.OutlineLevel = string.Format("{0}{1}.", parentRule.OutlineLevel, parentRule.IncidentBoxRuleId);

			newRow.RuleType = (int)RuleType;
			newRow.Key = Key;
			newRow.Value = Value;

			newRow.Update();

			return newRow.PrimaryKeyId;
		}

		/// <summary>
		/// Gets the index of the available.
		/// </summary>
		/// <param name="IncidentBoxId">The incident box id.</param>
		/// <returns></returns>
		public static int[] GetAvailableIndex(int IncidentBoxId)
		{
			IncidentBoxRuleRow[] rules = IncidentBoxRuleRow.List(IncidentBoxId);

			ArrayList retVal = new ArrayList();

			int index = 0;
			for (index = 0; index < rules.Length; index++)
			{
				IncidentBoxRuleRow rule = rules[index];
				if (rule.OutlineLevel == ".")
					retVal.Add((int)index);
			}

			retVal.Add(index);

			return (int[])retVal.ToArray(typeof(int));
		}

		/// <summary>
		/// Gets the index of the available.
		/// </summary>
		/// <param name="IncidentBoxId">The incident box id.</param>
		/// <param name="ParentIncidentBoxRuleId">The parent incident box rule id.</param>
		/// <returns></returns>
		public static int[] GetAvailableIndex(int IncidentBoxId, int ParentIncidentBoxRuleId)
		{
			IncidentBoxRuleRow[] rules = IncidentBoxRuleRow.List(IncidentBoxId);

			ArrayList retVal = new ArrayList();

			string outlineLevelMask = string.Format(".{0}.", ParentIncidentBoxRuleId);

			int parendIndex = 0;
			int childItems = 0;

			for (int index = 0; index < rules.Length; index++)
			{
				IncidentBoxRuleRow rule = rules[index];

				if (rule.IncidentBoxRuleId == ParentIncidentBoxRuleId)
					parendIndex = index;

				if (rule.OutlineLevel.Contains(outlineLevelMask))
					childItems++;

				if (rule.OutlineLevel.EndsWith(outlineLevelMask))
					retVal.Add((int)index);
			}

			if (retVal.Count > 0)
				retVal.Add(parendIndex + 1 + childItems);
			else
				retVal.Add(parendIndex + 1);

			return (int[])retVal.ToArray(typeof(int));
		}

		/// <summary>
		/// Deletes the specified incident box Rule id.
		/// </summary>
		/// <param name="IncidentBoxRuleId">The incident box Rule id.</param>
		public static void Delete(int IncidentBoxRuleId)
		{
			IncidentBoxRuleRow.Delete(IncidentBoxRuleId);
		}

		/// <summary>
		/// Deletes all.
		/// </summary>
		/// <param name="IncidentBoxId">The incident box id.</param>
		public static void DeleteAll(int IncidentBoxId)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				foreach (IncidentBoxRule rule in IncidentBoxRule.List(IncidentBoxId))
				{
					IncidentBoxRule.Delete(rule.IncidentBoxRuleId);
				}

				tran.Commit();
			}
		}


		/// <summary>
		/// Updates the specified Rule.
		/// </summary>
		/// <param name="Rule">The Rule.</param>
		public static void Update(IncidentBoxRule Rule)
		{
			Rule._srcRow.Update();
		}

		/// <summary>
		/// Updates the specified rule.
		/// </summary>
		/// <param name="Rule">The rule.</param>
		/// <param name="NewOutlineIndex">New index of the outline.</param>
		public static void Update(IncidentBoxRule Rule, int NewOutlineIndex)
		{
			Rule._srcRow.OutlineIndex = NewOutlineIndex;
			Rule._srcRow.Update();
		}

		/// <summary>
		/// Changes the index of the outline.
		/// </summary>
		/// <param name="IncidentBoxRuleId">The incident box Rule id.</param>
		/// <param name="NewOutlineIndex">New index of the outline.</param>
		public static void ChangeOutlineIndex(int IncidentBoxRuleId, int NewOutlineIndex)
		{
			IncidentBoxRuleRow newRow = new IncidentBoxRuleRow(IncidentBoxRuleId);

			newRow.OutlineIndex = NewOutlineIndex;

			newRow.Update();
		}

		/// <summary>
		/// Loads the specified incident box Rule id.
		/// </summary>
		/// <param name="IncidentBoxRuleId">The incident box Rule id.</param>
		/// <returns></returns>
		public static IncidentBoxRule Load(int IncidentBoxRuleId)
		{
			return new IncidentBoxRule(new IncidentBoxRuleRow(IncidentBoxRuleId));
		}

		#region Evaluate
		//public static IncidentBox Evaluate(int pop3BoxId, string emalFilePath)
		//{
		//    DefaultEMailIncidentMapping mapping = new DefaultEMailIncidentMapping();
		//    IncidentInfo info = mapping.Create(EMailRouterPop3Box.Load(pop3BoxId), emalFilePath);
		//    return Evaluate(info);
		//}

		/// <summary>
		/// Evaluates the specified E mail message id.
		/// </summary>
		/// <param name="EMailincidentInfoId">The E mail message id.</param>
		/// <returns></returns>
		public static IncidentBox Evaluate(IncidentInfo incidentInfo)
		{
			return Evaluate(incidentInfo, true);
		}

		public static IncidentBox Evaluate(IncidentInfo incidentInfo, bool returnDefault)
		{
			IncidentBox defaultBox = null;

			// Load incidentInfo Row
			foreach (IncidentBox box in IncidentBox.ListWithRules())
			{
				if (Evaluate(box.IncidentBoxId, incidentInfo))
					return box;

				//if (box.IsDefault)
				//    defaultBox = box;
			}

			// OZ: Find Default Box
			if (defaultBox == null && returnDefault)
			{
				foreach (IncidentBox box in IncidentBox.List())
				{
					if (box.IsDefault)
					{
						defaultBox = box;
						break;
					}
				}
			}

			return defaultBox;
		}

		/// <summary>
		/// Evaluates the specified incident box id.
		/// </summary>
		/// <param name="IncidentBoxId">The incident box id.</param>
		/// <returns></returns>
		public static bool Evaluate(int IncidentBoxId, IncidentInfo incidentInfo)
		{
			IncidentBoxRule[] ruleList = IncidentBoxRule.List(IncidentBoxId);

			if (ruleList.Length == 0)
				return false;

			int Index = 0;
			return EvaluateBlock(true, string.Empty, incidentInfo, ruleList, ref Index);

		}

		private static string GetStringFromObject(object Value)
		{
			if (Value == null)
				return null;

			if (Value is IEnumerable && !(Value is string))
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder(255);

				foreach (object Item in (IEnumerable)Value)
				{
					sb.Append(Item.ToString());
					sb.Append(';');
				}

				return sb.ToString();
			}

			return Value.ToString();
		}

		private static string GetStringFromEmailByKey(IncidentInfo incidentInfo, string Key)
		{
			object objRetVal = null;

			Type incidentInfoType = typeof(IncidentInfo);

			FieldInfo fi = incidentInfoType.GetField(Key);
			if (fi != null)
			{
				objRetVal = fi.GetValue(incidentInfo);
			}
			else
			{
				PropertyInfo pi = incidentInfoType.GetProperty(Key);
				if (pi != null)
				{
					objRetVal = pi.GetValue(incidentInfo, null);
				}
				else
				{
					// TODO: Try to Calculate MetaFields
				}
			}

			return GetStringFromObject(objRetVal);
		}

		private static void GoToBlockEnd(string BlockOutlineLevel, IncidentBoxRule[] ruleList, ref int Index)
		{
			for (; Index < ruleList.Length; Index++)
			{
				IncidentBoxRule Rule = ruleList[Index];
				if (Rule.OutlineLevel.Length <= BlockOutlineLevel.Length)
				{
					Index--;
					return;
				}
			}
		}

		private static ArrayList GetKeyArray(string strKeys)
		{
			ArrayList retVal = new ArrayList();

			strKeys = strKeys.ToUpper();

			if (strKeys.Contains("\r\n"))
			{
				retVal.Add(strKeys);
			}
			else
			{
				foreach (string strSubKey in strKeys.Split(';'))
				{
					if (strSubKey != string.Empty)
					{
						retVal.Add(strSubKey);
					}
				}
			}

			return retVal;
		}

		private static bool IsNumber(string str)
		{
			if (str == null || str == string.Empty)
				return false;

			for (int index = 0; index < str.Length; index++)
			{
				if (!char.IsNumber(str, index))
					return false;
			}
			return true;
		}

		private static bool EvaluateContains(IncidentInfo incidentInfo, IncidentBoxRule Rule)
		{
			string KeyValue = GetStringFromEmailByKey(incidentInfo, Rule.Key);

			if (KeyValue == null || KeyValue == string.Empty)
				return false;

			ArrayList keyValues = GetKeyArray(KeyValue);
			ArrayList ruleValues = GetKeyArray(Rule.Value);

			bool bIsNumber = true;

			foreach (string rule in ruleValues)
			{
				if (!IsNumber(rule))
				{
					bIsNumber = false;
					break;
				}
			}

			foreach (string rule in ruleValues)
			{
				bool bFound = false;

				foreach (string key in keyValues)
				{
					if (key == rule)
					{
						bFound = true;
						break;
					}
					else
						if (!bIsNumber && Pattern.Match(key, string.Format("*{0}*", rule)))
						{
							bFound = true;
							break;
						}
				}

				if (!bFound)
					return false;
			}

			return true;
		}

		private static bool EvaluateIsEqual(IncidentInfo incidentInfo, IncidentBoxRule Rule)
		{
			string KeyValue = GetStringFromEmailByKey(incidentInfo, Rule.Key);

			if (KeyValue == null || KeyValue == string.Empty)
				return false;

			ArrayList keyValues = GetKeyArray(KeyValue);
			ArrayList ruleValues = GetKeyArray(Rule.Value);

			foreach (string key in keyValues)
			{
				bool bFound = false;

				foreach (string rule in ruleValues)
				{
					if (rule.IndexOfAny(new char[] { '*', '?' }) != -1)
					{
						if (Pattern.Match(key, rule))
						{
							bFound = true;
							break;
						}
					}
					else if (key == rule)
					{
						bFound = true;
						break;
					}
				}

				if (!bFound)
					return false;
			}

			return true;
		}

		private static bool EvaluateRegexMatch(IncidentInfo incidentInfo, IncidentBoxRule Rule)
		{
			string KeyValue = GetStringFromEmailByKey(incidentInfo, Rule.Key);
			Match match = Regex.Match(KeyValue, Rule.Value, RegexOptions.IgnoreCase);
			if ((match.Success && (match.Index == 0)) && (match.Length == KeyValue.Length))
				return true;
			return false;
		}

		private static bool EvaluateFunction(IncidentInfo incidentInfo, IncidentBoxRule Rule)
		{
			int FunctionId = int.Parse(Rule.Key);

			IncidentBoxRuleFunction function = IncidentBoxRuleFunction.Load(FunctionId);

			ArrayList paramItems = new ArrayList();

			foreach (Match match in Regex.Matches(Rule.Value, "\"(?<Param>[^\"]+)\";?", RegexOptions.IgnoreCase | RegexOptions.Singleline))
			{
				string Value = match.Groups["Param"].Value;
				if (Value.StartsWith("[") && Value.EndsWith("]"))
				{
					Value = GetStringFromEmailByKey(incidentInfo, Value.Trim('[', ']'));
				}

				paramItems.Add(Value);
			}

			return function.Invoke((string[])paramItems.ToArray(typeof(string)));
		}

		private static bool EvaluateBlock(bool AndBlock, string BlockOutlineLevel, IncidentInfo incidentInfo, IncidentBoxRule[] ruleList, ref int Index)
		{
			if (Index >= ruleList.Length)
				return false;

			// Empty block is false ...
			bool bResult = false;

			for (; Index < ruleList.Length; Index++)
			{
				IncidentBoxRule Rule = ruleList[Index];

				if (Rule.OutlineLevel.Length <= BlockOutlineLevel.Length)
				{
					Index--;
					return bResult;
				}

				bResult = AndBlock ? true : false;

				switch (Rule.RuleType)
				{
					case IncidentBoxRuleType.AndBlock:
						#region AndBlock
						Index++; // Increase Index to move next element
						bResult = EvaluateBlock(true, Rule.OutlineLevel, incidentInfo, ruleList, ref Index);
						#endregion
						break;
					case IncidentBoxRuleType.OrBlock:
						#region OrBlock
						Index++; // Increase Index to move next element
						bResult = EvaluateBlock(false, Rule.OutlineLevel, incidentInfo, ruleList, ref Index);
						#endregion
						break;
					case IncidentBoxRuleType.Contains:
						bResult = EvaluateContains(incidentInfo, Rule);
						break;
					case IncidentBoxRuleType.FromEMailBox:
						//bResult = EvaluateFromEMailBox(EMailRouterPop3BoxId, Rule);
						break;
					case IncidentBoxRuleType.IsEqual:
						bResult = EvaluateIsEqual(incidentInfo, Rule);
						break;
					case IncidentBoxRuleType.RegexMatch:
						bResult = EvaluateRegexMatch(incidentInfo, Rule);
						break;
					case IncidentBoxRuleType.Function:
						bResult = EvaluateFunction(incidentInfo, Rule);
						break;
					case IncidentBoxRuleType.NotContains:
						bResult = !EvaluateContains(incidentInfo, Rule);
						break;
					case IncidentBoxRuleType.NotIsEqual:
						bResult = !EvaluateIsEqual(incidentInfo, Rule);
						break;
				}

				if (AndBlock)
				{
					if (!bResult)
					{
						GoToBlockEnd(BlockOutlineLevel, ruleList, ref Index);
						return false;
					}
				}
				else
				{
					if (bResult)
					{
						GoToBlockEnd(BlockOutlineLevel, ruleList, ref Index);
						return true;
					}
				}

			}

			return bResult;
		}

		#endregion

		#region Public Properties

		public virtual int IncidentBoxRuleId
		{
			get
			{
				return _srcRow.IncidentBoxRuleId;
			}

		}

		public virtual IncidentBoxRuleType RuleType
		{
			get
			{
				return (IncidentBoxRuleType)_srcRow.RuleType;
			}
			set
			{
				_srcRow.RuleType = (int)value;
			}
		}

		public virtual int OutlineIndex
		{
			get
			{
				return _srcRow.OutlineIndex;
			}
		}

		public virtual string OutlineLevel
		{
			get
			{
				return _srcRow.OutlineLevel;
			}
		}

		public virtual string Key
		{
			get
			{
				return _srcRow.Key;
			}

			set
			{
				_srcRow.Key = value;
			}

		}

		public virtual string Value
		{
			get
			{
				return _srcRow.Value;
			}

			set
			{
				_srcRow.Value = value;
			}

		}

		#endregion
	}
}
