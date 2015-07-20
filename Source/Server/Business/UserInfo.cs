using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Mediachase.IBN.Business
{
	public class UserInfo
	{
		#region * Enums *
		public enum IbnProperty
		{
			Login,
			IsActive,
			FirstName,
			LastName,
			Email,
			WindowsLogin,
			LdapUid,
			Phone,
			Mobile,
			Fax,
			Location,
			Company,
			Department,
			JobTitle,
		}

		public enum AdProperty
		{
			Company,
			Department,
			FacsimileTelephoneNumber,
			GivenName,
			L,
			Mail,
			Mobile,
			ObjectGuid,
			ObjectSid,
			SamAccountName,
			Sn,
			TelephoneNumber,
			Title,
			UserAccountControl,
			UserPrincipalName,
		}
		#endregion

		public int UserId;
		public int OriginalId;
		public bool BadLogin;
		public bool BadEmail;
		public int ImGroupId;
		public ArrayList Groups;
		public NameValueCollection UpdatedValues = new NameValueCollection();

		/// <summary>
		/// Returns property names excluding IsActive and WindowsLogin.
		/// </summary>
		public static string[] PropertyNamesIbn
		{
			get
			{
				ArrayList list = new ArrayList(PropertyNamesIbnAll);
				list.Remove(UserInfo.IbnProperty.IsActive.ToString());
				//list.Remove(UserInfo.IbnProperty.WindowsLogin.ToString());
				return ((string[])list.ToArray(typeof(string)));
			}
		}
		/// <summary>
		/// Returns all IBN property names.
		/// </summary>
		public static string[] PropertyNamesIbnAll{ get{ return Enum.GetNames(typeof(IbnProperty)); } }

		/// <summary>
		/// Returns all AD property names.
		/// </summary>
		public static string[] PropertyNamesAdAll{ get{ return Enum.GetNames(typeof(AdProperty)); } }

		private NameValueCollection m_Values = new NameValueCollection();

		#region * Properties *
		public string Login
		{
			get{ return this[UserInfo.IbnProperty.Login.ToString()]; }
			set{ this[UserInfo.IbnProperty.Login.ToString()] = value; }
		}
		public string IsActive
		{
			get{ return this[UserInfo.IbnProperty.IsActive.ToString()]; }
			set{ this[UserInfo.IbnProperty.IsActive.ToString()] = value; }
		}
		public string FirstName
		{
			get{ return this[UserInfo.IbnProperty.FirstName.ToString()]; }
			set{ this[UserInfo.IbnProperty.FirstName.ToString()] = value; }
		}
		public string LastName
		{
			get{ return this[UserInfo.IbnProperty.LastName.ToString()]; }
			set{ this[UserInfo.IbnProperty.LastName.ToString()] = value; }
		}
		public string Email
		{
			get{ return this[UserInfo.IbnProperty.Email.ToString()]; }
			set{ this[UserInfo.IbnProperty.Email.ToString()] = value; }
		}
		public string WindowsLogin
		{
			get{ return this[UserInfo.IbnProperty.WindowsLogin.ToString()]; }
			set{ this[UserInfo.IbnProperty.WindowsLogin.ToString()] = value; }
		}
		public string LdapUid
		{
			get{ return this[UserInfo.IbnProperty.LdapUid.ToString()]; }
			set{ this[UserInfo.IbnProperty.LdapUid.ToString()] = value; }
		}
		public string Phone
		{
			get{ return this[UserInfo.IbnProperty.Phone.ToString()]; }
			set{ this[UserInfo.IbnProperty.Phone.ToString()] = value; }
		}
		public string Fax
		{
			get{ return this[UserInfo.IbnProperty.Fax.ToString()]; }
			set{ this[UserInfo.IbnProperty.Fax.ToString()] = value; }
		}
		public string Mobile
		{
			get{ return this[UserInfo.IbnProperty.Mobile.ToString()]; }
			set{ this[UserInfo.IbnProperty.Mobile.ToString()] = value; }
		}
		public string Company
		{
			get{ return this[UserInfo.IbnProperty.Company.ToString()]; }
			set{ this[UserInfo.IbnProperty.Company.ToString()] = value; }
		}
		public string JobTitle
		{
			get{ return this[UserInfo.IbnProperty.JobTitle.ToString()]; }
			set{ this[UserInfo.IbnProperty.JobTitle.ToString()] = value; }
		}
		public string Department
		{
			get{ return this[UserInfo.IbnProperty.Department.ToString()]; }
			set{ this[UserInfo.IbnProperty.Department.ToString()] = value; }
		}
		public string Location
		{
			get{ return this[UserInfo.IbnProperty.Location.ToString()]; }
			set{ this[UserInfo.IbnProperty.Location.ToString()] = value; }
		}
		#endregion

		#region indexer
		public string this[string name]
		{
			get
			{
				if(!Enum.IsDefined(typeof(IbnProperty), name))
					throw new ArgumentException("Property does not exist.", name);
				return m_Values[name];
			}
			set
			{
				if(!Enum.IsDefined(typeof(IbnProperty), name))
					throw new ArgumentException("Property does not exist.", name);
				m_Values[name] = value;
			}
		}
		#endregion

		#region ImportXml()
		public static UserInfo[] ImportXml(XmlDocument doc)
		{
			ArrayList ret = new ArrayList();
			UserInfo ui;
			string sVal;
			XmlNode prop, val;
			string[] propertyNames = UserInfo.PropertyNamesIbnAll;

			foreach(XmlNode user in doc.SelectNodes("/users/user"))
			{
				ui = new UserInfo();
				foreach(string name in propertyNames)
				{
					sVal = string.Empty;

					prop = user.SelectSingleNode(string.Format("prop[@name='{0}']", name));
					if(prop != null)
					{
						val = prop.SelectSingleNode("val");
						if(val != null)
							sVal = val.InnerXml;
					}
					ui[name] = sVal;
				}
				ret.Add(ui);
			}
			return (UserInfo[])ret.ToArray(typeof(UserInfo));
		}
		#endregion

		#region ImportTxt()
		public static UserInfo[] ImportTxt(TextReader doc)
		{
			ArrayList ret = new ArrayList();
			UserInfo ui;
			string separator, valsLine, propsLine, name;
			string[] aVals, aNames;
			string[] propertyNames = UserInfo.PropertyNamesIbnAll;
			Hashtable props = new Hashtable();

			separator = doc.ReadLine();
			propsLine = doc.ReadLine();
			aNames = Regex.Split(propsLine, separator);
			for(int i=0; i<aNames.Length; i++)
				props[i] = aNames[i];

			while((valsLine = doc.ReadLine()) != null)
			{
				aVals = Regex.Split(valsLine, separator);

				if(aVals.Length != aNames.Length)
					throw new Exception("Invalid file format.");

				ui = new UserInfo();
				for(int i=0; i<aVals.Length; i++)
				{
					name = (string)props[i];
					ui[name] = aVals[i];
				}
				ret.Add(ui);
			}
			return (UserInfo[])ret.ToArray(typeof(UserInfo));
		}
		#endregion

		#region Load()
		public void Load(IDataReader reader)
		{
			UserId = (int)reader["UserId"];
			OriginalId = (int)reader["OriginalId"];
			foreach(string name in PropertyNamesIbnAll)
				this[name] = reader[name].ToString();
		}
		#endregion
	}
}
