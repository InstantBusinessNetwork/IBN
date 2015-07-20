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
	public abstract class ControlPropertiesBase : ProviderBase
	{
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
			//if (!string.IsNullOrEmpty(config["allowSearch"]))
			//{
			//    _allowSearch = bool.Parse(config["allowSearch"]);
			//    config.Remove("allowSearch");
			//}

            base.Initialize(name, config);
        }

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="cUid">The c uid.</param>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public abstract object GetValue(string cUid, string key);

		/// <summary>
		/// Saves the value.
		/// </summary>
		/// <param name="cUid">The c uid.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public abstract void SaveValue(string cUid, string key, object value);


		/// <summary>
		/// Deletes the value.
		/// </summary>
		/// <param name="cUid">The c uid.</param>
		public abstract void DeleteValue(string cUid);

	}
}
