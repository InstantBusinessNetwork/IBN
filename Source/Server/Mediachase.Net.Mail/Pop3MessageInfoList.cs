using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Net.Mail
{
    public class Pop3MessageInfoList : CollectionBase
    {
        internal Pop3MessageInfoList()
        {
        }

        internal void Add(Pop3MessageInfo message)
        {
            this.List.Add(message);
        }

        public Pop3MessageInfo this[int index]
        {
            get
            {
                return (Pop3MessageInfo)this.InnerList[index];
            }
        }
    }
}
