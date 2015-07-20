using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Events.McCalendar.Serialization.Algoritm
{
	public class EntityPropertyEnumerator : IEnumerable<EntityObjectProperty>
	{
		#region Const
		#endregion

		#region Fields
		EntityObject _entity;
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="IntervalGenerator<T> : IEnumerable<T>"/> class.
		/// </summary>
		public EntityPropertyEnumerator(EntityObject entity)
		{
			_entity = entity;
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		#endregion

		#region IEnumerable<EntityObjectProperty> Members

		public IEnumerator<EntityObjectProperty> GetEnumerator()
		{
			return new EntityPropertyIterator(_entity);
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}

}
