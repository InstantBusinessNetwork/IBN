using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

using Mediachase.Ibn;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Database;
using Mediachase.MetaDataPlus;

using Idm = Mediachase.Ibn.Data.Meta;
using IdmServices = Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Business.Customization;


namespace Mediachase.IBN.Business
{
	/// <summary>
	/// 
	/// </summary>
	public class User
	{
		// we use this variable for development and testing purpose only
		//private bool hashPasswords = false; // warning CS0414: The private field 'Mediachase.IBN.Business.User.hashPasswords' is assigned but its value is never used

		#region USER_TYPE
		private static ObjectTypes USER_TYPE
		{
			get { return ObjectTypes.User; }
		}
		#endregion

		public enum UserActivity
		{
			Inactive = 1,
			Pending = 2,
			Active = 3
		}

		public enum ActionTypes
		{
			PendingUsers = 1,
			IncidentsToAssign = 2,
			TasksTodosToApprove = 3,
			MailIncidents = 4,
			TasksTodosToAccept = 5,
			EventsToAccept = 6,
			TasksTodosToAssign = 7,
			IncidentsToAccept = 8,
			PendingTimeSheets = 9,
			ActiveProjects = 10
		}


		#region DefaultTimeZoneId
		public static int DefaultTimeZoneId
		{
			get
			{
				return 0;
			}
		}
		#endregion

		#region CanCreate
		public static bool CanCreate()
		{
			return Security.IsUserInGroup(InternalSecureGroups.Administrator);
		}
		#endregion

		#region CanRead
		public static bool CanRead(int UserId)
		{
			bool RetVal = false;
			if (Security.IsUserInGroup(InternalSecureGroups.Partner))
			{
				int PartnerGroupId = DBGroup.GetGroupForPartnerUser(Security.CurrentUser.UserID);

				if (PartnerGroupId > 0)
					RetVal = DBGroup.CheckUserVisibilityForPartnerGroup(PartnerGroupId, UserId);
			}
			else
				RetVal = true;

			return RetVal;
		}
		#endregion

		#region CanCreateExternal
		public static bool CanCreateExternal()
		{
			return (Security.IsUserInGroup(InternalSecureGroups.Administrator)
				|| Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
				|| Security.IsUserInGroup(InternalSecureGroups.ProjectManager))
				&& License.ExternalUsersCount != 0;
		}
		#endregion

		#region CanCreatePending
		public static bool CanCreatePending()
		{
			return !Security.IsUserInGroup(InternalSecureGroups.Administrator)
				&& !Security.IsUserInGroup(InternalSecureGroups.Partner);
		}
		#endregion

		#region CanCreatePartner
		public static bool CanCreatePartner()
		{
			return Security.IsUserInGroup(InternalSecureGroups.Administrator)
				&& License.PartnersEnabled;
		}
		#endregion

		#region CanUpdateUserInfo
		public static bool CanUpdateUserInfo()
		{
			return Security.IsUserInGroup(InternalSecureGroups.Administrator);
		}

		public static bool CanUpdateUserInfo(int user_id)
		{
			return user_id == Security.UserID || Security.IsUserInGroup(InternalSecureGroups.Administrator);
		}
		#endregion

		#region CanUpdateExternal
		public static bool CanUpdateExternal()
		{
			return (Security.IsUserInGroup(InternalSecureGroups.Administrator)
				|| Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
				|| Security.IsUserInGroup(InternalSecureGroups.ProjectManager))
				&& License.ExternalUsersCount != 0;
		}
		#endregion

		#region CanUpdateSecureFields
		public static bool CanUpdateSecureFields(int user_id)
		{
			return Security.IsUserInGroup(InternalSecureGroups.Administrator);
		}
		#endregion

		#region CanUpdatePassword
		public static bool CanUpdatePassword(int user_id)
		{
			return user_id == Security.UserID ||
				Security.IsUserInGroup(InternalSecureGroups.Administrator);
		}
		#endregion

		#region CanUpdatePreferences
		public static bool CanUpdatePreferences(int user_id)
		{
			return user_id == Security.UserID ||
				Security.IsUserInGroup(InternalSecureGroups.Administrator);
		}
		#endregion

		#region CanReadPreferences
		public static bool CanReadPreferences(int user_id)
		{
			return user_id == Security.CurrentUser.UserID ||
				Security.IsUserInGroup(InternalSecureGroups.Administrator);
		}
		#endregion

		#region CanDelete
		public static bool CanDelete()
		{
			return Security.IsUserInGroup(InternalSecureGroups.Administrator);
		}

		public static bool CanDelete(int UserId)
		{
			bool retval = Security.IsUserInGroup(InternalSecureGroups.Administrator) && (Security.CurrentUser.UserID != UserId);
			if (retval)
			{
				using (IDataReader reader = DBUser.GetUserInfo(UserId))
				{
					if (reader.Read() && reader["login"].ToString().ToLower() == "alert")
						retval = false;
				}
			}
			return retval;
		}
		#endregion

		#region CanAddNewActiveUser*
		public static bool CanAddNewActiveUser(bool External)
		{
			return CanAddNewActiveUsers(External, 1);
		}

		public static bool CanAddNewActiveUsers(bool External, int count)
		{
			return (GetRemainingActiveUsers(External) >= count);
		}
		#endregion

		#region CanImportFromActiveDirectory
		public static bool CanImportFromActiveDirectory()
		{
			return License.ActiveDirectoryImport;
		}
		#endregion


		#region Create
		public static int Create(string login, string password, string firstName,
			string lastName, string email, ArrayList groups, int imGroupId,
			string phone, string mobile, string position, string department,
			string company, string location, int languageId, string inviteText)
		{
			return Create(login, password, firstName, lastName, email, true, groups,
				imGroupId, phone, string.Empty, mobile, position, department, company, location,
				DefaultTimeZoneId, languageId, inviteText, null, null, null, -1);
		}

		public static int Create(string login, string password, string firstName,
			string lastName, string email, bool isActive, ArrayList groups, int imGroupId,
			string phone, string fax, string mobile, string position, string department,
			string company, string location, int timeZoneId, int languageId,
			string fileName, Stream data, int profileId)
		{
			return Create(login, password, firstName, lastName, email, isActive, groups,
				imGroupId, phone, fax, mobile, position, department, company, location,
				timeZoneId, languageId, string.Empty, fileName, data, null, profileId);
		}

		public static int Create(string login, string password, string firstName,
			string lastName, string email, bool isActive, ArrayList groups, int imGroupId,
			string phone, string fax, string mobile, string position, string department,
			string company, string location, int timeZoneId, int languageId,
			string inviteText, string fileName, Stream data, string windowsLogin,
			int profileId)
		{
			if (groups.Count <= 0)
				throw new WrongDataException();

			//check user rights
			if (!CanCreate())
				throw new AccessDeniedException();

			if (password == "")
				throw new PasswordRequiredException();

			int Activity = (int)UserActivity.Inactive;
			if (isActive)
			{
				if (!CanAddNewActiveUser(false))
					throw new MaxUsersCountException();
				Activity = (int)UserActivity.Active;
			}

			if (!Regex.IsMatch(login, @"^[\w-\.]+"))
				throw new WrongLoginException();

			if (DBUser.GetUserByEmail(email, false) > 0)
				throw new EmailDuplicationException();

			if (DBUser.CheckEmail(email) > 0)
				throw new EmailDuplicationException();

			int[] dependentIMGroups = GetDependentIMGroups(imGroupId);

			string salt = PasswordUtil.CreateSalt(PasswordUtil.SaltSize);
			string hash = PasswordUtil.CreateHash(password, salt);

			int UserID = -1;
			int OriginalUserId = -1;
			//Begin transaction
			try
			{
				using (DbTransaction tran = DbTransaction.Begin())
				{
					OriginalUserId = DBUser.AddToMain(-1, login, password, firstName, lastName, email, salt, hash, isActive, imGroupId);

					UserID = DBUser.Create(login, windowsLogin, password, firstName, lastName, email, salt, hash, Activity, false, Security.CurrentUser.UserID, inviteText);

					//update user profile
					DBUser.UpdateProfile(UserID, phone, fax, mobile, position, department, company, location);

					// Picture
					BindPhoto(UserID, false, fileName, data);

					// Create default user preferences
					DBUser.UpdatePreferences(UserID, true, true, true, false, 60, 8, 20, timeZoneId, languageId, 3);

					//Add user's groups to DB
					foreach (int GroupID in groups)
					{
						DBUser.AddUserToGroup(UserID, GroupID);
					}

					//Set IM Group and OriginalUserId
					DBUser.AssignIMGroup(UserID, imGroupId);
					if (OriginalUserId > 0)
						DBUser.AssignOriginalId(UserID, OriginalUserId);

					try
					{
						foreach (int dependentIMGroupId in dependentIMGroups)
							IMManager.UpdateGroup(dependentIMGroupId);
					}
					catch (Exception)
					{
					}

					if (profileId > 0)
					{
						EntityObject entity = BusinessManager.InitializeEntity(CustomizationProfileUserEntity.ClassName);
						entity[CustomizationProfileUserEntity.FieldPrincipalId] = (PrimaryKeyId)UserID;
						entity[CustomizationProfileUserEntity.FieldProfileId] = (PrimaryKeyId)profileId;
						BusinessManager.Create(entity);
					}

					AssignDefaultCalendar(UserID);

					// Send allerts
					Dictionary<string, string> additionalValues = new Dictionary<string, string>();
					additionalValues.Add(Variable.Password.ToString(), password);
					SystemEvents.AddSystemEvents(SystemEventTypes.User_Created, UserID, additionalValues);

					tran.Commit();
				}
			}
			catch (Exception exception)
			{
				if (exception is SqlException)
				{
					SqlException sqlException = exception as SqlException;
					if (sqlException.Number == 2627 || sqlException.Number == 50000)
						throw new LoginDuplicationException();
				}
				throw;
			}

			return UserID;
		}
		#endregion

