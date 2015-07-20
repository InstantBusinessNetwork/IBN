using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.WebDAV.Definition
{

    /// <summary>
    /// Дополнительная информация о файле, предназначенная для вставки 
    /// </summary>
    internal class MetaDataPlusAbsolutePath : WebDavAbsolutePath
    {
        string _metaObjectType;
        string _metaFieldName;
        int _metaObjectId;
        int _metaFileId;
        string _metaFileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaDataPlusAbsolutePath"/> class.
        /// </summary>
        public MetaDataPlusAbsolutePath()
            :base(ObjectTypes.File_MetaDataPlus)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaDataPlusAbsolutePath"/> class.
        /// </summary>
        public MetaDataPlusAbsolutePath(string str)
            : this()
        {
            if (String.IsNullOrEmpty(str))
                throw new ArgumentException(str);

            string[] parts = str.Split(SEPARATOR);
            if (parts.Length < 4)
                throw new ArgumentException("str");

            UniqueId = Convert.ToInt32(parts[0]);
            MetaObjectType = parts[1];
            MetaObjectId = Convert.ToInt32(parts[2]);
            MetaFieldName = parts[3];

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaDataPlusAbsolutePath"/> class.
        /// </summary>
        /// <param name="byteArr">The byte arr.</param>
        public MetaDataPlusAbsolutePath(byte [] byteArr)
            : this()
        {
            UniqueId = BitConverter.ToInt32(byteArr, 0);
            MetaObjectId = BitConverter.ToInt32(byteArr, 4);
            string str = Encoding.ASCII.GetString(byteArr, 8, byteArr.Length - 8);
            string[] parts = str.Split(SEPARATOR);
            if(parts.Length < 2)
                throw new ArgumentException("byteArr");

            MetaObjectType = parts[0];
            MetaFieldName = parts[1];
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Join(SEPARATOR.ToString(), new string[] { base.ToString(), UniqueId.ToString(), 
                                                                   MetaObjectType, MetaObjectId.ToString(), MetaFieldName.ToString() }) + SEPARATOR;
        }


        /// <summary>
        /// Gets the byte array.
        /// </summary>
        /// <returns></returns>
        public override byte[] GetByteArray()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(base.GetByteArray());
            retVal.AddRange(BitConverter.GetBytes(UniqueId));
            retVal.AddRange(BitConverter.GetBytes(MetaObjectId));
            retVal.AddRange(Encoding.ASCII.GetBytes(MetaObjectType + SEPARATOR + MetaFieldName + SEPARATOR));
            
            return retVal.ToArray();
        }
        public override int UniqueId
        {
            get { return _metaFileId; }
            set { _metaFileId = value; }
        }

        public override string FileName
        {
            get { return _metaFileName; }
            set { _metaFileName = value; }
        }


        public string MetaObjectType
        {
            get { return _metaObjectType; }
             set { _metaObjectType = value; }
        }
        public string MetaFieldName
        {
            get { return _metaFieldName; }
             set { _metaFieldName = value; }
        }
        public int MetaObjectId
        {
            get { return _metaObjectId; }
             set { _metaObjectId = value; }

        }
    }
}
