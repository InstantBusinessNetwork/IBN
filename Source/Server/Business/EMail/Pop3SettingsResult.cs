using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.EMail
{
    public enum Pop3SettingsResult
    {
        None = 0,
        ServerName = 1,
        Pop3User = 2,
        Pop3Password = 4,
        AllOk = ServerName | Pop3User | Pop3Password
    }
}
