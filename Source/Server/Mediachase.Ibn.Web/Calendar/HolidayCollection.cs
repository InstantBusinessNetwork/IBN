using System;
using System.Collections;
using System.Web.UI;
using System.Drawing.Design;
using System.ComponentModel;

namespace Mediachase.Web.UI.WebControls
{
	/// <summary>
	/// Summary description for HolidayCollection.
	/// </summary>
	public class HolidayCollection : IEnumerable, ICollection
	{
		ArrayList InnerList = new ArrayList();
		private Calendar _Calendar;
		public HolidayCollection(Calendar owner)
		{
			_Calendar = owner;
		}

		public Holiday this[int index] 
		{
			get
			{				
				return InnerList[index] as Holiday;
			}
		}

		public bool IsHoliday(DateTime date)
		{
			foreach(Holiday item in InnerList)
			{
				if(item.IsHoliday(date))
					return true;
			}
			return false;
		}

		public void Add(Holiday value)
		{
			InnerList.Add(value);
		}

		public int IndexOf(Holiday value)
		{
			return InnerList.IndexOf(value);
		}

		public int IndexOf(Holiday value, int StartIndex, int Count)
		{
			return InnerList.IndexOf(value, StartIndex, Count);
		}

		public int IndexOf(Holiday value, int StartIndex)
		{
			return InnerList.IndexOf(value, StartIndex);
		}

		public  bool Contains(Holiday value)
		{
			return InnerList.Contains(value);
		}

		public void Clear()
		{
			InnerList.Clear();
		}


		#region Implementation of ICollection
		public void CopyTo(System.Array array, int index)
		{
			InnerList.CopyTo(array, index);
		}

		public bool IsSynchronized
		{
			get
			{
				return InnerList.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return InnerList.Count;
			}
		}

		public object SyncRoot
		{
			get
			{
				return InnerList.SyncRoot;
			}
		}
		#endregion

		#region Implementation of IEnumerable
		public System.Collections.IEnumerator GetEnumerator()
		{
			return InnerList.GetEnumerator();
		}
		#endregion
	}
}
