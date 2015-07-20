using System;
using System.Collections;
using System.Data;

using Mediachase.IBN.Database;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus.Import;


namespace Mediachase.IBN.Business.Import
{
	/// <summary>
	/// Summary description for UserMappingMetaClass.
	/// </summary>
	public class EmptyFieldException : Exception
	{
		public EmptyFieldException(string field)
			: base(String.Format("{0} cannot be empty", field))
		{
		}
	}

	public class UserMappingMetaClass : MappingMetaClass
	{
		public UserMappingMetaClass()
		{
			InnerMetaClassName = "UsersEx";
		}

		protected override void FillSystemColumnInfo(ArrayList array)
		{
			MetaClass mc = MetaClass.Load(InnerMetaClassName);

			FillTypes fillTypes = FillTypes.CopyValue | FillTypes.Custom | FillTypes.Default;

			array.Add(new ColumnInfo(mc.SystemMetaFields["Login"], fillTypes));
			array.Add(new ColumnInfo(mc.SystemMetaFields["FirstName"], fillTypes));
			array.Add(new ColumnInfo(mc.SystemMetaFields["LastName"], fillTypes));
			array.Add(new ColumnInfo(mc.SystemMetaFields["Password"], fillTypes));
			array.Add(new ColumnInfo(mc.SystemMetaFields["Email"], fillTypes));
			array.Add(new ColumnInfo(MetaField.CreateVirtual(mc.Namespace, "Phone", "Phone", "",
																MetaDataType.ShortString, 255,
																true, false, false, false), fillTypes));
			array.Add(new ColumnInfo(MetaField.CreateVirtual(mc.Namespace, "Fax", "Fax", "",
																MetaDataType.ShortString, 255,
																true, false, false, false), fillTypes));
			array.Add(new ColumnInfo(MetaField.CreateVirtual(mc.Namespace, "Mobile", "Mobile", "",
																MetaDataType.ShortString, 255,
																true, false, false, false), fillTypes));
			array.Add(new ColumnInfo(MetaField.CreateVirtual(mc.Namespace, "Company", "Company", "",
																MetaDataType.ShortString, 255,
																true, false, false, false), fillTypes));
			array.Add(new ColumnInfo(MetaField.CreateVirtual(mc.Namespace, "JobTitle", "JobTitle", "",
																MetaDataType.ShortString, 255,
																true, false, false, false), fillTypes));
			array.Add(new ColumnInfo(MetaField.CreateVirtual(mc.Namespace, "Department", "Department", "",
																MetaDataType.ShortString, 255,
																true, false, false, false), fillTypes));
			array.Add(new ColumnInfo(MetaField.CreateVirtual(mc.Namespace, "Location", "Location", "",
																MetaDataType.ShortString, 255,
																true, false, false, false), fillTypes));
		}

		protected override int CreateSystemRow(FillDataMode mode, params object[] item)
		{
			if (mode == FillDataMode.Update)
			{
				throw new NotSupportedException("Update is not supported for User import");
			}
			string Login = "";
			string Password = "";
			string FirstName = "";
			string LastName = "";
			string Email = "";
			string Phone = "";
			string Fax = "";
			string Mobile = "";
			string Company = "";
			string JobTitle = "";
			string Department = "";
			string Location = "";

			if (item[0] == null || (string)item[0] == "") throw new EmptyFieldException("Login");
			else Login = (string)item[0];

			if (item[1] == null || (string)item[2] == "") throw new EmptyFieldException("FirstName");
			else FirstName = (string)item[2];

			if (item[2] == null || (string)item[3] == "") throw new EmptyFieldException("LastName");
			else LastName = (string)item[3];

			if (item[3] == null || (string)item[1] == "") throw new EmptyFieldException("Password");
			else Password = (string)item[1];

			if (item[4] != null) Email = (string)item[4];
			if (item[5] != null) Phone = (string)item[5];
			if (item[6] != null) Fax = (string)item[6];
			if (item[7] != null) Mobile = (string)item[7];
			if (item[8] != null) Company = (string)item[8];
			if (item[9] != null) JobTitle = (string)item[9];
			if (item[10] != null) Department = (string)item[10];
			if (item[11] != null) Location = (string)item[11];

			//TODO: IMGroupId?, LanguageId?
			int UserId = User.Create(Login, Password, FirstName, LastName, Email, true, null, 0, Phone, Fax, Mobile, JobTitle,
										Department, Company, Location, User.DefaultTimeZoneId, 1, null, null, -1);

			return UserId;
		}

		public FillResult FillData(FillDataMode mode, DataTable rawData, Mediachase.MetaDataPlus.Import.Rule rule, int maximumErrors)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				FillResult retVal = FillData(mode, rawData, rule, Security.CurrentUser.UserID, DateTime.UtcNow, DbContext.Current.Transaction);
				if (maximumErrors == -1 || retVal.ErrorRows <= maximumErrors)
					tran.Commit();
				else tran.Rollback();

				return retVal;
			}
		}
	}
}
