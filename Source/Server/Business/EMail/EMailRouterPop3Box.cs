using System;
using System.Collections;
using System.Net;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.EMail;
using Mediachase.Net.Mail;

namespace Mediachase.IBN.Business.EMail
{
    public enum Pop3SecureConnectionType
    {
        None = 0,
        Tls = 1,
        Ssl = 2
    }

	/// <summary>
	/// Summary description for EMailRouterBox.
	/// </summary>
	public class EMailRouterPop3Box
	{
		private EMailRouterPop3BoxRow _srcRow = null;

		private EMailRouterPop3BoxSettings _settings = null;
		private EMailRouterPop3BoxActivity _activity = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="EMailRouterPop3Box"/> class.
		/// </summary>
		/// <param name="row">The row.</param>
		private EMailRouterPop3Box(EMailRouterPop3BoxRow row)
		{
			_srcRow = row;
		}

        /// <summary>
        /// Determines whether this instance can modify.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can modify; otherwise, <c>false</c>.
        /// </returns>
		public static bool CanModify()
		{
			return Security.IsUserInGroup(InternalSecureGroups.Administrator);
		}

        /// <summary>
        /// Loads the specified E mail router POP3 box id.
        /// </summary>
        /// <param name="EMailRouterPop3BoxId">The E mail router POP3 box id.</param>
        /// <returns></returns>
		public static EMailRouterPop3Box Load(int EMailRouterPop3BoxId)
		{
			return new EMailRouterPop3Box(new EMailRouterPop3BoxRow(EMailRouterPop3BoxId));
		}

        /// <summary>
        /// Creates the external.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="EMailAddress">The E mail address.</param>
        /// <param name="Server">The server.</param>
        /// <param name="Port">The port.</param>
        /// <param name="Login">The login.</param>
        /// <param name="Password">The password.</param>
        /// <returns></returns>
        public static int CreateExternal(string Name, string EMailAddress, string Server, int Port, string Login, string Password)
        {
            return CreateExternal(Name, EMailAddress, Server, Port, Login, Password, Pop3SecureConnectionType.None);
        }

		/// <summary>
		/// Creates the external.
		/// </summary>
		/// <param name="Server">The server.</param>
		/// <param name="Port">The port.</param>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <returns></returns>
        public static int CreateExternal(string Name, string EMailAddress, string Server, int Port, string Login, string Password, Pop3SecureConnectionType SecureConnectionType)
		{
			if(!CanModify())
				throw new AccessDeniedException();

			EMailRouterPop3BoxRow newRow = new EMailRouterPop3BoxRow();

			newRow.IsInternal = false;
			newRow.Name = Name;
            newRow.InternalEMailAddress = EMailAddress;
			newRow.Server = Server;
			newRow.Port = Port;
			newRow.Login = Login;
			newRow.Pass = Password;
            newRow.UseSecureConnection = (int)SecureConnectionType;

			newRow.Update();

			return newRow.PrimaryKeyId;
		}

        /// <summary>
        /// Creates the internal.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="EMailAddress">The E mail address.</param>
        /// <param name="Server">The server.</param>
        /// <param name="Port">The port.</param>
        /// <param name="Login">The login.</param>
        /// <param name="Password">The password.</param>
        /// <returns></returns>
        public static int CreateInternal(string Name, string EMailAddress, string Server, int Port, string Login, string Password)
        {
            return CreateInternal(Name, EMailAddress, Server, Port, Login, Password, Pop3SecureConnectionType.None);
        }


		/// <summary>
		/// Creates the internal.
		/// </summary>
		/// <param name="InternalEMailAddress">The internal E mail address.</param>
		/// <param name="Server">The server.</param>
		/// <param name="Port">The port.</param>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
        public static int CreateInternal(string Name, string EMailAddress, string Server, int Port, string Login, string Password, Pop3SecureConnectionType SecureConnectionType)
		{
			if(!CanModify())
				throw new AccessDeniedException();

			EMailRouterPop3BoxRow newRow = new EMailRouterPop3BoxRow();

			newRow.IsInternal = true;
			newRow.Name = Name;
			newRow.InternalEMailAddress = EMailAddress;
			newRow.Server = Server;
			newRow.Port = Port;
			newRow.Login = Login;
			newRow.Pass = Password;
            newRow.UseSecureConnection = (int)SecureConnectionType;

			newRow.Update();

			return newRow.PrimaryKeyId;
		}

		/// <summary>
		/// Lists the external.
		/// </summary>
		/// <returns></returns>
		public static EMailRouterPop3Box[] ListExternal()
		{
			ArrayList retVal = new ArrayList();

			foreach(EMailRouterPop3BoxRow row in EMailRouterPop3BoxRow.List(false))
			{
				retVal.Add(new EMailRouterPop3Box(row));
			}

			return (EMailRouterPop3Box[])retVal.ToArray(typeof(EMailRouterPop3Box));
		}

