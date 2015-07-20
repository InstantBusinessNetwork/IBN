using System;
using System.Collections;

namespace Mediachase.Ibn.ControlSystem
{
	/// <summary>
	/// Summary description for IbnControlLoaderConfiguration.
	/// </summary>
	public class IbnContainerConfiguration
	{
		private IbnContainerInfoCollection	_containers = new IbnContainerInfoCollection();

		public IbnContainerConfiguration()
		{
		}

		public IbnContainerInfoCollection Containers
		{
			get
			{
				return _containers;
			}
		}
	}
}
