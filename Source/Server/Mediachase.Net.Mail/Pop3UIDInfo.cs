using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Net.Mail
{
    public class Pop3UIDInfo
    {
        private int _Id;
        private string _UID;

        internal Pop3UIDInfo(int Id, string UID)
        {
            this._Id = Id;
            this._UID = UID;
        }

        public int ID { get { return _Id; } }
        public string UID { get { return _UID; } set { _UID = value; } }
    }

}
