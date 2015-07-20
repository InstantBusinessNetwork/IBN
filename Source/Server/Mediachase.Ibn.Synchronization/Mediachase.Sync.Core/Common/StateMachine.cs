using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Sync.Core;

namespace Mediachase.Sync.Core.Common
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class State<T>
	{
		public T stateName;
		public List<State<T>> AvailTransitions = new List<State<T>>();
		public delegate void StateAction(State<T> prevState);

		public StateAction Action;

		public State(T name)
		{
			stateName = name;
		}

		public State(T name, StateAction action)
		{
			stateName = name;
			Action = action;
		}

		/// <summary>
		/// Transitions to.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <returns></returns>
		public State<T> TransitionTo(State<T> state, string debugPrefix)
		{
			State<T> nextState = AvailTransitions.Find(x => state.stateName.Equals(x.stateName));
			if (nextState != null)
			{
				DebugAssistant.Log("SM " + debugPrefix + ": transition  " + stateName.ToString()
												   + "-> " + nextState.stateName.ToString());
				return nextState;
			}
			throw new Exception("SM" + debugPrefix + ": Unable to transition from state " + stateName.ToString()
								 + "to state " + state.stateName.ToString());
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StateMachine<T>
	{
		public string DebugPrefix { get; set; }
		public List<State<T>> RegisteredStates = new List<State<T>>();
		public State<T> CurrentState;

		/// <summary>
		/// Initializes a new instance of the <see cref="StateMachine&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="initialState">The initial state.</param>
		public StateMachine(State<T> initialState)
		{
			CurrentState = initialState;
		}

		/// <summary>
		/// Sets the state.
		/// </summary>
		/// <param name="stateName">Name of the state.</param>
		public void SetState(T stateName)
		{
			State<T> nextState = RegisteredStates.Find(x => x.stateName.Equals(stateName));
			State<T> prevState = CurrentState;

			if (nextState == null)
				throw new Exception("state with name " + stateName.ToString() + " not registered in SM");

			if (CurrentState == null)
			{
				CurrentState = nextState;
			}
			else
			{
				CurrentState = CurrentState.TransitionTo(nextState, DebugPrefix);
			}

			if (CurrentState != null && CurrentState.Action != null)
			{
				CurrentState.Action(prevState);
			}
		}

	}

}
