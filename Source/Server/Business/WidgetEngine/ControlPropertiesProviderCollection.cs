using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Configuration.Provider;

namespace Mediachase.IBN.Business.WidgetEngine
{
	public class ControlPropertiesProviderCollection : ProviderCollection
	{
		#region ctor : ControlPropertiesProviderCollection
		/// <summary>
		/// Initializes a new instance of the <see cref="BlobStorageProviderCollection"/> class.
		/// </summary>
		public ControlPropertiesProviderCollection()
		{
		} 
		#endregion

		#region Add
		/// <summary>
		/// Adds a provider to the collection.
		/// </summary>
		/// <param name="provider">The provider to be added.</param>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Configuration.Provider.ProviderBase.Name"></see> of provider is null.- or -The length of the <see cref="P:System.Configuration.Provider.ProviderBase.Name"></see> of provider is less than 1.</exception>
		/// <exception cref="T:System.ArgumentNullException">provider is null.</exception>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		/// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
		public override void Add(ProviderBase provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}

			if (!(provider is ControlPropertiesBase))
			{
				throw new ArgumentException(string.Format("Provider must implement the interface '{0}'",
					typeof(ControlPropertiesBase).Name), "provider");
			}

			this.Add((ControlPropertiesBase)provider);
		} 
		#endregion

		#region Add
		/// <summary>
		/// Adds the specified provider.
		/// </summary>
		/// <param name="provider">The provider.</param>
		public void Add(ControlPropertiesBase provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			base.Add(provider);
		} 
		#endregion

		#region AddArray
		/// <summary>
		/// Adds the array.
		/// </summary>
		/// <param name="providerArray">The provider array.</param>
		public void AddArray(ControlPropertiesBase[] providerArray)
		{
			if (providerArray == null)
			{
				throw new ArgumentNullException("providerArray");
			}
			foreach (ControlPropertiesBase provider in providerArray)
			{
				if (this[provider.Name] != null)
				{
					throw new ArgumentException("ControlPropertiesBase already exists.");
				}
				this.Add(provider);
			}
		} 
		#endregion

        /// <summary>
        /// Gets the <see cref="FileStorageProvider"/> with the specified name.
        /// </summary>
        /// <value></value>
		public new ControlPropertiesBase this[string name]
        {
            get
            {
				return (ControlPropertiesBase)base[name];
            }
        }
	}
}
