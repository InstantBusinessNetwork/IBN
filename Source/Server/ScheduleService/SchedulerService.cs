using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mediachase.Schedule.Service.IbnPortalSchedulerService
{
	/// <summary>
	/// Extends SchedulerService auto-generated class.
	/// </summary>
	partial class SchedulerService
	{
		/// <summary>
		/// Gets or sets the activated.
		/// </summary>
		/// <value>The activated.</value>
		public DateTime? Activated { get; set; }

		/// <summary>
		/// Gets or sets the completed.
		/// </summary>
		/// <value>The completed.</value>
		public DateTime? Completed { get; set; }

		/// <summary>
		/// Gets or sets the result.
		/// </summary>
		/// <value>The result.</value>
		public AsyncCompletedEventArgs Result { get; set; }
	}
}
