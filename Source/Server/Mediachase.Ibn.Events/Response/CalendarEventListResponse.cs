using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Events.Response
{
	public class CalendarEventListResponse : Mediachase.Ibn.Core.Business.Response
	{
			#region Const
		#endregion

		#region Fields
		Dictionary<EntityObject, string> _entityObjects;
		#endregion

		#region .Ctor
		public CalendarEventListResponse()
		{
		}
		public CalendarEventListResponse(Dictionary<EntityObject, string> entityObjects)
		{
			_entityObjects = entityObjects;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the target.
		/// </summary>
		/// <value>The target.</value>
		public Dictionary<EntityObject, string> EntityObjects
		{
			get { return _entityObjects; }
			set { _entityObjects = value; }
		}
		#endregion

		#region Methods
		#endregion
	}
}
