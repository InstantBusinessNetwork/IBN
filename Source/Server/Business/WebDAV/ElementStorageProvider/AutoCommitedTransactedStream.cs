using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Business.WebDAV.ElementStorageProvider
{
	internal class AutoCommitedTransactedStream : Stream
	{
		private Stream _stream;
		private TransactionScope _tran;
		private bool _closed;
		private long _contentLength = 0;

		public TransactionScope Transaction
		{
			get
			{
				return _tran;
			}
		}

		public bool IsClosed
		{
			get
			{
				return _closed;
			}
		}

		public long ContentLength
		{
			get
			{
				return _contentLength;
			}
		}

		public Stream InnerStream
		{
			get
			{
				return _stream;
			}

			set
			{
				if (_closed)
					throw new ArgumentException("closed");

				_stream = value;
			}
		}

		public AutoCommitedTransactedStream(TransactionScope tran, long contentLength)
		{
			_tran = tran;
			_closed = false;
			_contentLength = contentLength;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !IsClosed)
			{
				if (Transaction != null)
				{
					//if recieve not all content do nothing
					if (InnerStream.Length == ContentLength)
					{
						CommitTransaction();
					}
					Transaction.Dispose();
				}

				if (InnerStream != null)
				{
					//clean stream
					InnerStream.Close();
				}
				_tran = null;
				_stream = null;
				_closed = true;
			}
		}

		protected virtual void CommitTransaction()
		{
			//commit transaction
			Transaction.Commit();
		}

		public override bool CanRead
		{
			get { return InnerStream.CanRead; }
		}

		public override bool CanSeek
		{
			get { return InnerStream.CanSeek; }
		}

		public override bool CanWrite
		{
			get { return InnerStream.CanWrite; }
		}

		public override void Flush()
		{
			InnerStream.Flush();
		}

		public override long Length
		{
			get { return InnerStream.Length; }
		}

		public override long Position
		{
			get
			{
				return InnerStream.Position;
			}
			set
			{
				InnerStream.Position = value;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return InnerStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return InnerStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			InnerStream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			InnerStream.Write(buffer, offset, count);
		}
	}
}
