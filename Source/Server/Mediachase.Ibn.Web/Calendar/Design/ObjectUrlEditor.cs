//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------

namespace Mediachase.Web.UI.WebControls.Design
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Windows.Forms.Design;
    using System.Web.UI.Design;
    using Mediachase.Web.UI.WebControls;

    /// <summary>
    /// Provides an editor for visually picking an URL.
    /// </summary>
    public class ObjectUrlEditor : UITypeEditor 
    {
        /// <summary>
        /// Gets the caption for the URL.
        /// </summary>
        protected virtual string Caption
        {
            get { return DesignUtil.GetStringResource("UrlCaption"); }
        }

        /// <summary>
        /// Gets the filter to use.
        /// </summary>
        protected virtual string Filter
        {
            get { return DesignUtil.GetStringResource("UrlFilter"); }
        }

        /// <summary>
        /// Gets the options for the URL picker.
        /// </summary>
        protected virtual UrlBuilderOptions Options
        {
            get { return UrlBuilderOptions.None; }
        }

        /// <summary>
        /// Gets the editting style of the Edit method.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The style</returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// Edits the specified object value using the editor style provided by GetEditorStyle.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="provider">Provider</param>
        /// <param name="value">The value to edit</param>
        /// <returns>The editted value</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null) 
                {
                    object node = null;

                    if (context.Instance is BaseChildNode)
                    {
                        node = context.Instance;
                    }
                    else if (context.Instance is object[])
                    {
                        object[] components = (object[])context.Instance;
                        if (components[0] is BaseChildNode)
                        {
                            node = components[0];
                        }
                    }

                    while ((node != null) && (node is BaseChildNode))
                    {
                        node = ((BaseChildNode)node).Parent;
                    }

                    if ((node != null) && (node is IComponent))
                    {
                        string url = UrlBuilder.BuildUrl((IComponent)node, null, (string)value, Caption, Filter, Options);
                        if (url != null) 
                        {
                            value = url;
                        }
                    }
                }
            }

            return value;
        }
    }
}
