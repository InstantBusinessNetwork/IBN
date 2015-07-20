//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------

namespace Mediachase.Web.UI.WebControls
{
    using System;
    using System.Web.UI;
    using System.Collections.Specialized;
	using Mediachase.Web.UI.WebControls.Util;


    /// <summary>
    /// Implements the IPostBackEventHandler and IPostBackDataHandler interfaces.
    /// Creates a hidden helper, if needed, and allows controls to set and receive
    /// the data from that helper.
    /// </summary>
    public abstract class BasePostBackControl : BaseRichControl, IPostBackEventHandler, IPostBackDataHandler
    {
        private string _szHelperID;
        private string _szHelperData = String.Empty;

        /// <summary>
        /// Processes post back data for the server control given the data from the hidden helper.
        /// </summary>
        /// <param name="szData">The data from the hidden helper</param>
        /// <returns>true if the server control's state changes as a result of the post back; otherwise false.</returns>
        protected virtual bool ProcessData(string szData)
        {
            return false;
        }

        /// <summary>
        /// Processes post back data for a server control.
        /// </summary>
        /// <param name="postDataKey">The key identifier for the control.</param>
        /// <param name="postCollection">The collection of all incoming name values.</param>
        /// <returns>true if the server control's state changes as a result of the post back; otherwise false.</returns>
        protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            //EnsureChildControls();

            if (NeedHelper)
            {
                string szData = postCollection[HelperID];
                if (szData != null)
                {
                    return ProcessData(szData);
                }
            }

            return false;
        }

        /// <summary>
        /// Signals the server control object to notify the ASP.NET application that the state of the control has changed.
        /// </summary>
        protected virtual void RaisePostDataChangedEvent()
        {
        }

        /// <summary>
        /// Enables a server control to process an event raised when a form is posted to the server.
        /// </summary>
        /// <param name="eventArgument">A String that represents an optional event argument to be passed to the event handler.</param>
        protected virtual void RaisePostBackEvent(string eventArgument)
        {
        }

        /// <summary>
        /// Returns a true value to indicate that a hidden helper is needed by this control.
        /// </summary>
        protected virtual bool NeedHelper
        {
            get { return IsUpLevelBrowser; }
        }

        /// <summary>
        /// The ID of the hidden helper.
        /// </summary>
        protected virtual string HelperID
        {
            get
            {
                if (_szHelperID == null)
                {
                    _szHelperID = "__" + ClientID + "_State__";
                }

                return _szHelperID;
            }
        }

        /// <summary>
        /// Client-side script ID of the hidden helper.
        /// </summary>
                  public virtual string ClientHelperID
        {
            get
            {
                Control form = Helper.FindForm(this);
                if (form != null)
                {
                    return "document." + form.ClientID + "." + HelperID;
                }

                return HelperID;
            }
        }

        /// <summary>
        /// The data to store inside the hidden helper.
        /// </summary>
        protected virtual string HelperData
        {
            get { return _szHelperData; }
            set { _szHelperData = value; }
        }


        // -------------------------------------------------------------------
        // Implementation of IPostBackDataHandler and IPostBackEventHandler
        // Override protected instance methods (at top) instead.
        // -------------------------------------------------------------------

        /// <summary>
        /// Processes post back data for a server control.
        /// </summary>
        /// <param name="postDataKey">The key identifier for the control.</param>
        /// <param name="postCollection">The collection of all incoming name values.</param>
        /// <returns>true if the server control's state changes as a result of the post back; otherwise false.</returns>
        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            return ((BasePostBackControl)this).LoadPostData(postDataKey, postCollection);
        }

        /// <summary>
        /// Signals the server control object to notify the ASP.NET application that the state of the control has changed.
        /// </summary>
        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            ((BasePostBackControl)this).RaisePostDataChangedEvent();
        }

        /// <summary>
        /// Enables a server control to process an event raised when a form is posted to the server.
        /// </summary>
        /// <param name="eventArgument">A String that represents an optional event argument to be passed to the event handler.</param>
        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            ((BasePostBackControl)this).RaisePostBackEvent(eventArgument);
        }
    }
}
