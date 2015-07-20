using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Business.Customization;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.IBN.Business.WidgetEngine
{
	internal static class MetaClassInstaller
	{
		internal static void CreateFix()
		{
			foreach (EntityObject obj in BusinessManager.List(CustomPageEntity.ClassName, new FilterElement[]{}))
			{
				string xml = McXmlSerializer.GetString<EntityObject>(obj);
			}
		}

		internal static void CreateGoogleGadgetMetaClass()
		{
			DataContext.Current = new DataContext("Data source=S2;Initial catalog=ibn48portal;Integrated Security=SSPI;");

			using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
			{
				MetaClass googleGadget = DataContext.Current.MetaModel.CreateMetaClass("GoogleGadget", "Google Gadget", "Google Gadgets", "cls_GoogleGadget", PrimaryKeyId.ValueType.Guid);

				using (MetaFieldBuilder builder = new MetaFieldBuilder(googleGadget))
				{
					builder.CreateText("Title", "Title", false, 255, false);
					builder.CreateLongText("Description", "Description", true);
					builder.CreateUrl("Link", "Link", false, 1024, false, string.Empty);

					builder.SaveChanges();
				}

				googleGadget.TitleFieldName = "Title";

				BusinessObjectServiceManager.InstallService(googleGadget, ChangeDetectionService.ServiceName);


				scope.SaveChanges();
			}
		}

		internal static void CreateCustomPageMetaClass()
		{
			DataContext.Current = new DataContext("Data source=S2;Initial catalog=ibn48portal;Integrated Security=SSPI;");

			using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
			{
				MetaClass googleGadget = DataContext.Current.MetaModel.CreateMetaClass("CustomPage", "Custom Page", "Custom Pages", "cls_CustomPage", PrimaryKeyId.ValueType.Guid);

				using (MetaFieldBuilder builder = new MetaFieldBuilder(googleGadget))
				{
					builder.CreateText("Title", "Title", false, 255, false);
					builder.CreateLongText("Description", "Description", true);
					builder.CreateFile("Icon", "Icon", true, string.Empty);
					builder.CreateLongText("JsonData", "Json Data", false);
					builder.CreateGuid("TemplateId", "Template Id", false);

					builder.CreateGuid("Uid", "Uid", false);
					builder.CreateReference("Profile", "Profile", true, CustomizationProfileEntity.ClassName, true);
					builder.CreateReference("User", "User", true, "Principal", true);

					builder.SaveChanges();
				}

				googleGadget.TitleFieldName = "Title";

				BusinessObjectServiceManager.InstallService(googleGadget, ChangeDetectionService.ServiceName);


				scope.SaveChanges();
			}
		}

	}
}
