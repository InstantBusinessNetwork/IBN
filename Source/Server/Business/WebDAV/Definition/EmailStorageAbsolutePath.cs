using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.WebDAV.Definition
{
    internal class EmailStorageAbsolutePath : WebDavAbsolutePath
    {
        private int _emailMsgId;
        private int _attachmentIndex;
        private string _fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailStorageAbsolutePath"/> class.
        /// </summary>
        public EmailStorageAbsolutePath()
            : base(ObjectTypes.File_Incident)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailStorageAbsolutePath"/> class.
        /// </summary>
        /// <param name="str">The STR.</param>
        public EmailStorageAbsolutePath(string str)
            : this()
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("str");

            string[] parts = str.Split(SEPARATOR);
            if (parts.Length != 2)
                throw new ArgumentException("str");

            _emailMsgId = Convert.ToInt32(parts[0]);
            _attachmentIndex = Convert.ToInt32(parts[1]);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailStorageAbsolutePath"/> class.
        /// </summary>
        /// <param name="byteArr">The byte arr.</param>
        public EmailStorageAbsolutePath(byte[] byteArr)
            : this()
        {
            if (byteArr.Length < 8)
                throw new ArgumentException("byteArr");

            _emailMsgId = BitConverter.ToInt32(byteArr, 0);
            _attachmentIndex = BitConverter.ToInt32(byteArr, 4);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Join(SEPARATOR.ToString(), new string[] { base.ToString(), EmailMsgId.ToString(), EmailAttachmentIndex.ToString() });
        }

        /// <summary>
        /// Gets the byte array.
        /// </summary>
        /// <returns></returns>
        public override byte[] GetByteArray()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(base.GetByteArray());
            retVal.AddRange(BitConverter.GetBytes(EmailMsgId));
            retVal.AddRange(BitConverter.GetBytes(EmailAttachmentIndex));
            return retVal.ToArray();
        }

        public override int UniqueId
        {
            get
            {
                return (_emailMsgId << 8) | _attachmentIndex;
            }
            set
            {
            }
        }

        public override string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        public int EmailMsgId
        {
            get { return _emailMsgId; }
            set { _emailMsgId = value; }
        }

        public int EmailAttachmentIndex
        {
            get { return _attachmentIndex; }
            set { _attachmentIndex = value; }
        }
    }
}
