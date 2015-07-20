using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;

namespace Mediachase.Ibn.Business.Messages
{
	/// <summary>
	/// Represents base message delivery provider.
	/// </summary>
	public abstract class MessageDeliveryProvider : ProviderBase
	{
		#region Const
		#endregion

		#region Fields
		private bool _enable = true;
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="MessageDeliveryProvider"/> class.
		/// </summary>
		public MessageDeliveryProvider()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="MessageDeliveryProvider"/> is enable.
		/// </summary>
		/// <value><c>true</c> if enable; otherwise, <c>false</c>.</value>
		public bool Enable
		{
			get { return _enable; }
			set { _enable = value; }
		}
	
		#endregion

		#region Abstract Methods
		/// <summary>
		/// Invokes this instance.
		/// </summary>
		public abstract void Invoke();
		#endregion

		#region Methods
		/// <summary>
		/// Initializes the provider.
		/// </summary>
		/// <param name="name">The friendly name of the provider.</param>
		/// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// The name of the provider is null.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// The name of the provider has a length of zero.
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> on a provider after the provider has already been initialized.
		/// </exception>
		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			base.Initialize(name, config);

			if (config["enable"] != null)
			{
				this.Enable = bool.Parse(config["enable"]);
			}

			// TODO: Read Custom Parameters here
		}
		#endregion

		
	}
}
