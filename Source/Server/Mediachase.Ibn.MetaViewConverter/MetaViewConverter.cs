using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

using Mediachase.Database;
//using Mediachase.Ibn.Core;
//using Mediachase.Ibn.Core.Database;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Converter
{
	public sealed class MetaViewConverter
	{
		private MetaViewConverter()
		{
		}

		public static void Convert(DBHelper dbHelper)
		{
			List<MetaViewInfo> views = new List<MetaViewInfo>();

			using (MetadataContext context = new MetadataContext(dbHelper))
			{
				// Load MetaView and MetaViewPreference objects for each List_* class.
				//Console.WriteLine(DateTime.Now);

				Regex listDetector = new Regex(@"List_\d+");

				MetaClassManager metaClassManager = DataContext.Current.MetaModel;
				foreach (MetaClass metaClass in metaClassManager.MetaClasses)
				{
					if (listDetector.IsMatch(metaClass.Name))
					{
						foreach (MetaView metaView in metaClassManager.GetMetaViews(metaClass))
						{
							MetaViewInfo view = new MetaViewInfo();
							view.Load(metaView);
							views.Add(view);
						}
					}
				}

				// Save ListViewProfile.
				//Console.WriteLine(DateTime.Now);

				foreach (MetaViewInfo view in views)
				{
					view.SaveListViewProfiles();
				}

				context.Commit();
				//Console.WriteLine(DateTime.Now);
			}
		}
	}
}
