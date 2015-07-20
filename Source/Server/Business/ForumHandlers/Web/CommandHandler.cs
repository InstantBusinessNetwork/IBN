using System;
using System.Data;
using System.Configuration;
using System.Collections;

namespace Mediachase.IBN.Business
{
    public delegate void CommandHandler(Object Sender, InvokeCommandArgs e);

    #region CommandManagerArgs
    public class InvokeCommandArgs : EventArgs
    {
        public InvokeCommandArgs(Mediachase.Forum.Node Node, string CommandUid, bool IsBreak)
        {
            this.CommandUid = CommandUid;
            this.Node = Node;
            this.IsBreak = IsBreak;
        }

        public readonly string CommandUid;
        public Mediachase.Forum.Node Node;
        public bool IsBreak;
    }
    #endregion
}