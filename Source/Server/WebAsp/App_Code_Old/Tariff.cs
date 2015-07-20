using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

using Mediachase.Ibn.Configuration;

namespace Mediachase.Ibn.WebAsp
{
	public class Tariff
	{
		#region TariffType
		#region GetTariffType
		/// <summary>
		/// Reader returns fields:
		///		typeId, typeName, isActive, canDelete
		/// </summary>
		public static IDataReader GetTariffType(int typeId)
		{
			return DBHelper.RunSPReturnDataReader("ASP_TARIFF_TYPE_GET",
				DBHelper.mp("@typeId", SqlDbType.Int, typeId));
		}
		#endregion

		#region AddTariffType
		public static int AddTariffType(string typeName, bool isActive)
		{
			return DBHelper.RunSPReturnInteger("ASP_TARIFF_TYPE_ADD",
				DBHelper.mp("@typeName", SqlDbType.NVarChar, 250, typeName),
				DBHelper.mp("@isActive", SqlDbType.Bit, isActive));
		}
		#endregion

		#region UpdateTariffType
		public static void UpdateTariffType(int typeId, string typeName, bool isActive)
		{
			DBHelper.RunSP("ASP_TARIFF_TYPE_UPDATE",
				DBHelper.mp("@typeId", SqlDbType.Int, typeId),
				DBHelper.mp("@typeName", SqlDbType.NVarChar, 250, typeName),
				DBHelper.mp("@isActive", SqlDbType.Bit, isActive));
		}
		#endregion

		#region DeleteTariffType
		public static void DeleteTariffType(int typeId)
		{
			DBHelper.RunSP("ASP_TARIFF_TYPE_DELETE",
				DBHelper.mp("@typeId", SqlDbType.Int, typeId));
		}
		#endregion
		#endregion

		#region Currency
		#region GetCurrency
		/// <summary>
		/// Reader returns fields:
		///		currencyId, currencyName, symbol, canDelete
		/// </summary>
		public static IDataReader GetCurrency(int currencyId)
		{
			return DBHelper.RunSPReturnDataReader("ASP_CURRENCY_GET",
				DBHelper.mp("@currencyId", SqlDbType.Int, currencyId));
		}
		#endregion

		#region AddCurrency
		public static int AddCurrency(string currencyName, string symbol)
		{
			return DBHelper.RunSPReturnInteger("ASP_CURRENCY_ADD",
				DBHelper.mp("@currencyName", SqlDbType.NVarChar, 50, currencyName),
				DBHelper.mp("@symbol", SqlDbType.NVarChar, 10, symbol));
		}
		#endregion

		#region UpdateCurrency
		public static void UpdateCurrency(int currencyId, string currencyName, string symbol)
		{
			DBHelper.RunSP("ASP_CURRENCY_UPDATE",
				DBHelper.mp("@currencyId", SqlDbType.Int, currencyId),
				DBHelper.mp("@currencyName", SqlDbType.NVarChar, 50, currencyName),
				DBHelper.mp("@symbol", SqlDbType.NVarChar, 10, symbol));
		}
		#endregion

		#region DeleteCurrency
		public static void DeleteCurrency(int currencyId)
		{
			DBHelper.RunSP("ASP_CURRENCY_DELETE",
				DBHelper.mp("@currencyId", SqlDbType.Int, currencyId));
		}
		#endregion
		#endregion

		#region Tariff
		#region GetTariff
		/// <summary>
		/// Reader returns fields:
		///		tariffId, tariffName, description, typeId, typeName, 
		///		currencyId, currencyName, symbol,
		///		monthlyCost, dailyCost28, dailyCost29, dailyCost30, dailyCost31, 
		///		maxHdd, maxUsers, maxExternalUsers
		/// </summary>
		public static IDataReader GetTariff(int tariffId, int typeId)
		{
			return DBHelper.RunSPReturnDataReader("ASP_TARIFF_GET",
				DBHelper.mp("@tariffId", SqlDbType.Int, tariffId),
				DBHelper.mp("@typeId", SqlDbType.Int, typeId));
		}
		#endregion

		#region GetTariffActive
		/// <summary>
		/// Gets the active tariffs.
		/// </summary>
		/// <returns>tariffId, tariffName, description, typeId, typeName, currencyId, currencyName, symbol, monthlyCost, dailyCost28, dailyCost29, dailyCost30, dailyCost31, maxHdd, maxUsers, maxExternalUsers</returns>
		public static DataTable GetTariffActive()
		{
			return DBHelper.RunSPReturnDataTable("ASP_TARIFF_GET_ACTIVE");
		}
		#endregion