		#region CreateFromExternal
		public static void CreateFromExternal(
			int userId,
			string login,
			string password,
			string firstName,
			string lastName,
			string email,
			bool isActive,
			ArrayList groups,
			int imGroupId,
			string phone,
			string fax,
			string mobile,
			string position,
			string department,
			string company,
			string location,
			int timeZoneId,
			int languageId,
			string fileName,
			Stream data)
		{
			//check user rights
			if (!CanCreate())
				throw new AccessDeniedException();

			if (password == "")
				throw new PasswordRequiredException();

			int Activity = (int)UserActivity.Inactive;
			if (isActive)
			{
				if (!CanAddNewActiveUser(false))
					throw new MaxUsersCountException();
				Activity = (int)UserActivity.Active;
			}

			if (!Regex.IsMatch(login, @"^[\w-\.]+"))
				throw new WrongLoginException();

			//			// User Photo
			//			int FileId = 0;
			//			if (fileName != null && fileName != "")
			//			{
			//				using(IDataReader ProfileRader = DBUser.GetUserProfile(user_id))
			//				{
			//					if (ProfileRader.Read())
			//					{
			//						string PictureUrl = "";
			//						if (ProfileRader["PictureUrl"] != DBNull.Value)
			//						{
			//							PictureUrl = ProfileRader["PictureUrl"].ToString();
			//						}
			//						if (PictureUrl != "")
			//							FileId = DSFile.GetFileIDFromURL(PictureUrl);
			//					}
			//				}
			//			}

			int[] dependentIMGroups = GetDependentIMGroups(imGroupId);

			//update group list
			ArrayList NewGroups = new ArrayList(groups);
			ArrayList DeletedGroups = new ArrayList();
			if (CanUpdateSecureFields(userId))
			{
				using (IDataReader reader = DBUser.GetListSecureGroupExplicit(userId))
				{
					while (reader.Read())
					{
						int groupID = reader.GetInt32(0);
						if (NewGroups.Contains(groupID))
							NewGroups.Remove(groupID);
						else
							DeletedGroups.Add(groupID);
					}
				}
			}
			else
				NewGroups = new ArrayList();

			string salt = PasswordUtil.CreateSalt(PasswordUtil.SaltSize);
			string hash = PasswordUtil.CreateHash(password, salt);

			//Begin transaction
			int OriginalUserId = -1;
			try
			{
				using (DbTransaction tran = DbTransaction.Begin())
				{
					DBUser.ConvertFromExternal(userId, login, password, firstName, lastName, email, salt, hash, Activity, Security.CurrentUser.UserID);

					OriginalUserId = DBUser.AddToMain(-1, login, password, firstName, lastName, email, salt, hash, isActive, imGroupId);

					//update user profile
					DBUser.UpdateProfile(userId, phone, fax, mobile, position, department, company, location);

					// update user photo
					BindPhoto(userId, false, fileName, data);

					// Create default user preferences
					DBUser.UpdatePreferences(userId, true, true, true, false, 60, 8, 20, timeZoneId, languageId, 3);

					//Remove user from group
					foreach (int GroupID in DeletedGroups)
						DBUser.DeleteUserFromGroup(userId, GroupID);

					//Add user's groups to DB
					foreach (int GroupID in NewGroups)
						DBUser.AddUserToGroup(userId, GroupID);

					//Set IM Group and OriginalUserId
					DBUser.AssignIMGroup(userId, imGroupId);
					if (OriginalUserId > 0)
						DBUser.AssignOriginalId(userId, OriginalUserId);

					try
					{
						foreach (int dependentIMGroupId in dependentIMGroups)
							IMManager.UpdateGroup(dependentIMGroupId);
					}
					catch (Exception)
					{
					}

					// Send allerts
					Dictionary<string, string> additionalValues = new Dictionary<string, string>();
					additionalValues.Add(Variable.Password.ToString(), password);
					SystemEvents.AddSystemEvents(SystemEventTypes.User_Created, userId, additionalValues);

					tran.Commit();
				}
			}
			catch (Exception exception)
			{
				if (exception is SqlException)
				{
					SqlException sqlException = exception as SqlException;
					if (sqlException.Number == 2627 || sqlException.Number == 50000)
						throw new LoginDuplicationException();
				}
				throw;
			}
		}
		#endregion

		#region UpdateUserInfo
		public static void UpdateUserInfo(
			int userId,
			string password,
			string firstName,
			string lastName,
			string email,
			bool isActive,
			ArrayList groups,
			int imGroupId,
			string phone,
			string fax,
			string mobile,
			string position,
			string department,
			string company,
			string location,
			string fileName,
			Stream data,
			bool deletePicture,
			int profileId)
		{
			//check user rights
			if (!CanUpdateUserInfo(userId))
				throw new AccessDeniedException();

			int Activity = (int)UserActivity.Inactive;
			bool WasInactive = true;
			string oldEmail = "";
			bool isExternal = false;
			string login = "";

			using (IDataReader reader = DBUser.GetUserInfo(userId))
			{
				reader.Read();
				WasInactive = !(bool)reader["IsActive"];
				oldEmail = reader["Email"].ToString();
				isExternal = (bool)reader["IsExternal"];
				login = reader["Login"].ToString();

				if (reader["login"].ToString().ToLower() == "alert")	// AlertService user
				{
					isActive = false;
					groups.Clear();
					if (reader["IMGroupId"] != DBNull.Value)
						imGroupId = (int)reader["IMGroupId"];
				}
			}

			if (isActive)
			{
				if (WasInactive && !CanAddNewActiveUser(false))
					throw new MaxUsersCountException();

				Activity = (int)UserActivity.Active;
			}

			// Check Email
			if (oldEmail != email)
			{
				if (DBUser.GetUserByEmail(email, false) > 0)
					throw new EmailDuplicationException();

				if (DBUser.CheckEmail(email) > 0)
					throw new EmailDuplicationException();
			}

			//update group list
			ArrayList NewGroups = new ArrayList(groups);
			ArrayList DeletedGroups = new ArrayList();
			if (CanUpdateSecureFields(userId))
			{
				using (IDataReader reader = DBUser.GetListSecureGroupExplicit(userId))
				{
					while (reader.Read())
					{
						int groupID = reader.GetInt32(0);
						if (NewGroups.Contains(groupID))
							NewGroups.Remove(groupID);
						else
							DeletedGroups.Add(groupID);
					}
				}
			}
			else
				NewGroups = new ArrayList();
			// Check for Last Admin
			if (DeletedGroups.Contains((int)InternalSecureGroups.Administrator)
				&& IsLastAdmin(userId))
				DeletedGroups.Remove((int)InternalSecureGroups.Administrator);

			string salt = PasswordUtil.CreateSalt(PasswordUtil.SaltSize);
			string hash = PasswordUtil.CreateHash(password, salt);

			int OriginalUserId = DBUser.GetOriginalId(userId);

			//Begin transaction
			try
			{
				using (DbTransaction tran = DbTransaction.Begin())
				{
					if (CanUpdateSecureFields(userId))
					{
						//update user info
						DBUser.UpdateUserInfo(userId, password, firstName, lastName, email, salt, hash, Activity, false);

						//update user profile
						DBUser.UpdateProfile(userId, phone, fax, mobile, position, department, company, location);

						if (!isExternal)
						{
							OriginalUserId = DBUser.AddToMain(OriginalUserId, login, password, firstName, lastName, email, salt, hash, isActive, imGroupId);
							DBUser.AssignOriginalId(userId, OriginalUserId);
							DBUser.AssignIMGroup(userId, imGroupId);
						}

						//Remove user from group
						foreach (int GroupID in DeletedGroups)
							DBUser.DeleteUserFromGroup(userId, GroupID);

						//Add user's groups to DB
						foreach (int GroupID in NewGroups)
							DBUser.AddUserToGroup(userId, GroupID);
					}
					else
					{
						if (CanUpdatePassword(userId))
						{
							DBUser.UpdateUserInfo(userId, password, firstName, lastName, email, salt, hash);
							if (OriginalUserId > 0)
								DBUser.AddToMain(OriginalUserId, "", password, firstName, lastName, email, salt, hash, isActive, -1);
						}
						else
						{
							DBUser.UpdateUserInfo(userId, "", firstName, lastName, email, salt, hash);
							if (OriginalUserId > 0)
								DBUser.AddToMain(OriginalUserId, "", "", firstName, lastName, email, salt, hash, isActive, -1);
						}
					}

					DBUser.UpdateProfile(userId, phone, fax, mobile, position, department, company, location);

					//photo
					BindPhoto(userId, deletePicture, fileName, data);

					if (DeletedGroups.Count > 0 || NewGroups.Count > 0 || (WasInactive && isActive))
						IncreaseStubsVersion(userId);

					if (OriginalUserId > 0)
					{
						try
						{
							IMManager.UpdateUser(userId);
						}
						catch (Exception)
						{
						}
					}

					if (profileId > 0)
					{
						EntityObject entity = BusinessManager.InitializeEntity(CustomizationProfileUserEntity.ClassName);
						entity[CustomizationProfileUserEntity.FieldPrincipalId] = (PrimaryKeyId)userId;
						entity[CustomizationProfileUserEntity.FieldProfileId] = (PrimaryKeyId)profileId;
						BusinessManager.Create(entity);
					}
					else
					{
						EntityObject[] list = BusinessManager.List(CustomizationProfileUserEntity.ClassName, new FilterElement[] { FilterElement.EqualElement(CustomizationProfileUserEntity.FieldPrincipalId, userId) });
						if (list.Length > 0)
							BusinessManager.Delete(list[0]);
					}

					// Send allerts
					//SendAlert(AlertEventType.User_Updated, user_id);

					tran.Commit();
				}
			}
			catch (Exception exception)
			{
				if (exception is SqlException)
				{
					SqlException sqlException = exception as SqlException;
					if (sqlException.Number == 2627)
						throw new EmailDuplicationException();
				}
				throw;
			}

		}

		public static void UpdateUserInfo(
			string firstName,
			string lastName,
			string email,
			string phone,
			string mobile)
		{
			int userId = Security.CurrentUser.UserID;
			UpdateUserInfo(userId, firstName, lastName, email, phone, mobile);
		}

		public static void UpdateUserInfo(
			int userId,
			string firstName,
			string lastName,
			string email,
			string phone,
			string mobile)
		{
			int OriginalUserId = DBUser.GetOriginalId(userId);

			// Check Email
			string oldEmail = "";
			using (IDataReader reader = DBUser.GetUserInfo(userId))
			{
				reader.Read();
				oldEmail = reader["Email"].ToString();
			}
			if (oldEmail != email)
			{
				if (DBUser.GetUserByEmail(email, false) > 0)
					throw new EmailDuplicationException();

				if (DBUser.CheckEmail(email) > 0)
					throw new EmailDuplicationException();
			}

			try
			{
				using (DbTransaction tran = DbTransaction.Begin())
				{
					DBUser.UpdateUserInfo(userId, "", firstName, lastName, email, "", "");
					if (OriginalUserId > 0)
						DBUser.UpdateMain(OriginalUserId, firstName, lastName, email);
					DBUser.UpdateProfile(userId, phone, mobile);

					if (OriginalUserId > 0)
					{
						try
						{
							IMManager.UpdateUser(userId);
						}
						catch (Exception)
						{
						}
					}

					// Send allerts
					//SendAlert(AlertEventType.User_Updated, user_id);

					tran.Commit();
				}
			}
			catch (Exception exception)
			{
				if (exception is SqlException)
				{
					SqlException sqlException = exception as SqlException;
					if (sqlException.Number == 2627)
						throw new EmailDuplicationException();
				}
				throw;
			}
		}

