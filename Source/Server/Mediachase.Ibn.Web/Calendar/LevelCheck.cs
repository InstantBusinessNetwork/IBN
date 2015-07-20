//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------

namespace Mediachase.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Web;
    using System.Web.UI;

    /// <summary>
    /// Represents a Browser's version.
    /// </summary>
    public class BrowserLevel
    {
        private int _MajorVersion;
        private double _MinorVersion;
        private bool _RequireWindows;

        /// <summary>
        /// Initializes a new instance of a BrowserLevel.
        /// </summary>
        /// <param name="majorversion">Major Version</param>
        /// <param name="minorversion">Minor Version</param>
        /// <param name="requireWindows">Requires Win32</param>
        public BrowserLevel(int majorversion, double minorversion, bool requireWindows)
        {
            MajorVersion = majorversion;
            MinorVersion = minorversion;
            RequireWindows = requireWindows;
        }

        /// <summary>
        /// The major version (3 in 3.1).
        /// </summary>
        public int MajorVersion
        {
            get { return _MajorVersion; }
            set { _MajorVersion = value; }
        }

        /// <summary>
        /// The minor version (0.1 in 3.1).
        /// </summary>
        public double MinorVersion
        {
            get { return _MinorVersion; }
            set { _MinorVersion = value; }
        }

        /// <summary>
        /// Indicates whether the Windows client is required.
        /// </summary>
        public bool RequireWindows
        {
            get { return _RequireWindows; }
            set { _RequireWindows = value; }
        }

        /// <summary>
        /// Tests for equality between to BrowserLevel objects.
        /// </summary>
        /// <param name="bl1">The first object.</param>
        /// <param name="bl2">The second object.</param>
        /// <returns>true if the same.</returns>
        public static bool operator ==(BrowserLevel bl1, BrowserLevel bl2)
        {
            return (
                (bl1.MajorVersion == bl2.MajorVersion) &&
                (bl1.MinorVersion == bl2.MinorVersion) &&
                (bl1.RequireWindows == bl2.RequireWindows)
            );
        }

        /// <summary>
        /// Tests for equality between to BrowserLevel objects.
        /// </summary>
        /// <param name="obj">The object to compare this object to.</param>
        /// <returns>true if the same.</returns>
        public override bool Equals(object obj)
        {
            if (obj is BrowserLevel)
            {
                return ((BrowserLevel)obj) == this;
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        /// <param name="bl1">The first object.</param>
        /// <param name="bl2">The second object.</param>
        /// <returns>true if different.</returns>
        public static bool operator !=(BrowserLevel bl1, BrowserLevel bl2)
        {
            return !(bl1 == bl2);
        }

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        /// <param name="bl1">The first object.</param>
        /// <param name="bl2">The second object.</param>
        /// <returns>true if the first object is greater than the second.</returns>
        public static bool operator >(BrowserLevel bl1, BrowserLevel bl2)
        {
            return ((bl1.RequireWindows == bl2.RequireWindows) && (
                (bl1.MajorVersion > bl2.MajorVersion) ||
                (
                    (bl1.MajorVersion == bl2.MajorVersion) &&
                    (bl1.MinorVersion > bl2.MinorVersion)
                ))
            );
        }

        /// <summary>
        /// Test for inequality.
        /// </summary>
        /// <param name="bl1">The first object.</param>
        /// <param name="bl2">The second object.</param>
        /// <returns>Returns true if the first object is less than the second.</returns>
        public static bool operator <(BrowserLevel bl1, BrowserLevel bl2)
        {
            return ((bl1.RequireWindows == bl2.RequireWindows) && (
                (bl1.MajorVersion < bl2.MajorVersion) ||
                (
                    (bl1.MajorVersion == bl2.MajorVersion) &&
                    (bl1.MinorVersion < bl2.MinorVersion)
                ))
            );
        }

        /// <summary>
        /// Test for equality or greater than inequality.
        /// </summary>
        /// <param name="bl1">The first object.</param>
        /// <param name="bl2">The second object.</param>
        /// <returns>true if the first object is greater than or equal to the second.</returns>
        public static bool operator >=(BrowserLevel bl1, BrowserLevel bl2)
        {
            return (
                (bl1 == bl2) ||
                (bl1 > bl2)
            );
        }

        /// <summary>
        /// Tests for equality or less than inequality.
        /// </summary>
        /// <param name="bl1">The first object.</param>
        /// <param name="bl2">The second object.</param>
        /// <returns>true if the first object is less than or equal to the second.</returns>
        public static bool operator <=(BrowserLevel bl1, BrowserLevel bl2)
        {
            return (
                (bl1 == bl2) ||
                (bl1 < bl2)
            );
        }
    }
    
    /// <summary>
    /// Determines if a browser falls into the uplevel or downlevel category.
    /// </summary>
    public class BrowserLevelChecker
    {
        private Hashtable _Browsers;

        /// <summary>
        /// Initializes a new instance of a BrowserLevelChecker.
        /// </summary>
        public BrowserLevelChecker()
        {
            _Browsers = new Hashtable();
        }

        /// <summary>
        /// Initializes a new instance of a BrowserLevelChecker.
        /// </summary>
        /// <param name="browser">Uplevel browser name.</param>
        /// <param name="majorversion">Uplevel major version.</param>
        /// <param name="minorversion">Uplevel minor version.</param>
        /// <param name="requireWindows">Uplevel Win32 requirement.</param>
        public BrowserLevelChecker(string browser, int majorversion, double minorversion, bool requireWindows) : this()
        {
            Add(browser, majorversion, minorversion, requireWindows);
        }

        /// <summary>
        /// Initializes a new instance of a BrowserLevelChecker.
        /// </summary>
        /// <param name="browser">Uplevel browser name.</param>
        /// <param name="minBrowser">Uplevel browser version.</param>
        public BrowserLevelChecker(string browser, BrowserLevel minBrowser) : this()
        {
            Add(browser, minBrowser);
        }

        /// <summary>
        /// Adds an uplevel browser.
        /// </summary>
        /// <param name="browser">Uplevel browser name.</param>
        /// <param name="majorversion">Uplevel major version.</param>
        /// <param name="minorversion">Uplevel minor version.</param>
        /// <param name="requireWindows">Uplevel Win32 required.</param>
        public void Add(string browser, int majorversion, double minorversion, bool requireWindows)
        {
            BrowserLevel bl = new BrowserLevel(majorversion, minorversion, requireWindows);
            Add(browser, bl);
        }

        /// <summary>
        /// Adds an uplevel browser.
        /// </summary>
        /// <param name="browser">Uplevel browser name.</param>
        /// <param name="minBrowser">Uplevel browser version.</param>
        public void Add(string browser, BrowserLevel minBrowser)
        {
            _Browsers.Add(browser.ToLower(), minBrowser);
        }

        /// <summary>
        /// Determines if a browser is uplevel.
        /// </summary>
        /// <param name="context">HttpContext containing browser information.</param>
        /// <returns>true if uplevel, false if downlevel.</returns>
        public bool IsUpLevelBrowser(HttpContext context)
        {
            if ((context == null) || (context.Request == null))
            {
                return false;
            }

            HttpBrowserCapabilities browser = context.Request.Browser;
            if (browser == null)
            {
                return false;
            }

            BrowserLevel minLevel = new BrowserLevel(browser.MajorVersion, browser.MinorVersion, browser.Win32);

            Object obj = _Browsers[browser.Browser.ToLower()];
            if (obj == null)
            {
                return false;
            }

            BrowserLevel bl = (BrowserLevel)obj;
            return (minLevel >= bl);
        }
    }
}
