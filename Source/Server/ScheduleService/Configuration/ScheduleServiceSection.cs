using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Mediachase.Schedule.Service.Configuration
{
	/// <summary>
	/// Represents ScheduleServiceSectionGroup.
	/// </summary>
	public class ScheduleServiceSection : ConfigurationSection
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets the web services.
		/// </summary>
		/// <value>The web services.</value>
		[ConfigurationProperty("webServices", IsDefaultCollection = true)]
		public WebServiceElementCollection WebServices
		{
			get
			{
				return (WebServiceElementCollection)base["webServices"];
			}
		}

		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ScheduleServiceSectionGroup"/> class.
		/// </summary>
		public ScheduleServiceSection()
		{
		}
		#endregion

		#region Methods
		#endregion

		
	}
}
