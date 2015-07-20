using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Configuration;
using System.Globalization;

namespace Mediachase.Ibn.WebAsp.Pages
{
	public partial class CreateTicket : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Guid companyUid = new Guid(Request["companyUid"].ToString());
			int principalId = int.Parse(Request["principalId"]);

			string ticket = "";
			string login = "";

			using (IDataReader reader = CManage.CreateUserTicket(companyUid, principalId))
			{
				if (reader.Read())
				{
					login = reader["Login"].ToString();
					ticket = reader["Ticket"].ToString();
				}
			}

			ICompanyInfo company = Configurator.Create().GetCompanyInfo(companyUid.ToString());
			string qs = String.Concat(
				"?login=", login, "&ticket=", ticket, "&redirect=",
				Server.UrlEncode("Workspace/default.aspx"));
			int port = (string.IsNullOrEmpty(company.Port) ? -1 : int.Parse(company.Port, CultureInfo.InvariantCulture));
			UriBuilder uriBuilder = new UriBuilder(company.Scheme, company.Host, port, "Public/default.aspx", qs);

			Response.Clear();
			Response.Write(uriBuilder.ToString());
			Response.End();
		}
	}
}
