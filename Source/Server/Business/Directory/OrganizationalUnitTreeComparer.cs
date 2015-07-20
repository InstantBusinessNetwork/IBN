using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Business.Directory
{
	/// <summary>
	/// Represents TreeElementComparer.
	/// </summary>
	internal class OrganizationalUnitTreeComparer: IComparer<EntityObject>
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="TreeElementComparer"/> class.
		/// </summary>
		/// <param name="items">The items.</param>
		public OrganizationalUnitTreeComparer(List<EntityObject> items)
		{
			foreach (EntityObject item in items)
			{
				InnerDictionary.Add(item.PrimaryKeyId.Value, (DirectoryOrganizationalUnitEntity)item);
			}
		}
		#endregion

		#region Properties
		private Dictionary<PrimaryKeyId, DirectoryOrganizationalUnitEntity> _dic = new Dictionary<PrimaryKeyId, DirectoryOrganizationalUnitEntity>();

		/// <summary>
		/// Gets or sets the inner dictionary.
		/// </summary>
		/// <value>The inner dictionary.</value>
		public Dictionary<PrimaryKeyId, DirectoryOrganizationalUnitEntity> InnerDictionary
		{
			get { return _dic; }
			protected set { _dic = value; }
		}
	
		#endregion

		#region Methods
		/// <summary>
		/// Compares the specified x unit.
		/// </summary>
		/// <param name="xUnit">The x unit.</param>
		/// <param name="yUnit">The y unit.</param>
		/// <returns></returns>
		protected virtual int Compare(DirectoryOrganizationalUnitEntity xUnit, DirectoryOrganizationalUnitEntity yUnit)
		{
			// Root Should be First
			if (xUnit == yUnit)
				return 0;
			if (xUnit.ParentId == null)
				return -1;
			if (yUnit.ParentId == null)
				return 1;

			if (xUnit.OutlineLevel == yUnit.OutlineLevel)
			{
				if (xUnit.ParentId == yUnit.ParentId)
				{
					return xUnit.Name.CompareTo(yUnit.Name);
				}
				else
				{
					return Compare(this.InnerDictionary[xUnit.ParentId.Value],
						this.InnerDictionary[yUnit.ParentId.Value]);
				}
			}
			else if (xUnit.OutlineLevel > yUnit.OutlineLevel)
			{
				DirectoryOrganizationalUnitEntity xUnitParent = xUnit;

				for (int index = 0; index < (xUnit.OutlineLevel - yUnit.OutlineLevel); index++)
				{
					xUnitParent = this.InnerDictionary[xUnitParent.ParentId.Value];
				}

				int iCompareResult = Compare(xUnitParent, yUnit);
				return iCompareResult==0?1:iCompareResult;
			}
			else if (xUnit.OutlineLevel < yUnit.OutlineLevel)
			{
				DirectoryOrganizationalUnitEntity yUnitParent = yUnit;

				for (int index = 0; index < (yUnit.OutlineLevel -xUnit.OutlineLevel) ; index++)
				{
					yUnitParent = this.InnerDictionary[yUnitParent.ParentId.Value];
				}

				int iCompareResult = Compare(xUnit, yUnitParent);
				return iCompareResult == 0 ? -1 : iCompareResult;
			}

			return 0;
		}

		#endregion

		#region IComparer<EntityObject> Members

		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>
		/// Value Condition Less than zerox is less than y.Zerox equals y.Greater than zerox is greater than y.
		/// </returns>
		int IComparer<EntityObject>.Compare(EntityObject x, EntityObject y)
		{
			return Compare((DirectoryOrganizationalUnitEntity)x, (DirectoryOrganizationalUnitEntity)y);
		}

		#endregion
	}
}
