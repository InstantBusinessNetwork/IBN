//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------

namespace Mediachase.Web.UI.WebControls
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Web.UI;
	using System.ComponentModel;
	using System.Drawing.Design;

	/// <summary>
	/// Base class for collections of BaseChildNode objects.
	/// </summary>
	[Editor(typeof(Mediachase.Web.UI.WebControls.Design.ItemCollectionEditor), typeof(UITypeEditor))]
	public abstract class BaseChildNodeCollection : CollectionBase, ICloneable, IStateManager
	{
		private bool _Tracking = false;
		private bool _Reloading = false;
		private ArrayList _Actions = new ArrayList(4);

		/// <summary>
		/// Creates a new deep copy of the current collection.
		/// </summary>
		/// <returns>A new object that is a deep copy of this instance.</returns>
		public virtual object Clone()
		{
			BaseChildNodeCollection copy = (BaseChildNodeCollection)Activator.CreateInstance(this.GetType(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);

			foreach (BaseChildNode node in List)
			{
				copy.List.Add(node.Clone());
			}

			copy._Tracking = this._Tracking;
			copy._Reloading = this._Reloading;
			copy._Actions.Clear();
			foreach (Action action in this._Actions)
			{
				copy._Actions.Add(action.Clone());
			}

			return copy;
		}

		/// <summary>
		/// List of actions made to this collection. Used for state tracking.
		/// </summary>
		private ArrayList Actions
		{
			get { return _Actions; }
		}

		/// <summary>
		/// Type of action made to this collection.
		/// </summary>
		private enum ActionType { Clear, Insert, Remove };

		/// <summary>
		/// Stores information about an action made to this collection
		/// </summary>
		private class Action
		{
			/// <summary>
			/// The type of action that this object represents (Clear, Insert, or Remove)
			/// </summary>
			public ActionType ActionType;

			/// <summary>
			/// The index at which this action occurred.
			/// </summary>
			public int Index;

			private string nodeType = String.Empty;
			private int nodeTypeIndex = -1;
			private static string[] knownTypes =
			{
				typeof(Mediachase.Web.UI.WebControls.CalendarItem).AssemblyQualifiedName,
			};

			/// <summary>
			/// The type name of the node that is being inserted.
			/// </summary>
			public string NodeType
			{
				get { return nodeType; }
				set
				{
					nodeType = value;
					int index = System.Array.IndexOf(knownTypes, nodeType);
					if (index >= 0)
					{
						nodeTypeIndex = index;
					}
				}
			}

			/// <summary>
			/// Loads an Action from ViewState.
			/// </summary>
			/// <param name="stateObj">The state object.</param>
			public void Load(object stateObj)
			{
				if (stateObj != null)
				{
					object[] state = (object[])stateObj;
					ActionType = (ActionType)state[0];

					switch (ActionType)
					{
						case ActionType.Insert:
							Index = (int)state[1];
							if (state[2] is string)
							{
								nodeType = (string)state[2];
							}
							else
							{
								// Load from an index
								nodeTypeIndex = (int)state[2];
								nodeType = (string)knownTypes[nodeTypeIndex];
							}
							break;

						case ActionType.Remove:
							Index = (int)state[1];
							break;
					}
				}
			}

			/// <summary>
			/// Saves an Action to ViewState.
			/// </summary>
			/// <returns>The state object.</returns>
			public object Save()
			{
				object[] state;
				switch (ActionType)
				{
					case ActionType.Insert:
						state = new object[3];
						state[1] = Index;
						state[2] = (nodeTypeIndex >= 0) ? (object)nodeTypeIndex : (object)nodeType;
						break;

					case ActionType.Remove:
						state = new object[2];
						state[1] = Index;
						break;

					default:
						state = new object[1];
						break;
				}
				state[0] = ActionType;

				return state;
			}

			/// <summary>
			/// Clones an Action object.
			/// </summary>
			/// <returns>The copy.</returns>
			public Action Clone()
			{
				Action action = new Action();
				action.ActionType = this.ActionType;
				action.Index = this.Index;
				action.nodeType = this.nodeType;
				action.nodeTypeIndex = this.nodeTypeIndex;
				return action;
			}
		}

		/// <summary>
		/// Records the Clear action when clearing the contents of the collection.
		/// </summary>
		protected override void OnClear()
		{
			bool needClear = (Count > 0);
			base.OnClear();

			if (needClear && ((IStateManager)this).IsTrackingViewState)
			{
				// Since the entire list is cleared, all prior
				// actions do not need to be saved
				Actions.Clear();

				Action action = new Action();
				action.ActionType = ActionType.Clear;

				Actions.Add(action);
			}
		}

		/// <summary>
		/// Sets the entire view state for the collection and everything under it to dirty
		/// </summary>
		protected virtual void SetViewStateDirty()
		{
			if (!((IStateManager)this).IsTrackingViewState)
			{
				((IStateManager)this).TrackViewState();
			}

			Actions.Clear();
			Action action = new Action();
			action.ActionType = ActionType.Clear;
			Actions.Add(action);
			for (int index = 0; index < Count; index++)
			{
				BaseChildNode item = (BaseChildNode)List[index];

				action = new Action();
				action.ActionType = ActionType.Insert;
				action.Index = index;
				action.NodeType = item.GetType().FullName;

				Actions.Add(action);

				item.SetViewStateDirty();
			}
		}

		/// <summary>
		/// Records the Remove action when removing a node from the collection.
		/// </summary>
		/// <param name="index">The zero-based index at which value can be found.</param>
		/// <param name="value">The value of the element to remove from index.</param>
		protected override void OnRemove(int index, object value)
		{
			base.OnRemove(index, value);

			if (((IStateManager)this).IsTrackingViewState)
			{
				Action action = new Action();
				action.ActionType = ActionType.Remove;
				action.Index = index;

				Actions.Add(action);
			}
		}

		/// <summary>
		/// Records the Insert action when adding a node to the collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, object value)
		{
			base.OnInsert(index, value);

			if (((IStateManager)this).IsTrackingViewState)
			{
				Action action = new Action();
				action.ActionType = ActionType.Insert;
				action.Index = index;
				action.NodeType = value.GetType().AssemblyQualifiedName;

				Actions.Add(action);

				// The new node needs to start tracking its view state
				((IStateManager)value).TrackViewState();
				((BaseChildNode)value).SetViewStateDirty();
			}
		}

		/// <summary>
		/// Records the Set action when a node is replaced in the collection.
		/// </summary>
		/// <param name="index">The zero-based index at which oldValue can be found.</param>
		/// <param name="oldValue">The value to replace with newValue.</param>
		/// <param name="newValue">The new value of the element at index.</param>
		protected override void OnSet(int index, object oldValue, object newValue)
		{
			base.OnSet(index, oldValue, newValue);

			if (((IStateManager)this).IsTrackingViewState && 
				!oldValue.GetType().Equals(newValue.GetType()))
			{
				Action action = new Action();
				action.ActionType = ActionType.Remove;
				action.Index = index;

				Actions.Add(action);

				action = new Action();
				action.ActionType = ActionType.Insert;
				action.Index = index;
				action.NodeType = newValue.GetType().FullName;

				Actions.Add(action);

				// The new node needs to start tracking its view state
				((IStateManager)newValue).TrackViewState();
				((BaseChildNode)newValue).SetViewStateDirty();
			}
		}

		/// <summary>
		/// Recreates a node of type nodeTypeName and inserts it into the index location.
		/// </summary>
		/// <param name="index">The zero-based index at which to insert the node.</param>
		/// <param name="nodeTypeName">The name of the type of the node.</param>
		private void RedoInsert(int index, String nodeTypeName)
		{
			try
			{
				// Create a Type object from the type name
				Type nodeType = Type.GetType(nodeTypeName);

				if (nodeType != null)
				{
					// Invoke the constructor, creating the object
					object node = Activator.CreateInstance(nodeType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);

					// Insert the node if creation was successful
					if ((node != null) && (node is BaseChildNode))
					{
						List.Insert(index, node);
					}
				}
			}
			catch
			{
				// Ignore errors
			}
		}

		/// <summary>
		/// Gets a value indicating whether a server control is tracking its view state changes.
		/// </summary>
		bool IStateManager.IsTrackingViewState
		{
			get { return _Tracking; }
		}

		/// <summary>
		/// Indicates whether the collection is reloading its items
		/// </summary>
		public bool Reloading
		{
			get { return _Reloading; }
		}

		/// <summary>
		/// Loads the collection's previously saved view state.
		/// </summary>
		/// <param name="state">An Object that contains the saved view state values for the collection.</param>
		void IStateManager.LoadViewState(object state)
		{
			if (state == null)
			{
				return;
			}

			object[] viewState = (object[])state;

			if (viewState[0] != null)
			{
				_Reloading = true;

				// Restore and re-do actions
				object[] actions = (object[])viewState[0];

				ArrayList newActionList = new ArrayList();
				foreach (object actionState in actions)
				{
					Action action = new Action();
					action.Load(actionState);

					newActionList.Add(action);

					switch (action.ActionType)
					{
						case ActionType.Clear:
							List.Clear();
							break;

						case ActionType.Remove:
							List.RemoveAt(action.Index);
							break;

						case ActionType.Insert:
							RedoInsert(action.Index, action.NodeType);
							break;
					}
				}

				_Actions = newActionList;
				_Reloading = false;
			}

			if (viewState[1] != null)
			{
				// Load view state changes

				object[] lists = (object[])viewState[1];
				ArrayList indices = (ArrayList)lists[0];
				ArrayList items = (ArrayList)lists[1];

				for (int i = 0; i < indices.Count; i++)
				{
					((IStateManager)List[(int)indices[i]]).LoadViewState(items[i]);
				}
			}
		}

		/// <summary>
		/// Saves the changes to the collection's view state to an Object.
		/// </summary>
		/// <returns>The Object that contains the view state changes.</returns>
		object IStateManager.SaveViewState()
		{
			object[] state = new object[2];

			if (Actions.Count > 0)
			{
				// Save the actions made to the collection
				object[] obj = new object[Actions.Count];
				for (int i = 0; i < Actions.Count; i++)
				{
					obj[i] = ((Action)Actions[i]).Save();
				}
				state[0] = obj;
			}

			if (List.Count > 0)
			{
				// Save the view state changes of the child nodes

				ArrayList indices = new ArrayList(4);
				ArrayList items = new ArrayList(4);

				for (int i = 0; i < List.Count; i++)
				{
					object item = ((IStateManager)List[i]).SaveViewState();
					if (item != null)
					{
						indices.Add(i);
						items.Add(item);
					}
				}

				if (indices.Count > 0)
				{
					state[1] = new object[] { indices, items };
				}
			}

			if ((state[0] != null) || (state[1] != null))
			{
				return state;
			}
            
			return null;
		}

		/// <summary>
		/// Instructs the collection to track changes to its view state.
		/// </summary>
		void IStateManager.TrackViewState()
		{
			_Tracking = true;

			// Tracks view state changes in the child nodes
			foreach (BaseChildNode node in List)
			{
				((IStateManager)node).TrackViewState();
			}
		}
	}
}