		public static void UpdateUserInfo(
			string position,
			string department,
			string company,
			string location,
			string fileName,
			Stream data)
		{
			int user_id = Security.CurrentUser.UserID;

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBUser.UpdateProfile(user_id, position, department, company, location);
				// update user photo
				BindPhoto(user_id, false, fileName, data);
				// Send allerts
				//SendAlert(AlertEventType.User_Updated, user_id);

				tran.Commit();
			}
		}
		#endregion

		#region UpdatePreferences
		public static void UpdatePreferences(int user_id,
			bool is_notified,
			bool is_notifiedbymail,
			bool is_notifiedbyibn,
			bool is_batchnotifications,
			int period,
			int from,
			int till,
			int time_zone_id,
			int language_id,
			bool bMenuInAlerts,
			int ReminderType)
		{
			//check user rights
			if (!CanUpdatePreferences(user_id))
				throw new AccessDeniedException();

			//TODO Check incoming params
			is_notified = is_notified && (is_notifiedbymail || is_notifiedbyibn);

			DateTime nextBatch = DateTime.MinValue;
			if (is_batchnotifications)
			{
				DateTime now = User.GetLocalDate(time_zone_id, DateTime.UtcNow);
				nextBatch = now.Date;
				if (from != till)
					nextBatch = nextBatch.AddHours(from);
				while (nextBatch < now)
					nextBatch = nextBatch.AddMinutes(period);
				if (from != till && nextBatch.Hour >= till || nextBatch.Hour < from)
					nextBatch = now.Date.AddDays(1).AddHours(from);
				nextBatch = GetUTCDate(time_zone_id, nextBatch);
			}

			//Begin transaction
			using (DbTransaction tran = DbTransaction.Begin())
			{
				// Update  userinfo in DB
				DBUser.UpdatePreferences(user_id, is_notified, is_notifiedbymail, is_notifiedbyibn, is_batchnotifications, period, from, till, time_zone_id, language_id, ReminderType);
				DBUser.UpdateMenuInAlerts(user_id, bMenuInAlerts);
				if (is_batchnotifications)
				{
					// Update next batch time
					User.UpdateBatchNextSend(user_id, nextBatch);
					// Add next batch time to schedule
					Schedule.UpdateDateTypeValue(DateTypes.BatchAlert, user_id, nextBatch);
				}
				else
					Schedule.DeleteDateTypeValue(DateTypes.BatchAlert, user_id);

				//TODO send alerts

				tran.Commit();
			}
		}
		#endregion

		#region GetUserInfo
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, Password, FirstName, LastName, Email, Activity, IsActive, IMGroupId, CreatedBy, Comments, IsExternal, IsPending, OriginalId
		/// </summary>
		public static IDataReader GetUserInfo(int user_id)
		{
			return GetUserInfo(user_id, true);
		}
		#endregion

		#region GetUserInfo
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, Password, FirstName, LastName, Email, Activity, IsActive, IMGroupId, CreatedBy, Comments, IsExternal, IsPending, OriginalId
		/// </summary>
		public static IDataReader GetUserInfo(int user_id, bool check_rights)
		{
			if (check_rights)
				if (!CanRead(user_id))
					throw new AccessDeniedException();

			return DBUser.GetUserInfo(user_id);
		}
		#endregion

		#region GetUserInfoByOriginalId
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, Activity, IMGroupId
		/// </summary>
		public static IDataReader GetUserInfoByOriginalId(int OriginalId)
		{
			return DBUser.GetUserInfoByOriginalId(OriginalId);
		}
		#endregion

		#region GetUserPreferences
		/// <summary>
		/// UserId, IsNotified, IsNotifiedByEmail, IsNotifiedByIBN, IsBatchNotifications, 
		/// Period, From, Till, TimeZoneId, TimeOffsetLatest, LanguageId, Locale, 
		/// LanguageName, ReminderType, BatchLastSent, BatchNextSend
		/// </summary>
		public static IDataReader GetUserPreferencesTO(int TimeZoneId, int user_id)
		{
			return DBUser.GetUserPreferences(TimeZoneId, user_id);
		}

		public static IDataReader GetUserPreferences(int user_id)
		{
			return DBUser.GetUserPreferences(user_id);
		}
		#endregion

		#region GetUserLanguage
		/// <summary>
		/// Gets the user language.
		/// </summary>
		/// <param name="user_id">The user_id.</param>
		/// <returns></returns>
		public static int GetUserLanguage(int user_id)
		{
			using (IDataReader reader = User.GetUserPreferences(user_id))
			{
				reader.Read();
				return (int)reader["LanguageId"];
			}
		}
		#endregion

		#region GetUserLogin
		/// <summary>
		/// Gets the user login.
		/// </summary>
		/// <param name="user_id">The user_id.</param>
		/// <returns></returns>
		public static int GetUserTimeZoneId(int user_id)
		{
			using (IDataReader reader = User.GetUserPreferences(user_id))
			{
				reader.Read();
				return (int)reader["TimeZoneId"];
			}
		}
		#endregion
		#region GetUserLogin
		/// <summary>
		/// Gets the user login.
		/// </summary>
		/// <param name="user_id">The user_id.</param>
		/// <returns></returns>
		public static string GetUserLogin(int user_id)
		{
			using (IDataReader reader = DBUser.GetUserInfo(user_id))
			{
				reader.Read();
				return (string)reader["Login"];
			}
		}
		#endregion

		#region GetUserEmail
		/// <summary>
		/// Gets the user email.
		/// </summary>
		/// <param name="user_id">The user_id.</param>
		/// <returns></returns>
		public static string GetUserEmail(int user_id)
		{
			using (IDataReader reader = DBUser.GetUserInfo(user_id))
			{
				reader.Read();
				return (string)reader["Email"];
			}
		}
		#endregion

		#region GetUserProfile
		/// <summary>
		/// Reader returns fields:
		///		UserId, phone, fax, mobile, position, department, company, location, PictureUrl 
		/// </summary>
		public static IDataReader GetUserProfile(int user_id)
		{
			return GetUserProfile(user_id, true);
		}

		public static IDataReader GetUserProfile(int user_id, bool check_rights)
		{
			if (check_rights)
				if (!CanRead(user_id))
					throw new AccessDeniedException();

			return DBUser.GetUserProfile(user_id);
		}
		#endregion

		#region GetListSecureGroup
		/// <summary>
		/// Reader returns fields:
		///		GroupId, GroupName
		/// </summary>
		public static IDataReader GetListSecureGroup(int user_id)
		{
			return DBUser.GetListSecureGroupExplicit(user_id);
		}
		#endregion

		#region GetListTimeZone
		/// <summary>
		/// TimeZoneId, Bias, StandardBias, DaylightBias, DaylightMonth, DaylightDayOfWeek, 
		///	DaylightWeek, DaylightHour, StandardMonth, StandardDayOfWeek, StandardWeek, 
		///	StandardHour, DisplayName, LanguageId
		/// </summary>
		public static IDataReader GetListTimeZone()
		{
			int LanguageId;
			if (Security.CurrentUser != null)
				LanguageId = Security.CurrentUser.LanguageId;
			else
				LanguageId = GetDefaultLanguage();
			return DBCommon.GetListTimeZone(LanguageId);
		}
		#endregion

		#region UpdateLanguage
		public static void UpdateLanguage(int user_id, int language_id)
		{
			DBUser.UpdateLanguage(user_id, language_id);
		}
		#endregion

		#region UpdateTimeZoneId
		public static void UpdateTimeZoneId(int user_id, int time_zone_id)
		{
			DBUser.UpdateTimeZoneId(user_id, time_zone_id);
		}
		#endregion

		#region UpdateTimeOffsetLatest
		public static void UpdateTimeOffsetLatest(int user_id, int time_offset)
		{
			DBUser.UpdateTimeOffsetLatest(user_id, time_offset);
		}
		#endregion

		#region GetListPeopleForSharing
		/// <summary>
		/// Reader returns fields:
		///  UserId, FirstName, LastName, Email, Login, Level
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListPeopleForSharing(int UserId)
		{
			return DBUser.GetListSharingByUser(UserId);
		}
		#endregion

		#region GetListPeopleForSharingDataTable
		/// <summary>
		/// DataTable returns fields:
		///  UserId, FirstName, LastName, Email, Login, Level
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListPeopleForSharingDataTable(int UserId)
		{
			return DBUser.GetListSharingByUserDataTable(UserId);
		}
		#endregion

		#region UpdateSharing
		// dt contains fields: int UserId, int Level
		public static void UpdateSharing(int CurrentUserId, DataTable dt)
		{
			ArrayList NewUsers = new ArrayList();
			ArrayList DeletedUsers = new ArrayList();
			foreach (DataRow dr in dt.Rows)
			{
				NewUsers.Add((int)dr["UserId"]);
			}

			using (IDataReader reader = DBUser.GetListSharingByUser(CurrentUserId))
			{
				while (reader.Read())
				{
					int UserId = (int)reader["UserId"];
					if (NewUsers.Contains(UserId))
						NewUsers.Remove(UserId);
					else
						DeletedUsers.Add(UserId);
				}
			}

			using (DbTransaction tran = DbTransaction.Begin())
			{
				// Remove Users
				foreach (int UserId in DeletedUsers)
					DBUser.DeleteSharing(CurrentUserId, UserId);

				// Add new Users and update Level for existing Users
				foreach (DataRow dr in dt.Rows)
				{
					int UserId = (int)dr["UserId"];
					int Level = (int)dr["Level"];
					DBUser.AddSharing(CurrentUserId, UserId, Level);
				}

				//Send alerts
				//if(DeletedUsers.Count > 0)
				//SendAlert(AlertEventType.Sharing_Disabled, CurrentUserId, (int[])DeletedUsers.ToArray(typeof(int)));
				//if(NewUsers.Count > 0)
				//SendAlert(AlertEventType.Sharing_Enabled, CurrentUserId, (int[])NewUsers.ToArray(typeof(int)));

				tran.Commit();
			}
		}
		#endregion

