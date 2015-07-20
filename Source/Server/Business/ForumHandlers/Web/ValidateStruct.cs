using System;
using System.Data;
using System.Configuration;
using System.Collections;

namespace Mediachase.IBN.Business
{
    public class ValidateStruct
    {
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

        public ValidateStruct(bool IsVisible, bool IsEnabled)
        {
            this.IsVisible = IsVisible;
            this.IsEnabled = IsEnabled;
        }
    }
}