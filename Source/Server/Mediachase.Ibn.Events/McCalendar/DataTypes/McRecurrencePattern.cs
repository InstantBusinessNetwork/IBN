using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.iCal.DataTypes;
using Mediachase.iCal.Components;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Events.McCalendar.DataTypes
{
	public class McRecurrencePattern : RecurrencePattern
	{
		#region Custom parameter
		public const string METACLASS_NAME_PARAM = "X-MC-METACLASS-NAME";
		public const string PRIMARY_KEY_ID_PARAM = "X-MC-PRIMARY-KEY-ID";
		#endregion

		public McRecurrencePattern()
			:base()
		{

		}
		public McRecurrencePattern(string value)
			:base(value)
		{

		}

		#region Public Properties
		public string MetaClassName
		{
			get
			{
				string retVal = null;
				if (Parameters.ContainsKey(METACLASS_NAME_PARAM))
				{
					Parameter p = (Parameter)Parameters[METACLASS_NAME_PARAM];
					retVal = p.Values.Count != 0 ? p.Values[0] : null;
				}
				return retVal;
			}

			set
			{
				SetParameter(METACLASS_NAME_PARAM, value);
			}
		}
		public PrimaryKeyId? MetaObjectId
		{
			get
			{
				PrimaryKeyId? retVal = null;
				if (Parameters.ContainsKey(PRIMARY_KEY_ID_PARAM))
				{
					Parameter p = (Parameter)Parameters[PRIMARY_KEY_ID_PARAM];
					if(p.Values.Count != 0)
					{
						retVal = PrimaryKeyId.Parse(p.Values[0]);
					}
				}
				return retVal;
			}
			set
			{
				if (value != null)
				{
					SetParameter(PRIMARY_KEY_ID_PARAM, value.Value.ToString());
				}
			}
		}
		#endregion

		#region Private member
		private void SetParameter(string paramName, string paramValue)
		{
			if (Parameters.ContainsKey(paramName))
			{
				Parameter p = (Parameter)Parameters[paramName];
				p.Values.Add(paramValue);
			}
			else
			{
				this.AddParameter(paramName, paramValue);
			}
		}

		#endregion

	}
}
