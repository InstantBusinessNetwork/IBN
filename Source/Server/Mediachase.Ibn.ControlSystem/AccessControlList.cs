using System;
using System.Collections;
using System.Data;

using Mediachase.Database;


namespace Mediachase.Ibn.ControlSystem
{
	/// <summary>
	/// Represents an Access Control Entry Collection.
	/// </summary>
	[Serializable]
	public class AccessControlList: CollectionBase
	{
		private int _id = 0;

		private bool _isInherited = false;
		private int _ownerDirectoryId = 0;

		private bool _isInheritedInitialValue = false;
		private bool _isChanged = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="AccessControlList"/> class.
		/// </summary>
		protected AccessControlList()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AccessControlList"/> class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		internal AccessControlList(IDataReader reader)
		{
			bool bFirstRun = true;

			while(reader.Read())
			{
				if(bFirstRun)
				{
					_id = (int)reader["AclId"];
					_isInherited = (bool)reader["AclIsInherited"];
					_isInheritedInitialValue = _isInherited;		
					_ownerDirectoryId = (int)reader["DirectoryId"];
					bFirstRun = false;
				}

				if(reader["AceId"]!=DBNull.Value)
				{
					this.Add(AccessControlEntry.InternalCreate((int)reader["AceId"],
						(bool)reader["IsInherited"],
						(string)SqlHelper.DBNull2Null(reader["role"]),
						(int)SqlHelper.DBNull2Null(reader["principalId"],0),
						(string)SqlHelper.DBNull2Null(reader["action"]),
						(bool)(((byte)SqlHelper.DBNull2Null(reader["allow"]))==1),
						false));
				}
			}

			_isInheritedInitialValue = _isInherited;

			Reset();
		}

		/// <summary>
		/// Gets the ACL.
		/// </summary>
		/// <param name="DirectoryId">The directory id.</param>
		/// <returns></returns>
		public static AccessControlList GetACL(int DirectoryId)
		{
			using(IDataReader reader = DBAccessControlList.GetAcl(DirectoryId))
			{
				AccessControlList retVal = new AccessControlList(reader);
				return retVal;
			}
		}

		/// <summary>
		/// Sets the ACL.
		/// </summary>
		/// <param name="control">The control.</param>
		/// <param name="acl">The acl.</param>
		/// <param name="ValidateACL">if set to <c>true</c> [validate ACL].</param>
		public static void SetACL(IIbnControl control, AccessControlList acl)
		{
			if(control==null)
				throw new ArgumentNullException("control");

			if(acl==null)
				throw new ArgumentNullException("acl");

			if(acl.OwnerDirectoryId == 0)
				throw new ArgumentException("You can not use a dettached ACL.","acl");

			using(DBTransaction tran = DBHelper2.DBHelper.BeginTransaction())
			{
				// Step 2. Update Inherited ACEs
				if(acl.IsInheritedChanged)
				{
					if(acl.IsInherited)
					{
						DBAccessControlList.TurnOnIsInherited(acl.Id);
					}
					else
					{
						DBAccessControlList.TurnOffIsInherited(acl.Id,false);
					}
				}

				// Step 3. Update Common ACEs
				if(acl.IsChanged)
				{
					DBAccessControlList.Clear(acl.Id);

					foreach(AccessControlEntry ace in acl)
					{
						if(!ace.IsIherited)
						{
							DBAccessControlList.AddAce(acl.Id,ace.Role,ace.PrincipalId,ace.Action,ace.Allow, false);

							if(ace.Allow)
							{
								foreach(string BaseAction in control.GetBaseActions(ace.Action))
								{
									DBAccessControlList.AddAce(acl.Id,ace.Role,ace.PrincipalId,BaseAction,ace.Allow, true);
								}
							}
							else
							{
								foreach(string BaseAction in control.GetDerivedActions(ace.Action))
								{
									DBAccessControlList.AddAce(acl.Id,ace.Role,ace.PrincipalId,BaseAction,ace.Allow, true);
								}
							}
						}
					}
				}

				// Step 4. Update child ACL
				DBAccessControlList.RefreshInheritedACL(acl.OwnerDirectoryId);

				tran.Commit();
			}
		}

		/// <summary>
		/// Creates the dettached ACL.
		/// </summary>
		/// <returns></returns>
		public static AccessControlList CreateDettachedACL()
		{
			return new AccessControlList();
		}

		/// <summary>
		/// Resets this instance.
		/// </summary>
		protected void Reset()
		{
			_isChanged = false;
		}

		/// <summary>
		/// Sets the changed.
		/// </summary>
		protected void SetChanged()
		{
			_isChanged = true;
		}

