namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using System.Reflection;

	/// <summary>
	///		Summary description for EMailAntiSpamList.
	/// </summary>
	public partial class EMailAntiSpamList : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		private string constString = "400, 300, false";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbars();
			if (!Page.IsPostBack)
				BindDG();
		}

		#region BindToolbars
		private void BindToolbars()
		{
			secHeader.Title = LocRM.GetString("tAntiSpamList");
			secHeader.AddLink(String.Format("<img align='absmiddle' border='0' src='{0}' />&nbsp;", this.Page.ResolveUrl("~/layouts/images/newitem.gif")) + LocRM.GetString("tAddRule"),
				"javascript:OpenWindow('" + this.Page.ResolveUrl("~/Admin/EMailAntiSpamEdit.aspx") + "', " + constString + ")");
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			int i = 1;
			dgRules.Columns[i++].HeaderText = LocRM.GetString("tWeight");
			dgRules.Columns[i++].HeaderText = LocRM.GetString("tAction");
			dgRules.Columns[i++].HeaderText = LocRM.GetString("tKey");
			dgRules.Columns[i++].HeaderText = LocRM.GetString("tType");
			dgRules.Columns[i++].HeaderText = LocRM.GetString("tValue");

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Weight", typeof(int)));
			dt.Columns.Add(new DataColumn("IsAccept", typeof(bool)));
			dt.Columns.Add(new DataColumn("Key", typeof(string)));
			dt.Columns.Add(new DataColumn("Type", typeof(string)));
			dt.Columns.Add(new DataColumn("Value", typeof(string)));
			DataRow dr;
			EMailMessageAntiSpamRule[] mas = EMailMessageAntiSpamRule.List();
			foreach (EMailMessageAntiSpamRule asp in mas)
			{
				dr = dt.NewRow();
				dr["Id"] = asp.AntiSpamRuleId;
				dr["Weight"] = asp.Weight;
				dr["IsAccept"] = asp.Accept;
				dr["Key"] = GetKey(asp.Key);
				switch (asp.RuleType)
				{
					case EMailMessageAntiSpamRuleType.Contains:
						dr["Type"] = LocRM.GetString("tContains");
						break;
					case EMailMessageAntiSpamRuleType.IsEqual:
						dr["Type"] = LocRM.GetString("tIsEqual");
						break;
					case EMailMessageAntiSpamRuleType.RegexMatch:
						dr["Type"] = LocRM.GetString("tRegExMatch");
						break;
					case EMailMessageAntiSpamRuleType.Service:
						dr["Type"] = LocRM.GetString("tService");
						break;
					default:
						dr["Type"] = "";
						break;
				}
				dr["Value"] = asp.Value;
				dt.Rows.Add(dr);
			}

			DataView dv = dt.DefaultView;
			dv.Sort = "Weight";

			dgRules.DataSource = dv;
			dgRules.DataBind();

			foreach (DataGridItem dgi in dgRules.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
				{
					ib.Attributes.Add("title", LocRM.GetString("tDelete"));
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning3") + "')");
				}
			}
		}
		#endregion

		#region Protected DG Strings
		private string GetKey(string _key)
		{
			switch (_key)
			{
				case "BlackList":
					_key = LocRM.GetString("tBlackList2");
					break;
				case "WhiteList":
					_key = LocRM.GetString("tWhiteList2");
					break;
				case "Ticket":
					_key = LocRM.GetString("tTicket");
					break;
				case "IncidentBoxRules":
					_key = LocRM.GetString("tIssBoxRulesService");
					break;
				default:
					break;
			}
			return _key;
		}
		protected string GetIcon(bool IsAccept)
		{
			string retVal = "";
			if (IsAccept)
				retVal = String.Format("<img align='absmiddle' border='0' src='{0}' />",
					this.Page.ResolveUrl("~/layouts/images/accept_green.gif"));
			else
				retVal = String.Format("<img align='absmiddle' border='0' src='{0}' />",
					this.Page.ResolveUrl("~/layouts/images/deny.gif"));
			return retVal;
		}

		protected string GetLink(int Id, string sKey)
		{
			string retVal = "";
			retVal = String.Format("<a href=\"javascript:OpenWindow('{0}', " + constString + ")\">{1}</a>",
				this.Page.ResolveUrl("~/Admin/EMailAntiSpamEdit.aspx") + "?RuleId=" + Id.ToString(),
				sKey);
			return retVal;
		}

		protected string GetEditButton(int id, string Tooltip)
		{
			return String.Format("<a href=\"javascript:OpenWindow('{3}', {2})\"><img border='0' align='absmiddle' src='{0}' title='{1}'/></a>",
				this.Page.ResolveUrl("~/layouts/images/edit.gif"),
				Tooltip, constString,
				this.Page.ResolveUrl("~/Admin/EMailAntiSpamEdit.aspx") + "?RuleId=" + id.ToString());
		}

		#endregion

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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgRules.DeleteCommand += new DataGridCommandEventHandler(dg_DeleteCommand);
		}
		#endregion

		#region DataGrid_Events
		private void dg_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int Id = int.Parse(e.CommandArgument.ToString());
			EMailMessageAntiSpamRule.Delete(Id);
			Response.Redirect("~/Admin/EMailAntiSpamList.aspx");
		}
		#endregion
	}
}
