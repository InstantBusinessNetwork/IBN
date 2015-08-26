using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Mediachase.ClientOutlook.Configuration.ProfileSetting;
using Mediachase.Sync.Core;
using OutlookAddin.ClientOutlook.Configuration;

namespace Mediachase.ClientOutlook.Configuration
{
    internal abstract class RegistrySettingLinkBase<T> where T: class, new()
    {
        public enum LinkDataType
        {
            File,
            Registry
        }

        private static string SettingLinkRoot { get; set; }
        public Guid LinkId { get; set; }
        private T _referensedSetting;
        
        private IDictionary<string, object> _defaultValues = null;

        protected delegate void PropertyAction(PropertyInfo propInfo, object owner);

        static RegistrySettingLinkBase()
        {
            SettingLinkRoot = ApplicationConfig.ProfileRegLinkRootKey;
        }

        protected RegistrySettingLinkBase(Guid id)
        {
            LinkId = id;
        }
        
        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <returns></returns>
        public static RegistrySettingLinkBase<T> CreateInstance(string filePath)
        {
            return new RegistrySettingLinkFile<T>(Guid.NewGuid(), filePath);
        }

        public static RegistrySettingLinkBase<T> CreateInstance()
        {
            Guid newId = Guid.NewGuid();
            return new RegistrySettingLinkReg<T>( newId, Id2RegKey(newId));
        }
    
        public static RegistrySettingLinkBase<T> CreateInstance(Guid linkId)
        {
           return LoadLinkSetting(linkId);
        }

        public static IEnumerable<RegistrySettingLinkBase<T>> List()
        {
            RegistryController regConn = new RegistryController(SettingLinkRoot);
            return regConn.GetSubKayNames().Select(x => RegKey2Id(x)).Select(x => LoadLinkSetting(x));
        }

        #region Abstract methods

        protected abstract T LoadSetting();
        
        public virtual void SaveLink()
        {

            DebugAssistant.Log("RegistrySettingLinkBase: save link setting"); 
            RegistryController regConn = new RegistryController(Id2RegKey(LinkId));

            LinkDataType linkType = RegistrySettingLinkBase<T>.LinkDataType.File;
            RegistrySettingLinkFile<T> selfType = this as RegistrySettingLinkFile<T>;
            if(selfType != null)
            {
                regConn.WriteRegKey("filePath", selfType.FilePath);
            }
            else
            {
                linkType = LinkDataType.Registry;
            }

            //Save link type
            regConn.WriteRegKey("linkType", ((int)linkType).ToString());
        }

        #endregion

		/// <summary>
		/// Получет или устанавливает объект ссылки.
		/// </summary>
		/// <value>The referenced setting.</value>
        public T ReferencedSetting 
        {
            get
            {
                return _referensedSetting;
            }

            set
            {
                _referensedSetting = value;
            }
         }

		protected virtual void InitializeDefaultValues()
		{
			//пробегаем по всем публичным свойствам объекта
			foreach (PropertyInfo propInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				object typeDefaultValue = propInfo.PropertyType.IsValueType ?
													Activator.CreateInstance(propInfo.PropertyType) : null;
				object propValue = propInfo.GetValue(_referensedSetting, null);
				//если значение свойства равно null то пытаемся проинициализировать его значением по умолчанию
				//получаемых из статического свойства типа объекта ссылки
				if (DefaultValues != null && (propValue == null || propValue.Equals(typeDefaultValue)))
				{
					object defaultValue;
					if (DefaultValues.TryGetValue(propInfo.Name, out defaultValue))
					{
						defaultValue = Type2ConfigPropType.ConvertType2Type(propInfo.PropertyType, defaultValue);
						propInfo.SetValue(_referensedSetting, defaultValue, null);
					}
				}
			}

		}

		private  IDictionary<string, object> DefaultValues
		{
			get
			{
				//Read default values from type
				if (_defaultValues == null)
				{
					if(typeof(T).GetInterfaces().Contains(typeof(ISettingHaveDefaultValues)))
					{
						ISettingHaveDefaultValues defaultValuesItem = ReferencedSetting as ISettingHaveDefaultValues;
						_defaultValues = defaultValuesItem.DefaultValues;
					}
				}

				return _defaultValues;
			}
		}

        #region generic actions wrapper
        /// <summary>
        /// Settings the property action.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="func">The func.</param>
        protected static T2 SettingPropertyAction<T2>(T2 owner, PropertyAction func) where T2 : class, new()
        {
            if (owner == null)
            {
                owner = new T2();
            }
            foreach (PropertyInfo propInfo in typeof(T2).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                func(propInfo, owner);
            }
            return owner;
        }

        #endregion

        #region Util methods

        /// <summary>
        /// Loads the link setting.
        /// </summary>
        /// <param name="linkId">The link id.</param>
        /// <returns></returns>
        private static RegistrySettingLinkBase<T> LoadLinkSetting(Guid linkId)
        {
            RegistrySettingLinkBase<T> retVal = null;
            RegistryController regConn = new RegistryController(Id2RegKey(linkId));
            String regVal = (string)regConn.ReadRegKey("linkType", LinkDataType.Registry.ToString());
            LinkDataType linkdataType = (LinkDataType)Convert.ToInt32(regVal);
            if (linkdataType == LinkDataType.File)
            {
                string filePath = (string)regConn.ReadRegKey("filePath", string.Empty);
                retVal = new RegistrySettingLinkFile<T>(linkId, filePath);
            }
            else
            {
                retVal = new RegistrySettingLinkReg<T>(linkId, Id2RegKey(linkId));
            }

            retVal.LoadSetting();

            return retVal;
        }
        /// <summary>
        /// Gets the name of the reg key.
        /// </summary>
        /// <param name="linkId">The link id.</param>
        /// <returns></returns>
        protected static  string Id2RegKey(Guid linkId)
        {
            return SettingLinkRoot + "\\" + linkId.ToString() + "\\";
        }

        /// <summary>
        /// Regs the key2 id.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        /// <returns></returns>
        protected static  Guid RegKey2Id(string regKey)
        {

            Guid retVal = Guid.Empty;
            string id = regKey != null ? regKey.Substring(regKey.LastIndexOf("\\")) : string.Empty;
            if (!string.IsNullOrEmpty(id))
            {
                retVal = new Guid(id);
            }

            return retVal;
        }
        #endregion
    }
}
