using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Xml;

namespace Mediachase.UI.Web
{
	/// <summary>
	/// Summary description for InstallerClass.
	/// </summary>
	[RunInstaller(true)]
	public class InstallerClass : System.Configuration.Install.Installer
	{
		const string WebConfigPath = "portal\\web.config";

		#region Constructor & destructor
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public InstallerClass()
		{
			// This call is required by the Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		private XmlNode getSubReportNode(XmlNode reportNode, string name)
		{
			foreach (XmlNode node in reportNode.ChildNodes)
			{
				if (node.Name=="add" && node.Attributes["name"].Value==name) 
				{
					return node;
				}
			}
			return null;
		}

		private void addReportNode(XmlNode reportNode, string name, string url, string info)
		{
			addReportNode(reportNode, name, url, String.Empty, info);
		}

		private void addReportNode(XmlNode reportNode, string name, string url, string isPersonal, string info)
		{
			if (getSubReportNode(reportNode, name)!=null)
				return;

			XmlNode node = reportNode.OwnerDocument.CreateElement("add");

			XmlAttribute nameAttr = reportNode.OwnerDocument.CreateAttribute("name");
			nameAttr.Value = name;
			node.Attributes.Append(nameAttr);

			XmlAttribute urlAttr = reportNode.OwnerDocument.CreateAttribute("url");
			urlAttr.Value = url;
			node.Attributes.Append(urlAttr);

			if (isPersonal != String.Empty)
			{
				XmlAttribute perAttr = reportNode.OwnerDocument.CreateAttribute("isPersonal");
				perAttr.Value = isPersonal;
				node.Attributes.Append(perAttr);
			}

			XmlAttribute infoAttr = reportNode.OwnerDocument.CreateAttribute("infoType");
			infoAttr.Value = name;
			node.Attributes.Append(infoAttr);

			reportNode.AppendChild(node);
		}

		private void modifyWebConfig(string path)
		{
			XmlDocument doc = new XmlDocument();

			doc.Load(path);

			XmlNode userReportNode = doc.SelectSingleNode("configuration/userReports");

			addReportNode(userReportNode, "PortalQuickSnapshot", "/UserReports/PortalQuickSnapshot.aspx", "Mediachase.UI.Web.UserReports.Modules.PortalQuickSnapshotInfo, Mediachase.Ibn.DefaultUserReports");
			addReportNode(userReportNode, "MostActiveReport", "/UserReports/MostActiveReport.aspx", "Mediachase.UI.Web.UserReports.Modules.MostActiveReportInfo, Mediachase.Ibn.DefaultUserReports");
			addReportNode(userReportNode, "GroupAndUserImStat", "/UserReports/GroupAndUserImStat.aspx", "Mediachase.UI.Web.UserReports.Modules.GroupAndUserImStatInfo, Mediachase.Ibn.DefaultUserReports");
			addReportNode(userReportNode, "GroupAndUserStat", "/UserReports/GroupAndUserStat.aspx", "Mediachase.UI.Web.UserReports.Modules.GroupAndUserStatInfo, Mediachase.Ibn.DefaultUserReports");
			addReportNode(userReportNode, "ChatHistory_Admin", "/UserReports/chat_history.aspx?ReportType=Admin", "Mediachase.UI.Web.UserReports.Modules.ChatHistoryAdminInfo, Mediachase.Ibn.DefaultUserReports");
			addReportNode(userReportNode, "ChatHistory_Personal", "/UserReports/chat_history.aspx?ReportType=User", "true" , "Mediachase.UI.Web.UserReports.Modules.ChatHistoryInfo, Mediachase.Ibn.DefaultUserReports");
			addReportNode(userReportNode, "MessageHistory_Admin", "/UserReports/User_History.aspx?ReportType=Admin", "Mediachase.UI.Web.UserReports.Modules.MessageHistoryAdminInfo, Mediachase.Ibn.DefaultUserReports");
			addReportNode(userReportNode, "MessageHistory_Personal", "/UserReports/User_History.aspx?ReportType=User", "true", "Mediachase.UI.Web.UserReports.Modules.MessageHistoryInfo, Mediachase.Ibn.DefaultUserReports");
			addReportNode(userReportNode, "AlertHistory_Admin", "/UserReports/AlertsHistory.aspx?ReportType=Admin", "Mediachase.UI.Web.UserReports.Modules.AlertsHistoryAdminInfo, Mediachase.Ibn.DefaultUserReports");
			addReportNode(userReportNode, "AlertHistory_Personal", "/UserReports/AlertsHistory.aspx?ReportType=User", "true", "Mediachase.UI.Web.UserReports.Modules.AlertsHistoryInfo, Mediachase.Ibn.DefaultUserReports");

			doc.Save(path);
		}

		private void removeReportNode(XmlNode reportNode, string name)
		{
			XmlNode node = getSubReportNode(reportNode, name);
			if (node != null)
			{
				reportNode.RemoveChild(node);
			}
		}

		private void clearWebConfig(string path)
		{
			XmlDocument doc = new XmlDocument();

			doc.Load(path);

			XmlNode userReportNode = doc.SelectSingleNode("configuration/userReports");

			removeReportNode(userReportNode, "PortalQuickSnapshot");
			removeReportNode(userReportNode, "MostActiveReport");
			removeReportNode(userReportNode, "GroupAndUserImStat");
			removeReportNode(userReportNode, "GroupAndUserStat");
			removeReportNode(userReportNode, "ChatHistory_Admin");
			removeReportNode(userReportNode, "ChatHistory_Personal");
			removeReportNode(userReportNode, "MessageHistory_Admin");
			removeReportNode(userReportNode, "MessageHistory_Personal");
			removeReportNode(userReportNode, "AlertHistory_Admin");
			removeReportNode(userReportNode, "AlertHistory_Personal");

			doc.Save(path);
		}

		public override void Commit( IDictionary mySavedState )
		{
			base.Commit( mySavedState );

			try
			{
				string path = Context.Parameters["WebPath"];

				modifyWebConfig(Path.Combine(path, WebConfigPath));
			}
			catch (Exception ex)
			{
				throw new InstallException(ex.Message);
			}
		}		

		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall (savedState);

			try
			{
				string path = Context.Parameters["WebPath"];

				clearWebConfig(Path.Combine(path, WebConfigPath));
			}
			catch (Exception ex)
			{
				throw new InstallException(ex.Message);
			}
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
