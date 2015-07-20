using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.IBN.Database.EMail;
using Mediachase.Net.Mail;
using Mediachase.Ibn.Data;
using System.Diagnostics;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Represents Smtp box.
	/// </summary>
	public class SmtpBox
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the source row.
		/// </summary>
		/// <value>The source row.</value>
		private SmtpBoxRow SourceRow { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="SmtpBox"/> class.
		/// </summary>
		/// <param name="source">The source.</param>
		private SmtpBox(SmtpBoxRow source)
		{
			this.SourceRow = source;
		}
		#endregion

		#region Properties
		#region Public Properties

		/// <summary>
		/// Gets or sets the primary key id.
		/// </summary>
		/// <value>The primary key id.</value>
		public virtual Nullable<int> PrimaryKeyId
		{
			get { return this.SourceRow.PrimaryKeyId; }
			set { this.SourceRow.PrimaryKeyId = value; }
		}

		/// <summary>
		/// Gets the SMTP box id.
		/// </summary>
		/// <value>The SMTP box id.</value>
		public virtual int SmtpBoxId
		{
			get
			{

				return (int)this.SourceRow.SmtpBoxId;

			}

		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public virtual string Name
		{
			get
			{

				return this.SourceRow.Name;

			}

			set
			{
				this.SourceRow.Name = value;
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

				return this.SourceRow.Server;

			}

			set
			{
				// OZ Remove White Space [2009-04-28]
				this.SourceRow.Server = value.Trim();
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

				return this.SourceRow.Port;

			}

			set
			{
				this.SourceRow.Port = value;
			}

		}

		/// <summary>
		/// Gets or sets the secure connection.
		/// </summary>
		/// <value>The secure connection.</value>
		public virtual SecureConnectionType SecureConnection
		{
			get
			{

				return (SecureConnectionType)this.SourceRow.SecureConnection;

			}

			set
			{
				this.SourceRow.SecureConnection = (int)value;
				
			}

		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SmtpBox"/> is authenticate.
		/// </summary>
		/// <value><c>true</c> if authenticate; otherwise, <c>false</c>.</value>
		public virtual bool Authenticate
		{
			get
			{

				return this.SourceRow.Authenticate;

			}

			set
			{
				this.SourceRow.Authenticate = value;
			}

		}

		/// <summary>
		/// Gets or sets the user.
		/// </summary>
		/// <value>The user.</value>
		public virtual string User
		{
			get
			{

				return this.SourceRow.User;

			}

			set
			{
				if (value != null)
					this.SourceRow.User = value.Trim();
				else
					this.SourceRow.User = null;
			}

		}

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		public virtual string Password
		{
			get
			{

				return this.SourceRow.Password;

			}

			set
			{
				this.SourceRow.Password = value;
				
			}

		}

		/// <summary>
		/// Gets a value indicating whether this instance is default.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is default; otherwise, <c>false</c>.
		/// </value>
		public virtual bool IsDefault
		{
			get
			{

				return this.SourceRow.IsDefault;

			}
		}

		/// <summary>
		/// Gets or sets the check uid.
		/// </summary>
		/// <value>The check uid.</value>
		public virtual Guid CheckUid
		{
			get
			{

				return this.SourceRow.CheckUid;

			}
			set
			{
				this.SourceRow.CheckUid = value;
			}

		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="SmtpBox"/> is checked.
		/// </summary>
		/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
		public virtual bool Checked
		{
			get
			{

				return this.SourceRow.Checked;

			}
		}

		#endregion
		#endregion

		#region Methods
		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <returns></returns>
		public static SmtpBox Initialize()
		{
			return new SmtpBox(new SmtpBoxRow());
		}

		/// <summary>
		/// Creates the specified SMPT box.
		/// </summary>
		/// <param name="smptBox">The SMPT box.</param>
		/// <returns></returns>
		public static int Create(SmtpBox smptBox)
		{
			// [IN SP] Check that exists element with IsDefault = 1 

			// Reset Check params
			smptBox.SourceRow.Checked = false;
			smptBox.SourceRow.CheckUid = Guid.NewGuid();

			smptBox.SourceRow.Update();

			return smptBox.PrimaryKeyId.Value;
		}

		/// <summary>
		/// Updates the specified SMPT box.
		/// </summary>
		/// <param name="smptBox">The SMPT box.</param>
		public static void Update(SmtpBox smptBox)
		{
			// [IN SP] Check that exists element with IsDefault = 1 

			// Reset Check params
			smptBox.SourceRow.Checked = false;
			smptBox.SourceRow.CheckUid = Guid.NewGuid();

			smptBox.SourceRow.Update();
		}


		/// <summary>
		/// Loads the specified SMTP box id.
		/// </summary>
		/// <param name="smtpBoxId">The SMTP box id.</param>
		/// <returns></returns>
		public static SmtpBox Load(int smtpBoxId)
		{
			return new SmtpBox(new SmtpBoxRow(smtpBoxId));
		}

		/// <summary>
		/// Lists this instance.
		/// </summary>
		/// <returns></returns>
		public static SmtpBox[] List()
		{
			List<SmtpBox> retVal = new List<SmtpBox>();

			foreach (SmtpBoxRow row in SmtpBoxRow.List())
			{
				retVal.Add(new SmtpBox(row));
			}

			return retVal.ToArray();
		}

		/// <summary>
		/// Totals the count.
		/// </summary>
		/// <returns></returns>
		public static int TotalCount()
		{
			return SmtpBoxRow.GetTotalCount();
		}

		/// <summary>
		/// Deletes the specified SMTP box id.
		/// </summary>
		/// <param name="smtpBoxId">The SMTP box id.</param>
		public static void Delete(int smtpBoxId)
		{
			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				// TODO: Check that exists element with IsDefault = 1
				SmtpBoxRow row = new SmtpBoxRow(smtpBoxId);
				row.Delete();

				if (row.IsDefault)
				{
					// Set First SMTP Box As Default
					SmtpBox newDefaultBox = GetDefault();
					if(newDefaultBox!=null)
					{
						newDefaultBox.SourceRow.IsDefault = true;
						newDefaultBox.SourceRow.Update();
					}
				}

				tran.Commit();
			}
		}

		/// <summary>
		/// Checks the settings.
		/// </summary>
		/// <param name="smtpServerHost">The SMTP server host.</param>
		/// <param name="port">The port.</param>
		/// <param name="secureConnection">The secure connection.</param>
		/// <param name="authenticate">if set to <c>true</c> [authenticate].</param>
		/// <param name="user">The user.</param>
		/// <param name="password">The password.</param>
		/// <returns></returns>
		public static SmtpSettingsResult CheckSettings(string smtpServerHost, int port,
			SecureConnectionType secureConnection,
			bool authenticate,
			string user, string password)
		{
			// OZ Remove White Space [2009-04-28]
			smtpServerHost = smtpServerHost.Trim();
			user = user.Trim();
			//

			try
			{
				SmtpClient client = new SmtpClient(smtpServerHost, port);
				client.SecureConnectionType = secureConnection;
				client.Authenticate = authenticate;
				client.User = user;
				client.Password = password;

				client.Send();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex, "SmtpBox.CheckSettings");

				Mediachase.Ibn.Log.WriteError("Check Smtp Settings Error: " + ex.ToString());

				return SmtpSettingsResult.None;
			}

			return SmtpSettingsResult.AllOk;
			//return SmtpClientUtility.CheckSettings(smtpServerHost, port,
			//secureConnection,
			//authenticate,
			//user, password);
		}

		/// <summary>
		/// Sends the test email.
		/// </summary>
		/// <param name="smtpBoxId">The SMTP box id.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="htmlBody">The HTML body.</param>
		/// <param name="completeCheckPageUrl">The complete check page URL.</param>
		public static void SendTestEmail(int smtpBoxId, string subject, string htmlBody, string completeCheckPageUrl)
		{
			if (subject == null)
				throw new ArgumentNullException("subject");
			if (htmlBody == null)
				throw new ArgumentNullException("htmlBody");
			if (completeCheckPageUrl == null)
				throw new ArgumentNullException("completeCheckPageUrl");

			SmtpBoxRow row = new SmtpBoxRow(smtpBoxId);

			// Reset Check params
			row.Checked = false;
			row.CheckUid = Guid.NewGuid();

			row.Update();

			// Create Complete Back Link
			string backLink = completeCheckPageUrl + "?uid=" + row.CheckUid.ToString();

			SmtpClient client = SmtpClientUtility.CreateSmtpClient(new SmtpBox(row));

			string to = Security.CurrentUser.Email;

			// Create Mail Message
			MailMessage tesMsg = new MailMessage();

			MailAddress currentUser = new MailAddress(to, Security.CurrentUser.DisplayName);

			tesMsg.To.Add(currentUser);
			tesMsg.From = currentUser;
			tesMsg.Subject = subject;

			tesMsg.IsBodyHtml = true;
			tesMsg.Body = htmlBody + "<br/><a href='" + backLink + "'>" + backLink + "</a>";

			client.Send(tesMsg);

			if (!string.IsNullOrEmpty(tesMsg.ErrorMessage))
			{
				throw new SmtpException(tesMsg.ErrorMessage);
			}
		}


		/// <summary>
		/// Sets the default.
		/// </summary>
		/// <param name="smtpBoxId">The SMTP box id.</param>
		public static void SetDefault(int smtpBoxId)
		{
			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				// Reset All Default
				foreach (SmtpBoxRow row in SmtpBoxRow.List(FilterElement.EqualElement(SmtpBoxRow.ColumnIsDefault, true)))
				{
					row.IsDefault = false;
					row.Update();
				}

				SmtpBoxRow newDefaultRow = new SmtpBoxRow(smtpBoxId);
				newDefaultRow.IsDefault = true;
				newDefaultRow.Update();

				tran.Commit();
			}
		}

		/// <summary>
		/// Commits the test email.
		/// </summary>
		/// <param name="checkUid">The check uid.</param>
		/// <returns></returns>
		public static bool CommitTestEmail(Guid checkUid)
		{
			SmtpBoxRow[] rows = SmtpBoxRow.List(FilterElement.EqualElement(SmtpBoxRow.ColumnCheckUid, checkUid));

			if (rows.Length > 0)
			{
				using (TransactionScope tran = DataContext.Current.BeginTransaction())
				{
					foreach (SmtpBoxRow row in rows)
					{
						row.Checked = true;
						row.Update();
					}

					tran.Commit();
				}
			}

			return rows.Length > 0;
		}

		/// <summary>
		/// Gets the default.
		/// </summary>
		/// <returns></returns>
		public static SmtpBox GetDefault()
		{
			SmtpBoxRow[] rows = SmtpBoxRow.List(FilterElement.EqualElement(SmtpBoxRow.ColumnIsDefault, true));

			// Fix No Default Problem
			if (rows.Length == 0)
				rows = SmtpBoxRow.List();

			if (rows.Length > 0)
				return new SmtpBox(rows[0]);

			return null;
		}

		#endregion

	}
}