		#region AddTariff
		public static int AddTariff(string tariffName, string description, int typeId, int currencyId,
			decimal monthlyCost, int maxHdd, int maxUsers, int maxExternalUsers)
		{
			decimal dailyCost28 = monthlyCost / 28m;
			decimal dailyCost29 = monthlyCost / 29m;
			decimal dailyCost30 = monthlyCost / 30m;
			decimal dailyCost31 = monthlyCost / 31m;

			return DBHelper.RunSPReturnInteger("ASP_TARIFF_ADD",
				DBHelper.mp("@tariffName", SqlDbType.NVarChar, 250, tariffName),
				DBHelper.mp("@description", SqlDbType.NText, description),
				DBHelper.mp("@typeId", SqlDbType.Int, typeId),
				DBHelper.mp("@currencyId", SqlDbType.Int, currencyId),
				DBHelper.mp("@monthlyCost", SqlDbType.Money, monthlyCost),
				DBHelper.mp("@dailyCost28", SqlDbType.Money, dailyCost28),
				DBHelper.mp("@dailyCost29", SqlDbType.Money, dailyCost29),
				DBHelper.mp("@dailyCost30", SqlDbType.Money, dailyCost30),
				DBHelper.mp("@dailyCost31", SqlDbType.Money, dailyCost31),
				DBHelper.mp("@maxHdd", SqlDbType.Int, maxHdd),
				DBHelper.mp("@maxUsers", SqlDbType.Int, maxUsers),
				DBHelper.mp("@maxExternalUsers", SqlDbType.Int, maxExternalUsers)
				);
		}
		#endregion

		#region UpdateTariff
		public static void UpdateTariff(int tariffId, string tariffName, string description, int typeId, int currencyId,
			decimal monthlyCost, int maxHdd, int maxUsers, int maxExternalUsers)
		{
			decimal dailyCost28 = monthlyCost / 28m;
			decimal dailyCost29 = monthlyCost / 29m;
			decimal dailyCost30 = monthlyCost / 30m;
			decimal dailyCost31 = monthlyCost / 31m;

			DBHelper.RunSP("ASP_TARIFF_UPDATE",
				DBHelper.mp("@tariffId", SqlDbType.Int, tariffId),
				DBHelper.mp("@tariffName", SqlDbType.NVarChar, 250, tariffName),
				DBHelper.mp("@description", SqlDbType.NText, description),
				DBHelper.mp("@typeId", SqlDbType.Int, typeId),
				DBHelper.mp("@currencyId", SqlDbType.Int, currencyId),
				DBHelper.mp("@monthlyCost", SqlDbType.Money, monthlyCost),
				DBHelper.mp("@dailyCost28", SqlDbType.Money, dailyCost28),
				DBHelper.mp("@dailyCost29", SqlDbType.Money, dailyCost29),
				DBHelper.mp("@dailyCost30", SqlDbType.Money, dailyCost30),
				DBHelper.mp("@dailyCost31", SqlDbType.Money, dailyCost31),
				DBHelper.mp("@maxHdd", SqlDbType.Int, maxHdd),
				DBHelper.mp("@maxUsers", SqlDbType.Int, maxUsers),
				DBHelper.mp("@maxExternalUsers", SqlDbType.Int, maxExternalUsers)
				);
		}
		#endregion

		#region DeleteTariff
		public static void DeleteTariff(int tariffId)
		{
			DBHelper.RunSP("ASP_TARIFF_DELETE",
				DBHelper.mp("@tariffId", SqlDbType.Int, tariffId));
		}
		#endregion
		#endregion

		#region TariffRequest
		#region GetTariffRequests
		/// <summary>
		/// Reader returns fields:
		///		requestId, companyUid, company_name, tariffId, tariffName, dt, description
		/// </summary>
		public static IDataReader GetTariffRequests(int requestId)
		{
			return DBHelper.RunSPReturnDataReader("ASP_TARIFF_REQUESTS_GET",
				DBHelper.mp("@requestId", SqlDbType.Int, requestId));
		}
		#endregion

