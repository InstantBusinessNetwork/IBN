//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------

namespace Mediachase.Web.UI.WebControls.Design
{
    /// <summary>
    /// Provides an editor for visually picking an image URL.
    /// </summary>
    public class ObjectImageUrlEditor : ObjectUrlEditor
    {
        /// <summary>
        /// Gets the caption for the URL.
        /// </summary>
        protected override string Caption
        {
            get { return DesignUtil.GetStringResource("ImageUrlCaption"); }
        }

        /// <summary>
        /// Gets the filter to use.
        /// </summary>
        protected override string Filter
        {
            get { return DesignUtil.GetStringResource("ImageUrlFilter"); }
        }
    }
}
