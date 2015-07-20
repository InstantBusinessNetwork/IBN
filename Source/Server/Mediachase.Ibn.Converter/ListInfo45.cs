using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

using Mediachase.Database;
using MD45 = Mediachase.MetaDataPlus;
using MD47 = Mediachase.Ibn.Data;


namespace Mediachase.Ibn.Converter
{
	internal class ListInfo45
	{
		private static Dictionary<int, string> _listTypes = new Dictionary<int,string>();

		private int _listId;
		private int _folderId;
		private string _title;
		private string _description;
		private int _typeId;
		private int _statusId;
		private int _creatorId;
		private DateTime _creationDate;

		public static Dictionary<int, string> ListTypes
		{
			get
			{
				return _listTypes;
			}
		}

		public int ListId { get { return _listId; } }
		public int FolderId { get { return _folderId; } }
		public string Title { get { return _title; } }
		public string Description { get { return _description; } }
		public int TypeId { get { return _typeId; } }
		public int StatusId { get { return _statusId; } }
		public int CreatorId { get { return _creatorId; } }
		public DateTime CreationDate { get { return _creationDate; } }

		/// <summary>
		/// Loads the list types.
		/// </summary>
		static private void LoadListTypes(DBHelper source)
		{
			_listTypes.Clear();
			//if (_listTypes != null)
			//    throw new ArgumentException("Invalid call. LoadListTypes must be called once.");

			//_listTypes = new Dictionary<int, string>();
			using (IDataReader reader = source.RunTextDataReader("SELECT * FROM [LIST_TYPES]"))
			{
				while (reader.Read())
				{
					string listTypeName = (string)reader["TypeName"];
					int listTypeId = (int)reader["TypeId"];

					_listTypes.Add(listTypeId, listTypeName);
				}
			}

		}

		#region public LoadList()
		public static IDictionary<int, ListInfo45> LoadList(DBHelper source)
		{
			//Load all exists list types 
			LoadListTypes(source);

			Dictionary<int, ListInfo45> ret = new Dictionary<int, ListInfo45>();

			using (IDataReader reader = source.RunTextDataReader("SELECT * FROM [LISTS]"))
			{
				while (reader.Read())
				{
					ListInfo45 item = Load(reader);
					if (!ret.ContainsKey(item._listId))
						ret.Add(item._listId, item);
				}
			}

			return ret;
		}
		#endregion

		#region public Load(IDataRecord reader)
		public static ListInfo45 Load(IDataRecord reader)
		{
			ListInfo45 ret = new ListInfo45();

			ret._listId = (int)reader["ListId"];
			ret._folderId = (int)reader["FolderId"];
			ret._title = reader["Title"].ToString();
			ret._description = reader["Description"].ToString();
			ret._typeId = (int)reader["TypeId"];
			ret._statusId = (int)reader["StatusId"];
			ret._creatorId = (int)reader["CreatorId"];
			ret._creationDate = (DateTime)reader["CreationDate"];

			return ret;
		}
		#endregion
	}
}
