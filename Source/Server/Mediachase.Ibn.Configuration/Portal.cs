using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Mediachase.Database;

namespace Mediachase.Ibn.Configuration
{
	internal class Portal
	{
		private Portal()
		{
		}

		#region internal static IConfigurationParameter[] ListPortalParameters(DBHelper dbHelper)
		internal static IConfigurationParameter[] ListPortalParameters(DBHelper dbHelper)
		{
			List<IConfigurationParameter> list = new List<IConfigurationParameter>();

			using (IDataReader reader = dbHelper.RunTextDataReader("SELECT [Key],[Value] FROM [PortalConfig] ORDER BY [Key]"))
			{
				while (reader.Read())
				{
					string name = reader[0].ToString();
					string value = reader[1].ToString();

					if (!name.StartsWith("system.", StringComparison.OrdinalIgnoreCase))
					{
						ConfigurationParameter item = new ConfigurationParameter(name, value);
						list.Add(item);
					}
				}
			}

			return list.ToArray();
		}
		#endregion
		#region internal static string GetPortalParameterValue(DBHelper dbHelper, string name)
		internal static string GetPortalParameterValue(DBHelper dbHelper, string name)
		{
			string result = null;

			using (IDataReader reader = dbHelper.RunTextDataReader("SELECT [Value] FROM [PortalConfig] WHERE [Key]=@Key"
				, DBHelper.MP("@Key", SqlDbType.NVarChar, 100, name)))
			{
				if (reader.Read())
					result = reader[0].ToString();
			}

			return result;
		}
		#endregion
		#region internal static void SetPortalParameterValue(DBHelper dbHelper, string name, string value)
		internal static void SetPortalParameterValue(DBHelper dbHelper, string name, string value)
		{
			if (value == null)
			{
				dbHelper.RunText("DELETE FROM [PortalConfig] WHERE [Key]=@Key"
					, DBHelper.MP("@Key", SqlDbType.NVarChar, 100, name)
					);
			}
			else
			{
				dbHelper.RunText("IF EXISTS (SELECT 1 FROM [PortalConfig] WHERE [Key]=@Key) UPDATE [PortalConfig] SET [Value]=@Value WHERE [Key]=@Key ELSE INSERT INTO [PortalConfig] ([Key],[Value]) VALUES (@Key,@Value)"
					, DBHelper.MP("@Key", SqlDbType.NVarChar, 100, name)
					, DBHelper.MP("@Value", SqlDbType.NText, value)
					);
			}
		}
		#endregion
	}
}
