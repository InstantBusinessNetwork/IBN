using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.IBN.Business.Documents.Configuration;
using System.Configuration;

namespace Mediachase.IBN.Business.Documents
{
	/// <summary>
	/// Êepresents document request handler.
	/// </summary>
	public class DocumentRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="DocumentRequestHandler"/> class.
		/// </summary>
		public DocumentRequestHandler()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods


		#region CreateEntityObject
		/// <summary>
		/// Creates the entity object.
		/// </summary>
		/// <param name="metaClassName">Name of the meta class.</param>
		/// <param name="primaryKeyId">The primary key id.</param>
		/// <returns></returns>
		protected override EntityObject CreateEntityObject(string metaClassName, PrimaryKeyId? primaryKeyId)
		{
			if (metaClassName == DocumentEntity.GetAssignedMetaClassName())
			{
				DocumentEntity retVal = new DocumentEntity();
				retVal.PrimaryKeyId = primaryKeyId;
				return retVal;
			}

			return base.CreateEntityObject(metaClassName, primaryKeyId);
		}
		#endregion

		#region Create
		/// <summary>
		/// Pres the create inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreCreateInsideTransaction(BusinessContext context)
		{
			base.PreCreateInsideTransaction(context);

			// Initialize DocumentTypeId from
			string metaClassName = context.GetTargetMetaClassName();

			EntityObject[] documentTypes = BusinessManager.List(DocumentTypeEntity.GetAssignedMetaClassName(),
				new FilterElement[] { FilterElement.EqualElement("Name", metaClassName) });

			if (documentTypes.Length > 0)
			{
				context.Request.Target["DocumentTypeId"] = documentTypes[0].PrimaryKeyId.Value;
			}

			// Initialize Processor Keys
			context.Items["MC_DocumentTemplateProcessorKeys"] = new Dictionary<string, EntityObject>();
		}

		protected override void PostCreateInsideTransaction(BusinessContext context)
		{
			base.PostCreateInsideTransaction(context);

			PrimaryKeyId documentPrimaryKeyId = ((CreateResponse)context.Response).PrimaryKeyId;

			// Check TemplateId
			PrimaryKeyId templatePrimaryKeyId = context.Request.Parameters.GetValue<PrimaryKeyId>(DocumentRequestParameters.Create_DocumentTemplatedId, PrimaryKeyId.Empty);
			if (templatePrimaryKeyId != PrimaryKeyId.Empty)
			{
				// Load Template
				DocumentTemplateEntity template = (DocumentTemplateEntity)BusinessManager.Load(DocumentTemplateEntity.GetAssignedMetaClassName(), templatePrimaryKeyId);

				if(template.File!=null)
				{
					// Initialize Content 
					//DocumentContentEntity content = (DocumentContentEntity)BusinessManager.InitializeEntity(DocumentContentEntity.GetAssignedMetaClassName());
					//content.Name = template.File.Name;
					//content.OwnerDocumentId = documentPrimaryKeyId;

					//PrimaryKeyId contentPrimaryKeyId = BusinessManager.Create(content);

					// Initialize Version
					DocumentContentVersionEntity version = (DocumentContentVersionEntity)BusinessManager.InitializeEntity(DocumentContentVersionEntity.GetAssignedMetaClassName());
					version.Name = template.File.Name;
					version.OwnerDocumentId = documentPrimaryKeyId;

					// version.DocumentContentId is not updateble
					//version.DocumentContentId = contentPrimaryKeyId; 

					// Upload File
					using (System.IO.Stream inputStream = template.File.OpenRead())
					{
						// Modify Template
						string extension = System.IO.Path.GetExtension(template.File.Name);
						IDocumentTemplateProcessor processor = FindTemplateProcessor(extension);

						System.IO.Stream outputStream = null;
						if (processor != null)
						{
							Dictionary<string, EntityObject> keys = (Dictionary<string,EntityObject>)context.Items["MC_DocumentTemplateProcessorKeys"];

							context.Request.Target.PrimaryKeyId = ((CreateResponse)context.Response).PrimaryKeyId;

							keys.Add("Document", context.Request.Target);
							keys.Add("DocumentContentVersion", version);
							keys.Add("DocumentTemplateEntity", template);

							outputStream = processor.Convert(extension, inputStream, keys);
						}
						else
						{
							outputStream = inputStream;
						}

						FileInfo fileInfo = new FileInfo(template.File.Name, outputStream);
						version.File = fileInfo;

						PrimaryKeyId versionPrimaryKeyId = BusinessManager.Create(version);
					}
				}
			}
			else
			{
				// Initialize Content 
				//DocumentContentEntity content = (DocumentContentEntity)BusinessManager.InitializeEntity(DocumentContentEntity.GetAssignedMetaClassName());
				//content.Name = string.Empty;
				//content.OwnerDocumentId = documentPrimaryKeyId;

				//PrimaryKeyId contentPrimaryKeyId = BusinessManager.Create(content);
			}
		}

