using System;
using System.Collections;

namespace Mediachase.Web.UI.WebControls.Util
{
	[Serializable]
	public class ItemLinkedList : IList
	{
		private ItemNode _HeaderNode = null;
		private int _Count = 0;

		private ItemNode HeaderNode
		{
			get
			{
				return _HeaderNode;
			}
		}

		public int Add(object value)
		{
			if(_HeaderNode == null)
			{
				_HeaderNode = new ItemNode(value);
			}
			else
			{
				ItemNode item = HeaderNode;
				while(item.NextNode != null)
				{
					item = item.NextNode;
				}

				ItemNode newItem = new ItemNode(value);
				item.NextNode = newItem;
			}
			_Count++;
			return _Count;
		}
		
		#region IList Members

		public bool IsReadOnly
		{
			get
			{
				// TODO:  Add ItemLinkedList.IsReadOnly getter implementation
				return false;
			}
		}

		public object this[int index]
		{
			get
			{
				// TODO:  Add ItemLinkedList.this getter implementation
				return null;
			}
			set
			{
				// TODO:  Add ItemLinkedList.this setter implementation
			}
		}

		public void RemoveAt(int index)
		{
			// TODO:  Add ItemLinkedList.RemoveAt implementation
		}

		public void Insert(int index, object value)
		{
			// TODO:  Add ItemLinkedList.Insert implementation
		}

		public void Remove(object value)
		{
			// TODO:  Add ItemLinkedList.Remove implementation
		}

		public bool Contains(object value)
		{
			// TODO:  Add ItemLinkedList.Contains implementation
			return false;
		}

		public void Clear()
		{
			// TODO:  Add ItemLinkedList.Clear implementation
		}

		public int IndexOf(object value)
		{
			// TODO:  Add ItemLinkedList.IndexOf implementation
			return 0;
		}

		int System.Collections.IList.Add(object value)
		{
			return this.Add(value);
		}

		public bool IsFixedSize
		{
			get
			{
				// TODO:  Add ItemLinkedList.IsFixedSize getter implementation
				return false;
			}
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				// TODO:  Add ItemLinkedList.IsSynchronized getter implementation
				return false;
			}
		}

		public int Count
		{
			get
			{
				return _Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			// TODO:  Add ItemLinkedList.CopyTo implementation
		}

		public object SyncRoot
		{
			get
			{
				return null;
			}
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new ItemLinkedListEnumerator(this);
		}

		#endregion


		[Serializable]
		private class ItemLinkedListEnumerator : IEnumerator
		{
			#region Private Member Variables
			protected ItemLinkedList linkedList;
			protected ItemNode			startingNode;
			protected ItemNode			currentNode;
			#endregion

			#region Public Constructor
			public ItemLinkedListEnumerator(ItemLinkedList linkedList)
			{
				this.linkedList			= linkedList;
				startingNode			= new ItemNode(null);//linkedList.HeaderNode;
				startingNode.NextNode = linkedList.HeaderNode;
				currentNode				= startingNode;
				//currentNode.NextNode	= linkedList.HeaderNode;
			}			

			#endregion

			#region Public Member Properties
			public virtual object Current
			{
				get
				{
					return currentNode.CurrentValue;
				}
			}
			#endregion

			#region Public Member Methods
			public virtual void Reset()
			{
				currentNode				= startingNode;
			}

			public virtual bool MoveNext()
			{
				bool moveSuccessful = false;

				currentNode = currentNode.NextNode;

				if (currentNode!=null/* && currentNode != linkedList.HeaderNode*/)
					moveSuccessful = true;

				return moveSuccessful;
			}
			#endregion
		}

		private class ItemNode
		{
			private ItemNode _NextNode = null;
			private object _CurrentValue = null;

			public ItemNode NextNode
			{
				get
				{
					return _NextNode;
				}
				set
				{
					_NextNode = value;
				}
			}

			public object CurrentValue
			{
				get
				{
					return _CurrentValue;
				}
				set
				{
					_CurrentValue = value;
				}
			}

			public ItemNode(object val)
			{
				_CurrentValue = val;
			}
		}

	}
}
