using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.IBN.Business.WidgetEngine
{
	partial class CustomPageEntity
	{
		static void CreatePropertyJsonDataField()
		{
			DataContext.Current = new DataContext("Data source=S2;Initial catalog=ibn48portal;Integrated Security=SSPI;");

			using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
			{
				MetaClass metaClass = DataContext.Current.GetMetaClass(CustomPageEntity.ClassName);

				using (MetaFieldBuilder builder = new MetaFieldBuilder(metaClass))
				{
					builder.CreateLongText("PropertyJsonData", "Property Json Data", true);

					builder.SaveChanges();
				}

				scope.SaveChanges();
			}
		}
	}
}
