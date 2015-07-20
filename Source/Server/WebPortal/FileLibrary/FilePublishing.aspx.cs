using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.Ibn.Service;
using Mediachase.MetaDataPlus.Import.Parser;
using Mediachase.UI.Web.Util;
using Mediachase.IBN.Business.EMail;


namespace Mediachase.IBN.UI.Web.FileLibrary
{
	/// <summary>
	/// Summary description for FilePublished.
	/// </summary>
	public partial class FilePublishing : System.Web.UI.Page
	{


		protected void Page_Load(object sender, System.EventArgs e)
		{
		}

		#region Toolbox List Import
		private string SaveNParseExcel(string fileName, Stream InputStream)
		{
			string wwwpath = CommonHelper.ChartPath + Guid.NewGuid().ToString() + Path.GetExtension(fileName);
			using (Stream stream = File.Create(Server.MapPath(wwwpath)))
			{
				InputStream.Seek(0, SeekOrigin.Begin);
				BinaryReader reader = new BinaryReader(InputStream);
				int BufferSize = 655360; // 640 KB
				byte[] buffer = reader.ReadBytes(BufferSize);

				while (buffer.Length > 0)
				{
					stream.Write(buffer, 0, buffer.Length);
					buffer = reader.ReadBytes(BufferSize);
				}
				reader.Close();
			}

			/*OleDbIncomingDataParser	parser = new OleDbIncomingDataParser(ExcelVersion.Excel80);
			DataSet ds = parser.Parse(Server.MapPath(wwwpath), null);*/

			IMCOleDBHelper helper = (IMCOleDBHelper)Activator.GetObject(typeof(IMCOleDBHelper), ConfigurationManager.AppSettings["McOleDbServiceString"]);
			DataSet ds = helper.ConvertExcelToDataSet(Server.MapPath(wwwpath));

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml("<Lists><File>" + Server.MapPath(wwwpath) + "</File></Lists>");

			XmlNode rootNode = xmlDoc.SelectSingleNode("Lists");
			foreach (DataTable table in ds.Tables)
			{
				XmlNode listNode = xmlDoc.CreateElement("List");

				XmlNode sheetNode = xmlDoc.CreateElement("SheetName");
				sheetNode.InnerText = table.TableName;

				foreach (DataColumn column in table.Columns)
				{
					XmlNode colNode = xmlDoc.CreateElement("Field");

					XmlNode nameNode = xmlDoc.CreateElement("Name");
					nameNode.InnerText = column.ColumnName;

					XmlNode typeNode = xmlDoc.CreateElement("Type");
					typeNode.InnerText = column.DataType.Name;

					colNode.AppendChild(nameNode);
					colNode.AppendChild(typeNode);

					listNode.AppendChild(colNode);
				}
				listNode.AppendChild(sheetNode);

				rootNode.AppendChild(listNode);
			}
			return xmlDoc.InnerXml;
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		protected void btnSubmit_Click(object sender, System.EventArgs e)
		{
			bool withResponse = false;

			if (this.IsPostBack)
			{
				try
				{
					string sUserLight = "userlight";

					// check user's name and password here
					UserLight currentUser = Security.GetUser(Login.Value, Password.Value);
					if (currentUser == null)
						throw new HttpException(405, "Your login or password is invalid.");

					// Security Addon [3/2/2004]
					UserLight retUser = null;
					if (HttpContext.Current.Items.Contains(sUserLight))
					{
						retUser = (UserLight)HttpContext.Current.Items[sUserLight];
						HttpContext.Current.Items.Remove(sUserLight);
					}
					HttpContext.Current.Items.Add(sUserLight, currentUser);
					// End Security Addon [3/2/2004]

					// New Folder System Addon [12/27/2005]
					string ContainerName = "FileLibrary";
					string ContainerKey = String.Empty;
					int objectId = Int32.Parse(ObjectId.Value);
					int objectTypeId = Int32.Parse(ObjectTypeId.Value);
					int folderId = 0;

					switch ((ObjectTypes)objectTypeId)
					{
						case ObjectTypes.Project:
							ContainerKey = "ProjectId_" + objectId.ToString();
							break;
						case ObjectTypes.Issue:
							ContainerKey = string.Empty;
							break;
						case ObjectTypes.Task:
							ContainerKey = "TaskId_" + objectId.ToString();
							break;
						case ObjectTypes.CalendarEntry:
							ContainerKey = "EventId_" + objectId.ToString();
							break;
						case ObjectTypes.Folder:
							ContainerKey = "Workspace";
							if (objectId != 0)
							{
								folderId = objectId;
								ContainerKey = Mediachase.IBN.Business.ControlSystem.DirectoryInfo.GetContainerKey(folderId);
							}
							break;
						case ObjectTypes.Document:
							ContainerKey = "DocumentId_" + objectId.ToString();
							break;
						case ObjectTypes.ToDo:
							ContainerKey = "ToDoId_" + objectId.ToString();
							break;
					}
					if (ContainerKey != String.Empty)
					{
						Mediachase.IBN.Business.ControlSystem.BaseIbnContainer bic = Mediachase.IBN.Business.ControlSystem.BaseIbnContainer.Create(ContainerName, ContainerKey);
						Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

						if (folderId == 0)
							folderId = fs.Root.Id;

						string fileName = Path.GetFileName(PublishedFile.PostedFile.FileName);
						if (FileName.Value != string.Empty)
						{
							fileName = FileName.Value;
						}
						/*int	index = 0;
						while (fs.FileExist(fileName, folderId))
						{
							fileName = Path.GetFileNameWithoutExtension(PublishedFile.PostedFile.FileName) + (index++).ToString();
							fileName += Path.GetExtension(PublishedFile.PostedFile.FileName);
						}*/
						fs.SaveFile(folderId, fileName, PublishedFile.PostedFile.InputStream);
					}
					else if ((ObjectTypes)objectTypeId == ObjectTypes.List)
					{
						string xml = SaveNParseExcel(PublishedFile.PostedFile.FileName, PublishedFile.PostedFile.InputStream);

						Response.BinaryWrite(System.Text.Encoding.UTF8.GetBytes(xml));
						withResponse = true;
					}
					else if ((ObjectTypes)objectTypeId == ObjectTypes.Issue)
					{
						string fileName = Path.GetFileName(PublishedFile.PostedFile.FileName);
						if (FileName.Value != string.Empty)
						{
							fileName = FileName.Value;
						}

						// OZ: 2008-08-19 Add Process Eml Attachments
						if (Path.GetExtension(fileName).ToLower() == ".eml")
						{
							// Calculate email box
							int emailBoxId = EMailRouterOutputMessage.FindEMailRouterPublicId(objectId);

							// Save Email to email storage
							int eMailMessageId = EMailMessage.CreateFromStream(emailBoxId, PublishedFile.PostedFile.InputStream);

							EMailMessage.AddToIncidentMessage(false, objectId, eMailMessageId);

							try
							{
								ArrayList excludeUsers = EMailRouterOutputMessage.Send(objectId, eMailMessageId);
								SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Forum_MessageAdded, objectId, -1, excludeUsers);
							}
							catch (Exception ex)
							{
								System.Diagnostics.Trace.WriteLine(ex);
								//Log.WriteError(ex.ToString());
							}
						}
						// Process Default files
						else
						{
							BaseIbnContainer destContainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", objectId));
							ForumStorage forumStorage = (ForumStorage)destContainer.LoadControl("ForumStorage");

							ForumThreadNodeInfo info = forumStorage.CreateForumThreadNode(string.Empty, Security.CurrentUser.UserID, (int)ForumStorage.NodeContentType.TextWithFiles);

							BaseIbnContainer forumContainer = BaseIbnContainer.Create("FileLibrary", string.Format("ForumNodeId_{0}", info.Id));
							FileStorage fs = (FileStorage)forumContainer.LoadControl("FileStorage");


							fs.SaveFile(fileName, PublishedFile.PostedFile.InputStream);

							ForumThreadNodeSettingCollection settings1 = new ForumThreadNodeSettingCollection(info.Id);
							settings1.Add(ForumThreadNodeSetting.Internal, "1");
						}
					}
					// End New Folder System Addon [12/27/2005]

					// Security Addon [3/2/2004]
					HttpContext.Current.Items.Remove(sUserLight);
					HttpContext.Current.Items.Add(sUserLight, retUser);
					// End Security Addon [3/2/2004]
				}
				catch (Exception ex)
				{
					throw new HttpException(405, "Internal Exception", ex);
				}
				if (!withResponse)
				{
					this.Response.Write("Published Completed");
				}
				this.Response.End();
			}
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
