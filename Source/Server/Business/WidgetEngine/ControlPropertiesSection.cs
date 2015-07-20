using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Configuration;

namespace Mediachase.IBN.Business.WidgetEngine
{
	public class ControlPropertiesSection : ConfigurationSection
	{
            private static ConfigurationPropertyCollection _properties;

            private static readonly ConfigurationProperty _propDefaultProvider;
            private static readonly ConfigurationProperty _propEnabled;
            private static readonly ConfigurationProperty _propProviders;

			private ControlPropertiesProviderCollection _controlPropertiesProviders;

            /// <summary>
            /// Initializes the <see cref="BlobStorageSection"/> class.
            /// </summary>
            static ControlPropertiesSection()
            {
				_propDefaultProvider = new ConfigurationProperty("defaultProvider", typeof(string), "XmlControlProperties", null, new StringValidator(1), ConfigurationPropertyOptions.None);
                _propEnabled = new ConfigurationProperty("enabled", typeof(bool), true, ConfigurationPropertyOptions.None);
                _propProviders = new ConfigurationProperty("providers", typeof(ProviderSettingsCollection), null, ConfigurationPropertyOptions.None);

                _properties = new ConfigurationPropertyCollection();

                _properties.Add(_propDefaultProvider);
                _properties.Add(_propEnabled);
                _properties.Add(_propProviders);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="BlobStorageSection"/> class.
            /// </summary>
			public ControlPropertiesSection()
            {
            }

			#region Properties
			/// <summary>
			/// Gets the collection of properties.
			/// </summary>
			/// <value></value>
			/// <returns>The <see cref="T:System.Configuration.ConfigurationPropertyCollection"></see> of properties for the element.</returns>
			protected override ConfigurationPropertyCollection Properties
			{
				get
				{
					return _properties;
				}
			} 
			#endregion

			#region DefaultProvider
			/// <summary>
			/// Gets or sets the default provider.
			/// </summary>
			/// <value>The default provider.</value>
			[StringValidator(MinLength = 1), ConfigurationProperty("defaultProvider", DefaultValue = "XmlControlProperties")]
			public string DefaultProvider
			{
				get
				{
					return (string)base[_propDefaultProvider];
				}
				set
				{
					base[_propDefaultProvider] = value;
				}
			} 
			#endregion

			#region Enabled
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="FileStorageSection"/> is enabled.
			/// </summary>
			/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
			[ConfigurationProperty("enabled", DefaultValue = true)]
			public bool Enabled
			{
				get
				{
					return (bool)base[_propEnabled];
				}
				set
				{
					base[_propEnabled] = value;
				}
			} 
			#endregion

			#region Providers
			/// <summary>
			/// Gets the providers.
			/// </summary>
			/// <value>The providers.</value>
			[ConfigurationProperty("providers")]
			public ProviderSettingsCollection Providers
			{
				get
				{
					return (ProviderSettingsCollection)base[_propProviders];
				}
			} 
			#endregion

			#region ControlPropertiesCollecition
			/// <summary>
			/// Gets the temp file storage providers.
			/// </summary>
			/// <value>The temp file storage providers.</value>
			public ControlPropertiesProviderCollection ControlPropertiesCollecition
			{
				get
				{
					if (this._controlPropertiesProviders == null)
					{
						lock (this)
						{
							if (this._controlPropertiesProviders == null)
							{
								ControlPropertiesProviderCollection providers = new ControlPropertiesProviderCollection();

								ProvidersHelper.InstantiateProviders(this.Providers,
																	 providers,
																	 typeof(ControlPropertiesBase));

								this._controlPropertiesProviders = providers;
							}
						}
					}
					return this._controlPropertiesProviders;
				}
			} 
			#endregion

			#region ValidateDefaultProvider
			/// <summary>
			/// Validates the default provider.
			/// </summary>
			internal void ValidateDefaultProvider()
			{
				if (!string.IsNullOrEmpty(this.DefaultProvider) &&
					(this.Providers[this.DefaultProvider] == null))
				{
					throw new ConfigurationErrorsException("Default ControlPropertiesBase must exist.",
						base.ElementInformation.Properties[_propDefaultProvider.Name].Source,
						base.ElementInformation.Properties[_propDefaultProvider.Name].LineNumber);
				}
			} 
			#endregion		
	}
}
