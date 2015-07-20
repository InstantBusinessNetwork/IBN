using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Business.Messages
{
	/// <summary>
	/// Represents OutgoingMessageQueuePlugin.
	/// </summary>
	public class OutgoingMessageQueuePlugin: IPlugin 
	{
		#region Const
		public const string AddToQueue = "OmqAdd";
		public const string SourceName = "OmqSource";
		public const string ExprationDate = "ExprationDate";
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="OutgoingMessageQueuePlugin"/> class.
		/// </summary>
		public OutgoingMessageQueuePlugin()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		/// <summary>
		/// Executes the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void Execute(BusinessContext context)
		{
			if (context.Request.Parameters.Contains(OutgoingMessageQueuePlugin.AddToQueue))
			{
				PrimaryKeyId primaryKeyId = ResolvePrimaryKey(context);

				if (primaryKeyId != PrimaryKeyId.Empty)
				{
					string referenceFieldName = GetReferenceFieldName(context.GetTargetMetaClassName());

					if (!string.IsNullOrEmpty(referenceFieldName))
					{
						string source = context.Request.Parameters.GetValue<string>(OutgoingMessageQueuePlugin.SourceName, string.Empty);

						PrimaryKeyId queuePk = AddMessageToOutgoingQueue(context.GetTargetMetaClassName(), referenceFieldName, primaryKeyId, source);
					}
				}
			}
		}

		/// <summary>
		/// Adds the message to outgoing queue.
		/// </summary>
		/// <param name="referenceFieldName">Name of the reference field.</param>
		/// <param name="primaryKeyId">The primary key id.</param>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		protected PrimaryKeyId AddMessageToOutgoingQueue(string metaClassName, string referenceFieldName, PrimaryKeyId primaryKeyId, string source)
		{
			OutgoingMessageQueueEntity newElement = BusinessManager.InitializeEntity<OutgoingMessageQueueEntity>(OutgoingMessageQueueEntity.ClassName);

			newElement[referenceFieldName] = primaryKeyId;
			newElement.Source = source;

			return BusinessManager.Create(newElement);
		}

		/// <summary>
		/// Gets the name of the reference field.
		/// </summary>
		/// <param name="metaClassName">Name of the meta class.</param>
		/// <returns></returns>
		protected virtual string GetReferenceFieldName(string metaClassName)
		{
			if (metaClassName == EmailEntity.ClassName)
			{
				return OutgoingMessageQueueEntity.FieldEmailId;
			}
			else if (metaClassName == IbnClientMessageEntity.ClassName)
			{
				return OutgoingMessageQueueEntity.FieldIbnClientMessageId;
			}
			else 
			{
				MetaClass metaClass = DataContext.Current.GetMetaClass(OutgoingMessageQueueEntity.ClassName);

				MetaField[] fields = metaClass.GetReferences(metaClassName);

				if (fields.Length > 0)
					return fields[0].Name;
			}

			return string.Empty;
		}

		/// <summary>
		/// Resolves the primary key from context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		protected PrimaryKeyId ResolvePrimaryKey(BusinessContext context)
		{
			// Read From PrimaryKeyId
			if (context.Request.Target.PrimaryKeyId.HasValue)
				return context.Request.Target.PrimaryKeyId.Value;

			// Read From Create Response
			if (context.GetMethod() == RequestMethod.Create && context.Response is CreateResponse)
				return ((CreateResponse)context.Response).PrimaryKeyId;

			return PrimaryKeyId.Empty;
		}
		#endregion



		#region IPlugin Members

		/// <summary>
		/// Executes the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		void IPlugin.Execute(BusinessContext context)
		{
			this.Execute(context);
		}

		#endregion
	}
}
