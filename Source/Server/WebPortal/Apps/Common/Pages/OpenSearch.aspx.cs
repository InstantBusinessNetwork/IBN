using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Mediachase.Ibn.Data.Sql;
using System.Data;
using Mediachase.IBN.Business.Rss;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Data;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Apps.Common.Pages
{
	/// <summary>
	/// Represents open search.
	/// </summary>
	public partial class OpenSearch : System.Web.UI.Page
	{
		#region IbnObjectInfo class
		/// <summary>
		/// Represents ibn object info.
		/// </summary>
		internal class IbnObjectInfo
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="IbnObjectInfo"/> class.
			/// </summary>
			public IbnObjectInfo()
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="IbnObjectInfo"/> class.
			/// </summary>
			/// <param name="objectId">The object id.</param>
			/// <param name="objectTypeId">The object type id.</param>
			/// <param name="title">The title.</param>
			public IbnObjectInfo(int objectId, int objectTypeId, string title)
			{
				this.ObjectId = objectId;
				this.ObjectTypeId = ObjectTypeId;
				this.Title = title;
			}

			/// <summary>
			/// Gets or sets the object id.
			/// </summary>
			/// <value>The object id.</value>
			public int ObjectId { get; set; }

			/// <summary>
			/// Gets or sets the object type id.
			/// </summary>
			/// <value>The object type id.</value>
			public int ObjectTypeId { get; set; }

			/// <summary>
			/// Gets or sets the title.
			/// </summary>
			/// <value>The title.</value>
			public string Title { get; set; }
		}
		#endregion

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			string userId = this.Request.QueryString["u"].ToLower();

			string mode = this.Request.QueryString["mode"];
			string searchTerms = this.Request.QueryString["s"];

			if (!string.IsNullOrEmpty(mode))
				mode = mode.ToLower();

			RssGenerator.LogonUserByRssKey(new Guid(userId));

			switch (mode)
			{
				case "xml":
					RenderXmlSugesstions(searchTerms);
					break;
				case "json":
					RenderJsonSugesstions(searchTerms);
					break;
				default:
					RenderOpenSearchDescription();
					break;
			}
		}

		/// <summary>
		/// Renders the json sugesstions.
		/// </summary>
		/// <param name="searchTerms">The search terms.</param>
		private void RenderJsonSugesstions(string searchTerms)
		{
			StringBuilder sbSearchTerms = new StringBuilder();
			StringBuilder sbDescriptions = new StringBuilder();
			StringBuilder sbQueryUrls = new StringBuilder();

			// Create Titles
			bool bFrist = true;
			foreach (IbnObjectInfo info in List(searchTerms))
			{
				if (bFrist)
				{
					bFrist = false;
				}
				else
				{
					sbSearchTerms.Append(", ");
					sbDescriptions.Append(", ");
					sbQueryUrls.Append(", ");
				}

				sbSearchTerms.AppendFormat(@"""{0}""", info.Title);
				sbDescriptions.AppendFormat(@"""{0}""", "Description Here");
				sbQueryUrls.AppendFormat(@"""{0}""", "www.gazeta.ru");
			}

			//sbJson = new StringBuilder();

			//sbJson.Append(@"[""xbox"","+
			//            @"[""Xbox 360"", ""Xbox cheats"", ""Xbox 360 games""], "+
			//            @"[""The official Xbox website from Microsoft"", ""Codes and walkthroughs"", ""Games and accessories""],"+
			// @"[""http://www.xbox.com"",""http://www.example.com/xboxcheatcodes.aspx"", ""http://www.example.com/games""]]");

			HttpContext.Current.Response.Write(
				string.Format(@"[""{0}"", [{1}], [{2}], [{3}]]",
					searchTerms,
					sbSearchTerms.ToString(),
					sbDescriptions.ToString(),
					sbQueryUrls.ToString())
					);

			HttpContext.Current.Response.End();
		}

		/// <summary>
		/// Renders the XML sugesstions.
		/// </summary>
		/// <param name="searchTerms">The search terms.</param>
		private void RenderXmlSugesstions(string searchTerms)
		{
		}

		private static string CreateFullLink(Page page, string relativeLink)
		{
			return string.Format("{0}{1}", Configuration.PortalLink, page.ResolveUrl(relativeLink));
		}

		/// <summary>
		/// Renders the open search description.
		/// </summary>
		private void RenderOpenSearchDescription()
		{
			Guid userId = Mediachase.IBN.Business.User.GetRssKeyByUserId(Security.CurrentUser.UserID); 

			string strXml =
				@"<OpenSearchDescription xmlns=""http://a9.com/-/spec/opensearch/1.1/"">" + Environment.NewLine +

				@"<ShortName>IBN Portal Search</ShortName>" + Environment.NewLine +
				@"<Description>Allows search object in IBN Portal</Description>" + Environment.NewLine +

				string.Format(@"<Image width=""16"" height=""16"" type=""image/x-icon"">{0}</Image>", CreateFullLink(this, "~/portal.ico")) + Environment.NewLine +

				string.Format(@"<Url type=""text/html"" template=""{0}""/>", 
					CreateFullLink(this, "~/Apps/Common/Pages/Search.aspx?s={searchTerms}&amp;u=" + userId.ToString())) + Environment.NewLine +

				string.Format(@"<Url type=""application/x-suggestions+json"" template=""{0}""/>", 
					CreateFullLink(this, "~/Apps/Common/Pages/OpenSearch.aspx?mode=jason&amp;s={searchTerms}&amp;u=" + userId.ToString())) + Environment.NewLine +

				string.Format(@"<Url type=""application/x-suggestions+xml"" template=""{0}""/>", 
					CreateFullLink(this, "~/Apps/Common/Pages/OpenSearch.aspx?mode=xml&amp;s={searchTerms}&amp;u=" + userId.ToString())) + Environment.NewLine +

				@"<InputEncoding>UTF-8</InputEncoding>" + Environment.NewLine +
				@"</OpenSearchDescription>" + Environment.NewLine +
				@"";

			this.Response.ContentType = "application/opensearchdescription+xml";
			this.Response.Write(strXml);
			this.Response.End();
		}

		private IbnObjectInfo[] List(string searchTerms)
		{
			List<IbnObjectInfo> retVal = new List<IbnObjectInfo>();

			string sqlQuery;

			#region Init Sql Query
			sqlQuery = @"SET NOCOUNT ON" + Environment.NewLine +
	@"-- 57041362-9022-4A30-A73C-91EFBE11F7E0" + Environment.NewLine +
	@"-- SELECT * FROM USERS" + Environment.NewLine +
	Environment.NewLine +
	@"DECLARE @UserId NVARCHAR(255)" + Environment.NewLine +
	@"SET @UserId = 21" + Environment.NewLine +
	Environment.NewLine +
	@"--SELECT * FROM Users" + Environment.NewLine +
	Environment.NewLine +
				//@"DECLARE @SearchTerms NVARCHAR(255)" + Environment.NewLine +
				//@"SET @SearchTerms = N'test %'" + Environment.NewLine +
	Environment.NewLine +
	@"-- Project, Task, Todo, Event Security" + Environment.NewLine +
	@"DECLARE @IsPPM_Exec bit" + Environment.NewLine +
	@"SET @IsPPM_Exec = 0" + Environment.NewLine +
	@"IF EXISTS(SELECT * FROM USER_GROUP WHERE UserId = @UserId AND (GroupId = 4 OR GroupId = 7))		-- PPM " +
	@"or Exec" + Environment.NewLine +
	@"	SET @IsPPM_Exec = 1" + Environment.NewLine +
	@"-- End Project, Task, Todo, Event Security" + Environment.NewLine +
	Environment.NewLine +
	@"-- Incident Security" + Environment.NewLine +
	@"DECLARE @IsPPM bit" + Environment.NewLine +
	@"SET @IsPPM = 0" + Environment.NewLine +
	@"IF EXISTS(SELECT * FROM USER_GROUP WHERE UserId = @UserId AND GroupId = 4)" + Environment.NewLine +
	@"	SET @IsPPM = 1" + Environment.NewLine +
	@"DECLARE @IsExec bit" + Environment.NewLine +
	@"SET @IsExec = 0" + Environment.NewLine +
	@"IF EXISTS(SELECT * FROM USER_GROUP WHERE UserId = @UserId AND GroupId = 7)" + Environment.NewLine +
	@"	SET @IsExec = 1" + Environment.NewLine +
	@"DECLARE @IsHDM bit" + Environment.NewLine +
	@"SET @IsHDM = 0" + Environment.NewLine +
	@"IF EXISTS(SELECT * FROM USER_GROUP WHERE UserId = @UserId AND GroupId = 5)" + Environment.NewLine +
	@"	SET @IsHDM = 1" + Environment.NewLine +
	@"-- End Incident Security" + Environment.NewLine +
	Environment.NewLine +
	@"SELECT TOP 15 A.ObjectId, A.ObjectType, A.Title  " + Environment.NewLine +
	@"FROM" + Environment.NewLine +
	@"(" + Environment.NewLine +
	@"-- Project" + Environment.NewLine +
	@"SELECT TOP 15 P.ProjectId as ObjectId, 2 As ObjectType, P.Title As Title  " + Environment.NewLine +
	@"FROM PROJECTS P WITH(NOLOCK)" + Environment.NewLine +
	@"	-- Project Security" + Environment.NewLine +
	@"	LEFT JOIN PROJECT_SECURITY_ALL PS ON (P.ProjectId = PS.ProjectId AND PS.PrincipalId = @UserId)" + Environment.NewLine +
	@"	-- End Project Security" + Environment.NewLine +
	@"WHERE Title LIKE @SearchTerms AND" + Environment.NewLine +
	@"	-- Project Security" + Environment.NewLine +
	@"	(" + Environment.NewLine +
	@"		@IsPPM_Exec = 1 OR " + Environment.NewLine +
	@"		PS.IsManager = 1 OR " + Environment.NewLine +
	@"		PS.IsExecutiveManager = 1 OR " + Environment.NewLine +
	@"		PS.IsTeamMember = 1 OR " + Environment.NewLine +
	@"		PS.IsSponsor = 1 OR " + Environment.NewLine +
	@"		PS.IsStakeHolder = 1" + Environment.NewLine +
	@"	)" + Environment.NewLine +
	@"	-- End Project Security" + Environment.NewLine +
	@"ORDER BY LEN(Title), Title" + Environment.NewLine +
	@"UNION ALL" + Environment.NewLine +
	@"-- Task" + Environment.NewLine +
	@"SELECT TOP 15 T.TaskId as ObjectId, 5 As ObjectType, T.Title As Title " + Environment.NewLine +
	@"FROM TASKS T WITH(NOLOCK)" + Environment.NewLine +
	@"WHERE Title LIKE @SearchTerms AND" + Environment.NewLine +
	@"	-- Task Security" + Environment.NewLine +
	@"	(" + Environment.NewLine +
	@"        @IsPPM_Exec = 1" + Environment.NewLine +
	@"        OR T.ProjectId IN" + Environment.NewLine +
	@"			(SELECT ProjectId FROM PROJECT_SECURITY " + Environment.NewLine +
	@"				WHERE PrincipalId = @UserId" + Environment.NewLine +
	@"					AND (IsManager = 1 OR IsExecutiveManager = 1 OR IsTeamMember = 1 OR IsSponsor = 1 OR " +
	@"IsStakeHolder = 1)" + Environment.NewLine +
	@"			)" + Environment.NewLine +
	@"        OR TaskId IN " + Environment.NewLine +
	@"    	    (SELECT TaskId FROM TASK_SECURITY S " + Environment.NewLine +
	@"        	  WHERE PrincipalId = @UserId AND (IsResource = 1 OR IsManager = 1))" + Environment.NewLine +
	@"    )" + Environment.NewLine +
	@"	-- End Task Security" + Environment.NewLine +
	@"ORDER BY LEN(Title), Title" + Environment.NewLine +
	@"UNION ALL" + Environment.NewLine +
	@"-- ToDo" + Environment.NewLine +
	@"SELECT TOP 15 T.TodoId as ObjectId, 6 As ObjectType, T.Title As Title " + Environment.NewLine +
	@"FROM TODO T WITH(NOLOCK)" + Environment.NewLine +
	@"WHERE Title LIKE @SearchTerms AND" + Environment.NewLine +
	@"-- Todo Security" + Environment.NewLine +
	@"	(" + Environment.NewLine +
	@"        @IsPPM_Exec = 1" + Environment.NewLine +
	@"        OR T.ProjectId IN" + Environment.NewLine +
	@"			(SELECT ProjectId FROM PROJECT_SECURITY " + Environment.NewLine +
	@"				WHERE PrincipalId = @UserId" + Environment.NewLine +
	@"					AND (IsManager = 1 OR IsExecutiveManager = 1 OR IsTeamMember = 1 OR IsSponsor = 1 OR " +
	@"IsStakeHolder = 1)" + Environment.NewLine +
	@"			)" + Environment.NewLine +
	@"        OR ToDoId IN " + Environment.NewLine +
	@"	        (SELECT ToDoId FROM TODO_SECURITY_ALL S " + Environment.NewLine +
	@"        	  WHERE PrincipalId = @UserId AND (IsResource = 1 OR IsManager = 1))" + Environment.NewLine +
	@"    )" + Environment.NewLine +
	@"-- Todo Security" + Environment.NewLine +
	@"ORDER BY LEN(Title), Title" + Environment.NewLine +
	@"UNION ALL" + Environment.NewLine +
	@"-- Incident" + Environment.NewLine +
	@"SELECT TOP 15 I.IncidentId as ObjectId, 7 As ObjectType, I.Title As Title " + Environment.NewLine +
	@"FROM INCIDENTS I WITH(NOLOCK)" + Environment.NewLine +
	@"WHERE Title LIKE @SearchTerms AND" + Environment.NewLine +
	@"	(	" + Environment.NewLine +
	@"		@IsPPM = 1 OR " + Environment.NewLine +
	@"		@IsExec = 1" + Environment.NewLine +
	@"		OR (I.ProjectId IS NULL AND @IsHDM = 1)" + Environment.NewLine +
	@"		OR I.ProjectId IN" + Environment.NewLine +
	@"			(SELECT ProjectId FROM PROJECT_SECURITY" + Environment.NewLine +
	@"				WHERE PrincipalId = @UserId" + Environment.NewLine +
	@"					AND (IsManager = 1 OR IsExecutiveManager = 1 OR IsTeamMember = 1 OR IsSponsor = 1 OR " +
	@"IsStakeHolder = 1)" + Environment.NewLine +
	@"			)" + Environment.NewLine +
	@"		OR I.IncidentId IN" + Environment.NewLine +
	@"			(SELECT IncidentId FROM INCIDENT_SECURITY_ALL" + Environment.NewLine +
	@"				WHERE PrincipalId = @UserId AND (IsManager = 1 OR IsCreator = 1 OR IsResource = 1)" + Environment.NewLine +
	@"			)" + Environment.NewLine +
	@"	)" + Environment.NewLine +
	@"ORDER BY LEN(Title), Title" + Environment.NewLine +
	@"UNION ALL" + Environment.NewLine +
	@"-- Event" + Environment.NewLine +
	@"SELECT TOP 15 E.eventId as ObjectId, 4 As ObjectType, E.Title As Title " + Environment.NewLine +
	@"FROM EVENTS E WITH(NOLOCK)" + Environment.NewLine +
	@"WHERE Title LIKE @SearchTerms AND" + Environment.NewLine +
	@"-- Event Security" + Environment.NewLine +
	@"    (" + Environment.NewLine +
	@"        @IsPPM_Exec = 1" + Environment.NewLine +
	@"        OR E.ProjectId IN" + Environment.NewLine +
	@"			(SELECT ProjectId FROM PROJECT_SECURITY " + Environment.NewLine +
	@"				WHERE PrincipalId = @UserId" + Environment.NewLine +
	@"					AND (IsManager = 1 OR IsExecutiveManager = 1 OR IsTeamMember = 1 OR IsSponsor = 1 OR " +
	@"IsStakeHolder = 1)" + Environment.NewLine +
	@"			)" + Environment.NewLine +
	@"        OR EventId IN " + Environment.NewLine +
	@"	        (SELECT EventId FROM EVENT_SECURITY_ALL S " + Environment.NewLine +
	@"    	      WHERE PrincipalId = @UserId AND (IsResource = 1 OR IsManager = 1))" + Environment.NewLine +
	@"    )" + Environment.NewLine +
	@"-- End Event Security" + Environment.NewLine +
	@"ORDER BY LEN(Title), Title" + Environment.NewLine +
	@"UNION ALL" + Environment.NewLine +
	@"-- Files" + Environment.NewLine +
	@"SELECT TOP 15 F.FileId as ObjectId, 8 As ObjectType, F.Name As Title " + Environment.NewLine +
	@"FROM fsc_Files F WITH(NOLOCK)" + Environment.NewLine +
	@"-- Security" + Environment.NewLine +
	@"	INNER JOIN fsc_Directories D WITH(NOLOCK) ON F.DirectoryId = D.DirectoryId" + Environment.NewLine +
	@"	INNER JOIN fsc_FolderSecurityAll FSA ON  " + Environment.NewLine +
	@"		FSA.DirectoryId = F.DirectoryId AND " + Environment.NewLine +
	@"		FSA.ContainerKey = D.ContainerKey AND " + Environment.NewLine +
	@"		FSA.[Action] = N'Read' AND " + Environment.NewLine +
	@"		FSA.Allow = 1 AND" + Environment.NewLine +
	@"		FSA.PrincipalId = @UserId" + Environment.NewLine +
	@"WHERE F.Name LIKE @SearchTerms " + Environment.NewLine +
	@"ORDER BY LEN(F.Name), F.Name" + Environment.NewLine +
	@"UNION ALL" + Environment.NewLine +
	@"-- Documents" + Environment.NewLine +
	@"SELECT TOP 15 D.DocumentId as ObjectId, 16 As ObjectType, D.Title As Title " + Environment.NewLine +
	@"FROM DOCUMENTS D WITH(NOLOCK)" + Environment.NewLine +
	@"WHERE Title LIKE @SearchTerms AND" + Environment.NewLine +
	@"-- Document Security" + Environment.NewLine +
	@"	(" + Environment.NewLine +
	@"		@IsPPM = 1 OR @IsExec = 1" + Environment.NewLine +
	@"		OR D.ProjectId IN" + Environment.NewLine +
	@"			(SELECT ProjectId FROM PROJECT_SECURITY " + Environment.NewLine +
	@"				WHERE PrincipalId = @UserId" + Environment.NewLine +
	@"					AND (IsManager = 1 OR IsExecutiveManager = 1 OR IsTeamMember = 1 OR IsSponsor = 1 OR " +
	@"IsStakeHolder = 1)" + Environment.NewLine +
	@"			)" + Environment.NewLine +
	@"		OR D.DocumentId IN " + Environment.NewLine +
	@"			(SELECT DocumentId FROM DOCUMENT_SECURITY_ALL" + Environment.NewLine +
	@"				WHERE PrincipalId = @UserId AND (IsManager = 1 OR IsResource = 1)" + Environment.NewLine +
	@"			)" + Environment.NewLine +
	@"	)" + Environment.NewLine +
	@"-- End Document Security" + Environment.NewLine +
	@"ORDER BY LEN(Title), Title" + Environment.NewLine +
	@"-- TODO: Organizations" + Environment.NewLine +
	@"-- TODO: Contacts" + Environment.NewLine +
	@"-- TODO: Users" + Environment.NewLine +
	@"-- TODO: Group" + Environment.NewLine +
	@") A" + Environment.NewLine +
	@"ORDER BY LEN(A.Title), A.Title" + Environment.NewLine +
	Environment.NewLine +
	@"";
			#endregion

			using (IDataReader reader = SqlHelper.ExecuteReader(DataContext.Current.SqlContext,
				System.Data.CommandType.Text,
				sqlQuery,
				SqlHelper.SqlParameter("@SearchTerms", SqlDbType.NVarChar, 255, searchTerms + "%")))
			{
				while (reader.Read())
				{
					IbnObjectInfo newItem = new IbnObjectInfo((int)reader["ObjectId"],
						(int)reader["ObjectType"],
						(string)reader["Title"]);

					retVal.Add(newItem);
				}
			}

			return retVal.ToArray();
		}

	}
}
