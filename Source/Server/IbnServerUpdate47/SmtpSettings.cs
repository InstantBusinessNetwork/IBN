using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Mediachase.Database;
using System.Data;
using System.Globalization;

namespace IbnServerUpdate
{
	class SmtpSettings
	{
		private enum SecureConnectionType
		{
			None = 0,
			Ssl3 = 1,
			Tls = 2,
		}

		private string AllowOverride { get; set; }

		private string Server { get; set; }
		private string Port { get; set; }
		private string SecureConnection { get; set; }
		private string Authenticate { get; set; }
		private string User { get; set; }
		private string Password { get; set; }

		private SmtpSettings()
		{
		}

		#region internal static SmtpSettings Load(string databaseName)
		internal static SmtpSettings Load(string databaseName)
		{
			SmtpSettings settings = new SmtpSettings();

			settings.LoadFromFile(Path.Combine(Settings.InstallDir, @"Services\AlertService.exe.config"));
			if (settings.AllowOverride == bool.TrueString)
				settings.LoadFromDatabase(databaseName);

			return settings;
		}
		#endregion

		#region internal void Save()
		internal void Save()
		{
			int port = int.Parse(Port, CultureInfo.InvariantCulture);
			SecureConnectionType secureConnectionType = (SecureConnectionType)Enum.Parse(typeof(SecureConnectionType), SecureConnection);
			bool authenticate = bool.Parse(Authenticate);

			DBHelper2.DBHelper.RunTextScalar("INSERT INTO [SmtpBox] ([Name],[Server],[Port],[SecureConnection],[Authenticate],[User],[Password],[IsDefault]) VALUES (@Name,@Server,@Port,@SecureConnection,@Authenticate,@User,@Password,1)"
				, DBHelper.MP("@Name", SqlDbType.NVarChar, 255, Server)
				, DBHelper.MP("@Server", SqlDbType.NVarChar, 255, Server)
				, DBHelper.MP("@Port", SqlDbType.Int, port)
				, DBHelper.MP("@SecureConnection", SqlDbType.Int, (int)secureConnectionType)
				, DBHelper.MP("@Authenticate", SqlDbType.Bit, authenticate)
				, DBHelper.MP("@User", SqlDbType.NVarChar, 255, User)
				, DBHelper.MP("@Password", SqlDbType.NVarChar, 255, Password)
				);
		}
		#endregion


		#region private void LoadFromFile(string path)
		private void LoadFromFile(string path)
		{
			XmlNode section = null;

			if (File.Exists(path))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(path);

				section = doc.SelectSingleNode("/configuration/EmailAlertProvider");
			}

			AllowOverride = GetNodeAttribute(section, "AllowOverride", bool.TrueString);
			Server = GetChildNodeValue(section, "SmtpServer", "localhost");
			Port = GetChildNodeValue(section, "SmtpPort", "25");
			SecureConnection = GetChildNodeValue(section, "SmtpSecureConnection", SecureConnectionType.None.ToString());
			Authenticate = GetChildNodeValue(section, "Authenticate", bool.FalseString);
			User = GetChildNodeValue(section, "User", string.Empty);
			Password = GetChildNodeValue(section, "Password", string.Empty);
		}
		#endregion

		#region private void LoadFromDatabase(string databaseName)
		private void LoadFromDatabase(string databaseName)
		{
			string previousDatabase = DBHelper2.DBHelper.Database;
			try
			{
				DBHelper2.DBHelper.Database = databaseName;

				string useDefault = GetDatabaseValue("email.smtp.default", bool.TrueString);
				if (useDefault == bool.FalseString)
				{
					Server = GetDatabaseValue("email.smtp.servername", Server);
					Port = GetDatabaseValue("email.smtp.serverport", Port);
					SecureConnection = GetDatabaseValue("email.smtp.secure", SecureConnection);
					Authenticate = GetDatabaseValue("email.smtp.authenticate", Authenticate);
					User = GetDatabaseValue("email.smtp.user", User);
					Password = GetDatabaseValue("email.smtp.password", Password);
				}
			}
			finally
			{
				DBHelper2.DBHelper.Database = previousDatabase;
			}
		}
		#endregion

		#region private static string GetChildNodeValue(XmlNode parent, string childNodeName, string defaultValue)
		private static string GetChildNodeValue(XmlNode parent, string childNodeName, string defaultValue)
		{
			string result = defaultValue;

			if (parent != null)
			{
				XmlNode child = parent.SelectSingleNode(childNodeName);
				result = GetNodeAttribute(child, "value", defaultValue);
			}

			return result;
		}
		#endregion
		#region private static string GetNodeAttribute(XmlNode node, string attributeName, string defaultValue)
		private static string GetNodeAttribute(XmlNode node, string attributeName, string defaultValue)
		{
			string result = defaultValue;

			if (node != null)
			{
				XmlAttribute attribute = node.Attributes[attributeName];
				if (attribute != null)
					result = attribute.Value;
			}

			return result;
		}
		#endregion

		#region private static string GetDatabaseValue(string name, string defaultValue)
		private static string GetDatabaseValue(string name, string defaultValue)
		{
			string result = defaultValue;

			object value = DBHelper2.DBHelper.RunTextScalar("SELECT [Value] FROM [PortalConfig] WHERE [Key]=@Key"
				, DBHelper.MP("@Key", SqlDbType.NVarChar, 100, name));

			if (value != null && value != DBNull.Value)
				result = (string)value;

			return result;
		}
		#endregion
	}
}
