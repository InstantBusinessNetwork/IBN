using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using System.Web.Hosting;
using Mediachase.Ibn.Data.Meta;
using System.IO;

namespace Mediachase.Ibn.Assignments.Schemas
{
	/// <summary>
	/// Represents schema manager.
	/// </summary>
	public static class SchemaManager
	{
		#region Const
		#endregion

		#region Fields
		private static SchemaMasterCollection _master = null;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the masters.
		/// </summary>
		/// <value>The masters.</value>
		public static SchemaMasterCollection Masters 
		{
			get
			{
				if (_master == null)
				{
					lock (typeof(SchemaMaster))
					{
						if (_master == null)
						{
							_master = LoadDefaultMasterts();
						}
					}
				}

				return _master;
			}
		}
		#endregion

		#region Test Methods
		[System.Diagnostics.Conditional("DEBUG")]
		static void Test()
		{
			SchemaMaster master = new SchemaMaster();

			master.TypeName = AssemblyUtil.GetTypeString(typeof(SequentialWorkflowInstanceFactory));
			master.InstanceFactory = new SequentialWorkflowInstanceFactory();
			master.Description.Id = Guid.NewGuid();
			master.Description.Name = "Agreement";
			master.Description.Creator = "Mediachase";
			master.Description.Icon = string.Empty;
			master.Description.Comment = string.Empty;
			master.Description.UI.CreateControl = string.Empty;
			master.Description.UI.EditControl = string.Empty;
			master.Description.UI.ViewControl = string.Empty;

			ActivityMaster create = new ActivityMaster();
			create.TypeName = AssemblyUtil.GetTypeString(typeof(CreateAssignmentAndWaitResultActivityInstanceFactory));
			CreateAssignmentAndWaitResultActivityInstanceFactory ifact = new CreateAssignmentAndWaitResultActivityInstanceFactory();
			ifact.AssignmentProperties = new PropertyValueCollection();
			ifact.AssignmentProperties.Add(new PropertyValue("Subject", "Test Subject"));
			ifact.AssignmentProperties.Add(new PropertyValue("UserId", 12));

			create.InstanceFactory = ifact;

			create.Description.Name = "AgreeWith";
			create.Description.Icon = string.Empty;
			create.Description.Comment = string.Empty;
			create.Description.UI.CreateControl = string.Empty;
			create.Description.UI.EditControl = string.Empty;
			create.Description.UI.ViewControl = string.Empty;


			ActivityMaster block = new ActivityMaster();
			block.TypeName = AssemblyUtil.GetTypeString(typeof(BlockActivityInstanceFactory));
			block.InstanceFactory = new BlockActivityInstanceFactory();
			block.Description.Name = "AgreementBlock";
			block.Description.Icon = string.Empty;
			block.Description.Comment = string.Empty;
			block.Description.UI.CreateControl = string.Empty;
			block.Description.UI.EditControl = string.Empty;
			block.Description.UI.ViewControl = string.Empty;

			master.Description.Activities.Add(create);
			master.Description.Activities.Add(block);

			master.Description.SupportedIbnObjectTypes.Add(16);

			string xml = McXmlSerializer.GetString<SchemaMaster>(master, typeof(SequentialWorkflowInstanceFactory),
						typeof(CreateAssignmentAndWaitResultActivityInstanceFactory),
						typeof(BlockActivityInstanceFactory));
		}
		#endregion

		#region Methods

		private static SchemaMasterCollection LoadDefaultMasterts()
		{
			SchemaMasterCollection retVal = new SchemaMasterCollection();

			//string path = HostingEnvironment.MapPath("~/Apps/BusinessProcess/Config/Schemas");

			//if (Directory.Exists(path))
			//{
			//    foreach (string filePath in Directory.GetFiles(path, "*.xml"))
			//    {
			//        SchemaMaster master = McXmlSerializer.GetObjectFromFile<SchemaMaster>(filePath,
			//            typeof(SequentialWorkflowInstanceFactory),
			//            typeof(CreateAssignmentAndWaitResultActivityInstanceFactory),
			//            typeof(BlockActivityInstanceFactory));

			//        retVal.Add(master);
			//    }
			//}

			// O.R. [2009-07-28]: Use subdirectories to find schemas
			string rootPath = HostingEnvironment.MapPath("~/Apps/BusinessProcess");
			if (Directory.Exists(rootPath))
			{
				foreach (string filePath in Directory.GetFiles(rootPath, "*.xml", SearchOption.AllDirectories))
				{
					if (filePath.ToLowerInvariant().Contains("\\config\\schemas\\"))
					{
						SchemaMaster master = McXmlSerializer.GetObjectFromFile<SchemaMaster>(filePath,
							typeof(SequentialWorkflowInstanceFactory),
							typeof(CreateAssignmentAndWaitResultActivityInstanceFactory),
							typeof(BlockActivityInstanceFactory));

						retVal.Add(master);
					}
				}
			}

			return retVal;
		}

		/// <summary>
		/// Gets the available shema masters.
		/// </summary>
		/// <returns></returns>
		public static SchemaMaster[] GetAvailableShemaMasters()
		{
			return Masters.ToArray();
		}

		/// <summary>
		/// Gets the available shema masters.
		/// </summary>
		/// <param name="objectTypeId">The object type id.</param>
		/// <returns></returns>
		public static SchemaMaster[] GetAvailableShemaMasters(int objectTypeId)
		{
			return Masters.FindAll(delegate(SchemaMaster master) 
				{ 
					return master.Description.SupportedIbnObjectTypes.Count ==0 || 
						master.Description.SupportedIbnObjectTypes.Contains(objectTypeId); 
				}).ToArray();
		}

		/// <summary>
		/// Gets the shema master.
		/// </summary>
		/// <param name="Id">The id.</param>
		/// <returns></returns>
		public static SchemaMaster GetShemaMaster(Guid Id)
		{
			foreach (SchemaMaster master in Masters)
			{
				if (master.Description.Id == Id)
					return master;
			}

			return null;
		}
		#endregion
	}
}
