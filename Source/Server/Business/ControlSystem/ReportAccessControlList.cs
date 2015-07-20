using System;
using System.Collections;
using System.Data;
using Mediachase.IBN.Database.ControlSystem;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Represents a Report Access Control Entry Collection.
	/// </summary>
	[Serializable]
	public class ReportAccessControList: CollectionBase
	{
		private int _id = 0;

		private int _ownerReportId = 0;

		private bool _isChanged = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportAccessControList"/> class.
		/// </summary>
		protected ReportAccessControList()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportAccessControList"/> class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		internal ReportAccessControList(IDataReader reader)
		{
			bool bFirstRun = true;

			while(reader.Read())
			{
				if(bFirstRun)
				{
					_id = (int)reader["AclId"];
					_ownerReportId = (int)reader["ReportId"];
					bFirstRun = false;
				}

				if(reader["AceId"]!=DBNull.Value)
				{
					this.Add(ReportAccessControlEntry.InternalCreate((int)reader["AceId"],
						(string)SqlHelper.DBNull2Null(reader["role"]),
						(int)SqlHelper.DBNull2Null(reader["principalId"],0),
						(string)SqlHelper.DBNull2Null(reader["action"]),
						(bool)SqlHelper.DBNull2Null(reader["allow"]),
						false));
				}
			}

			Reset();
		}

		/// <summary>
		/// Gets the ACL.
		/// </summary>
		/// <param name="ReportId">The directory id.</param>
		/// <returns></returns>
		public static ReportAccessControList GetACL(int ReportId)
		{
			using(IDataReader reader = DBReportAccessControList.GetAcl(ReportId))
			{
				ReportAccessControList retVal = new ReportAccessControList(reader);
				return retVal;
			}
		}

		/// <summary>
		/// Sets the ACL.
		/// </summary>
		/// <param name="control">The control.</param>
		/// <param name="acl">The acl.</param>
		/// <param name="ValidateACL">if set to <c>true</c> [validate ACL].</param>
		public static void SetACL(IIbnControl control, ReportAccessControList acl)
		{
			if(control==null)
				throw new ArgumentNullException("control");

			if(acl==null)
				throw new ArgumentNullException("acl");

			if(acl.OwnerReportId == 0)
				throw new ArgumentException("You can not use a dettached ACL.","acl");

			// Validation 1 - 2
//			if(ValidateACL)
//			{
//				if(acl.Count==0)
//					throw new AllUserAccessWillBeDeniedException();
//			}

			using(Mediachase.IBN.Database.DbTransaction tran = Mediachase.IBN.Database.DbTransaction.Begin())
			{
				// Step 3. Update Common ACEs
				if(acl.IsChanged)
				{
					DBReportAccessControList.Clear(acl.Id);

					foreach(ReportAccessControlEntry ace in acl)
					{
						DBReportAccessControList.AddAce(acl.Id,ace.Role,ace.PrincipalId,ace.Action,ace.Allow, false);

						if(ace.Allow)
						{
							foreach(string BaseAction in control.GetBaseActions(ace.Action))
							{
								DBReportAccessControList.AddAce(acl.Id,ace.Role,ace.PrincipalId,BaseAction,ace.Allow, true);
							}
						}
						else
						{
							foreach(string BaseAction in control.GetDerivedActions(ace.Action))
							{
								DBReportAccessControList.AddAce(acl.Id,ace.Role,ace.PrincipalId,BaseAction,ace.Allow, true);
							}
						}
					}
				}

				// Validation 2 - 2
//				if(ValidateACL)
//				{
//					if(!DBFileStorage.CanUserRunAction(Security.CurrentUser.UserID, control.OwnerContainer.Key , acl.OwnerReportId, "Admin"))
//						throw new AdminAccessWillBeDeniedException();
//				}

				tran.Commit();
			}
		}

		/// <summary>
		/// Creates the dettached ACL.
		/// </summary>
		/// <returns></returns>
		public static ReportAccessControList CreateDettachedACL()
		{
			return new ReportAccessControList();
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
		/// Gets the owner directory id.
		/// </summary>
		/// <value>The owner directory id.</value>
		public int OwnerReportId
		{
			get
			{
				return _ownerReportId;
			}
		}

		/// <summary>
		/// Adds the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public int Add(ReportAccessControlEntry item)
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
		public bool CheckAceOriginality(ReportAccessControlEntry item)
		{
			foreach(ReportAccessControlEntry ace in this)
			{
				if(!ace.IsInternal&&
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
		public void Remove(ReportAccessControlEntry item)
		{
			this.List.Remove(item);
		}

		/// <summary>
		/// Removes the specified aceid.
		/// </summary>
		/// <param name="aceid">The aceid.</param>
		public void Remove(int aceid)
		{
			foreach(ReportAccessControlEntry item in this)
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
		public bool Contains(ReportAccessControlEntry item)
		{
			return this.List.Contains(item);
		}

		/// <summary>
		/// Gets the <see cref="ReportAccessControlEntry"/> at the specified index.
		/// </summary>
		/// <value></value>
		public ReportAccessControlEntry this[int index]
		{
			get
			{
				return (ReportAccessControlEntry)this.List[index];
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
			base.OnRemove (index, value);
		}
	}

}
