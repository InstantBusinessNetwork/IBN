using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.IBN.Database.EMail;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Represents Outgoing Email Service Config.
	/// </summary>
	public static class OutgoingEmailServiceConfig
	{
		#region Const
		#endregion

		#region Properties
		#endregion

		#region Methods
		#region AssignWithSmtpBox
		/// <summary>
		/// Assigns the with SMTP box.
		/// </summary>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="smtpBoxId">The SMTP box id.</param>
		public static void AssignWithSmtpBox(OutgoingEmailServiceType serviceType, int smtpBoxId)
		{
			AssignWithSmtpBox(serviceType, null, smtpBoxId);
		}

		/// <summary>
		/// Assigns the with SMTP box.
		/// </summary>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="key">The key.</param>
		/// <param name="smtpBoxId">The SMTP box id.</param>
		public static void AssignWithSmtpBox(OutgoingEmailServiceType serviceType, int? key, int smtpBoxId)
		{
			OutgoingEmailServiceConfigRow row = FindConfigRow(serviceType, key);
			if(row==null)
			{
				row = new OutgoingEmailServiceConfigRow();
				row.ServiceType = (int)serviceType;
				row.ServiceKey = key;
			}
			row.SmtpBoxId = smtpBoxId;
			row.Update();
		} 
		#endregion

		#region AssignWithDefaultSmtpBox
		/// <summary>
		/// Assigns the with default SMTP box.
		/// </summary>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="key">The key.</param>
		public static void AssignWithDefaultSmtpBox(OutgoingEmailServiceType serviceType, int? key)
		{
			OutgoingEmailServiceConfigRow row = FindConfigRow(serviceType, key);
			if (row != null)
				row.Delete();
		}
		#endregion


		#region FindSmtpBox
		/// <summary>
		/// Finds the SMTP box.
		/// </summary>
		/// <param name="serviceType">Type of the service.</param>
		/// <returns></returns>
		public static SmtpBox FindSmtpBox(OutgoingEmailServiceType serviceType)
		{
			return FindSmtpBox(serviceType, null, true);
		}

		/// <summary>
		/// Finds the SMTP box.
		/// </summary>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="returnDefault">if set to <c>true</c> [return default].</param>
		/// <returns></returns>
		public static SmtpBox FindSmtpBox(OutgoingEmailServiceType serviceType, bool returnDefault)
		{
			return FindSmtpBox(serviceType, null, returnDefault);
		}

		/// <summary>
		/// Finds the SMTP box.
		/// </summary>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static SmtpBox FindSmtpBox(OutgoingEmailServiceType serviceType, int? key)
		{
			return FindSmtpBox(serviceType, key, true);
		}

		/// <summary>
		/// Finds the Smtp box.
		/// </summary>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="key">The key.</param>
		/// <param name="returnDefault">if set to <c>true</c> [return default].</param>
		/// <returns></returns>
		public static SmtpBox FindSmtpBox(OutgoingEmailServiceType serviceType, int? key, bool returnDefault)
		{
			OutgoingEmailServiceConfigRow config = FindConfigRow(serviceType, key);

			if (config!=null)
			{
				return SmtpBox.Load(config.SmtpBoxId);
			}

			// Return Default
			if(returnDefault)
				return SmtpBox.GetDefault();

			return null;
		}

		#endregion

		#region FindConfigRow
		/// <summary>
		/// Finds the config row.
		/// </summary>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		private static OutgoingEmailServiceConfigRow FindConfigRow(OutgoingEmailServiceType serviceType, int? key)
		{
			OutgoingEmailServiceConfigRow[] configs = OutgoingEmailServiceConfigRow.List(
						 FilterElement.EqualElement(OutgoingEmailServiceConfigRow.ColumnServiceType, serviceType),
						 key.HasValue ? 
							 FilterElement.EqualElement(OutgoingEmailServiceConfigRow.ColumnServiceKey, key):
							 FilterElement.IsNullElement(OutgoingEmailServiceConfigRow.ColumnServiceKey));

			if (configs.Length > 0)
				return configs[0];

			return null;
		} 
		#endregion

		#endregion

		
	}
}
