using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using System.Configuration;
using System.IO;
using Mediachase.Ibn.Data;
using System.Globalization;
using System.Web.Hosting;
using Mediachase.Ibn.XmlTools;
using System.Collections;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Represents local disk entity object plugin.
	/// </summary>
	public sealed class LocalDiskEntityObjectPlugin : IPlugin
	{
		#region Const
		public const string EntityObjectsDirs = "EntityObjects";
		public const string RootFolderAppSettingName = "LocalDiskEntityObjectFolder";
		public const string IsLoadDiskEntityPropertyName = "McBusiness_IsLoadDiskEntity";
		#endregion

		#region Properties
		private static Dictionary<string, EntityObject[]> entityObjectHash = new Dictionary<string, EntityObject[]>();

		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="LocalDiskEntityObjectPlugin"/> class.
		/// </summary>
		public LocalDiskEntityObjectPlugin()
		{
		}
		#endregion



		#region Methods
		/// <summary>
		/// Loads the entity objects.
		/// </summary>
		/// <param name="metaClassName">Name of the meta class.</param>
		/// <returns></returns>
		private static EntityObject[] LoadEntityObjects(string metaClassName)
		{
			if (metaClassName == null)
				throw new ArgumentNullException("metaClassName");

			if (!entityObjectHash.ContainsKey(metaClassName))
			{
				lock (typeof(LocalDiskEntityObjectPlugin))
				{
					if (!entityObjectHash.ContainsKey(metaClassName))
					{
						FileDescriptor[] files = FileResolver.GetFiles(EntityObjectsDirs + Path.DirectorySeparatorChar + metaClassName, "*.xml");

						List<EntityObject> items = new List<EntityObject>();

						// Load EntityObject From File
						foreach (FileDescriptor file in files)
						{
							PrimaryKeyId pk;

							if (PrimaryKeyId.TryParse(Path.GetFileNameWithoutExtension(file.FilePath), out pk))
							{
								// Load EntityObject From File
								EntityObject tmpItem = McXmlSerializer.GetObjectFromFile<EntityObject>(file.FilePath);

								// Create TEntityObject
								EntityObject item = BusinessManager.InitializeEntity(metaClassName);

								// Copy Fields From tmpItem To item
								foreach (EntityObjectProperty property in tmpItem.Properties)
								{
									item[property.Name] = property.Value;
								}

								//
								item.PrimaryKeyId = pk;

								// Add Abstract Property
								item[IsLoadDiskEntityPropertyName] = true;

								items.Add(item);
							}
						}

						entityObjectHash[metaClassName] = items.ToArray();
					}
				}
			}

			return entityObjectHash[metaClassName];
		}

		/// <summary>
		/// Loads the entity objects from disk.
		/// </summary>
		/// <param name="metaClass">The meta class.</param>
		/// <returns></returns>
		private EntityObject[] LoadEntityObjects(string metaClass, FilterElement[] filters)
		{
			if (metaClass == null)
				throw new ArgumentNullException("metaClass");

			List<EntityObject> retVal = new List<EntityObject>();

			foreach (EntityObject item in LoadEntityObjects(metaClass))
			{
				if (InMemoryFilterElementEvaluator.Eval(item, filters))
				{
					retVal.Add(item);
				}
			}

			return retVal.ToArray();
		}

		/// <summary>
		/// Loads the entity object from disk.
		/// </summary>
		/// <param name="metaClass">The meta class.</param>
		/// <param name="primaryKeyId">The primary key id.</param>
		/// <returns></returns>
		private EntityObject LoadEntityObject(string metaClass, PrimaryKeyId primaryKeyId)
		{
			if (metaClass == null)
				throw new ArgumentNullException("metaClass");

			foreach (EntityObject item in LoadEntityObjects(metaClass))
			{
				if (item.PrimaryKeyId == primaryKeyId)
					return item;
			}

			return null;
		}

		#endregion

		#region IPlugin Members

		/// <summary>
		/// Executes the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Execute(BusinessContext context)
		{
			switch (context.GetMethod())
			{
				case RequestMethod.Load:
					if (context.GetTargetPrimaryKeyId().HasValue)
					{
						EntityObject item = LoadEntityObject(context.GetTargetMetaClassName(), context.GetTargetPrimaryKeyId().Value);

						if (item != null)
						{
							LoadResponse response = new LoadResponse(item);

							context.SetResponse(response);
						}
					}
					break;
				case RequestMethod.List:
					{
						// Filters Not Implemented
						ListResponse response = context.Response as ListResponse;

						if (response != null)
						{
							EntityObject[] items = LoadEntityObjects(context.GetTargetMetaClassName(), ((ListRequest)context.Request).Filters);

							if (items.Length > 0)
							{
								List<EntityObject> newArray = new List<EntityObject>(response.EntityObjects.Length + items.Length);

								newArray.AddRange(response.EntityObjects);
								newArray.AddRange(items);

								// TODO: Sorting Not Implemented

								response.EntityObjects = newArray.ToArray();
							}
						}
					}
					break;
			}
		}

		#endregion
	}
}
