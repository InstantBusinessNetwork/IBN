using System;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Text;

namespace Mediachase.UI.Web.Workspace.Modules
{
	public partial class LastAlert : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strWorkspace", typeof(Assistant).Assembly);

		protected void Page_Load(object sender, EventArgs e)
		{
			BindAlert();
		}

		#region BindAlert
		/// <summary>
		/// Binds the alert.
		/// </summary>
		void BindAlert()
		{
			StringBuilder builder = new StringBuilder();

			using (IDataReader reader = Mediachase.IBN.Business.Common.GetBroadCastMessages(true))
			{
				while (reader.Read())
				{
					builder.AppendFormat(CultureInfo.InvariantCulture, "<div style=\"padding-bottom:10px\"><font color=\"#ff0000\">[&nbsp;{0}&nbsp;{1}&nbsp;]</font><div>{2}</div></div>", ((DateTime)reader["CreationDate"]).ToShortDateString(), ((DateTime)reader["CreationDate"]).ToShortTimeString(), reader["Text"].ToString());
				}
			}

			if (builder.Length == 0)
				builder.AppendFormat(CultureInfo.InvariantCulture, "<font color=\"#808080\">[&nbsp;{0}&nbsp;] - {1}</font>", DateTime.Today.ToShortDateString(), LocRM.GetString("tNoAlerts"));

			lblLastAlert.InnerHtml = builder.ToString();
		}
		#endregion
	}
}
