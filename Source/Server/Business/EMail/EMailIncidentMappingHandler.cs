using System;
using System.Collections;
using Mediachase.IBN.Database.EMail;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for EMailIncidentMappingHandler.
	/// </summary>
	public class EMailIncidentMappingHandler
	{
		private EMailIncidentMappingRow _srcRow = null;

		private EMailIncidentMappingHandler(EMailIncidentMappingRow row)
		{
			_srcRow = row;
		}

		/// <summary>
		/// Loads the specified E mail incident mapping id.
		/// </summary>
		/// <param name="EMailIncidentMappingId">The E mail incident mapping id.</param>
		/// <returns></returns>
		public static EMailIncidentMappingHandler	Load(int EMailIncidentMappingId)
		{
			return new EMailIncidentMappingHandler(new EMailIncidentMappingRow(EMailIncidentMappingId));
		}

		#region Public Properties
		
        
		/// <summary>
		/// Gets the E mail incident mapping id.
		/// </summary>
		/// <value>The E mail incident mapping id.</value>
		public virtual int EMailIncidentMappingId
	    
		{
			get
			{
				return _srcRow.EMailIncidentMappingId;
			}
			
		}
		
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public virtual string Name
	    
		{
			get
			{
				return _srcRow.Name;
			}
			
		}
		
		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>The type.</value>
		public virtual string Type
	    
		{
			get
			{
				return _srcRow.Type;
			}
		}
		
		/// <summary>
		/// Gets the user control.
		/// </summary>
		/// <value>The user control.</value>
		public virtual string UserControl
	    
		{
			get
			{
				return _srcRow.UserControl;
			}
		}
		
		#endregion

		/// <summary>
		/// Lists this instance.
		/// </summary>
		/// <returns></returns>
		public static EMailIncidentMappingHandler[]	List()
		{
			ArrayList retVal = new ArrayList();

			foreach(EMailIncidentMappingRow row in EMailIncidentMappingRow.List())
			{
				retVal.Add(new EMailIncidentMappingHandler(row));
			}

			return (EMailIncidentMappingHandler[])retVal.ToArray(typeof(EMailIncidentMappingHandler));
		}

		public static IncidentInfo CreateMapping(int EMailMessageId)
		{
			// Load Message Info
			EMailMessageInfo mi = EMailMessageInfo.Load(EMailMessageId);

			// Load Emal Box
			EMailRouterPop3Box emailBox = EMailRouterPop3Box.Load(mi.EMailRouterPop3BoxId);

			// Load Mapping Handler By EMail Box
			IEMailIncidentMapping mappingHandler = EMailIncidentMappingHandler.LoadHandler(emailBox.Settings.SelectedHandlerId);

			// Mapping
			return mappingHandler.Create(emailBox,EMailMessage.GetPop3Message(EMailMessageId));
		}

		public static IEMailIncidentMapping LoadHandler(int EMailIncidentMappingId)
		{
			string TypeName = string.Empty;

			if(EMailIncidentMappingId>0)
			{
				EMailIncidentMappingRow row = new EMailIncidentMappingRow(EMailIncidentMappingId);
				TypeName = row.Type;
			}
			else
			{
				// Load Default
				foreach(EMailIncidentMappingRow row in EMailIncidentMappingRow.List())
				{
					TypeName = row.Type;
					if(row.Name=="Default")
						break;
				}
			}

			return (IEMailIncidentMapping)Mediachase.IBN.Business.ControlSystem.AssemblyHelper.LoadObject(TypeName, typeof(IEMailIncidentMapping));
		}

	}
}
