using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
    public static class DbEMailTempFile
    {
        public static void CleanUp()
        {
            DbHelper2.RunSp("EMailTempFileCleanUp");
        }
    }
}
