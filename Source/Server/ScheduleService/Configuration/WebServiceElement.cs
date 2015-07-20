using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Net;

namespace Mediachase.Schedule.Service.Configuration
{
	/// <summary>
	/// Represents WebServiceElement.
	/// </summary>
	public class WebServiceElement : ConfigurationElement
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		[ConfigurationProperty("url", DefaultValue = "", IsRequired = true)]
		public string Url 
		{
			get
			{
				return (string)base["url"];
			}
			set
			{
				base["url"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the credential domain.
		/// </summary>
		/// <value>The credential domain.</value>
		[ConfigurationProperty("credentialDomain", DefaultValue = "", IsRequired = false)]
		public string CredentialDomain
		{
			get
			{
				if (base["credentialDomain"] == null)
					return string.Empty;

				return (string)base["credentialDomain"];
			}
			set
			{
				base["credentialDomain"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the credential password.
		/// </summary>
		/// <value>The credential password.</value>
		[ConfigurationProperty("credentialUserName", DefaultValue = "", IsRequired = false)]
		public string CredentialUserName
		{
			get
			{
				return (string)base["credentialUserName"];
			}
			set
			{
				base["credentialUserName"] = value;
			}
		}


		/// <summary>
		/// Gets or sets the credential password.
		/// </summary>
		/// <value>The credential password.</value>
		[ConfigurationProperty("credentialPassword", DefaultValue = "", IsRequired = false)]
		public string CredentialPassword
		{
			get
			{
				return (string)base["credentialPassword"];
			}
			set
			{
				base["credentialPassword"] = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has credential.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has credential; otherwise, <c>false</c>.
		/// </value>
		public bool HasCredential
		{
			get
			{
				return !string.IsNullOrEmpty(this.CredentialPassword) &&
					!string.IsNullOrEmpty(this.CredentialUserName);
			}
		}

		private NetworkCredential _сredential;

		/// <summary>
		/// Gets the credential.
		/// </summary>
		/// <value>The credential.</value>
		public NetworkCredential Credential 
		{
			get
			{
				if (!this.HasCredential)
					return CredentialCache.DefaultNetworkCredentials;

				if(_сredential==null)
					_сredential = new NetworkCredential(this.CredentialUserName, this.CredentialPassword, this.CredentialDomain);

				return _сredential;
			}
		}
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceElement"/> class.
		/// </summary>
		public WebServiceElement()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceElement"/> class.
		/// </summary>
		/// <param name="url">The URL.</param>
		public WebServiceElement(string url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			this.Url = url;
		}
		#endregion

		#region Methods
		#endregion
	}
}
