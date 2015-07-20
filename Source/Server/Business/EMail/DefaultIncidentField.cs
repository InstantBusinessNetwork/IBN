using System;
using System.Collections;
using Mediachase.IBN.Database.EMail;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for DefaultIncidentField.
	/// </summary>
	public class DefaultIncidentField
	{
		private DefaultIncidentFieldRow _srcRow = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultIncidentField"/> class.
		/// </summary>
		/// <param name="row">The row.</param>
		private DefaultIncidentField(DefaultIncidentFieldRow row)
		{
			_srcRow = row;
		}

		/// <summary>
		/// Loads this instance.
		/// </summary>
		/// <returns></returns>
		public static DefaultIncidentField Load()
		{
			DefaultIncidentFieldRow rowItem = null;

			DefaultIncidentFieldRow[] rows = DefaultIncidentFieldRow.List();
			if(rows.Length>0)
				rowItem = rows[0];
			else
				rowItem = new DefaultIncidentFieldRow();

			return new DefaultIncidentField(rowItem);
		}

		/// <summary>
		/// Updates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public static void Update(DefaultIncidentField item)
		{
			item._srcRow.Update();
		}

		#region Public Properties
		
		/// <summary>
		/// Gets or sets the project id.
		/// </summary>
		/// <value>The project id.</value>
		public virtual int ProjectId
		{
			get
			{
				return _srcRow.ProjectId;
			}
			
			set
			{
				_srcRow.ProjectId = value;
			}	
			
		}
		
		/// <summary>
		/// Gets or sets the creator id.
		/// </summary>
		/// <value>The creator id.</value>
		public virtual int CreatorId
	    
		{
			get
			{
				return _srcRow.CreatorId;
			}
			
			set
			{
				_srcRow.CreatorId = value;
			}	
			
		}
		
		/// <summary>
		/// Gets or sets the manager id.
		/// </summary>
		/// <value>The manager id.</value>
		public virtual int ManagerId
	    
		{
			get
			{
				return _srcRow.ManagerId;
			}
			
			set
			{
				_srcRow.ManagerId = value;
			}	
			
		}
		
		/// <summary>
		/// Gets or sets the type id.
		/// </summary>
		/// <value>The type id.</value>
		public virtual int TypeId
	    
		{
			get
			{
				return _srcRow.TypeId;
			}
			
			set
			{
				_srcRow.TypeId = value;
			}	
			
		}
		
		/// <summary>
		/// Gets or sets the priority id.
		/// </summary>
		/// <value>The priority id.</value>
		public virtual int PriorityId
	    
		{
			get
			{
				return _srcRow.PriorityId;
			}
			
			set
			{
				_srcRow.PriorityId = value;
			}	
			
		}
		
		/// <summary>
		/// Gets or sets the severity id.
		/// </summary>
		/// <value>The severity id.</value>
		public virtual int SeverityId
	    
		{
			get
			{
				return _srcRow.SeverityId;
			}
			
			set
			{
				_srcRow.SeverityId = value;
			}	
			
		}
		
		/// <summary>
		/// Gets or sets the task time.
		/// </summary>
		/// <value>The task time.</value>
		public virtual int TaskTime
	    
		{
			get
			{
				return _srcRow.TaskTime;
			}
			
			set
			{
				_srcRow.TaskTime = value;
			}	
			
		}
		
		#endregion
	}
}