		/// <summary>
		/// Lists the internal.
		/// </summary>
		/// <returns></returns>
		public static EMailRouterPop3Box ListInternal()
		{
			EMailRouterPop3BoxRow[] rows = EMailRouterPop3BoxRow.List(true);

			if(rows.Length==0)
				return null;

			return new EMailRouterPop3Box(rows[0]);
		}

		/// <summary>
		/// Deletes the specified E mail router POP3 box id.
		/// </summary>
		/// <param name="EMailRouterPop3BoxId">The E mail router POP3 box id.</param>
		public static void Delete(int EMailRouterPop3BoxId)
		{
			if(!CanModify())
				throw new AccessDeniedException();

			EMailRouterPop3BoxRow.Delete(EMailRouterPop3BoxId);
		}

		/// <summary>
		/// Updates the specified POP3 box.
		/// </summary>
		/// <param name="pop3Box">The POP3 box.</param>
		public static void Update(EMailRouterPop3Box pop3Box)
		{
			if(!CanModify())
				throw new AccessDeniedException();

			pop3Box._srcRow.Update();
		}

		/// <summary>
		/// Determines whether this instance can activate the specified E mail router POP3 box I.
		/// </summary>
		/// <param name="EMailRouterPop3BoxI">The E mail router POP3 box I.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can activate the specified E mail router POP3 box I; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanActivate(int EMailRouterPop3BoxI)
		{
			EMailRouterPop3Box box = EMailRouterPop3Box.Load(EMailRouterPop3BoxI);
			return box.IsInternal || box.Settings.DefaultEMailIncidentMappingBlock.DefaultCreator!=-1;
		}

		public static void Activate(int EMailRouterPop3BoxId, bool IsActive)
		{
			if(!CanModify())
				throw new AccessDeniedException();

			if(!CanActivate(EMailRouterPop3BoxId))
				throw new Exception("Can not activate email box with empty mapping block.");

			EMailRouterPop3BoxActivityRow[] rows = EMailRouterPop3BoxActivityRow.List(EMailRouterPop3BoxId);

			EMailRouterPop3BoxActivityRow activityRow = null;

			if(rows.Length>0)
				activityRow = rows[0];
			else
			{
				activityRow = new EMailRouterPop3BoxActivityRow();
				activityRow.EMailRouterPop3BoxId = EMailRouterPop3BoxId;
			}

			activityRow.IsActive = IsActive;
			activityRow.Update();
		}

		public static void UpdateStatistic(int EMailRouterPop3BoxId, bool IsSuccesfull, string ErrorText, int ProcessedMessageCount)
		{
			EMailRouterPop3BoxActivityRow[] rows = EMailRouterPop3BoxActivityRow.List(EMailRouterPop3BoxId);

			EMailRouterPop3BoxActivityRow activityRow = null;

			if(rows.Length>0)
				activityRow = rows[0];
			else
			{
				activityRow = new EMailRouterPop3BoxActivityRow();
				activityRow.EMailRouterPop3BoxId = EMailRouterPop3BoxId;
			}

			activityRow.LastRequest = DateTime.UtcNow;
			if(IsSuccesfull)
				activityRow.LastSuccessfulRequest = activityRow.LastRequest;
			activityRow.ErrorText = ErrorText;
			activityRow.TotalMessageCount += ProcessedMessageCount;
			
			activityRow.Update();
		}

		public static bool CanDeleteUser(int UserId)
		{
			foreach(EMailRouterPop3Box box in EMailRouterPop3Box.ListExternal())
			{
				if(box.Settings.DefaultEMailIncidentMappingBlock.DefaultCreator==UserId)
					return false;
			}
			return true;
		}

		public static void DeleteUser(int UserId)
		{
			// Nothing do
		}

		public static void ReplaseUser(int OldUserId, int NewUserId)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				foreach(EMailRouterPop3Box box in EMailRouterPop3Box.ListExternal())
				{
					EMailRouterPop3BoxSettings settings = box.Settings;
					if(settings.DefaultEMailIncidentMappingBlock.DefaultCreator==OldUserId)
					{
						settings.DefaultEMailIncidentMappingBlock.DefaultCreator = NewUserId;
						EMailRouterPop3BoxSettings.Save(settings);
					}
				}

