using System;
using System.Collections;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.SpreadSheet;


namespace Mediachase.IBN.Business.SpreadSheet
{
	/// <summary>
	/// Summary description for ActualFinances.
	/// </summary>
	public class ActualFinances
	{
		private ActualFinancesRow _srcRow = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActualFinances"/> class.
		/// </summary>
		/// <param name="row">The row.</param>
		private ActualFinances(ActualFinancesRow row)
		{
			_srcRow = row;
		}

		#region Create
		/// <summary>
		/// Creates the specified user id.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="ObjectId">The object id.</param>
		/// <param name="ObjectType">Type of the object.</param>
		/// <param name="Date">The date.</param>
		/// <param name="RowId">The row id.</param>
		/// <param name="Value">The value.</param>
		/// <param name="Comment">The comment.</param>
		/// <returns></returns>
		public static int Create(int UserId, int ObjectId, ObjectTypes ObjectType, DateTime Date, string RowId, double Value, string Comment)
		{
			ActualFinancesRow newRow = new ActualFinancesRow();

			newRow.CreatorId = UserId;

			newRow.ObjectId = ObjectId;
			newRow.ObjectTypeId = (int)ObjectType;

			newRow.Date = Date;

			newRow.RowId = RowId;
			newRow.Value = Value;
			newRow.Comment = Comment;

			newRow.Update();

			return newRow.PrimaryKeyId;

		}

		/// <summary>
		/// Creates the specified object id.
		/// </summary>
		/// <param name="ObjectId">The object id.</param>
		/// <param name="ObjectTypeId">The object type id.</param>
		/// <param name="Date">The date.</param>
		/// <param name="RowId">The row id.</param>
		/// <param name="Value">The value.</param>
		/// <param name="Comment">The comment.</param>
		/// <returns></returns>
		public static int Create(int ObjectId, ObjectTypes ObjectType, DateTime Date, string RowId, double Value, string Comment)
		{
			ActualFinancesRow newRow = new ActualFinancesRow();

			newRow.CreatorId = Security.CurrentUser.UserID;

			newRow.ObjectId = ObjectId;
			newRow.ObjectTypeId = (int)ObjectType;

			newRow.Date = Date;

			newRow.RowId = RowId;
			newRow.Value = Value;
			newRow.Comment = Comment;

			newRow.Update();

			return newRow.PrimaryKeyId;
		}

		/// <summary>
		/// Creates the specified object id.
		/// </summary>
		/// <param name="ObjectId">The object id.</param>
		/// <param name="ObjectTypeId">The object type id.</param>
		/// <param name="Date">The date.</param>
		/// <param name="RowId">The row id.</param>
		/// <param name="Value">The value.</param>
		/// <param name="Comment">The comment.</param>
		/// <param name="BlockId">The TimeTrackingBlock id.</param>
		/// <returns></returns>
		public static int Create(int ObjectId, ObjectTypes ObjectType, DateTime Date, string RowId, double Value, string Comment, int BlockId, double TotalApproved, int OwnerId)
		{
			ActualFinancesRow newRow = new ActualFinancesRow();

			newRow.CreatorId = Security.CurrentUser.UserID;

			newRow.ObjectId = ObjectId;
			newRow.ObjectTypeId = (int)ObjectType;

			newRow.Date = Date;

			newRow.RowId = RowId;
			newRow.Value = Value;
			newRow.Comment = Comment;
			newRow.BlockId = BlockId;
			newRow.TotalApproved = TotalApproved;
			newRow.OwnerId = OwnerId;

			newRow.Update();

			return newRow.PrimaryKeyId;
		}
		#endregion

		#region Load
		/// <summary>
		/// Loads the specified actual finances id.
		/// </summary>
		/// <param name="ActualFinancesId">The actual finances id.</param>
		/// <returns></returns>
		public static ActualFinances Load(int ActualFinancesId)
		{
			return new ActualFinances(new ActualFinancesRow(ActualFinancesId));
		}
		#endregion

		#region Update
		/// <summary>
		/// Updates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public static void Update(ActualFinances item)
		{
			item._srcRow.CreatorId = Security.CurrentUser.UserID;
			item._srcRow.Update();
		}
		#endregion

