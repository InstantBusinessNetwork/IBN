using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Net.Mail
{
    public class Pop3MessageInfo
    {
        private int _Id;
        private int _Size;
        private string _UID;

        internal Pop3MessageInfo(int Id, string UID, int Size)
        {
            this._Id = Id;
            this._UID = UID;
            this._Size = Size;
        }

        public int ID { get { return _Id; } }
        public string UID { get { return _UID; } set { _UID = value; } }
        public int Size { get { return _Size; } }
    }

}
