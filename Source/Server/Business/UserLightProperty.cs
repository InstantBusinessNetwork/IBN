using System;
using System.Collections;
using System.Data;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business
{
	public class UserLightProperty
	{
		private int m_PropertyID;
		private string m_Name;
		private string m_Value;

		internal UserLightProperty(int propID, string name, string value)
		{
			m_PropertyID = propID;
			m_Name = name;
			m_Value = value;
		}

		internal int PropertyID{ get{ return m_PropertyID; } }
		
		public string Name{ get{ return m_Name; } }

		public string Value
		{
			get
			{
				return m_Value;
			}
			set
			{
				m_Value = value;
			}
		}
	}

	public class UserLightPropertyCollection : IEnumerable
	{
		private	int m_UserID;
		private Hashtable m_properties =  Hashtable.Synchronized(new Hashtable());

		protected UserLightPropertyCollection(int UserID)
		{
			m_UserID = UserID;
			LoadProperties();
		}

		#region Cache
		private static Hashtable _userPropHash = Hashtable.Synchronized(new Hashtable(256));

		internal static UserLightPropertyCollection Load(int UserId)
		{
			if(!_userPropHash.ContainsKey(UserId))
			{
				// OZ 2009-02-16 Fix Item has already been added.  
				// New code structure with double check: Check, Lock, Check Again, Add, Unlock
				lock (_userPropHash.SyncRoot)
				{
					if (!_userPropHash.ContainsKey(UserId))
					{
						_userPropHash.Add(UserId, new UserLightPropertyCollection(UserId));
					}
				}
				//
			}

			return (UserLightPropertyCollection)_userPropHash[UserId];
		}
		#endregion


		public string this[string Name]
		{
			get
			{
				string ret = null;
				UserLightProperty prop = (UserLightProperty)m_properties[Name];
				if(prop != null)
					ret = prop.Value;
				return ret;
			}
			set
			{
				if(value != null)
					this.Add(Name, value);
				else
					this.Remove(Name);
			}
		}

		public void Add(string Name, string Value)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				int id = DBUser.SetProperty(m_UserID, Name, Value);

				UserLightProperty prop = (UserLightProperty)m_properties[Name];
				if(prop != null && prop.PropertyID == id)
					prop.Value = Value;
				else
					m_properties[Name] = new UserLightProperty(id, Name, Value);

				tran.Commit();
			}
		}

		public void Remove(string Name)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBUser.RemoveProperty(m_UserID, Name);
				m_properties.Remove(Name);

				tran.Commit();
			}
		}

		#region RemoveLike
		/// <summary>
		/// Removes the like. (dvs 2009-05-07)
		/// Using for delete control properties, when remove control from dashboard
		/// </summary>
		/// <param name="Name">The name.</param>
		public void RemoveLike(string Name)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBUser.RemovePropertyLike(m_UserID, Name);
				foreach (string key in m_properties.Keys)
				{
					if (key.Contains(Name))
						m_properties.Remove(key);
				}

				tran.Commit();
			}
		} 
		#endregion
		
		private void LoadProperties()
		{
			using(IDataReader reader = DBUser.GetProperties(m_UserID))
			{
				if(reader != null)
				{
					while(reader.Read())
					{
						string name = (string)reader["Key"];
						if(!m_properties.ContainsKey(name))
							m_properties[name] = new UserLightProperty((int)reader["SettingId"], name, (string)reader["Value"]);
					}
				}
			}
		}

		#region ClearInternalCache
		/// <summary>
		/// Clears the internal cache. (dvs - 2008-09-08)
		/// </summary>
		public static void ClearInternalCache()
		{
			_userPropHash.Clear();
		}
		#endregion

		#region Implementation of IEnumerable
		public System.Collections.IEnumerator GetEnumerator()
		{
			return m_properties.Values.GetEnumerator();
		}
		#endregion
	}
}
