using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Mediachase.MetaDataPlus;

namespace Mediachase.IBN.Business.WebDAV.ElementStorageProvider
{
    internal class MetaFileStream : Stream
    {
        MetaObject _metaObject;
        MetaFile _metaFile;
        string _metaFieldName;
        MemoryStream _stream;
		long _contentLength = 0;

        public MetaFileStream(MetaObject mo, MetaFile mf, string metaFieldName, long contentLength)
        {
            _metaObject = mo;
            _metaFile = mf;
            _metaFieldName = metaFieldName;
            _stream = new MemoryStream();
			_contentLength = contentLength;
        }
        
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (_stream != null && _stream.Length == _contentLength)
                {
                    _metaFile.Buffer = _stream.GetBuffer();
                    //force changes
                    _metaObject[_metaFieldName] = _metaFile;
					_stream.Close();
                }
			
                _metaObject.AcceptChanges();
            }
        }
     
        public Stream InnerStream
        {
            get
            {
                return _stream;
            }

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
