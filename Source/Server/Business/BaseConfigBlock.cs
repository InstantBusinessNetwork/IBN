using System;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for BaseConfigBlock.
	/// </summary>
	public class BaseConfigBlock
	{
		private ConfigBlockParameters _params = new ConfigBlockParameters();

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseConfigBlock"/> class.
		/// </summary>
		public BaseConfigBlock()
		{
		}

		/// <summary>
		/// Gets the params.
		/// </summary>
		/// <value>The params.</value>
		public ConfigBlockParameters Params
		{
			get 
			{
				return _params;
			}
		}	
	}
}
