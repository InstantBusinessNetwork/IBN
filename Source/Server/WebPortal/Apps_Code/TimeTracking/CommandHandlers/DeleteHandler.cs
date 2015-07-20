using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Apps.MetaUI.Grid;
using Mediachase.Ibn.Data;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Services;

namespace Mediachase.Ibn.Web.UI.TimeTracking.CommandHandlers
{
	public class DeleteHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				// we'll use this list to collect unique block id's taken from deleted entries
				// at the end we'll delete empty blocks
				ArrayList blocks = new ArrayList();

				int deletedItems = 0;
				CommandParameters cp = (CommandParameters)Element;
				string[] elemsToDelete = MetaGridServer.GetCheckedCollection(((CommandManager)Sender).Page, cp.CommandArguments["GridId"]);
				using (TransactionScope tran = DataContext.Current.BeginTransaction())
				{
					//1. All entries
					foreach (string elem in elemsToDelete)
					{
						string type = elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1];
						if (type == MetaViewGroupUtil.keyValueNotDefined)
						{
							deletedItems++;
							continue;
						}
						
						int id = Convert.ToInt32(elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0]);
						
						//int id = int.Parse(elem);
						if (type != TimeTrackingEntry.GetAssignedMetaClass().Name)
							continue;

						TimeTrackingEntry tte = MetaObjectActivator.CreateInstance<TimeTrackingEntry>(TimeTrackingEntry.GetAssignedMetaClass(), id);
						if (TimeTrackingManager.DeleteEntry(tte))
						{
							deletedItems++;

							if (!blocks.Contains(tte.ParentBlockId))
								blocks.Add(tte.ParentBlockId);
						}
					}
					//2. All blocks
					foreach (string elem in elemsToDelete)
					{
						string type = elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1];
						if (type == MetaViewGroupUtil.keyValueNotDefined)
							continue;

						int id = Convert.ToInt32(elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0]);
						
						//if (id >= 0)
						//    continue;
						//id = -id;
						if (type != TimeTrackingBlock.GetAssignedMetaClass().Name)
							continue;

						TimeTrackingBlock ttb = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), id);
						if (Mediachase.Ibn.Data.Services.Security.CanDelete(ttb))
						{
							ttb.Delete();
							deletedItems++;

							if (blocks.Contains(ttb.PrimaryKeyId))
								blocks.Remove(ttb.PrimaryKeyId);
						}
					}

					// delete empty blocks
					using (SkipSecurityCheckScope scope = Mediachase.Ibn.Data.Services.Security.SkipSecurityCheck())
					{
						foreach (PrimaryKeyId blockId in blocks)
						{
							TimeTrackingEntry[] entries = TimeTrackingEntry.List(FilterElement.EqualElement("ParentBlockId", blockId));
							if (entries.Length == 0)
							{
								TimeTrackingBlock ttb = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), blockId);
								ttb.Delete();
							}
						}
					}

					tran.Commit();
				}

				CommandManager cm = Sender as CommandManager;
				if (cm != null)
				{
					if (deletedItems != elemsToDelete.Length)
						cm.InfoMessage = CHelper.GetResFileString("{IbnFramework.Common:NotAllSelectedItemsWereProcessed}");
					//    cm.InfoMessage = CHelper.GetResFileString("{IbnFramework.Common:AllSelectedItemsWereProcessed}");
				}

				//CHelper.RequireBindGrid();
				CHelper.AddToContext("NeedToClearSelector", "true");
			}
		}
		

		#endregion
	}
}
