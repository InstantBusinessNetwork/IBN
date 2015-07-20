using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

using Mediachase.IBN.Database.Pop3;

namespace Mediachase.IBN.Business.Pop3
{
	/// <summary>
	/// Summary description for Pop3Parameters.
	/// </summary>
	[Serializable]
	public class Pop3Parameters : NameObjectCollectionBase
	{
		private bool _modified = false;

		private Pop3Box _ownerPop3Box = null;

		// Methods
		protected Pop3Parameters(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context):base(info,context)
		{
		}

		public Pop3Parameters(Pop3Box pop3Box)
		{
			_ownerPop3Box = pop3Box;

			using (IDataReader reader = DBPop3Box.GetParameterList(pop3Box.Id))
			{
				while (reader.Read())
				{
					string name = (string)reader["Name"];
					string value = (reader["Value"]==DBNull.Value) ? string.Empty : (string)reader["Value"];

					Add(name, value);
				}
			}

			_modified = false;
		}

		internal bool IsModified
		{
			get
			{
				return _modified;
			}
		}

		protected Pop3Box OwnerPop3Box
		{
			get
			{
				return _ownerPop3Box;
			}
		}

		public void Add(Pop3Parameters c)
		{
			this.InvalidateCachedArrays();
			int num1 = c.Count;
			for (int num2 = 0; num2 < num1; num2++)
			{
				string text1 = c.GetKey(num2);
				string[] textArray1 = c.GetValues(num2);
				if (textArray1 != null)
				{
					for (int num3 = 0; num3 < textArray1.Length; num3++)
					{
						this.Add(text1, textArray1[num3]);
					}
				}
				else
				{
					this.Add(text1, null);
				}
			}
		}

		public virtual void Add(string name, string value)
		{
			this.InvalidateCachedArrays();
			ArrayList list1 = (ArrayList) base.BaseGet(name);
			if (list1 == null)
			{
				list1 = new ArrayList(1);
				if (value != null)
				{
					list1.Add(value);
				}
				base.BaseAdd(name, list1);
			}
			else if (value != null)
			{
				list1.Add(value);
			}
			_modified = true;
		}

		public void Clear()
		{
			this.InternalClear();
			_modified = true;
		}

		public void CopyTo(Array dest, int index)
		{
			if (this._all == null)
			{
				int num1 = this.Count;
				this._all = new string[num1];
				for (int num2 = 0; num2 < num1; num2++)
				{
					this._all[num2] = this.Get(num2);
				}
			}
			this._all.CopyTo(dest, index);
		}

		public virtual string Get(int index)
		{
			ArrayList list1 = (ArrayList) base.BaseGet(index);
			return Pop3Parameters.GetAsOneString(list1);
		}

		public virtual string Get(string name)
		{
			ArrayList list1 = (ArrayList) base.BaseGet(name);
			return Pop3Parameters.GetAsOneString(list1);
		}

		private static string GetAsOneString(ArrayList list)
		{
			int num1 = (list != null) ? list.Count : 0;
			if (num1 == 1)
			{
				return (string) list[0];
			}
			if (num1 <= 1)
			{
				return null;
			}
			StringBuilder builder1 = new StringBuilder((string) list[0]);
			for (int num2 = 1; num2 < num1; num2++)
			{
				builder1.Append(',');
				builder1.Append((string) list[num2]);
			}
			return builder1.ToString();
		}

		private static string[] GetAsStringArray(ArrayList list)
		{
			int num1 = (list != null) ? list.Count : 0;
			if (num1 == 0)
			{
				return null;
			}
			string[] textArray1 = new string[num1];
			list.CopyTo(0, textArray1, 0, num1);
			return textArray1;
		}

		public virtual string GetKey(int index)
		{
			return base.BaseGetKey(index);
		}

		public virtual string[] GetValues(int index)
		{
			ArrayList list1 = (ArrayList) base.BaseGet(index);
			return Pop3Parameters.GetAsStringArray(list1);
		}

		public virtual string[] GetValues(string name)
		{
			ArrayList list1 = (ArrayList) base.BaseGet(name);
			return Pop3Parameters.GetAsStringArray(list1);
		}

		public bool HasKeys()
		{
			return this.InternalHasKeys();
		}

		internal virtual void InternalClear()
		{
			this.InvalidateCachedArrays();
			base.BaseClear();
		}

		internal virtual bool InternalHasKeys()
		{
			return base.BaseHasKeys();
		}

		protected void InvalidateCachedArrays()
		{
			this._all = null;
			this._allKeys = null;
		}

		public virtual void Remove(string name)
		{
			this.InvalidateCachedArrays();
			base.BaseRemove(name);
			_modified = true;
		}

		public virtual void Set(string name, string value)
		{
			this.InvalidateCachedArrays();
			ArrayList list1 = new ArrayList(1);
			list1.Add(value);
			base.BaseSet(name, list1);
			_modified = true;
		}


		// Properties
		public virtual string[] AllKeys
		{
			get
			{
				if (this._allKeys == null)
				{
					this._allKeys = base.BaseGetAllKeys();
				}
				return this._allKeys;
			}
		}

		public string this[int index]
		{
			get
			{
				return this.Get(index);
			}
		}

		public string this[string name]
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
			string value = this[name];
			return (value!=null);
		}

		// Fields
		private string[] _all;
		private string[] _allKeys;
	}
}
