using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Events.McCalendar.Serialization.Algoritm
{
	public class EntityPropertyIterator : IEnumerator<EntityObjectProperty>
	{
		#region Const
		#endregion

		#region Fields
		IEnumerator<EntityObjectProperty> _current;
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="RangeIntervalIterator"/> class.
		/// </summary>
		public EntityPropertyIterator(EntityObject entity)
		{
			_current = entity.Properties.GetEnumerator();
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		#endregion

		#region IEnumerator<EntityObjectProperty> Members

		public EntityObjectProperty Current
		{
			get { return _current.Current; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			
		}

		#endregion

		#region IEnumerator Members

		object System.Collections.IEnumerator.Current
		{
			get { return Current; }
		}

		public bool MoveNext()
		{
			return _current.MoveNext();
		}

		public void Reset()
		{
			_current.Reset();
		}

		#endregion
	}

}
