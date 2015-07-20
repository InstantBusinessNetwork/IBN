using System;
using System.Collections.Specialized;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for ConfigBlockParameters.
	/// </summary>
	public class ConfigBlockParameters: NameObjectCollectionBase
	{
		public ConfigBlockParameters()
		{
		}

		// Methods
		protected ConfigBlockParameters(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context):base(info,context)
		{
		}

		public virtual void Add(string name, object value)
		{
			base.BaseAdd(name, value);
		}

		public void Clear()
		{
			this.BaseClear();
		}

		public virtual object Get(int index)
		{
			return this.BaseGet(index);
		}

		public virtual object Get(string name)
		{
			return this.BaseGet(name);
		}

		public virtual string GetKey(int index)
		{
			return base.BaseGetKey(index);
		}

		public bool HasKeys()
		{
			return this.BaseHasKeys();
		}

		public virtual void Remove(string name)
		{
			base.BaseRemove(name);
		}

		public virtual void Set(string name, object value)
		{
			base.BaseSet(name, value);
		}


		// Properties
		public virtual string[] AllKeys
		{
			get
			{
				return base.BaseGetAllKeys();
			}
		}

		public object this[int index]
		{
			get
			{
				return this.Get(index);
			}
		}

		public object this[string name]
		{
			get
			{
				return this.Get(name);
			}
			set
			{
				this.Set(name, value);
			}
		}

		public bool Contains(string name)
		{
			object Value = this[name];
			return (Value!=null);
		}

	}
}
