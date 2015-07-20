using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Assignments
{
	partial class WorkflowInstanceEntity
	{
		[System.Diagnostics.Conditional("DEBUG")]
		static void CreateMetaField2009_06_03()
		{
			using (DataContext.Current = new DataContext("Data source=S2;Initial catalog=ibn48portal;Integrated Security=SSPI;"))
			{
				MetaClass workflowClass = DataContext.Current.GetMetaClass(WorkflowInstanceEntity.ClassName);

				using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
				{
					// WorkflowInstanceTimeStatus
					MetaFieldType workflowInstanceTimeStatus = MetaEnum.Create("WorkflowInstanceTimeStatus", "Workflow Instance Time Status", false);

					MetaEnum.AddItem(workflowInstanceTimeStatus, "OverStart", 1);
					MetaEnum.AddItem(workflowInstanceTimeStatus, "OverDue", 2);

					// FinishTimeType
					MetaFieldType timeType = MetaEnum.Create("TimeType", "Time Type", false);

					MetaEnum.AddItem(timeType, "NotSet", 1);
					MetaEnum.AddItem(timeType, "DateTime", 2);
					MetaEnum.AddItem(timeType, "Duration", 3);


					using (MetaFieldBuilder mfb = new MetaFieldBuilder(workflowClass))
					{
						//ActualStartDate: DateTime
						mfb.CreateDateTime("ActualStartDate", "Actual Start Date", true, true);

						//ActualFinishDate: DateTime
						mfb.CreateDateTime("ActualFinishDate", "Actual Finish Date", true, true);

						//TimeStatus: DateTime
						mfb.CreateEnumField("TimeStatus", "Time Status", "WorkflowInstanceTimeStatus", true, "", false);

						//PlanFinishDate: DateTime
						mfb.CreateDateTime("PlanFinishDate", "Plan Finish Date", true, true);

						//PlanDuration: Duration
						mfb.CreateDuration("PlanDuration", "Plan Duration", true, 60);

						//PlanFinishTimeType: Enum (None, DateTime, Duration)
						mfb.CreateEnumField("PlanFinishTimeType", "Plan Finish Time Type", "TimeType", false, "1", false);

						mfb.SaveChanges();
					}

					scope.SaveChanges();
				}
			}
		}

		[System.Diagnostics.Conditional("DEBUG")]
		static void CreateMetaField2009_06_05()
		{
			using (DataContext.Current = new DataContext("Data source=S2;Initial catalog=ibn48portal;Integrated Security=SSPI;"))
			{
				MetaClass workflowClass = DataContext.Current.GetMetaClass(WorkflowInstanceEntity.ClassName);

				using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
				{
					using (MetaFieldBuilder mfb = new MetaFieldBuilder(workflowClass))
					{
						//PlanFinishTimeType: Enum (None, DateTime, Duration)
						mfb.CreateLongText("XSParameters", "Xml Serialized Parameters", true);

						mfb.SaveChanges();
					}

					scope.SaveChanges();
				}
			}
		}
	}
}
