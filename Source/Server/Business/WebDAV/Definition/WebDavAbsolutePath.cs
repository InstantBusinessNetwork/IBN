using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Business.WebDAV.Util;

namespace Mediachase.IBN.Business.WebDAV.Definition
{
    /// <summary>
    /// Испльзуется в WebDavServer (StorageProvider и Request/Response handler)как альтернатива абсолютному пути
    /// </summary>
    public abstract class WebDavAbsolutePath
    {
        private ObjectTypes _storageType;
        protected const char SEPARATOR = ':';
      
        protected WebDavAbsolutePath(ObjectTypes storageType)
        {
            _storageType = storageType;
        }

        public static WebDavAbsolutePath CreateInstance(ObjectTypes storageType)
        {
            WebDavAbstractFactory factory = new WebDavAbstractFactory();
            return factory.Create<WebDavAbsolutePath>(storageType);
        }
        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="storageType">Type of the storage.</param>
        /// <returns></returns>
        public static WebDavAbsolutePath CreateInstance(ObjectTypes storageType, string str)
        {
            WebDavAbstractFactory factory = new WebDavAbstractFactory();
            return factory.Create<WebDavAbsolutePath>(storageType, str);
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="storageType">Type of the storage.</param>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static WebDavAbsolutePath CreateInstance(ObjectTypes storageType, byte [] byteArr)
        {
            WebDavAbstractFactory factory = new WebDavAbstractFactory();
            return factory.Create<WebDavAbsolutePath>(storageType, byteArr);
        }
        /// <summary>
        /// Parses the specified STR.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static WebDavAbsolutePath Parse(string str)
        {
            WebDavAbsolutePath retVal = null;
            try
            {
                ObjectTypes storageType = ObjectTypes.UNDEFINED;
                string strStorageType = str.Substring(0, str.IndexOf(SEPARATOR));
                if (!String.IsNullOrEmpty(strStorageType))
                {
                    storageType = (ObjectTypes)Convert.ToInt32(strStorageType);
                    retVal = CreateInstance(storageType, str.Substring(strStorageType.Length + 1));
                    //retVal.Initialize(str.Substring(strStorageType.Length + 1));
                }
            }
            catch (System.Exception)
            {
            }

            return retVal;
        }

        /// <summary>
        /// Parses the specified byte arr.
        /// </summary>
        /// <param name="byteArr">The byte arr.</param>
        /// <returns></returns>
        public static WebDavAbsolutePath Parse(byte[] byteArr)
        {
            WebDavAbsolutePath retVal = null;
            try
            {
                ObjectTypes storageType = ObjectTypes.UNDEFINED;

                storageType = (ObjectTypes)byteArr[0];
                //remove self info
                byte[] arr = new byte[byteArr.Length - 1];
                if (arr.Length > 1)
                {
                    Array.Copy(byteArr, 1, arr, 0, arr.Length);
                    retVal = CreateInstance(storageType, arr);
                }

            }
            catch (System.Exception)
            {
            }

            return retVal;
        }
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return ((int)StorageType).ToString();
        }


        public virtual byte[] GetByteArray()
        {
            return new byte[] { (byte)StorageType };
        }

        public ObjectTypes StorageType
        {
            get
            {
                return _storageType;
            }
        }
      
        public abstract int UniqueId { get; set;}
        public abstract string FileName { get; set; }
        
    }
}
