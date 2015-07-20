using System;
using System.Collections;

namespace Mediachase.IBN.Business.EMail
{
	public enum ExternalEMailActionType
	{
		None = 0,
		SetReOpenState = 1,
	}

	public enum InternalEMailActionType
	{
		None = 0,
		SetSuspendState = 1,
		SetCompletedState = 2,
		SendToCheck = 3,
	}

	/// <summary>
	/// Summary description for EMailRouterIncidentBoxBlock.
	/// </summary>
	public class EMailRouterIncidentBoxBlock: BaseIncidentBoxBlock
	{
		public EMailRouterIncidentBoxBlock()
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether [allow E mail routing].
		/// </summary>
		/// <value><c>true</c> if [allow E mail routing]; otherwise, <c>false</c>.</value>
		public bool AllowEMailRouting
		{
			set 
			{
				base.Params["AllowEMailRouting"] = value;
			}
			get 
			{
				if(base.Params.Contains("AllowEMailRouting"))
					return (bool)base.Params["AllowEMailRouting"];

				return false;
			}
		}

		/// <summary>
		/// Gets the external recipient list.
		/// </summary>
		/// <value>The external recipient list.</value>
		public ArrayList InformationRecipientList
		{
			set 
			{
				base.Params["InformationRecipientList"] = value;
			}
			get 
			{
				if(!base.Params.Contains("InformationRecipientList"))
					base.Params["InformationRecipientList"] = new ArrayList();

				return (ArrayList)base.Params["InformationRecipientList"];
			}
		}

		/// <summary>
		/// Gets or sets the external E mail action.
		/// </summary>
		/// <value>The external E mail action.</value>
		public ExternalEMailActionType IncomingEMailAction
		{
			set 
			{
				base.Params["ExternalEMailAction"] = value;
			}
			get 
			{
				if(base.Params.Contains("ExternalEMailAction"))
					return (ExternalEMailActionType)base.Params["ExternalEMailAction"];

				return ExternalEMailActionType.None;
			}
		}

		/// <summary>
		/// Gets or sets the internal E mail action.
		/// </summary>
		/// <value>The internal E mail action.</value>
		public InternalEMailActionType OutgoingEMailAction
		{
			set 
			{
				base.Params["InternalEMailAction"] = value;
			}
			get 
			{
				if(base.Params.Contains("InternalEMailAction"))
					return (InternalEMailActionType)base.Params["InternalEMailAction"];

				return InternalEMailActionType.None;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [send auto reply].
		/// </summary>
		/// <value><c>true</c> if [send auto reply]; otherwise, <c>false</c>.</value>
		public bool SendAutoReply
		{
			set 
			{
				base.Params["SendAutoReply"] = value;
			}
			get 
			{
				if(base.Params.Contains("SendAutoReply"))
					return (bool)base.Params["SendAutoReply"];

				return false;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [send auto incident closed].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [send auto incident closed]; otherwise, <c>false</c>.
		/// </value>
		public bool SendAutoIncidentClosed
		{
			set
			{
				base.Params["SendAutoIncidentClosed"] = value;
			}
			get
			{
				if (base.Params.Contains("SendAutoIncidentClosed"))
					return (bool)base.Params["SendAutoIncidentClosed"];

				return false;
			}
		}
	}
}
