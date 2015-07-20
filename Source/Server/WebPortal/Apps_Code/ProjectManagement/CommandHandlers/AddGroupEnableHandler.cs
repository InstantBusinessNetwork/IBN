using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.CommandHandlers
{
	public class AddGroupEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			bool fl = false;
			
			if (((Control)Sender).Page.Request["ObjectTypeId"] != null && ((Control)Sender).Page.Request["ObjectId"] != null)
			{
				int otypeid = int.Parse(((Control)Sender).Page.Request["ObjectTypeId"]);
				int oid = int.Parse(((Control)Sender).Page.Request["ObjectId"]);
				switch (otypeid)
				{
					case (int)ObjectTypes.ToDo:
						using (IDataReader rdr = ToDo.GetToDo(oid))
						{
							if (rdr.Read())
							{
								int compltype = (int)rdr["CompletionTypeId"];
								if (compltype != (int)Mediachase.IBN.Business.CompletionType.All)
									fl = true;
							}
						}
						break;
					case (int)ObjectTypes.Task:
						using (IDataReader rdr = Task.GetTask(oid))
						{
							if (rdr.Read())
							{
								int compltype = (int)rdr["CompletionTypeId"];
								if (compltype != (int)Mediachase.IBN.Business.CompletionType.All)
									fl = true;
							}
						}
						break;
					default:
						break;
				}
			}
			
			return fl;
		}

		#endregion
	}
}
