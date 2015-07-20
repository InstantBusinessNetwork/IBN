using System;
using System.Collections;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.SpreadSheet;

namespace Mediachase.IBN.Business.SpreadSheet
{
	/// <summary>
	/// Summary description for BasePlan.
	/// </summary>
	public class BasePlan
	{
		private ProjectBasePlanInfoRow _srcRow = null;

		private BasePlan(ProjectBasePlanInfoRow row)
		{
			_srcRow = row;
		}

		/// <summary>
		/// Saves the specified Active Data by Project to Base Plan Slot.
		/// </summary>
		/// <param name="ProjectId">The project id.</param>
		/// <param name="BasePlanSlotId">The base plan slot id.</param>
		public static void Save(int ProjectId, int BasePlanSlotId)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				// Step. Create or Update ProjectBasePlanInfoRow
				

				ProjectBasePlanInfoRow infoRow = new ProjectBasePlanInfoRow();

				ProjectBasePlanInfoRow[] activeInfoRowList = ProjectBasePlanInfoRow.List(ProjectId); 

				foreach(ProjectBasePlanInfoRow activeInfoRow in activeInfoRowList)
				{
					if(activeInfoRow.BasePlanSlotId==BasePlanSlotId)
					{
						infoRow = activeInfoRow;
						break;
					}
				}

				infoRow.BasePlanSlotId = BasePlanSlotId;
				infoRow.ProjectId = ProjectId;
				infoRow.Created = DateTime.UtcNow;

				infoRow.Update();
                //DV. Fix Exception, when finances are not active!
                if (ProjectSpreadSheet.IsActive(ProjectId))
                {
                    // Step 1. Save Spread Sheet View from Currency
                    ProjectSpreadSheetDataRow.Delete(ProjectId, BasePlanSlotId);

                    ProjectSpreadSheetDataRow[] srcRowList = ProjectSpreadSheetDataRow.ListCurrentByProjectId(ProjectId);

                    foreach (ProjectSpreadSheetDataRow srcRow in srcRowList)
                    {
                        ProjectSpreadSheetDataRow newRow = new ProjectSpreadSheetDataRow();

                        newRow.Index = BasePlanSlotId;

                        newRow.ProjectSpreadSheetId = srcRow.ProjectSpreadSheetId;

                        newRow.ColumnId = srcRow.ColumnId;
                        newRow.RowId = srcRow.RowId;

                        newRow.CellType = srcRow.CellType;

                        newRow.Value = srcRow.Value;

                        newRow.Update();
                    }

                    SpreadSheetView view = ProjectSpreadSheet.LoadView(ProjectId, BasePlanSlotId, 2001, 2001);
                    ProjectSpreadSheet.SaveBusinessScore(ProjectId, BasePlanSlotId, view.Document);
                }

				// Step 2. Save Tasks
				ProjectTaskBasePlanRow.Fill(ProjectId, BasePlanSlotId);

				tran.Commit();
			}
		}


		/// <summary>
		/// Lists the specified project id.
		/// </summary>
		/// <param name="ProjectId">The project id.</param>
		/// <returns></returns>
		public static BasePlan[] List(int ProjectId)
		{
			ArrayList retVal = new ArrayList();

			foreach(ProjectBasePlanInfoRow row in ProjectBasePlanInfoRow.List(ProjectId))
			{
				retVal.Add(new BasePlan(row));
			}

			return (BasePlan[])retVal.ToArray(typeof(BasePlan));
		}


		#region Public Properties
		
		/// <summary>
		/// Gets the project base plan info id.
		/// </summary>
		/// <value>The project base plan info id.</value>
		public virtual int ProjectBasePlanInfoId
		{
			get
			{
				return _srcRow.ProjectBasePlanInfoId;
			}
			
		}
		
		/// <summary>
		/// Gets the project id.
		/// </summary>
		/// <value>The project id.</value>
		public virtual int ProjectId
	    
		{
			get
			{
				return _srcRow.ProjectId;
			}
		}
		
		/// <summary>
		/// Gets the base plan slot id.
		/// </summary>
		/// <value>The base plan slot id.</value>
		public virtual int BasePlanSlotId
	    
		{
			get
			{
				return _srcRow.BasePlanSlotId;
			}
		}
		
		/// <summary>
		/// Gets the created.
		/// </summary>
		/// <value>The created.</value>
		public virtual DateTime Created
	    
		{
			get
			{
				return Database.DBCommon.GetLocalDate(Security.CurrentUser.TimeZoneId,_srcRow.Created);
			}
		}
		
		#endregion
	}
}
