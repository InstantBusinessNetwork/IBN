using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Mediachase.ClientOutlook.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class RegistryController
    {
        public delegate void ErrorOccuredDelegate(Exception ex);

        public event ErrorOccuredDelegate OnError;

        public string RegRoot { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryController"/> class.
        /// </summary>
        /// <param name="regRoot">The reg root.</param>
        public RegistryController(string regRoot)
        {
            RegRoot = regRoot;
        }

        /// <summary>
        /// Gets the sub kay names.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<String> GetSubKayNames()
        {
            RegistryKey lmKey = Registry.LocalMachine.OpenSubKey(this.RegRoot);
            RegistryKey cuKey = Registry.CurrentUser.OpenSubKey(this.RegRoot);
            return lmKey.GetSubKeyNames().Union(cuKey.GetSubKeyNames());
        }
		
        /// <summary>
        /// Reads the reg key.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public object ReadRegKey(string keyName, object defaultValue)
        {
            object retVal = defaultValue;
            try
            {
                string str = this.ReadRegistryKeyCurrentUser(keyName);
                if (str == null)
                {
                    str = this.ReadRegistryKeyLocalMachine(keyName);
                }
                retVal = str == null ? retVal : str;
            }
            catch
            {
                this.RaiseOnError(new ArgumentOutOfRangeException(keyName));
            }

            return retVal;
        }


        /// <summary>
        /// Writes the reg key.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="inValue">The in value.</param>
        public void WriteRegKey(string keyName, string inValue)
        {
            try
            {
                WriteRegistryKeyCurrentUser(keyName, inValue);
            }
            catch (System.Exception)
            {
                this.RaiseOnError(new ArgumentOutOfRangeException(keyName));
            }
        }

        /// <summary>
        /// Reads the registry key current user.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        private string ReadRegistryKeyCurrentUser(string keyName)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(this.RegRoot);
            return ReadRegistryKey(key, keyName);
        }

        /// <summary>
        /// Reads the registry key local machine.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        private string ReadRegistryKeyLocalMachine(string keyName)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(this.RegRoot);
            return ReadRegistryKey(key, keyName);
        }

        /// <summary>
        /// Reads the registry key.
        /// </summary>
        /// <param name="regKeyParent">The reg key parent.</param>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        private string ReadRegistryKey(RegistryKey regKeyParent, string keyName)
        {
            string retVal = null;

            if (regKeyParent != null)
            {
                object regKeyValue = regKeyParent.GetValue(keyName);
                retVal = regKeyValue == null ? retVal : regKeyValue.ToString();
            }
            else
            {
                this.RaiseOnError(new ArgumentOutOfRangeException(keyName));
            }

            return retVal;
        }

        /// <summary>
        /// Writes the registry key current user.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="inValue">The in value.</param>
        private void WriteRegistryKeyCurrentUser(string keyName, string inValue)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(this.RegRoot, true);
            if (regKey == null)
            {
                regKey = Registry.CurrentUser.CreateSubKey(this.RegRoot);
            }

            WriteRegistryKey(regKey, keyName, inValue);
        }

        /// <summary>
        /// Writes the registry key local machine.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="inValue">The in value.</param>
        private void WriteRegistryKeyLocalMachine(string keyName, string inValue)
        {
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(this.RegRoot);
            if (regKey == null)
            {
                regKey = Registry.CurrentUser.CreateSubKey(this.RegRoot);
            }
            WriteRegistryKey(regKey, keyName, inValue);

        }

        /// <summary>
        /// Writes the registry key current user.
        /// </summary>
        /// <param name="newKey">The new key.</param>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="inValue">The in value.</param>
        private void WriteRegistryKey(RegistryKey regKey, string keyName, string inValue)
        {
            if (regKey != null)
            {
                regKey.SetValue(keyName, inValue);
            }
            else
            {
                this.RaiseOnError(new ArgumentOutOfRangeException(keyName));
            }
        }

        /// <summary>
        /// Raises the on error.
        /// </summary>
        /// <param name="ex">The ex.</param>
        private void RaiseOnError(Exception ex)
        {
            ErrorOccuredDelegate tmp = this.OnError;
            if (tmp != null)
            {
                this.OnError(ex);
            }
        }

    }
}
