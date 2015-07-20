using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Xml;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail; 
using System.Web.Services.Protocols;

namespace Mediachase.UI.Web.WebServices
{
	/// <summary>
	/// Summary description for IncidentInfo.
	/// </summary>
	[WebService(Namespace="http://mediachase.com/webservices/")]
	public class IncidentInfo : System.Web.Services.WebService
	{
		const int NotSetResponsibleId = -1;
		const int GroupResponsibleId = -2;

		const string NotSetResponsibleName = "<not set>";
		const string GroupResponsibleName = "Group Responsibility";

		public IncidentInfo()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

/*
  // Build the detail element of the SOAP fault.
   System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
   System.Xml.XmlNode node = doc.CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);
   System.Xml.XmlNode details = doc.CreateNode(XmlNodeType.Element, "alertId", string.Empty);
   details.InnerText = ex.AlertId.ToString();
   node.AppendChild(details);
   throw new SoapException(string.Format("{0} Method: {1} ( {2} ).",ex.Message,Method,Params),new XmlQualifiedName(ex.GetType().FullName),System.Web.HttpContext.Current.Request.Url.AbsoluteUri,node);
*/
		[WebMethod]
		public string GetUsers(string UID)
		{
			XmlDocument	xmlDocument = new XmlDocument();
			try
			{
				IncidentUserTicket ticket = IncidentUserTicket.Load(new Guid(UID));
				if (ticket == null)
				{
					System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
					System.Xml.XmlNode node = doc.CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);
					System.Xml.XmlNode resource = doc.CreateNode(XmlNodeType.Element, "resource", string.Empty);
					resource.InnerText = "strNotfound";
					node.AppendChild(resource);
					throw new SoapException(String.Format("The ticket '{0}' not found", UID), new XmlQualifiedName(typeof(ArgumentException).FullName), System.Web.HttpContext.Current.Request.Url.AbsoluteUri, node);
				}

				Security.UserLoginByTicket(ticket);

				xmlDocument.LoadXml("<Users></Users>");
				XmlNode xmlRootNode = xmlDocument.SelectSingleNode("Users");

				DataView dataView = Mediachase.IBN.Business.User.GetListActiveDataTable(string.Empty).DefaultView;

				foreach (DataRow row in dataView.Table.Rows)
				{
					/// Row includes fields:
					///	PrincipalId, Login, Password, FirstName, LastName, Email, IMGroupId, OriginalId, 
					/// DisplayName, Department
					XmlElement userNode = xmlDocument.CreateElement("User");
					xmlRootNode.AppendChild(userNode);

					userNode.SetAttribute("id", row["PrincipalId"].ToString());
					userNode.SetAttribute("fn", row["FirstName"].ToString());
					userNode.SetAttribute("ln", row["LastName"].ToString());
					userNode.SetAttribute("email", row["Email"].ToString());
					userNode.SetAttribute("department", row["Department"].ToString());
				}

			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
				throw;
			}
			return xmlDocument.InnerXml;		
		}

