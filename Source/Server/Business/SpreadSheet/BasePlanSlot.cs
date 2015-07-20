using System;
using System.Collections;
using Mediachase.IBN.Database.SpreadSheet;

namespace Mediachase.IBN.Business.SpreadSheet
{
	/// <summary>
	/// Summary description for BasePlanSlot.
	/// </summary>
	public class BasePlanSlot
	{
		private BasePlanSlotRow _srcRow = null;

		private BasePlanSlot(BasePlanSlotRow row)
		{
			_srcRow = row;
		}

		public static BasePlanSlot Load(int BasePlanSlotId)
		{
			return new BasePlanSlot(new BasePlanSlotRow(BasePlanSlotId));
		}

		/// <summary>
		/// Lists this instance.
		/// </summary>
		/// <returns></returns>
		public static BasePlanSlot[] List()
		{
			ArrayList retVal = new ArrayList();

			foreach(BasePlanSlotRow row in BasePlanSlotRow.List())
			{
				retVal.Add(new BasePlanSlot(row));
			}

			return (BasePlanSlot[])retVal.ToArray(typeof(BasePlanSlot));
		}

		/// <summary>
		/// Creates the specified name.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <param name="IsDefault">if set to <c>true</c> [is default].</param>
		/// <returns></returns>
		public static int Create(string Name, bool IsDefault)
		{
			BasePlanSlotRow row = new BasePlanSlotRow();
			row.Name = Name;
			row.IsDefault = IsDefault;
			row.Update();

			return row.PrimaryKeyId;
		}

		/// <summary>
		/// Updates the specified item.
		/// </summary>
		/// <param name="Item">The item.</param>
		public static void Update(BasePlanSlot Item)
		{
			Item._srcRow.Update();
		}

		/// <summary>
		/// Deletes the specified base plan slot id.
		/// </summary>
		/// <param name="BasePlanSlotId">The base plan slot id.</param>
		public static void Delete(int BasePlanSlotId)
		{
			BasePlanSlotRow.Delete(BasePlanSlotId);
		}


		#region Public Properties
		
		/// <summary>
		/// Gets the base plan slot id.
		/// </summary>
		/// <value>The base plan slot id.</value>
		public virtual int BasePlanSlotId
	    
		{
			get
			{
				return _srcRow.BasePlanSlotId;
			}
			
		}
		
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public virtual string Name
	    
		{
			get
			{
				return _srcRow.Name;
			}
			set
			{
				_srcRow.Name = value;
			}	
			
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this instance is default.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is default; otherwise, <c>false</c>.
		/// </value>
		public virtual bool IsDefault
	    
		{
			get
			{
				return _srcRow.IsDefault;
			}
			
			set
			{
				_srcRow.IsDefault = value;
			}	
			
		}
		
		#endregion
	}
}
