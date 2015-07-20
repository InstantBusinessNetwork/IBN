using System;
using System.Text;
using System.IO;
using Mediachase.Net.Mail;

namespace Mediachase.IBN.Business.EMail
{
    internal class Pop3MessageModifier
    {
        private byte[] _pop3Message;
        private int _bodyOffset = 0;
        private Rfc822HeaderCollection _headers = new Rfc822HeaderCollection();

        public Pop3MessageModifier(byte[] pop3Message)
        {
            _pop3Message = pop3Message;
            _headers.ParseHeaders(pop3Message, pop3Message.Length, ref _bodyOffset);
        }

        public Rfc822HeaderCollection Headers
        {
            get
            {
                return _headers;
            }
        }

        public byte[] GetBuffer()
        {
            MemoryStream ms = new MemoryStream(1024 * 10);

            byte[] bHeaders = _headers.ToByteArray();
            ms.Write(bHeaders, 0, bHeaders.Length);

            ms.Write(_pop3Message, _bodyOffset, _pop3Message.Length - _bodyOffset);

            ms.Capacity = (int)ms.Length;
            return ms.GetBuffer();
        }
    }
}