		[WebMethod]
		public string GetIncidentInfo(string UID)
		{
			XmlDocument	xmlIncident = new XmlDocument();
			try
			{
				IncidentUserTicket ticket = IncidentUserTicket.Load(new Guid(UID));
				if (ticket==null)
				{
					System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
					System.Xml.XmlNode node = doc.CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);
					System.Xml.XmlNode resource = doc.CreateNode(XmlNodeType.Element, "resource", string.Empty);
					resource.InnerText = "strNotfound";
					node.AppendChild(resource);
					throw new SoapException(String.Format("The ticket '{0}' not found", UID), new XmlQualifiedName(typeof(ArgumentException).FullName),System.Web.HttpContext.Current.Request.Url.AbsoluteUri,node);
				}

				Security.UserLoginByTicket(ticket);

				xmlIncident.LoadXml("<Incident></Incident>");
		
				XmlNode xmlRootNode = xmlIncident.SelectSingleNode("Incident");

				XmlElement NameNode = xmlIncident.CreateElement("Name");
				XmlElement TypeNode = xmlIncident.CreateElement("Type");
				XmlElement DescriptionNode = xmlIncident.CreateElement("Description");
				//XmlElement ManagerNode = xmlIncident.CreateElement("Manager");
				XmlElement ClientNode = xmlIncident.CreateElement("Client");
				XmlElement CreatedNode = xmlIncident.CreateElement("Created");
				XmlElement StateNode = xmlIncident.CreateElement("State");
				XmlElement StateIdNode = xmlIncident.CreateElement("StateId");
				XmlElement ColorNode = xmlIncident.CreateElement("StateColor");

				int controllerId = -1;
				int IssBoxId = -1;

				using (IDataReader reader = Incident.GetIncident(ticket.IncidentId))
				{
					if (reader.Read())
					{
						if (reader["ControllerId"] != DBNull.Value)
							controllerId = (int)reader["ControllerId"];
						IssBoxId = (int)reader["IncidentBoxId"];

						NameNode.InnerText = (string)reader["Title"];
						TypeNode.InnerText = (string)reader["TypeName"];
						DescriptionNode.InnerText = (string)reader["Description"];
						//ManagerNode.InnerText = reader["Title"];
						ClientNode.InnerText = (string)reader["ClientName"];
						CreatedNode.InnerText = ((DateTime)reader["CreationDate"]).ToString("s");
						StateNode.InnerText = (string)reader["StateName"];
						StateIdNode.InnerText = reader["StateId"].ToString();
						ColorNode.InnerText = (Util.CommonHelper.GetStateColor((int)reader["StateId"])).ToArgb().ToString();

						xmlRootNode.AppendChild(NameNode);
						xmlRootNode.AppendChild(TypeNode);
						xmlRootNode.AppendChild(DescriptionNode);
						xmlRootNode.AppendChild(ClientNode);
						xmlRootNode.AppendChild(CreatedNode);
						xmlRootNode.AppendChild(StateNode);
						xmlRootNode.AppendChild(StateIdNode);
						xmlRootNode.AppendChild(ColorNode);
					}
				}

				int stateId = 0;
				int userId = -1;
				bool isGroup = false;

				using(IDataReader reader = Incident.GetIncidentTrackingState(ticket.IncidentId))
				{
					if(reader.Read())
					{
						stateId = (int)reader["StateId"];
						userId = (int)reader["ResponsibleId"];
						if(reader["IsResponsibleGroup"]!=DBNull.Value && (bool)reader["IsResponsibleGroup"])
						{
							isGroup = true;
						}
					}
				}


				if (stateId==7)
				{
					isGroup = false;
					userId = controllerId;
				}

				if (isGroup)
				{
					XmlElement responsibleNode = xmlIncident.CreateElement("GroupResponsible");
					XmlElement responsibleIdNode = xmlIncident.CreateElement("ResponsibleId");

					responsibleNode.InnerText = "1";
					responsibleIdNode.InnerText = IncidentInfo.GroupResponsibleId.ToString();

					xmlRootNode.AppendChild(responsibleNode);
					xmlRootNode.AppendChild(responsibleIdNode);
				}
				else if (userId != -1)
				{
					XmlElement responsibleNode = xmlIncident.CreateElement("Responsible");
					XmlElement responsibleIdNode = xmlIncident.CreateElement("ResponsibleId");

					string UserName = Mediachase.IBN.Business.User.GetUserName(userId);

					responsibleNode.InnerText = UserName;
					responsibleIdNode.InnerText = userId.ToString();

					xmlRootNode.AppendChild(responsibleNode);
					xmlRootNode.AppendChild(responsibleIdNode);
				}
				else if (stateId != 1 && stateId != 4 && stateId != 5)
				{
					XmlElement responsibleNode = xmlIncident.CreateElement("Responsible");
					XmlElement responsibleIdNode = xmlIncident.CreateElement("ResponsibleId");

					responsibleNode.InnerText = NotSetResponsibleName;
					responsibleIdNode.InnerText = IncidentInfo.NotSetResponsibleId.ToString();

					xmlRootNode.AppendChild(responsibleNode);
					xmlRootNode.AppendChild(responsibleIdNode);
				}

				if (IssBoxId != -1)
				{
					IncidentBox ib = IncidentBox.Load(IssBoxId);

					XmlElement BoxNode = xmlIncident.CreateElement("IncidentBox");
					BoxNode.InnerText = ib.Name;

					xmlRootNode.AppendChild(BoxNode);
				}

				DataTable tbl = Incident.GetListIncidentStates(ticket.IncidentId);
				foreach (DataRow row in tbl.Rows)
				{
					if ((int)row["StateId"] != 1 &&  (int)row["StateId"] != 2)
					{
						XmlElement newStateNode = xmlIncident.CreateElement("NewState");
					
						XmlElement idNode = xmlIncident.CreateElement("Id");
						XmlElement nameNode = xmlIncident.CreateElement("Name");

						idNode.InnerText = row["StateId"].ToString();
						nameNode.InnerText = (string)row["StateName"];

						newStateNode.AppendChild(idNode);
						newStateNode.AppendChild(nameNode);

						xmlRootNode.AppendChild(newStateNode);
					}
				}

				// OZ: Add Available Responsible (...)
				Incident.Tracking trk = Incident.GetTrackingInfo(ticket.IncidentId);

				// Add No User Element (Custom ID)
				if (trk.CanSetNoUser)
				{
					AppendNewResponsible(xmlRootNode, NotSetResponsibleName, NotSetResponsibleId);
				}

				// Add Group Element (Custom ID)
				if (trk.CanSetGroup)
				{
					AppendNewResponsible(xmlRootNode, GroupResponsibleName, GroupResponsibleId);
				}

				// Add Users
				if (trk.CanSetUser)
				{
					ArrayList alUsers = Incident.GetResponsibleList(ticket.IncidentId);
					foreach (int iUserId in alUsers)
					{
						string userName = Mediachase.IBN.Business.User.GetUserName(iUserId);
						AppendNewResponsible(xmlRootNode, userName, iUserId);
					}
				}

				// Custom User  select ...
				// TODO:

				//
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);

				throw;
			}
			return xmlIncident.InnerXml;			
		}

		private void AppendNewResponsible(XmlNode xmlRootNode, string name, int id)
		{
			XmlNode xmlNewResponsible = xmlRootNode.OwnerDocument.CreateElement("NewResponsible");

			XmlNode xmlResponsibleName = xmlRootNode.OwnerDocument.CreateElement("Name");
			XmlNode xmlResponsibleId = xmlRootNode.OwnerDocument.CreateElement("Id");

			xmlResponsibleName.InnerText = name;
			xmlResponsibleId.InnerText = id.ToString();

			xmlNewResponsible.AppendChild(xmlResponsibleName);
			xmlNewResponsible.AppendChild(xmlResponsibleId);

			xmlRootNode.AppendChild(xmlNewResponsible);
		}
	}
}
