using System;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for IbnContainerInfo.
	/// </summary>
	public class IbnContainerInfo
	{
		private string _name = string.Empty;
		private IbnControlInfoCollection _controls = new IbnControlInfoCollection();
		

		public IbnContainerInfo(string name)
		{
			_name = name;
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public IbnControlInfoCollection Controls
		{
			get
			{
				return _controls;
			}
		}

	}
}
