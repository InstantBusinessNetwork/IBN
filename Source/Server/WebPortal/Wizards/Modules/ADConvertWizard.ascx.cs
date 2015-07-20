using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.Import;
using Mediachase.Ibn.Service;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus.Import;
using Mediachase.MetaDataPlus.Import.Parser;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.Interfaces;
using Mediachase.Ibn.Web.UI.WebControls;


namespace Mediachase.UI.Web.Wizards.Modules
{
	/// <summary>
	///		Summary description for ADConvertWizard.
	/// </summary>
	public partial class ADConvertWizard : System.Web.UI.UserControl, IWizardControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Wizards.Resources.strNewUsWd", typeof(ADConvertWizard).Assembly);
		ArrayList subtitles = new ArrayList();
		ArrayList steps = new ArrayList();
		private int _stepCount = 6;
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterScript(Page, "~/Scripts/List2List.js");

			if (!Page.IsPostBack)
				BindStep1();
			btnSave.Text = LocRM.GetString("tSave");
			btnSave2.Text = LocRM.GetString("tSave");
			btnSave2.Attributes.Add("onclick", "SaveGroups();");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnSave2.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
		}

		#region BindStep1
		private void BindStep1()
		{
			if (User.CanImportFromActiveDirectory())
				rbSourceObject.Items.Add(new ListItem(" " + LocRM.GetString("tAD"), "0"));
			rbSourceObject.Items.Add(new ListItem(" " + LocRM.GetString("tFile"), "1"));

			rbSourceType.Items.Add(new ListItem(" " + LocRM.GetString("tExcel"), "0"));
			rbSourceType.Items.Add(new ListItem(" " + LocRM.GetString("tXML"), "1"));

			rbSourceObject.Items[0].Selected = true;
			rbSourceType.Items[0].Selected = true;

			ddLDAPSettings.DataSource = LdapSettings.Get(-1);
			ddLDAPSettings.DataTextField = "Title";
			ddLDAPSettings.DataValueField = "LdapId";
			ddLDAPSettings.DataBind();
			ddLDAPSettings.Items.Add(new ListItem(LocRM.GetString("tChoose"), "0"));

			if (pc["ADDomain"] != null)
				txtDomain.Text = pc["ADDomain"];
			if (pc["ADUserName"] != null)
				txtUserName.Text = pc["ADUserName"];
			if (pc["ADLDAPSettings"] != null)
				Util.CommonHelper.SafeSelect(ddLDAPSettings, pc["ADLDAPSettings"]);
			lgdSourceType.InnerText = LocRM.GetString("tSourceType");
			lgdFields.InnerText = LocRM.GetString("CompareFields");
			lgdUserList.InnerText = LocRM.GetString("tUserList");
			//lgdUserList2.InnerText = LocRM.GetString("tUserList");
			lgdRolesGroups.InnerText = LocRM.GetString("tGroups");
			lgdUserInfo.InnerText = LocRM.GetString("tUserInfo");
			lblConnectError.Text = LocRM.GetString("ConnectError");
			cvUserLogin.ErrorMessage = LocRM.GetString("DuplicatedLogin");
			cvWindowsLogin.ErrorMessage = LocRM.GetString("DuplicatedWindowsLogin");
			cvEmail.ErrorMessage = LocRM.GetString("EmailDuplicate");
			GroupValidator.ErrorMessage = LocRM.GetString("GroupSelectError");

			btnAddOneGr.Attributes.Add("onclick", "MoveOne(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); return false;");
			btnAddAllGr.Attributes.Add("onclick", "MoveAll(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); return false;");
			btnRemoveOneGr.Attributes.Add("onclick", "MoveOne(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + "); return false;");
			btnRemoveAllGr.Attributes.Add("onclick", "MoveAll(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + ");return false;");

			lbAvailableGroups.Attributes.Add("ondblclick", "MoveOne(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); return false;");
			lbSelectedGroups.Attributes.Add("ondblclick", "MoveOne(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + "); return false;");

			txtCommonPassword.Text = "ibn";
		}
		#endregion

		private void ShowStep(int step)
		{
			for (int i = 0; i <= _stepCount; i++)
				((Panel)steps[i]).Visible = false;

			((Panel)steps[step - 1]).Visible = true;

			#region Step 2
			if (step == 2)
			{
				if (rbSourceObject.SelectedValue == "0")
				{
					lgdConnectInf.InnerText = LocRM.GetString("ConnectToAD");
					tblADConnection.Visible = true;
					tblFileConnection.Visible = false;
					ChangedLdap();
					tblFilter.Visible = true;
				}
				else
				{
					lgdConnectInf.InnerText = LocRM.GetString("tLoadToServer");
					tblADConnection.Visible = false;
					tblFileConnection.Visible = true;
					((WizardTemplate)Page.Controls[0]).btnNext.Attributes.Add("onclick", "ShowProgress();");
					tblFilter.Visible = false;
				}
				ViewState["Fields"] = null;
				ViewState["ADFields"] = null;
				ViewState["ldapPassword"] = null;
				grdFields.EditItemIndex = -1;
				tbFilter.Text = "";
				wwwPath.Value = "";

				((WizardTemplate)Page.Controls[0]).btnNext.Disabled = false;
			}
			#endregion

			#region Step 3
			if (step == 3)
			{
				if (ViewState["Fields"] == null || ViewState["Fields"].ToString().Length == 0)
				{
					DataTable dtFields = new DataTable();
					dtFields.Columns.Add(new DataColumn("IBNField", typeof(string)));
					dtFields.Columns.Add(new DataColumn("IBNFieldDisplay", typeof(string)));
					dtFields.Columns.Add(new DataColumn("ADField", typeof(string)));
					dtFields.Columns.Add(new DataColumn("realADField", typeof(string)));
					DataRow dr1;
					string[] sarray = null;
					string[] alIBNFields = UserInfo.PropertyNamesIbn;
					if (rbSourceObject.SelectedValue == "0")
					{
						pc["ADDomain"] = txtDomain.Text;
						pc["ADUserName"] = txtUserName.Text;
						pc["ADLDAPSettings"] = ddLDAPSettings.SelectedValue;

						LdapSettings lsets = null;
						if (int.Parse(ddLDAPSettings.SelectedValue) > 0)
							lsets = LdapSettings.Load(int.Parse(ddLDAPSettings.SelectedValue));
						else
							lsets = new Mediachase.IBN.Business.LdapSettings();

						string sPassword = txtPassword.Text;

						if (txtDomain.Text == lsets.Domain && txtUserName.Text == lsets.Username && txtPassword.Text == "")
							sPassword = lsets.Password;
						ViewState["ldapPassword"] = sPassword;

						ActiveDirectory ad = new ActiveDirectory(txtDomain.Text, txtUserName.Text, sPassword, tbFilter.Text);

						tbFilter.Text = lsets.Filter;
						foreach (string name in alIBNFields)
						{
							dr1 = dtFields.NewRow();
							dr1["IBNField"] = name;
							dr1["IBNFieldDisplay"] = Mediachase.Ibn.Web.UI.CHelper.GetResFileString(String.Format("{{IbnFramework.Common:{0}}}", name));
							dr1["ADField"] = ad.FieldsMatch[name];
							dr1["realADField"] = ad.FieldsMatch[name];
							if (ddLDAPSettings.SelectedValue != "0")
							{
								foreach (LdapField lf in lsets.Fields)
									if (lf.IbnName == name)
									{
										dr1["ADField"] = lf.LdapName;
										dr1["realADField"] = lf.LdapName;
										break;
									}
							}
							dtFields.Rows.Add(dr1);
						}
						sarray = ad.GetProperties();
					}
					else
					{
						if (fSourceFile.PostedFile != null && fSourceFile.PostedFile.ContentLength > 0)
						{
							ProcessFileCache(Server.MapPath(CommonHelper.ChartPath));
							String dir = CommonHelper.ChartPath;
							string wwwpath = dir + Guid.NewGuid().ToString();
							switch (rbSourceType.SelectedValue)
							{
								case "0":
									//wwwpath += ".xls";
									// OZ: Added XLS and XLSX extensions
									wwwpath += Path.GetExtension(fSourceFile.PostedFile.FileName);
									break;
								case "1":
									wwwpath += ".xml";
									break;
								default:
									break;
							}
							wwwPath.Value = wwwpath;
							using (Stream sw = File.Create(Server.MapPath(wwwpath)))
							{
								fSourceFile.PostedFile.InputStream.Seek(0, SeekOrigin.Begin);
								System.IO.BinaryReader br = new System.IO.BinaryReader(fSourceFile.PostedFile.InputStream);
								int iBufferSize = 655360; // 640 KB
								byte[] outbyte = br.ReadBytes(iBufferSize);

								while (outbyte.Length > 0)
								{
									sw.Write(outbyte, 0, outbyte.Length);
									outbyte = br.ReadBytes(iBufferSize);
								}
								br.Close();
							}

							IIncomingDataParser parser = null;
							DataSet rawData = null;
							switch (rbSourceType.SelectedIndex)
							{
								case 0:
									IMCOleDBHelper helper = (IMCOleDBHelper)Activator.GetObject(typeof(IMCOleDBHelper), System.Configuration.ConfigurationManager.AppSettings["McOleDbServiceString"]);
									rawData = helper.ConvertExcelToDataSet(Server.MapPath(wwwpath));

									break;
								case 1:
									parser = new XmlIncomingDataParser();
									rawData = parser.Parse(Server.MapPath(wwwPath.Value), null);
									break;
							}

							DataTable dtSource = rawData.Tables[0];
							int i = 0;
							sarray = new string[dtSource.Columns.Count];
							foreach (DataColumn dc in dtSource.Columns)
							{
								sarray[i++] = dc.ColumnName;
							}

							foreach (string name in alIBNFields)
							{
								dr1 = dtFields.NewRow();
								dr1["IBNField"] = name;
								dr1["IBNFieldDisplay"] = Mediachase.Ibn.Web.UI.CHelper.GetResFileString(String.Format("{{IbnFramework.Common:{0}}}", name));
								dr1["ADField"] = LocRM.GetString("tNotSet");
								dr1["realADField"] = "0";

								foreach (string str in sarray)
									if (String.Compare(str, name, true) == 0 ||
										String.Compare(str, Mediachase.Ibn.Web.UI.CHelper.GetResFileString(String.Format("{{IbnFramework.Common:{0}}}", name)), true) == 0)
									{
										dr1["ADField"] = str;
										dr1["realADField"] = str;
									}
								dtFields.Rows.Add(dr1);
							}
						}
						else
							((WizardTemplate)Page.Controls[0]).btnNext.Disabled = true;
					}



					ViewState["Fields"] = dtFields;
					grdFields.DataSource = dtFields.DefaultView;
					grdFields.DataBind();

					if (sarray != null)
					{
						string sValues = String.Join(",", sarray);
						ViewState["ADFields"] = sValues;
					}
				}
				else
				{
					BindDG();
				}
				ViewState["Users"] = null;
				ViewState["FullUsers"] = null;
			}
			#endregion

			#region Step 4
			if (step == 4)
			{
				grdFields.EditItemIndex = -1;
				DataView dv = null;
				if (ViewState["FullUsers"] == null || ViewState["FullUsers"].ToString().Length == 0)
				{
					string[] alIBNFields = UserInfo.PropertyNamesIbn;
					DataTable dtFields = (DataTable)ViewState["Fields"];

					DataTable dt = new DataTable();
					dt.Columns.Add(new DataColumn("Add", typeof(bool)));
					dt.Columns.Add(new DataColumn("Weight", typeof(int)));
					dt.Columns.Add(new DataColumn("Login", typeof(string)));
					dt.Columns.Add(new DataColumn("FirstName", typeof(string)));
					dt.Columns.Add(new DataColumn("LastName", typeof(string)));
					dt.Columns.Add(new DataColumn("Email", typeof(string)));
					dt.Columns.Add(new DataColumn("Phone", typeof(string)));
					dt.Columns.Add(new DataColumn("Fax", typeof(string)));
					dt.Columns.Add(new DataColumn("Mobile", typeof(string)));
					dt.Columns.Add(new DataColumn("Company", typeof(string)));
					dt.Columns.Add(new DataColumn("JobTitle", typeof(string)));
					dt.Columns.Add(new DataColumn("Department", typeof(string)));
					dt.Columns.Add(new DataColumn("Location", typeof(string)));
					dt.Columns.Add(new DataColumn("BadLogin", typeof(bool)));
					dt.Columns.Add(new DataColumn("BadEmail", typeof(bool)));
					//dt.Columns.Add(new DataColumn("BadWinLogin", typeof(bool)));
					dt.Columns.Add(new DataColumn("IsBad", typeof(bool)));
					dt.Columns.Add(new DataColumn("IsBadGroup", typeof(bool)));
					dt.Columns.Add(new DataColumn("Groups", typeof(string)));
					dt.Columns.Add(new DataColumn("IMGroup", typeof(int)));
					dt.Columns.Add(new DataColumn("LdapUid", typeof(string)));
					dt.Columns.Add(new DataColumn("WindowsLogin", typeof(string)));
					DataRow dr;

					if (rbSourceObject.SelectedValue == "0")
					{
						string sPassword = ViewState["ldapPassword"].ToString();

						if (String.IsNullOrEmpty(sPassword) && int.Parse(ddLDAPSettings.SelectedValue) > 0)
						{
							LdapSettings lsets = LdapSettings.Load(int.Parse(ddLDAPSettings.SelectedValue));
							if (txtDomain.Text == lsets.Domain && txtUserName.Text == lsets.Username && txtPassword.Text == "")
								sPassword = lsets.Password;
						}

						ActiveDirectory ad = new ActiveDirectory(txtDomain.Text, txtUserName.Text, sPassword, tbFilter.Text);

						foreach (string s in alIBNFields)
						{
							DataRow[] drMas = dtFields.Select("IBNField = '" + s + "'");
							if (drMas.Length > 0)
							{
								string ss = drMas[0]["realADField"].ToString();
								if (ss != "0" && !ss.Equals(ad.FieldsMatch[s]))
								{
									ad.FieldsMatch[s] = ss;
								}
							}
						}
						ArrayList alUsers = ad.GetUsers();


						foreach (UserInfo _ui in alUsers)
						{
							dr = dt.NewRow();
							dr["Login"] = _ui.Login;
							dr["FirstName"] = _ui.FirstName;
							dr["LastName"] = _ui.LastName;
							dr["Email"] = _ui.Email;
							if (_ui.FirstName != "" && _ui.LastName != "" && _ui.Email != "")
								dr["Add"] = true;
							else
								dr["Add"] = false;
							if (_ui.FirstName != "" && _ui.LastName != "")
								dr["Weight"] = 0;
							else
								dr["Weight"] = 1;
							dr["Phone"] = _ui.Phone;
							dr["Fax"] = _ui.Fax;
							dr["Mobile"] = _ui.Mobile;
							dr["Company"] = _ui.Company;
							dr["JobTitle"] = _ui.JobTitle;
							dr["Department"] = _ui.Department;
							dr["Location"] = _ui.Location;
							dr["BadLogin"] = false;
							dr["BadEmail"] = false;
							//dr["BadWinLogin"] = false;
							dr["IsBad"] = false;
							dr["IsBadGroup"] = false;
							dr["Groups"] = ((int)InternalSecureGroups.ProjectManager).ToString() + ",";
							dr["IMGroup"] = -1;
							dr["LdapUid"] = _ui.LdapUid;
							dr["WindowsLogin"] = _ui.WindowsLogin;
							dt.Rows.Add(dr);
						}
					}
					else
					{
						IIncomingDataParser parser = null;
						DataSet rawData = null;
						switch (rbSourceType.SelectedIndex)
						{
							case 0:
								// [03.06.05] fix some problems with oledb and asp.net
								IMCOleDBHelper helper = (IMCOleDBHelper)Activator.GetObject(typeof(IMCOleDBHelper), System.Configuration.ConfigurationManager.AppSettings["McOleDbServiceString"]);
								rawData = helper.ConvertExcelToDataSet(Server.MapPath(wwwPath.Value));
								break;
							case 1:
								parser = new XmlIncomingDataParser();
								rawData = parser.Parse(Server.MapPath(wwwPath.Value), null);
								break;
						}

						DataTable dtSource = rawData.Tables[0];

						string sValues = ViewState["ADFields"].ToString();
						ArrayList alADFields = new ArrayList(sValues.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
						int count = 1;
						foreach (DataRow drFile in dtSource.Rows)
						{
							dr = dt.NewRow();
							dr["Add"] = true;
							dr["Weight"] = 0;
							dr["BadLogin"] = false;
							dr["BadEmail"] = false;
							//dr["BadWinLogin"] = false;
							dr["IsBad"] = false;
							dr["IsBadGroup"] = false;
							dr["Groups"] = ((int)InternalSecureGroups.ProjectManager).ToString() + ",";
							dr["IMGroup"] = -1;
							bool flLogin = false;
							foreach (string sname in alADFields)
							{
								DataRow[] drF = dtFields.Select("realADField = '" + sname + "'");
								if (drF.Length > 0)
								{
									string ss = drF[0]["IBNField"].ToString();
									if (ss == "Login")
										flLogin = true;
									dr[ss] = drFile[sname];
								}
							}
							if (!flLogin)
								dr["Login"] = "login" + (count++).ToString();
							dt.Rows.Add(dr);
						}
					}

					ViewState["Users"] = dt;
					DataTable dtFull = dt.Clone();
					DataRow drNew;
					foreach (DataRow dr1 in dt.Rows)
					{
						drNew = dtFull.NewRow();
						drNew.ItemArray = dr1.ItemArray;
						dtFull.Rows.Add(drNew);
					}
					ViewState["FullUsers"] = dtFull;

					dv = dt.DefaultView;
				}
				else
					dv = ((DataTable)ViewState["FullUsers"]).DefaultView;

				dv.Sort = "Weight, LastName, FirstName, Login";
				dgUsers.DataSource = dv;
				dgUsers.DataBind();

				((WizardTemplate)Page.Controls[0]).btnNext.CausesValidation = true;
				((WizardTemplate)Page.Controls[0]).btnNext.Disabled = false;
			}
			#endregion

			#region Step 5
			if (step == 5)
			{
				DataTable dt = (DataTable)ViewState["Users"];
				DataTable dtFull = (DataTable)ViewState["FullUsers"];
				string sExceptions = "";
				foreach (DataGridItem item in dgUsers.Items)
				{
					foreach (Control control in item.Cells[0].Controls)
					{
						if (control is HtmlInputCheckBox)
						{
							HtmlInputCheckBox checkBox = (HtmlInputCheckBox)control;
							//*****Undo*****
							//string filterElement = "Login = '" + item.Cells[3].Text + "'";
							try
							{
								//DataRow[] dr = dt.Select(filterElement);
								//DataRow[] drF = dtFull.Select(filterElement);
								DataRow dr = GetRowIsEqual(dt, "Login", item.Cells[3].Text);
								DataRow drF = GetRowIsEqual(dtFull, "Login", item.Cells[3].Text);
								if (!checkBox.Checked)
								{
									//if (dr.Length > 0)
									//    dt.Rows.Remove(dr[0]);
									//if (drF.Length > 0)
									//    drF[0]["Add"] = false;
									if (dr != null)
										dt.Rows.Remove(dr);
									if (drF != null)
										drF["Add"] = false;
								}
								else
								{
									//if (drF.Length > 0)
									//    drF[0]["Add"] = true;
									//if (dr.Length == 0 && drF.Length > 0)
									//{
									//    DataRow drN = dt.NewRow();
									//    drN.ItemArray = drF[0].ItemArray;
									//    dt.Rows.Add(drN);
									//}
									if (drF != null)
										drF["Add"] = true;
									if (dr == null && drF != null)
									{
										DataRow drN = dt.NewRow();
										drN.ItemArray = drF.ItemArray;
										dt.Rows.Add(drN);
									}
								}
							}
							catch
							{
								sExceptions += item.Cells[3].Text + "\r\n";
							}
						}
					}
				}

				foreach (DataRow bad in dt.Rows)
				{
					string sEmail = bad["Email"].ToString();
					if (sEmail.Length == 0 || User.GetUserByEmail(sEmail) != -1)
						bad["BadEmail"] = true;
					string sLogin = bad["Login"].ToString();
					if (sLogin.Length == 0 || User.GetUserByLogin(sLogin) != -1)
						bad["BadLogin"] = true;
					//string sWinLogin = bad["WindowsLogin"].ToString();
					//if (sWinLogin.Length == 0 || User.GetUserByWindowsLogin(sWinLogin) != -1)
					//    bad["BadWinLogin"] = true;

					if ((bool)bad["BadEmail"] || (bool)bad["BadLogin"]/* || (bool)bad["BadWinLogin"]*/)
						bad["IsBad"] = true;
				}
				ViewState["Users"] = dt;
				ViewState["UsersGroups"] = null;
				ViewState["FullUsers"] = dtFull;
				if (!String.IsNullOrEmpty(sExceptions))
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
						String.Format("alert('Bad Logins:\r\n{0}');", sExceptions), true);

				DataView dv = dt.DefaultView;
				dv.Sort = "IsBad DESC, Login";

				dlUsers.DataSource = dv;
				dlUsers.DataBind();
				dlUsers.SelectedIndex = 0;
				LinkButton lb = (LinkButton)dlUsers.Items[0].FindControl("lbUser");
				if (lb != null)
					BinddgGroupsUsers(lb.CommandName);

				((WizardTemplate)Page.Controls[0]).btnNext.CausesValidation = false;
				((WizardTemplate)Page.Controls[0]).btnNext.Disabled = false;
			}
			#endregion

			#region Step 6
			if (step == 6)
			{
				ddIMGroups.DataTextField = "IMGroupName";
				ddIMGroups.DataValueField = "IMGroupId";
				ddIMGroups.DataSource = IMGroup.GetListIMGroupsWithoutPartners();
				ddIMGroups.DataBind();

				DataTable dt = (DataTable)ViewState["Users"];
				DataTable dtFullOwn = dt.Clone();
				DataRow drNew;
				foreach (DataRow dr1 in dt.Rows)
				{
					drNew = dtFullOwn.NewRow();
					drNew.ItemArray = dr1.ItemArray;
					dtFullOwn.Rows.Add(drNew);
				}
				foreach (DataRow bad in dtFullOwn.Rows)
				{
					if ((bool)bad["IsBad"])
					{
						string sLogin = bad["Login"].ToString();
						#region *****Undo*****
						//DataRow[] dr = dt.Select("Login = '" + sLogin + "'");
						//if (dr.Length > 0)
						//    dt.Rows.Remove(dr[0]); 
						#endregion
						DataRow dr = GetRowIsEqual(dt, "Login", sLogin);
						if (dr != null)
							dt.Rows.Remove(dr);
					}
				}

				foreach (DataRow dr1 in dt.Rows)
				{
					dr1["IMGroup"] = int.Parse(ddIMGroups.SelectedValue);
					if (dr1["Groups"].ToString().Length < 2)
						dr1["IsBadGroup"] = true;
				}

				ViewState["UsersGroups"] = dt;

				DataView dv = dt.DefaultView;
				dv.Sort = "IsBadGroup DESC, LastName, FirstName";

				dlUserGroups.DataSource = dv;
				dlUserGroups.DataBind();
				if (dlUserGroups.Items.Count > 0)
				{
					dlUserGroups.SelectedIndex = 0;
					LinkButton lb = (LinkButton)dlUserGroups.Items[0].FindControl("lbUser2");
					if (lb != null)
						BindGroups(lb.CommandName, true);
					btnSave2.Disabled = false;
				}
				else
				{
					((WizardTemplate)Page.Controls[0]).btnNext.Disabled = true;
					btnSave2.Disabled = true;
				}
				((WizardTemplate)Page.Controls[0]).btnNext.CausesValidation = false;
			}
			#endregion

			#region Step 7 - Save
			if (step == 7)
			{
				DataTable dt = (DataTable)ViewState["UsersGroups"];
				ArrayList alUsers = new ArrayList();
				foreach (DataRow saverow in dt.Rows)
				{
					string sGroups = saverow["Groups"].ToString();
					ArrayList alGroups = new ArrayList();
					while (sGroups.Length > 0)
					{
						alGroups.Add(Int32.Parse(sGroups.Substring(0, sGroups.IndexOf(","))));
						sGroups = sGroups.Remove(0, sGroups.IndexOf(",") + 1);
					}
					if (alGroups.Count > 0)
					{
						UserInfo _ui = new UserInfo();

						_ui.Login = saverow["Login"].ToString();
						_ui.FirstName = saverow["FirstName"].ToString();
						_ui.LastName = saverow["LastName"].ToString();
						_ui.Email = saverow["EMail"].ToString();
						_ui.Phone = saverow["Phone"].ToString();
						_ui.Fax = saverow["Fax"].ToString();
						_ui.Mobile = saverow["Mobile"].ToString();
						_ui.Company = saverow["Company"].ToString();
						_ui.JobTitle = saverow["JobTitle"].ToString();
						_ui.Department = saverow["Department"].ToString();
						_ui.Location = saverow["Location"].ToString();
						_ui.Groups = alGroups;
						_ui.ImGroupId = int.Parse(saverow["IMGroup"].ToString());
						_ui.WindowsLogin = saverow["WindowsLogin"].ToString();

						alUsers.Add(_ui);
					}
				}
				bool fl = false;
				if (alUsers.Count > 0)
					try
					{
						string password = "ibn";
						if (!String.IsNullOrEmpty(txtCommonPassword.Text.Trim()))
							password = txtCommonPassword.Text.Trim();
						User.CreateMultiple(alUsers, password, Security.CurrentUser.LanguageId);
					}
					catch
					{
						fl = true;
					}
				else
					fl = true;
				if (fl)
					lblError.Text = LocRM.GetString("tWereErrors");
				else
				{
					foreach (UserInfo _ui in alUsers)
					{
						lblError.Text += CommonHelper.GetUserStatusUL(User.GetUserByLogin(_ui.Login)) + "<br>";
					}
				}
			}
			#endregion

		}

		#region GetGroupTitleById
		private string GetGroupTitleById(int GrId)
		{
			///		GroupId, GroupName, IMGroupId
			using (IDataReader reader = SecureGroup.GetGroup(GrId))
			{
				if (reader.Read())
					return CommonHelper.GetResFileString(reader["GroupName"].ToString());
			}
			return String.Empty;
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

			subtitles.Add(LocRM.GetString("s0Convert"));
			subtitles.Add(LocRM.GetString("s1Convert"));
			subtitles.Add(LocRM.GetString("s2Convert"));
			subtitles.Add(LocRM.GetString("s3Convert"));
			subtitles.Add(LocRM.GetString("s4Convert"));
			subtitles.Add(LocRM.GetString("s5Convert"));
			subtitles.Add(LocRM.GetString("s6Convert"));

			steps.Add(step0);
			steps.Add(step1);
			steps.Add(step2);
			steps.Add(step3);
			steps.Add(step4);
			steps.Add(step5);
			steps.Add(step6);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.cvLogin.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cvLogin_ServerValidate);
			this.cvAtLeastOne.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cvAtLeastOne_ServerValidate);
			this.cvUserLogin.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cvUserLogin_ServerValidate);
			this.cvEmail.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cvEmail_ServerValidate);
			this.cvWindowsLogin.ServerValidate += new ServerValidateEventHandler(cvWindowsLogin_ServerValidate);
			this.grdFields.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_cancel);
			this.grdFields.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_edit);
			this.grdFields.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_update);
			this.dlUsers.ItemCommand += new System.Web.UI.WebControls.DataListCommandEventHandler(this.dlUsers_ItemCommand);
			this.GroupValidator.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.GroupValidator_ServerValidate);
			this.dlUserGroups.ItemCommand += new System.Web.UI.WebControls.DataListCommandEventHandler(this.dlUserGroups_ItemCommand);
			this.ddLDAPSettings.SelectedIndexChanged += new EventHandler(ddLDAPSettings_SelectedIndexChanged);
		}
		#endregion

		#region BindDGFields
		private void BindDG()
		{
			DataTable dtFields = (DataTable)ViewState["Fields"];

			grdFields.DataSource = dtFields.DefaultView;
			grdFields.DataBind();

			string sValues = ViewState["ADFields"].ToString();
			ArrayList alADFields = new ArrayList(sValues.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
			foreach (DataGridItem dgi in grdFields.Items)
			{
				DropDownList ddl = (DropDownList)dgi.FindControl("ddADFields");
				if (ddl != null)
				{
					ddl.Items.Add(new ListItem(LocRM.GetString("tNotSet"), "0"));
					foreach (string s in alADFields)
						ddl.Items.Add(new ListItem(s, s));
				}
			}
		}
		#endregion

		#region DataList - Users
		private void dlUsers_ItemCommand(object source, System.Web.UI.WebControls.DataListCommandEventArgs e)
		{
			string sLogin = e.CommandName;
			dlUsers.SelectedIndex = e.Item.ItemIndex;
			BinddgGroupsUsers(sLogin);
			string NewHref = ((LinkButton)e.CommandSource).ClientID;
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
			  "window.location.href=window.location.href+'#" + NewHref + "';", true);
		}

		private void BinddgGroupsUsers(string sLogin)
		{
			DataTable dt = (DataTable)ViewState["Users"];
			#region *****Undo*****
			//DataRow[] dr = dt.Select("Login = '" + sLogin + "'");
			//if (dr.Length > 0)
			//{
			//    txtLogin.Text = dr[0]["Login"].ToString();
			//    tbWindowsLogin.Text = dr[0]["LdapUid"].ToString();
			//    txtFirstName.Text = dr[0]["FirstName"].ToString();
			//    txtLastName.Text = dr[0]["LastName"].ToString();
			//    txtEmail.Text = dr[0]["Email"].ToString();
			//    txtPhone.Text = dr[0]["Phone"].ToString();
			//    txtFax.Text = dr[0]["Fax"].ToString();
			//    txtMobile.Text = dr[0]["Mobile"].ToString();
			//    txtCompany.Text = dr[0]["Company"].ToString();
			//    txtDepartment.Text = dr[0]["Department"].ToString();
			//    txtJobTitle.Text = dr[0]["JobTitle"].ToString();
			//    txtLocation.Text = dr[0]["Location"].ToString();
			//} 
			#endregion
			DataRow dr = GetRowIsEqual(dt, "Login", sLogin);
			if (dr != null)
			{
				txtLogin.Text = dr["Login"].ToString();
				tbWindowsLogin.Text = dr["WindowsLogin"].ToString();
				txtFirstName.Text = dr["FirstName"].ToString();
				txtLastName.Text = dr["LastName"].ToString();
				txtEmail.Text = dr["Email"].ToString();
				txtPhone.Text = dr["Phone"].ToString();
				txtFax.Text = dr["Fax"].ToString();
				txtMobile.Text = dr["Mobile"].ToString();
				txtCompany.Text = dr["Company"].ToString();
				txtDepartment.Text = dr["Department"].ToString();
				txtJobTitle.Text = dr["JobTitle"].ToString();
				txtLocation.Text = dr["Location"].ToString();
			}
			Page.Validate();
		}

		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;
			DataTable dt = (DataTable)ViewState["Users"];
			DataTable dtFull = (DataTable)ViewState["FullUsers"];
			string sLogin = "";
			LinkButton lb = (LinkButton)dlUsers.SelectedItem.FindControl("lbUser");
			if (lb != null)
				sLogin = lb.CommandName;
			bool IsChange = false;

			#region *****Undo*****
			//DataRow[] dr = dt.Select("Login = '" + sLogin + "'");
			//DataRow[] drFull = dtFull.Select("Login = '" + sLogin + "'");
			//if (dr.Length > 0)
			//{
			//    dr[0]["Login"] = txtLogin.Text;
			//    drFull[0]["Login"] = txtLogin.Text;
			//    dr[0]["LdapUid"] = tbWindowsLogin.Text;
			//    drFull[0]["LdapUid"] = tbWindowsLogin.Text;
			//    dr[0]["FirstName"] = txtFirstName.Text;
			//    drFull[0]["FirstName"] = txtFirstName.Text;
			//    dr[0]["LastName"] = txtLastName.Text;
			//    drFull[0]["LastName"] = txtLastName.Text;
			//    dr[0]["Email"] = txtEmail.Text;
			//    drFull[0]["Email"] = txtEmail.Text;
			//    dr[0]["Phone"] = txtPhone.Text;
			//    drFull[0]["Phone"] = txtPhone.Text;
			//    dr[0]["Fax"] = txtFax.Text;
			//    drFull[0]["Fax"] = txtFax.Text;
			//    dr[0]["Mobile"] = txtMobile.Text;
			//    drFull[0]["Mobile"] = txtMobile.Text;
			//    dr[0]["Company"] = txtCompany.Text;
			//    drFull[0]["Company"] = txtCompany.Text;
			//    dr[0]["Department"] = txtDepartment.Text;
			//    drFull[0]["Department"] = txtDepartment.Text;
			//    dr[0]["JobTitle"] = txtJobTitle.Text;
			//    drFull[0]["JobTitle"] = txtJobTitle.Text;
			//    dr[0]["Location"] = txtLocation.Text;
			//    drFull[0]["Location"] = txtLocation.Text;
			//    if (User.GetUserByEmail(txtEmail.Text) == -1)
			//    {
			//        dr[0]["BadEmail"] = false;
			//        drFull[0]["BadEmail"] = false;
			//    }
			//    if (User.GetUserByLogin(txtLogin.Text) == -1)
			//    {
			//        dr[0]["BadLogin"] = false;
			//        drFull[0]["BadLogin"] = false;
			//    }
			//    if (User.GetUserByWindowsLogin(tbWindowsLogin.Text) == -1)
			//    {
			//        dr[0]["BadWinLogin"] = false;
			//        drFull[0]["BadWinLogin"] = false;
			//    }
			//    if (!(bool)dr[0]["BadEmail"] && !(bool)dr[0]["BadLogin"] && !(bool)dr[0]["BadWinLogin"])
			//    {
			//        IsChange = (bool)dr[0]["IsBad"];
			//        dr[0]["IsBad"] = false;
			//        drFull[0]["IsBad"] = false;
			//    }
			//} 
			#endregion

			DataRow dr = GetRowIsEqual(dt, "Login", sLogin);
			DataRow drFull = GetRowIsEqual(dtFull, "Login", sLogin);
			if (dr != null)
			{
				dr["Login"] = txtLogin.Text;
				drFull["Login"] = txtLogin.Text;
				dr["WindowsLogin"] = tbWindowsLogin.Text;
				drFull["WindowsLogin"] = tbWindowsLogin.Text;
				dr["FirstName"] = txtFirstName.Text;
				drFull["FirstName"] = txtFirstName.Text;
				dr["LastName"] = txtLastName.Text;
				drFull["LastName"] = txtLastName.Text;
				dr["Email"] = txtEmail.Text;
				drFull["Email"] = txtEmail.Text;
				dr["Phone"] = txtPhone.Text;
				drFull["Phone"] = txtPhone.Text;
				dr["Fax"] = txtFax.Text;
				drFull["Fax"] = txtFax.Text;
				dr["Mobile"] = txtMobile.Text;
				drFull["Mobile"] = txtMobile.Text;
				dr["Company"] = txtCompany.Text;
				drFull["Company"] = txtCompany.Text;
				dr["Department"] = txtDepartment.Text;
				drFull["Department"] = txtDepartment.Text;
				dr["JobTitle"] = txtJobTitle.Text;
				drFull["JobTitle"] = txtJobTitle.Text;
				dr["Location"] = txtLocation.Text;
				drFull["Location"] = txtLocation.Text;
				if (User.GetUserByEmail(txtEmail.Text) == -1)
				{
					dr["BadEmail"] = false;
					drFull["BadEmail"] = false;
				}
				if (User.GetUserByLogin(txtLogin.Text) == -1)
				{
					dr["BadLogin"] = false;
					drFull["BadLogin"] = false;
				}
				//if (User.GetUserByWindowsLogin(tbWindowsLogin.Text) == -1)
				//{
				//    dr["BadWinLogin"] = false;
				//    drFull["BadWinLogin"] = false;
				//}
				if (!(bool)dr["BadEmail"] && !(bool)dr["BadLogin"]/* && !(bool)dr["BadWinLogin"]*/)
				{
					IsChange = (bool)dr["IsBad"];
					dr["IsBad"] = false;
					drFull["IsBad"] = false;
				}
			}

			ViewState["FullUsers"] = dtFull;
			ViewState["Users"] = dt;

			DataView dv = dt.DefaultView;
			dv.Sort = "IsBad DESC, Login";

			dlUsers.DataSource = dv;
			dlUsers.DataBind();

			string lbClientID = lb.ClientID;
			if (IsChange)
			{
				for (int i = 0; i < dlUsers.Items.Count; i++)
				{
					LinkButton lb1 = (LinkButton)dlUsers.Items[i].FindControl("lbUser");
					if (lb1 != null && lb1.CommandName == sLogin)
					{
						dlUsers.SelectedIndex = i;
						lbClientID = lb1.ClientID;
						break;
					}
				}
			}
			BinddgGroupsUsers(sLogin);
			string NewHref = lbClientID;
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
			  "window.location.href=window.location.href+'#" + NewHref + "';", true);
		}
		#endregion

		#region DataList - Groups
		private void dlUserGroups_ItemCommand(object source, System.Web.UI.WebControls.DataListCommandEventArgs e)
		{
			string sLogin = e.CommandName;
			dlUserGroups.SelectedIndex = e.Item.ItemIndex;
			BindGroups(sLogin, true);
			string NewHref = ((LinkButton)e.CommandSource).ClientID;
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
			  "window.location.href=window.location.href+'#" + NewHref + "';", true);
		}

		#region BindGroups
		private void BindGroups(string sLogin, bool check)
		{
			lbAvailableGroups.Items.Clear();
			lbSelectedGroups.Items.Clear();

			using (IDataReader reader = SecureGroup.GetListGroupsWithParameters(false, false, false, false, false, false, false, false, false, false, false))
			{
				while (reader.Read())
				{
					lbAvailableGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(reader["GroupName"].ToString()), reader["GroupId"].ToString()));
				}
			}

			lbSecurityRoles.Items.Clear();
			int iRoleId = (int)InternalSecureGroups.Administrator;
			ListItem liSecRole = new ListItem(GetGroupTitleById(iRoleId), iRoleId.ToString());
			lbSecurityRoles.Items.Add(liSecRole);

			iRoleId = (int)InternalSecureGroups.HelpDeskManager;
			liSecRole = new ListItem(GetGroupTitleById(iRoleId), iRoleId.ToString());
			lbSecurityRoles.Items.Add(liSecRole);

			iRoleId = (int)InternalSecureGroups.PowerProjectManager;
			liSecRole = new ListItem(GetGroupTitleById(iRoleId), iRoleId.ToString());
			lbSecurityRoles.Items.Add(liSecRole);

			iRoleId = (int)InternalSecureGroups.ProjectManager;
			liSecRole = new ListItem(GetGroupTitleById(iRoleId), iRoleId.ToString());
			lbSecurityRoles.Items.Add(liSecRole);

			iRoleId = (int)InternalSecureGroups.ExecutiveManager;
			liSecRole = new ListItem(GetGroupTitleById(iRoleId), iRoleId.ToString());
			lbSecurityRoles.Items.Add(liSecRole);

			iRoleId = (int)InternalSecureGroups.TimeManager;
			liSecRole = new ListItem(GetGroupTitleById(iRoleId), iRoleId.ToString());
			lbSecurityRoles.Items.Add(liSecRole);

			DataTable dt = (DataTable)ViewState["UsersGroups"];
			#region *****Undo*****
			//DataRow[] dr = dt.Select("Login = '" + sLogin + "'");
			//if (dr.Length > 0)
			//{
			//    iGroups.Value = dr[0]["Groups"].ToString();
			//        ListItem liItem = ddIMGroups.SelectedItem;
			//        if (liItem != null)
			//            liItem.Selected = false;
			//        liItem = ddIMGroups.Items.FindByValue(dr[0]["IMGroup"].ToString());
			//        if (liItem != null)
			//            liItem.Selected = true;
			//} 
			#endregion
			DataRow dr = GetRowIsEqual(dt, "Login", sLogin);
			if (dr != null)
			{
				iGroups.Value = dr["Groups"].ToString();
				ListItem liItem = ddIMGroups.SelectedItem;
				if (liItem != null)
					liItem.Selected = false;
				liItem = ddIMGroups.Items.FindByValue(dr["IMGroup"].ToString());
				if (liItem != null)
					liItem.Selected = true;
			}
			string sGroups = iGroups.Value;
			ArrayList alGroups = new ArrayList();
			while (sGroups.Length > 0)
			{
				alGroups.Add(Int32.Parse(sGroups.Substring(0, sGroups.IndexOf(","))));
				sGroups = sGroups.Remove(0, sGroups.IndexOf(",") + 1);
			}

			foreach (ListItem liRole in lbSecurityRoles.Items)
				if (alGroups.Contains(int.Parse(liRole.Value)))
					liRole.Selected = true;

			foreach (int i in alGroups)
			{
				ListItem liGroup = lbAvailableGroups.Items.FindByValue(i.ToString());
				if (liGroup != null)
				{
					ListItem liNewGroup = new ListItem(liGroup.Text, liGroup.Value);
					lbSelectedGroups.Items.Add(liNewGroup);
					lbAvailableGroups.Items.Remove(liGroup);
				}
			}
			if (check)
				Page.Validate();
		}
		#endregion

		protected void btnSave2_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
			{
				string _sLogin = "";
				LinkButton _lb = (LinkButton)dlUserGroups.SelectedItem.FindControl("lbUser2");
				if (_lb != null)
					_sLogin = _lb.CommandName;
				BindGroups(_sLogin, false);
				return;
			}
			string sGroups = iGroups.Value;
			DataTable dt = (DataTable)ViewState["UsersGroups"];
			DataTable dt1 = (DataTable)ViewState["Users"];
			DataTable dtFull = (DataTable)ViewState["FullUsers"];

			string sLogin = "";
			LinkButton lb = (LinkButton)dlUserGroups.SelectedItem.FindControl("lbUser2");
			if (lb != null)
				sLogin = lb.CommandName;
			ArrayList alLogins = new ArrayList();
			alLogins.Add(sLogin);

			for (int i = 0; i < dlUserGroups.Items.Count; i++)
			{
				LinkButton lb1 = (LinkButton)dlUserGroups.Items[i].FindControl("lbUser2");
				CheckBox chkItem = (CheckBox)dlUserGroups.Items[i].FindControl("chkItem");
				if (lb1 != null && chkItem != null && chkItem.Checked && !alLogins.Contains(lb1.CommandName))
					alLogins.Add(lb1.CommandName);
			}

			bool IsChange = false;
			foreach (string sLoginFromArray in alLogins)
			{
				#region *****Undo*****
				//DataRow[] dr = dt.Select("Login = '" + sLoginFromArray + "'");
				//DataRow[] dr1 = dt1.Select("Login = '" + sLoginFromArray + "'");
				//DataRow[] drFull = dtFull.Select("Login = '" + sLoginFromArray + "'");

				//if (dr.Length > 0)
				//{
				//    dr[0]["Groups"] = sGroups;
				//        dr[0]["IMGroup"] = int.Parse(ddIMGroups.SelectedValue);
				//    if (sGroups.Length >= 2)
				//    {
				//        IsChange = (sLoginFromArray == sLogin) && (bool)dr[0]["IsBadGroup"];
				//        dr[0]["IsBadGroup"] = false;
				//    }
				//}
				//if (dr1.Length > 0)
				//{
				//    dr1[0]["Groups"] = sGroups;
				//        dr1[0]["IMGroup"] = int.Parse(ddIMGroups.SelectedValue);
				//    if (sGroups.Length >= 2)
				//        dr1[0]["IsBadGroup"] = false;
				//}
				//if (drFull.Length > 0)
				//{
				//    drFull[0]["Groups"] = sGroups;
				//        drFull[0]["IMGroup"] = int.Parse(ddIMGroups.SelectedValue);
				//    if (sGroups.Length >= 2)
				//        drFull[0]["IsBadGroup"] = false;
				//} 
				#endregion
				DataRow dr = GetRowIsEqual(dt, "Login", sLoginFromArray);
				DataRow dr1 = GetRowIsEqual(dt1, "Login", sLoginFromArray);
				DataRow drFull = GetRowIsEqual(dtFull, "Login", sLoginFromArray);

				if (dr != null)
				{
					dr["Groups"] = sGroups;
					dr["IMGroup"] = int.Parse(ddIMGroups.SelectedValue);
					if (sGroups.Length >= 2)
					{
						IsChange = (sLoginFromArray == sLogin) && (bool)dr["IsBadGroup"];
						dr["IsBadGroup"] = false;
					}
				}
				if (dr1 != null)
				{
					dr1["Groups"] = sGroups;
					dr1["IMGroup"] = int.Parse(ddIMGroups.SelectedValue);
					if (sGroups.Length >= 2)
						dr1["IsBadGroup"] = false;
				}
				if (drFull != null)
				{
					drFull["Groups"] = sGroups;
					drFull["IMGroup"] = int.Parse(ddIMGroups.SelectedValue);
					if (sGroups.Length >= 2)
						drFull["IsBadGroup"] = false;
				}
			}
			ViewState["FullUsers"] = dtFull;
			ViewState["Users"] = dt1;
			ViewState["UsersGroups"] = dt;

			DataView dv = dt.DefaultView;
			dv.Sort = "IsBadGroup DESC, LastName, FirstName";

			dlUserGroups.DataSource = dv;
			dlUserGroups.DataBind();

			string lbClientID2 = lb.ClientID;
			if (IsChange)
			{
				for (int i = 0; i < dlUserGroups.Items.Count; i++)
				{
					LinkButton lb2 = (LinkButton)dlUserGroups.Items[i].FindControl("lbUser2");
					if (lb2 != null && lb2.CommandName == sLogin)
					{
						dlUserGroups.SelectedIndex = i;
						lbClientID2 = lb2.ClientID;
						break;
					}
				}
			}
			BindGroups(sLogin, true);
			string NewHref = lbClientID2;
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
			  "window.location.href=window.location.href+'#" + NewHref + "';", true);
		}
		#endregion

		#region DataGridCommands
		private void dg_edit(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			grdFields.EditItemIndex = e.Item.ItemIndex;
			grdFields.DataKeyField = "IBNField";
			BindDG();
			foreach (DataGridItem dgi in grdFields.Items)
			{
				DropDownList ddl = (DropDownList)dgi.FindControl("ddADFields");
				TextBox tb = (TextBox)dgi.FindControl("txtADField");
				if (ddl != null && tb != null)
				{
					try
					{
						ddl.SelectedValue = e.Item.Cells[3].Text;
					}
					catch { }
					string sCompare = e.Item.Cells[3].Text;
					tb.Text = (sCompare != "&nbsp;" && sCompare != "0") ? sCompare : "";
					ddl.Attributes.Add("onchange", "ChangeAD(this)");
					HtmlAnchor aLink = (HtmlAnchor)dgi.FindControl("linkTo");
					if (aLink != null)
						Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
						  "window.location.href=window.location.href+'#" + aLink.ClientID + "';", true);
					if (rbSourceObject.SelectedValue == "1")
						tb.Style.Add("display", "none");
				}
			}
		}

		private void dg_cancel(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			HtmlAnchor aLink = (HtmlAnchor)e.Item.FindControl("linkTo");
			if (aLink != null)
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				  "window.location.href=window.location.href+'#" + aLink.ClientID + "';", true);
			grdFields.EditItemIndex = -1;
			BindDG();
		}

		private void dg_update(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			string sIBNField = grdFields.DataKeys[e.Item.ItemIndex].ToString();
			DropDownList ddl = (DropDownList)e.Item.FindControl("ddADFields");
			TextBox tb = (TextBox)e.Item.FindControl("txtADField");
			if (ddl != null && tb != null)
			{
				DataTable dt = (DataTable)ViewState["Fields"];
				DataRow[] dr = dt.Select("IBNField = '" + sIBNField + "'");
				if (dr.Length > 0)
				{
					string sValue = (tb.Text != "") ? tb.Text : ddl.SelectedValue;
					dr[0]["realADField"] = sValue;
					dr[0]["ADField"] = (sValue == "0") ? LocRM.GetString("tNotSet") : sValue;
				}
				ViewState["Fields"] = dt;
				HtmlAnchor aLink = (HtmlAnchor)e.Item.FindControl("linkTo");
				if (aLink != null)
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "window.location.href=window.location.href+'#" + aLink.ClientID + "';", true);
			}
			grdFields.EditItemIndex = -1;
			BindDG();
		}
		#endregion

		#region Validation
		private void cvLogin_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			string sDomain = txtDomain.Text;
			string sUserName = txtUserName.Text;
			string sPassword = txtPassword.Text;

			int LdapId = int.Parse(ddLDAPSettings.SelectedValue);
			if (LdapId > 0)
			{
				LdapSettings lsets = LdapSettings.Load(LdapId);
				if (txtDomain.Text == lsets.Domain && txtUserName.Text == lsets.Username && txtPassword.Text == "")
					sPassword = lsets.Password;
			}
			if (ActiveDirectory.CheckLogin(txtDomain.Text, txtUserName.Text, sPassword))
			{
				lblConnectError.Visible = false;
				args.IsValid = true;
			}
			else
			{
				lblConnectError.Visible = true;
				args.IsValid = false;
			}
		}

		private void cvAtLeastOne_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			bool fl = false;
			foreach (DataGridItem dgi in dgUsers.Items)
			{
				HtmlInputCheckBox icb = (HtmlInputCheckBox)dgi.FindControl("cbConvert");
				if (icb != null && icb.Checked)
				{
					fl = true;
					break;
				}
			}
			if (fl)
				args.IsValid = true;
			else
				args.IsValid = false;
		}

		private void cvUserLogin_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{

			if (User.GetUserByLogin(txtLogin.Text) != -1)
				args.IsValid = false;
			else if (txtLogin.Text.Length > 0)
			{
				DataTable dt = (DataTable)ViewState["Users"];
				string sLogin = "";
				LinkButton lb = (LinkButton)dlUsers.SelectedItem.FindControl("lbUser");
				if (lb != null)
					sLogin = lb.CommandName;
				if (sLogin != txtLogin.Text)
				{
					#region *****Undo*****
					//DataRow[] dr = dt.Select("Login = '" + txtLogin.Text + "'");
					//if (dr.Length > 0)
					//    args.IsValid = false;
					//else
					//    args.IsValid = true; 
					#endregion
					DataRow dr = GetRowIsEqual(dt, "Login", txtLogin.Text);
					if (dr != null)
						args.IsValid = false;
					else
						args.IsValid = true;
				}
				else
					args.IsValid = true;
			}
			else
				args.IsValid = true;
		}

		private void cvEmail_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			if (User.GetUserByEmail(txtEmail.Text) != -1)
				args.IsValid = false;
			else if (txtEmail.Text.Length > 1)
			{
				DataTable dt = (DataTable)ViewState["Users"];
				string sLogin = "";
				LinkButton lb = (LinkButton)dlUsers.SelectedItem.FindControl("lbUser");
				if (lb != null)
					sLogin = lb.CommandName;
				string sEmail = "";
				#region *****Undo*****
				//DataRow[] dr = dt.Select("Login = '" + sLogin + "'");
				//if (dr.Length > 0)
				//{
				//    sEmail = dr[0]["Email"].ToString();
				//    if (sEmail != txtEmail.Text)
				//    {
				//        DataRow[] drEM = dt.Select("Email = '" + txtEmail.Text + "'");
				//        if (drEM.Length > 0)
				//            args.IsValid = false;
				//        else
				//            args.IsValid = true;
				//    }
				//    else
				//        args.IsValid = true;
				//}
				//else
				//    args.IsValid = true; 
				#endregion
				DataRow dr = GetRowIsEqual(dt, "Login", sLogin);
				if (dr != null)
				{
					sEmail = dr["Email"].ToString();
					if (sEmail != txtEmail.Text)
					{
						DataRow drEM = GetRowIsEqual(dt, "Email", txtEmail.Text);
						if (drEM != null)
							args.IsValid = false;
						else
							args.IsValid = true;
					}
					else
						args.IsValid = true;
				}
				else
					args.IsValid = true;
			}
			else
				args.IsValid = true;
		}

		private void cvWindowsLogin_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if (tbWindowsLogin.Text != string.Empty && User.GetUserByWindowsLogin(tbWindowsLogin.Text) != -1)
				args.IsValid = false;
			else args.IsValid = true;
		}

		private void GroupValidator_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			string sGroups = iGroups.Value;
			if (sGroups.Length > 1 && sGroups.IndexOf(",") > 0)
				args.IsValid = true;
			else
				args.IsValid = false;
		}
		#endregion

		#region Implementation of IWizardControl

		public int StepCount { get { return _stepCount; } }
		public string TopTitle { get { return LocRM.GetString("ConvertGlobalTitle"); } }
		public bool ShowSteps { get { return true; } }
		public string Subtitle { get; private set; }
		public string MiddleButtonText { get; private set; }
		public string CancelText { get; private set; }

		public void SetStep(int stepNumber)
		{
			ShowStep(stepNumber);
			Subtitle = (string)subtitles[stepNumber - 1];
		}

		public string GenerateFinalStepScript()
		{
			return "try{window.opener.top.right.location.href='../Directory/Directory.aspx?Tab=3';} catch (e) {} window.close();";
		}

		public void CancelAction()
		{

		}
		#endregion

		#region ProcessFileCache
		public static void ProcessFileCache(string _Path)
		{
			DirectoryInfo dir = new DirectoryInfo(_Path);
			FileInfo[] fi = dir.GetFiles("*.*");
			foreach (FileInfo _fi in fi)
			{
				if (String.Compare(_fi.Name, "readme.txt", true) == 0)
					continue;

				if (_fi.CreationTime < DateTime.Now.AddMinutes(-1440))
					_fi.Delete();
			}
		}
		#endregion

		private void ddLDAPSettings_SelectedIndexChanged(object sender, EventArgs e)
		{
			ChangedLdap();
		}

		#region ChangedLdap
		private void ChangedLdap()
		{
			int LdapId = int.Parse(ddLDAPSettings.SelectedValue);
			if (LdapId > 0)
			{
				LdapSettings lsets = LdapSettings.Load(LdapId);
				txtDomain.Text = lsets.Domain;
				txtUserName.Text = lsets.Username;
				rfPassword.Enabled = false;
			}
			else
				rfPassword.Enabled = true;
		}
		#endregion

		private DataRow GetRowIsEqual(DataTable dt, string keyWord, string keyValue)
		{
			foreach (DataRow dr in dt.Rows)
				if (dr[keyWord].ToString() == keyValue)
					return dr;
			return null;
		}
	}
}
