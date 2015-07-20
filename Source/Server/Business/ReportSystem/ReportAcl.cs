using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using System.Collections;

namespace Mediachase.IBN.Business.ReportSystem
{
	/// <summary>
	/// Represents Report access control list.
	/// </summary>
	[Serializable]
	public class ReportAcl: CollectionBase
	{
		private PrimaryKeyId _ownerReportId = 0;
		private bool _isChanged = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportAcl"/> class.
		/// </summary>
		protected ReportAcl()
		{
		}

		/// <summary>
		/// Gets the ACL.
		/// </summary>
		/// <param name="reportId">The report id.</param>
		/// <returns></returns>
		public static ReportAcl GetAcl(PrimaryKeyId reportId)
		{
			ReportAcl retVal = new ReportAcl();

			retVal.OwnerReportId = reportId;

			foreach (mcweb_ReportAceRow row in mcweb_ReportAceRow.List(FilterElement.EqualElement(mcweb_ReportAceRow.Columns.ReportId, reportId)))
			{
				retVal.Add(new ReportAce(row.ReportAceId, row.Role, row.PrincipalId, row.Allow));
			}

			retVal.Reset();

			return retVal;
		}

		/// <summary>
		/// Sets the ACL.
		/// </summary>
		/// <param name="acl">The acl.</param>
		public static void SetAcl(ReportAcl acl)
		{
			if(acl==null)
				throw new ArgumentNullException("acl");

			if (acl.IsChanged)
			{
				Guid reportId = (Guid)acl.OwnerReportId;

				using (TransactionScope tran = DataContext.Current.BeginTransaction())
				{
					// Step 3. Remove old ace
					foreach (mcweb_ReportAceRow row in mcweb_ReportAceRow.List(FilterElement.EqualElement(mcweb_ReportAceRow.Columns.ReportId, reportId)))
					{
						row.Delete();
					}

					// Save New Ace
					foreach (ReportAce ace in acl)
					{
						mcweb_ReportAceRow newRow = new mcweb_ReportAceRow();

						newRow.ReportId = reportId;
						newRow.Role = ace.Role;
						newRow.PrincipalId = ace.PrincipalId;

						// Hot Fix
						if (newRow.Role == string.Empty)
							newRow.Role = null;
						if (newRow.PrincipalId.HasValue && newRow.PrincipalId.Value <= 0)
							newRow.PrincipalId = null;

						newRow.Allow = ace.Allow;

						newRow.Update();
					}

					tran.Commit();
				}
			}
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
		/// Gets the owner directory id.
		/// </summary>
		/// <value>The owner directory id.</value>
		public PrimaryKeyId OwnerReportId
		{
			get
			{
				return _ownerReportId;
			}
			protected set
			{
				_ownerReportId = value;
			}
		}

		/// <summary>
		/// Adds the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public int Add(ReportAce item)
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
		public bool CheckAceOriginality(ReportAce item)
		{
			foreach(ReportAce ace in this)
			{
				if(ace.PrincipalId==item.PrincipalId&&
					ace.Role==item.Role&&
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
		public void Remove(ReportAce item)
		{
			this.List.Remove(item);
		}

		/// <summary>
		/// Removes the specified aceid.
		/// </summary>
		/// <param name="aceid">The aceid.</param>
		public void Remove(int aceid)
		{
			foreach(ReportAce item in this)
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
		public bool Contains(ReportAce item)
		{
			return this.List.Contains(item);
		}

		/// <summary>
		/// Gets the <see cref="ReportAce"/> at the specified index.
		/// </summary>
		/// <value></value>
		public ReportAce this[int index]
		{
			get
			{
				return (ReportAce)this.List[index];
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
