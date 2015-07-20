using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web
{
	public partial class Search : System.Web.UI.Page
	{
		private string sSID = "";
		private string sFN = "";
		private string sLN = "";
		private string sEM = "";
		private int iUserId = -1;

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
			if (Request["sid"] != null)
				sSID = Request["sid"].ToString();
			if (Request["fn"] != null)
				sFN = Request["fn"].ToString();
			if (Request["ln"] != null)
				sLN = Request["ln"].ToString();
			if (Request["em"] != null)
				sEM = Request["em"].ToString();

			try
			{
				iUserId = Root.CheckActiveUser(sSID);
			}
			catch (Exception)
			{
				Finish();
			}
		} 
		#endregion

		#region Content
		private string Content()
		{
			string ret = string.Empty;
			try
			{
				StringBuilder sb = new StringBuilder();
				StringWriter stringWriter = new StringWriter(sb);
				XmlTextWriter writer = new XmlTextWriter(stringWriter);

				writer.WriteStartElement("users");
				using (IDataReader reader = Root.SearchUsers("", sFN, sLN, sEM))
				{
					while (reader.Read())
					{
						writer.WriteStartElement("user");
						writer.WriteAttributeString("user_id", reader["user_id"].ToString());
						writer.WriteAttributeString("login", reader["login"].ToString());
						writer.WriteAttributeString("first_name", reader["first_name"].ToString());
						writer.WriteAttributeString("last_name", reader["last_name"].ToString());
						writer.WriteAttributeString("email", reader["email"].ToString());
						writer.WriteEndElement();//user
					}
				}
				writer.WriteEndElement();//users

				ret = sb.ToString();
			}
			catch (Exception)
			{
				Finish();
			}
			return ret;
		} 
		#endregion

		#region Finish
		private void Finish()
		{
			Response.Clear();
			Response.Write("<users/>");
			Response.End();
		} 
		#endregion
	}
}
