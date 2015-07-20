using System;

namespace Mediachase.IBN.Business.Pop3
{
	/// <summary>
	/// Summary description for BasePop3MessageHandler.
	/// </summary>
	public class BasePop3MessageHandler: IPop3MessageHandler
	{
		private string _name = string.Empty;
		private string _description = string.Empty;
		private Pop3Manager _ownerManager = null;

		protected System.Web.UI.Control _propertyPage = null;

		public BasePop3MessageHandler()
		{
		}

		public BasePop3MessageHandler(string Name, string Description)
		{
			_name = Name;
			_description = Description;
		}

		protected virtual void OnInit()
		{
		}

		protected virtual void OnProcessPop3Message(Pop3Box box, Mediachase.Net.Mail.Pop3Message message)
		{
		}

		#region IPop3MessageHandler Members

		public void Init(Pop3Manager manager)
		{
			_ownerManager = manager;

			OnInit();
		}

		public void ProcessPop3Message(Pop3Box box, Mediachase.Net.Mail.Pop3Message message)
		{
			OnProcessPop3Message(box, message);
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string Description
		{
			get
			{
				return _description;
			}
		}
		#endregion

		protected string InnerName
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		protected string InnerDescription
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		public virtual Pop3Manager OwnerManager
		{
			get
			{
				return _ownerManager;
			}
		}
	}
}