		#region AddTariffRequest
		public static void AddTariffRequest(Guid companyUid, int tariffId, string description)
		{
			DBHelper.RunSP("ASP_TARIFF_REQUEST_ADD",
				DBHelper.mp("@companyUid", SqlDbType.UniqueIdentifier, companyUid),
				DBHelper.mp("@tariffId", SqlDbType.Int, tariffId),
				DBHelper.mp("@description", SqlDbType.NText, description)
				);
		}
		#endregion

		#region DeleteTariffRequest
		public static void DeleteTariffRequest(int requestId)
		{
			DBHelper.RunSP("ASP_TARIFF_REQUEST_DELETE",
				DBHelper.mp("@requestId", SqlDbType.Int, requestId));
		}
		#endregion
		#endregion

		#region Payment
		#region GetPayment
		/// <summary>
		///		paymentId, companyUid, company_name, dt, amount, bonus, orderNo
		/// </summary>
		public static DataTable GetPayment(int paymentId, Guid companyUid)
		{
			return DBHelper.RunSPReturnDataTable("ASP_PAYMENT_GET",
				DBHelper.mp("@paymentId", SqlDbType.Int, paymentId),
				DBHelper.mp("@companyUid", SqlDbType.UniqueIdentifier, companyUid));
		}
		#endregion

		#region GetPaymentForPeriod
		/// <summary>
		///		paymentId, companyUid, company_name, dt, amount, bonus, orderNo
		/// </summary>
		public static DataTable GetPaymentForPeriod(Guid companyUid, int days)
		{
			return DBHelper.RunSPReturnDataTable("ASP_PAYMENT_GET_FOR_PERIOD",
				DBHelper.mp("@companyUid", SqlDbType.UniqueIdentifier, companyUid),
				DBHelper.mp("@days", SqlDbType.Int, days));
		}
		#endregion

		#region AddPayment
		public static void AddPayment(Guid companyUid, decimal amount, string orderNo)
		{
			AddPayment(companyUid, DateTime.Now, amount, 0m, orderNo, true);
		}

		public static void AddPayment(Guid companyUid, DateTime dt, decimal amount, decimal bonus, string orderNo, bool checkUnicity)
		{
			if (amount + bonus <= 0)
				throw new ArgumentException("Wrong value", "amount");

			IConfigurator config = Configurator.Create();
			ICompanyInfo company = config.GetCompanyInfo(companyUid.ToString());
			if (company == null)
				throw new ArgumentException(String.Concat("CompanyUid = ", companyUid, " doesn't exist."));

			if (checkUnicity && !string.IsNullOrEmpty(orderNo) && CheckPaymentOrderNo(companyUid, orderNo) > 0)
				throw new Exception(String.Concat("Order # ", orderNo, " already registered."));

			bool newTran = DBHelper.BeginTransaction();
			try
			{
				DBHelper.RunSP("ASP_PAYMENT_ADD",
					DBHelper.mp("@companyUid", SqlDbType.UniqueIdentifier, companyUid),
					DBHelper.mp("@dt", SqlDbType.DateTime, dt),
					DBHelper.mp("@amount", SqlDbType.Money, amount),
					DBHelper.mp("@bonus", SqlDbType.Money, bonus),
					DBHelper.mp("@orderNo", SqlDbType.NVarChar, 50, orderNo));

				UpdateBalance(companyUid, amount + bonus);

				// Activate inactive company
				if (!company.IsActive)
				{
					DBHelper.RunSP("ASP_COMPANY_UPDATE_IS_ACTIVE",
						DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, companyUid),
						DBHelper.mp("@is_active", SqlDbType.Bit, true));

					config.ActivateCompany(companyUid.ToString(), true, false);
				}

				#region Change company type from trial to billable
				if (int.Parse(config.GetCompanyPropertyValue(companyUid.ToString(), CManage.keyCompanyType)) == (int)CompanyType.Trial)
				{
					bool tariffIsValid = false;
					int maxUsers = -1;
					int maxExternalUsers = -1;
					int maxDiskSpace = -1;

					AspSettings settings = AspSettings.Load();
					if (settings.UseTariffs && settings.DefaultTariff > 0)
					{
						using (IDataReader reader = GetTariff(settings.DefaultTariff, 0))
						{
							if (reader.Read())
							{
								tariffIsValid = true;
								maxUsers = (int)reader["maxUsers"];
								maxExternalUsers = (int)reader["maxExternalUsers"];
								maxDiskSpace = (int)reader["maxHdd"];
							}
						}
					}

					if (tariffIsValid)
					{
						config.SetCompanyPropertyValue(companyUid.ToString(), CManage.keyCompanyMaxUsers, maxUsers.ToString());
						config.SetCompanyPropertyValue(companyUid.ToString(), CManage.keyCompanyMaxExternalUsers, maxExternalUsers.ToString());
						config.SetCompanyPropertyValue(companyUid.ToString(), CManage.keyCompanyDatabaseSize, maxDiskSpace.ToString());

						DBHelper.RunSP("ASP_COMPANY_UPDATE_TARIFF",
							DBHelper.mp("@companyUid", SqlDbType.UniqueIdentifier, companyUid),
							DBHelper.mp("@tariffId", SqlDbType.Int, settings.DefaultTariff));
					}

					config.SetCompanyPropertyValue(companyUid.ToString(), CManage.keyCompanyType, ((int)CompanyType.Billable).ToString());
					CManage.UpdateCompanyType(companyUid, (byte)CompanyType.Billable);
				}
				#endregion

				// Ensure, that the unpaid period is recalculated
				RecalculateBalance();

				TemplateVariables vars = CManage.CompanyGetVariables(companyUid);
				if (!string.IsNullOrEmpty(vars["ContactEmail"]))
				{
					vars["Amount"] = amount.ToString("f");
					vars["Bonus"] = bonus.ToString("f");
					vars["Total"] = (amount + bonus).ToString("f");
					CManage.SendEmail(vars["ContactEmail"], EmailType.ClientBalanceUp, vars);
				}

				DBHelper.Commit(newTran);
			}
			catch (Exception)
			{
				DBHelper.Rollback(newTran);
				throw;
			}
		}
		#endregion

