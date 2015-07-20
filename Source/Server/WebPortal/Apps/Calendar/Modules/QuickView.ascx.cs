using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Events;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Calendar.Modules
{
	public partial class QuickView : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			CalendarEventEntity ceo = (CalendarEventEntity)BusinessManager.Load(CalendarEventEntity.ClassName, PrimaryKeyId.Parse(Request["ObjectId"]));
			lblTitle.Text = String.Format("<a href=\"javascript:{{window.parent.location.href='{1}';}}\">{0}</a>",
				ceo.Subject, CHelper.GetAbsolutePath("/Apps/MetaUIEntity/Pages/EntityView.aspx?ClassName=CalendarEvent&ObjectId=" + ceo.PrimaryKeyId.Value.ToString()));
			if (ceo.Start.Date == ceo.End.Date)
				lblTime.Text = ceo.Start.ToString("ddd, MMM dd") + ", " + ceo.Start.ToShortTimeString() + " - " + ceo.End.ToShortTimeString();
			else
				lblTime.Text = ceo.Start.ToShortDateString() + " " + ceo.Start.ToShortTimeString() + " - " +
					ceo.End.ToShortDateString() + " " + ceo.End.ToShortTimeString();

			divSeries.Visible = false;
			spanDelete2.Visible = false;
			if (((VirtualEventId)ceo.PrimaryKeyId).RealEventId == ceo.PrimaryKeyId ||
				ceo.CalendarEventExceptionId.HasValue)
			{
				lbGoToEdit.Text = GetGlobalResourceObject("IbnFramework.Calendar", "EditEventDetails").ToString();
				lbDelete.Text = GetGlobalResourceObject("IbnFramework.Calendar", "DeleteQuick").ToString();
			}
			else
			{
				lbGoToEdit.Text = GetGlobalResourceObject("IbnFramework.Calendar", "EditOnlyThisEventInstance").ToString();
				lbEditSeries.Text = GetGlobalResourceObject("IbnFramework.Calendar", "EditAllEventInstances").ToString();
				lbDelete.Text = GetGlobalResourceObject("IbnFramework.Calendar", "DeleteQuickThisInstance").ToString();
				lbDeleteSeries.Text = GetGlobalResourceObject("IbnFramework.Calendar", "DeleteQuickAllInstances").ToString();
				divSeries.Visible = true;
				spanDelete2.Visible = true;
			}

			lbDelete.Click += new EventHandler(lbDelete_Click);
			lbGoToEdit.Click += new EventHandler(lbGoToEdit_Click);
			lbEditSeries.Click += new EventHandler(lbEditSeries_Click);
			lbDeleteSeries.Click += new EventHandler(lbDeleteSeries_Click);
		}

		void lbDeleteSeries_Click(object sender, EventArgs e)
		{
			CommandParameters cp = new CommandParameters(Request["CommandName"]);
			cp.CommandArguments = new Dictionary<string, string>();
			cp.AddCommandArgument("action", "delete");
			PrimaryKeyId pKey = PrimaryKeyId.Parse(Request["ObjectId"]);
			pKey = ((VirtualEventId)pKey).RealEventId;
			cp.AddCommandArgument("Uid", pKey.ToString());
			CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}

		void lbEditSeries_Click(object sender, EventArgs e)
		{
			CommandParameters cp = new CommandParameters(Request["CommandName"]);
			cp.CommandArguments = new Dictionary<string, string>();
			cp.AddCommandArgument("action", "edit");
			PrimaryKeyId pKey = PrimaryKeyId.Parse(Request["ObjectId"]);
			pKey = ((VirtualEventId)pKey).RealEventId;
			cp.AddCommandArgument("Uid", pKey.ToString());
			CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}

		void lbGoToEdit_Click(object sender, EventArgs e)
		{
			CommandParameters cp = new CommandParameters(Request["CommandName"]);
			cp.CommandArguments = new Dictionary<string, string>();
			cp.AddCommandArgument("action", "edit");
			cp.AddCommandArgument("Uid", Request["ObjectId"]);
			CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}

		void lbDelete_Click(object sender, EventArgs e)
		{
			CommandParameters cp = new CommandParameters(Request["CommandName"]);
			cp.CommandArguments = new Dictionary<string, string>();
			cp.AddCommandArgument("action", "delete");
			cp.AddCommandArgument("Uid", Request["ObjectId"]);
			CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
	}
}