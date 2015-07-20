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
	public partial class Chat_Details : System.Web.UI.Page
	{
		private string sSID = "";
		private string sCUID = "";
		private int iChatID = -1;

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
				sSID = Request["sid"];
				sCUID = Request["id"];

				if (sSID.Length <= 0 || sCUID.Length <= 0)
					Finish();

				// Checking User
				int iUserId = Root.CheckActiveUser(sSID);
				
				if (iUserId <= 0)
					Finish();

				iChatID = Root.GetChatIdByCUID(sCUID);
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
			Response.Write("<Package/>");
			Response.End();
		} 
		#endregion

		#region Content
		private string Content()
		{
			StringBuilder sb = new StringBuilder();
			StringWriter stringWriter = new StringWriter(sb);
			XmlTextWriter writer = new XmlTextWriter(stringWriter);

			writer.WriteStartElement("Package");
			using (IDataReader reader = Root.GetChatDetails(iChatID))
			{
				if (reader.Read())
				{
					writer.WriteStartElement("Chats");
					writer.WriteStartElement("Chat");
					writer.WriteElementString("chat_id", reader["chat_id"].ToString());
					writer.WriteElementString("name", reader["name"].ToString());
					writer.WriteElementString("desc", reader["desc"].ToString());
					writer.WriteElementString("begin_time", reader["begin_time"].ToString());
					writer.WriteElementString("owner_id", reader["owner_id"].ToString());
					writer.WriteElementString("first_name", reader["first_name"].ToString());
					writer.WriteElementString("last_name", reader["last_name"].ToString());
					writer.WriteElementString("mess_count", reader["mess_count"].ToString());
					reader.Close();
					writer.WriteEndElement();//chat
					writer.WriteEndElement();//chats
				}
				else
				{
					writer.WriteEndElement();//package
					return sb.ToString();
				}
			}


			writer.WriteStartElement("Users");
			using (IDataReader reader = Root.GetChatUsers(iChatID))
			{
				while (reader.Read())
				{
					writer.WriteStartElement("User");
					writer.WriteElementString("user_id", reader["user_id"].ToString());
					writer.WriteElementString("accepted", reader["accepted"].ToString());
					writer.WriteElementString("exited", reader["exited"].ToString());
					writer.WriteElementString("user_status", reader["user_status"].ToString());
					writer.WriteElementString("first_name", reader["first_name"].ToString());
					writer.WriteElementString("last_name", reader["last_name"].ToString());
					writer.WriteEndElement();//user
				}
			}
			writer.WriteEndElement();//users

			writer.WriteEndElement();//package

			return sb.ToString();
		} 
		#endregion
	}
}