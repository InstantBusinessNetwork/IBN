using System;
using System.Collections;
using System.Xml;
using System.IO;
using System.Text;
using Mediachase.IBN.Database.EMail;
using System.Reflection;
using System.Xml.Serialization;
using Mediachase.IBN.Business;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for IncidentBoxDocument.
	/// </summary>
	public class IncidentBoxDocument: BaseConfigDocument
	{
		private int _ownerIncidentBoxId;

		/// <summary>
		/// Initializes a new instance of the <see cref="IncidentBoxDocument"/> class.
		/// </summary>
		/// <param name="OwnerIncidentBoxId">The owner incident box id.</param>
		protected IncidentBoxDocument(int OwnerIncidentBoxId)
		{
			this._ownerIncidentBoxId = OwnerIncidentBoxId;

			// Add default blocks
			base.AddBlock(new GeneralIncidentBoxBlock());
			base.AddBlock(new EMailRouterIncidentBoxBlock());
			base.AddBlock(new BaseIncidentBoxBlock());
		}

		/// <summary>
		/// Gets the owner incident box id.
		/// </summary>
		/// <value>The owner incident box id.</value>
		public int OwnerIncidentBoxId
		{
			get {return _ownerIncidentBoxId;}
		}

		/// <summary>
		/// Gets the general block.
		/// </summary>
		/// <value>The general block.</value>
		public GeneralIncidentBoxBlock GeneralBlock
		{
			get 
			{
				return (GeneralIncidentBoxBlock)base.GetBlock(typeof(GeneralIncidentBoxBlock));
			}
		}

		/// <summary>
		/// Gets the E mail router block.
		/// </summary>
		/// <value>The E mail router block.</value>
		public EMailRouterIncidentBoxBlock EMailRouterBlock
		{
			get 
			{
				return (EMailRouterIncidentBoxBlock)base.GetBlock(typeof(EMailRouterIncidentBoxBlock));
			}
		}

		/// <summary>
		/// Gets the base block.
		/// </summary>
		/// <value>The base block.</value>
		public BaseIncidentBoxBlock ExtendedBlock
		{
			get 
			{
				return (BaseIncidentBoxBlock)base.GetBlock(typeof(BaseIncidentBoxBlock));
			}
		}

		#region Load
		public static IncidentBoxDocument  Load(int IncidentBoxId, string Xml)
		{
			IncidentBoxDocument retVal = new IncidentBoxDocument(IncidentBoxId);
			retVal.Load(Xml);
			return retVal;
		}
		/// <summary>
		/// Loads the specified incident box id.
		/// </summary>
		/// <param name="IncidentBoxId">The incident box id.</param>
		/// <returns></returns>
		public static IncidentBoxDocument  Load(int IncidentBoxId)
		{
			IncidentBoxDocument retVal = new IncidentBoxDocument(IncidentBoxId);

			IncidentBoxRow row = IncidentBoxRow.Load(IncidentBoxId);

			if(row.Document!=string.Empty)
			{
				retVal.Load(row.Document);
			}

			return retVal;
		}
		#endregion

		#region Save
		public static void Save(IncidentBoxDocument document)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				IncidentBoxRow row = IncidentBoxRow.Load(document.OwnerIncidentBoxId);

				IncidentBoxDocument prevDocument = IncidentBoxDocument.Load(document.OwnerIncidentBoxId, row.Document);

				row.Document = document.GetDocumentString();

				row.ManagerId = document.GeneralBlock.Manager;
                
				if(document.GeneralBlock.AllowControl)
                {
                    if(document.GeneralBlock.ControllerAssignType== ControllerAssignType.CustomUser)
					    row.ControllerId = document.GeneralBlock.Controller;

                    if (document.GeneralBlock.ControllerAssignType == ControllerAssignType.Manager)
                        row.ControllerId = document.GeneralBlock.Manager;

                    if (document.GeneralBlock.ControllerAssignType == ControllerAssignType.Creator)
                        row.ControllerId = 0;
                }
                else
                    row.ControllerId = -1;

				row.CalendarId = document.GeneralBlock.CalendarId;
				row.ExpectedDuration = document.GeneralBlock.ExpectedDuration;
				row.ExpectedResponseTime = document.GeneralBlock.ExpectedResponseTime;
				row.ExpectedAssignTime = document.GeneralBlock.ExpectedAssignTime;
				row.TaskTime = document.GeneralBlock.TaskTime;

				row.Update();

				// Find Real Update
				if(prevDocument.GeneralBlock.AllowControl && !document.GeneralBlock.AllowControl)
				{
					// Turn OffControling
					Incident.RaiseTurnOffControling(document.OwnerIncidentBoxId);
				}
				else if (document.GeneralBlock.AllowControl)
				{
					if (prevDocument.GeneralBlock.Manager!=document.GeneralBlock.Manager && 
						document.GeneralBlock.ControllerAssignType == ControllerAssignType.Manager)
					{
						Incident.RaiseChangeController(document.OwnerIncidentBoxId, 
							prevDocument.GeneralBlock.Manager,
							document.GeneralBlock.Manager);
					}
					else if(document.GeneralBlock.ControllerAssignType == ControllerAssignType.CustomUser &&
						prevDocument.GeneralBlock.Controller!=document.GeneralBlock.Controller)
					{
						Incident.RaiseChangeController(document.OwnerIncidentBoxId, 
							prevDocument.GeneralBlock.Controller,
							document.GeneralBlock.Controller);
					}
				}

				if (prevDocument.GeneralBlock.Manager!=document.GeneralBlock.Manager)
				{
					Incident.RaiseChangeManager(document.OwnerIncidentBoxId, prevDocument.GeneralBlock.Manager, document.GeneralBlock.Manager); 
				}
				tran.Commit();
			}
		}

		#endregion

		#region RaiseDocumentModifications
		/// <summary>
		/// Raises the document modifications.
		/// </summary>
		/// <param name="IncidentId">The incident id.</param>
		/// <param name="OldIncidentBoxId">The old incident box id.</param>
		/// <param name="NewIncidentBoxId">The new incident box id.</param>
		public static void RaiseModifications(int IncidentId, int OldIncidentBoxId, int NewIncidentBoxId)
		{
			if(OldIncidentBoxId==NewIncidentBoxId)
				return;

			IncidentBoxDocument oldDoc = IncidentBoxDocument.Load(OldIncidentBoxId);
			IncidentBoxDocument newDoc = IncidentBoxDocument.Load(NewIncidentBoxId);

			// Find Real Update
			if(oldDoc.GeneralBlock.AllowControl && !newDoc.GeneralBlock.AllowControl)
			{
				// Turn OffControling
				Incident.RaiseTurnOffControling(IncidentId, NewIncidentBoxId);
			}
			else if (newDoc.GeneralBlock.AllowControl)
			{
				if (oldDoc.GeneralBlock.Manager!=newDoc.GeneralBlock.Manager && 
					newDoc.GeneralBlock.ControllerAssignType == ControllerAssignType.Manager)
				{
					Incident.RaiseChangeController(IncidentId, NewIncidentBoxId,
						oldDoc.GeneralBlock.Manager,
						newDoc.GeneralBlock.Manager);
				}
				else if(newDoc.GeneralBlock.ControllerAssignType == ControllerAssignType.CustomUser &&
					oldDoc.GeneralBlock.Controller!=newDoc.GeneralBlock.Controller)
				{
					Incident.RaiseChangeController(IncidentId, NewIncidentBoxId,
						oldDoc.GeneralBlock.Controller,
						newDoc.GeneralBlock.Controller);
				}
			}

			if (oldDoc.GeneralBlock.Manager!=newDoc.GeneralBlock.Manager)
			{
				Incident.RaiseChangeManager(IncidentId, NewIncidentBoxId, 
					oldDoc.GeneralBlock.Manager, newDoc.GeneralBlock.Manager); 
			}
		}
		#endregion
	}
}
