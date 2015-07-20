using System;
using System.Configuration;

namespace Mediachase.IBN.Business.ControlSystem
{
	/// <summary>
	/// Summary description for BaseIbnContainer.
	/// </summary>
	public class BaseIbnContainer: IIbnContainer
	{
		private string _name;
		private string _key;
		private IbnContainerInfo _info;

		protected BaseIbnContainer(string name, string key, IbnContainerInfo info)
		{
			_name = name;
			_key = key;
			_info = info;
		}

		public static BaseIbnContainer Create(string name, string key)
		{
			IbnContainerConfiguration config = (IbnContainerConfiguration)ConfigurationManager.GetSection("ibnContainers");
			
			BaseIbnContainer retVal = new BaseIbnContainer(name,key,config.Containers[name]);

			return retVal;
		}

		#region IIbnContainer Members

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string Key
		{
			get
			{
				return _key;
			}
		}

		public string[] Roles
		{
			get
			{
				// TODO:
				return new string[]{};
			}
		}

		#endregion

		public IbnContainerInfo Info
		{
			get
			{
				return _info;
			}
		}

		public IIbnControl	LoadControl(string name)
		{
			IbnControlInfo controlInfo = this.Info.Controls[name];
			if(controlInfo==null)
				throw new ArgumentException("Invalid agument.","name");

			IIbnControl retVal = (IIbnControl)AssemblyHelper.LoadObject(controlInfo.Type,typeof(IIbnControl));

			retVal.Init(this,controlInfo);

			return retVal;
		}
	}
}
