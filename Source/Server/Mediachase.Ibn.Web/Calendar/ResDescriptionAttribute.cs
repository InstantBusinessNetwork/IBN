//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------

namespace Mediachase.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
	using Mediachase.Web.UI.WebControls.Util;

    /// <summary>
    /// Implements the DescriptionAttribute except that the parameter is a resource name.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple=false, Inherited=true)]
    public class ResDescriptionAttribute : DescriptionAttribute
    {
        /// <summary>
        /// Initializes a new instance of a ResDescriptionAttribute.
        /// </summary>
        /// <param name="resourceName">The name of the string resource.</param>
        public ResDescriptionAttribute(string resourceName): base()
        {
            DescriptionValue = Helper.GetStringResource(resourceName);
        }
    }
}
