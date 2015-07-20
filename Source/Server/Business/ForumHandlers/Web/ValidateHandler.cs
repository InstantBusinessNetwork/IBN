using System;
using System.Data;
using System.Configuration;
using System.Collections;

namespace Mediachase.IBN.Business
{
    public delegate void ValidateHandler(Object Sender, ValidateArgs e);

    #region ValidateArgs
    public class ValidateArgs : EventArgs
    {
        #region prop: Node
        private Mediachase.Forum.Node node;

        public Mediachase.Forum.Node Node
        {
            get { return node; }
            set { node = value; }
        }
        #endregion

        #region prop: CommandUid
        private string commandUid;

        public string CommandUid
        {
            get { return commandUid; }
            set { commandUid = value; }
        }
        #endregion

        #region prop IsVisible
        private bool isVisible;

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }
        #endregion

        #region prop: IsEnabled
        private bool isEnabled;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }
        #endregion

        #region prop: IsBreak
        private bool isBreak;

        public bool IsBreak
        {
            get { return isBreak; }
            set { isBreak = value; }
        }
        #endregion

        public ValidateArgs(Mediachase.Forum.Node Node, string CommandUid, bool IsVisible, bool IsEnabled, bool IsBreak)
        {
            this.Node = Node;
            this.CommandUid = CommandUid;
            this.IsVisible = IsVisible;
            this.IsEnabled = IsEnabled;
            this.IsBreak = IsBreak;
        }
    }
    #endregion
}