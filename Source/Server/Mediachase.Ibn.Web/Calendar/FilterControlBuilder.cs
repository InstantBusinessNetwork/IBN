//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------

namespace Mediachase.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Web.UI;
	using Mediachase.Web.UI.WebControls.Util;

    /// <summary>
    /// Subclass this class to create a ControlBuilder that
    /// only allows specific child types/tags and strips out any
    /// literal strings as well.
    /// </summary>
    public abstract class FilterControlBuilder : ControlBuilder
    {

        /// <summary> 
        /// Implement this method and use Add to add
        /// key value pairs that translate a tag name to
        /// a type.
        ///
        /// Example: Add("tagname", typeof(TagType));
        ///
        /// </summary>
        protected abstract void FillTagTypeTable();


        private Hashtable _TagTypeTable;    // Tag to Type relationship table
        
        /// <summary>
        /// Initializes a new instance of a FilterControlBuilder.
        /// </summary>
        public FilterControlBuilder() : base()
        {
            // Create the table
            _TagTypeTable = new Hashtable();

            // Fill the table with tag to type relationships
            FillTagTypeTable();
        }

        /// <summary>
        /// Adds a tagname to type entry.
        /// </summary>
        /// <param name="tagName">The tag name.</param>
        /// <param name="type">The type.</param>
        public void Add(string tagName, Type type)
        {
            _TagTypeTable.Add(tagName.ToLower(), type);
        }

        /// <summary>
        /// Determines a type given a tag name.
        /// </summary>
        /// <param name="tagName">The tagname.</param>
        /// <param name="attribs">Attributes.</param>
        /// <returns>The type of the tag.</returns>
        public override Type GetChildControlType(string tagName, IDictionary attribs)
        {
            // Let the base class have the tagname
            Type baseType = base.GetChildControlType(tagName, attribs);
            if (baseType != null)
            {
                // If the type returned is valid, then return it
                if (_TagTypeTable.ContainsValue(baseType))
                {
                    return baseType;
                }
            }

            // Allows children without runat=server to be added
            // and to limit to specific types

            string szTagName = tagName.ToLower();
            int colon = szTagName.IndexOf(':');
            if ((colon >= 0) && (colon < (szTagName.Length + 1)))
            {
                // Separate the tagname from the namespace
                szTagName = szTagName.Substring(colon + 1, szTagName.Length - colon - 1);
            }

            // Find Type associated with tagname
            Object obj = _TagTypeTable[szTagName];

            // Return the Type if found
            if ((obj != null) && (obj is Type))
            {
                return (Type)obj;                    
            }

            // No Type was found, throw an exception
            throw new Exception(String.Format(Helper.GetStringResource("InvalidChildTagName"), tagName));
        }

        /// <summary>
        /// Rejects appending literal strings.
        /// </summary>
        /// <param name="s">The string.</param>
        public override void AppendLiteralString(string s)
        {
            if (AllowLiterals())
            {
                base.AppendLiteralString(s);
            }
            else
            {
                s = s.Trim();
                if (s != String.Empty)
                {
                    throw new Exception(String.Format(Helper.GetStringResource("InvalidLiteralString"), s));
                }
            }
        }

        /// <summary>
        /// Allows subclasses to override the rejection of literal strings.
        /// </summary>
        /// <returns>false to reject literals.</returns>
        public virtual bool AllowLiterals()
        {
            // Ignore all literals
            return false;
        }

        /// <summary>
        /// Rejects whitespace.
        /// </summary>
        /// <returns>false to reject whitespace.</returns>
        public override bool AllowWhitespaceLiterals()
        {
            // Ignore whitespace literals
            return false;
        }
    }
}
