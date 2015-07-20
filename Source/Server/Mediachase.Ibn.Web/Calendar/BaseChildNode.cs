//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------

namespace Mediachase.Web.UI.WebControls
{
	using System;
	using System.Web.UI;
	using System.Reflection;
	using System.ComponentModel;

	/// <summary>
	/// Base class for child nodes of parent controls.
	/// </summary>
	[DefaultProperty("ID")]
	public abstract class BaseChildNode : Control, INamingContainer, ICloneable, IStateManager
	{
		private bool _IsTrackingViewState = false;
		private StateBag _ViewState;
        
		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns>A String that represents the current Object.</returns>
		public override string ToString()
		{
			if (ID != String.Empty)
			{
				return ID;
			}
			else
			{
				return this.GetType().Name;
			}
		}

		/// <summary>
		/// Returns the parent object.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public new abstract object Parent
		{ 
			get;
		}

		/// <summary>
		/// Adds attributes to the HtmlTextWriter.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		protected virtual void AddAttributesToRender(HtmlTextWriter writer)
		{
			if (ID != String.Empty)
			{
				writer.AddAttribute("ID", ID);
			}
		}

		/// <summary>
		/// Writes attributes to the HtmlTextWriter.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		protected virtual void WriteAttributes(HtmlTextWriter writer)
		{
			if (ID != String.Empty)
			{
				writer.WriteAttribute("ID", ID);
			}
		}

		/// <summary>
		/// Implement the UpLevel rendering path in this method.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		protected abstract void RenderUpLevelPath(HtmlTextWriter writer);

		/// <summary>
		/// Implement the DownLevel rendering path in this method.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		protected abstract void RenderDownLevelPath(HtmlTextWriter writer);

		/// <summary>
		/// Implement the Designer rendering path in this method.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		protected abstract void RenderDesignerPath(HtmlTextWriter writer);

		/// <summary>
		/// Outputs content to a provided HtmlTextWriter output stream.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the control content.</param>
		/// <param name="pathID">The ID (from BaseRichControl) of the rendering path to use.</param>
		          public virtual void Render(HtmlTextWriter writer, RenderPathID pathID)
		{
			switch (pathID)
			{
				case RenderPathID.DownLevelPath:
					RenderDownLevelPath(writer);
					break;

				case RenderPathID.UpLevelPath:
					RenderUpLevelPath(writer);
					break;

				case RenderPathID.DesignerPath:
					RenderDesignerPath(writer);
					break;
			}
		}

		/// <summary>
		/// The ID of the node.
		/// </summary>
		[DefaultValue("")]
		[ParenthesizePropertyName(true)]
		[ResDescription("BaseID")]
		public override string ID
		{
			get
			{
				object obj = ViewState["ID"];
				return (obj == null) ? String.Empty : (string)obj;
			}

			set { ViewState["ID"] = value; }
		}

		/// <summary>
		/// Sets all items within the StateBag to be dirty
		/// </summary>
		          public virtual void SetViewStateDirty()
		{
			if (_ViewState != null)
			{
				foreach (StateItem item in ViewState.Values)
				{
					item.IsDirty = true;
				}
			}
		}

		/// <summary>
		/// Sets all items within the StateBag to be clean
		/// </summary>
		          public virtual void SetViewStateClean()
		{
			if (_ViewState != null)
			{
				foreach (StateItem item in ViewState.Values)
				{
					item.IsDirty = false;
				}
			}
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public virtual object Clone()
		{
			BaseChildNode copy = (BaseChildNode)Activator.CreateInstance(this.GetType(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);

			// Merge in the properties from this object into the copy
			copy._IsTrackingViewState = this._IsTrackingViewState;

			if (this._ViewState != null)
			{
				StateBag viewState = copy.ViewState;
				foreach (string key in this.ViewState.Keys)
				{
					object item = this.ViewState[key];
					if (item is ICloneable)
					{
						item = ((ICloneable)item).Clone();
					}

					viewState[key] = item;
				}
			}

			return copy;
		}

		/// <summary>
		/// An instance of the StateBag class that contains the view state information.
		/// </summary>
		protected override StateBag ViewState
		{
			get
			{
				// To concerve resources, especially on the page,
				// only create the view state when needed.
				if (_ViewState == null)
				{
					_ViewState = new StateBag();
					if (((IStateManager)this).IsTrackingViewState)
					{
						((IStateManager)_ViewState).TrackViewState();
					}
				}

				return _ViewState;
			}
		}

		/// <summary>
		/// Loads the node's previously saved view state.
		/// </summary>
		/// <param name="state">An Object that contains the saved view state values for the node.</param>
		void IStateManager.LoadViewState(object state)
		{
			((BaseChildNode)this).LoadViewState(state);
		}

		/// <summary>
		/// Loads the node's previously saved view state.
		/// </summary>
		/// <param name="state">An Object that contains the saved view state values for the node.</param>
		protected override void LoadViewState(object state)
		{
			if (state != null)
			{
				((IStateManager)ViewState).LoadViewState(state);
			}
		}

		/// <summary>
		/// Saves the changes to the node's view state to an Object.
		/// </summary>
		/// <returns>The Object that contains the view state changes.</returns>
		object IStateManager.SaveViewState()
		{
			return ((BaseChildNode)this).SaveViewState();
		}

		/// <summary>
		/// Saves the changes to the node's view state to an Object.
		/// </summary>
		/// <returns>The Object that contains the view state changes.</returns>
		protected override object SaveViewState()
		{
			if (_ViewState != null)
			{
				return ((IStateManager)_ViewState).SaveViewState();
			}

			return null;
		}

		/// <summary>
		/// Instructs the node to track changes to its view state.
		/// </summary>
		void IStateManager.TrackViewState()
		{
			((BaseChildNode)this).TrackViewState();
		}

		/// <summary>
		/// Instructs the node to track changes to its view state.
		/// </summary>
		protected override void TrackViewState()
		{
			_IsTrackingViewState = true;

			if (_ViewState != null)
			{
				((IStateManager)_ViewState).TrackViewState();
			}
		}

		/// <summary>
		/// Gets a value indicating whether a server control is tracking its view state changes.
		/// </summary>
		bool IStateManager.IsTrackingViewState
		{
			get { return _IsTrackingViewState; }
		}
	}
}
