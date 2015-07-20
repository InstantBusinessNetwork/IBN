using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.WebDAV.Definition
{
    /// <summary>
    /// 
    /// </summary>
    internal class MetaDataAbsolutePath : WebDavAbsolutePath
    {

        private Guid _fileUID;
        private string _fileName;


        /// <summary>
        /// Initializes a new instance of the <see cref="MetaDataAbsolutePath"/> class.
        /// </summary>
        public MetaDataAbsolutePath()
            :base(ObjectTypes.File_MetaData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaDataAbsolutePath"/> class.
        /// </summary>
        public MetaDataAbsolutePath(string str)
            : this()
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("str");

            string[] parts = str.Split(SEPARATOR);
            if (parts.Length != 1)
                throw new ArgumentException("str");

            _fileUID = new Guid(parts[0]);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaDataAbsolutePath"/> class.
        /// </summary>
        /// <param name="byteArr">The byte arr.</param>
        public MetaDataAbsolutePath(byte[] byteArr)
            : this()
        {
			if (byteArr.Length < 16)
				throw new Exception("byteArr");

			byte[] guidByteArr = new byte[16];
			Array.Copy(byteArr, guidByteArr, 16);
            _fileUID = new Guid(guidByteArr);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Join(SEPARATOR.ToString(), new string[] { base.ToString(), FileUID.ToString() });
        }

        /// <summary>
        /// Gets the byte array.
        /// </summary>
        /// <returns></returns>
        public override byte[] GetByteArray()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(base.GetByteArray());
            retVal.AddRange(FileUID.ToByteArray());
            
            return retVal.ToArray();
        }
        public override int UniqueId
        {
            get
            {
                return _fileUID.GetHashCode();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override string FileName
        {
            get { return _fileName;  }
            set { _fileName = value; }
        }

        public Guid FileUID
        {
            get
            {
                return _fileUID;
            }
            set
            {
                _fileUID = value;
            }
        }
  
    }
}
