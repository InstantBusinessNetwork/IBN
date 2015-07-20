using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

using Mediachase.Database;


namespace Mediachase.Ibn.Converter
{
	internal class MyMetaClass
	{
		public int MetaClassId;
		public string Name;
		public string TableName;
		public string TableNameHistory;

		public ArrayList Fields = new ArrayList();

		public MyMetaClass()
		{
		}

		#region LoadList()
		public static ArrayList LoadList(DBHelper source)
		{
			ArrayList ret = new ArrayList();
			using (IDataReader reader = source.RunTextDataReader("SELECT [MetaClassId], [Name], [TableName] FROM [MetaClass] WHERE [IsSystem] = 0 AND [IsAbstract] = 0"))
			{
				while (reader.Read())
				{
					MyMetaClass item = Load(reader);
					if (item.Name != "AssetsEx")
						ret.Add(item);
				}
			}
			return ret;
		}
		#endregion

		#region Load()
		public static MyMetaClass Load(IDataRecord reader)
		{
			MyMetaClass ret = new MyMetaClass();

			ret.MetaClassId = (int)reader["MetaClassId"];
			ret.Name = reader["Name"].ToString();
			ret.TableName = reader["TableName"].ToString();
			ret.TableNameHistory = ret.TableName + "_History";

			return ret;
		}
		#endregion

		public void LoadFields(DBHelper source)
		{
			Fields.Clear();
			MyMetaField.LoadList(source, MetaClassId, Fields);
		}

		public void CreateTables(DBHelper target)
		{
			StringBuilder sb = new StringBuilder();
			StringBuilder sb2 = new StringBuilder();
			StringBuilder sb3 = new StringBuilder();

			target.RunText(string.Format(CultureInfo.InvariantCulture, "if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[{0}]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [dbo].[{0}]", TableName));
			target.RunText(string.Format(CultureInfo.InvariantCulture, "if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[{0}]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [dbo].[{0}]", TableNameHistory));

			sb.Append(string.Format(CultureInfo.InvariantCulture, "CREATE TABLE [dbo].[{0}] ([ObjectId] [int] NOT NULL, [CreatorId] [int] NULL, [Created] [datetime] NULL, [ModifierId] [int] NULL, [Modified] [datetime] NULL", TableName));
			sb2.Append(string.Format(CultureInfo.InvariantCulture, "CREATE TABLE [dbo].[{0}] ([Id] [int] IDENTITY (1, 1) NOT NULL, [ObjectId] [int] NOT NULL, [ModifierId] [int] NULL, [Modified] [datetime] NULL", TableNameHistory));
			sb3.Append(string.Format(CultureInfo.InvariantCulture, "ALTER TABLE [dbo].[{0}] ADD ", TableName));

			string column;
			int defaultCount = 0;
			foreach (MyMetaField field in Fields)
			{
				column = string.Format(CultureInfo.InvariantCulture, ", [{0}] [{1}] {2}", field.Name, field.SqlName, field.Variable ? ("(" + field.Length + ") ") : "");
				sb.Append(column);
				sb2.Append(column);

				if (!field.AllowNulls)
					sb.Append("NOT ");
				sb.Append("NULL");
				sb2.Append("NULL");

				if (!field.AllowNulls)
				{
					if (defaultCount > 0)
						sb3.Append(", ");
					sb3.Append(string.Format(CultureInfo.InvariantCulture, "CONSTRAINT [DF__{0}__{1}] DEFAULT ({2}) FOR [{1}]", TableName, field.Name, (field.DefaultValue.Length > 0 ? field.DefaultValue : "''")));
					defaultCount++;
				}
			}
			sb.Append(") ON [PRIMARY]");
			sb2.Append(") ON [PRIMARY]");

			target.RunText(sb.ToString());
			target.RunText(sb2.ToString());

			target.RunText(string.Format(CultureInfo.InvariantCulture, "ALTER TABLE [dbo].[{0}] WITH NOCHECK ADD CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ([ObjectId]) ON [PRIMARY]", TableName));
			target.RunText(string.Format(CultureInfo.InvariantCulture, "ALTER TABLE [dbo].[{0}] WITH NOCHECK ADD CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ([Id]) ON [PRIMARY]", TableNameHistory));

			if (defaultCount > 0)
				target.RunText(sb3.ToString());

			target.RunSP("mdpsp_sys_CreateMetaClassHistoryTrigger", DBHelper.MP("@MetaClassId", SqlDbType.Int, MetaClassId));
			//if(!Name.StartsWith("ListsEx_"))
			target.RunSP("mdpsp_sys_CreateMetaClassProcedure", DBHelper.MP("@MetaClassId", SqlDbType.Int, MetaClassId));
		}
	}
}
