using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ListApp.CommandHandlers
{
	public class MetaFieldEditedHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			CHelper.AddToContext("NeedToChangeDataSource", true);
		}

		#endregion
	}
}
