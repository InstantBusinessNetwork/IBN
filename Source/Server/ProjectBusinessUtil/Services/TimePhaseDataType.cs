using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBusinessUtil.Services
{
    public class TimePhasedDataType
    {
        public enum TimePhaseType
        {
            AssignmentRemainingWork = 0x1,
            AssignmentActualWork    = 0x2

        }
        public enum TimePhaseUnit
        {
            Minute  = 0x0,
            Hours   = 0x1,
            Day     = 0x2,
            Week    = 0x3,
            Mounth  = 0x5,
            Year    = 0x8
        }
        public int Type;
        public int UID;
        public long Start;
        public long Finish;
        public long Value;
        public TimePhaseUnit Unit;
    };
}
