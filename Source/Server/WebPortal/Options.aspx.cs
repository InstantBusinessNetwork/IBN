using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.IBN.Business;
using System.Data;
using System.Globalization;
using System.Text;
using System.IO;
using System.Xml;

namespace Mediachase.UI.Web
{
	public partial class Options : System.Web.UI.Page
	{
		private int iBuild = -1;

		private int iStubsVersion = -1;
		private int iLogoVersion = -1;
		private string sSID = "";
		private int currentUserID = -1;
		private string sProdGUID = "";

		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Charset = "UTF-8";
			Response.ContentType = "text/xml";
			Response.Cache.SetExpires(DateTime.Now);

			LoadRequestVariables();
			CheckUser();
			Response.Write(Content());
		}

		#region LoadRequestVariables
		private void LoadRequestVariables()
		{
			if (Request["build"] != null)
			{
				try
				{
					iBuild = Int32.Parse(Request["build"]);
				}
				catch (Exception) { }
			}

			if (Request["s_version"] != null)
			{
				try
				{
					iStubsVersion = Int32.Parse(Request["s_version"]);
				}
				catch (Exception) { }
			}

			if (Request["l_version"] != null)
			{
				try
				{
					iLogoVersion = Int32.Parse(Request["l_version"]);
				}
				catch (Exception) { }
			}

			if (Request["sid"] != null)
				sSID = Request["sid"];

			if (Request["productguid"] != null)
				sProdGUID = Request["productguid"];
		} 
		#endregion

		#region CheckUser
		private void CheckUser()
		{
			int iUserId = -1;

			try
			{
				iUserId = Root.CheckActiveUser(sSID);
				if (iUserId <= 0)
					throw new Exception("IBN Root: CheckUser Exception: No active users.");
				currentUserID = iUserId;
			}
			catch
			{
				Response.Write("<options/>");
				Response.End();
			}
		} 
		#endregion

		#region Content
		private string Content()
		{
			int iStubID;
			int iCurrentStubVersion = Root.GetStubsVersion(currentUserID);
			StringBuilder sb = new StringBuilder();
			StringWriter stringWriter = new StringWriter(sb);
			XmlTextWriter writer = new XmlTextWriter(stringWriter);

			writer.WriteStartElement("options");
			writer.WriteAttributeString("xmlns", "dt", null, "urn:schemas-microsoft-com:datatypes");

			// Stubs
			int iGroupId = Root.GetGroupByUser(currentUserID);
			if (iStubsVersion != iCurrentStubVersion && iCurrentStubVersion >= 0)
			{
				writer.WriteStartElement("stubs");
				writer.WriteAttributeString("version", iCurrentStubVersion.ToString());
				using (IDataReader reader = Root.GetAllStubsForUser(currentUserID))
				{
					while (reader.Read())
					{
						writer.WriteStartElement("stub");

						iStubID = Convert.ToInt32(reader["StubId"]);
						writer.WriteElementString("stub_id", iStubID.ToString());
						writer.WriteElementString("stub_name", reader["Abbreviation"].ToString());
						writer.WriteElementString("tooltip", reader["ToolTip"].ToString());
						writer.WriteElementString("url", reader["Url"].ToString());
						writer.WriteElementString("open_window", reader["OpenInBrowser"].ToString());

						byte[] bit_icon = null;
						using (IDataReader reader_for_BLOB = Root.GetBinaryStubIcon(iStubID))
						{
							if (reader_for_BLOB.Read())
							{
								try
								{
									bit_icon = (byte[])reader_for_BLOB["Icon"];
								}
								catch { }
							}
						}
						if (bit_icon != null)
						{
							writer.WriteStartElement("icon");
							writer.WriteAttributeString("dt:dt", "bin.base64");
							writer.WriteBase64(bit_icon, 0, bit_icon.Length);
							writer.WriteEndElement(); //icon
						}
						else
							writer.WriteElementString("icon", "");

						writer.WriteEndElement();//stub
					}
				}
				writer.WriteEndElement();//stubs
			}

			// Versions
			int iMaxBuild = -1;// CManage.GetMaxBuild(sProdGUID);
			if (iMaxBuild > iBuild)
			{
				writer.WriteStartElement("versions");
				writer.WriteAttributeString("latest", iMaxBuild.ToString());
				writer.WriteAttributeString("url", "Download/IBN Client.msi");
				writer.WriteAttributeString("mcupdate", "Download/McUpdate.exe#1.1.25.0");
				//DateTime dt;
				//using (IDataReader reader = CManage.GetHightVersions(iBuild, sProdGUID))
				//{
				//    while (reader.Read())
				//    {
				//        writer.WriteStartElement("version");
				//        writer.WriteAttributeString("build", reader["build"].ToString());
				//        dt = Convert.ToDateTime(reader["date"].ToString());
				//        writer.WriteAttributeString("date", dt.ToString("d", DateTimeFormatInfo.InvariantInfo));
				//        writer.WriteAttributeString("description", reader["description"].ToString());
				//        writer.WriteEndElement(); //version
				//    }
				//}
				writer.WriteEndElement(); //versions
			}

			// Logos
			using (IDataReader reader = Root.GetLogoByGroup(iGroupId))
			{
				if (reader.Read())
				{
					int iLogoGroupVersion = Convert.ToInt32(reader["logo_version"]);

					// Write color and encoded image
					if (iLogoVersion != iLogoGroupVersion)
					{
						try
						{
							string version = iLogoGroupVersion.ToString();
							string color = reader["color"].ToString();

							byte[] bit_logo = null;
							using (IDataReader reader_for_BLOB = Root.GetBinaryClientLogo(iGroupId))
							{
								try
								{
									if (reader_for_BLOB.Read())
										bit_logo = (byte[])reader_for_BLOB["client_logo"];
								}
								catch { }
							}

							writer.WriteStartElement("logos");
							writer.WriteAttributeString("version", version);
							writer.WriteStartElement("logo");
							writer.WriteElementString("color", color);

							if (bit_logo != null)
							{
								writer.WriteStartElement("client_logo");
								writer.WriteAttributeString("dt:dt", "bin.base64");
								writer.WriteBase64(bit_logo, 0, bit_logo.Length);
								writer.WriteEndElement(); // client_logo
							}

							writer.WriteEndElement();	// logo
							writer.WriteEndElement();	// logos
						}
						catch { }
					}
				}
			}

			writer.WriteEndElement();	// options
			return sb.ToString();
		}
		#endregion
	}
}
