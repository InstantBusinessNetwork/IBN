using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Mediachase.ClientOutlook.Configuration
{
    public static class ApplicationConfig
    {
		//private static ProfileSettingSection ProfileSettingCfg
		//{
		//    get
		//    {
		//        return (ProfileSettingSection)ConfigurationManager.GetSection("applicationSettings/MediachaseCTI.Profile.Settings");
		//    }
		//}

     
        public static string ProfileRegLinkRootKey
        {
            get
            {
				return @"Software\Mediachase.Sync.Outlook\LinkSettings";
				//string retVal = string.Empty;
				//if(ProfileSettingCfg != null)
				//{
				//    retVal = ProfileSettingCfg.Settings["regLinkRootKey"];
				//}

				//return retVal;
            }
        }

        public static string ProfileRegRootKey
        {
            get
            {
				return @"Software\Mediachase.Sync.Outlook";
				//string retVal = string.Empty;
				//if(ProfileSettingCfg != null)
				//{
				//    retVal = ProfileSettingCfg.Settings["regRootKey"];
				//}
				//return retVal;
            }
        }

		public static bool Debugging
		{
			get
			{
				return true;
			}
		}

		public static string LogPathFile
		{
			get
			{
				return "c:\\outlookSync.log";
			}
		}

		public static string GenerateReplicaStoreFileName(Outlook.OlItemType oItemType)
		{
			return "c:\\" + oItemType.ToString() + ".dat";
		}
		
    }

}
