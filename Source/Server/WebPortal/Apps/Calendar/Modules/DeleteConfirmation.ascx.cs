using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Events;

namespace Mediachase.Ibn.Web.UI.Calendar.Modules
{
	public partial class DeleteConfirmation : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			btnAllSeries.Click += new EventHandler(btnAllSeries_Click);
			btnOnlyThis.Click += new EventHandler(btnOnlyThis_Click);
			btnCancel.Attributes.Add("onclick", CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true));
			lblWarning.Text = GetGlobalResourceObject("IbnFramework.Calendar", "DeleteSeriesWarning").ToString();
			btnOnlyThis.Text = GetGlobalResourceObject("IbnFramework.Calendar", "OnlyThisInstance").ToString();
			btnAllSeries.Text = GetGlobalResourceObject("IbnFramework.Calendar", "AllInstances").ToString();
			btnCancel.Text = GetGlobalResourceObject("IbnFramework.Calendar", "Cancel").ToString();

			PrimaryKeyId pKey = PrimaryKeyId.Parse(Request["ObjectId"]);
			if (pKey == ((VirtualEventId)pKey).RealEventId)
			{
				btnAllSeries.Visible = false;
				lblWarning.Text = GetGlobalResourceObject("IbnFramework.Calendar", "DeleteWarning").ToString();
				btnOnlyThis.Text = GetGlobalResourceObject("IbnFramework.Calendar", "Delete").ToString();
			}
		}

		void btnOnlyThis_Click(object sender, EventArgs e)
		{
			PrimaryKeyId pKey = PrimaryKeyId.Parse(Request["ObjectId"]);
			CommandParameters cp = new CommandParameters(Request["CommandName"]);
			cp.CommandArguments = new Dictionary<string, string>();
			cp.AddCommandArgument("Uid", pKey.ToString());
			CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}

		void btnAllSeries_Click(object sender, EventArgs e)
		{
			PrimaryKeyId pKey = PrimaryKeyId.Parse(Request["ObjectId"]);
			pKey = ((VirtualEventId)pKey).RealEventId;
			CommandParameters cp = new CommandParameters(Request["CommandName"]);
			cp.CommandArguments = new Dictionary<string, string>();
			cp.AddCommandArgument("Uid", pKey.ToString());
			CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
	}
}