		#region List
		/// <summary>
		/// Lists the specified object id.
		/// </summary>
		/// <param name="ObjectId">The object id.</param>
		/// <param name="ObjectTypeId">The object type id.</param>
		/// <returns></returns>
		public static ActualFinances[] List(int objectId, ObjectTypes objectType)
		{
			return List(objectId, objectType, null, null);
		}

		/// <summary>
		/// Lists the specified object id.
		/// </summary>
		/// <param name="objectId">The object id.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <returns></returns>
		public static ActualFinances[] List(int objectId, ObjectTypes objectType, DateTime? startDate, DateTime? endDate)
		{
			ArrayList retVal = new ArrayList();

			foreach (ActualFinancesRow row in ActualFinancesRow.List(objectId, (int)objectType, startDate, endDate))
			{
				retVal.Add(new ActualFinances(row));
			}

			return (ActualFinances[])retVal.ToArray(typeof(ActualFinances));
		}
		#endregion

		#region Delete
		/// <summary>
		/// Deletes the specified actual finances id.
		/// </summary>
		/// <param name="ActualFinancesId">The actual finances id.</param>
		public static void Delete(int ActualFinancesId)
		{
			ActualFinancesRow.Delete(ActualFinancesId);
		}

		//DV
		public static void Delete(int ObjectId, int ObjectTypeId)
		{
			ActualFinancesRow.Delete(ObjectId, ObjectTypeId);
		}

		// OR [2008-09-02]
		public static void DeleteByBlockId(int blockId)
		{
			ActualFinancesRow.DeleteByBlockId(blockId);
		}
		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the actual finances id.
		/// </summary>
		/// <value>The actual finances id.</value>
		public virtual int ActualFinancesId
		{
			get
			{
				return _srcRow.ActualFinancesId;
			}
		}
		
		/// <summary>
		/// Gets the creator id.
		/// </summary>
		/// <value>The creator id.</value>
		public virtual int CreatorId
		{
			get
			{
				return _srcRow.CreatorId;
			}
		}
		
		/// <summary>
		/// Gets or sets the date.
		/// </summary>
		/// <value>The date.</value>
		public virtual DateTime Date
	    
		{
			get
			{
				return _srcRow.Date;
			}
			
			set
			{
				_srcRow.Date = value;
			}	
			
		}
		
		/// <summary>
		/// Gets the object id.
		/// </summary>
		/// <value>The object id.</value>
		public virtual int ObjectId
	    
		{
			get
			{
				return _srcRow.ObjectId;
			}
		}
		
		/// <summary>
		/// Gets the object type id.
		/// </summary>
		/// <value>The object type id.</value>
		public virtual int ObjectTypeId
	    
		{
			get
			{
				return _srcRow.ObjectTypeId;
			}
			
		}
		
		/// <summary>
		/// Gets the row id.
		/// </summary>
		/// <value>The row id.</value>
		public virtual string RowId
	    
		{
			get
			{
				return _srcRow.RowId;
			}
			set
			{
				_srcRow.RowId = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public virtual double Value
	    
		{
			get
			{
				return _srcRow.Value;
			}
			set
			{
				_srcRow.Value = value;
			}	
			
		}
		
		/// <summary>
		/// Gets or sets the comment.
		/// </summary>
		/// <value>The comment.</value>
		public virtual string Comment
		{
			get
			{
				return _srcRow.Comment;
			}
			
			set
			{
				_srcRow.Comment = value;
			}	
			
		}

		/// <summary>
		/// Gets the Block id.
		/// </summary>
		/// <value>The Block id.</value>
		public virtual int? BlockId
		{
			get
			{
				return _srcRow.BlockId;
			}
			set
			{
				_srcRow.BlockId = value;
			}
		}

		/// <summary>
		/// Gets or sets the total approved.
		/// </summary>
		/// <value>The total approved.</value>
		public virtual double? TotalApproved
		{
			get
			{
				return _srcRow.TotalApproved;
			}
			set
			{
				_srcRow.TotalApproved = value;
			}
		}

		/// <summary>
		/// Gets or sets the owner id.
		/// </summary>
		/// <value>The owner id.</value>
		public virtual int? OwnerId
		{
			get
			{
				return _srcRow.OwnerId;
			}
			set
			{
				_srcRow.OwnerId = value;
			}
		}
		#endregion
	}
}
