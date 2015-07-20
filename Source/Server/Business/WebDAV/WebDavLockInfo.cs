using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.WebDAV
{
	public class WebDavLockInfo
	{
		private int _propertyId;
		private string _fileName;
		private int _contentTypeId;
		private DateTime _startLocking;
		private TimeSpan _duration;
		private string _lockedBy;



		public int WebDavElementPropertyId
		{
			get
			{
				return _propertyId;
			}
			set
			{
				_propertyId = value;
			}
		}
		public string FileName
		{
			get
			{
				return _fileName;
			}
			set
			{
				_fileName = value;
			}
		}

		public int ContentTypeId
		{
			get
			{
				return _contentTypeId;
			}
			set
			{
				_contentTypeId = value;
			}
		}

		public DateTime StartLocking
		{
			get
			{
				return _startLocking;
			}
			set
			{
				_startLocking = value;
			}
		}
		public TimeSpan Duration
		{
			get
			{
				return _duration;
			}
			set
			{
				_duration = value;
			}
		}
		public string LockedBy
		{
			get
			{
				return _lockedBy;
			}
			set
			{
				_lockedBy = value;
			}
		}
	}
}
