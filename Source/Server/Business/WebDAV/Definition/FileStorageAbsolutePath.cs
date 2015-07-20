using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.WebDAV.Definition
{
    internal class FileStorageAbsolutePath : WebDavAbsolutePath
    {
        private int _fileId;
        private int _historyId = -1;
        private string _fileName;

        public FileStorageAbsolutePath()
            :base(ObjectTypes.File_FileStorage)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageAbsolutePath"/> class.
        /// </summary>
        /// <param name="str">The STR.</param>
        public FileStorageAbsolutePath(string str)
            : this()
        {

            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("str");

            string[] parts = str.Split(SEPARATOR);
            if (parts.Length != 2)
                throw new ArgumentException("str");

            _fileId = Convert.ToInt32(parts[0]);
            _historyId = Convert.ToInt32(parts[1]);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageAbsolutePath"/> class.
        /// </summary>
        /// <param name="byteArr">The byte arr.</param>
        public FileStorageAbsolutePath(byte [] byteArr)
            : this()
        {
            _fileId = BitConverter.ToInt32(byteArr, 0);
			bool historyPresent = BitConverter.ToBoolean(byteArr, 4);
			if (historyPresent)
			{
				_historyId = BitConverter.ToInt32(byteArr, 5);
			}
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
			//вставляем признак наличия истории
			retVal.Add(Convert.ToByte(HistoryId != -1));
			if (HistoryId != -1)
			{
				retVal.AddRange(BitConverter.GetBytes(HistoryId));
            }
            return retVal.ToArray();
        }
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Join(SEPARATOR.ToString(), new string[] { base.ToString(), UniqueId.ToString(), HistoryId.ToString() });
        }

      
        /// <summary>
        /// Gets or sets the object id.
        /// </summary>
        /// <value>The object id.</value>
        public override int UniqueId
        {
            get { return _fileId; }
            set { _fileId = value; }
        }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        /// <value>The name of the object.</value>
        public override string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public bool IsHistory
        {
            get { return _historyId != -1; }
        }

        public int HistoryId
        {
            get { return _historyId; }
            set { _historyId = value; }
        }
    }
}
