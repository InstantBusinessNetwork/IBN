using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.IBN.Business;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;

namespace Mediachase.UI.Web
{
	public partial class Details : System.Web.UI.Page
	{
		private string sSID = "";
		private int iUserID = -1;
		private int iCurrentUserId = -1;

		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Charset = "UTF-8";
			Response.ContentType = "text/xml";
			Response.Cache.SetExpires(DateTime.Now);

			LoadRequestVariables();

			Response.Write(Content());
		}

		#region LoadRequestVariables
		private void LoadRequestVariables()
		{
			try
			{
				iUserID = Int32.Parse(Request["id"]);
				if (iUserID <= 0)
					Finish();

				if (Request["sid"] != null)
					sSID = Request["sid"];
				iCurrentUserId = Root.CheckActiveUser(sSID);

				if (iCurrentUserId <= 0)
					Finish();
			}
			catch (Exception)
			{
				Finish();
			}
		}
		#endregion

		#region Finish
		private void Finish()
		{
			Response.Clear();
			Response.Write("<UserDetails/>");
			Response.End();
		} 
		#endregion

		#region Content
		private string Content()
		{
			string ret;
			try
			{
				StringBuilder sb = new StringBuilder();
				StringWriter stringWriter = new StringWriter(sb);
				XmlTextWriter writer = new XmlTextWriter(stringWriter);

				writer.WriteStartElement("Package");
				writer.WriteStartElement("UserDetails");
				writer.WriteStartElement("User");
				writer.WriteAttributeString("user_id", iUserID.ToString());
				using (IDataReader reader = Root.GetUserInfo(iUserID, 1))
				{
					if (reader.Read())
					{
						writer.WriteAttributeString("name", reader["login"].ToString());
						writer.WriteAttributeString("first_name", reader["first_name"].ToString());
						writer.WriteAttributeString("last_name", reader["last_name"].ToString());
						writer.WriteAttributeString("eMail", reader["email"].ToString());
						int iGroupID = Convert.ToInt32(reader["imgroup_id"]);
						writer.WriteAttributeString("role_id", iGroupID.ToString()); //imgroup_id
						writer.WriteAttributeString("user_type", reader["imgroup_name"].ToString());
					}
				}
				writer.WriteEndElement();//user	
				writer.WriteEndElement();//userdetails

				writer.WriteStartElement("details");
				writer.WriteStartElement("detail");
				writer.WriteElementString("user_id", iUserID.ToString());

				using (IDataReader reader = Root.GetUserDetails(iUserID))
				{
					if (reader.Read())
					{
						writer.WriteElementString("phone", reader["phone"].ToString());
						writer.WriteElementString("fax", reader["fax"].ToString());
						writer.WriteElementString("mobile", reader["mobile"].ToString());
						writer.WriteElementString("position", reader["position"].ToString());
						writer.WriteElementString("department", reader["department"].ToString());
						writer.WriteElementString("location", reader["location"].ToString());
						string pic_url = reader["PictureURL"].ToString();
						bool bShowPic = false;
						if (pic_url != null && pic_url.Length > 0)
							bShowPic = true;
						writer.WriteElementString("show_picture", Convert.ToString(bShowPic));
					}
				}

				writer.WriteStartElement("SecureGroups");
				using (IDataReader reader = Root.GetUserGroups(iUserID))
				{
					while (reader.Read())
					{
						writer.WriteStartElement("Group");
						writer.WriteAttributeString("Id", ((int)reader["GroupId"]).ToString());
						writer.WriteAttributeString("Name", Mediachase.Ibn.Web.UI.CHelper.GetResFileString(reader["GroupName"].ToString()));
						writer.WriteEndElement();//Group
					}
				}
				writer.WriteEndElement();//SecureGroups

				writer.WriteEndElement();//detail
				writer.WriteEndElement();//details

				writer.WriteEndElement();//package

				return sb.ToString();
			}
			catch (Exception)
			{
				ret = "<UserDetails/>";
			}
			return ret;
		} 
		#endregion
	}
}
