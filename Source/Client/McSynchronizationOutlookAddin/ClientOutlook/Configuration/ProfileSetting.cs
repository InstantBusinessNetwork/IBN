using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Sync.Core;

namespace Mediachase.ClientOutlook.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class UserProfileSetting
    {
        internal RegistrySettingLinkBase<syncAppSetting> SyncAppSettingLink { get; set; }
        internal RegistrySettingLinkBase<syncAppointmentSetting> SyncAppointmentSettingLink { get; set; }
		internal RegistrySettingLinkBase<syncContactSetting> SyncContactSettingLink { get; set; }
		internal RegistrySettingLinkBase<syncTaskSetting> SyncTaskSettingLink { get; set; }
      

        private const string SYNC_APPLICATION_REGKEY = "syncApplicationSetting";
        private const string SYNC_APPOINTMENT_REGKEY = "syncAppointmentSetting";
		private const string SYNC_CONTACT_REGKEY	 = "syncContactSetting";
		private const string SYNC_TASK_REGKEY		 = "syncTaskSetting";
      

        public static string ProfileSettingRoot { get; set; }
      
        static UserProfileSetting()
        {
            ProfileSettingRoot = ApplicationConfig.ProfileRegRootKey;
        }

        public UserProfileSetting()
        {
        }

        #region Methods
        public static UserProfileSetting LoadActiveProfile()
        {
            UserProfileSetting retVal = new UserProfileSetting();

            RegistryController regConn = new RegistryController(ProfileSettingRoot);
			string activeSyncAppSettingLink = (string)regConn.ReadRegKey(SYNC_APPLICATION_REGKEY, null);
			string activeAppointmentSettingLink = (string)regConn.ReadRegKey(SYNC_APPOINTMENT_REGKEY, null);
			string activeContactSettingLink = (string)regConn.ReadRegKey(SYNC_CONTACT_REGKEY, null);
			string activeTaskSettingLink = (string)regConn.ReadRegKey(SYNC_TASK_REGKEY, null);
			//Application
			if (!string.IsNullOrEmpty(activeSyncAppSettingLink))
			{
				DebugAssistant.Log("ProfileSetting: Load sync application setting.");
				retVal.SyncAppSettingLink = RegistrySettingLinkBase<syncAppSetting>.CreateInstance(new Guid(activeSyncAppSettingLink));
			}
			else
			{
				DebugAssistant.Log("ProfileSetting: Sync application setting not found. Create default..");
				retVal.SyncAppSettingLink = RegistrySettingLinkBase<syncAppSetting>.CreateInstance();
				retVal.SyncAppSettingLink.SaveLink();
			}
			//Appointment
			if (!string.IsNullOrEmpty(activeAppointmentSettingLink))
			{
				DebugAssistant.Log("ProfileSetting: Load sync appointment setting.");
				retVal.SyncAppointmentSettingLink = RegistrySettingLinkBase<syncAppointmentSetting>.CreateInstance(new Guid(activeAppointmentSettingLink));
			}
			else
			{
				DebugAssistant.Log("ProfileSetting: Sync appointment setting not found. Create default..");
				retVal.SyncAppointmentSettingLink = RegistrySettingLinkBase<syncAppointmentSetting>.CreateInstance();
				retVal.SyncAppointmentSettingLink.SaveLink();

			}
			//Contact
			if (!string.IsNullOrEmpty(activeContactSettingLink))
			{
				retVal.SyncContactSettingLink = RegistrySettingLinkBase<syncContactSetting>.CreateInstance(new Guid(activeContactSettingLink));
			}
			else
			{
				retVal.SyncContactSettingLink = RegistrySettingLinkBase<syncContactSetting>.CreateInstance();
				retVal.SyncContactSettingLink.SaveLink();
			}
			//Task
			if (!string.IsNullOrEmpty(activeTaskSettingLink))
			{
				retVal.SyncTaskSettingLink = RegistrySettingLinkBase<syncTaskSetting>.CreateInstance(new Guid(activeTaskSettingLink));
			}
			else
			{
				retVal.SyncTaskSettingLink = RegistrySettingLinkBase<syncTaskSetting>.CreateInstance();
				retVal.SyncTaskSettingLink.SaveLink();
			}

			retVal.SaveActiveProfile();

            return retVal;
        }

        public void SaveActiveProfile()
        {
            DebugAssistant.Log("ProfileSetting: Save active profile");

            RegistryController regConn = new RegistryController(ProfileSettingRoot);

            if(SyncAppSettingLink != null)
            {
                SyncAppSettingLink.SaveLink();
                regConn.WriteRegKey(SYNC_APPLICATION_REGKEY, SyncAppSettingLink.LinkId.ToString());
            }
            if(SyncAppointmentSettingLink != null)
            {
                SyncAppointmentSettingLink.SaveLink();
                regConn.WriteRegKey(SYNC_APPOINTMENT_REGKEY, SyncAppointmentSettingLink.LinkId.ToString());
            }
			if (SyncContactSettingLink != null)
			{
				SyncContactSettingLink.SaveLink();
				regConn.WriteRegKey(SYNC_CONTACT_REGKEY, SyncContactSettingLink.LinkId.ToString());
			}
			if (SyncTaskSettingLink != null)
			{
				SyncTaskSettingLink.SaveLink();
				regConn.WriteRegKey(SYNC_TASK_REGKEY, SyncTaskSettingLink.LinkId.ToString());
			}
          
        }

        #endregion

        public syncAppSetting CurrentSyncAppSetting
        {
            get
            {
				syncAppSetting retVal = null;

                if (SyncAppSettingLink != null)
                {
                    retVal = SyncAppSettingLink.ReferencedSetting;
                }

                return retVal;
            }

            set
            {
                if (SyncAppSettingLink != null)
                {
                    SyncAppSettingLink.ReferencedSetting = value;
                }

            }
        }

        public syncAppointmentSetting CurrentSyncAppointentSetting
        {
            get
            {

                syncAppointmentSetting retVal = null;
                if (SyncAppointmentSettingLink != null)
                {
                    retVal = SyncAppointmentSettingLink.ReferencedSetting;
                }

                return retVal;
            }

            set
            {
				if (SyncAppointmentSettingLink != null)
                {
					SyncAppointmentSettingLink.ReferencedSetting = value;
                }
            }
        }

		public syncContactSetting CurrentContactSetting
		{
			get
			{

				syncContactSetting retVal = null;
				if (SyncContactSettingLink != null)
				{
					retVal = SyncContactSettingLink.ReferencedSetting;
				}

				return retVal;
			}

			set
			{
				if (SyncContactSettingLink != null)
				{
					SyncContactSettingLink.ReferencedSetting = value;
				}
			}
		}

		public syncTaskSetting CurrentTaskSetting
		{
			get
			{

				syncTaskSetting retVal = null;
				if (SyncTaskSettingLink != null)
				{
					retVal = SyncTaskSettingLink.ReferencedSetting;
				}

				return retVal;
			}

			set
			{
				if (SyncTaskSettingLink != null)
				{
					SyncTaskSettingLink.ReferencedSetting = value;
				}
			}
		}
    }
}
