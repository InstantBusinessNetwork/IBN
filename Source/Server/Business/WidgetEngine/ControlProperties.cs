using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Mediachase.IBN.Business.WidgetEngine
{
	public static class ControlProperties
	{
		private static ControlPropertiesProviderCollection _providers;
		private static ControlPropertiesBase _defaultProvider;
		private static bool _enabled = true;

		public static readonly string _pageUidKey = "__CurrentPageUid";
		public static readonly string _profileUidKey = "__CurrentProfileUid";
		public static readonly string _userUidKey = "__CurrentUserUid";
		public static readonly string _nullValueKey = "NULL";

		public static readonly string _countKey = "__Count";
		public static readonly string _titleKey = "__Title";

		#region Initialize
		/// <summary>
		/// Initializes this instance.
		/// </summary>
		internal static void Initialize()
		{
			if (_providers == null)
			{
				lock (typeof(ControlProperties))
				{
					if (_providers == null)
					{
						ControlPropertiesSection section = (ControlPropertiesSection)
														ConfigurationManager.GetSection("mediachase.ControlPropertiesProviders/storageProvider");
						if (section == null)
						{
							_providers = new ControlPropertiesProviderCollection();
							_enabled = false;
						}
						else
						{
							_enabled = section.Enabled;

							section.ValidateDefaultProvider();

							_providers = section.ControlPropertiesCollecition;
							_defaultProvider = _providers[section.DefaultProvider];
							_providers.SetReadOnly();
						}
					}
				}
			}
		}
		#endregion

		/// <summary>
		/// Gets a value indicating whether this <see cref="BlobStorage"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public static bool Enabled
		{
			get
			{
				Initialize();
				return _enabled;
			}
		}
		/// <summary>
		/// Gets the providers.
		/// </summary>
		/// <value>The providers.</value>
		public static ControlPropertiesProviderCollection Providers
		{
			get
			{
				Initialize();
				return _providers;
			}
		}
		/// <summary>
		/// Gets the provider.
		/// </summary>
		/// <value>The provider.</value>
		public static ControlPropertiesBase Provider
		{
			get
			{
				Initialize();
				return _defaultProvider;
			}
		}


	}
}
