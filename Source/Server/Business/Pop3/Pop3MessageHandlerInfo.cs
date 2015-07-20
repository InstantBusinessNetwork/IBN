using System;

namespace Mediachase.IBN.Business.Pop3
{
	/// <summary>
	/// Summary description for Pop3MessageHandlerInfo.
	/// </summary>
	[Serializable]
	public class Pop3MessageHandlerInfo
	{
		private string _name = string.Empty;
		private string _type = string.Empty;
		private string _propertyControlPath = string.Empty;

		[NonSerialized]
		private IPop3MessageHandler _handler = null;

		public Pop3MessageHandlerInfo(string name, string type, string propertyControl)
		{
			_name = name;
			_type = type;
			_propertyControlPath = propertyControl;
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string Type
		{
			get
			{
				return _type;
			}
		}

		public string PropertyControlPath
		{
			get
			{
				return _propertyControlPath;
			}
		}

		public System.Web.UI.Control CreatePropertyControl(System.Web.UI.Page ownerPage)
		{
			return ownerPage.LoadControl(this.PropertyControlPath);
		}

		public IPop3MessageHandler Handler
		{
			get
			{
				if(_handler==null)
				{
					lock(this)
					{
						_handler = (IPop3MessageHandler)Mediachase.IBN.Business.ControlSystem.AssemblyHelper.LoadObject(this.Type,typeof(IPop3MessageHandler));
					}
				}
				return _handler;
			}
		}
	}
}
