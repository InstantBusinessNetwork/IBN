using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Events
{
	public class VirtualEventId
	{
		private Guid _guid;
		private Int16 _recurrenceId = -1;
	
		private VirtualEventId(Guid guid, Int16 recurrenceId)
		{
			_guid = guid;
			_recurrenceId = recurrenceId;
		}

		public Int16 RecurrenceId
		{
			get
			{
				return _recurrenceId;
			}
			set
			{
				_recurrenceId = value;
			}
		}

		public bool IsRecurrence
		{
			get
			{
				return _recurrenceId != -1;
			}
		}

		public static VirtualEventId CreateInstance(Guid guid, Int16 reccurenseId)
		{
			return new VirtualEventId(guid, reccurenseId);
		}

		public static VirtualEventId CreateInstance()
		{
			byte[] buffer = Guid.NewGuid().ToByteArray();
			//Set recurrence id is -1
			for (int i = 0; i < 2; i++)
			{
				buffer[i + 14] = 0xff;//-1
			}
			return new VirtualEventId(new Guid(buffer), -1);
		}

		public PrimaryKeyId RealEventId
		{
			get
			{
				return (PrimaryKeyId)this;
			}
		}

		#region PrimaryKeyId explicit conversion to VirtualEventId
		public static explicit operator VirtualEventId(PrimaryKeyId value)
		{
			byte[] buffer = ((Guid)(PrimaryKeyId)value).ToByteArray();
			Int16 recurrenceId = BitConverter.ToInt16(buffer, 14);
			for (int i = 0; i < 2; i++)
			{
				buffer[i + 14] = 0xff; //-1
			}
			return new VirtualEventId(new Guid(buffer), recurrenceId);
		}

		public static explicit operator PrimaryKeyId(VirtualEventId value)
		{
			return new PrimaryKeyId(value._guid);
		}
		#endregion

		public override string ToString()
		{
			byte[] buffer = _guid.ToByteArray();
			byte[] recurr = BitConverter.GetBytes(_recurrenceId);
			for (int i = 0; i < 2; i++)
			{
				buffer[i + 14] = recurr[i];
			}

			return new Guid(buffer).ToString();
		}

	}
}
