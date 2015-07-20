using System;
using System.Collections;
using System.Web.UI;
using System.Drawing.Design;
using System.ComponentModel;

namespace Mediachase.Web.UI.WebControls
{
	/// <summary>
	/// Summary description for OwnerCollection.
	/// </summary>
	public class OwnerCollection : CollectionBase
	{
		//private ArrayList _Items			=	new ArrayList();

		private Calendar _Calendar;
		public OwnerCollection(Calendar cal)
		{
			_Calendar = cal;
		}

		public Owner this[int index] 
		{
			get
			{
				return InnerList[index] as Owner;
			}
		}

		public Owner this[object Value] 
		{
			get
			{
				int index = InnerList.IndexOf(Value);
				if(index>0)
					return InnerList[index] as Owner;
				else
					return null;
			}
		}

		public Owner this[string Value] 
		{
			get
			{
				foreach(Owner owner in InnerList)
				{
					if(owner.Value.ToString().CompareTo(Value)==0)
						return owner;
				}
				return null;
			}
		}

		public void Add(Owner Value)
		{
			InnerList.Add(Value);
		}

		public void Add(object Value, string text)
		{
			InnerList.Add(new Owner(Value, text));
		}

		public int IndexOf(Owner Value)
		{
			return InnerList.IndexOf(Value);
		}

		public int IndexOf(Holiday value, int StartIndex, int Count)
		{
			return InnerList.IndexOf(value, StartIndex, Count);
		}

		public int IndexOf(Holiday value, int StartIndex)
		{
			return InnerList.IndexOf(value, StartIndex);
		}


		new public void  Clear()
		{
			InnerList.Clear();
		}

		public  bool Contains(Holiday value)
		{
			return InnerList.Contains(value);
		}

		new public void  RemoveAt(int Index)
		{
			InnerList.RemoveAt(Index);
		}


		/*
		#region Implementation of ICollection
		public void CopyTo(System.Array array, int index)
		{
			_Items.CopyTo(array, index);
		}

		public bool IsSynchronized
		{
			get
			{
				return _Items.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return _Items.Count;
			}
		}

		public object SyncRoot
		{
			get
			{
				return _Items.SyncRoot;
			}
		}
		#endregion

		#region Implementation of IEnumerable
		public System.Collections.IEnumerator GetEnumerator()
		{
			return _Items.GetEnumerator();
		}
		#endregion
		*/
	}
}
