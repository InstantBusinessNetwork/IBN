using System;
using System.Data;
using System.Web;
using System.Xml;
using System.Globalization;

namespace Mediachase.Ibn.WebTrial
{
	/// <summary>
	/// Summary description for CManage.
	/// </summary>
	public class CManage
	{
		public CManage()
		{
		}

		private static string fName = "Referer.xml";

		public static string GetReferrer(HttpRequest request)
		{
			string retVal = request["referer"];
			if (string.IsNullOrEmpty(retVal))
				retVal = request.UrlReferrer.AbsolutePath;

			//DVS: new info in trial
			if (request["googleCampagain"] != null)
				retVal += "&googleCampagain=" + request["googleCampagain"];

			if (request["googleKeyword"] != null)
				retVal += "&googleKeyword=" + request["googleSource"];

			if (request["googleRelative"] != null)
				retVal += "&googleRelative=" + request["googleRelative"];

			if (request["googleSource"] != null)
				retVal += "&googleSource=" + request["googleSource"];

			AppendDebugInfo(retVal);

			return retVal;
		}

		public static void AppendDebugInfo(string info)
		{
			string retVal = string.Empty;

			XmlDocument doc = new XmlDocument();
			doc.Load(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + fName);

			foreach (XmlNode node in doc.DocumentElement.SelectNodes("Referer"))
			{
				//if (node.Attributes["Id"].InnerText == uid && node.Attributes["RefValue"] != null)
				//{
				//    return node.Attributes["RefValue"].InnerText;
				//}

				if (node.Attributes["RefValueDate"] == null)
				{
					doc.DocumentElement.RemoveChild(node);
					continue;
				}

				if (DateTime.Now - Convert.ToDateTime(node.Attributes["RefValueDate"].InnerText, CultureInfo.InvariantCulture) > TimeSpan.FromMinutes(1440))
				{
					doc.DocumentElement.RemoveChild(node);
					continue;
				}
			}

			XmlElement el = doc.CreateElement("Referer");
			el.Attributes.Append(doc.CreateAttribute("Id")).Value = Guid.NewGuid().ToString("N");

			el.Attributes.Append(doc.CreateAttribute("RefValue")).Value = info;
			el.Attributes.Append(doc.CreateAttribute("Ip")).Value = HttpContext.Current.Request.UserHostAddress;
			el.Attributes.Append(doc.CreateAttribute("RefValueDate")).Value = DateTime.Now.ToString("f", CultureInfo.InvariantCulture);

			doc.DocumentElement.AppendChild(el);
			doc.Save(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + fName);
		}

		public static string GetReseller(HttpRequest request)
		{
			string retVal = request["Reseller"];
			if (string.IsNullOrEmpty(retVal))
				retVal = Settings.UnknownResellerGuid;
			return retVal;
		}

/*
		@RequestID as int ,
		@CompanyName as nvarchar(250) ,
		@SizeOfGroup as nvarchar(50) ,
		@Description as ntext ,
		@Domain as nvarchar(255) ,
		@FirstName as nvarchar(100) ,
		@LastName as nvarchar(100) ,
		@Email as nvarchar(100) ,
		@Phone as nvarchar(100) ,
		@Country as nvarchar(100) ,
		@TimeZone as nvarchar(150) ,
		@Login as nvarchar(50) ,
		@Password as nvarchar(50) ,
		@ResellerGUID as uniqueidentifier ,*/


		public static int CreateRequest
		(
			string PortalName,
			string SizeOfGroup,
			string Description,
			string Domain,
			string FirstName,
			string LastName,
			string Email,
			string Country,
			string Phone,
			string TimeZone,
			string Login,
			string Password,
			Guid ResellerGuid,
			int TrialResult,
			string TrialComments
		)
		{
			return DBHelper.RunSPReturnInteger("TRIAL_REQUEST_ADD",
				DBHelper.mp("@RequestID",SqlDbType.Int,0),
				DBHelper.mp("@CompanyName",SqlDbType.NVarChar,250,PortalName),
				DBHelper.mp("@SizeOfGroup",SqlDbType.NVarChar,50,SizeOfGroup),
				DBHelper.mp("@Description",SqlDbType.NText,Description),
				DBHelper.mp("@Domain",SqlDbType.NVarChar,255,Domain),
				DBHelper.mp("@FirstName",SqlDbType.NVarChar,100,FirstName),
				DBHelper.mp("@LastName",SqlDbType.NVarChar,100,LastName),
				DBHelper.mp("@Email",SqlDbType.NVarChar,100,Email),
				DBHelper.mp("@Phone",SqlDbType.NVarChar,100,Phone),
				DBHelper.mp("@Country",SqlDbType.NVarChar,100,Country),
				DBHelper.mp("@TimeZone",SqlDbType.NVarChar,150,TimeZone),
				DBHelper.mp("@Login",SqlDbType.NVarChar,50,Login),
				DBHelper.mp("@Password",SqlDbType.NVarChar,50,Password),
				DBHelper.mp("@ResellerGUID",SqlDbType.UniqueIdentifier,ResellerGuid),
				DBHelper.mp("@TrialResult",SqlDbType.Int,TrialResult),
				DBHelper.mp("@TrialResultComments",SqlDbType.NText,TrialComments)
			);
		}
	}
}
