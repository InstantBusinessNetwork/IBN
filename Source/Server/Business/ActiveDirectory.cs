using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.DirectoryServices;
using System.Net;

using Mediachase.Ibn;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business
{
	#region class IPAddressRange
	[Serializable]
	public class InvalidRangeException : System.Exception
	{
		public InvalidRangeException()
			: this(null)
		{
		}
		public InvalidRangeException(Exception  innerException)
			: base("Invalid range.", innerException)
		{
		}
	}
	#endregion

	#region class IPAddressRange
	public class IPAddressRange
	{
		#region * Fields *
		
		private int m_HashCode;
		private int m_ID = -1;
		private IPAddress m_StartAddress;
		private IPAddress m_EndAddress;
		
		#endregion

		#region * Properties *

		public int ID{ get{ return m_ID; } }
		public IPAddress StartAddress{ get{ return m_StartAddress; } }
		public IPAddress EndAddress{ get{ return m_EndAddress; } }
		
		#endregion

		public IPAddressRange(string startAddress, string endAddress)
		{
			m_StartAddress = IPAddress.Parse(startAddress);
			m_EndAddress = IPAddress.Parse(endAddress);

			if(CompareIP(m_StartAddress, m_EndAddress) > 0)
				throw new InvalidRangeException();

			m_HashCode = string.Format("{0}{1}", m_StartAddress, m_EndAddress).GetHashCode();
		}

		public IPAddressRange(int rangeId, string startAddress, string endAddress)
			: this(startAddress, endAddress)
		{
			if(rangeId < 0)
				throw new InvalidRangeException();
			m_ID = rangeId;
		}

		public bool Contains(IPAddress address)
		{
			return (CompareIP(address, StartAddress) >= 0 && CompareIP(address, EndAddress) <= 0);
		}

		override public int GetHashCode()
		{
			return m_HashCode;
		}

		override public bool Equals(object obj)
		{
			IPAddressRange range = obj as IPAddressRange;
			if(range != null)
				return (this.m_HashCode == range.m_HashCode);
			else
				return false;
		}

		#region CompareIP()
		private static int CompareIP(IPAddress ip1, IPAddress ip2)
		{
			int ret = 0;

			byte[] bytes1 = ip1.GetAddressBytes();
			byte[] bytes2 = ip2.GetAddressBytes();
			int len1 = bytes1.Length;
			int len2 = bytes2.Length;

			if(len1 > len2)
				ret = 1;
			else if(len1 < len2)
				ret = -1;
			else
			{
				for(int i=0; i<len1; i++)
				{
					byte b1 = bytes1[i];
					byte b2 = bytes2[i];
					
					if(b1 > b2)
					{
						ret = 1;
						break;
					}
					else if(b1 < b2)
					{
						ret = -1;
						break;
					}
				}
			}
			return ret;
		}
		#endregion
	}
	#endregion

	public class ActiveDirectory
	{
		#region * Variables *

		string m_domain;
		string m_username;
		string m_password;
		string m_filter;
		NameValueCollection m_fields = new NameValueCollection();

		#endregion

		#region * Properties *

		public NameValueCollection FieldsMatch{ get{ return m_fields; } }
		
		#endregion

		#region CheckLogin()
		public static bool CheckLogin(string domain, string username, string password)
		{
			bool ret = false;
			try
			{
				using(DirectoryEntry de = new DirectoryEntry(string.Format("LDAP://{0}", domain), username, password))
				{
					de.RefreshCache();
					ret = true;
				}
			}
			catch{}
			return ret;
		}
		#endregion

		#region * Constructor *

		public ActiveDirectory(string domain, string user, string password, string filter)
		{
			if (!User.CanImportFromActiveDirectory())
				throw new AccessDeniedException();

			m_domain = domain;
			m_username = user;
			m_password = password;
			m_filter = filter;

			m_fields[UserInfo.IbnProperty.Login.ToString()] = UserInfo.AdProperty.SamAccountName.ToString();
			m_fields[UserInfo.IbnProperty.FirstName.ToString()] = UserInfo.AdProperty.GivenName.ToString();
			m_fields[UserInfo.IbnProperty.LastName.ToString()] = UserInfo.AdProperty.Sn.ToString();
			m_fields[UserInfo.IbnProperty.Email.ToString()] = UserInfo.AdProperty.Mail.ToString();
			m_fields[UserInfo.IbnProperty.LdapUid.ToString()] = UserInfo.AdProperty.ObjectGuid.ToString();
			m_fields[UserInfo.IbnProperty.WindowsLogin.ToString()] = UserInfo.AdProperty.SamAccountName.ToString();
			m_fields[UserInfo.IbnProperty.Phone.ToString()] = UserInfo.AdProperty.TelephoneNumber.ToString();
			m_fields[UserInfo.IbnProperty.Mobile.ToString()] = UserInfo.AdProperty.Mobile.ToString();
			m_fields[UserInfo.IbnProperty.Fax.ToString()] = UserInfo.AdProperty.FacsimileTelephoneNumber.ToString();
			m_fields[UserInfo.IbnProperty.Location.ToString()] = UserInfo.AdProperty.L.ToString();
			m_fields[UserInfo.IbnProperty.Company.ToString()] = UserInfo.AdProperty.Company.ToString();
			m_fields[UserInfo.IbnProperty.Department.ToString()] = UserInfo.AdProperty.Department.ToString();
			m_fields[UserInfo.IbnProperty.JobTitle.ToString()] = UserInfo.AdProperty.Title.ToString();
		}
		
		#endregion

		#region GetProperties()
		public string[] GetProperties()
		{
			return Enum.GetNames(typeof(UserInfo.AdProperty));;
		}
		#endregion

		#region GetUsers()
		public ArrayList GetUsers()
		{
			ArrayList ret = new ArrayList();
			
			DirectoryEntry root = new DirectoryEntry(string.Format("LDAP://{0}", m_domain), m_username, m_password);
			DirectorySearcher s = new DirectorySearcher(root, m_filter);

			string ldapName;
			string[] ibnNames = UserInfo.PropertyNamesIbnAll;

			foreach(SearchResult result in s.FindAll())
			{
				UserInfo ui = new UserInfo();
				foreach(string ibnName in ibnNames)
				{
					if(ibnName == UserInfo.IbnProperty.WindowsLogin.ToString())
						ldapName = UserInfo.AdProperty.SamAccountName.ToString();
					else
						ldapName = m_fields[ibnName];

					string sVal = Ldap.GetPropertyValue(result, ldapName);

					if(ibnName == UserInfo.IbnProperty.WindowsLogin.ToString())
						sVal = string.Format("{0}\\{1}", m_domain, sVal).ToLower();

					ui[ibnName] = sVal;
				}
				ret.Add(ui);
			}
			return ret;
		}
		#endregion


		#region *** Local address ranges ***

		// Public
		#region GetLocalAddressRanges()
		public static IDataReader GetLocalAddressRanges()
		{
			CheckAccess();

			return DbActiveDirectory.LocalAddressRangeGetList();
		}
		#endregion
		#region AddLocalAddressRange()
		public static int AddLocalAddressRange(string startAddress, string endAddress)
		{
			CheckAccess();

			int id;
			// Validate range
			IPAddressRange range = new IPAddressRange(startAddress, endAddress);

			// Add range
			using(DbTransaction tran = DbTransaction.Begin())
			{
				id = DbActiveDirectory.LocalAddressRangeAdd(startAddress, endAddress);
				tran.Commit();
			}
			return id;
		}
		#endregion
		#region DeleteLocalAddressRange()
		public static void DeleteLocalAddressRange(int rangeId)
		{
			CheckAccess();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbActiveDirectory.LocalAddressRangeDelete(rangeId);
				tran.Commit();
			}
		}
		#endregion
		#region UpdateLocalAddressRange()
		public static void UpdateLocalAddressRange(int rangeId, string startAddress, string endAddress)
		{
			CheckAccess();

			// Validate range
			IPAddressRange range = new IPAddressRange(rangeId, startAddress, endAddress);

			// Update range
			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbActiveDirectory.LocalAddressRangeUpdate(rangeId, startAddress, endAddress);
				tran.Commit();
			}
		}
		#endregion
		#region UpdateLocalAddressRanges()
		public static void UpdateLocalAddressRanges(DataTable items)
		{
			CheckAccess();

			ArrayList oldItems = GetLocalAddressesList();

			ArrayList add = new ArrayList();
			ArrayList del = new ArrayList();
			foreach(DataRow row in items.Rows)
			{
				IPAddressRange item = new IPAddressRange((string)row["StartAddress"], (string)row["EndAddress"]);
				if(oldItems.Contains(item))
					oldItems.Remove(item);
				else
					add.Add(item);
			}

			del.AddRange(oldItems);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				foreach(IPAddressRange range in add)
				{
					DbActiveDirectory.LocalAddressRangeAdd(range.StartAddress.ToString(), range.EndAddress.ToString());
				}

				foreach(IPAddressRange range in del)
				{
					DbActiveDirectory.LocalAddressRangeDelete(range.ID);
				}

				tran.Commit();
			}
		}
		#endregion

		// Internal
		#region IsLocalAddress()
		internal static bool IsLocalAddress(string address)
		{
			bool ret = false;

			IPAddress ip = IPAddress.Parse(address);
			ArrayList list = GetLocalAddressesList();
			foreach(IPAddressRange range in list)
			{
				if(range.Contains(ip))
				{
					ret = true;
					break;
				}
			}

			return ret;
		}
		#endregion

		// Private
		#region CheckAccess()
		private static void CheckAccess()
		{
			if(!Security.IsUserInGroup(InternalSecureGroups.Administrator))
				throw new AccessDeniedException();
		}
		#endregion
		#region GetLocalAddressesList()
		private static ArrayList GetLocalAddressesList()
		{
			ArrayList list = new ArrayList();

			using(IDataReader reader = DbActiveDirectory.LocalAddressRangeGetList())
			{
				while(reader.Read())
				{
					list.Add(new IPAddressRange((int)reader["RangeId"], reader["StartAddress"].ToString(), reader["EndAddress"].ToString()));
				}
			}

			return list;
		}
		#endregion

		#endregion
	}
}
