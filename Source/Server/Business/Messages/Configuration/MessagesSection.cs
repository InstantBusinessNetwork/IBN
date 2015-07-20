using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;

namespace Mediachase.Ibn.Business.Messages.Configuration
{
	/// <summary>
	/// Represents Messages Section.
	/// </summary>
	public class MessagesSection : ConfigurationSection
	{
		#region Const
		#endregion

		#region Fields
		ProviderCollection _deliveryProvider;
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="MessagesSection"/> class.
		/// </summary>
		public MessagesSection()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the providers.
		/// </summary>
		/// <value>The providers.</value>
		[ConfigurationProperty("deliveryProviders")]
		public ProviderSettingsCollection DeliveryProviders
		{
			get
			{
				return (ProviderSettingsCollection)base["deliveryProviders"];
			}
		}

		/// <summary>
		/// Gets the provider collection.
		/// </summary>
		/// <value>The provider collection.</value>
		public ProviderCollection DeliveryProviderCollection
		{
			get
			{
				if (this._deliveryProvider == null)
				{
					lock (this)
					{
						if (this._deliveryProvider == null)
						{
							ProviderCollection providers = new ProviderCollection();

							ProvidersHelper.InstantiateProviders(this.DeliveryProviders,
																 providers,
																 typeof(ProviderBase));

							this._deliveryProvider = providers;
						}
					}
				}

				return this._deliveryProvider;
			}
		}
		#endregion

		#region Methods
		#endregion

		
	}
}
