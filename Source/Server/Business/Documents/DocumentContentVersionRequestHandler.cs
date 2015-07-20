using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Sql;
using System.Data;

namespace Mediachase.IBN.Business.Documents
{
	/// <summary>
	/// Represents document content version request handler.
	/// </summary>
	public class DocumentContentVersionRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="DocumentContentVersionRequestHandler"/> class.
		/// </summary>
		public DocumentContentVersionRequestHandler()
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
			if (metaClassName == DocumentContentVersionEntity.GetAssignedMetaClassName())
			{
				DocumentContentVersionEntity retVal = new DocumentContentVersionEntity();
				retVal.PrimaryKeyId = primaryKeyId;
				return retVal;
			}

			return base.CreateEntityObject(metaClassName, primaryKeyId);
		}
		#endregion

		#region Create
		protected override void PreCreate(BusinessContext context)
		{
			base.PreCreate(context);

			// TODO: Check CreateRequest parameters here
		}

		protected override void PreCreateInsideTransaction(BusinessContext context)
		{
			// Call base method
			base.PreCreateInsideTransaction(context);

			DocumentContentVersionEntity newVersion = (DocumentContentVersionEntity)context.Request.Target;

			#region Check Source Version Id
			PrimaryKeyId? sourceVersionId = (PrimaryKeyId?)context.Request.Parameters.GetValue<PrimaryKeyId?>(DocumentContentVersionRequestParameters.Create_SourceVersionId);

			if (sourceVersionId.HasValue)
			{
				// Load Version
				DocumentContentVersionEntity srcVersion = (DocumentContentVersionEntity)BusinessManager.Load(DocumentContentVersionEntity.GetAssignedMetaClassName(), sourceVersionId.Value);

				newVersion.OwnerDocumentId = srcVersion.OwnerDocumentId;
				//newVersion.DocumentContentId = srcVersion.DocumentContentId;

				// Copy File
				if (srcVersion.File != null)
				{
					using (System.IO.Stream srcStream = srcVersion.File.OpenRead())
					{
						FileInfo fileInfo = new FileInfo(srcVersion.File.Name, srcStream);
						newVersion.File = fileInfo;
					}
				}
			} 
			#endregion
		}

		protected override void CopyEntityObjectToMetaObject(EntityObject target, MetaObject metaObject)
		{
			// Base Copy
			base.CopyEntityObjectToMetaObject(target, metaObject);

			// Process not updatable field
			DocumentContentVersionEntity srcVersion = (DocumentContentVersionEntity)target;

			#region Index
			// Only if new object
			if (metaObject.ObjectState == MetaObjectState.Created)
			{
				// Calculate max index
				int maxIndex = 0;
				SqlScript selectMaxIndex = new SqlScript();

				selectMaxIndex.Append("SELECT MAX([Index]) AS [Index] FROM [dbo].[cls_DocumentContentVersion] WHERE [OwnerDocumentId] = @OwnerDocumentId");
				selectMaxIndex.AddParameter("@OwnerDocumentId", (Guid)srcVersion.OwnerDocumentId);

				using (IDataReader reader = SqlHelper.ExecuteReader(SqlContext.Current,
					CommandType.Text, selectMaxIndex.ToString(), selectMaxIndex.Parameters.ToArray()))
				{
					if (reader.Read())
					{
						object value = reader["Index"];
						if (value is int)
						{
							maxIndex = (int)value;
						}
					}
				}

				// update index
				metaObject["Index"] = maxIndex + 1;
			}
			#endregion

			// Update State via Custom Method = UpdateState
		}
		#endregion

		#region Update
		protected override void PostUpdateInsideTransaction(BusinessContext context)
		{
			// Call base method
			base.PostUpdateInsideTransaction(context);

			// TODO: Process Active Version Id to Document Content
		}
		#endregion

		#region Delete
		protected override void PreDeleteInsideTransaction(BusinessContext context)
		{
			// Call Base method
			base.PreDeleteInsideTransaction(context);

		}
		#endregion

		#region UpdateState
		/// <summary>
		/// Pres the custom method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void PreUpdateState(BusinessContext context)
		{
		}

		/// <summary>
		/// Pres the custom method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void PreUpdateStateInsideTransaction(BusinessContext context)
		{
		}

		/// <summary>
		/// Customs the method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void UpdateState(BusinessContext context)
		{
			// Load Meta Object
			MetaObject metaObject = MetaObjectActivator.CreateInstance(DocumentContentVersionEntity.GetAssignedMetaClassName(),
				context.GetTargetPrimaryKeyId().Value);

			// Extract old and new states
			DocumentContentVersionState newState = (DocumentContentVersionState)(int)context.Request.Target["State"];
			DocumentContentVersionState oldState = (DocumentContentVersionState)(int)metaObject["State"];

			if (oldState == newState)
				return;

			BusinessContext.Current.Items["MC_Document_SourceMetaObject"] = metaObject;
			BusinessContext.Current.Items["MC_Document_NewState"] = newState;
			BusinessContext.Current.Items["MC_Document_OldState"] = oldState;

			


			// Pre-Process
			switch(oldState)
			{
				case DocumentContentVersionState.Active:
					{
						// Reset Content Reference to Active Document here
						DocumentEntity document = (DocumentEntity)BusinessManager.Load(DocumentEntity.GetAssignedMetaClassName(), (PrimaryKeyId)metaObject["OwnerDocumentId"]);
						document.ActiveVersionId = null;
						BusinessManager.Update(document);
					}
					break;
				case DocumentContentVersionState.Draft:
					// Nothing
					break;
				case DocumentContentVersionState.Obsolete:
					// Nothing
					break;
			}
			
			// Process
			switch(newState)
			{
				case DocumentContentVersionState.Active:
					{
						DocumentEntity document = (DocumentEntity)BusinessManager.Load(DocumentEntity.GetAssignedMetaClassName(), (PrimaryKeyId)metaObject["OwnerDocumentId"]);

						// Reset Current Reference to Active Document here
						if (document.ActiveVersionId.HasValue)
						{
							DocumentContentVersionEntity oldVersion = new DocumentContentVersionEntity(document.ActiveVersionId.Value);
							oldVersion.State = (int)DocumentContentVersionState.Draft;

							UpdateStateRequest request = new UpdateStateRequest(oldVersion);
							BusinessManager.Execute(request);
						}
						
						// Update Current Reference to Active Document here
						document.ActiveVersionId = context.GetTargetPrimaryKeyId();
						BusinessManager.Update(document);
					}
					break;
				case DocumentContentVersionState.Draft:
					break;
				case DocumentContentVersionState.Obsolete:
					break;
			}

			// Update Data Storage And Save
			metaObject["State"] = (int)newState;
			metaObject.Save();

			context.SetResponse(new Response());
		}

		/// <summary>
		/// Posts the custom method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void PostUpdateStateInsideTransaction(BusinessContext context)
		{
		}

		/// <summary>
		/// Posts the custom method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void PostUpdateState(BusinessContext context)
		{
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

			switch (context.GetMethod())
			{
				case DocumentContentVersionRequestMethod.UpdateState:
					PreUpdateState(context);
					break;
			}
		}

		/// <summary>
		/// Pres the custom method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreCustomMethodInsideTransaction(BusinessContext context)
		{
			base.PreCustomMethodInsideTransaction(context);

			switch (context.GetMethod())
			{
				case DocumentContentVersionRequestMethod.UpdateState:
					PreUpdateStateInsideTransaction(context);
					break;
			}
		}

		/// <summary>
		/// Customs the method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void CustomMethod(BusinessContext context)
		{
			base.CustomMethod(context);

			switch (context.GetMethod())
			{
				case DocumentContentVersionRequestMethod.UpdateState:
					UpdateState(context);
					break;
			}
		}

		/// <summary>
		/// Posts the custom method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PostCustomMethodInsideTransaction(BusinessContext context)
		{
			base.PostCustomMethodInsideTransaction(context);

			switch (context.GetMethod())
			{
				case DocumentContentVersionRequestMethod.UpdateState:
					PostUpdateStateInsideTransaction(context);
					break;
			}

		}

		/// <summary>
		/// Posts the custom method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PostCustomMethod(BusinessContext context)
		{
			base.PostCustomMethod(context);

			switch (context.GetMethod())
			{
				case DocumentContentVersionRequestMethod.UpdateState:
					PostUpdateState(context);
					break;
			}

		}
		#endregion

		#endregion

		
	}
}
