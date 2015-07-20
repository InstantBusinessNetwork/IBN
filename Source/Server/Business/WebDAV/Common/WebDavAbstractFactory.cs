using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.IBN.Business.WebDAV.Util;
using Mediachase.Net.WebDavServer;
using Mediachase.IBN.Business.WebDAV.ElementStorageProvider;

namespace Mediachase.IBN.Business.WebDAV.Definition
{
    /// <summary>
    /// WebDav abstract factory
    /// </summary>
    internal class WebDavAbstractFactory : AbstractFactory, IFactoryMethod<WebDavAbsolutePath>, IFactoryMethod<WebDavElementStorageProvider>
    {

        #region IFactoryMethod<WebDavElementStorageProvider> Members

        public WebDavElementStorageProvider Create(object obj)
        {
            WebDavElementStorageProvider retVal = null;
            ObjectTypes storageType = (ObjectTypes)obj;
            switch (storageType)
            {
                case ObjectTypes.File_FileStorage:
                    retVal = WebDavApplication.WebDavStorageProviders["FileStorageProvider"] as WebDavElementStorageProvider;
                    break;
                case ObjectTypes.File_MetaDataPlus:
                    retVal = WebDavApplication.WebDavStorageProviders["MetaDataPlusStorageProvider"] as MetaDataPlusProvider;
                    break;
                case ObjectTypes.File_MetaData:
                    retVal = WebDavApplication.WebDavStorageProviders["MetaDataStorageProvider"] as MetaDataProvider;
                    break;
                case ObjectTypes.File_Incident:
                    retVal = WebDavApplication.WebDavStorageProviders["EmailStorageProvider"] as EmailStorageProvider;
                    break;
				case ObjectTypes.Folder:
					retVal = WebDavApplication.WebDavStorageProviders["RootFolderStorageProvider"] as RootFolderStorageProvider;
					break;
            }

            return retVal;
        }

        WebDavElementStorageProvider IFactoryMethod<WebDavElementStorageProvider>.Create(object obj1, object obj2)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IFactoryMethod<WebDavAbsolutePath> Members

        WebDavAbsolutePath IFactoryMethod<WebDavAbsolutePath>.Create(object obj)
        {
            WebDavAbsolutePath retVal = null;
            ObjectTypes storageType = (ObjectTypes)obj;
            switch (storageType)
            {
                case ObjectTypes.File_FileStorage:
                    retVal = GenericCreate<FileStorageAbsolutePath>();
                    break;
                case ObjectTypes.File_MetaData:
                    retVal = GenericCreate<MetaDataAbsolutePath>();
                    break;
                case ObjectTypes.File_MetaDataPlus:
                    retVal = GenericCreate<MetaDataPlusAbsolutePath>();
                    break;
                case ObjectTypes.File_Incident:
                    retVal = GenericCreate<EmailStorageAbsolutePath>();
                    break;
            }

            return retVal;
        }

     
        /// <summary>
        /// Creates the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public WebDavAbsolutePath Create(object obj1, object obj2)
        {
            ObjectTypes storageType = (ObjectTypes)obj1;

            WebDavAbsolutePath retVal = null;
            switch (storageType)
            {
                case ObjectTypes.File_FileStorage:
                        retVal = GenericCreate<FileStorageAbsolutePath>(obj2);
                    break;
                case ObjectTypes.File_MetaData:
                      retVal = GenericCreate<MetaDataAbsolutePath>(obj2);
                    break;
                case ObjectTypes.File_MetaDataPlus:
                        retVal = GenericCreate<MetaDataPlusAbsolutePath>(obj2);
                    break;
                case ObjectTypes.File_Incident:
                        retVal = GenericCreate<EmailStorageAbsolutePath>(obj2);
                    break;
            }

            return retVal;
        }

        private T1 GenericCreate<T1>()
        {
            return Activator.CreateInstance<T1>();
        }

        private T1 GenericCreate<T1>(object param)
        {
            return (T1)Activator.CreateInstance(typeof(T1), param);
        }

        #endregion
    }
}
