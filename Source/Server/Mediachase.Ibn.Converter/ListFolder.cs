using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

using Mediachase.Database;


namespace Mediachase.Ibn.Converter
{
	internal class ListFolder
	{
		// 4.5 fields
		private int _listFolderId;
		private string _name;
		private int _parentFolderId;
		private int? _projectId;
		private bool _isPrivate;
		private int _creatorId;
		private DateTime _creationDate;

		// 4.7 fields
		private int _outlineLevel;
		private string _outlineNumber;
		private bool _hasChildren;

		#region public LoadList()
		public static IDictionary<int, ListFolder> LoadList(DBHelper source)
		{
			Dictionary<int, ListFolder> ret = new Dictionary<int, ListFolder>();

			using (IDataReader reader = source.RunTextDataReader("SELECT * FROM [LIST_FOLDERS]"))
			{
				while (reader.Read())
				{
					ListFolder item = Load(reader);
					if (!ret.ContainsKey(item._listFolderId))
						ret.Add(item._listFolderId, item);
				}
			}

			// Calculate OutlineLevel, OutlineNumber, HasChildren.
			foreach (ListFolder folder in ret.Values)
				folder.CalculateOutline(ret);

			return ret;
		}
		#endregion

		#region public Load(IDataRecord reader)
		public static ListFolder Load(IDataRecord reader)
		{
			ListFolder ret = new ListFolder();

			ret._listFolderId = (int)reader["ListFolderId"];
			ret._name = reader["Name"].ToString();
			ret._parentFolderId = (int)reader["ParentFolderId"];
			ret._projectId = Helper.NullToNulableInt32(reader["ProjectId"]);
			ret._isPrivate = (bool)reader["IsPrivate"];
			ret._creatorId = Helper.NullToInt32(reader["CreatorId"]);
			ret._creationDate = (DateTime)reader["CreationDate"];

			return ret;
		}
		#endregion

		#region public static void SaveList(DBHelper target, IDictionary<int, ListFolder> folders)
		public static void SaveList(DBHelper target, IDictionary<int, ListFolder> folders)
		{
			List<ListFolder> list = new List<ListFolder>(folders.Values);
			list.Sort(Compare);

			const string targetTableName = "cls_ListFolder";

			IbnConverter.SetInsertIdentity(target, targetTableName, true);

			foreach (ListFolder folder in list)
			{
				folder.Save(target);
			}

			IbnConverter.SetInsertIdentity(target, targetTableName, false);
		}
		#endregion

		#region public void Save(DBHelper target)
		public void Save(DBHelper target)
		{
			target.RunText("INSERT INTO [cls_ListFolder] ([ListFolderId],[Title],[Created],[Modified],[CreatorId],[ModifierId],[OutlineLevel],[OutlineNumber],[HasChildren],[ParentId],[ProjectId]) VALUES (@p1,@p2,@p3,@p3,@p4,@p4,@p5,@p6,@p7,@p8,@p9)"
				, DBHelper.MP("@p1", SqlDbType.Int, _listFolderId)
				, DBHelper.MP("@p2", SqlDbType.NVarChar, 100, _name)
				, DBHelper.MP("@p3", SqlDbType.DateTime, _creationDate)
				, DBHelper.MP("@p4", SqlDbType.Int, _creatorId)
				, DBHelper.MP("@p5", SqlDbType.Int, _outlineLevel)
				, DBHelper.MP("@p6", SqlDbType.NVarChar, 250, _outlineNumber)
				, DBHelper.MP("@p7", SqlDbType.Bit, _hasChildren)
				, DBHelper.MP("@p8", SqlDbType.Int, _parentFolderId == 0 ? DBNull.Value : (object)_parentFolderId)
				, DBHelper.MP("@p9", SqlDbType.Int, _projectId == null ? DBNull.Value : (object)_projectId)
				);

			if (_isPrivate)
			{
				target.RunText("INSERT INTO [cls_ListFolder_Role_Principal] ([ObjectId],[PrincipalId],[RoleId]) VALUES (@p1,@p2,@p3)"
					, DBHelper.MP("@p1", SqlDbType.Int, _listFolderId)
					, DBHelper.MP("@p2", SqlDbType.Int, _creatorId)
					, DBHelper.MP("@p3", SqlDbType.Int, 1) // 1 = Owner
					);
			}
		}
		#endregion


		#region private void CalculateOutline(IDictionary<int, ListFolder> folders)
		private void CalculateOutline(IDictionary<int, ListFolder> folders)
		{
			if (_outlineLevel < 1)
			{
				if (_parentFolderId > 0)
				{
					ListFolder parent = folders[_parentFolderId];
					parent.CalculateOutline(folders);

					parent._hasChildren = true;
					_outlineLevel = parent._outlineLevel + 1;
					_outlineNumber = parent._outlineNumber + ".";
				}
				else
				{
					_outlineLevel = 1;
					_outlineNumber = string.Empty;
				}
				_outlineNumber += _listFolderId.ToString(CultureInfo.InvariantCulture);
			}
		}
		#endregion

		#region private static int Compare(ListFolder x, ListFolder y)
		private static int Compare(ListFolder x, ListFolder y)
		{
			int retVal = 0;

			if (x == null)
			{
				if (y != null)
					retVal = -1;
			}
			else
			{
				if (y == null)
					retVal = 1;
				else
				{
					retVal = x._parentFolderId.CompareTo(y._parentFolderId);
					if (retVal == 0)
						retVal = x._listFolderId.CompareTo(y._listFolderId);
				}
			}

			return retVal;
		}
		#endregion
	}
}
