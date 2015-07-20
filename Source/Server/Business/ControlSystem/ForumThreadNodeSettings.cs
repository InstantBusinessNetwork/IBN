using System;
using System.Collections;
using System.Data;

using Mediachase.IBN.Database;
using Mediachase.IBN.Database.ControlSystem;

namespace Mediachase.IBN.Business.ControlSystem
{
	#region Class ForumThreadNodeSetting
	public class ForumThreadNodeSetting
	{
		#region Consts

//		[Obsolete]
//		public const string External = "External";
//		[Obsolete]
//		public const string Private = "Private";

		public const string Incoming = "Incoming";
		public const string Outgoing = "Outgoing";
		public const string Internal = "Internal";

		public const string Question = "Question";
		public const string Resolution = "Resolution";
		public const string Workaround = "Workaround";
		public const string AllRecipients = "AllRecipients";
		#endregion

		private int m_SettingId;
		private string m_Name;
		private string m_Value;

		internal ForumThreadNodeSetting(int settingId, string name, string value)
		{
			m_SettingId = settingId;
			m_Name = name;
			m_Value = value;
		}

		internal int SettingId{ get{ return m_SettingId; } }
		
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
	#endregion

	#region Class ForumThreadNodeSettingCollection
	public class ForumThreadNodeSettingCollection : IEnumerable
	{
		private	int m_NodeId;
		private Hashtable m_settings = new Hashtable();

		public ForumThreadNodeSettingCollection(int NodeId)
		{
			m_NodeId = NodeId;
			SetSettings();
		}

		public string this[string Name]
		{
			get
			{
				string ret = null;
				ForumThreadNodeSetting setting = (ForumThreadNodeSetting)m_settings[Name];
				if(setting != null)
					ret = setting.Value;
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

		public bool Contains(string Name)
		{
			return m_settings.ContainsKey(Name);
		}

		public void Add(string Name, string Value)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				int id = DBForum.SetSetting(m_NodeId, Name, Value);

				ForumThreadNodeSetting setting = (ForumThreadNodeSetting)m_settings[Name];
				if(setting != null && setting.SettingId == id)
					setting.Value = Value;
				else
					m_settings[Name] = new ForumThreadNodeSetting(id, Name, Value);

				tran.Commit();
			}
		}

		public void Remove(string Name)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBForum.RemoveSetting(m_NodeId, Name);
				m_settings.Remove(Name);

				tran.Commit();
			}
		}

		private void SetSettings()
		{
			using(IDataReader reader = DBForum.GetSettings(m_NodeId))
			{
				if(reader != null)
				{
					while(reader.Read())
					{
						string name = (string)reader["Key"];
						m_settings[name] = new ForumThreadNodeSetting((int)reader["SettingId"], name, (string)reader["Value"]);
					}
				}
			}
		}

		#region Implementation of IEnumerable
		public System.Collections.IEnumerator GetEnumerator()
		{
			return m_settings.Values.GetEnumerator();
		}
		#endregion
	}
	#endregion
}
