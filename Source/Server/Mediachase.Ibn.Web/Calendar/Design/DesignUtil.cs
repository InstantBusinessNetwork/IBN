//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------

namespace Mediachase.Web.UI.WebControls.Design
{
    using System;
    using System.ComponentModel;

    /// <summary>
    ///  Utility class with various useful static functions.
    /// </summary>
    public class DesignUtil
    {
        private static System.Resources.ResourceManager _ResourceManager = null;

        public static System.Resources.ResourceManager GetResourceManager()
        {
            if (_ResourceManager == null)
            {
                Type ourType = typeof(DesignUtil);
                _ResourceManager = new System.Resources.ResourceManager("Mediachase.Ibn.Web.Calendar.Resources.Design", typeof(DesignUtil).Assembly);
            }

            return _ResourceManager;
        }

        public static string GetStringResource(string name)
        {
			try{return (string)GetResourceManager().GetObject(name);}
			catch(System.Exception e){
				Type ourType = typeof(DesignUtil);
				return "DLL: " + ourType.Namespace + "," + ourType.Module.Assembly + ". Couldnot find following resource '" + name + "'. " + e.ToString();}
            
        }

        /// <summary>
        /// Given an object and a property name, retrieves the property descriptor.
        /// </summary>
        /// <param name="obj">The source object</param>
        /// <param name="propName">The property name</param>
        /// <returns>The PropertyDescriptor of the property on the object, or null if not found.</returns>
        public static PropertyDescriptor GetPropertyDescriptor(object obj, string propName)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);

            if (props != null)
            {
                foreach (PropertyDescriptor propDesc in props)
                {
                    if (propDesc.Name == propName)
                    {
                        return propDesc;
                    }
                }
            }

            return null;
        }
    }
}
