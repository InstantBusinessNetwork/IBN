using System;
using System.Collections;
using Mediachase.IBN.Database.EMail;
using System.Text.RegularExpressions;
using Mediachase.Net.Mail;

namespace Mediachase.IBN.Business.EMail
{

	/// <summary>
	/// Summary description for EMailMessageAntiSpamRule.
	/// </summary>
	public class EMailMessageAntiSpamRule
	{
		public const string BlackListServiceName = "BlackList";
		public const string WhiteListServiceName = "WhiteList";
		public const string TicketServiceName = "Ticket";
		public const string IncidentBoxRulesServiceName = "IncidentBoxRules";

		private EMailMessageAntiSpamRuleRow _srcRow = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="EMailMessageAntiSpamRule"/> class.
		/// </summary>
		/// <param name="row">The row.</param>
		private EMailMessageAntiSpamRule(EMailMessageAntiSpamRuleRow row)
		{
			_srcRow = row;
		}

		/// <summary>
		/// Loads the specified anti spam rule id.
		/// </summary>
		/// <param name="AntiSpamRuleId">The anti spam rule id.</param>
		/// <returns></returns>
		public static EMailMessageAntiSpamRule Load(int AntiSpamRuleId)
		{
			return new EMailMessageAntiSpamRule(new EMailMessageAntiSpamRuleRow(AntiSpamRuleId));
		}

		public static int Create(bool Accept, EMailMessageAntiSpamRuleType Type, string Key, string Value, int Weight)
		{
			EMailMessageAntiSpamRuleRow newRow = new EMailMessageAntiSpamRuleRow();

			newRow.Accept = Accept;
			newRow.RuleType = (int)Type;
			newRow.Key = Key;
			newRow.Value = Value;
			newRow.Weight = Weight;

			newRow.Update();

			return newRow.PrimaryKeyId;
		}

		public static EMailMessageAntiSpamRule[] List()
		{
			ArrayList retVal = new ArrayList();

			foreach (EMailMessageAntiSpamRuleRow row in EMailMessageAntiSpamRuleRow.List())
			{
				retVal.Add(new EMailMessageAntiSpamRule(row));
			}

			return (EMailMessageAntiSpamRule[])retVal.ToArray(typeof(EMailMessageAntiSpamRule));
		}

		public static void Delete(int ruleId)
		{
			EMailMessageAntiSpamRuleRow.Delete(ruleId);
		}

		public static void Update(EMailMessageAntiSpamRule rule)
		{
			rule._srcRow.Update();
		}

		#region Check

		#region Util
		private static string GetStringFromEmailByKey(Pop3Message message, string Key)
		{
			switch (Key)
			{
				case "From":
					return message.Headers["From"];
				case "Sender":
					return message.Headers["Sender"];
				case "Reply-To":
					return message.Headers["Reply-To"];
				case "To":
					return message.Headers["To"];
				case "Subject":
					return message.Subject;
				case "Body":
					return message.BodyText;
				case "SubjectOrBody":
					return message.Subject + "\r\n" + message.BodyText;
				default:
					return message.Headers[Key];
			}
		}

		#endregion

		/// <summary>
		/// Checks the specified file name.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		public static EMailMessageAntiSpamRuleRusult Check(string fileName)
		{
			System.IO.StreamReader st = new System.IO.StreamReader(fileName, System.Text.Encoding.Default);
			string strmsg = st.ReadToEnd();

			byte[] buffer = System.Text.Encoding.Default.GetBytes(strmsg);
			System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer, 0, buffer.Length, true, true);

			Pop3Message msg = new Pop3Message(ms);

			return Check(EMailRouterPop3Box.ListExternal()[0], msg);
		}

