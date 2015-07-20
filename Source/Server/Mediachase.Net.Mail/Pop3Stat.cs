using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Net.Mail
{
    public class Pop3Stat
    {
        private int _MessageCout;
        private int _MailboxSize;

        internal Pop3Stat(int MessageCout, int MailboxSize)
        {
            this._MessageCout = MessageCout;
            this._MailboxSize = MailboxSize;
        }

        public int MessageCout { get { return _MessageCout; } }
        public int MailboxSize { get { return _MailboxSize; } }
    }
}
