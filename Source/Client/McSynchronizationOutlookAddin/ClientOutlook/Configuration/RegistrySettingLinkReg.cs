using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Mediachase.Sync.Core;

namespace Mediachase.ClientOutlook.Configuration.ProfileSetting
{
    internal class RegistrySettingLinkReg<T> : RegistrySettingLinkBase<T> where T : class, new()
    {
        public string SettingRegKey { get; set; }

        public RegistrySettingLinkReg(Guid id, string settingRegKey)
            : base(id)
        {
            SettingRegKey = settingRegKey;
            ReferencedSetting = new T();

			InitializeDefaultValues();
        }

        /// <summary>
        /// Loads the setting.
        /// </summary>
        /// <returns></returns>
        protected override T LoadSetting()
        {
            DebugAssistant.Log("RegistrySettingLinkreg: Load setting from registry");
			return SettingPropertyAction<T>(ReferencedSetting, LoadPropertyRegAction);
        }

        /// <summary>
        /// Saves the setting.
        /// </summary>
        /// <param name="setting">The setting.</param>
        public override void SaveLink()
        {
            DebugAssistant.Log("RegistrySettingLinkreg: Save setting in registry");
            SettingPropertyAction<T>(base.ReferencedSetting, SavePropertyRegAction);
            base.SaveLink();

        }

        /// <summary>
        /// Saves the property reg action.
        /// </summary>
        /// <param name="propInfo">The prop info.</param>
        /// <returns></returns>
        protected void SavePropertyRegAction(PropertyInfo propInfo, object owner)
        {
            RegistryController regConn = new RegistryController(SettingRegKey);
            object value = Type2ConfigPropType.ConvertType2Type(propInfo.PropertyType,
                                                                    propInfo.GetValue(owner, null));
            if (value != null)
            {
                regConn.WriteRegKey(propInfo.Name, value != null ? value.ToString() : string.Empty);
            }
        }

        /// <summary>
        /// Loads the property reg action.
        /// </summary>
        /// <param name="propInfo">The prop info.</param>
        /// <returns></returns>
        protected void LoadPropertyRegAction(PropertyInfo propInfo, object owner)
        {
            RegistryController regConn = new RegistryController(SettingRegKey);

            object value = Type2ConfigPropType.ConvertType2Type(propInfo.PropertyType,
                                                                regConn.ReadRegKey(propInfo.Name, null));
            if (value != null)
            {
                propInfo.SetValue(owner, value, null);
            }
        }

    }
}