		#region RecalculateBalance
		public static void RecalculateBalance()
		{
			bool newTran = DBHelper.BeginTransaction();
			try
			{
				Dictionary<string, decimal> dailyCostList = new Dictionary<string, decimal>();

				DateTime nowDate = DateTime.Now.Date;
				DataTable companies = Tariff.GetCompaniesWithTariff();

				foreach (DataRow row in companies.Rows)
				{
					// tariff and discount do not depends of the month
					int tariffId = (int)row["tariffId"];
					int discount = (int)row["discount"];

					#region find out the start date of calculation interval
					DateTime startDate = DateTime.Now.Date;	// start of the interval
					DateTime tariffStartDate = DateTime.Now.Date;
					if (row["tariffStartDate"] != DBNull.Value)
						tariffStartDate = ((DateTime)row["tariffStartDate"]).Date;

					DateTime? recalculateDate = null;	// last recalculation date
					if (row["recalculateDate"] != DBNull.Value)
						recalculateDate = ((DateTime)row["recalculateDate"]).Date;

					if (recalculateDate == null)
					{
						startDate = tariffStartDate;
					}
					else
					{
						if (recalculateDate < tariffStartDate)
							startDate = tariffStartDate;
						else
							startDate = recalculateDate.Value.AddDays(1);
					}
					#endregion

					DateTime dt = startDate;
					while (dt <= DateTime.Now.Date)
					{
						decimal dailyCost;
						string key = string.Concat(tariffId, "_", dt.Year, "_", dt.Month);
						if (dailyCostList.ContainsKey(key))
						{
							dailyCost = dailyCostList[key];
						}
						else
						{
							if (DateTime.DaysInMonth(dt.Year, dt.Month) == 28)
								dailyCost = (decimal)row["dailyCost28"];
							else if (DateTime.DaysInMonth(dt.Year, dt.Month) == 29)
								dailyCost = (decimal)row["dailyCost29"];
							else if (DateTime.DaysInMonth(dt.Year, dt.Month) == 30)
								dailyCost = (decimal)row["dailyCost30"];
							else
								dailyCost = (decimal)row["dailyCost31"];

							dailyCostList.Add(key, dailyCost);
						}

						decimal amount = dailyCost - dailyCost * discount / 100m;
						decimal balance = Tariff.UpdateBalance((Guid)row["company_uid"], -amount);

						Tariff.AddDailyLog((Guid)row["company_uid"], dt, tariffId, amount, balance);

						dt = dt.AddDays(1);
					}
				}
				DBHelper.Commit(newTran);
			}
			catch (Exception ex)
			{
				DBHelper.Rollback(newTran);

				Log.WriteError(ex.ToString());
			}
		}
		#endregion

