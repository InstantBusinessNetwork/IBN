using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mediachase.Sync.Core.Common
{
	public class CycleCollection<T> : IEnumerable<T>
	{
		private class CycleIterator<T1> : IEnumerator<T1>
		{
			CycleCollection<T1> _owner;
			private int _index;
			public CycleIterator(CycleCollection<T1> owner, int index)
			{
				_owner = owner;
				_index = index;
			}

			#region IEnumerator<T1> Members

			public T1 Current
			{
				get { return _owner.DataVals[_index]; }
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
				bool retVal = _owner.EnableCycling;

				if (retVal)
				{
					_index++;
					if (_index == _owner.DataVals.Length)
					{
						_index = 0;
					}
				}
				else
				{
					_index = 0;
				}

				return retVal;
			}

			public void Reset()
			{
				_index = 0;
			}

			#endregion
		}

		private bool _enableCycling = true;
		private object _tag;
		private T[] _data;
		private int _defaultIndex;
		private CycleIterator<T> _iterator;

		private CycleCollection(T[] data, int defaultIndex)
		{
			_data = data;
			_defaultIndex = defaultIndex;
		}

		public static CycleCollection<T2> CreateInstance<T2>(T2 defaultItem, T2[] dataVals)
		{
			if (dataVals == null || dataVals.Length == 0)
				throw new Exception("data vals not set");

			int defaultIndex = Array.IndexOf(dataVals, defaultItem);
			return new CycleCollection<T2>(dataVals, defaultIndex);
		}

		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}
		public T[] DataVals
		{
			get { return _data; }
		}
		public bool EnableCycling
		{
			get { return _enableCycling; }
			set { _enableCycling = value; }
		}

		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator()
		{
			if (_iterator == null)
			{
				_iterator = new CycleIterator<T>(this, _defaultIndex);
			}
			return _iterator;
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
