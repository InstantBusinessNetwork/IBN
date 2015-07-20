using System;
using System.Collections;
using System.Web;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for Test.
	/// </summary>
	public class Test
	{
		public Test()
		{
		}

		#region CreateUsers
		public static void CreateUsers(
			int GroupCount,
			int UserCount,
			string SecurityGroup,
			string GroupName,
			string Login,
			string Password,
			string FirstName,
			string LastName,
			string Email,
			int TimeZoneId,
			int LangID)
		{
			ArrayList SecGroups = new ArrayList();
			ArrayList groups = new ArrayList();
			ArrayList YouCanSee = new ArrayList();
			ArrayList CanSeeYou = new ArrayList();

			const string NUMBER = "[%=#=%]";

			if(-1 == GroupName.IndexOf(NUMBER))
				GroupName += NUMBER;
			if(-1 == Login.IndexOf(NUMBER))
				Login += NUMBER;
			if(-1 == FirstName.IndexOf(NUMBER))
				FirstName += NUMBER;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				int sg = SecureGroup.Create((int)InternalSecureGroups.Everyone, SecurityGroup);
				SecGroups.Add(sg);

				for(int g=1; g<=GroupCount; g++)
				{
					string gName = GroupName.Replace("[%=#=%]", g.ToString());
					int gim = IMGroup.Create(gName, "2B6087", false, null, YouCanSee, CanSeeYou);
					groups.Add(gim);

					for(int u=1; u<=UserCount; u++)
					{
						string su = (u+(g-1)*UserCount).ToString();
						
						string login = Login.Replace("[%=#=%]", su);
						string fName = FirstName.Replace("[%=#=%]", su);
						string lName = LastName.Replace("[%=#=%]", su);
						string em = Email.Replace("[%=#=%]", su);

						//						HttpContext.Current.Response.Write(su+"<br>");
						//						HttpContext.Current.Response.Flush();

						int uim = User.Create(login, Password, fName, lName, em, true,
							SecGroups, gim, "", "", "", "", "", "", "", TimeZoneId, LangID, "", null, -1);
					}
				}

				// Add groups visibility
				foreach(int g1 in groups)
				{
					foreach(int g2 in groups)
					{
						if(g1 != g2)
						{
							DBIMGroup.AddDependences(g1, g2);
							//							DBIMGroup.AddDependences(g2, g1);
						}
					}
				}

				//				throw new Exception("Test");
				tran.Commit();
			}
		}
		#endregion
	}
}
