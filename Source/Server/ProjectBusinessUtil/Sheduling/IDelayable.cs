using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Sheduling
{
    public interface IDelayable
    {
        long CalcTotalDelay();

         long Delay { get; set; }


    }
}