		/// <summary>
		/// Gets a value indicating whether this instance is changed.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is changed; otherwise, <c>false</c>.
		/// </value>
		public bool IsChanged
		{
			get
			{
				return _isChanged;
			}
		}

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		public int Id
		{
			get
			{
				return _id;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is inherited changed.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is inherited changed; otherwise, <c>false</c>.
		/// </value>
		public bool IsInheritedChanged
		{
			get
			{
				return _isInherited!=_isInheritedInitialValue;
			}
		}

		/// <summary>
		/// Gets the owner directory id.
		/// </summary>
		/// <value>The owner directory id.</value>
		public int OwnerDirectoryId
		{
			get
			{
				return _ownerDirectoryId;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is inherited.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is inherited; otherwise, <c>false</c>.
		/// </value>
		public bool IsInherited
		{
			get
			{
				return _isInherited;
			}
		}

		/// <summary>
		/// Turns the on is inherited.
		/// </summary>
		public void TurnOnIsInherited()
		{
			using(IDataReader fileReader = DBDirectory.GetById(this.OwnerDirectoryId))
			{
				if(fileReader.Read())
				{
					int ParentDirectoryId = (int)fileReader["ParentDirectoryId"];

					using(IDataReader aclReader = DBAccessControlList.GetAcl(ParentDirectoryId))
					{
						while(aclReader.Read())
						{
							this.Add(AccessControlEntry.InternalCreate((int)aclReader["AceId"],
								true,
								(string)SqlHelper.DBNull2Null(aclReader["role"]),
								(int)SqlHelper.DBNull2Null(aclReader["principalId"],0),
								(string)SqlHelper.DBNull2Null(aclReader["action"]),
								(bool)(((byte)SqlHelper.DBNull2Null(aclReader["allow"]))==1),
								false));
						}
					}
				}
			}

			_isInherited = true;
			SetChanged();
		}

		/// <summary>
		/// Turns the off is inherited.
		/// </summary>
		/// <param name="bCopyACL">if set to <c>true</c> [b copy ACL].</param>
		public void TurnOffIsInherited(bool bCopyACL)
		{
			for(int index = this.InnerList.Count -1;index>=0;index--)
			{
				if(this[index].IsIherited)
				{
					if(bCopyACL)
					{
						AccessControlEntry tmpAce = this[index];
						tmpAce.SetIsInherited(false);
						this.InnerList.RemoveAt(index);
						this.Add(tmpAce);
					}
					else
						this.InnerList.RemoveAt(index);
				}
			}

			_isInherited = false;
			SetChanged();
		}

		/// <summary>
		/// Adds the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public int Add(AccessControlEntry item)
		{
			// Check item originality
			if(!CheckAceOriginality(item))
				return this.List.Add(item);
			return -1;
		}

		/// <summary>
		/// Checks the ace originality.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public bool CheckAceOriginality(AccessControlEntry item)
		{
			foreach(AccessControlEntry ace in this)
			{
				if(ace.IsIherited==item.IsIherited&&
					!ace.IsInternal&&
					ace.PrincipalId==item.PrincipalId&&
					ace.Role==item.Role&&
					ace.Action==item.Action&&
					ace.Allow==item.Allow)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Removes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Remove(AccessControlEntry item)
		{
			this.List.Remove(item);
		}

		/// <summary>
		/// Removes the specified aceid.
		/// </summary>
		/// <param name="aceid">The aceid.</param>
		public void Remove(int aceid)
		{
			foreach(AccessControlEntry item in this)
			{
				if(item.Id==aceid)
				{
					this.List.Remove(item);
					break;
				}
			}
		}

		/// <summary>
		/// Determines whether [contains] [the specified item].
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(AccessControlEntry item)
		{
			return this.List.Contains(item);
		}

		/// <summary>
		/// Gets the <see cref="AccessControlEntry"/> at the specified index.
		/// </summary>
		/// <value></value>
		public AccessControlEntry this[int index]
		{
			get
			{
				return (AccessControlEntry)this.List[index];
			}
		}
	
		/// <summary>
		/// Performs additional custom processes after inserting a
		/// new element into the <see cref="T:System.Collections.CollectionBase"/> instance.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnInsertComplete(int index, object value)
		{
			SetChanged();
			base.OnInsertComplete (index, value);
		}
	
		/// <summary>
		/// Performs additional custom processes after removing an
		/// element from the <see cref="T:System.Collections.CollectionBase"/> instance.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="value"/> can be found.</param>
		/// <param name="value">The value of the element to remove from <paramref name="index"/>.</param>
		protected override void OnRemoveComplete(int index, object value)
		{
			SetChanged();
			base.OnRemoveComplete (index, value);
		}

	
		/// <summary>
		/// Performs additional custom processes when removing an element from the
		/// <see cref="T:System.Collections.CollectionBase"/> instance.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="value"/> can be found.</param>
		/// <param name="value">The value of the element to remove from <paramref name="index"/>.</param>
		protected override void OnRemove(int index, object value)
		{
			if(((AccessControlEntry)value).IsIherited)
				throw new Exception("Iherited Access Control Entry is readonly. Use TurnOffIsInherited or TurnOnIsInherited methods to modify the iherited ACE.");

			base.OnRemove (index, value);
		}
	}
}
