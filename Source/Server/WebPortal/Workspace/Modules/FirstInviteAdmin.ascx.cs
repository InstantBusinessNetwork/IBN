namespace Mediachase.UI.Web.Workspace.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.IO;
	using System.Resources;
	using System.Text;
	using System.Web.UI;

	using Mediachase.Ibn;
	using Mediachase.Ibn.Data;
	using Mediachase.IBN.Business;

	public partial class FirstInviteAdmin : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Wizards.Resources.strFirstLogAdmWd", typeof(FirstInviteAdmin).Assembly);

		protected void Page_Load(object sender, EventArgs e)
		{
			BindTexts();
			BindToolbar();
		}

		#region BindTexts
		private void BindTexts()
		{
			lblMessage.Text = String.Format("Вы можете прямо сейчас пригласить ещё нескольких человек на свой сайт. " +
			  "Привлечение новых людей поможет вам организовать совместную работу средствами {0}. " +
			  "Каждый человек, данные о котором вы введёте, получит сообщение по электронной почте " +
			  "с информацией о том, как можно попасть на ваш сайт.<br/>", IbnConst.ProductFamilyShort);

			lblMakeAdmin.Text = "<b>Администратор?</b>";

			lblAddUsers.Text = "Приглашение новых пользователей";

			lblElements.Text = "Создание демонстрационных элементов";
			lblMessage1.Text = String.Format("Для того, чтобы в наглядной форме увидеть ключевые моменты {0}, " +
			  "вы можете создать несколько демонстрационных элементов - " +
			  "поручений, мероприятий, документов.", IbnConst.ProductFamilyShort);

			cbAdd.Text = "&nbsp;Создать демонстрационные элементы";
			lblDemo1.Text = String.Format("1. Демо Поручение - Ознакомиться со &quot;Справочником по работе с {0}&quot;", IbnConst.ProductFamilyShort);
			lblDemo2.Text = "2. Демо Поручение - Прочитать про администрирование портала";
			lblDemo3.Text = String.Format("3. Демо Поручение - Ознакомиться с документом &quot;Что нового в {0} {1}&quot;", IbnConst.ProductFamilyShort, IbnConst.VersionMajorDotMinor);
			lblDemo4.Text = "4. Демо Поручение - Прочитать про настройку Help Desk";
			if (Configuration.HelpDeskEnabled)
			{
				lblDemo5.Text = "5. Демо инцидент";
				lblDemo6.Text = "6. Демо мероприятие";
				lblDemo7.Text = String.Format("7. Демо документ - Подготовить регламент по работе с {0}", IbnConst.ProductFamilyShort);
			}
			else
			{
				lblDemo5.Text = "5. Демо мероприятие";
				lblDemo6.Text = String.Format("6. Демо документ - Подготовить регламент по работе с {0}", IbnConst.ProductFamilyShort);
				lblDemo7.Visible = false;
			}

			btnSend.Text = "Начать работу с порталом";
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("InviteGlobalTitle");
		}
		#endregion

		protected void btnSend_Click(object sender, EventArgs e)
		{
			ArrayList res = new ArrayList();
			ArrayList resAdmin = new ArrayList();

			#region MakeUsers
			int iId1 = MakeUser(txtFirstName1.Text, txtLastName1.Text, txtEMail1.Text, cbAdmin1.Checked);
			if (iId1 > 0)
			{
				if (cbAdmin1.Checked)
					resAdmin.Add(iId1);
				res.Add(iId1);
			}

			int iId2 = MakeUser(txtFirstName2.Text, txtLastName2.Text, txtEMail2.Text, cbAdmin2.Checked);
			if (iId2 > 0)
			{
				if (cbAdmin2.Checked)
					resAdmin.Add(iId2);
				res.Add(iId2);
			}

			int iId3 = MakeUser(txtFirstName3.Text, txtLastName3.Text, txtEMail3.Text, cbAdmin3.Checked);
			if (iId3 > 0)
			{
				if (cbAdmin3.Checked)
					resAdmin.Add(iId3);
				res.Add(iId3);
			}

			int iId4 = MakeUser(txtFirstName4.Text, txtLastName4.Text, txtEMail4.Text, cbAdmin4.Checked);
			if (iId4 > 0)
			{
				if (cbAdmin4.Checked)
					resAdmin.Add(iId4);
				res.Add(iId4);
			}

			res.Add(Security.CurrentUser.UserID);
			if (Security.IsUserInGroup(InternalSecureGroups.Administrator))
				resAdmin.Add(Security.CurrentUser.UserID);
			#endregion

			#region MakeObjects
			if (cbAdd.Checked)
			{
				//1.
				string data0 = string.Format("[InternetShortcut]\r\nURL={0}", GlobalResourceManager.Strings["QuickHelpLink"]);
				MemoryStream memStream0 = new MemoryStream();
				StreamWriter writer0 = new StreamWriter(memStream0, Encoding.Unicode);
				writer0.Write(data0);
				writer0.Flush();
				memStream0.Seek(0, SeekOrigin.Begin);
				string title0 = String.Format("Справочник по работе с {0}", IbnConst.ProductFamilyShort);
				string html_filename0 = Mediachase.UI.Web.Util.CommonHelper.GetHtmlFileTitle(title0);
				if (html_filename0.IndexOf(".url") < 0)
					html_filename0 += ".url";

				PrimaryKeyId org_id = PrimaryKeyId.Empty;
				PrimaryKeyId contact_id = PrimaryKeyId.Empty;
				Common.GetDefaultClient(PortalConfig.ToDoDefaultValueClientField, out contact_id, out org_id);

				ToDo.Create(-1, Security.CurrentUser.UserID, String.Format("Ознакомиться со Справочником по работе с {0}", IbnConst.ProductFamilyShort), "", DateTime.Now, DateTime.MinValue,
					int.Parse(PortalConfig.ToDoDefaultValuePriorityField),
					int.Parse(PortalConfig.ToDoDefaultValueActivationTypeField),
					int.Parse(PortalConfig.ToDoDefaultValueCompetionTypeField),
					bool.Parse(PortalConfig.ToDoDefaultValueMustConfirmField),
					int.Parse(PortalConfig.ToDoDefaultValueTaskTimeField),
					Common.StringToArrayList(PortalConfig.ToDoDefaultValueGeneralCategoriesField),
					html_filename0, memStream0, res, contact_id, org_id);

				//2.
				if (resAdmin.Count > 0)
				{
					string data = string.Format("[InternetShortcut]\r\nURL={0}", "http://friends.pmbox.ru/media/p/5422.aspx");
					MemoryStream memStream = new MemoryStream();
					StreamWriter writer = new StreamWriter(memStream, Encoding.Unicode);
					writer.Write(data);
					writer.Flush();
					memStream.Seek(0, SeekOrigin.Begin);
					string title = String.Format("Администрирование {0}", IbnConst.ProductName);
					string html_filename = Mediachase.UI.Web.Util.CommonHelper.GetHtmlFileTitle(title);
					if (html_filename.IndexOf(".url") < 0)
						html_filename += ".url";

					ToDo.Create(-1, Security.CurrentUser.UserID, "Прочитать про администрирование портала", "", DateTime.Now, DateTime.MinValue,
						int.Parse(PortalConfig.ToDoDefaultValuePriorityField),
						int.Parse(PortalConfig.ToDoDefaultValueActivationTypeField),
						int.Parse(PortalConfig.ToDoDefaultValueCompetionTypeField),
						bool.Parse(PortalConfig.ToDoDefaultValueMustConfirmField),
						int.Parse(PortalConfig.ToDoDefaultValueTaskTimeField),
						Common.StringToArrayList(PortalConfig.ToDoDefaultValueGeneralCategoriesField),
						html_filename, memStream, resAdmin, contact_id, org_id);
				}

				//3.
				string data1 = string.Format("[InternetShortcut]\r\nURL={0}", "http://friends.pmbox.ru/files/folders/ibn/entry2685.aspx");
				MemoryStream memStream1 = new MemoryStream();
				StreamWriter writer1 = new StreamWriter(memStream1, Encoding.Unicode);
				writer1.Write(data1);
				writer1.Flush();
				memStream1.Seek(0, SeekOrigin.Begin);
				string title1 = String.Format("Что нового в {0} {1}", IbnConst.ProductFamilyShort, IbnConst.VersionMajorDotMinor);
				string html_filename1 = Mediachase.UI.Web.Util.CommonHelper.GetHtmlFileTitle(title1);
				if (html_filename1.IndexOf(".url") < 0)
					html_filename1 += ".url";

				ToDo.Create(-1, Security.CurrentUser.UserID, String.Format("Ознакомиться с документом Что нового в {0} {1}", IbnConst.ProductFamilyShort, IbnConst.VersionMajorDotMinor), "", DateTime.Now, DateTime.MinValue,
					int.Parse(PortalConfig.ToDoDefaultValuePriorityField),
					int.Parse(PortalConfig.ToDoDefaultValueActivationTypeField),
					int.Parse(PortalConfig.ToDoDefaultValueCompetionTypeField),
					bool.Parse(PortalConfig.ToDoDefaultValueMustConfirmField),
					int.Parse(PortalConfig.ToDoDefaultValueTaskTimeField),
					Common.StringToArrayList(PortalConfig.ToDoDefaultValueGeneralCategoriesField),
					html_filename1, memStream1, res, contact_id, org_id);

				//4.
				if (resAdmin.Count > 0)
				{
					string data2 = string.Format("[InternetShortcut]\r\nURL={0}", "http://friends.pmbox.ru/wikis/docs/help-desk-ibn.aspx");
					MemoryStream memStream2 = new MemoryStream();
					StreamWriter writer2 = new StreamWriter(memStream2, Encoding.Unicode);
					writer2.Write(data2);
					writer2.Flush();
					memStream2.Seek(0, SeekOrigin.Begin);
					string title2 = String.Format("Как настроить работу Help Desk в {0} {1}", IbnConst.ProductFamilyShort, IbnConst.VersionMajorDotMinor);
					string html_filename2 = Mediachase.UI.Web.Util.CommonHelper.GetHtmlFileTitle(title2);
					if (html_filename2.IndexOf(".url") < 0)
						html_filename2 += ".url";

					ToDo.Create(-1, Security.CurrentUser.UserID, "Прочитать про настройку Help Desk", "", DateTime.Now, DateTime.MinValue,
						int.Parse(PortalConfig.ToDoDefaultValuePriorityField),
						int.Parse(PortalConfig.ToDoDefaultValueActivationTypeField),
						int.Parse(PortalConfig.ToDoDefaultValueCompetionTypeField),
						bool.Parse(PortalConfig.ToDoDefaultValueMustConfirmField),
						int.Parse(PortalConfig.ToDoDefaultValueTaskTimeField),
						Common.StringToArrayList(PortalConfig.ToDoDefaultValueGeneralCategoriesField),
						html_filename2, memStream2, resAdmin, contact_id, org_id);
				}

				DataTable dt = new DataTable();
				dt.Columns.Add(new DataColumn("PrincipalId", typeof(int)));
				dt.Columns.Add(new DataColumn("IsNew", typeof(bool)));
				dt.Columns.Add(new DataColumn("ResponsePending", typeof(bool)));
				dt.Columns.Add(new DataColumn("MustBeConfirmed", typeof(bool)));
				dt.Columns.Add(new DataColumn("CanManage", typeof(bool)));
				foreach (int usId in res)
				{
					DataRow row = dt.NewRow();
					row["PrincipalId"] = usId;
					row["IsNew"] = true;
					row["ResponsePending"] = true;
					row["MustBeConfirmed"] = false;
					row["CanManage"] = (usId == Security.CurrentUser.UserID);
					dt.Rows.Add(row);
				}

				if (Configuration.HelpDeskEnabled)
				{
					//5.
					int IssId = Incident.Create("Демо инцидент", "",
						int.Parse(PortalConfig.IncidentDefaultValueTypeField),
						int.Parse(PortalConfig.IncidentDefaultValuePriorityField),
						int.Parse(PortalConfig.IncidentDefaultValueSeverityField),
						Security.CurrentUser.UserID, DateTime.UtcNow);
					Issue2.UpdateQuickTracking(IssId, "", (int)ObjectStates.Active, -1, -1, true, dt, false);
				}
				//6.
				org_id = PrimaryKeyId.Empty;
				contact_id = PrimaryKeyId.Empty;
				Common.GetDefaultClient(PortalConfig.CEntryDefaultValueClientField, out contact_id, out org_id);
				CalendarEntry.Create("Демо мероприятие", "", "", -1, Security.CurrentUser.UserID,
				  int.Parse(PortalConfig.CEntryDefaultValuePriorityField),
				  (int)CalendarEntry.EventType.Event,
				  DateTime.Today.AddDays(1).AddHours(10),
				  DateTime.Today.AddDays(1).AddHours(11),
				  Mediachase.IBN.Business.Common.StringToArrayList(PortalConfig.CEntryDefaultValueGeneralCategoriesField),
				  null, null, res, contact_id, org_id);

				//7.
				org_id = PrimaryKeyId.Empty;
				contact_id = PrimaryKeyId.Empty;
				Common.GetDefaultClient(PortalConfig.DocumentDefaultValueClientField, out contact_id, out org_id);
				int docId = Document.Create(String.Format("Подготовить регламент по работе с {0}", IbnConst.ProductFamilyShort), "", -1,
					int.Parse(PortalConfig.DocumentDefaultValuePriorityField), Security.CurrentUser.UserID, 1, int.Parse(PortalConfig.DocumentDefaultValueTaskTimeField),
					contact_id, org_id);
				Document2.UpdateResources(docId, dt);
				Document.ActivateDocument(docId);
			}
			#endregion

			PortalConfig.PortalShowAdminWizard = false;
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "window.top.location.href='" + ResolveClientUrl("~/Apps/Shell/Pages/default.aspx") + "';", true);
		}

		#region MakeUser
		private int MakeUser(string sfirst, string slast, string semail, bool badmin)
		{
			ArrayList alGroups = new ArrayList();
			alGroups.Add((int)InternalSecureGroups.ProjectManager);
			using (IDataReader reader = SecureGroup.GetListGroups())
			{
				while (reader.Read())
				{
					int iGroupId = (int)reader["GroupId"];
					if (iGroupId > 8)
					{
						alGroups.Add(iGroupId);
						break;
					}
				}
			}
			int iIMGroup = 0;
			using (DataTable table = IMGroup.GetListIMGroup())
			{
				foreach (DataRow row in table.Rows)
				{
					if (!(bool)row["is_partner"])
					{
						iIMGroup = (int)row["IMGroupId"];
						break;
					}
				}
			}

			int userId = -1;

			#region makeuser
			if (semail != "")
			{
				int id = User.GetUserByEmail(semail);
				if (id <= 0)
				{
					string strLogin = semail.Substring(0, semail.IndexOf("@"));
					int i = 0;
					bool fl = false;
					do
					{
						string tmpLogin = strLogin;
						if (i > 0)
							tmpLogin = tmpLogin + i.ToString();
						using (IDataReader reader = User.GetUserInfoByLogin(tmpLogin))
						{
							if (reader.Read())
								fl = true;
							else
								fl = false;
						}
						i++;
					}
					while (fl);

					string sFirst = (sfirst == "") ? strLogin : sfirst;
					string sLast = (slast == "") ? strLogin : slast;
					if (i > 1)
						strLogin = strLogin + (i - 1).ToString();

					ArrayList newList = new ArrayList(alGroups);
					if (badmin)
						newList.Add((int)InternalSecureGroups.Administrator);

					userId = User.Create(strLogin, "ibn", sFirst, sLast, semail, true,
					  newList, iIMGroup, "", "", "", "", "", "", "", Security.CurrentUser.TimeZoneId,
					  Security.CurrentUser.LanguageId, null, null, -1);
				}
			}
			#endregion

			return userId;
		}
		#endregion

	}
}