		protected DocumentEntitySection GetConfigSection()
		{
			// Load From Web.Config
			DocumentEntitySection section = (DocumentEntitySection)ConfigurationManager.GetSection("documentEntity");
			return section;
		}

		protected virtual IDocumentTemplateProcessor FindTemplateProcessor(string extension)
		{
			if (string.IsNullOrEmpty(extension))
				return null;

			DocumentEntitySection section = GetConfigSection();
			if (section == null)
				return null;

			string typeName = section.TemplateProcessors.FindTypeName(extension);

			if (string.IsNullOrEmpty(typeName))
				return null;

			return AssemblyUtil.LoadObject<IDocumentTemplateProcessor>(typeName);
		}
		#endregion

		#region Delete

		protected override void PreDeleteInsideTransaction(BusinessContext context)
		{
			base.PreDeleteInsideTransaction(context);

			#region Delete DocumentVersion
			EntityObject[] versions = BusinessManager.List(DocumentContentVersionEntity.GetAssignedMetaClassName(),
				new FilterElement[] { FilterElement.EqualElement("OwnerDocumentId", context.GetTargetPrimaryKeyId().Value) });

			foreach(EntityObject version in versions)
			{
				BusinessManager.Delete(version);
			}
			#endregion

			#region Process RelatedDocumentAction
			RelatedDocumentAction relDocumentAction = context.Request.Parameters.GetValue<RelatedDocumentAction>(DocumentRequestParameters.Delete_RelatedDocumentAction, RelatedDocumentAction.None);

			switch (relDocumentAction)
			{
				// Detach all assigned contacts
				case RelatedDocumentAction.Detach:
					EntityObject[] detachedItems = BusinessManager.List(DocumentEntity.GetAssignedMetaClassName(),
						new FilterElement[] { FilterElement.EqualElement("MasterDocumentId", context.GetTargetPrimaryKeyId()) });

					foreach (DocumentEntity document in detachedItems)
					{
						document.MasterDocumentId = null;

						BusinessManager.Update(document);
					}
					break;
				// Delete all assigned actions
				case RelatedDocumentAction.Delete:
					EntityObject[] deletedItems = BusinessManager.List(DocumentEntity.GetAssignedMetaClassName(),
						new FilterElement[] { FilterElement.EqualElement("MasterDocumentId", context.GetTargetPrimaryKeyId()) });

					foreach (DocumentEntity document in deletedItems)
					{
						DeleteRequest request = new DeleteRequest(document);
						request.Parameters.Add(DocumentRequestParameters.Delete_RelatedDocumentAction, RelatedDocumentAction.Delete);

						BusinessManager.Execute(request);
					}
					break;
			} 

			#endregion

			// TODO: Process Link To Current Document
		}
		#endregion

		#region CustomMethod
		/// <summary>
		/// Pres the custom method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreCustomMethod(BusinessContext context)
		{
			base.PreCustomMethod(context);
		}

		/// <summary>
		/// Pres the custom method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreCustomMethodInsideTransaction(BusinessContext context)
		{
			base.PreCustomMethodInsideTransaction(context);
		}

		/// <summary>
		/// Customs the method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void CustomMethod(BusinessContext context)
		{
			base.CustomMethod(context);
		}

		/// <summary>
		/// Posts the custom method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PostCustomMethodInsideTransaction(BusinessContext context)
		{
			base.PostCustomMethodInsideTransaction(context);
		}

		/// <summary>
		/// Posts the custom method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PostCustomMethod(BusinessContext context)
		{
			base.PostCustomMethod(context);
		}

		#endregion

		#endregion
	}

}
