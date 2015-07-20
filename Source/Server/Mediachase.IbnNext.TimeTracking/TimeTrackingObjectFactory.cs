using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data;

namespace Mediachase.IbnNext.TimeTracking
{
	/// <summary>
	/// Represents .
	/// </summary>
	public class TimeTrackingObjectFactory : IMetaObjectFactory
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="TimeTrackingObjectFactory"/> class.
		/// </summary>
		public TimeTrackingObjectFactory()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		#endregion

		#region IMetaObjectFactory Members

		/// <summary>
		/// Determines whether this instance can create the specified meta class.
		/// </summary>
		/// <param name="metaClass">The meta class.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can create the specified meta class; otherwise, <c>false</c>.
		/// </returns>
		bool IMetaObjectFactory.CanCreate(Mediachase.Ibn.Data.Meta.Management.MetaClass metaClass)
		{
			if(metaClass==null)
				throw new ArgumentNullException("metaClass");

			switch (metaClass.Name)
			{
				case "TimeTrackingEntry":
				case "TimeTrackingBlock":
				case "TimeTrackingBlockType":
				case "TimeTrackingBlockTypeInstance":
					return true;
			}

			return false;
		}

		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <param name="metaClass">The meta class.</param>
		/// <returns></returns>
		MetaObject IMetaObjectFactory.CreateInstance(Mediachase.Ibn.Data.Meta.Management.MetaClass metaClass)
		{
			if (metaClass == null)
				throw new ArgumentNullException("metaClass");

			switch (metaClass.Name)
			{
				case "TimeTrackingEntry":
					return new TimeTrackingEntry();
				case "TimeTrackingBlock":
					return new TimeTrackingBlock();
				case "TimeTrackingBlockType":
					return new TimeTrackingBlockType();
				case "TimeTrackingBlockTypeInstance":
					return new TimeTrackingBlockTypeInstance();
			}

			throw new NotSupportedException("MetaClass '" + metaClass.Name + "' is not supported.");
		}

		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <param name="metaClass">The meta class.</param>
		/// <param name="primaryKeyId">The primary key id.</param>
		/// <returns></returns>
		MetaObject IMetaObjectFactory.CreateInstance(Mediachase.Ibn.Data.Meta.Management.MetaClass metaClass, PrimaryKeyId primaryKeyId)
		{
			if (metaClass == null)
				throw new ArgumentNullException("metaClass");

			switch (metaClass.Name)
			{
				case "TimeTrackingEntry":
					return new TimeTrackingEntry(primaryKeyId);
				case "TimeTrackingBlock":
					return new TimeTrackingBlock(primaryKeyId);
				case "TimeTrackingBlockType":
					return new TimeTrackingBlockType(primaryKeyId);
				case "TimeTrackingBlockTypeInstance":
					return new TimeTrackingBlockTypeInstance(primaryKeyId);
			}

			throw new NotSupportedException("MetaClass '" + metaClass.Name + "' is not supported.");
		}

		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <param name="metaClass">The meta class.</param>
		/// <param name="row">The row.</param>
		/// <returns></returns>
		MetaObject IMetaObjectFactory.CreateInstance(Mediachase.Ibn.Data.Meta.Management.MetaClass metaClass, Mediachase.Ibn.Data.Sql.CustomTableRow row)
		{
			if (metaClass == null)
				throw new ArgumentNullException("metaClass");

			switch (metaClass.Name)
			{
				case "TimeTrackingEntry":
					return new TimeTrackingEntry(row);
				case "TimeTrackingBlock":
					return new TimeTrackingBlock(row);
				case "TimeTrackingBlockType":
					return new TimeTrackingBlockType(row);
				case "TimeTrackingBlockTypeInstance":
					return new TimeTrackingBlockTypeInstance(row);
			}

			throw new NotSupportedException("MetaClass '" + metaClass.Name + "' is not supported.");
		}

		#endregion
	}
}
