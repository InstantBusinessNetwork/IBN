using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Net.Mail
{
    public class Pop3UIDInfoList : CollectionBase
    {
        internal Pop3UIDInfoList()
        {
        }

        internal void Add(Pop3UIDInfo uid)
        {
            this.List.Add(uid);
        }

        public Pop3UIDInfo this[int index]
        {
            get
            {
                return (Pop3UIDInfo)this.InnerList[index];
            }
        }
    }
}
