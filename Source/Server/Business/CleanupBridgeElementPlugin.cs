using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Represents cleanup bridge element plugin.
	/// </summary>
	public sealed class CleanupBridgeElementPlugin:IPlugin
	{
		#region Const
		#endregion

		#region Properties
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="CleanupBridgeElementPlugin"/> class.
		/// </summary>
		public CleanupBridgeElementPlugin()
		{
		}
		#endregion

		#region Methods
		#endregion

		#region IPlugin Members

		/// <summary>
		/// Executes the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		void IPlugin.Execute(BusinessContext context)
		{
			if (context.GetMethod() == RequestMethod.Delete && 
				context.GetTargetPrimaryKeyId().HasValue)
			{
				MetaClass currentMetaClass = DataContext.Current.GetMetaClass(context.GetTargetMetaClassName());

				foreach (MetaField mf in currentMetaClass.FindReferencesTo())
				{
					if (mf.Owner.IsBridge)
					{
						foreach(EntityObject entity in BusinessManager.List(mf.Owner.Name,
							new FilterElement[] { FilterElement.EqualElement(mf.Name, context.GetTargetPrimaryKeyId().Value) }))
						{
							BusinessManager.Delete(entity);
						}
					}
				}
			}
		}

		#endregion
	}
}
