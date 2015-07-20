using System;
using System.Collections;

namespace Mediachase.IBN.Business.Pop3
{
	/// <summary>
	/// Summary description for Pop3MessageHandlerInfoList.
	/// </summary>
	[Serializable]
	public class Pop3MessageHandlerInfoList : CollectionBase
	{
		private bool _modified = false;
		/// <summary>
		/// Initializes a new instance of the <see cref="Pop3MessageHandlerInfoList"/> class.
		/// </summary>
		public Pop3MessageHandlerInfoList()
		{
		}

		internal void ResetModified()
		{
			_modified = false;
		}

		/// <summary>
		/// Adds the specified info.
		/// </summary>
		/// <param name="info">The info.</param>
		public int Add(Pop3MessageHandlerInfo info)
		{
			return this.List.Add(info);
		}

		public int Add(string pop3HandlerName)
		{
			return this.List.Add(Pop3Manager.Current.Config.Handlers[pop3HandlerName]);
		}

		/// <summary>
		/// Removes the specified info.
		/// </summary>
		/// <param name="info">The info.</param>
		public void Remove(Pop3MessageHandlerInfo info)
		{
			this.List.Remove(info);
		}

		/// <summary>
		/// Removes the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		public void Remove(string name)
		{
			Pop3MessageHandlerInfo info = this[name];
			if(info!=null)
				this.Remove(info);
		}

		/// <summary>
		/// Indexes the of.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <returns></returns>
		public int IndexOf(Pop3MessageHandlerInfo info)
		{
			return this.List.IndexOf(info);
		}

		/// <summary>
		/// Gets the <see cref="Pop3MessageHandlerInfo"/> at the specified index.
		/// </summary>
		/// <value></value>
		public Pop3MessageHandlerInfo this[int Index]
		{
			get
			{
				return (Pop3MessageHandlerInfo)base.InnerList[Index];
			}
		}

		/// <summary>
		/// Gets the <see cref="Pop3MessageHandlerInfo"/> with the specified name.
		/// </summary>
		/// <value></value>
		public Pop3MessageHandlerInfo this[string Name]
		{
			get
			{
				foreach(Pop3MessageHandlerInfo item in this)
					if(item.Name==Name)
                        return item;
				return null;
			}
		}

		/// <summary>
		/// Determines whether [contains] [the specified name].
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified name]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(string Name)
		{
			return this[Name]!=null;
		}

		internal bool IsModified
		{
			get
			{
				return _modified;
			}
		}
	
		protected override void OnRemoveComplete(int index, object value)
		{
			_modified = true;
		}
	
		protected override void OnClearComplete()
		{
			_modified = true;
		}
	
		protected override void OnInsertComplete(int index, object value)
		{
			_modified = true;
		}
	
		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			_modified = true;
		}
	}
}

