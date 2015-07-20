using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Assignments
{
	partial class WorkflowDefinitionEntity
	{
		#region Test Methods
		[System.Diagnostics.Conditional("DEBUG")]
		static void TestCreateFields()
		{
			DataContext.Current = new DataContext("Data source=S2;Initial catalog=ibn48portal;Integrated Security=SSPI;");

			using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
			{
				MetaClass mc = DataContext.Current.GetMetaClass(WorkflowDefinitionEntity.ClassName);

				// Create Enum
				MetaFieldType enumType = MetaEnum.Create("SupportedIbnObjectType", "Supported Ibn Object Type", true);

				MetaEnum.AddItem(enumType, 16, "Document", 1);
				MetaEnum.AddItem(enumType, 5, "Task", 2);
				MetaEnum.AddItem(enumType, 6, "Todo", 3);
				MetaEnum.AddItem(enumType, 7, "Incident", 4);

				// Create Project Field
				using (MetaFieldBuilder bulder = new MetaFieldBuilder(mc))
				{
					bulder.CreateReference("Project", "Project", true, "Project", true);

					bulder.CreateEnumField("SupportedIbnObjectTypes", "Supported Ibn Object Types", "SupportedIbnObjectType", true, "16", false);

					bulder.SaveChanges();
				}

				scope.SaveChanges();
			}
		}

		[System.Diagnostics.Conditional("DEBUG")]
		static void TestCreate()
		{
			DataContext.Current = new DataContext("Data source=S2;Initial catalog=ibn48portal;Integrated Security=SSPI;");

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				WorkflowDefinitionEntity entity1 = new WorkflowDefinitionEntity();
				entity1.Name = "Test 111";
				entity1.SupportedIbnObjectTypes = new int[] { 16 };

				BusinessManager.Create(entity1);

				WorkflowDefinitionEntity entity2 = new WorkflowDefinitionEntity();
				entity2.Name = "Test 222";
				entity2.SupportedIbnObjectTypes = new int[] { 16, 5, 6, 7 };

				BusinessManager.Create(entity2);

				WorkflowDefinitionEntity entity3 = new WorkflowDefinitionEntity();
				entity3.Name = "Test 333";
				entity3.SupportedIbnObjectTypes = new int[] { 5, 6, 7 };

				BusinessManager.Create(entity3);

				tran.Commit();
			}

		}

		[System.Diagnostics.Conditional("DEBUG")]
		static public void TestList()
		{
			//DataContext.Current = new DataContext("Data source=S2;Initial catalog=ibn48portal;Integrated Security=SSPI;");

			EntityObject[] items = BusinessManager.List(WorkflowDefinitionEntity.ClassName, new FilterElement[]
				{
					FilterElement.EqualElement(WorkflowDefinitionEntity.FieldSupportedIbnObjectTypes, 16)
				});


			EntityObject[] items2 = BusinessManager.List(WorkflowDefinitionEntity.ClassName, new FilterElement[]
				{
					FilterElement.EqualElement(WorkflowDefinitionEntity.FieldSupportedIbnObjectTypes, 5)
				});

			EntityObject[] items3 = BusinessManager.List(WorkflowDefinitionEntity.ClassName, new FilterElement[]
				{
					new OrBlockFilterElement(
						FilterElement.EqualElement(WorkflowDefinitionEntity.FieldSupportedIbnObjectTypes, 5),
						FilterElement.EqualElement(WorkflowDefinitionEntity.FieldSupportedIbnObjectTypes, 16)
						)
				});

		}

		[System.Diagnostics.Conditional("DEBUG")]
		static void CreateMetaField2009_06_03()
		{
			using (DataContext.Current = new DataContext("Data source=S2;Initial catalog=ibn48portal;Integrated Security=SSPI;"))
			{
				MetaClass workflowClass = DataContext.Current.GetMetaClass(WorkflowDefinitionEntity.ClassName);

				using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
				{
					workflowClass.DeleteMetaField("PlanDuration");

					using (MetaFieldBuilder mfb = new MetaFieldBuilder(workflowClass))
					{
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
				MetaClass workflowClass = DataContext.Current.GetMetaClass(WorkflowDefinitionEntity.ClassName);

				using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
				{
					using (MetaFieldBuilder mfb = new MetaFieldBuilder(workflowClass))
					{
						mfb.CreateLongText("XSParameters", "Xml Serialized Parameters", true);

						mfb.SaveChanges();
					}

					scope.SaveChanges();
				}
			}
		}
		#endregion
	}
}
