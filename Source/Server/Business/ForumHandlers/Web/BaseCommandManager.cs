using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Runtime.Remoting;
using System.Collections;
using System.Reflection;
using Mediachase.IBN.Business;

namespace Mediachase.IBN.Business
{
    public class BaseCommandManager : WebControl
    {
        #region event: OnValidate
        public event ValidateHandler Validate;

        protected virtual void OnValidate(Object Sender, ValidateArgs e)
        {
            if (Validate != null)
            {
                foreach (ValidateHandler item in Validate.GetInvocationList())
                {
                    item(this, e);

                    if (e.IsBreak)
                        return;
                }
                //Validate(Sender, e);
            }
        }

        private ValidateStruct RaiseValidateHandler(Mediachase.Forum.Node Node, string CommandUid, bool IsVisible, bool IsEnabled, bool IsBreak)
        {
            ValidateArgs args = new ValidateArgs(Node, CommandUid, IsVisible, IsEnabled, IsBreak);
            OnValidate(this, args);
            return new ValidateStruct(args.IsVisible, args.IsEnabled);
        }

        public virtual ValidateStruct RunValidate(Mediachase.Forum.Node Node, string CommandUid)
        {
            return RaiseValidateHandler(Node, CommandUid, false, false, false);
        }
        #endregion

        #region event: OnCommand
        public event CommandHandler InvokeCommand;
        protected virtual void OnInvokeCommand(InvokeCommandArgs e)
        {
            if (InvokeCommand != null)
            {
                //InvokeCommand(Sender, e);
                foreach (CommandHandler item in InvokeCommand.GetInvocationList())
                {
                    item(this, e);

                    if (e.IsBreak)
                        return;
                }
            }
        }

        private void RaiseInvokeCommand(Mediachase.Forum.Node Node, string CommandUid, bool IsBreak)
        {
            InvokeCommandArgs args = new InvokeCommandArgs(Node, CommandUid, IsBreak);
            OnInvokeCommand(args);
        }

        public virtual void RunInvokeCommand(Mediachase.Forum.Node Node, string CommandUid)
        {
            RaiseInvokeCommand(Node, CommandUid, false);
        }
        #endregion

        private Hashtable dllInsure = null;

        #region GetDelegatesCounter
        private int GetDelegatesCounter(Delegate handler)
        {
            if (handler == null)
                return 0;
            else
                return handler.GetInvocationList().Length;
        } 
        #endregion

        #region InitManager
        private void InitManager()
        {
            //TO DO: load all DLLs from Web.Config
            ArrayList aList;
            aList = (ArrayList)System.Configuration.ConfigurationManager.GetSection("Mediachase.CommandManager");

            if (!Page.IsPostBack)
                dllInsure = new Hashtable();

            //Get control assembly 
            foreach (string path in aList)
            {
                try
                {
                    Assembly currentAssembly = null;

                    if (!dllInsure.ContainsKey(path))
                    {
                        currentAssembly = Assembly.Load(path);
                    }
                    else
                    {
                        if (dllInsure[path] == null)
                        {
                            continue;
                        }
                        else
                        {
                            currentAssembly = (Assembly)dllInsure[path];
                        }
                    }

                    int validateCounter = GetDelegatesCounter(this.Validate);
                    int commandCounter = GetDelegatesCounter(this.InvokeCommand);

                    foreach (Type typeItem in currentAssembly.GetTypes())
                    {
                        //TO DO: Get all classes that invokes interface ICommandHandler
                      if (typeItem.GetInterface("Mediachase.IBN.Business.ICommandHandler") != null)
                        {
                            //Модули подписываются на нужные events
                            ((Mediachase.IBN.Business.ICommandHandler)Activator.CreateInstance(typeItem)).Init(this);
                        }
                    }

                    if (GetDelegatesCounter(this.InvokeCommand) == commandCounter && GetDelegatesCounter(this.Validate) == validateCounter)
                    {
                        if (!dllInsure.ContainsKey(path))
                            dllInsure.Add(path, null);
                    }
                    else
                    {
                        if (!dllInsure.ContainsKey(path))
                            dllInsure.Add(path, currentAssembly);
                    }
                }

                catch
                {
                  
                }

            }
        } 
        #endregion

        #region OnLoad
        protected override void OnLoad(EventArgs e)
        {
            InitManager();
            base.OnLoad(e);
        } 
        #endregion

        #region OnInit
        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this);
            base.OnInit(e);
        } 
        #endregion

        #region SaveControlState
        protected override object SaveControlState()
        {
            return dllInsure;
        } 
        #endregion

        #region LoadControlState
        protected override void LoadControlState(object savedState)
        {
            if (savedState != null)
            {
                dllInsure = (Hashtable)savedState;
            }
        } 
        #endregion
    }
}