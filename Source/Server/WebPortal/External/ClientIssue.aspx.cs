using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn;
using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.External
{
	/// <summary>
	/// Summary description for ClientIssue.
	/// </summary>
	public partial class ClientIssue : System.Web.UI.Page
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentView", typeof(ClientIssue).Assembly);

		#region IncidentId
		private int IncidentId
		{
			get
			{
				int retval = -1;
				if (Request["IncidentId"] != null)
				{
					try
					{
						retval = int.Parse(Request["IncidentId"]);
					}
					catch {}
				}
				else if (Request["IssueId"] != null)
				{
					try
					{
						retval = int.Parse(Request["IssueId"]);
					}
					catch {}
				}
				return retval;
			}
		}
		#endregion

		#region ExternalID
		private string ExternalID
		{
			get
			{
				return Request.QueryString["guid"];
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			CheckSecurity();
			pT.Title = LocRM.GetString("tabForum");
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion

		#region CheckSecurity
		private void CheckSecurity()
		{
			int iObjectTypeId = -1;
			int iObjectId = -1;
			string sEmail = "";

			using (IDataReader reader = Mediachase.IBN.Business.User.GetGateByGuid(ExternalID))
			{
				if (reader.Read())
				{
					iObjectTypeId = (int)reader["ObjectTypeId"];
					iObjectId = (int)reader["ObjectId"];
					sEmail = reader["Email"].ToString();
				}
			}

			if (iObjectTypeId != (int)ObjectTypes.Issue || iObjectId != IncidentId)
				throw new AccessDeniedException();

		}
		#endregion
	}
}