		#region GetListUsersBySubstring
		/// <summary>
		/// Reader returns fields:
		///		UserId, FirstName, LastName, Email, Login, IsExternal, IsPending
		/// </summary>
		public static IDataReader GetListUsersBySubstring(string SubString)
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
				return DBUser.GetListUsersBySubstring(SubString);
			else
				return DBUser.GetListUsersBySubstringForPartnerUser(Security.CurrentUser.UserID, SubString);
		}
		#endregion

		#region GetListUsersBySubstringDataTable
		/// <summary>
		/// Reader returns fields:
		///		UserId, FirstName, LastName, Email, Login, IsExternal, IsPending
		/// </summary>
		public static DataTable GetListUsersBySubstringDataTable(string SubString)
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
				return DBUser.GetListUsersBySubstringDataTable(SubString);
			else
				return DBUser.GetListUsersBySubstringForPartnerUserDataTable(Security.CurrentUser.UserID, SubString);
		}
		#endregion

		#region CreateExternal
		public static int CreateExternal(string first_name, string last_name, string email, ArrayList groups,
			string phone, string mobile, string position, string department,
			string company, string location, int language_id, string invite_text)
		{
			return CreateExternal(first_name, last_name, email, groups, true, phone, "", mobile,
				position, department, company, location, DefaultTimeZoneId, language_id, invite_text, null, null);
		}

		public static int CreateExternal(string first_name, string last_name, string email, ArrayList groups,
			bool isactive, string phone, string fax, string mobile, string position,
			string department, string company, string location, int time_zone_id,
			int language_id, string invite_text, string fileName, Stream data)
		{

			//check user rights
			if (!CanCreateExternal())
				throw new AccessDeniedException();

			int Activity = (int)UserActivity.Inactive;
			if (isactive)
			{
				if (!CanAddNewActiveUser(true))
					throw new MaxUsersCountException();
				Activity = (int)UserActivity.Active;
			}

			// Check Email
			if (DBUser.GetUserByEmail(email, false) > 0)
				throw new EmailDuplicationException();

			if (DBUser.CheckEmail(email) > 0)
				throw new EmailDuplicationException();

			string salt = PasswordUtil.CreateSalt(PasswordUtil.SaltSize);
			string hash = PasswordUtil.CreateHash("", salt);

			int UserID = -1;
			try
			{
				using (DbTransaction tran = DbTransaction.Begin())
				{
					UserID = DBUser.Create(email, null, "", first_name, last_name, email, salt, hash, Activity, true, Security.CurrentUser.UserID, invite_text);

					//update user profile
					DBUser.UpdateProfile(UserID, phone, fax, mobile, position, department, company, location);

					// Picture
					BindPhoto(UserID, false, fileName, data);
					//					string FileUrl = "";
					//					if (fileName != null && fileName != "")
					//					{
					//						FileUrl = DSFile.UploadFile(fileName, data);
					//						DBUser.UpdateUserPhoto(UserID, FileUrl);
					//					}

					// Create default user preferences
					DBUser.UpdatePreferences(UserID, true, true, false, false, 60, 8, 20, time_zone_id, language_id, 3);

					foreach (int GroupID in groups)
					{
						DBUser.AddUserToGroup(UserID, GroupID);
					}

					AssignDefaultCalendar(UserID);

					// Send allerts
					SystemEvents.AddSystemEvents(SystemEventTypes.User_Created_External, UserID);

					tran.Commit();
				}
			}
			catch (Exception exception)
			{
				if (exception is SqlException)
				{
					SqlException sqlException = exception as SqlException;
					if (sqlException.Number == 2627)
						throw new LoginDuplicationException();
				}
				throw;
			}

			return UserID;
		}
		#endregion

		#region UpdateExternalInfo
		public static void UpdateExternalInfo(
			int user_id,
			string first_name,
			string last_name,
			string email,
			ArrayList groups,
			bool isactive,
			string phone,
			string fax,
			string mobile,
			string position,
			string department,
			string company,
			string location,
			string fileName,
			Stream data)
		{
			//check user rights
			if (!CanUpdateExternal())
				throw new AccessDeniedException();

			int Activity = (int)UserActivity.Inactive;
			bool WasInactive = true;
			string oldEmail = "";
			using (IDataReader reader = DBUser.GetUserInfo(user_id))
			{
				reader.Read();
				WasInactive = !(bool)reader["IsActive"];
				oldEmail = reader["Email"].ToString();
			}
			if (isactive)
			{
				if (WasInactive && !CanAddNewActiveUser(true))
					throw new MaxUsersCountException();

				Activity = (int)UserActivity.Active;
			}

			// Check Email
			if (oldEmail != email)
			{
				if (DBUser.GetUserByEmail(email, false) > 0)
					throw new EmailDuplicationException();

				if (DBUser.CheckEmail(email) > 0)
					throw new EmailDuplicationException();
			}

			//			// User Photo
			//			int FileId = 0;
			//			if (fileName != null && fileName != "")
			//			{
			//				using(IDataReader ProfileRader = DBUser.GetUserProfile(user_id))
			//				{
			//					if (ProfileRader.Read())
			//					{
			//						string PictureUrl = "";
			//						if (ProfileRader["PictureUrl"] != DBNull.Value)
			//						{
			//							PictureUrl = ProfileRader["PictureUrl"].ToString();
			//						}
			//						if (PictureUrl != "")
			//							FileId = DSFile.GetFileIDFromURL(PictureUrl);
			//					}
			//				}
			//			}

			//update group list
			ArrayList NewGroups = new ArrayList(groups);
			ArrayList DeletedGroups = new ArrayList();
			if (CanUpdateSecureFields(user_id))
			{
				using (IDataReader reader = DBUser.GetListSecureGroupExplicit(user_id))
				{
					while (reader.Read())
					{
						int groupID = reader.GetInt32(0);
						if (NewGroups.Contains(groupID))
							NewGroups.Remove(groupID);
						else
							DeletedGroups.Add(groupID);
					}
				}
			}
			else
				NewGroups = new ArrayList();

			//Begin transaction
			try
			{
				using (DbTransaction tran = DbTransaction.Begin())
				{
					DBUser.UpdateExternalUserInfo(user_id, first_name, last_name, email, Activity);
					DBUser.UpdateProfile(user_id, phone, fax, mobile, position, department, company, location);

					//Remove user from group
					foreach (int GroupID in DeletedGroups)
						DBUser.DeleteUserFromGroup(user_id, GroupID);

					//Add user's groups to DB
					foreach (int GroupID in NewGroups)
						DBUser.AddUserToGroup(user_id, GroupID);

					// update user photo
					BindPhoto(user_id, false, fileName, data);
					//					if (fileName != null && fileName != "")
					//					{
					//						if (FileId > 0)
					//							DSFile.Delete(FileId);
					//						string FileUrl = DSFile.UploadFile(fileName, data);
					//						DBUser.UpdateUserPhoto(user_id, FileUrl);
					//					}

					// Send allerts
					//SendAlert(AlertEventType.User_Updated, user_id);

					tran.Commit();
				}
			}
			catch (Exception exception)
			{
				if (exception is SqlException)
				{
					SqlException sqlException = exception as SqlException;
					if (sqlException.Number == 2627)
						throw new LoginDuplicationException();
				}
				throw;
			}

		}
		#endregion

		#region CreatePending
		public static int CreatePending(string login, string password, string first_name,
			string last_name, string email, ArrayList groups, string phone, string mobile,
			string position, string department, string company, string location,
			int language_id, string invite_text)
		{
			return CreatePending(login, password, first_name, last_name, email, groups, phone, "",
				mobile, position, department, company, location, DefaultTimeZoneId, language_id, invite_text, null, null);
		}

		public static int CreatePending(string login, string password, string first_name,
			string last_name, string email, ArrayList groups, string phone, string fax, string mobile,
			string position, string department, string company, string location,
			int time_zone_id, int language_id, string invite_text, string fileName, Stream data)
		{

			if (login == "")
				login = "___PENDING_USER___" + Guid.NewGuid().ToString();

			if (!Regex.IsMatch(login, @"^[\w-\.]+"))
				throw new WrongLoginException();

			// Check Email
			if (DBUser.GetUserByEmail(email, false) > 0)
				throw new EmailDuplicationException();
			if (DBUser.CheckEmail(email) > 0)
				throw new EmailDuplicationException();

			int CreatedBy = -1;
			if (Security.CurrentUser != null)
				CreatedBy = Security.CurrentUser.UserID;

			string salt = PasswordUtil.CreateSalt(PasswordUtil.SaltSize);
			string hash = PasswordUtil.CreateHash(password, salt);

			int UserID = -1;
			try
			{
				using (DbTransaction tran = DbTransaction.Begin())
				{
					UserID = DBUser.Create(login, null, password, first_name, last_name, email, salt, hash, (int)UserActivity.Pending, false, CreatedBy, invite_text);

					//update user profile
					DBUser.UpdateProfile(UserID, phone, fax, mobile, position, department, company, location);

					// Picture
					BindPhoto(UserID, false, fileName, data);
					//					string FileUrl = "";
					//					if (fileName != null && fileName != "")
					//					{
					//						FileUrl = DSFile.UploadFile(fileName, data);
					//						DBUser.UpdateUserPhoto(UserID, FileUrl);
					//					}

					// Create default user preferences
					DBUser.UpdatePreferences(UserID, true, true, false, false, 60, 8, 20, time_zone_id, language_id, 3);

					//Add user's groups to DB
					foreach (int GroupID in groups)
					{
						DBUser.AddUserToGroup(UserID, GroupID);
					}

					// Send alerts
					SystemEvents.AddSystemEvents(SystemEventTypes.User_Created_Request, UserID);

					tran.Commit();
				}
			}
			catch (Exception exception)
			{
				if (exception is SqlException)
				{
					SqlException sqlException = exception as SqlException;
					if (sqlException.Number == 2627)
						throw new LoginDuplicationException();
				}
				throw;
			}

			return UserID;
		}
		#endregion

		#region ApprovePending
		public static void ApprovePending(
			int userId,
			string login,
			string password,
			string firstName,
			string lastName,
			string email,
			bool isActive,
			ArrayList groups,
			int imGroupId,
			string phone,
			string fax,
			string mobile,
			string position,
			string department,
			string company,
			string location,
			string fileName,
			Stream data,
			int profileId)
		{
			if (!CanCreate())
				throw new AccessDeniedException();

			if (password == "")
				password = DBUser.GetPassword(userId);

			if (password == "")
				throw new PasswordRequiredException();

			int Activity = (int)UserActivity.Inactive;
			if (isActive)
			{
				if (!CanAddNewActiveUser(false))
					throw new MaxUsersCountException();

				Activity = (int)UserActivity.Active;
			}

			// Check Email
			string oldEmail = "";
			using (IDataReader reader = DBUser.GetUserInfo(userId))
			{
				reader.Read();
				oldEmail = reader["Email"].ToString();
			}
			if (oldEmail != email)
			{
				if (DBUser.GetUserByEmail(email, false) > 0)
					throw new EmailDuplicationException();

				if (DBUser.CheckEmail(email) > 0)
					throw new EmailDuplicationException();
			}

			int[] dependentIMGroups = GetDependentIMGroups(imGroupId);

			//update group list
			ArrayList NewGroups = new ArrayList(groups);
			ArrayList DeletedGroups = new ArrayList();
			if (CanUpdateSecureFields(userId))
			{
				using (IDataReader reader = DBUser.GetListSecureGroupExplicit(userId))
				{
					while (reader.Read())
					{
						int groupID = reader.GetInt32(0);
						if (NewGroups.Contains(groupID))
							NewGroups.Remove(groupID);
						else
							DeletedGroups.Add(groupID);
					}
				}
			}
			else
				NewGroups = new ArrayList();

			string salt = PasswordUtil.CreateSalt(PasswordUtil.SaltSize);
			string hash = PasswordUtil.CreateHash(password, salt);

			int OriginalUserId;
			//Begin transaction
			try
			{
				using (DbTransaction tran = DbTransaction.Begin())
				{
					DBUser.UpdatePendingUserInfo(userId, login, password, firstName, lastName, email, salt, hash, Activity, Security.CurrentUser.UserID);
					DBUser.UpdateProfile(userId, phone, fax, mobile, position, department, company, location);

					// update user photo
					BindPhoto(userId, false, fileName, data);

					//Remove user from group
					foreach (int GroupID in DeletedGroups)
						DBUser.DeleteUserFromGroup(userId, GroupID);

					//Add user's groups to DB
					foreach (int GroupID in NewGroups)
						DBUser.AddUserToGroup(userId, GroupID);

					//Set IM Group and OriginalUserId
					OriginalUserId = DBUser.AddToMain(-1, login, password, firstName, lastName, email, salt, hash, isActive, imGroupId);
					DBUser.AssignOriginalId(userId, OriginalUserId);
					DBUser.AssignIMGroup(userId, imGroupId);

					try
					{
						foreach (int dependentIMGroupId in dependentIMGroups)
							IMManager.UpdateGroup(dependentIMGroupId);
					}
					catch (Exception)
					{
					}

					if (profileId > 0)
					{
						EntityObject entity = BusinessManager.InitializeEntity(CustomizationProfileUserEntity.ClassName);
						entity[CustomizationProfileUserEntity.FieldPrincipalId] = (PrimaryKeyId)userId;
						entity[CustomizationProfileUserEntity.FieldProfileId] = (PrimaryKeyId)profileId;
						BusinessManager.Create(entity);
					}

					AssignDefaultCalendar(userId);

					// Send allerts
					Dictionary<string, string> additionalValues = new Dictionary<string, string>();
					additionalValues.Add(Variable.Password.ToString(), password);
					SystemEvents.AddSystemEvents(SystemEventTypes.User_Created, userId, additionalValues);

					tran.Commit();
				}
			}
			catch (Exception exception)
			{
				if (exception is SqlException)
				{
					SqlException sqlException = exception as SqlException;
					if (sqlException.Number == 2627 || sqlException.Number == 50000)
						throw new LoginDuplicationException();
				}
				throw;
			}

		}
		#endregion

		#region GetUserStatus
		public static int GetUserStatus(int UserId)
		{
			return DBUser.GetUserStatus(UserId);
		}
		#endregion

		#region GetTimeZoneNameByTimeOffset
		public static string GetTimeZoneName(int TimeZoneId)
		{
			return DBCommon.GetTimeZoneName(TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetDefaultLanguage
		public static int GetDefaultLanguage()
		{
			return DBCommon.GetDefaultLanguage();
		}
		#endregion

		#region GetUserByEmail
		public static int GetUserByEmail(string Email)
		{
			return DBUser.GetUserByEmail(Email, false);
		}
		#endregion

		#region GetUserByLogin
		public static int GetUserByLogin(string Login)
		{
			return DBUser.GetUserByLogin(Login);
		}
		#endregion

		#region GetUserByWindowsLogin()
		public static int GetUserByWindowsLogin(string windowsLogin)
		{
			return DBUser.GetUserByWindowsLogin(windowsLogin);
		}
		#endregion

		#region GetUserInfoByLogin
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IsActive, IMGroupId, salt, hash
		/// </summary>
		public static IDataReader GetUserInfoByLogin(string Login)
		{
			return DBUser.GetUserInfoByLogin(Login);
		}
		#endregion

		#region CheckForChangeableRoles
		public static bool CheckForChangeableRoles(int UserId)
		{
			return DBProject.CheckForChangeableRoles(UserId) == 1
				|| DBTask.CheckForChangeableRoles(UserId) == 1
				|| DBToDo.CheckForChangeableRoles(UserId) == 1
				|| DBEvent.CheckForChangeableRoles(UserId) == 1
				|| DBIncident.CheckForChangeableRoles(UserId) == 1
				|| DBDocument.CheckForChangeableRoles(UserId) == 1;
		}
		#endregion

		#region CheckForUnchangeableRoles
		public static bool CheckForUnchangeableRoles(int UserId)
		{
			return DBProject.CheckForUnchangeableRoles(UserId) == 1
				|| DBTask.CheckForUnchangeableRoles(UserId) == 1
				|| DBToDo.CheckForUnchangeableRoles(UserId) == 1
				|| DBEvent.CheckForUnchangeableRoles(UserId) == 1
				|| DBIncident.CheckForUnchangeableRoles(UserId) == 1
				|| DBDocument.CheckForUnchangeableRoles(UserId) == 1
				|| DBCommon.CheckDiscussionsForUnchangeableRoles(UserId) == 1
				|| DBReport.CheckForUnchangeableRoles(UserId) == 1
				|| DBProjectTemplate.CheckForUnchangeableRoles(UserId) == 1
				|| DBProjectGroup.CheckForUnchangeableRoles(UserId) == 1
				|| DBFinance.CheckForUnchangeableRoles(UserId) == 1
				|| DBUser.CheckForUnchangeableRoles(UserId) == 1
				|| !EMail.EMailRouterPop3Box.CanDeleteUser(UserId)
				|| !EMail.IncidentBox.CanDeleteUser(UserId);
		}
		#endregion

		#region ReplaceRelations
		public static void ReplaceRelations(int FromUserId, int ToUserId, bool UseManager)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				if (UseManager)
				{
					// Projects 
					DBProject.ReplaceUnchangeableUserToManager(FromUserId);

					// Tasks
					DBTask.ReplaceUnchangeableUserToManager(FromUserId);

					// ToDo
					DBToDo.ReplaceUnchangeableUserToManager(FromUserId);

					// Events
					DBEvent.ReplaceUnchangeableUserToManager(FromUserId);

					// Incidents
					DBIncident.ReplaceUnchangeableUserToManager(FromUserId);

					// Documents
					DBDocument.ReplaceUnchangeableUserToManager(FromUserId);

					// Discussions
					DBCommon.ReplaceDiscussionsUnchangeableUserToManager(FromUserId);

					// Finances
					DBFinance.ReplaceUnchangeableUserToManager(FromUserId);
				}
				else
				{
					// Projects 
					DBProject.ReplaceUnchangeableUser(FromUserId, ToUserId);

					// Tasks
					DBTask.ReplaceUnchangeableUser(FromUserId, ToUserId);

					// ToDo
					DBToDo.ReplaceUnchangeableUser(FromUserId, ToUserId);

					// Events
					DBEvent.ReplaceUnchangeableUser(FromUserId, ToUserId);

					// Incidents
					DBIncident.ReplaceUnchangeableUser(FromUserId, ToUserId);

					// Documents
					DBDocument.ReplaceUnchangeableUser(FromUserId, ToUserId);
				}

				// Discussions (оставшиеся)
				DBCommon.ReplaceDiscussionsUnchangeableUser(FromUserId, ToUserId);

				// Reports
				DBReport.ReplaceUnchangeableUser(FromUserId, ToUserId);

				// ProjectTemplates
				DBProjectTemplate.ReplaceUnchangeableUser(FromUserId, ToUserId);

				// Project Groups
				DBProjectGroup.ReplaceUnchangeableUser(FromUserId, ToUserId);

				// Finances (оставшиеся)
				DBFinance.ReplaceUnchangeableUser(FromUserId, ToUserId);

				// MetaData
				DBUser.ReplaceUserInMetaData(FromUserId, ToUserId);

				//EmailRouterPop3Box
				EMail.EMailRouterPop3Box.ReplaseUser(FromUserId, ToUserId);

				//IncidentBox
				EMail.IncidentBox.ReplaseUser(FromUserId, ToUserId);

				Delete(FromUserId);

				tran.Commit();
			}
		}
		#endregion

		#region Delete
		public static void Delete(int userId)
		{
			int imUserId = -1;
			int imGroupId = -1;
			string email;

			using (IDataReader reader = DBUser.GetUserInfo(userId))
			{
				reader.Read();

				string Login = reader["Login"].ToString();
				if (Login.ToLower() == "alert")
					throw new AccessDeniedException();

				email = reader["Email"].ToString();

				if (reader["OriginalId"] != DBNull.Value)
					imUserId = (int)reader["OriginalId"];
				if (reader["IMGroupId"] != DBNull.Value)
					imGroupId = (int)reader["IMGroupId"];

			}

			int[] dependentIMGroups = GetDependentIMGroups(imGroupId);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				// Send allerts
				SystemEvents.AddSystemEvents(SystemEventTypes.User_Deleted, userId, email);

				if (imUserId > 0)
				{
					DBUser.UpdateActivityInMain(imUserId, false);

					try
					{
						IMManager.LogOff(imUserId);
					}
					catch (Exception)
					{
					}
				}

				if (imUserId > 0)
					DBUser.DeleteFromMain(imUserId);

				MetaObject.Delete(userId, "UsersEx");

				//EmailRouterPop3Box
				EMail.EMailRouterPop3Box.DeleteUser(userId);

				//IncidentBox
				EMail.IncidentBox.DeleteUser(userId);

				//OZ: При удалении пользователя вычищать таблицы cls_XXX_GlobalAcl, cls_XXX_ExceptionAcl, cls_XXX_Role_Principal, cls_XXX_ReadAcl и 
				foreach (Idm.Management.MetaClass mc in DataContext.Current.MetaModel.MetaClasses)
				{
					// вычищать таблицы cls_XXX_GlobalAcl
					if (mc.Name.EndsWith(IdmServices.Security.GlobalACLPostfix))
					{
						foreach (Idm.MetaObject obj in Idm.MetaObject.List(mc, FilterElement.EqualElement("PrincipalId", userId)))
						{
							obj.Delete();
						}
					}
					// вычищать таблицы cls_XXX_ExceptionAcl
					else if (mc.Name.EndsWith(IdmServices.Security.ExceptionACLPostfix))
					{
						foreach (Idm.MetaObject obj in Idm.MetaObject.List(mc, FilterElement.EqualElement("PrincipalId", userId)))
						{
							obj.Delete();
						}
					}
					// cls_XXX_Role_Principal
					else if (mc.Name.EndsWith(IdmServices.RoleManager.RolePrincipalPostfix))
					{
						foreach (Idm.MetaObject obj in Idm.MetaObject.List(mc, FilterElement.EqualElement("PrincipalId", userId)))
						{
							obj.Delete();
						}
					}
					// cls_XXX_ReadAcl
					else if (mc.Name.EndsWith("_ReadACL"))
					{
						foreach (Idm.MetaObject obj in Idm.MetaObject.List(mc, FilterElement.EqualElement("PrincipalId", userId)))
						{
							obj.Delete();
						}
					}
				}

				//OZ: MetaViewPreferences,  связанные с пользователем.
				Mediachase.Ibn.Core.UserMetaViewPreference.DeleteAll(userId);

				DBUser.Delete(userId);

				if (imUserId > 0)
				{
					try
					{
						foreach (int dependentIMGroupId in dependentIMGroups)
							IMManager.UpdateGroup(dependentIMGroupId);
					}
					catch (Exception)
					{
					}
				}

				tran.Commit();
			}
		}
		#endregion

		#region IncreaseStubsVersion
		public static void IncreaseStubsVersion(int UserId)
		{
			DBUser.IncreaseStubsVersion(UserId);
		}
		#endregion

		#region SendForgottenPassword
		public static bool SendForgottenPassword(string login)
		{
			bool ret = false;
			int userId = -1;
			if (login.IndexOf("@") > -1)	// email
			{
				userId = User.GetUserByEmail(login);
				if (userId > 0)
				{
					using (IDataReader reader = DBUser.GetUserInfo(userId))
					{
						if (reader.Read())
							login = reader["Login"].ToString();
					}
				}
			}
			else
			{
				using (IDataReader reader = GetUserInfoByLogin(login))
				{
					if (reader.Read())
						userId = (int)reader["UserId"];
				}
			}

			if (userId > 0)
			{
				string ticket = string.Empty;
				using (IDataReader reader = DBUser.CreateUserTicket(userId))
				{
					if (reader.Read())
					{
						ticket = reader["Ticket"].ToString();
					}
				}

				if (!String.IsNullOrEmpty(ticket))
				{
					string link = String.Concat
					(
						Configuration.PortalLink, "/",
						"Public/default.aspx?login=", login, "&ticket=", ticket, "&redirect=",
						 System.Web.HttpUtility.UrlEncode("Directory/UserEdit.aspx?UserID="),
						 userId.ToString()
					);
					Alerts2.SendForgottenPassword(userId, link);
					ret = true;
				}
			}

			return ret;
		}
		#endregion

		#region IsExternal
		public static bool IsExternal(int UserId)
		{
			bool RetVal = false;
			using (IDataReader reader = DBUser.GetUserInfo(UserId))
			{
				if (reader.Read())
				{
					RetVal = (bool)reader["IsExternal"];
				}
			}
			return RetVal;
		}
		#endregion

		#region CheckUserIdByExternalGate
		public static bool CheckUserIdByExternalGate(int ObjectTypeId, int ObjectId, int UserId)
		{
			if (IsExternal(UserId))
			{
				string DBGUID = DBCommon.GetGateGuid(ObjectTypeId, ObjectId, UserId);
				if (String.IsNullOrEmpty(DBGUID))
					return false;

				return true;
			}
			else
				return false;
		}
		#endregion

		#region GetGateByGuid
		public static IDataReader GetGateByGuid(string guid)
		{
			return DBCommon.GetGateByGuid(guid);
		}
		#endregion

		#region GetUserStatistic
		/// <summary>
		/// TotalUserCount, ActiveUserCount, InactiveUserCount, 
		/// ExternalCount, ExternalActiveCount, ExternalInactiveCount,
		/// PendingCount, SecureGroupCount, AvgCountUserInGroup, PartnerGroupCount,
		/// PartnerUserCount, RegularUserCount, PortalLoginsCount,
		/// ActiveUserTotalCount, InactiveUserTotalCount
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetUserStatistic()
		{
			return DBUser.GetUserStatistic();
		}
		#endregion

		#region GetIMUserStatistics
		/// <summary>
		/// Reader returns fields:
		/// IMGroupCount, AvgCountUserInIMGroup
		/// </summary>
		/// <returns></returns>
		public static DataTable GetIMUserStatistics()
		{
			return DBUser.GetIMUserStatistics();
		}
		#endregion


		#region CreatePartnerUser
		public static int CreatePartnerUser(string login, string password, string first_name,
			string last_name, string email, bool isactive, int GroupId,
			string phone, string fax, string mobile, string position, string department,
			string company, string location, int time_zone_id, int language_id, 
			string fileName, Stream data, int profileId)
		{
			return CreatePartnerUser(login, password, first_name, last_name, email, isactive,
				GroupId, phone, fax, mobile, position, department, company, location, time_zone_id,
				language_id, "", fileName, data, profileId);
		}

		public static int CreatePartnerUser(string login, string password, string first_name,
			string last_name, string email, bool isactive, int GroupId,
			string phone, string fax, string mobile, string position, string department,
			string company, string location, int time_zone_id, int language_id,
			string invite_text, string fileName, Stream data, int profileId)
		{
			//check user rights
			if (!CanCreatePartner())
				throw new AccessDeniedException();

			if (password == "")
				throw new PasswordRequiredException();

			int Activity = (int)UserActivity.Inactive;
			if (isactive)
			{
				if (!CanAddNewActiveUser(false))
					throw new MaxUsersCountException();
				Activity = (int)UserActivity.Active;
			}

			if (!Regex.IsMatch(login, @"^[\w-\.]+"))
				throw new WrongLoginException();

			// Check Email
			if (DBUser.GetUserByEmail(email, false) > 0)
				throw new EmailDuplicationException();
			if (DBUser.CheckEmail(email) > 0)
				throw new EmailDuplicationException();

			if (!SecureGroup.IsPartner(GroupId))
				throw new AccessDeniedException();

			int imGroupId = -1;
			using (IDataReader reader = DBGroup.GetGroup(GroupId))
			{
				if (reader.Read())
				{
					if (reader["IMGroupId"] != DBNull.Value)
						imGroupId = (int)reader["IMGroupId"];
				}
			}

			int[] dependentIMGroups = GetDependentIMGroups(imGroupId);

			string salt = PasswordUtil.CreateSalt(PasswordUtil.SaltSize);
			string hash = PasswordUtil.CreateHash(password, salt);

			int UserID = -1;
			int OriginalUserId = -1;
			try
			{
				using (DbTransaction tran = DbTransaction.Begin())
				{
					if (imGroupId > 0)
						OriginalUserId = DBUser.AddToMain(-1, login, password, first_name, last_name, email, salt, hash, isactive, imGroupId);

					UserID = DBUser.Create(login, null, password, first_name, last_name, email, salt, hash, Activity, false, Security.CurrentUser.UserID, invite_text);

					//update user profile
					DBUser.UpdateProfile(UserID, phone, fax, mobile, position, department, company, location);

					// Picture
					BindPhoto(UserID, false, fileName, data);
					//					string FileUrl = "";
					//					if (fileName != null && fileName != "")
					//					{
					//						FileUrl = DSFile.UploadFile(fileName, data);
					//						DBUser.UpdateUserPhoto(UserID, FileUrl);
					//					}

					// Create default user preferences
					DBUser.UpdatePreferences(UserID, true, true, true, false, 60, 8, 20, time_zone_id, language_id, 3);

					//Add user's group to DB
					DBUser.AddUserToGroup(UserID, GroupId);

					//Set IM Group and OriginalUserId
					DBUser.AssignIMGroup(UserID, imGroupId);
					if (OriginalUserId > 0)
						DBUser.AssignOriginalId(UserID, OriginalUserId);

					try
					{
						foreach (int IMGroupId in dependentIMGroups)
							IMManager.UpdateGroup(IMGroupId);
					}
					catch (Exception)
					{
					}

					if (profileId > 0)
					{
						EntityObject entity = BusinessManager.InitializeEntity(CustomizationProfileUserEntity.ClassName);
						entity[CustomizationProfileUserEntity.FieldPrincipalId] = (PrimaryKeyId)UserID;
						entity[CustomizationProfileUserEntity.FieldProfileId] = (PrimaryKeyId)profileId;
						BusinessManager.Create(entity);
					}

					AssignDefaultCalendar(UserID);

					// Send allerts
					Dictionary<string, string> additionalValues = new Dictionary<string, string>();
					additionalValues.Add(Variable.Password.ToString(), password);
					SystemEvents.AddSystemEvents(SystemEventTypes.User_Created_Partner, UserID, additionalValues);

					tran.Commit();
				}
			}
			catch (Exception exception)
			{
				if (exception is SqlException)
				{
					SqlException sqlException = exception as SqlException;
					if (sqlException.Number == 2627 || sqlException.Number == 50000)
						throw new LoginDuplicationException();
				}
				throw;
			}

			return UserID;
		}
		#endregion

		#region CreatePartnerUserFromWizard
		public static int CreatePartnerUserFromWizard(string login, string password,
			string first_name, string last_name, string email, int GroupId,
			string phone, string mobile, string position, string department,
			string company, string location, int language_id)
		{
			return CreatePartnerUser(login, password, first_name, last_name, email, true,
				GroupId, phone, "", mobile, position, department, company, location, DefaultTimeZoneId,
				language_id, "", null, null, -1);
		}
		#endregion

		#region UpdatePartnerUser
		public static void UpdatePartnerUser(int user_id, string password, string first_name,
			string last_name, string email, bool isactive, string phone, string fax,
			string mobile, string position, string department, string company,
			string location, string fileName, Stream data, int profileId)
		{
			//check user rights
			if (!CanUpdateUserInfo(user_id))
				throw new AccessDeniedException();

			int Activity = (int)UserActivity.Inactive;
			int IMGroupId = -1;
			bool WasInactive = true;
			string oldEmail = "";
			string login = "";

			using (IDataReader reader = DBUser.GetUserInfo(user_id))
			{
				reader.Read();
				WasInactive = !(bool)reader["IsActive"];
				if (reader["IMGroupId"] != DBNull.Value)
					IMGroupId = (int)reader["IMGroupId"];
				oldEmail = reader["Email"].ToString();
				login = reader["Login"].ToString();
			}
			if (isactive)
			{
				if (WasInactive && !CanAddNewActiveUser(false))
					throw new MaxUsersCountException();

				Activity = (int)UserActivity.Active;
			}

			if (IMGroupId <= 0)
			{
				int groupId = GetGroupForPartnerUser(user_id);
				using (IDataReader reader = SecureGroup.GetGroup(groupId))
				{
					reader.Read();
					IMGroupId = (int)reader["IMGroupId"];
				}
			}

			// Check Email
			if (oldEmail != email)
			{
				if (DBUser.GetUserByEmail(email, false) > 0)
					throw new EmailDuplicationException();

				if (DBUser.CheckEmail(email) > 0)
					throw new EmailDuplicationException();
			}

			string salt = PasswordUtil.CreateSalt(PasswordUtil.SaltSize);
			string hash = PasswordUtil.CreateHash(password, salt);

			int OriginalUserId = DBUser.GetOriginalId(user_id);
			//Begin transaction
			try
			{
				using (DbTransaction tran = DbTransaction.Begin())
				{
					if (CanUpdateSecureFields(user_id))
					{
						//update user info
						DBUser.UpdateUserInfo(user_id, password, first_name, last_name, email, salt, hash, Activity, false);

						//update user profile
						DBUser.UpdateProfile(user_id, phone, fax, mobile, position, department, company, location);

						OriginalUserId = DBUser.AddToMain(OriginalUserId, login, password, first_name, last_name, email, salt, hash, isactive, IMGroupId);
						DBUser.AssignOriginalId(user_id, OriginalUserId);
						DBUser.AssignIMGroup(user_id, IMGroupId);
					}
					else
					{
						if (CanUpdatePassword(user_id))
						{
							DBUser.UpdateUserInfo(user_id, password, first_name, last_name, email, salt, hash);
							if (OriginalUserId > 0)
								DBUser.AddToMain(OriginalUserId, "", password, first_name, last_name, email, salt, hash, isactive, -1);
						}
						else
						{
							DBUser.UpdateUserInfo(user_id, "", first_name, last_name, email, salt, hash);
							if (OriginalUserId > 0)
								DBUser.AddToMain(OriginalUserId, "", "", first_name, last_name, email, salt, hash, isactive, -1);
						}
					}

					DBUser.UpdateProfile(user_id, phone, fax, mobile, position, department, company, location);

					// update user photo
					BindPhoto(user_id, false, fileName, data);

					if (WasInactive && isactive)
						IncreaseStubsVersion(user_id);

					if (OriginalUserId > 0)
					{
						try
						{
							IMManager.UpdateUser(user_id);
						}
						catch (Exception)
						{
						}
					}

					if (profileId > 0)
					{
						EntityObject entity = BusinessManager.InitializeEntity(CustomizationProfileUserEntity.ClassName);
						entity[CustomizationProfileUserEntity.FieldPrincipalId] = (PrimaryKeyId)user_id;
						entity[CustomizationProfileUserEntity.FieldProfileId] = (PrimaryKeyId)profileId;
						BusinessManager.Create(entity);
					}
					else
					{
						EntityObject[] list = BusinessManager.List(CustomizationProfileUserEntity.ClassName, new FilterElement[] { FilterElement.EqualElement(CustomizationProfileUserEntity.FieldPrincipalId, user_id) });
						if (list.Length > 0)
							BusinessManager.Delete(list[0]);
					}

					// Send allerts
					//SendAlert(AlertEventType.User_Updated, user_id);

					tran.Commit();
				}
			}
			catch (Exception exception)
			{
				if (exception is SqlException)
				{
					SqlException sqlException = exception as SqlException;
					if (sqlException.Number == 2627)
						throw new EmailDuplicationException();
				}
				throw;
			}
		}

		#endregion

		#region GetGroupForPartnerUser
		public static int GetGroupForPartnerUser(int UserId)
		{
			return DBGroup.GetGroupForPartnerUser(UserId);
		}
		#endregion

		#region CheckUserGroup
		public static bool CheckUserGroup(int UserId, int GroupId)
		{
			bool retval = false;
			if (DBUser.CheckUserGroup(UserId, GroupId) > 0)
				retval = true;

			return retval;
		}
		#endregion

		#region DeleteUserFromGroup
		public static void DeleteUserFromGroup(int UserId, int GroupId)
		{
			if (!CanUpdateSecureFields(UserId))
				throw new AccessDeniedException();

			DBUser.DeleteUserFromGroup(UserId, GroupId);
		}
		#endregion

		#region GetTimeZoneByBias
		public static int GetTimeZoneByBias(int Bias)
		{
			int retval = DBCommon.GetTimeZoneByBias(Bias);
			if (retval <= 0)
				retval = DefaultTimeZoneId;
			return retval;
		}
		#endregion

		#region GetLocalDate
		public static DateTime GetLocalDate(DateTime UTCDate)
		{
			return GetLocalDate(Security.CurrentUser.TimeZoneId, UTCDate);
		}

		public static DateTime GetLocalDate(int TimeZoneId, DateTime UTCDate)
		{
			return DBCommon.GetLocalDate(TimeZoneId, UTCDate);
		}
		#endregion

		#region GetUTCDate
		public static DateTime GetUTCDate(DateTime LocalDate)
		{
			return GetUTCDate(Security.CurrentUser.TimeZoneId, LocalDate);
		}

		public static DateTime GetUTCDate(int TimeZoneId, DateTime LocalDate)
		{
			return DBCommon.GetUTCDate(TimeZoneId, LocalDate);
		}
		#endregion

		#region GetTimeZone
		/// <summary>
		///	TimeZoneId, Bias, StandardBias, DaylightBias, DaylightMonth, DaylightDayOfWeek, 
		///	DaylightWeek, DaylightHour, StandardMonth, StandardDayOfWeek, StandardWeek, 
		///	StandardHour, DisplayName, LanguageId
		/// </summary>
		public static IDataReader GetTimeZone(int TimeZoneId)
		{
			return GetTimeZone(TimeZoneId, Security.CurrentUser.TimeZoneId);
		}

		public static IDataReader GetTimeZone(int TimeZoneId, int LanguageId)
		{
			return DBCommon.GetTimeZone(TimeZoneId, LanguageId);
		}
		#endregion

		#region GetCurrentBias
		public static int GetCurrentBias()
		{
			return GetCurrentBias(Security.CurrentUser.TimeZoneId);
		}

		public static int GetCurrentBias(int TimeZoneId)
		{
			DateTime UTCDate = DateTime.UtcNow;
			DateTime LocalDate = DBCommon.GetLocalDate(TimeZoneId, UTCDate);
			int retval = UTCDate.Hour * 60 + UTCDate.Minute - LocalDate.Hour * 60 - LocalDate.Minute;
			if (UTCDate.Date > LocalDate.Date)
				retval = retval + 24 * 60;
			else if (UTCDate.Date < LocalDate.Date)
				retval = retval - 24 * 60;
			return retval;
		}
		#endregion

		#region GetTimeZoneBias
		public static int GetTimeZoneBias(int TimeZoneId)
		{
			return DBCommon.GetTimeZoneBias(TimeZoneId);
		}
		#endregion

		#region public static int GetAllowedUsersCount(bool isExternal)
		public static int GetAllowedUsersCount(bool isExternal)
		{
			int result;

			int maxLicense = isExternal ? License.ExternalUsersCount : License.ActiveUsersCount;
			int maxCompany = isExternal ? PortalConfig.CompanyMaxExternalUsers : PortalConfig.CompanyMaxUsers;

			if (maxLicense < 0)
				result = maxCompany;
			else
			{
				if (maxCompany < 0)
					result = maxLicense;
				else
					result = Math.Min(maxLicense, maxCompany);
			}

			return result;
		}
		#endregion

		#region internal static int GetRemainingActiveUsers(bool isExternal)
		internal static int GetRemainingActiveUsers(bool isExternal)
		{
			int remaining;

			int allowed = GetAllowedUsersCount(isExternal);
			int active = DBUser.GetActiveUsersCount(isExternal);

			if (allowed < 0)
				remaining = int.MaxValue;
			else
				remaining = allowed - active;

			return remaining;
		}
		#endregion

		#region GetListPendingUsers
		/// <summary>
		/// Reader returns fields:
		///  PrincipalId, Login, FirstName, LastName, Email, IMGroupId, OriginalId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListPendingUsers()
		{
			return DBUser.GetListPendingUsers();
		}
		#endregion

		#region AddEmail
		public static void AddEmail(string Email)
		{
			// Check Email
			if (DBUser.GetUserByEmail(Email, false) > 0)
				throw new EmailDuplicationException();
			if (DBUser.CheckEmail(Email) > 0)
				throw new EmailDuplicationException();

			DBUser.AddEmail(Security.CurrentUser.UserID, Email);
		}

		public static void AddEmail(int UserId, string Email)
		{
			if (!CanUpdateUserInfo(UserId))
				throw new AccessDeniedException();

			// Check Email
			if (DBUser.GetUserByEmail(Email, false) > 0)
				throw new EmailDuplicationException();
			if (DBUser.CheckEmail(Email) > 0)
				throw new EmailDuplicationException();

			DBUser.AddEmail(UserId, Email);
		}
		#endregion

		#region DeleteEmail
		public static void DeleteEmail(int EmailId)
		{
			int UserId = -1;
			using (IDataReader reader = DBUser.GetEmail(EmailId))
			{
				if (reader.Read())
					UserId = (int)reader["UserId"];
			}

			if (!CanUpdateUserInfo(UserId))
				throw new AccessDeniedException();

			DBUser.DeleteEmail(EmailId);
		}
		#endregion

		#region GetListEmails
		/// <summary>
		/// Reader returns fields:
		///  EmailId, Email
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListEmails(int UserId)
		{
			return DBUser.GetListEmails(UserId);
		}
		#endregion

		#region GetListEmailsDataTable
		/// <summary>
		/// Reader returns fields:
		///  EmailId, Email
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListEmailsDataTable(int UserId)
		{
			return DBUser.GetListEmailsDataTable(UserId);
		}
		#endregion

		#region EmailSetPrimary
		public static void EmailSetPrimary(int EmailId)
		{
			int UserId = -1;
			string Email = "";
			using (IDataReader reader = DBUser.GetEmail(EmailId))
			{
				if (reader.Read())
				{
					UserId = (int)reader["UserId"];
					Email = reader["Email"].ToString();
				}
			}

			if (!CanUpdateUserInfo(UserId))
				throw new AccessDeniedException();

			try
			{
				DBUser.EmailSetPrimary(UserId, Email);
			}
			catch (Exception exception)
			{
				if (exception is SqlException)
				{
					SqlException sqlException = exception as SqlException;
					if (sqlException.Number == 2627)
						throw new EmailDuplicationException();
				}

				throw exception;
			}
		}
		#endregion

		#region GetEmail
		/// <summary>
		/// Reader returns fields:
		///  EmailId, UserId, Email
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetEmail(int EmailId)
		{
			return DBUser.GetEmail(EmailId);
		}
		#endregion

		#region UpdateEmail
		public static void UpdateEmail(int EmailId, string Email)
		{
			int UserId = -1;
			string oldEmail = "";
			using (IDataReader reader = DBUser.GetEmail(EmailId))
			{
				if (reader.Read())
				{
					UserId = (int)reader["UserId"];
					oldEmail = reader["Email"].ToString();
				}
			}

			if (!CanUpdateUserInfo(UserId))
				throw new AccessDeniedException();

			// Check Email
			if (oldEmail != Email)
			{
				if (DBUser.GetUserByEmail(Email, false) > 0)
					throw new EmailDuplicationException();

				if (DBUser.CheckEmail(Email) > 0)
					throw new EmailDuplicationException();
			}

			DBUser.UpdateEmail(EmailId, Email);
		}
		#endregion

		#region CreateMultiple
		public static void CreateMultiple(ArrayList users, string password, int languageId)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				foreach (UserInfo ui in users)
				{
					int userId = Create(ui.Login, password, ui.FirstName, ui.LastName,
						ui.Email, true, ui.Groups, ui.ImGroupId, ui.Phone,
						ui.Fax, ui.Mobile, ui.JobTitle, ui.Department,
						ui.Company, ui.Location, DefaultTimeZoneId,
						languageId, null, null, -1);
					DBUser.UpdateWindowsLogin(userId, ui.WindowsLogin);
				}
				tran.Commit();
			}
		}
		#endregion

		#region GetListUsersGroupedByRole
		/// <summary>
		///  RoleId, RoleName, UserId, UserName, OpenTasks, CompletedTasks, Issues, IsHeader
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListUsersGroupedByRole()
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) &&
				!Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager))
				throw new AccessDeniedException();

			return DBUser.GetListUsersGroupedByRole();
		}
		#endregion

		#region GetListActions
		/// <summary>
		///  ActionType, Counter
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListActions()
		{
			return DBUser.GetListActions(Security.CurrentUser.UserID);
		}
		#endregion

		#region UpdateMenuInAlerts
		public static void UpdateMenuInAlerts(int UserId, bool bShow)
		{
			DBUser.UpdateMenuInAlerts(UserId, bShow);
		}
		#endregion

		#region GetMenuInAlerts
		public static bool GetMenuInAlerts(int UserId)
		{
			return DBUser.GetMenuInAlerts(UserId);
		}
		#endregion

		#region GetUserName
		public static string GetUserName(int UserId)
		{
			string userName = "";
			using (IDataReader reader = DBUser.GetUserInfo(UserId))
			{
				if (reader.Read())
					userName = reader["LastName"] + " " + reader["FirstName"];
			}
			return userName;
		}
		#endregion

		#region AddFavorites
		public static void AddFavorites(int UserId)
		{
			DBCommon.AddFavorites((int)USER_TYPE, UserId, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListSecureGroupAll
		/// <summary>
		/// Reader returns fields:
		///		GroupId
		/// </summary>
		public static ArrayList GetListSecureGroupAll(int UserId)
		{
			ArrayList secure_groups = new ArrayList();
			using (IDataReader reader = DBUser.GetListSecureGroupAll(UserId))
			{
				while (reader.Read())
					secure_groups.Add((int)reader["GroupId"]);
			}
			return secure_groups;
		}

		/// <summary>
		/// Gets the list secure group all array.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <returns></returns>
		public static int[] GetListSecureGroupAllArray(int UserId)
		{
			return (int[])GetListSecureGroupAll(UserId).ToArray(typeof(int));
		}
		#endregion

		#region IsGroup
		public static bool IsGroup(int PrincipalId)
		{
			return DBPrincipal.IsGroup(PrincipalId);
		}
		#endregion

		#region GetUserActivity
		public static UserActivity GetUserActivity(int UserId)
		{
			UserActivity ua = UserActivity.Inactive;
			using (IDataReader reader = GetUserInfo(UserId, false))
			{
				if (reader != null && reader.Read())
				{
					ua = (UserActivity)((byte)reader["Activity"]);
				}
			}
			return ua;
		}
		#endregion

		#region BindPhoto
		private static void BindPhoto(int user_id, bool bDeletePic, string fileName, Stream data)
		{
			// For Photo
			string ContainerName = "FileLibrary";
			string ContainerKey = "SystemPicture";

			BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
			FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");

			int iFileId = 0;
			using (IDataReader pReader = DBUser.GetUserProfile(user_id))
			{
				if (pReader.Read())
				{
					string PictureUrl = "";
					if (pReader["PictureUrl"] != DBNull.Value)
					{
						PictureUrl = pReader["PictureUrl"].ToString();
					}
					if (PictureUrl != "")
						iFileId = DSFile.GetFileIDFromURL(PictureUrl);
				}
			}
			//delete user photo
			if (bDeletePic)
			{
				if (iFileId > 0)
					fs.DeleteFile(iFileId);
				iFileId = 0;
				DBUser.UpdateUserPhoto(user_id, "");
			}
			// update user photo
			if (fileName != null && fileName != "")
			{
				if (iFileId > 0)
					fs.DeleteFile(iFileId);
				iFileId = 0;
				string oldFileName = fileName;
				int i = 0;
				while (fs.FileExist(fileName, fs.Root.Id))
					fileName = oldFileName + (i++).ToString();
				Mediachase.IBN.Business.ControlSystem.FileInfo fi = fs.SaveFile(fs.Root.Id, fileName, data);
				string FileUrl = String.Format("http://localhost?{0}{1}", "fileid=", fi.Id);
				DBUser.UpdateUserPhoto(user_id, FileUrl);
			}
		}
		#endregion

		#region UpdateBatchLastSent()
		internal static void UpdateBatchLastSent(int userId, DateTime dt)
		{
			DBUser.UpdateBatchLastSent(userId, dt);
		}
		#endregion
		#region UpdateBatchNextSend()
		internal static void UpdateBatchNextSend(int userId, DateTime dt)
		{
			DBUser.UpdateBatchNextSend(userId, dt);
		}
		#endregion

		#region GetListAll
		/// <summary>
		/// Reader returns fields:
		///		UserId, FirstName, LastName, Email, Login, IsExternal, Activity, IsPending
		/// </summary>
		public static IDataReader GetListAll()
		{
			return DBUser.GetListAll();
		}
		#endregion

		#region GetListActive
		/// <summary>
		/// Reader returns fields:
		///	PrincipalId, Login, FirstName, LastName, Email, IMGroupId, OriginalId, 
		/// DisplayName, Department
		/// </summary>
		public static IDataReader GetListActive()
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
				return DBUser.GetListActive("");
			else
				return DBUser.GetListActiveForPartner(Security.CurrentUser.UserID);
		}

		public static DataTable GetListActiveDataTable()
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
				return DBUser.GetListActiveDataTable("");
			else
				return DBUser.GetListActiveForPartnerDataTable(Security.CurrentUser.UserID);
		}

		public static DataTable GetListActiveDataTable(string sKeyword)
		{
			return DBUser.GetListActiveDataTable(sKeyword);
		}
		#endregion

		#region IsLastAdmin
		private static bool IsLastAdmin(int UserId)
		{
			bool retVal = false;
			ArrayList _users = new ArrayList();
			using (IDataReader reader = SecureGroup.GetListActiveUsersInGroup((int)InternalSecureGroups.Administrator))
			{
				while (reader.Read())
					_users.Add((int)reader["UserId"]);
			}
			if (_users.Count == 1 && (int)_users[0] == UserId)
				retVal = true;
			return retVal;
		}
		#endregion


		#region UpdateWindowsLogin()
		public static void UpdateWindowsLogin(int userId, string windowsLogin)
		{
			if (!CanUpdateUserInfo())
				throw new AccessDeniedException();

			DBUser.UpdateWindowsLogin(userId, windowsLogin);
		}
		#endregion

		#region GetListActiveManagers
		/// <summary>
		/// Reader returns fields:
		/// UserId, Login, FirstName, LastName, Email, IMGroupId, OriginalId, CreatedBy, Activity, IsExternal, DisplayName
		/// </summary>
		public static IDataReader GetListActiveManagers()
		{
			return DBUser.GetListActiveManagers();
		}
		#endregion

		#region GetLdap
		public static IDataReader GetLdap()
		{
			return DBUser.GetLdap();
		}
		#endregion

		#region UpdateActivity()
		public static void UpdateActivity(int UserId, bool isActive)
		{
			int OriginalUserId = DBUser.GetOriginalId(UserId);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				if (!isActive)
					SystemEvents.AddSystemEvents(SystemEventTypes.User_Deactivated, UserId);

				if (!isActive && OriginalUserId > 0)
				{
					try
					{
						IMManager.LogOff(OriginalUserId);
					}
					catch (Exception)
					{
					}
				}

				if (OriginalUserId > 0)
					DBUser.UpdateActivityInMain(OriginalUserId, isActive);

				DBUser.UpdateActivity(UserId, isActive);

				if (isActive)
					SystemEvents.AddSystemEvents(SystemEventTypes.User_Activated, UserId);

				tran.Commit();
			}
		}
		#endregion

		#region Notify(...)
		public static void Notify(int userId)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				SystemEvents.AddSystemEvents(SystemEventTypes.User_Created, userId);
				tran.Commit();
			}
		}
		#endregion


		#region private

		private static int[] GetDependentIMGroups(int imGroupId)
		{
			List<int> list = new List<int>();

			foreach (DataRow row in DBIMGroup.GetListIMGroupsByIMGroups(0, imGroupId).Rows)
			{
				list.Add((int)row["imgroup_id"]);
			}

			return list.ToArray();
		}

		#endregion

		#region GetListActiveUsersForPartnerUser
		/// <summary>
		/// Gets the list active users for partner user.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <returns>UserId, Login, FirstName, LastName, Email, IsExternal, IsPending</returns>
		public static IDataReader GetListActiveUsersForPartnerUser(int UserId)
		{
			return DBUser.GetListActiveUsersForPartnerUser(UserId);
		}
		#endregion

		#region AssignDefaultCalendar
		/// <summary>
		/// Assigns the default calendar to user.
		/// </summary>
		/// <param name="userId">The user id.</param>
		private static void AssignDefaultCalendar(int userId)
		{
			int calendarId = PortalConfig.DefaultCalendar;

			// Check thae calendar exists
			bool exists = false;
			using (IDataReader reader = DBCalendar.GetCalendar(calendarId))
			{
				if (reader.Read())
					exists = true;
			}

			if (exists)
				Calendar.AddUserCalendar(calendarId, userId);
		} 
		#endregion

		#region GetRssKeyByUserId
		/// <summary>
		/// Gets the RSS key by user id.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static Guid GetRssKeyByUserId(int userId)
		{
			Guid retval = Guid.Empty;
			using (IDataReader reader = DBUser.GetRssKeyByUserId(userId))
			{
				if (reader.Read())
				{
					retval = (Guid)reader["RssKey"];
				}
			}

			return retval;
		}
		#endregion	
	}
}

