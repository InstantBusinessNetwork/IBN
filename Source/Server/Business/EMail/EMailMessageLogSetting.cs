using System;
using System.Collections;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.EMail;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for EMailMessageLogSetting.
	/// </summary>
	public class EMailMessageLogSetting
	{
		private EMailMessageLogSettingsRow _srcRow = null;

		private EMailMessageLogSetting(EMailMessageLogSettingsRow row)
		{
			_srcRow = row;
		}

		public static EMailMessageLogSetting Current
		{
			get 
			{
				EMailMessageLogSettingsRow[] list = EMailMessageLogSettingsRow.List();

				if(list.Length>0)
					return new EMailMessageLogSetting(list[0]);

				return new EMailMessageLogSetting(new EMailMessageLogSettingsRow());
			}
		}

		public static void Update(EMailMessageLogSetting Setting)
		{
			Setting._srcRow.Update();
		}

		#region Public Properties
		
//		public virtual int EMailMessageLogSettingId
//	    
//		{
//			get
//			{
//				return _srcRow.EMailMessageLogSettingId;
//			}
//			
//		}
		
		public virtual bool IsActive
	    
		{
			get
			{
				return _srcRow.IsActive;
			}
			
			set
			{
				_srcRow.IsActive = value;
			}	
			
		}
		
		public virtual int Period
	    
		{
			get
			{
				return _srcRow.Period;
			}
			
			set
			{
				_srcRow.Period = value;
			}	
			
		}
		
		#endregion
	}
}
