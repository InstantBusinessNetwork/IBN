using System;
using Mediachase.IBN.Business;
using Mediachase.IBN.Database.EMail;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for EMailRouterPop3BoxSettings.
	/// </summary>
	public class EMailRouterPop3BoxSettings: BaseConfigDocument
	{
		private int _ownerEmailBoxId;

		/// <summary>
		/// Initializes a new instance of the <see cref="EMailRouterPop3BoxSettings"/> class.
		/// </summary>
		/// <param name="OwnerEmailBoxId">The owner email box id.</param>
		public EMailRouterPop3BoxSettings(int OwnerEmailBoxId)
		{
			this._ownerEmailBoxId = OwnerEmailBoxId;

			base.AddBlock(new DefaultEMailIncidentMappingBlock());
			base.AddBlock(new BaseConfigBlock());
		}

		/// <summary>
		/// Gets the owner email box id.
		/// </summary>
		/// <value>The owner email box id.</value>
		public int OwnerEmailBoxId
		{
			get {return _ownerEmailBoxId;}
		}

		/// <summary>
		/// Gets or sets the selected handler id.
		/// </summary>
		/// <value>The selected handler id.</value>
		public int SelectedHandlerId
		{
			set 
			{
				this.ExtendedBlock.Params["SelectedHandlerId"] = value;
			}
			get 
			{
				if(this.ExtendedBlock.Params.Contains("SelectedHandlerId"))
					return (int)this.ExtendedBlock.Params["SelectedHandlerId"];
				return -1;
			}
		}

		/// <summary>
		/// Gets the default E mail incident mapping block.
		/// </summary>
		/// <value>The default E mail incident mapping block.</value>
		public DefaultEMailIncidentMappingBlock DefaultEMailIncidentMappingBlock
		{
			get 
			{
				return (DefaultEMailIncidentMappingBlock)base.GetBlock(typeof(DefaultEMailIncidentMappingBlock));
			}
		}

		/// <summary>
		/// Gets the extended block.
		/// </summary>
		/// <value>The extended block.</value>
		public BaseConfigBlock ExtendedBlock
		{
			get 
			{
				return (BaseConfigBlock)base.GetBlock(typeof(BaseConfigBlock));
			}
		}

		#region Load
		public static EMailRouterPop3BoxSettings  Load(int EMailRouterPop3BoxId, string Xml)
		{
			EMailRouterPop3BoxSettings retVal = new EMailRouterPop3BoxSettings(EMailRouterPop3BoxId);
			retVal.Load(Xml);
			return retVal;
		}
		/// <summary>
		/// Loads the specified incident box id.
		/// </summary>
		/// <param name="EMailRouterPop3BoxId">The incident box id.</param>
		/// <returns></returns>
		public static EMailRouterPop3BoxSettings  Load(int EMailRouterPop3BoxId)
		{
			EMailRouterPop3BoxSettings retVal = new EMailRouterPop3BoxSettings(EMailRouterPop3BoxId);

			EMailRouterPop3BoxRow row = new EMailRouterPop3BoxRow(EMailRouterPop3BoxId);

			if(row.Settings!=string.Empty)
			{
				retVal.Load(row.Settings);
			}

			return retVal;
		}
		#endregion

		#region Save
		public static void Save(EMailRouterPop3BoxSettings document)
		{
			EMailRouterPop3BoxRow row = new EMailRouterPop3BoxRow(document.OwnerEmailBoxId);

			row.Settings = document.GetDocumentString();

			row.Update();
		}

		#endregion
	}
}
