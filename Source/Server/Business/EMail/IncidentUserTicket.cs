using System;
using System.Collections;
using Mediachase.IBN.Database.EMail;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for IncidentUserTicket.
	/// </summary>
	public class IncidentUserTicket
	{
		private IncidentUserTicketRow _srcRow = null;

		private IncidentUserTicket(IncidentUserTicketRow row)
		{
			_srcRow = row;
		}

		public static int Create(int UserId, int IncidentId)
		{
			IncidentUserTicketRow row = new IncidentUserTicketRow();

			row.UserId = UserId;
			row.IncidentId = IncidentId;

			row.Update();

			return row.PrimaryKeyId;
		}

		public static Guid CreateAndReturnUID(int UserId, int IncidentId)
		{
			IncidentUserTicketRow row = new IncidentUserTicketRow();

			row.UserId = UserId;
			row.IncidentId = IncidentId;

			row.Update();

			return row.UID;
		}

		public static IncidentUserTicket Load(int IncidentUserTicketId)
		{
			return new IncidentUserTicket(new IncidentUserTicketRow(IncidentUserTicketId));
		}

		public static IncidentUserTicket Load(Guid UID)
		{
			IncidentUserTicketRow[] rows = IncidentUserTicketRow.List(UID);
			if(rows.Length>0)
				return new IncidentUserTicket(rows[0]);
			return null;
		}

		#region Public Properties
		
		public virtual int IncidentUserTicketId
	    
		{
			get
			{
				return _srcRow.IncidentUserTicketId;
			}
			
		}
		
		public virtual Guid UID
	    
		{
			get
			{
				return _srcRow.UID;
			}
		}
		
		public virtual int UserId
	    
		{
			get
			{
				return _srcRow.UserId;
			}
			
			set
			{
				_srcRow.UserId = value;
			}	
			
		}
		
		public virtual int IncidentId
	    
		{
			get
			{
				return _srcRow.IncidentId;
			}
			
			set
			{
				_srcRow.IncidentId = value;
			}	
			
		}
		
		public virtual DateTime Created
	    
		{
			get
			{
				return _srcRow.Created;
			}
			
			set
			{
				_srcRow.Created = value;
			}	
			
		}
		
		#endregion
	}
}