		#region DeletePayment
		public static void DeletePayment(int paymentId)
		{
			DBHelper.RunSP("ASP_PAYMENT_DELETE",
				DBHelper.mp("@paymentId", SqlDbType.Int, paymentId));
		}
		#endregion

		#region UndoPayment
		public static void UndoPayment(int paymentId)
		{
			DataTable paymentInfo = GetPayment(paymentId, Guid.Empty);
			if (paymentInfo.Rows.Count > 0)
			{
				Guid companyUid = (Guid)paymentInfo.Rows[0]["companyUid"];
				decimal amount = (decimal)paymentInfo.Rows[0]["amount"] + (decimal)paymentInfo.Rows[0]["bonus"];

				bool newTran = DBHelper.BeginTransaction();
				try
				{
					UpdateBalance(companyUid, -amount);
					DeletePayment(paymentId);

					DBHelper.Commit(newTran);
				}
				catch (Exception)
				{
					DBHelper.Rollback(newTran);
					throw;
				}
			}
		}
		#endregion

		#region CheckPaymentOrderNo
		/// <summary>
		/// Returns RowCount
		/// </summary>
		/// <param name="companyUid"></param>
		/// <param name="orderNo"></param>
		/// <returns></returns>
		public static int CheckPaymentOrderNo(Guid companyUid, string orderNo)
		{
			return DBHelper.RunSPReturnInteger("ASP_PAYMENT_CHECK_ORDERNO",
				DBHelper.mp("@companyUid", SqlDbType.UniqueIdentifier, companyUid),
				DBHelper.mp("@orderNo", SqlDbType.NVarChar, 50, orderNo));
		}
		#endregion
		#endregion

		#region DailyLog
		#region GetDailyLog
		/// <summary>
		/// Reader returns fields:
		///		logId, dt, companyUid, company_name, tariffId, tariffName, amount, balance
		/// </summary>
		public static IDataReader GetDailyLog(Guid companyUid)
		{
			return DBHelper.RunSPReturnDataReader("ASP_DAILY_LOG_GET",
				DBHelper.mp("@companyUid", SqlDbType.UniqueIdentifier, companyUid));
		}
		#endregion

		#region GetDailyLogForPeriod
		/// <summary>
		/// Reader returns fields:
		///		logId, dt, companyUid, company_name, tariffId, tariffName, amount, balance
		/// </summary>
		public static DataTable GetDailyLogForPeriod(Guid companyUid, int days)
		{
			return DBHelper.RunSPReturnDataTable("ASP_DAILY_LOG_GET_FOR_PERIOD",
				DBHelper.mp("@companyUid", SqlDbType.UniqueIdentifier, companyUid),
				DBHelper.mp("@days", SqlDbType.Int, days));
		}
		#endregion

		#region AddDailyLog
		public static void AddDailyLog(Guid companyUid, DateTime dt, int tariffId, decimal amount, decimal balance)
		{
			DBHelper.RunSP("ASP_DAILY_LOG_ADD",
				DBHelper.mp("@companyUid", SqlDbType.UniqueIdentifier, companyUid),
				DBHelper.mp("@dt", SqlDbType.DateTime, dt.Date),
				DBHelper.mp("@tariffId", SqlDbType.Int, tariffId),
				DBHelper.mp("@amount", SqlDbType.Money, amount),
				DBHelper.mp("@balance", SqlDbType.Money, balance));
		}
		#endregion
		#endregion

		#region GetCompaniesWithTariff
		/// <summary>
		/// company_uid, tariffId, discount, tariffStartDate, recalculateDate, 
		/// dailyCost28, dailyCost29, dailyCost30, dailyCost31 
		/// </summary>
		/// <returns></returns>
		public static DataTable GetCompaniesWithTariff()
		{
			return DBHelper.RunSPReturnDataTable("ASP_COMPANIES_GET_WITH_TARIFF");
		}
		#endregion

		#region UpdateBalance()
		public static decimal UpdateBalance(Guid companyUid, decimal amount)
		{
			object retval = DBHelper.RunSPReturnObject("ASP_COMPANY_UPDATE_BALANCE",
				DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, companyUid),
				DBHelper.mp("@amount", SqlDbType.Money, amount));
			return (decimal)retval;
		}
		#endregion
	}
}
