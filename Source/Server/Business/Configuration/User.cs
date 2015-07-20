using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Mediachase.Database;

namespace Mediachase.Ibn.Business.Configuration
{
	internal class User
	{
		public int principalId;
		public string login;
		public string password;
		public string firstName;
		public string lastName;
		public string email;
		public string salt;
		public string hash;
		public int imGroupId;
		public int originalId;
		public bool isActive;

		private User()
		{
		}

		public static User[] GetUsers(DBHelper dbh, int groupId, int alertGroupID)
		{
			List<User> list = new List<User>();

			using (IDataReader reader = dbh.RunTextDataReader("SELECT PrincipalId, Login, Password, FirstName, LastName, Email, Activity, salt, [hash] FROM [USERS] WHERE Login IS NOT NULL AND Password IS NOT NULL AND IsExternal = 0"))
			{
				while (reader.Read())
				{
					User item = new User();

					item.principalId = (int)reader["PrincipalId"];
					item.login = reader["Login"].ToString();
					item.password = reader["Password"].ToString();
					item.firstName = reader["FirstName"].ToString();
					item.lastName = reader["LastName"].ToString();
					item.email = reader["Email"].ToString();

					if (reader["salt"] != DBNull.Value)
						item.salt = reader["salt"].ToString();

					if (reader["hash"] != DBNull.Value)
						item.hash = reader["hash"].ToString();

					item.isActive = (byte)reader["Activity"] == 3;

					if (string.Compare(item.login, "alert", StringComparison.OrdinalIgnoreCase) != 0)
						item.imGroupId = groupId;
					else
						item.imGroupId = alertGroupID;

					list.Add(item);
				}
			}

			return list.ToArray();
		}
	}
}
