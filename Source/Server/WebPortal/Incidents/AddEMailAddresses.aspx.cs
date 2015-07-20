using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Resources;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Incidents
{
	public partial class AddEMailAddresses : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strViewDocument", typeof(AddEMailAddresses).Assembly);

		#region emails
		protected string emails
		{
			get
			{
				if (Request["emails"] != null)
					return Request["emails"];
				else
					return "";
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetNoStore();
			RegisterLinks();
			ApplyAttributes();
			BindToolbar();
			if (!Page.IsPostBack)
			{
				ViewState["SearchMode"] = false;
				BindGroups();
				BindValues();
			}
		}

		#region BindValues
		private void BindValues()
		{
			string sTo = emails;

			string regex = "([0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*@(([0-9a-zA-Z])+([-\\w]*[0-9a-zA-Z])*\\.)+[a-zA-Z]" +
			  "{2,9})";
			List<string> dic = new List<string>();

			System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline)
			  | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(regex, options);


			foreach (Match item in reg.Matches(sTo))
			{
				if (!dic.Contains(item.Value))
					dic.Add(item.Value);
			}
			string[] _mas = dic.ToArray();

			for (int i = 0; i < _mas.Length; i++)
				lstSelected.Items.Add(new ListItem(_mas[i], _mas[i]));

			Page.ClientScript.RegisterStartupScript(this.GetType(), "_startValues", "SaveFields();", true);
		}
		#endregion

		#region ApplyAttributes
		private void ApplyAttributes()
		{
			btnAdd.InnerText = LocRM.GetString("Add");
			btnDel.InnerText = LocRM.GetString("tDelete");
			btnSearch.Text = LocRM.GetString("FindNow");

			btnSave.Attributes.Add("onclick", "SaveFields();DisableButtons(this);");
			btnAdd.Attributes.Add("onclick", "MoveFew(" + lbUsers.ClientID + "," + lstSelected.ClientID + "); SaveFields(); return false;");
			btnDel.Attributes.Add("onclick", "RemoveFew(" + lstSelected.ClientID + "," + lbUsers.ClientID + "); SaveFields(); return false;");

			lbUsers.Attributes.Add("ondblclick", "MoveFew(" + lbUsers.ClientID + "," + lstSelected.ClientID + "); SaveFields(); return false;");
			lstSelected.Attributes.Add("ondblclick", "RemoveFew(" + lstSelected.ClientID + "," + lbUsers.ClientID + "); SaveFields(); return false;");
		}
		#endregion

		#region RegisterLinks
		private void RegisterLinks()
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/common.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/emailsend.js");
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tAddEMails");
			secHeader.AddLink("<img alt='' src='../Layouts/Images/select.gif'/> " + LocRM.GetString("tSelect"), "javascript:" + Page.ClientScript.GetPostBackEventReference(btnSave, ""));
			secHeader.AddSeparator();
			secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tClose"), "javascript:window.close();");
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			using (IDataReader reader = SecureGroup.GetListGroupsAsTree())
			{
				while (reader.Read())
				{
					string GroupName = CommonHelper.GetResFileString(reader["GroupName"].ToString());
					string GroupId = reader["GroupId"].ToString();
					int Level = (int)reader["Level"];
					for (int i = 1; i < Level; i++)
						GroupName = "  " + GroupName;
					ListItem item = new ListItem(GroupName, GroupId);

					ddGroups.Items.Add(item);
				}
			}

			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindGroupUsers(int.Parse(li.Value));
		}
		#endregion

		#region BindGroupUsers
		private void BindGroupUsers(int GroupID)
		{
			lbUsers.Items.Clear();
			using (IDataReader rdr = SecureGroup.GetListActiveUsersInGroup(GroupID))
			{
				string str = "";
				while (rdr.Read())
				{
					str += rdr["UserId"].ToString() + ",";
					ListItem lItm = lstSelected.Items.FindByValue(rdr["UserId"].ToString());
					if (lItm == null)
						lbUsers.Items.Add(new ListItem((string)rdr["LastName"] + " " + (string)rdr["FirstName"], rdr["UserId"].ToString()));
				}
				iGroupFields.Value = str;
			}
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnSave.Click += new EventHandler(btnSave_Click);
			this.ddGroups.SelectedIndexChanged += new EventHandler(ddGroups_SelectedIndexChanged);
			this.btnSearch.Click += new EventHandler(btnSearch_Click);
		}
		#endregion

		#region Save
		private void btnSave_Click(object sender, EventArgs e)
		{
			string sFields = iFields.Value;
			ArrayList alFields = new ArrayList();
			while (sFields.Length > 0)
			{
				string s = sFields.Substring(0, sFields.IndexOf(","));
				try
				{
					int UserId = int.Parse(s);
					UserLight _ul = UserLight.Load(UserId);
					if (!alFields.Contains(_ul.Email))
						alFields.Add(_ul.Email);
				}
				catch
				{
					if (!alFields.Contains(s))
						alFields.Add(s);
				}
				sFields = sFields.Remove(0, sFields.IndexOf(",") + 1);
			}
			sFields = "";
			foreach (string _smail in alFields)
			{
				string sName = GetNameByEMail(_smail);
				if (sName != "")
					sFields += String.Format("{0} <{1}>; ", sName, _smail);
				else
					sFields += _smail + "; ";
			}

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
			  "<script language=javascript>" +
			  "window.opener.updateEMails('" + sFields + "'); window.close();</script>");
		}
		#endregion

		#region ddGroup - Change
		private void ddGroups_SelectedIndexChanged(object sender, EventArgs e)
		{
			tbSearch.Text = "";
			ViewState["SearchString"] = null;

			BindSelected();

			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindGroupUsers(int.Parse(li.Value));
		}
		#endregion

		#region Search
		private void btnSearch_Click(object sender, EventArgs e)
		{
			BindSelected();
			if (tbSearch.Text != "")
			{
				ViewState["SearchString"] = tbSearch.Text;
				BindSearchedUsers(tbSearch.Text);
			}
		}
		#endregion

		#region BindSelected
		private void BindSelected()
		{
			string sFields = iFields.Value;
			ArrayList alFields = new ArrayList();
			while (sFields.Length > 0)
			{
				alFields.Add(sFields.Substring(0, sFields.IndexOf(",")));
				sFields = sFields.Remove(0, sFields.IndexOf(",") + 1);
			}
			lstSelected.Items.Clear();
			foreach (string i in alFields)
			{
				ListItem lItm = lbUsers.Items.FindByValue(i);
				if (lItm != null)
				{
					lbUsers.Items.Remove(lItm);
					lstSelected.Items.Add(lItm);
				}
				else
				{
					try
					{
						int UserId = int.Parse(i);
						lstSelected.Items.Add(new ListItem(Util.CommonHelper.GetUserStatusPureName(UserId), i));
					}
					catch
					{
						lstSelected.Items.Add(new ListItem(i, i));
					}
				}
			}
		}
		#endregion

		#region BindSearchedUsers
		private void BindSearchedUsers(string searchstr)
		{
			lbUsers.Items.Clear();

			DataView dv = Mediachase.IBN.Business.User.GetListUsersBySubstringDataTable(searchstr).DefaultView;
			dv.Sort = "LastName, FirstName";

			for (int i = 0; i < dv.Count; i++)
			{
				ListItem lItm = lstSelected.Items.FindByValue(dv[i]["UserId"].ToString());
				if (lItm == null)
					lbUsers.Items.Add(new ListItem((string)dv[i]["LastName"] + " " + (string)dv[i]["FirstName"], dv[i]["UserId"].ToString()));
			}
		}
		#endregion

		#region GetNameByEMail
		protected string GetNameByEMail(string eMail)
		{
			int iUserId = Mediachase.IBN.Business.User.GetUserByEmail(eMail);
			if (iUserId > 0)
				return UserLight.Load(iUserId).DisplayName;
			else
			{
				Mediachase.IBN.Business.Client client = Mediachase.IBN.Business.Common.GetClient(eMail);
				if (client != null)
				{
					return client.Name;
				}
				else
					return "";
			}
		}
		#endregion

	}
}