				tran.Commit();
			}
		}

		#region Public Properties
		
		/// <summary>
		/// Gets the E mail router POP3 box id.
		/// </summary>
		/// <value>The E mail router POP3 box id.</value>
		public virtual int EMailRouterPop3BoxId
		{
			get
			{
				return _srcRow.EMailRouterPop3BoxId;
			}
		}
		
		public virtual string Name
	    
		{
			get
			{
				return _srcRow.Name;
			}
			
			set
			{
				_srcRow.Name = value;
			}	
			
		}

		/// <summary>
		/// Gets or sets the server.
		/// </summary>
		/// <value>The server.</value>
		public virtual string Server
	    
		{
			get
			{
				return _srcRow.Server;
			}
			
			set
			{
				_srcRow.Server = value;
			}	
			
		}
		
		/// <summary>
		/// Gets or sets the port.
		/// </summary>
		/// <value>The port.</value>
		public virtual int Port
	    
		{
			get
			{
				return _srcRow.Port;
			}
			set
			{
				_srcRow.Port = value;
			}	
			
		}
		
		/// <summary>
		/// Gets or sets the login.
		/// </summary>
		/// <value>The login.</value>
		public virtual string Login
	    
		{
			get
			{
				return _srcRow.Login;
			}
			
			set
			{
				_srcRow.Login = value;
			}	
			
		}

		/// <summary>
		/// Gets or sets the pass.
		/// </summary>
		/// <value>The pass.</value>
		public virtual string Pass
		{
			get
			{
				return _srcRow.Pass;
			}
			
			set
			{
				_srcRow.Pass = value;
			}	
			
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this instance is internal.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is internal; otherwise, <c>false</c>.
		/// </value>
		public virtual bool IsInternal
		{
			get
			{
				return _srcRow.IsInternal;
			}
//			set
//			{
//				_srcRow.IsInternal = value;
//			}	
		}
		
		/// <summary>
		/// Gets or sets the internal E mail address.
		/// </summary>
		/// <value>The internal E mail address.</value>
		public virtual string EMailAddress
		{
			get
			{
				return _srcRow.InternalEMailAddress;
			}
			
			set
			{
				_srcRow.InternalEMailAddress = value;
			}	
			
		}

		/// <summary>
		/// Gets a value indicating whether [E mail router POP3 box activity].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [E mail router POP3 box activity]; otherwise, <c>false</c>.
		/// </value>
		public EMailRouterPop3BoxActivity Activity
		{
			get 
			{
				if(_activity==null)
				{
					_activity =  EMailRouterPop3BoxActivity.Load(this.EMailRouterPop3BoxId);
				}
				return _activity;
			}
		}

        /// <summary>
        /// Determines whether this instance has problem.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance has problem; otherwise, <c>false</c>.
        /// </returns>
        public bool HasProblem()
        {
            return this.Activity.IsActive &&
                (this.Activity.LastRequest != this.Activity.LastSuccessfulRequest) ||
                (this.Activity.LastRequest.AddDays(1) < DateTime.Now);
        }
		
		/// <summary>
		/// Gets the settings.
		/// </summary>
		/// <value>The settings.</value>
		public EMailRouterPop3BoxSettings Settings
		{
			get 
			{
				if(_settings==null)
				{
					_settings = EMailRouterPop3BoxSettings.Load(this.EMailRouterPop3BoxId);
				}
				return _settings;
			}
		}

        public virtual Pop3SecureConnectionType SecureConnectionType
        {
            get
            {
                return (Pop3SecureConnectionType)_srcRow.UseSecureConnection;
            }

            set
            {
                _srcRow.UseSecureConnection = (int)value;
            }

        }
		#endregion

        #region CheckSettings
        public static Pop3SettingsResult CheckSettings(string Server, int Port, string User, string Password, Pop3SecureConnectionType SecureConnectionType)
        {
            string strEMailHost = Server;
            int iEMailPort = Port;
            string strEMailUser = User;
            string strEMailPassword = Password;

            Pop3SettingsResult retVal = Pop3SettingsResult.None;

            try
            {
                //IPHostEntry hostInfo = Dns.GetHostEntry(strEMailHost);
				IPAddress[] addresses = Dns.GetHostAddresses(strEMailHost);

				if (addresses.Length > 0)
                {
					IPEndPoint pop3ServerEndPoint = new IPEndPoint(addresses[0], iEMailPort);

                    Pop3Connection pop3Connection = new Pop3Connection();

                    if (SecureConnectionType == Pop3SecureConnectionType.Ssl)
                    {
                        pop3Connection.OpenSsl(pop3ServerEndPoint, Server);
                    }
                    else
                    {
                        pop3Connection.Open(pop3ServerEndPoint);
                    }

                    if (SecureConnectionType == Pop3SecureConnectionType.Tls)
                    {
                        pop3Connection.Stls(Server);
                    }

                    retVal |= Pop3SettingsResult.ServerName;

                    try
                    {
                        pop3Connection.User(strEMailUser);
                        retVal |= Pop3SettingsResult.Pop3User;

                        pop3Connection.Pass(strEMailPassword);
                        retVal |= Pop3SettingsResult.Pop3Password;

                        Pop3Stat stat = pop3Connection.Stat();
                    }
                    finally
                    {
                        pop3Connection.Quit();
                    }
                }
            }
            catch (Exception ex)
            {
                string strErrMsg = ex.Message;
            }

            return retVal;
        } 
        #endregion
	}
}
