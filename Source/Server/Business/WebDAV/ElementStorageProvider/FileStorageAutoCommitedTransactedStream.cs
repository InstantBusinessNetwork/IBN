using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Business.WebDAV.ElementStorageProvider
{
    /// <summary>
    /// Stream wrapper in transaction scope and dispose auto commited
    /// </summary>
	internal class FileStorageAutoCommitedTransactedStream : AutoCommitedTransactedStream
    {
        private string _containerKey;
        private int _parentDirectoryId;
        private string _fileName;
		
		public FileStorageAutoCommitedTransactedStream(TransactionScope tran, long contentLength)
            :this(tran, null, -1, null, contentLength)
        {
        }

        public FileStorageAutoCommitedTransactedStream(TransactionScope tran, string containerKey, 
                                            int parentDirectoryId, string fileName, long contentLength)
			:base(tran, contentLength)
        {
            _containerKey = containerKey;
            _parentDirectoryId = parentDirectoryId;
            _fileName = fileName;
        }

        public bool OpenedToWrite
        {
            get
            {
                return !string.IsNullOrEmpty(_fileName);
            }
        }

		protected override void CommitTransaction()
		{
			if (OpenedToWrite)
			{
				Mediachase.IBN.Business.ControlSystem.BaseIbnContainer bic =
										Mediachase.IBN.Business.ControlSystem.BaseIbnContainer.Create("FileLibrary", _containerKey);
				Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");
				if (InnerStream.CanSeek)
				{
					InnerStream.Seek(0, SeekOrigin.Begin);
				}

				fs.SaveFile(_parentDirectoryId, _fileName, InnerStream);

				base.CommitTransaction();
			}

		}
    }
}