		/// <summary>
		/// Checks the specified email box.
		/// </summary>
		/// <param name="emailBox">The email box.</param>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		public static EMailMessageAntiSpamRuleRusult Check(EMailRouterPop3Box emailBox, Pop3Message message)
		{
			// 2007-01-09: Check Empty From and Sender
			if (message.Sender == null && message.From == null || message.To == null)
				return EMailMessageAntiSpamRuleRusult.Deny;

			EMailMessageAntiSpamRule[] antiSpamRules = EMailMessageAntiSpamRule.List();

			// Load Rules
			//EMailMessageAntiSpamRule[] antiSpamRules = _antiSpamRules;

			// Check Rules

			foreach (EMailMessageAntiSpamRule rule in antiSpamRules)
			{
				if (((EMailMessageAntiSpamRuleType)rule.RuleType) != EMailMessageAntiSpamRuleType.Service)
				{
					string KeyValue = GetStringFromEmailByKey(message, rule.Key);

					if (KeyValue != null)
					{
						switch ((EMailMessageAntiSpamRuleType)rule.RuleType)
						{
							case EMailMessageAntiSpamRuleType.Contains:
								foreach (string ContainsSubStr in rule.Value.Split(';'))
								{
									string TrimContainsSubStr = ContainsSubStr.Trim();
									if (TrimContainsSubStr != string.Empty)
									{
										if (TrimContainsSubStr.IndexOfAny(new char[] { '*', '?' }) != -1)
										{
											if (Pattern.Match(KeyValue, TrimContainsSubStr))
												return rule.Accept ? EMailMessageAntiSpamRuleRusult.Accept : EMailMessageAntiSpamRuleRusult.Deny;
										}
										else
										{
											if (KeyValue.IndexOf(TrimContainsSubStr) != -1)
												return rule.Accept ? EMailMessageAntiSpamRuleRusult.Accept : EMailMessageAntiSpamRuleRusult.Deny;
										}
									}
								}
								break;
							case EMailMessageAntiSpamRuleType.IsEqual:
								if (string.Compare(KeyValue, rule.Value, true) == 0)
									return rule.Accept ? EMailMessageAntiSpamRuleRusult.Accept : EMailMessageAntiSpamRuleRusult.Deny;
								break;
							case EMailMessageAntiSpamRuleType.RegexMatch:
								Match match = Regex.Match(KeyValue, rule.Value);
								if ((match.Success && (match.Index == 0)) && (match.Length == KeyValue.Length))
									return rule.Accept ? EMailMessageAntiSpamRuleRusult.Accept : EMailMessageAntiSpamRuleRusult.Deny;
								break;
						}
					}
				}
				else
				{
					string FromEmail = EMailMessage.GetSenderEmail(message);

					switch (rule.Key)
					{
						case BlackListServiceName:
							if (BlackListItem.Contains(FromEmail))
								return rule.Accept ? EMailMessageAntiSpamRuleRusult.Accept : EMailMessageAntiSpamRuleRusult.Deny;
							break;
						case WhiteListServiceName:
							if (WhiteListItem.Contains(FromEmail))
								return rule.Accept ? EMailMessageAntiSpamRuleRusult.Accept : EMailMessageAntiSpamRuleRusult.Deny;
							break;
						case TicketServiceName:
							if (message.Subject != null && TicketUidUtil.LoadFromString(message.Subject) != string.Empty)
								return rule.Accept ? EMailMessageAntiSpamRuleRusult.Accept : EMailMessageAntiSpamRuleRusult.Deny;
							break;
						case IncidentBoxRulesServiceName:
							// Step 1. Get Incident Info
							IEMailIncidentMapping mappingHandler = EMailIncidentMappingHandler.LoadHandler(emailBox.Settings.SelectedHandlerId);
							IncidentInfo incidentInfo = mappingHandler.Create(emailBox, message);

							// Step 2. Evaluate IncidentBoxRules
							IncidentBox box = IncidentBoxRule.Evaluate(incidentInfo, false);
							if (box != null)
								return rule.Accept ? EMailMessageAntiSpamRuleRusult.Accept : EMailMessageAntiSpamRuleRusult.Deny;
							break;
					}


				}
			}

			return EMailMessageAntiSpamRuleRusult.Pending;
		}
		#endregion

		#region Public Properties

		public virtual int AntiSpamRuleId
		{
			get
			{
				return _srcRow.AntiSpamRuleId;
			}
		}

		public virtual bool Accept
		{
			get
			{
				return _srcRow.Accept;
			}

			set
			{
				_srcRow.Accept = value;
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

		public virtual EMailMessageAntiSpamRuleType RuleType
		{
			get
			{
				return (EMailMessageAntiSpamRuleType)_srcRow.RuleType;
			}

			set
			{
				_srcRow.RuleType = (int)value;
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

		public virtual int Weight
		{
			get
			{
				return _srcRow.Weight;
			}

			set
			{
				_srcRow.Weight = value;
			}
		}

		#endregion
	}
}
