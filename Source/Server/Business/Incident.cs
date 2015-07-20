using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Web;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using Mediachase.IBN.Business.ControlSystem; 
using Mediachase.IBN.Business.EMail;
using Mediachase.IBN.Database.EMail;
using Mediachase.MetaDataPlus;
using Mediachase.Net.Mail;
using Mediachase.IBN.Business.SpreadSheet;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Business
{
	public class Incident
	{
		#region FilterKeys
		public const string ProjectFilterKey = "IncidentProjectId";
		public const string ManagerFilterKey = "IncidentManagerId";
		public const string ResponsibleFilterKey = "IncidentResponsibleId";
		public const string CreatorFilterKey = "IncidentCreatorId";
		public const string PriorityFilterKey = "IncidentPriorityId";
		public const string IssueBoxFilterKey = "IncidentBoxId";
		public const string TypeFilterKey = "IncidentTypeId";
		public const string ClientFilterKey = "IncidentClient";
		public const string StatusFilterKey = "IncidentStatusId";
		public const string SeverityFilterKey = "IncidentSeverityId";
		public const string GenCategoryTypeFilterKey = "IncidentGeneralCategoryType";
		public const string GenCategoriesFilterKey = "IncidentGeneralCategories";
		public const string IssueCategoryTypeFilterKey = "IncidentCategoryType";
		public const string IssueCategoriesFilterKey = "IncidentCategories";
		public const string UnansweredFilterKey = "IncidentOnlyUnanswered";
		public const string OverdueFilterKey = "IncidentOnlyOverdue";
		public const string KeywordFilterKey = "IncidentKeyword";
		#endregion

		public enum FieldSetName
		{
			IncidentsDefault,
			IncidentsLight,
			GroupByProject,
			GroupByClient,
			GroupByBox
		}

		public enum IssueFieldSet
		{
			IncidentsDefault,
			IncidentsLight,
			IncidentsTracking
		}

		public enum AvailableGroupField
		{
			NotSet,
			Project,
			IssueBox,
			Client,
			Responsible
		}

		#region Tracking
		public struct Tracking
		{
			public bool CanSetNoUser;
			public bool CanSetUser;
			public bool CanSetGroup;
			public bool ShowAcceptDecline;
			public bool ShowAccept;
			public bool ShowDecline;
			public bool ShowResourceAceptDecline;
			public bool CanChangeState;


			public Tracking(bool canSetNoUser,bool canSetUser,bool canSetGroup,bool showAcceptDecline,bool showAccept,bool showDecline, bool showResourceAceptDecline, bool	canChangeState)
			{
				CanSetNoUser = canSetNoUser;
				CanSetUser = canSetUser;
				CanSetGroup = canSetGroup;
				ShowAcceptDecline = showAcceptDecline;
				ShowAccept = showAccept;
				ShowDecline = showDecline;
				ShowResourceAceptDecline = showResourceAceptDecline;
				CanChangeState = canChangeState;
			}
		}

		#endregion

		#region GetTrackingInfo
		public static Tracking GetTrackingInfo(int incident_id)
		{	
			int IncidentBoxId =-1;
			int ResponsibleId =-1;
			bool IsResponsibleGroup = false;
			int StateId = -1;

			using(IDataReader reader = GetIncident(incident_id, false))
			{
				if(reader.Read())
				{
					IncidentBoxId = (int)reader["IncidentBoxId"];
					ResponsibleId = reader["ResponsibleId"] == DBNull.Value? -1 :(int)reader["ResponsibleId"];
					IsResponsibleGroup = (bool)reader["IsResponsibleGroup"];
					StateId = (int)reader["StateId"];
				}
			}
			if(IncidentBoxId < 0) {return new Tracking(false,false,false,false,false,false,false,false);}

			IncidentBoxDocument IncDoc = IncidentBoxDocument.Load(IncidentBoxId);

			UserLight cu = Security.CurrentUser;
			
			bool IsUserWaitInGroup = false;
			bool IsUserInGroup = false;
			using(IDataReader reader= DBIncident.GetResponsibleByUser(incident_id,cu.UserID))
			{
				if(reader.Read())
				{
					IsUserWaitInGroup = (bool)reader["ResponsePending"];
					IsUserInGroup = true;
				}
			}
			
			//security
			bool CanSetNoUser = CanDoAction(incident_id,"AssignResponsible",Security.UserID);
						
			//security
			//responsible incident box - select user option
			bool CanSetUser = (IncDoc.GeneralBlock.AllowToReassignResponsibility && cu.UserID == ResponsibleId)
				|| (IncDoc.GeneralBlock.AllowToReassignResponsibility && IncDoc.GeneralBlock.ResponsibleAssignType == ResponsibleAssignType.Pool && IncDoc.GeneralBlock.ResponsiblePool.Contains(cu.UserID))
				|| CanDoAction(incident_id,"AssignResponsible",cu.UserID);
			
			//security
			bool CanSetGroup = CanDoAction(incident_id,"AssignResponsible",cu.UserID );
						
			//group state - and user in group and not said decline
			bool ShowAcceptDecline = IsResponsibleGroup && IsUserWaitInGroup && (StateId == (int)ObjectStates.Active || StateId == (int)ObjectStates.ReOpen);
			
			//security
			//reposible - incident box
			bool ShowAccept = !ShowAcceptDecline && ResponsibleId != cu.UserID &&
				(CanDoAction(incident_id,"AssignResponsible",cu.UserID ) || (IsUserInGroup && IncDoc.GeneralBlock.AllowUserToComeResponsible));
						 
			//responsible
			//	security
			//	incident box
			bool ShowDecline = ResponsibleId == cu.UserID && 
				(CanDoAction(incident_id,"AssignResponsible",cu.UserID ) || IncDoc.GeneralBlock.AllowToDeclineResponsibility)
				&& (StateId == (int)ObjectStates.Active || StateId == (int)ObjectStates.ReOpen);
						
			//resources state
			bool ShowResourceAceptDecline = ShowAcceptDeny(incident_id) &&
				(StateId == (int)ObjectStates.Active || StateId == (int)ObjectStates.ReOpen);
						
		
			bool CanChangeState = CanDoAction(incident_id,"ChangeState",cu.UserID );

			return new Tracking(CanSetNoUser,CanSetUser,CanSetGroup,ShowAcceptDecline,ShowAccept,ShowDecline,ShowResourceAceptDecline,CanChangeState);
		}
		#endregion GetTrackingInfo

		#region IncidentSecurity2	
		public class IncidentSecurity2
		{
			public bool IsManager = false;
			public bool IsCreator = false;
			public bool IsResource = false;
			public bool IsPendingResource = false;
			public bool IsResponsible = false;
			public bool IsPendingResponsible = false;
			public bool IsController = false;

			public IncidentSecurity2(int incident_id, int user_id)
			{
				using(IDataReader reader = DBIncident.GetSecurityForUser2(incident_id, user_id))
				{
					if (reader.Read())
					{


						IsManager = ((int)reader["IsManager"] > 0)?true:false;
						IsCreator = ((int)reader["IsCreator"] > 0)?true:false;
						IsResource = ((int)reader["IsResource"] > 0)?true:false;
						IsPendingResource = ((int)reader["IsPendingResource"] > 0)?true:false;;
						IsResponsible = ((int)reader["IsIncidentResponsible"] > 0)?true:false;;
						IsPendingResponsible = ((int)reader["IsPendingResponsible"] > 0)?true:false;;
						IsController = ((int)reader["IsIncidentController"] > 0)?true:false;;
					}
				}
			}
		}
		#endregion

		#region IncidentSecurity	
		public class IncidentSecurity
		{
			public bool IsManager = false;
			public bool IsCreator = false;
			public bool IsResource = false;

			public IncidentSecurity(int incident_id, int user_id)
			{
				using(IDataReader reader = DBIncident.GetSecurityForUser(incident_id, user_id))
				{
					if (reader.Read())
					{
						IsManager = ((int)reader["IsManager"] > 0)?true:false;
						IsCreator = ((int)reader["IsCreator"] > 0)?true:false;
						IsResource = ((int)reader["IsResource"] > 0)?true:false;
					}
				}
			}
		}
		#endregion

		#region INCIDENT_TYPE
		internal static ObjectTypes INCIDENT_TYPE
		{
			get {return ObjectTypes.Issue;}
		}
		#endregion

		#region CanDoAction
		public static bool CanDoAction(int incident_id, string action_name,int user_id)
		{
			bool retval = false;
			//Mediachase.IBN.Business.Project.ProjectSecurity
			Project.ProjectSecurity p_security = null;
			string RoleFilter = "";

			//global roles security
			if(Security.IsUserInGroup(user_id,InternalSecureGroups.Administrator))
				RoleFilter += "Role/@name='ADMIN' or ";

			if(Security.IsUserInGroup(user_id,InternalSecureGroups.ProjectManager))
				RoleFilter += "Role/@name='PM' or ";

			if(Security.IsUserInGroup(user_id,InternalSecureGroups.PowerProjectManager))
				RoleFilter += "Role/@name='PPM' or ";

			if(Security.IsUserInGroup(user_id,InternalSecureGroups.HelpDeskManager))
				RoleFilter += "Role/@name='HDM' or ";

			if(Security.IsUserInGroup(user_id, InternalSecureGroups.ExecutiveManager))
				RoleFilter += "Role/@name='EXEC' or ";

			//Incident's role security
			IncidentSecurity2 i_security = GetSecurity2(incident_id,user_id);
			if(i_security.IsController) RoleFilter += "Role/@name='Controller' or ";
			if(i_security.IsCreator) RoleFilter += "Role/@name='Creator' or ";
			if(i_security.IsManager) RoleFilter += "Role/@name='Manager' or ";
			if(i_security.IsPendingResource) RoleFilter += "Role/@name='wResource' or ";
			if(i_security.IsResource) RoleFilter += "Role/@name='Resource' or ";
			if(i_security.IsPendingResponsible) RoleFilter += "Role/@name='wResponsible' or ";
			if(i_security.IsResponsible) RoleFilter += "Role/@name='Responsible' or ";

			//Incident's role security
			int ProjectId = DBIncident.GetProject(incident_id);
			if (ProjectId > 0)
			{
				p_security = Project.GetSecurity(ProjectId,user_id);
				if(p_security.IsTeamMember) RoleFilter += "Role/@name='p_TeamMemeber' or ";
				if(p_security.IsSponsor) RoleFilter += "Role/@name='p_Sponsor' or ";
				if(p_security.IsStakeHolder) RoleFilter += "Role/@name='p_StakeHolder' or ";
				if(p_security.IsManager) RoleFilter += "Role/@name='p_Manager' or ";
			}

			//ToDO's role security
			//TODO: implement
			RoleFilter += "1=0";			
			ObjectStates oldStateId = ObjectStates.Overdue;
			UserLight cu = user_id==Security.CurrentUser.UserID? Security.CurrentUser:UserLight.Load(user_id);

			using(IDataReader reader = DBIncident.GetIncidentTrackingState(incident_id,cu.m_TimeZoneId,cu.LanguageId))
			{
				if(reader.Read())
					oldStateId = (ObjectStates)reader["StateId"];
			}

			string oldState = "";
			switch(oldStateId)
			{
				case ObjectStates.Upcoming:
					oldState += "new";
					break;
				case ObjectStates.Active:
					oldState += "open";
					break;
				case ObjectStates.Completed:
					oldState += "closed";
					break;
				case ObjectStates.OnCheck:
					oldState += "oncheck";
					break;
				case ObjectStates.ReOpen:
					oldState += "reopen";
					break;
				case ObjectStates.Suspended:
					oldState += "suspended";
					break;
			}


			//load xml
			XmlDocument doc;

			///////////////////////////////////////////////////////////////////////////
			// OZ Improve Page Rendering Speed
			string key = "IncidentStateMachine.xml";

			if (HttpContext.Current == null || HttpContext.Current.Items[key] == null)
			{
				System.IO.Stream st = System.Reflection.Assembly.GetAssembly(typeof(Incident)).GetManifestResourceStream("Mediachase.IBN.Business.Resources.IncidentStateMachine.xml");
				doc = new XmlDocument();
				doc.Load(st);

				if(HttpContext.Current != null)
					HttpContext.Current.Items[key] = doc;
			}
			else
			{
				doc = (XmlDocument)HttpContext.Current.Items[key];
			}
			///////////////////////////////////////////////////////////////////////////

			// [2008-02-05] O.R. hot fix: if incident doesn't exist, we could got an error in the Load method
			int incidentBoxId = GetIncidentBox(incident_id);
			if (incidentBoxId < 0)
				return false;

			bool incidentBoxAllowControl = false;

			///////////////////////////////////////////////////////////////////////////
			// OZ Improve Page Rendering Speed
			string keyIbd = "IncidentBoxDocument_" + incidentBoxId .ToString()+ "_AllowControl";

			if (HttpContext.Current == null || HttpContext.Current.Items[keyIbd] == null)
			{
				IncidentBoxDocument incdoc = IncidentBoxDocument.Load(incidentBoxId);
				incidentBoxAllowControl = incdoc.GeneralBlock.AllowControl;

				if (HttpContext.Current != null)
					HttpContext.Current.Items[keyIbd] = incidentBoxAllowControl;
			}
			else
			{
				incidentBoxAllowControl = (bool)HttpContext.Current.Items[keyIbd];
			}
			///////////////////////////////////////////////////////////////////////////

			string query = "";
			query +=@"/StateMachines/StateMachine[@name='";
			if (incidentBoxAllowControl)
				query +="with_check"; 
				else
				query +="without_check";

			query +=@"']/States/State[@name='";
			query +=oldState;
			query +=@"']/Action[@name='";
			query +=action_name;
			query +=@"'][";
			query += RoleFilter;
			query += "]";

			XmlNodeList nodeList = doc.SelectNodes(query);
			retval = nodeList.Count >0;

			return retval;
		}
		#endregion

		#region IsTransitionAllowed
		public static bool IsTransitionAllowed(int incident_id, ObjectStates oldStateId, ObjectStates StateId)
		{
			if(oldStateId == StateId)
				return true;
			bool retval = false;		

			//Mediachase.IBN.Business.Project.ProjectSecurity
			Project.ProjectSecurity p_security = null;
			string RoleFilter = "";

			//global roles security
			if(Security.IsUserInGroup(InternalSecureGroups.Administrator))
				RoleFilter += "Role/@name='ADMIN' or ";

			if(Security.IsUserInGroup(InternalSecureGroups.ProjectManager))
				RoleFilter += "Role/@name='PM' or ";

			if(Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
				RoleFilter += "Role/@name='PPM' or ";

			if(Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
				RoleFilter += "Role/@name='HDM' or ";

			if(Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager))
				RoleFilter += "Role/@name='EXEC' or ";

			//Incident's role security
			IncidentSecurity2 i_security = GetSecurity2(incident_id);
			if(i_security.IsController) RoleFilter += "Role/@name='Controller' or ";
			if(i_security.IsCreator) RoleFilter += "Role/@name='Creator' or ";
			if(i_security.IsManager) RoleFilter += "Role/@name='Manager' or ";
			if(i_security.IsPendingResource) RoleFilter += "Role/@name='wResource' or ";
			if(i_security.IsResource) RoleFilter += "Role/@name='Resource' or ";
			if(i_security.IsPendingResponsible) RoleFilter += "Role/@name='wResponsible' or ";
			if(i_security.IsResponsible) RoleFilter += "Role/@name='Responsible' or ";

			//Incident's role security
			int ProjectId = DBIncident.GetProject(incident_id);
			if (ProjectId > 0)
			{
				p_security = Project.GetSecurity(ProjectId);
				if(p_security.IsTeamMember) RoleFilter += "Role/@name='p_TeamMemeber' or ";
				if(p_security.IsSponsor) RoleFilter += "Role/@name='p_Sponsor' or ";
				if(p_security.IsStakeHolder) RoleFilter += "Role/@name='p_StakeHolder' or ";
				if(p_security.IsManager) RoleFilter += "Role/@name='p_Manager' or ";
			}

			//ToDO's role security
			//TODO: implement
			RoleFilter += "1=0";			
			
			string newState = "";
			switch(StateId)
			{
				case ObjectStates.Upcoming:
					newState += "new";
					break;
				case ObjectStates.Active:
					newState += "open";
					break;
				case ObjectStates.Completed:
					newState += "closed";
					break;
				case ObjectStates.OnCheck:
					newState += "oncheck";
					break;
				case ObjectStates.ReOpen:
					newState += "reopen";
					break;
				case ObjectStates.Suspended:
					newState += "suspended";
					break;
			}

			string oldState = "";
			switch(oldStateId)
			{
				case ObjectStates.Upcoming:
					oldState += "new";
					break;
				case ObjectStates.Active:
					oldState += "open";
					break;
				case ObjectStates.Completed:
					oldState += "closed";
					break;
				case ObjectStates.OnCheck:
					oldState += "oncheck";
					break;
				case ObjectStates.ReOpen:
					oldState += "reopen";
					break;
				case ObjectStates.Suspended:
					oldState += "suspended";
					break;
			}

			//load xml
			System.IO.Stream st = System.Reflection.Assembly.GetAssembly(typeof(Incident)).GetManifestResourceStream("Mediachase.IBN.Business.Resources.IncidentStateMachine.xml");
			XmlDocument doc = new XmlDocument();
			doc.Load(st);
			
			IncidentBoxDocument incdoc = IncidentBoxDocument.Load(GetIncidentBox(incident_id));


			string query = "";
			
			query +=@"/StateMachines/StateMachine[@name='";
			
			if(incdoc.GeneralBlock.AllowControl)
				query +="with_check"; 
			else
				query +="without_check";
			 
			query +=@"']/States/State[@name='";
			query +=oldState;
			query +=@"']/Action[@name='";
			query +="ChangeState";
			query +=@"']/Transition[";
			query += RoleFilter;
			query += "][@to ='";
			query += newState;
			query += "']";

			XmlNodeList nodeList = doc.SelectNodes(query);

			retval = nodeList.Count > 0;
			return retval;
		}
		#endregion

		#region CanCreate
		public static bool CanCreate(object project_id)
		{
			bool RetVal = false;

			using(IDataReader reader = DBUser.GetUserInfo(Security.CurrentUser.UserID))
			{
				if (reader.Read())
					RetVal = (bool)reader["IsActive"];
			}
			return RetVal;
		}
		#endregion

		#region CanUpdate
		public static bool CanUpdate(int incident_id)
		{
			return CanDoAction(incident_id, "Update",Security.UserID);
			// Обновлять можно только в состоянии New и Active
			// Обновлять могут: PPM, Incident Manager, Creator, HDM (Project=null)
			/*
			bool retval = false;

			int StateId = DBIncident.GetState(incident_id);
			if (StateId != (int)ObjectStates.Completed)
			{
				IncidentSecurity incs = GetSecurity(incident_id);
				retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) || incs.IsManager;

				if (!retval)
				{
					if (incs.IsCreator)
						retval = true;
					else if (Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
					{
						int ProjectId = DBIncident.GetProject(incident_id);
						if (ProjectId <= 0)
							retval = true;
					}
				}
			}
			
			return retval;
			*/
		}
		#endregion

		#region CanUpdateExternalRecipients
		public static bool CanUpdateExternalRecipients(int incident_id)
		{
			int respId = -1;
			using (IDataReader reader = DBIncident.GetIncidentTrackingState(incident_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
			{
				if (reader.Read())
					respId = (int)reader["ResponsibleId"];
			}
			return (respId == Security.CurrentUser.UserID || CanDoAction(incident_id, "Update", Security.UserID));
		}
		#endregion


		#region //CanUpdateStatus - replaced by IsTransitionAllowed
		/*public static bool CanUpdateState(int incident_id)
		{
			// Изменять статус могут: PPM, Incident Manager, Creator (кроме New), HDM (Project=null)
			// Project Team Members и Incident Resource
			bool retval = false;
			IncidentSecurity incs = GetSecurity(incident_id);
			retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) 
				|| incs.IsManager || incs.IsResource;

			int ProjectId = DBIncident.GetProject(incident_id);;

			if (!retval && Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
			{
				if (ProjectId <= 0)
					retval = true;
			}

			if (!retval)
			{
				int StateId = DBIncident.GetState(incident_id);
				if (incs.IsCreator && (StateId != (int)ObjectStates.Upcoming))
					retval = true;
			}

			if (!retval && ProjectId > 0)
			{
				Project.ProjectSecurity ps = Project.GetSecurity(ProjectId);
				if (ps.IsManager || ps.IsExecutiveManager || ps.IsTeamMember)
					retval = true;
			}

			return retval;
		}*/
		#endregion

		#region CanDelete
		public static bool CanDelete(int incident_id)
		{
			return CanDoAction(incident_id,"Delete",Security.UserID);
			/*
			// Удалять можно в любом состоянии
			// Удалять могут: PPM, Incident Manager, Creator (Status=New), HDM (Project=null)
			bool retval = false;

			IncidentSecurity incs = GetSecurity(incident_id);
			retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) || incs.IsManager;
			if (!retval)
			{
				int StatusId = DBIncident.GetState(incident_id);
				if (incs.IsCreator && StatusId == (int)ObjectStates.Upcoming)
					retval = true;
				else if (Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
				{
					int ProjectId = DBIncident.GetProject(incident_id);
					if (ProjectId <= 0)
						retval = true;
				}
			}	
			return retval;
			*/
		}
		#endregion

		#region CanChangeProject
		public static bool CanChangeProject(int incident_id)
		{
			bool retval = CanDoAction(incident_id,"ChangeProject",Security.UserID);

			// O.R. [2008-07-30] Check that there are no finances 
			if (retval && ActualFinances.List(incident_id, ObjectTypes.Issue).Length > 0)
				retval = false;

//			if (retval && Incident.GetListToDoDataTable(incident_id).Rows.Count > 0)
//				retval = false;

			return retval;
		}
		#endregion

		#region CanRead(int incident_id)
		public static bool CanRead(int incident_id)
		{
			int ProjectId = DBIncident.GetProject(incident_id);
			int UserId = Security.CurrentUser.UserID;
			bool reval = false;

			if(ProjectId > 0)
				reval =  CanDoAction(incident_id,"Read",UserId) || Security.CurrentUser.IsAlertService
						|| DBProject.GetSharingLevel(UserId, ProjectId) >= 0;
			else
				reval = CanDoAction(incident_id,"Read",UserId) || Security.CurrentUser.IsAlertService
						|| DBIncident.GetSharingLevel(UserId, incident_id) >= 0;
				
			return reval;
		/*
				bool retval = false;
				int UserId = Security.CurrentUser.UserID;

				IncidentSecurity incs = GetSecurity(incident_id);
				
				if (ProjectId > 0)
				{
					Project.ProjectSecurity ps = Project.GetSecurity(ProjectId);
				
					retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) 
						|| Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) 
						|| incs.IsManager || incs.IsCreator || incs.IsResource
						|| ps.IsTeamMember || ps.IsExecutiveManager || ps.IsSponsor || ps.IsStakeHolder 
											|| Security.CurrentUser.IsAlertService
						|| (DBProject.GetSharingLevel(UserId, ProjectId) >= 0);

				}
				else
				{
					retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) 
						|| Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) 
						|| Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager) 
						|| incs.IsManager || incs.IsCreator || incs.IsResource
						|| Security.CurrentUser.IsAlertService
						|| (DBIncident.GetSharingLevel(UserId, incident_id) >= 0);
				}

				return retval;
				*/
		}
		#endregion

		#region CanRead(int incident_id, int user_id)
		public static bool CanRead(int incident_id, int user_id)
		{
			int ProjectId = DBIncident.GetProject(incident_id);
			bool reval = false;

			if(ProjectId > 0)
				reval =  CanDoAction(incident_id,"Read",user_id) || Security.CurrentUser.IsAlertService
					|| DBProject.GetSharingLevel(user_id, ProjectId) >= 0;
			else
				reval = CanDoAction(incident_id,"Read",user_id) || Security.CurrentUser.IsAlertService
					|| DBIncident.GetSharingLevel(user_id, incident_id) >= 0;
				
			return reval;

			/*
			bool retval = false;

			IncidentSecurity incs = GetSecurity(incident_id, user_id);
			ArrayList secure_groups = User.GetListSecureGroupAll(user_id);
			int ProjectId = DBIncident.GetProject(incident_id);

			if (ProjectId > 0)
			{
				Project.ProjectSecurity ps = Project.GetSecurity(ProjectId, user_id);
				
				retval = secure_groups.Contains((int)InternalSecureGroups.PowerProjectManager)
					|| secure_groups.Contains((int)InternalSecureGroups.ExecutiveManager)
					|| incs.IsManager || incs.IsCreator || incs.IsResource
					|| ps.IsTeamMember || ps.IsExecutiveManager || ps.IsSponsor || ps.IsStakeHolder 
					|| Security.CurrentUser.IsAlertService
					|| (DBProject.GetSharingLevel(user_id, ProjectId) >= 0);
			}
			else
			{
				retval = secure_groups.Contains((int)InternalSecureGroups.PowerProjectManager)
					|| secure_groups.Contains((int)InternalSecureGroups.ExecutiveManager)
					|| secure_groups.Contains((int)InternalSecureGroups.HelpDeskManager)
					|| incs.IsManager || incs.IsCreator || incs.IsResource
					|| Security.CurrentUser.IsAlertService
					|| (DBIncident.GetSharingLevel(user_id, incident_id) >= 0);
			}

			return retval;
			*/
		}
		#endregion

		#region CanViewToDoList
		public static bool CanViewToDoList(int incident_id)
		{
			
			IncidentBoxDocument incDoc = IncidentBoxDocument.Load(GetIncidentBox(incident_id));

			return CanDoAction(incident_id, "CanViewToDoList",Security.UserID) && incDoc.GeneralBlock.AllowAddToDo;

			/*
			// Видеть ToDo List могут: PPM, Exec, Incident Manager, Incident Resources,
			//	участники проекта (Project!=null) и HDM (Project=null)

			bool retval = false;

			IncidentSecurity incs = GetSecurity(incident_id);
			retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) ||
				incs.IsManager || incs.IsResource;

			if (!retval)
			{
				int ProjectId = DBIncident.GetProject(incident_id);
				if (ProjectId > 0)
				{
					Project.ProjectSecurity ps = Project.GetSecurity(ProjectId);
					retval = ps.IsTeamMember || ps.IsExecutiveManager || ps.IsSponsor || ps.IsStakeHolder;
				}
				else if (Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
				{
					retval = true;
				}
			}
			return retval;
			*/
		}
		#endregion

		#region CanAddToDo
		public static bool CanAddToDo(int incident_id)
		{
			IncidentBoxDocument incDoc = IncidentBoxDocument.Load(GetIncidentBox(incident_id));
			return CanDoAction(incident_id, "AddToDo",Security.UserID) && incDoc.GeneralBlock.AllowAddToDo;
			/*
			// Добавлять ToDo можно в состоянии Active и New
			// Добавлять могут: PPM, Incident Manager, HDM (Project=null), Incident Resources с флагом CanManage
			bool retval = false;

			int StateId = DBIncident.GetState(incident_id);
			if (StateId == (int)ObjectStates.Upcoming 
				|| StateId == (int)ObjectStates.Active 
				|| StateId == (int)ObjectStates.ReOpen 
				|| StateId == (int)ObjectStates.OnCheck)
			{
				IncidentSecurity incs = GetSecurity(incident_id);
				retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
					incs.IsManager;

				if (!retval && Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
				{
					int ProjectId = DBIncident.GetProject(incident_id);
					if (ProjectId <= 0)
						retval = true;
				}
			}
			return retval;
			*/
		}
		#endregion

		#region CanDeleteToDo
		public static bool CanDeleteToDo(int incident_id)
		{
			return CanDoAction(incident_id,"DeleteToDo",Security.UserID);
			/*
			// Удалять могут: PPM, Incident Manager, HDM (Project=null), Incident Resources с флагом CanManage
			bool retval = false;

			IncidentSecurity incs = GetSecurity(incident_id);
			retval = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				incs.IsManager;

			if (!retval && Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
			{
				int ProjectId = DBIncident.GetProject(incident_id);
				if (ProjectId <= 0)
					retval = true;
			}
			return retval;
			*/
		}
		#endregion

		#region CanModifyResources
		public static bool CanModifyResources(int incident_id)
		{
			IncidentBoxDocument incDoc = IncidentBoxDocument.Load(GetIncidentBox(incident_id));
			return CanDoAction(incident_id, "AddToDo",Security.UserID) && incDoc.GeneralBlock.AllowAddResources;
		}

		public static bool CanModifyResourcesSecurity(int incident_id)
		{
			IncidentBoxDocument incDoc = IncidentBoxDocument.Load(GetIncidentBox(incident_id));
			return CanDoAction(incident_id, "AddToDo",Security.UserID);
		}
		#endregion

		#region CanViewFinances
		public static bool CanViewFinances(int incident_id)
		{
			return (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) 
				|| Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) 
				|| GetSecurity(incident_id).IsManager)
				&& DBIncident.GetProject(incident_id) > 0;
		}
		#endregion

		#region CanViewTimeTrackingInfo
		public static bool CanViewTimeTrackingInfo(int incident_id)
		{
			bool retval = (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
				|| Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager)
				|| Security.IsUserInGroup(InternalSecureGroups.TimeManager)
				|| GetSecurity(incident_id).IsManager);

			if (!retval)
			{
				int projectId = DBIncident.GetProject(incident_id);
				if (projectId > 0)
				{
					Mediachase.IBN.Business.Project.ProjectSecurity pSec = Project.GetSecurity(projectId);
					retval = pSec.IsManager || pSec.IsExecutiveManager;
				}
			}

			return retval;
		}
		#endregion

		#region GetSecurity
		public static IncidentSecurity GetSecurity(int incident_id)
		{
			return GetSecurity(incident_id, Security.CurrentUser.UserID);
		}

		public static IncidentSecurity GetSecurity(int incident_id, int user_id)
		{
			return new IncidentSecurity(incident_id, user_id);
		}
		#endregion

		#region GetSecurity2
		public static IncidentSecurity2 GetSecurity2(int incident_id)
		{
			return GetSecurity2(incident_id, Security.CurrentUser.UserID);
		}

		public static IncidentSecurity2 GetSecurity2(int incident_id, int user_id)
		{
			return new IncidentSecurity2(incident_id, user_id);
		}
		#endregion

		#region Create
		public static int Create(
			string title,
			string description,
			int type_id,
			int priority,
			int severity_id,
			int creator_id,
			DateTime creation_date)
		{
			PrimaryKeyId org_id = PrimaryKeyId.Empty;
			PrimaryKeyId contact_id = PrimaryKeyId.Empty;
			Common.GetDefaultClient(PortalConfig.IncidentDefaultValueClientField, out contact_id, out org_id);

			ArrayList alCategories = Common.StringToArrayList(PortalConfig.IncidentDefaultValueGeneralCategoriesField);
			ArrayList alIssCategories = Common.GetDefaultIncidentCategories();
			
			return Create(title, description, -1, type_id, priority, severity_id, 
				0, 0, 0, 0, creator_id, alCategories, alIssCategories, null, null, false, 
				creation_date, -1, contact_id, org_id, -1);
		}

		public static int Create(
			string title,
			string description,
			int project_id,
			int type_id,
			int priority,
			int severity_id,
			int task_time,
			int expected_duration,
			int expected_response_time,
			int expected_assign_time,
			int creator_id,
			ArrayList categories,
			ArrayList incident_categories,
			string FileName,
			System.IO.Stream _inputStream,
			bool IsEmail,
			DateTime creation_date,
			int IncidentBoxId,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid
			)
		{
			return Create(title, description, project_id, type_id, priority, severity_id,
				task_time, expected_duration, expected_response_time, expected_assign_time,
				creator_id, categories, incident_categories, FileName, _inputStream, IsEmail,
				creation_date, IncidentBoxId, contactUid, orgUid, -1);
		}

		public static int Create(
			string title,
			string description,
			int project_id,
			int type_id,
			int priority,
			int severity_id,
			int task_time,
			int expected_duration,
			int expected_response_time,
			int expected_assign_time,
			int creator_id,
			ArrayList categories,
			ArrayList incident_categories,
			string FileName,
			System.IO.Stream _inputStream,
			bool IsEmail,
			DateTime creation_date,
			int IncidentBoxId,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid,
			int mustResponsibleId
			)
		{
			return Create(title, description, project_id, type_id, priority, severity_id, 
				task_time, expected_duration, expected_response_time, expected_assign_time,
				creator_id, categories, incident_categories, FileName, _inputStream, IsEmail,
				creation_date, IncidentBoxId, contactUid, orgUid, mustResponsibleId,
				true, DateTime.UtcNow);
		}

		public static int Create(
			string title,
			string description,
			int project_id,
			int type_id,
			int priority,
			int severity_id,
			int task_time,
			int expected_duration, 
			int expected_response_time,
			int expected_assign_time,
			int creator_id,
			ArrayList categories,
			ArrayList incident_categories,
			string FileName,
			System.IO.Stream _inputStream,
			bool IsEmail,
			DateTime creation_date, 
			int IncidentBoxId,
			PrimaryKeyId contactUid,
			PrimaryKeyId orgUid,
			int mustResponsibleId,
			bool useDuration,
			DateTime expResolveDate
			)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			if (!CanCreate(project_id))
				throw new AccessDeniedException();

			// DateTime creation_date = DateTime.UtcNow;

			title = title.Replace("<", "&lt;").Replace(">", "&gt;");

			object oProjectId = null;
			if (project_id > 0)
				oProjectId = project_id;

			IncidentBox box = null;

			if(IncidentBoxId <= 0)
			{
				IncidentInfo info = new IncidentInfo();
				info.Description = description;
				info.GeneralCategories = categories;
				info.IncidentCategories = incident_categories;
				info.PriorityId = priority;
				info.SeverityId = severity_id;
				info.Title = title;
				info.TypeId = type_id;
				info.ContactUid = contactUid;
				info.OrgUid = orgUid;

				if (project_id > 0)
					info.ProjectId = project_id;

				if(creator_id < 0)
					info.CreatorId = Security.CurrentUser.UserID;
				else
					info.CreatorId = creator_id;

				box = IncidentBoxRule.Evaluate(info);
				if (box == null)
					throw new ArgumentNullException("box");
				IncidentBoxId = box.IncidentBoxId;
			}
			else
				box = IncidentBox.Load(IncidentBoxId);

			// OZ: Fixed expected_duration and expected_response_time
			if (expected_duration <= 0)
				expected_duration = box.Document.GeneralBlock.ExpectedDuration;
			if (expected_response_time <= 0)
				expected_response_time = box.Document.GeneralBlock.ExpectedResponseTime;
			if (expected_assign_time <= 0)
				expected_assign_time = box.Document.GeneralBlock.ExpectedAssignTime;
			//

			if (task_time <= 0)
				task_time = box.Document.GeneralBlock.TaskTime;

			int stateId, managerId, responsibleId;
			bool isResposibleGroup;
			ArrayList users = new ArrayList();

			Issue2.GetIssueBoxSettings(IncidentBoxId, out stateId, out managerId, out responsibleId, out isResposibleGroup, users);

			// O.R. [2008-09-09]: Exclude inactive users
			if (responsibleId > 0 && User.GetUserActivity(responsibleId) != User.UserActivity.Active)
				responsibleId = -1;

			ArrayList activeUsers = new ArrayList();
			foreach (int userId in users)
			{
				if (User.GetUserActivity(userId) == User.UserActivity.Active)
					activeUsers.Add(userId);
			}
			//

			//ak [2009-06-10] responsible from creation page (IncidentEdit1)
			if (mustResponsibleId > 0)
			{
				responsibleId = mustResponsibleId;
				isResposibleGroup = false;
			}

			if (useDuration)
				expResolveDate = DBCalendar.GetFinishDateByDuration(box.Document.GeneralBlock.CalendarId, creation_date, expected_duration);
			else
			{
				expResolveDate = User.GetUTCDate(expResolveDate);
				expected_duration = DBCalendar.GetDurationByFinishDate(box.Document.GeneralBlock.CalendarId, creation_date, expResolveDate);
			}
			
			int issueId;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				issueId = DBIncident.Create(oProjectId, creator_id, 
					title, description, creation_date, 
					type_id, priority, stateId, severity_id, IsEmail, task_time, 
					IncidentBoxId, responsibleId, isResposibleGroup?1:0, contactUid, orgUid,
					expected_duration, expected_response_time, expected_assign_time, expResolveDate, useDuration);

				string Identifier = TicketUidUtil.Create(box.IdentifierMask,issueId);
				DBIncident.UpdateIdentifier(issueId, Identifier);

				// Categories
				foreach(int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)INCIDENT_TYPE, issueId, CategoryId);
				}

				// Incident Categories
				foreach(int CategoryId in incident_categories)
				{
					DBIncident.AssignIncidentCategory(issueId, CategoryId);
				}

				foreach(int UserId in activeUsers)
				{
					DbIssue2.ResponsibleGroupAddUser(issueId, UserId);
				}

				ForumStorage.NodeContentType _type = (FileName != String.Empty && _inputStream != null) ?
					ForumStorage.NodeContentType.TextWithFiles : ForumStorage.NodeContentType.Text;
				// Forum
				BaseIbnContainer destContainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", issueId));
				ForumStorage forumStorage = (ForumStorage)destContainer.LoadControl("ForumStorage");

				if(description != String.Empty || (FileName != String.Empty && _inputStream != null))
				{
					ForumThreadNodeInfo info = forumStorage.CreateForumThreadNode(description, creator_id, (int)_type);
					if(FileName != String.Empty && _inputStream != null)
					{
						BaseIbnContainer forumContainer = BaseIbnContainer.Create("FileLibrary", string.Format("ForumNodeId_{0}", info.Id));
						FileStorage fs = (FileStorage)forumContainer.LoadControl("FileStorage");
						fs.SaveFile(FileName, _inputStream);
					}

					ForumThreadNodeSettingCollection settings1 = new ForumThreadNodeSettingCollection(info.Id);
					settings1.Add(ForumThreadNodeSetting.Internal, "1");
					settings1.Add(ForumThreadNodeSetting.Question, "1");
				}

				// O.R: Recalculating project TaskTime
				if (project_id > 0 && task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				// O.R.[2008-12-16]: Recalculate Current Responsible
				DBIncident.RecalculateCurrentResponsible(issueId);

				// O.R. [2009-06-15]: Recalculate project dates
				if (project_id > 0 && PortalConfig.UseIncidentDatesForProject && !useDuration)
					Project.RecalculateDates(project_id);

				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Created, issueId);
				if(project_id > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_IssueList_IssueAdded, project_id, issueId);

				Issue2.SendAlertsForNewIssue(issueId, managerId, responsibleId, activeUsers, null);

				tran.Commit();
			}

			return issueId;
		}
		#endregion

		#region Delete
		public static void Delete(int incident_id)
		{
			Delete(incident_id, true);
		}

		internal static void Delete(int incident_id, bool checkAccess)
		{
			if(checkAccess && !CanDelete(incident_id))
				throw new AccessDeniedException();

			ArrayList allRemovedToDo	=	new ArrayList();
			using(IDataReader todoReader = Incident.GetListToDo(incident_id))
			{
				while(todoReader.Read())
				{
					int ToDoId = (int)todoReader["ToDoId"];
					allRemovedToDo.Add(ToDoId);
				}
			}

			int projectId = DBIncident.GetProject(incident_id);

			// Forum
			BaseIbnContainer FOcontainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", incident_id));
			ForumStorage forumStorage = (ForumStorage)FOcontainer.LoadControl("ForumStorage");
			ForumThreadNodeInfo[] forumNodes = forumStorage.GetForumThreadNodes();

			// Transaction
			using(DbTransaction tran = DbTransaction.Begin())
			{
				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Deleted, incident_id);
				if(projectId > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_IssueList_IssueDeleted, projectId, incident_id);

				// Delete Todo and don't send todo alerts
				foreach(int ToDoId in allRemovedToDo)
				{
					ToDo.DeleteSimple(ToDoId);
				}


				foreach (ForumThreadNodeInfo node in forumNodes)
				{
					// Delete Emails
					if (node.ContentType == ForumStorage.NodeContentType.EMail)
					{
						EMailMessage.Delete(node.EMailMessageId);
					}

					// Delete Forum Files
					BaseIbnContainer FScontainer = BaseIbnContainer.Create("FileLibrary",string.Format("ForumNodeId_{0}",node.Id));
					FileStorage FSfileStorage = (FileStorage)FScontainer.LoadControl("FileStorage");
					FSfileStorage.DeleteAll();
				}

				// Delete Forum
				forumStorage.Delete();

				MetaObject.Delete(incident_id, "IncidentsEx");
				DBIncident.Delete(incident_id);

				// O.R: Recalculating project TaskTime
				if (projectId > 0)
					TimeTracking.RecalculateProjectTaskTime(projectId);

				tran.Commit();
			}
		}
		#endregion

		#region GetIncident
		/// <summary>
		/// Reader returns fields:
		///  IncidentId, ProjectId, ProjectTitle, CreatorId, 
		///  Title, Description, Resolution, Workaround, CreationDate, 
		///  TypeId, TypeName, PriorityId, PriorityName, 
		///  SeverityId, SeverityName, IsEmail, MailSenderEmail, StateId, TaskTime, 
		///  IncidentBoxId, OrgUid, ContactUid, ClientName, ExpectedDuration, 
		///  ExpectedResponseTime, ActualFinishDate, Identifier,
		///  ModifiedDate, ExpectedResponseDate, ExpectedResolveDate, ActualOpenDate,
		///  ControllerId, ResponsibleGroupState, TotalMinutes, TotalApproved, IsOverdue, 
		///  IsNewMessage, ExpectedAssignTime, ExpectedAssignDate, CurrentResponsibleId,
		///  UseDuration, ProjectCode
		/// </summary>
		/// <param name="IncidentId"></param>
		/// <returns></returns>
		public static IDataReader GetIncident(int incident_id)
		{
			return GetIncident(incident_id, true);
		}

		public static IDataReader GetIncident(int incident_id, bool checkAccess)
		{
			if(checkAccess && !CanRead(incident_id))
				throw new AccessDeniedException();

			return DBIncident.GetIncident(incident_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetIncidentTrackingState
		/// <summary>
		/// Reader returns fields:
		///  IncidentId, StateId, StateName, 
		///  ResponsibleId, IsResponsibleGroup, ResponsibleGroupState /*0 - empty list; 1 - List isn't empty; 2 - AllSayDeny
		/// </summary>
		/// <param name="IncidentId"></param>
		/// <returns></returns>
		
		public static IDataReader GetIncidentTrackingState(int incident_id)
		{
			return GetIncidentTrackingState(incident_id, true);
		}

		public static IDataReader GetIncidentTrackingState(int incident_id, bool checkAccess)
		{
			if(checkAccess && !CanRead(incident_id))
				throw new AccessDeniedException();

			return DBIncident.GetIncidentTrackingState(incident_id, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListIncidentStates
		public static DataTable GetListIncidentStates(int incident_id)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("StateId",Type.GetType("System.Int32")); 
			dt.Columns.Add("StateName",Type.GetType("System.String"));

			int StateId = -1;
			using(IDataReader reader = GetIncident(incident_id, false))
			{
				if(reader.Read())
					StateId = (int)reader["StateId"];
			}

			
			//TODO: xml
			using(IDataReader reader = DBIncident.GetListIncidentStates(0, Security.CurrentUser.LanguageId))
			{
				while(reader.Read())
				{
					int toStateId = (int)reader["StateId"];
					if(IsTransitionAllowed(incident_id,(ObjectStates)StateId,(ObjectStates)toStateId) || toStateId == StateId)
					{
						DataRow row = dt.NewRow();
						row["StateId"] = reader["StateId"];
						row["StateName"] = reader["StateName"];
						dt.Rows.Add(row);
					}
				}
			}
			dt.AcceptChanges();
			return dt;
		}
		#endregion



		#region GetListProjects
		/// <summary>
		/// Reader returns fields:
		///		ProjectId, Title
		/// </summary>
		public static IDataReader GetListProjects()
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
				return DBProject.GetListActiveProjects();
			else
				return DBProject.GetListActiveProjectsByUser(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId);
		}

		public static DataTable GetListProjectsDataTable()
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
				return DBProject.GetListActiveProjectsDataTable();
			else
				return DBProject.GetListActiveProjectsByUserDataTable(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListPriorities
		/// <summary>
		/// Reader returns fields:
		///		PriorityId, PriorityName 
		/// </summary>
		public static IDataReader GetListPriorities()
		{
			return DBCommon.GetListPriorities(Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListIncidentTypes
		/// <summary>
		/// Reader returns fields:
		///  TypeId, TypeName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentTypes()
		{
			return DBIncident.GetListIncidentTypes();
		}
		#endregion

		#region GetListIncidentSeverity
		/// <summary>
		/// Reader returns fields:
		///  SeverityId, SeverityName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentSeverity()
		{
			return DBIncident.GetListIncidentSeverity();
		}
		#endregion

		#region GetListCategoriesAll
		/// <summary>
		/// Reader returns fields:
		///		CategoryId, CategoryName 
		/// </summary>
		public static IDataReader GetListCategoriesAll()
		{
			return DBCommon.GetListCategories();
		}
		#endregion

		#region GetListCategories
		/// <summary>
		/// Reader returns fields:
		///		CategoryId, CategoryName 
		/// </summary>
		public static IDataReader GetListCategories(int incident_id)
		{
			return DBCommon.GetListCategoriesByObject((int)INCIDENT_TYPE, incident_id);
		}
		#endregion

		#region GetListDiscussions
		/// <summary>
		/// Reader returns fields:
		///		DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
		/// </summary>
		public static IDataReader GetListDiscussions(int incident_id)
		{
			return Common.GetListDiscussions(INCIDENT_TYPE, incident_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListDiscussionsDataTable
		/// <summary>
		/// Reader returns fields:
		///		DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
		/// </summary>
		public static DataTable GetListDiscussionsDataTable(int incident_id)
		{
			return Common.GetListDiscussionsDataTable(INCIDENT_TYPE, incident_id, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region AddDiscussion
		public static void AddDiscussion(int incident_id, string text)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			DateTime creation_date = DateTime.UtcNow;
			int UserId = Security.CurrentUser.UserID;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				int CommentId = DBCommon.AddDiscussion((int)INCIDENT_TYPE, incident_id, UserId, creation_date, text);

				SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_CommentList_CommentAdded, incident_id, CommentId);

				tran.Commit();
			}
		}
		#endregion

		#region GetListIncidentsByProject
		/// <summary>
		/// Reader returns fields:
		///  IncidentId, ProjectId, ProjectTitle, CreatorId, CreatorName, 
		///  ManagerId, ManagerName, Title, CreationDate, 
		///  TypeId, TypeName, PriorityId, PriorityName, 
		///  SeverityId, SeverityName, StateId, CanEdit, CanDelete
		/// </summary>
		/// <param name="project_id"></param>
		/// <returns></returns>
		public static IDataReader GetListIncidentsByProject(int project_id)
		{
			return DBIncident.GetListIncidentsByProject(project_id, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListIncidentsByProjectDataTable
		/// <summary>
		/// Reader returns fields:
		///  IncidentId, ProjectId, ProjectTitle, CreatorId, CreatorName, 
		///  ManagerId, ManagerName, Title, CreationDate, 
		///  TypeId, TypeName, PriorityId, PriorityName, 
		///  SeverityId, SeverityName, CanEdit, CanDelete
		/// </summary>
		/// <param name="project_id"></param>
		/// <returns></returns>
		public static DataTable GetListIncidentsByProjectDataTable(int project_id)
		{
			return DBIncident.GetListIncidentsByProjectDataTable(project_id, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion
		
		#region GetListToDo
		/// <summary>
		/// ToDoId, Title, Description, StartDate, FinishDate, ActualFinishDate, PercentCompleted, 
		///	 IsCompleted, CompletedBy, StateId
		/// </summary>
		/// <param name="incident_id"></param>
		/// <returns></returns>
		public static IDataReader GetListToDo(int incident_id)
		{
			return DBIncident.GetListToDo(incident_id, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		public static DataTable GetListToDoDataTable(int incident_id)
		{
			return DBIncident.GetListToDoDataTable(incident_id, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetTitle
		public static string GetTitle(int incident_id)
		{
			return DBIncident.GetTitle(incident_id);
		}
		#endregion

		#region GetListNotAssignedIncidents
		/// <summary>
		/// Reader returns fields:
		///  IncidentId, Title, PriorityId, PriorityName, 
		///  CreationDate, CreatorId, StateId, Identifier, IsOverdue
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListNotAssignedIncidents(int ProjectId)
		{
			return DBIncident.GetListNotAssignedIncidents(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		public static DataTable GetListNotAssignedIncidentsDataTable(int ProjectId)
		{
			return DBIncident.GetListNotAssignedIncidentsDataTable(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListIncidentCreators
		/// <summary>
		///  UserId, UserName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentCreators()
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
				return DBIncident.GetListIncidentCreators();
			else
				return DBIncident.GetListIncidentCreatorsForPartner(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListIncidentManagers
		/// <summary>
		///  UserId, UserName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentManagers()
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
				return DBIncident.GetListIncidentManagers();
			else
				return DBIncident.GetListIncidentManagersForPartner(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListIncidentsByFilterDataTable
		/// <summary>
		/// IncidentId, ProjectId, ProjectTitle, IncidentBoxId, IncidentBoxName, ClientName,
		/// CreatorId, CreatorName, ControllerId, ControllerName, ManagerId, ManagerName,
		/// ResponsibleId, ResponsibleName, IsResponsibleGroup, ResponsibleGroupState,
		/// ContactUid, OrgUid, Title, CreationDate, ModifiedDate, ActualOpenDate, ActualFinishDate, 
		/// ExpectedResponseDate, ExpectedResolveDate, ExpectedAssignDate,
		/// TypeId, TypeName, PriorityId, PriorityName, 
		/// StateId, StateName, SeverityId, Identifier, IsOverdue, IsNewMessage, CurrentResponsibleId, 
		/// CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsByFilterDataTable(int ProjectId, int ManagerId,
			int CreatorId, int ResourceId, int ResponsibleId, PrimaryKeyId orgUid, PrimaryKeyId contactUid, int IncidentBoxId,
			int PriorityId, int TypeId, int StateId, 
			int SeverityId, string Keyword,	int CategoryType, int IncidentCategoryType)
		{
			return DBIncident.GetListIncidentsByFilterDataTable(ProjectId, ManagerId,
				CreatorId, ResourceId, ResponsibleId, orgUid, contactUid, IncidentBoxId, PriorityId, TypeId, StateId, SeverityId, Keyword, 
				Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, 
				Security.CurrentUser.LanguageId, CategoryType, IncidentCategoryType);
		}

		public static DataTable GetListIncidentsByFilterDataTable(int ProjectId, int ManagerId,
			int CreatorId, int ResourceId, int ResponsibleId, 
			PrimaryKeyId orgUid, PrimaryKeyId contactUid, 
			int IncidentBoxId, int PriorityId, int TypeId, int StateId,
			int SeverityId, string Keyword, int CategoryType, int IncidentCategoryType,
			bool onlyUnanswered, bool onlyOverdue)
		{
			DataTable dt = DBIncident.GetListIncidentsByFilterDataTable(ProjectId, ManagerId,
				CreatorId, ResourceId, ResponsibleId, orgUid, contactUid, IncidentBoxId, PriorityId, TypeId, StateId, SeverityId, Keyword,
				Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId,
				Security.CurrentUser.LanguageId, CategoryType, IncidentCategoryType);
			if (onlyUnanswered)
			{
				DataTable dtClone = dt.Clone();
				foreach (DataRow dr in dt.Rows)
				{
					if ((bool)dr["IsNewMessage"])
					{
						DataRow drNew = dtClone.NewRow();
						drNew.ItemArray = dr.ItemArray;
						dtClone.Rows.Add(drNew);
					}
				}
				dt = dtClone.Copy();
			}
			if (onlyOverdue)
			{
				DataTable dtClone = dt.Clone();
				foreach (DataRow dr in dt.Rows)
				{
					if ((bool)dr["IsOverdue"])
					{
						DataRow drNew = dtClone.NewRow();
						drNew.ItemArray = dr.ItemArray;
						dtClone.Rows.Add(drNew);
					}
				}
				dt = dtClone.Copy();
			}
			return dt;
		}

		public static DataTable GetListIncidentsByFilterDataTable(string keyword, FilterElementCollection fec,
			FilterElementCollection fecAbove)
		{
			#region Variables
			int projId = 0; int iManId = 0; int iRespId = 0; int iCreatorId = 0; int iResId = 0;
			int priority_id = -1; int issbox_id = 0; int type_id = 0; 
			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			int state_id = 0; int severity_id = 0; int genCategory_type = 0; int issCategory_type = 0;
			bool isUnansweredOnly = false; bool isOverdueOnly = false;
			#endregion

			foreach (FilterElement fe in fec)
			{
				string source = fe.Source;
				string value = fe.Value.ToString();
				if (fe.ValueIsTemplate)
					value = TemplateResolver.ResolveAll(value);
				#region BindVariables
				switch (source)
				{
					case Incident.ProjectFilterKey:
						projId = int.Parse(value);
						break;
					case Incident.ManagerFilterKey:
						iManId = int.Parse(value);
						break;
					case Incident.ResponsibleFilterKey:
						iRespId = int.Parse(value);
						break;
					case Incident.CreatorFilterKey:
						iCreatorId = int.Parse(value);
						break;
					case Incident.PriorityFilterKey:
						priority_id = int.Parse(value);
						break;
					case Incident.IssueBoxFilterKey:
						issbox_id = int.Parse(value);
						break;
					case Incident.TypeFilterKey:
						type_id = int.Parse(value);
						break;
					case Incident.ClientFilterKey:
						GetContactId(value, out orgUid, out contactUid);
						break;
					case Incident.StatusFilterKey:
						state_id = int.Parse(value);
						break;
					case Incident.SeverityFilterKey:
						severity_id = int.Parse(value);
						break;
					case Incident.GenCategoryTypeFilterKey:
						genCategory_type = int.Parse(value);
						break;
					case Incident.GenCategoriesFilterKey:
						ArrayList alGenCats = new ArrayList();
						string[] mas = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in mas)
						{
							if (!alGenCats.Contains(int.Parse(s)))
								alGenCats.Add(int.Parse(s));
						}
						Incident.SaveGeneralCategories(alGenCats);
						break;
					case Incident.IssueCategoryTypeFilterKey:
						issCategory_type = int.Parse(value);
						break;
					case Incident.IssueCategoriesFilterKey:
						ArrayList alIssCats = new ArrayList();
						string[] masIss = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in masIss)
						{
							if (!alIssCats.Contains(int.Parse(s)))
								alIssCats.Add(int.Parse(s));
						}
						Incident.SaveIncidentCategories(alIssCats);
						break;
					case Incident.UnansweredFilterKey:
						if (bool.Parse(value))
							isUnansweredOnly = true;
						break;
					case Incident.OverdueFilterKey:
						if (bool.Parse(value))
							isOverdueOnly = true;
						break;
					default:
						break;
				}
				#endregion
			}

			foreach (FilterElement fe in fecAbove)
			{
				string source = fe.Source;
				string value = fe.Value.ToString();
				if (fe.ValueIsTemplate)
					value = TemplateResolver.ResolveAll(value);
				#region BindVariables
				switch (source)
				{
					case Incident.ProjectFilterKey:
						projId = int.Parse(value);
						break;
					case Incident.ManagerFilterKey:
						iManId = int.Parse(value);
						break;
					case Incident.ResponsibleFilterKey:
						iRespId = int.Parse(value);
						break;
					case Incident.CreatorFilterKey:
						iCreatorId = int.Parse(value);
						break;
					case Incident.PriorityFilterKey:
						priority_id = int.Parse(value);
						break;
					case Incident.IssueBoxFilterKey:
						issbox_id = int.Parse(value);
						break;
					case Incident.TypeFilterKey:
						type_id = int.Parse(value);
						break;
					case Incident.ClientFilterKey:
						GetContactId(value, out orgUid, out contactUid);
						break;
					case Incident.StatusFilterKey:
						state_id = int.Parse(value);
						break;
					case Incident.SeverityFilterKey:
						severity_id = int.Parse(value);
						break;
					case Incident.GenCategoryTypeFilterKey:
						genCategory_type = int.Parse(value);
						break;
					case Incident.GenCategoriesFilterKey:
						ArrayList alGenCats = new ArrayList();
						string[] mas = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in mas)
						{
							if (!alGenCats.Contains(int.Parse(s)))
								alGenCats.Add(int.Parse(s));
						}
						Incident.SaveGeneralCategories(alGenCats);
						break;
					case Incident.IssueCategoryTypeFilterKey:
						issCategory_type = int.Parse(value);
						break;
					case Incident.IssueCategoriesFilterKey:
						ArrayList alIssCats = new ArrayList();
						string[] masIss = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in masIss)
						{
							if (!alIssCats.Contains(int.Parse(s)))
								alIssCats.Add(int.Parse(s));
						}
						Incident.SaveIncidentCategories(alIssCats);
						break;
					case Incident.UnansweredFilterKey:
						if (bool.Parse(value))
							isUnansweredOnly = true;
						else
							isUnansweredOnly = false;
						break;
					case Incident.OverdueFilterKey:
						if (bool.Parse(value))
							isOverdueOnly = true;
						else
							isOverdueOnly = false;
						break;
					default:
						break;
				}
				#endregion
			}

			DataTable dt = DBIncident.GetListIncidentsByFilterDataTable(projId, iManId,
				iCreatorId, iResId, iRespId, orgUid, contactUid, issbox_id, priority_id,
				type_id, state_id, severity_id, keyword,
				Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId,
				Security.CurrentUser.LanguageId, genCategory_type, issCategory_type);

			if (isUnansweredOnly)
			{
				DataTable dtClone = dt.Clone();
				foreach (DataRow dr in dt.Rows)
				{
					if ((bool)dr["IsNewMessage"])
					{
						DataRow drNew = dtClone.NewRow();
						drNew.ItemArray = dr.ItemArray;
						dtClone.Rows.Add(drNew);
					}
				}
				dt = dtClone.Copy();
			}
			if (isOverdueOnly)
			{
				DataTable dtClone = dt.Clone();
				foreach (DataRow dr in dt.Rows)
				{
					if ((bool)dr["IsOverdue"])
					{
						DataRow drNew = dtClone.NewRow();
						drNew.ItemArray = dr.ItemArray;
						dtClone.Rows.Add(drNew);
					}
				}
				dt = dtClone.Copy();
			}
			return dt;
		}
		#endregion

		#region GetListIncidentsByFilterGroupedByProject
		/// <summary>
		/// ContactUid, OrgUid, IsClient, ClientName, ProjectId, ProjectTitle, 
		///IncidentId, Title, CreatorId, CreatorName, ControllerId, ControllerName, ManagerId, 
		///ManagerName,ResponsibleId , ResponsibleName , IsResponsibleGroup, ResponsibleGroupState ,
		///CreationDate, TypeId, TypeName, PriorityId, PriorityName,
		///StateId, StateName, SeverityId, CanEdit, CanDelete, IsCollapsed, IsOverdue, IsNewMessage,
		/// CurrentResponsibleId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsByFilterGroupedByProject(int ProjectId, int ManagerId,
			int CreatorId,  int ResourceId, int ResponsibleId, PrimaryKeyId orgUid, PrimaryKeyId contactUid, int IncidentBoxId,
			int PriorityId, int TypeId, int StateId, 
			int SeverityId, string Keyword,	int CategoryType, int IncidentCategoryType)
		{
			return DBIncident.GetListIncidentsByFilterGroupedByProject(ProjectId, ManagerId, 
				CreatorId, ResourceId,  ResponsibleId, orgUid, contactUid, IncidentBoxId,
				PriorityId, TypeId, StateId, SeverityId, Keyword, 
				Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, 
				Security.CurrentUser.LanguageId, CategoryType, IncidentCategoryType);
		}
		#endregion

		#region GetListIncidentsByFilterGroupedByClient
		/// <summary>
		///ContactUid, OrgUid, IsClient, ClientName, ProjectId, ProjectTitle, 
		///IncidentId, Title, CreatorId, CreatorName, ControllerId, ControllerName, ManagerId, 
		///ManagerName,ResponsibleId , ResponsibleName , IsResponsibleGroup, ResponsibleGroupState ,
		///CreationDate, TypeId, TypeName, PriorityId, PriorityName,
		///StateId, StateName, SeverityId, CanEdit, CanDelete, IsCollapsed, IsOverdue, IsNewMessage,
		/// CurrentResponsibleId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsByFilterGroupedByClient(int ProjectId, int ManagerId,
			int CreatorId,  int ResourceId, int ResponsibleId, PrimaryKeyId orgUid, PrimaryKeyId contactUid, int IncidentBoxId,
			int PriorityId, int TypeId, int StateId, 
			int SeverityId, string Keyword,	int CategoryType, int IncidentCategoryType)
		{
			return DBIncident.GetListIncidentsByFilterGroupedByClient(ProjectId, ManagerId, 
				CreatorId, ResourceId,  ResponsibleId, orgUid, contactUid, IncidentBoxId,
				PriorityId, TypeId, StateId, SeverityId, Keyword, 
				Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, 
				Security.CurrentUser.LanguageId, CategoryType, IncidentCategoryType);
		}
		#endregion

		#region GetListIncidentsUpdatedByUser
		/// <summary>
		///		IncidentId, Title, StateId, LastSavedDate
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentsUpdatedByUser(int Days, int ProjectId)
		{
			return DBIncident.GetListIncidentsUpdatedByUser(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}
		#endregion

		#region GetListIncidentsUpdatedByUserDataTable
		/// <summary>
		///		IncidentId, Title, LastSavedDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsUpdatedByUserDataTable(int Days, int ProjectId)
		{
			return DBIncident.GetListIncidentsUpdatedByUserDataTable(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}
		#endregion

		#region GetListIncidentsUpdatedForUser
		/// <summary>
		///		IncidentId, Title, CreatorId, ManagerId, LastEditorId, LastSavedDate,
		///		ProjectId, ProjectName, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentsUpdatedForUser(int Days, int ProjectId)
		{
			return DBIncident.GetListIncidentsUpdatedForUser(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}
		#endregion

		#region GetListIncidentsUpdatedForUserDataTable
		/// <summary>
		///		IncidentId, Title, CreatorId, ManagerId, LastEditorId, LastSavedDate,
		///		ProjectId, ProjectName, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsUpdatedForUserDataTable(int Days, int ProjectId)
		{
			return DBIncident.GetListIncidentsUpdatedForUserDataTable(ProjectId, Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Days);
		}
		#endregion

		#region GetListIncidentsByKeyword
		/// <summary>
		/// IncidentId, CreatorId, ManagerId, Title, Description, PriorityId, PriorityName, 
		/// CreationDate, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentsByKeyword(string Keyword)
		{
			return DBIncident.GetListIncidentsByKeyword(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, Keyword);
		}
		#endregion

		#region GetListIncidentsByKeywordDataTable
		/// <summary>
		/// IncidentId, CreatorId, ManagerId, Title, Description, PriorityId, PriorityName, 
		/// CreationDate, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsByKeywordDataTable(string Keyword)
		{
			return DBIncident.GetListIncidentsByKeywordDataTable(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId, Keyword);
		}
		#endregion

		#region GetListIncidentCategories
		/// <summary>
		/// Reader returns fields:
		///  CategoryId, CategoryName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentCategories()
		{
			return DBIncident.GetListIncidentCategories(0);
		}
		#endregion

		#region GetListIncidentCategoriesByIncident
		/// <summary>
		/// Reader returns fields:
		///  CategoryId, CategoryName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentCategoriesByIncident(int IncidentId)
		{
			return DBIncident.GetListIncidentCategoriesByIncident(IncidentId);
		}
		#endregion

		#region UploadFile
		public static void UploadFile(int incident_id, string FileName, System.IO.Stream _inputStream)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(FileName!=null)
				{
					string ContainerName = "FileLibrary";
					string ContainerKey = UserRoleHelper.CreateIssueContainerKey(incident_id);

					BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
					FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");
					fs.SaveFile(fs.Root.Id, FileName, _inputStream);
					//	Asset.Create(incident_id,ObjectTypes.Issue, FileName,"", FileName, _inputStream, true);
				}

				tran.Commit();
			}
		}
		#endregion

		#region GetListIncidentsForChangeableRoles
		/// <summary>
		/// Reader returns fields:
		///  IncidentId, Title, IsManager, CanView, CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentsForChangeableRoles(int UserId)
		{
			return DBIncident.GetListIncidentsForChangeableRoles(UserId, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetIncidentStatistic
		/// <summary>
		/// Reader returns fields:
		///  IncidentCount, Pop3IncidentCount, NewIncidentCount, ActiveIncidentCount, 
		///  ClosedIncidentCount, AvgTimeInNewState, AvgTimeInActiveState, 
		///  AvgTimeForResolveClosed, AvgTimeForResolveAll,
		///  OnCheckIncidentCount, ReOpenIncidentCount, SuspendedIncidentCount
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetIncidentStatistic(int ProjectId)
		{
			return GetIncidentStatistic(ProjectId, 0);
		}

		public static IDataReader GetIncidentStatistic(int ProjectId, int ManagerId)
		{
			return DBIncident.GetIncidentStatistic(ProjectId, ManagerId);
		}
		#endregion

		#region GetListIncidentStatisticByTypeDataTable
		/// <summary>
		/// Reader returns fields:
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticByTypeDataTable()
		{
			return GetListIncidentStatisticByTypeDataTable(0, 0);
		}

		public static DataTable GetListIncidentStatisticByTypeDataTable(int ProjectId, int ManagerId)
		{
			return DBIncident.GetListIncidentStatisticByTypeDataTable(ProjectId, ManagerId);
		}
		#endregion

		#region GetListIncidentStatisticByGeneralCategoryDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticByGeneralCategoryDataTable()
		{
			return GetListIncidentStatisticByGeneralCategoryDataTable(0, 0);
		}

		public static DataTable GetListIncidentStatisticByGeneralCategoryDataTable(int ProjectId, int ManagerId)
		{
			return DBIncident.GetListIncidentStatisticByGeneralCategoryDataTable(ProjectId, ManagerId);
		}
		#endregion

		#region GetListIncidentStatisticByIncidentCategoryDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticByIncidentCategoryDataTable()
		{
			return GetListIncidentStatisticByIncidentCategoryDataTable(0, 0);
		}

		public static DataTable GetListIncidentStatisticByIncidentCategoryDataTable(int ProjectId, int ManagerId)
		{
			return DBIncident.GetListIncidentStatisticByIncidentCategoryDataTable(ProjectId, ManagerId);
		}
		#endregion

		#region GetListProjectsForIncidents
		/// <summary>
		/// Reader returns fields:
		///		ProjectId, Title, StatusId, IncidentCount
		/// </summary>
		public static IDataReader GetListProjectsForIncidents()
		{
			return DBIncident.GetListProjectsForIncidents(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListContactsForIncidents
		/// <summary>
		/// Reader returns fields:
		///		ObjectId, ObjectTypeId, ClientName, IncidentCount
		/// </summary>
		public static IDataReader GetListContactsForIncidents()
		{
			return DBIncident.GetListContactsForIncidents(Security.CurrentUser.UserID);
		}

		public static DataTable GetListContactsForIncidentsDataTable()
		{
			return DBIncident.GetListContactsForIncidentsDataTable(Security.CurrentUser.UserID);
		}

		public static DataTable GetListContactsForIncidentsDataTable(string keyword, int count)
		{
			DataTable dt = DBIncident.GetListContactsForIncidentsDataTable(Security.CurrentUser.UserID);
			DataTable dtClone = dt.Clone();
			DataView dv = dt.DefaultView;
			if(!String.IsNullOrEmpty(keyword))
				dv.RowFilter = String.Format("ClientName LIKE '%{0}%'", keyword);
			dv.Sort = "IncidentCount DESC";
			int index = 0;
			DataRow dr;
			foreach (DataRowView drv in dv)
			{
				dr = dtClone.NewRow();
				dr.ItemArray = drv.Row.ItemArray;
				dtClone.Rows.Add(dr);
				index++;
				if (count > 0 && index >= count)
					break;
			}
			return dtClone;
		}
		#endregion

		#region GetListIncidentStatisticByProjectDataTable
		/// <summary>
		/// Reader returns fields:
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticByProjectDataTable()
		{
			return GetListIncidentStatisticByProjectDataTable(0);
		}

		public static DataTable GetListIncidentStatisticByProjectDataTable(int ManagerId)
		{
			return DBIncident.GetListIncidentStatisticByProjectDataTable(ManagerId);
		}
		#endregion

		#region GetListIncidentStatisticByStatusDataTable
		/// <summary>
		/// Reader returns fields:
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticByStatusDataTable()
		{
			return DBIncident.GetListIncidentStatisticByStatusDataTable(Security.CurrentUser.LanguageId);
		}
		#endregion
		#region GetListIncidentStatisticByIncidentBoxDataTable
		/// <summary>
		/// Reader returns fields:
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticByIncidentBoxDataTable()
		{
			return DBIncident.GetListIncidentStatisticByIncidentBoxDataTable(Security.CurrentUser.LanguageId);
		}
		#endregion
		#region GetListIncidentStatisticByPriorityDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticByPriorityDataTable()
		{
			return DBIncident.GetListIncidentStatisticByPriorityDataTable(Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListIncidentStatisticBySeverityDataTable
		/// <summary>
		///  ItemId, ItemName, Count
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentStatisticBySeverityDataTable()
		{
			return DBIncident.GetListIncidentStatisticBySeverityDataTable();
		}
		#endregion

		#region GetListCategoriesByUser
		/// <summary>
		/// Reader returns fields:
		///		CategoryId, UserId
		/// </summary>
		public static IDataReader GetListCategoriesByUser()
		{
			return DBCommon.GetListCategoriesByUser(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListIncidentCategoriesByUser
		/// <summary>
		/// Reader returns fields:
		///		CategoryId, UserId
		/// </summary>
		public static IDataReader GetListIncidentCategoriesByUser()
		{
			return DBIncident.GetListIncidentCategoriesByUser(Security.CurrentUser.UserID);
		}
		#endregion

		#region SaveGeneralCategories
		public static void SaveGeneralCategories(ArrayList general_categories)
		{
			Project.SaveGeneralCategories(general_categories);
		}
		#endregion

		#region SaveIncidentCategories
		public static void SaveIncidentCategories(ArrayList incident_categories)
		{
			if (incident_categories == null)
				incident_categories = new ArrayList();

			int UserId = Security.CurrentUser.UserID;

			// Incident Categories
			ArrayList NewIncidentCategories = new ArrayList(incident_categories);
			ArrayList DeletedIncidentCategories = new ArrayList();
			
			using(IDataReader reader = DBIncident.GetListIncidentCategoriesByUser(UserId))
			{
				while(reader.Read())
				{
					int CategoryId = (int)reader["CategoryId"];
					if (NewIncidentCategories.Contains(CategoryId))
						NewIncidentCategories.Remove(CategoryId);
					else
						DeletedIncidentCategories.Add(CategoryId);
				}
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				foreach(int CategoryId in DeletedIncidentCategories)
					DBIncident.DeleteIncidentCategoryUser(CategoryId, UserId);
				foreach(int CategoryId in NewIncidentCategories)
					DBIncident.AddIncidentCategoryUser(CategoryId, UserId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateDiscussion
		public static void UpdateDiscussion(
			int DiscussionId, string Text)
		{
			Project.UpdateDiscussion(DiscussionId, Text);
		}
		#endregion

		#region GetListOpenIncidentsByProject
		/// <summary>
		/// IncidentId, Title, TypeId, TypeName, StateId, ManagerId, ManagerName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListOpenIncidentsByProject(int ProjectId)
		{
			return DBIncident.GetListOpenIncidentsByProject(ProjectId);
		}
		#endregion

		#region GetListOpenIncidentsByProjectDataTable
		/// <summary>
		/// IncidentId, Title, TypeId, TypeName, StateId, ManagerId, ManagerName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListOpenIncidentsByProjectDataTable(int ProjectId)
		{
			return DBIncident.GetListOpenIncidentsByProjectDataTable(ProjectId);
		}
		#endregion

		#region GetListHighPriorityIncidents
		/// <summary>
		/// IncidentId, Title, ProjectId, ProjectTitle, 
		/// PriorityId, PriorityName, CreationDate, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListHighPriorityIncidents()
		{
			return DBIncident.GetListHighPriorityIncidents(Security.CurrentUser.LanguageId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListHighPriorityIncidentsDataTable
		/// <summary>
		/// IncidentId, Title, ProjectId, ProjectTitle, 
		/// PriorityId, PriorityName, CreationDate, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListHighPriorityIncidentsDataTable()
		{
			return DBIncident.GetListHighPriorityIncidentsDataTable(Security.CurrentUser.LanguageId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListNewIncidents
		/// <summary>
		/// IncidentId, Title, ProjectId, ProjectName, ManagerId, PriorityId, PriorityName, 
		/// TypeId, TypeName, ManagerName, CreationDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListNewIncidents()
		{
			return GetListNewIncidents(0);
		}
		public static DataTable GetListNewIncidents(int ProjectId)
		{
			return DBIncident.GetListIncidentsByState(ProjectId, (int)ObjectStates.Upcoming, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListActiveIncidents
		/// <summary>
		/// IncidentId, Title, ProjectId, ProjectName, ManagerId, PriorityId, PriorityName, 
		/// TypeId, TypeName, ManagerName, CreationDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListActiveIncidents()
		{
			return GetListActiveIncidents(0);
		}

		public static DataTable GetListActiveIncidents(int ProjectId)
		{
			return DBIncident.GetListIncidentsByState(ProjectId, (int)ObjectStates.Active, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListIncidentsByCategory
		/// <summary>
		/// IncidentId, Title, ProjectId, ProjectName, PriorityId, PriorityName, 
		/// StateId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsByCategory(int CategoryId)
		{
			return DBIncident.GetListIncidentsByCategory(CategoryId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListIncidentsByType
		/// <summary>
		/// IncidentId, Title, ProjectId, ProjectName, ManagerId, PriorityId, PriorityName, 
		/// ManagerName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentsByType(int TypeId)
		{
			return DBIncident.GetListIncidentsByType(TypeId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListClosedIncidents
		/// <summary>
		/// IncidentId, Title, ProjectId, ProjectName, ManagerId, PriorityId, PriorityName, 
		/// TypeId, TypeName, ManagerName, CreationDate
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListClosedIncidents()
		{
			return GetListClosedIncidents(0);
		}

		public static DataTable GetListClosedIncidents(int ProjectId)
		{
			return DBIncident.GetListIncidentsByState(ProjectId, (int)ObjectStates.Completed, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region ShowAcceptDeny
		public static bool ShowAcceptDeny(int incident_id)
		{
			bool retval = false;

			IncidentSecurity es = GetSecurity(incident_id);
			retval = es.IsResource;

			if (retval)
			{
				bool Resource_MustBeConfirmed = false;
				bool Resource_IsResponsePending = false;

				using(IDataReader reader = DBIncident.GetResourceByUser(incident_id, Security.CurrentUser.UserID))
				{
					if (reader.Read())
					{
						Resource_MustBeConfirmed = (bool)reader["MustBeConfirmed"];
						Resource_IsResponsePending = (bool)reader["ResponsePending"];
					}
				}
				retval = Resource_MustBeConfirmed && Resource_IsResponsePending;
			}
			return retval;
		}
		#endregion

		#region GetListIncidentResources
		/// <summary>
		/// Reader returns fields:
		///  ResourceId, IncidentId, PrincipalId, IsGroup, MustBeConfirmed, ResponsePending, 
		///  IsConfirmed, IsExternal, CanManage
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentResources(int IncidentId)
		{
			return DBIncident.GetListResources(IncidentId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListIncidentResourcesDataTable
		/// <summary>
		/// ResourceId, IncidentId, PrincipalId, IsGroup, MustBeConfirmed, ResponsePending, 
		///  IsConfirmed, IsExternal, CanManage
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentResourcesDataTable(int IncidentId)
		{
			return DBIncident.GetListIncidentResourcesDataTable(IncidentId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListResourcesWithResponsible
    /// <summary>
    /// Reader returns fields:
    ///  IncidentId, PrincipalId, IsGroup, IsResponsible, FirstName, LastName
    /// </summary>
    /// <returns></returns>
    public static IDataReader GetListResourcesWithResponsible(int IncidentId)
    {
      return DBIncident.GetListResourcesWithResponsible(IncidentId);
    }
    #endregion

		#region GetListPendingIncidents
		/// <summary>
		///		IncidentId, Title, Description, PriorityId, PriorityName, ManagerId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListPendingIncidents(int ProjectID)
		{
			return GetListPendingIncidents(ProjectID, Security.CurrentUser.UserID);
		}

		public static IDataReader GetListPendingIncidents(int ProjectID, int UserId)
		{
			return DBIncident.GetListPendingIncidents(ProjectID, UserId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}

		public static DataTable GetListPendingIncidentsDataTable(int ProjectID)
		{
			return GetListPendingIncidentsDataTable(ProjectID, Security.CurrentUser.UserID);
		}

		public static DataTable GetListPendingIncidentsDataTable(int ProjectID, int UserId)
		{
			return DBIncident.GetListPendingIncidentsDataTable(ProjectID, UserId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListIncidentRelationsDataTable
		/// <summary>
		///	 IncidentId, Title, ManagerId, StateId, CanView
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentRelationsDataTable(int IncidentId)
		{
			return DBIncident.GetListIncidentRelationsDataTable(IncidentId, Security.CurrentUser.UserID);
		}
		#endregion

		#region Collapse
		public static void Collapse(int ProjectId)
		{
			DBIncident.Collapse(Security.CurrentUser.UserID, ProjectId);
		}
		#endregion

		#region Expand
		public static void Expand(int ProjectId)
		{
			DBIncident.Expand(Security.CurrentUser.UserID, ProjectId);
		}
		#endregion
		
		#region CollapseByClient
		public static void CollapseByClient(PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DBIncident.CollapseByClient(Security.CurrentUser.UserID, contactUid, orgUid);
		}
		#endregion

		#region ExpandByClient
		public static void ExpandByClient(PrimaryKeyId contactUid, PrimaryKeyId orgUid)
		{
			DBIncident.ExpandByClient(Security.CurrentUser.UserID, contactUid, orgUid);
		}
		#endregion

		#region GetListOpenIncidentsByUserOnlyDataTable
		/// <summary>
		/// IncidentId, Title, CreationDate, PriorityId, PriorityName, IsManager, IsCreator, 
		/// IsResource, StateId, Identifier, IsOverdue, IsNewMessage
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListOpenIncidentsByUserOnlyDataTable()
		{
			return DBIncident.GetListOpenIncidentsByUserOnlyDataTable(Security.CurrentUser.UserID, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}

		public static DataTable GetListOpenIncidentsByUserOnlyDataTable(int UserId)
		{
			return DBIncident.GetListOpenIncidentsByUserOnlyDataTable(UserId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetIncidentTitle
		public static string GetIncidentTitle(int IncidentId)
		{
			string incidentTitle = "";
			using (IDataReader reader = GetIncident(IncidentId, false))
			{
				if (reader.Read())
					incidentTitle = reader["Title"].ToString();
			}
			return incidentTitle;
		}
		#endregion

		#region AddFavorites
		public static void AddFavorites(int IncidentId)
		{
			DBCommon.AddFavorites((int)INCIDENT_TYPE, IncidentId, Security.CurrentUser.UserID);
		}
		#endregion

		#region GetListFavorites
		/// <summary>
		///		FavoriteId, ObjectTypeId, ObjectId, UserId, Title 
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListFavoritesDT()
		{
			return DBCommon.GetListFavoritesDT((int)INCIDENT_TYPE, Security.CurrentUser.UserID);
		}
		#endregion

		#region CheckFavorites
		public static bool CheckFavorites(int IncidentId)
		{
			return DBCommon.CheckFavorites((int)INCIDENT_TYPE, IncidentId, Security.CurrentUser.UserID);
		}
		#endregion

		#region DeleteFavorites
		public static void DeleteFavorites(int IncidentId)
		{
			DBCommon.DeleteFavorites((int)INCIDENT_TYPE, IncidentId, Security.CurrentUser.UserID);
		}
		#endregion

		#region AddHistory
		public static void AddHistory(int IncidentId, string Title)
		{
			DBCommon.AddHistory((int)INCIDENT_TYPE, IncidentId, Title, Security.CurrentUser.UserID);
		}
		#endregion

		#region IsEmailIncident
		public static bool IsEmailIncident(int IncidentId)
		{
			bool isEmail = false;
			using (IDataReader reader = Incident.GetIncident(IncidentId, false))
			{
				if (reader.Read())
					isEmail = (bool)reader["IsEmail"];
			}
			return isEmail;
		}
		#endregion

		#region GetProject
		public static int GetProject(int IncidentId)
		{
			return DBIncident.GetProject(IncidentId);
		}
		#endregion

		#region GetForumThreadNodes
		public static ForumThreadNodeInfo[] GetForumThreadNodes(int IncidentId)
		{
			BaseIbnContainer destContainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", IncidentId));
			ForumStorage forumStorage = (ForumStorage)destContainer.LoadControl("ForumStorage");

			return forumStorage.GetForumThreadNodes();
		}
		#endregion

		#region GetForumNodeFiles
		public static Mediachase.IBN.Business.ControlSystem.FileInfo[] GetForumNodeFiles(int NodeId)
		{
			BaseIbnContainer nodeFSContainer = BaseIbnContainer.Create("FileLibrary",string.Format("ForumNodeId_{0}", NodeId));
			FileStorage nodeFileStorage = (FileStorage)nodeFSContainer.LoadControl("FileStorage");

			return nodeFileStorage.GetFiles();
		}
		#endregion

		#region CreateFromEmail
		public static int CreateFromEmail(
			string title,
			string description,
			int project_id,
			int type_id,
			int priority,
			int severity_id,
			int task_time,
			int expected_duration,
			int expected_response_time,
			int expected_assign_time,
			ArrayList categories,
			ArrayList incident_categories,
			int IncidentBoxId,
			int EMailMessageId,
			PrimaryKeyId orgUid,
			PrimaryKeyId contactUid
			)
		{
			return CreateFromEmail(title, description, project_id, type_id, priority, severity_id,
				task_time, expected_duration, expected_response_time, expected_assign_time, categories,
				incident_categories, IncidentBoxId, EMailMessageId, orgUid, contactUid, -1);
		}

		public static int CreateFromEmail(
			string title,
			string description,
			int project_id,
			int type_id,
			int priority,
			int severity_id,
			int task_time,
			int expected_duration,
			int expected_response_time,
			int expected_assign_time,
			ArrayList categories,
			ArrayList incident_categories,
			int IncidentBoxId,
			int EMailMessageId,
			PrimaryKeyId orgUid,
			PrimaryKeyId contactUid,
			int mustResponsibleId
			)
		{
			return CreateFromEmail(title, description, project_id, type_id, priority, severity_id,
				task_time, expected_duration, expected_response_time, expected_assign_time,
				categories, incident_categories, IncidentBoxId, EMailMessageId, orgUid, contactUid,
				mustResponsibleId, true, DateTime.UtcNow);
		}

		public static int CreateFromEmail(
			string title,
			string description,
			int project_id,
			int type_id,
			int priority,
			int severity_id,
			int task_time,
			int expected_duration, 
			int expected_response_time,
			int expected_assign_time,
			ArrayList categories,
			ArrayList incident_categories,
			int IncidentBoxId,
			int EMailMessageId,
			PrimaryKeyId orgUid,
			PrimaryKeyId contactUid,
			int mustResponsibleId,
			bool useDuration,
			DateTime expResolveDate
			)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();


			// OZ 2009-03-11 Fixed AccessDeniedException if user is inactive
			//if (!CanCreate(project_id))
			//    throw new AccessDeniedException();

			title = title.Replace("<", "&lt;").Replace(">", "&gt;");

			DateTime now = DateTime.UtcNow;

			object oProjectId = null;
			if (project_id > 0)
				oProjectId = project_id;

			EMailMessageInfo mi = EMailMessageInfo.Load(EMailMessageId);

			//			int SenderUserID = -1; // TODO: Find User By Email
			//			int UserId = Security.CurrentUser.UserID;
			//			if (SenderUserID > 0)
			//				UserId = SenderUserID;

			if (orgUid == PrimaryKeyId.Empty && contactUid == PrimaryKeyId.Empty)
			{
				Client client = Common.GetClient(mi.SenderEmail);
				if (client != null)
				{
					if (client.IsContact)
					{
						contactUid = client.Id;
					}
					else
					{
						orgUid = client.Id;
					}
				}
			}

			int stateId, managerId, responsibleId;
			bool isResposibleGroup;
			ArrayList users = new ArrayList();

			Issue2.GetIssueBoxSettings(IncidentBoxId, out stateId, out managerId, out responsibleId, out isResposibleGroup, users);

			// O.R. [2008-09-09]: Exclude inactive users
			if (responsibleId > 0 && User.GetUserActivity(responsibleId) != User.UserActivity.Active)
				responsibleId = -1;

			ArrayList activeUsers = new ArrayList();
			foreach (int userId in users)
			{
				if (User.GetUserActivity(userId) == User.UserActivity.Active)
					activeUsers.Add(userId);
			}
			//

			//ak [2009-06-10] responsible from creation page (IncidentEdit1)
			if (mustResponsibleId > 0)
			{
				responsibleId = mustResponsibleId;

				// O.R. [2009-12-17] upcoming state with responsible fix
				if (stateId == (int)ObjectStates.Upcoming)
					stateId = (int)ObjectStates.Active;

				isResposibleGroup = false;
			}

			IncidentBox box = IncidentBox.Load(IncidentBoxId);
			if (useDuration)
				expResolveDate = DBCalendar.GetFinishDateByDuration(box.Document.GeneralBlock.CalendarId, now, expected_duration);
			else
			{
				expResolveDate = User.GetUTCDate(expResolveDate);
				expected_duration = DBCalendar.GetDurationByFinishDate(box.Document.GeneralBlock.CalendarId, now, expResolveDate);
			}

			int IncidentId;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				// Remove from Pending
				PendingEMailMessageRow.DeleteByEMailMessageId(EMailMessageId);

				// Add to white list
				if (PortalConfig.UseAntiSpamFilter && PortalConfig.AutoFillWhiteList)
					WhiteListItem.Create(mi.SenderEmail);

				IncidentId = DBIncident.Create(oProjectId, Security.UserID, 
					title, description, now, 
					type_id, priority, stateId, severity_id, true, task_time,
					IncidentBoxId, responsibleId, isResposibleGroup ? 1 : 0, 
					contactUid, orgUid, 
					expected_duration, expected_response_time, expected_assign_time,
					expResolveDate, useDuration);

				string Identifier = TicketUidUtil.Create(box.IdentifierMask, IncidentId);
				DBIncident.UpdateIdentifier(IncidentId, Identifier);

				// Categories
				foreach(int CategoryId in categories)
				{
					DBCommon.AssignCategoryToObject((int)INCIDENT_TYPE, IncidentId, CategoryId);
				}

				// Incident Categories
				foreach(int CategoryId in incident_categories)
				{
					DBIncident.AssignIncidentCategory(IncidentId, CategoryId);
				}

				foreach(int UserId in activeUsers)
				{
					DbIssue2.ResponsibleGroupAddUser(IncidentId, UserId);
				}

				// Update Incident Identifier
				//string Ticket = TicketUidUtil.Create(box.IdentifierMask,IncidentId);
				//DBIncident.UpdateIdentifier(IncidentId, Ticket);

				// Forum
				// Upload Forum Message
				BaseIbnContainer FOcontainer = BaseIbnContainer.Create("FileLibrary",string.Format("IncidentId_{0}",IncidentId));
				ForumStorage forumStorage = (ForumStorage)FOcontainer.LoadControl("ForumStorage");

				ForumThreadNodeInfo info = forumStorage.CreateForumThreadNode(EMailMessageInfo.ExtractTextFromHtml(mi.HtmlBody), 
					DateTime.UtcNow,
					Security.UserID, 
					EMailMessageId,
					(int)ForumStorage.NodeContentType.EMail);

				ForumThreadNodeSettingCollection settings = new ForumThreadNodeSettingCollection(info.Id);
				settings.Add(ForumThreadNodeSetting.Incoming, "1");
				settings.Add(ForumThreadNodeSetting.Question, "1");

				// O.R: Recalculating project TaskTime
				if (project_id > 0 && task_time > 0)
					TimeTracking.RecalculateProjectTaskTime(project_id);

				// 2006-12-12 OZ: Register EMail External Recepient
				EMailIssueExternalRecipient.Create(IncidentId, mi.SenderEmail);

				string emailFrom = EMail.EMailRouterOutputMessage.FindEMailRouterPublicEmail(IncidentId);

				ArrayList excludeUsers = EMailRouterOutputMessage.Send(IncidentId, EMailMessageId);

				//SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Created, IncidentId, -1, sendAutoReply ? mi.SenderEmail : null, emailFrom, excludeUsers);
				// 2007-02-12 OZ: Auto reply per Issue Box
				EMailRouterOutputMessage.SendAutoReply(IncidentId, emailFrom, mi.SenderEmail);

				// O.R.[2008-12-16]: Recalculate Current Responsible
				DBIncident.RecalculateCurrentResponsible(IncidentId);

				// O.R. [2009-06-15]: Recalculate project dates
				if (project_id > 0 && PortalConfig.UseIncidentDatesForProject && !useDuration)
					Project.RecalculateDates(project_id);

				if(project_id > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Project_Updated_IssueList_IssueAdded, project_id, IncidentId, excludeUsers);
				Issue2.SendAlertsForNewIssue(IncidentId, managerId, responsibleId, activeUsers, excludeUsers);

				// 2007-02-15: OZ: New Message Addon
				Issue2.SetNewMessage(IncidentId, true);
				//

				// O.R. [2008-09-22]: Gate for sender
				Issue2.UpdateMailSenderEmail(IncidentId, mi.SenderEmail);

				tran.Commit();
			}

			return IncidentId;
		}
		#endregion

		#region HasControl
		public static bool HasControl(int IncidentId)
		{
			int IncidentBoxId = GetIncidentBox(IncidentId);
			return IncidentBoxDocument.Load(IncidentBoxId).GeneralBlock.AllowControl;
		}
		#endregion

		#region GetState
		public static int GetState(int IncidentId)
		{
			return DBIncident.GetState(IncidentId);
		}
		#endregion

		#region GetIncidentBox
		public static int GetIncidentBox(int IncidentId)
		{
			int IncidentBoxId = -1;

			// FIX: EMPTY CURRENT USER PROBLEM
			using (IDataReader reader = DBIncident.GetIncident(IncidentId, 
					   0, 
					   1))
			{
				if (reader.Read())
					IncidentBoxId = (int)reader["IncidentBoxId"];
			}
			return IncidentBoxId;
		}
		#endregion

		
		#region GetResponsiblePoolFromBox
		public static ArrayList GetResponsiblePoolFromBox(int IncidentId)
		{
			ArrayList ResponsiblePool = new ArrayList();
			int BoxId = GetIncidentBox(IncidentId);
			
			IncidentBoxDocument IncBoxDoc = IncidentBoxDocument.Load(BoxId);

			return IncBoxDoc.GeneralBlock.ResponsiblePool;
		}
		#endregion

		#region GetResponsibleList
		public static ArrayList GetResponsibleList(int IncidentId)
		{
			//TODO: sorting

			ArrayList outList = GetResponsiblePoolFromBox(IncidentId);
			
			//Объединение с текущим пользователем
			int UserId = Security.UserID;
			if(outList.Contains(UserId))
				outList.Remove(UserId);
			outList.Insert(0,UserId);
			
			
			//Объединение с текущей группой
			using(IDataReader reader = GetResponsibleGroup(IncidentId))
			{
				while(reader.Read())
				{
					UserId = (int)reader["PrincipalId"];
					if(!outList.Contains(UserId))
						outList.Add(UserId);
				}
			}

			return outList;
		}
		#endregion

		#region GetResponsibleGroup
		/// <summary>
		/// Reader returns fields:
		///  ResourceId, IncidentId, PrincipalId, IsGroup, MustBeConfirmed, ResponsePending, 
		///  IsConfirmed, IsExternal
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetResponsibleGroup(int IncidentId)
		{
			return DBIncident.GetResponsibleGroup(IncidentId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetResponsibleGroupDataTable
		/// <summary>
		/// ResourceId, IncidentId, PrincipalId, IsGroup, MustBeConfirmed, ResponsePending, 
		///  IsConfirmed, IsExternal
		/// </summary>
		/// <returns></returns>
		public static DataTable GetResponsibleGroupDataTable(int IncidentId)
		{
			return DBIncident.GetResponsibleGroupDataTable(IncidentId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetInternalEmailRecipients
		/// <summary>
		/// ArrayList of email strings
		/// </summary>
		/// <returns></returns>
		public static ArrayList GetInternalEmailRecipients(int IncidentId)
		{
			ObjectStates IncidentState = ObjectStates.Upcoming;
			int ControllerId = -1;
			ArrayList users = new ArrayList();

			bool IsResponsibleGroup = false;
			int ResponsibleId = 0;
			
			using(IDataReader reader = GetIncident(IncidentId, false))
			{
				if(reader.Read())
				{
					IncidentState = (ObjectStates)reader["StateId"];
					ControllerId = DBCommon.NullToInt32(reader["ControllerId"]);
					IsResponsibleGroup = (bool)reader["IsResponsibleGroup"];
					ResponsibleId = (int)reader["ResponsibleId"];
				}
			}

			//get from box
			IncidentBoxDocument incDoc = IncidentBoxDocument.Load(GetIncidentBox(IncidentId));
			
			foreach(int id in incDoc.EMailRouterBlock.InformationRecipientList)
				users.Add(id);
			

			//get responsible from incident
			if(IncidentState == ObjectStates.Active || 
				IncidentState == ObjectStates.ReOpen || 
				IncidentState == ObjectStates.Upcoming)
			{
				if(IsResponsibleGroup)
				{
					using(IDataReader reader = GetResponsibleGroup(IncidentId))
					{
						while(reader.Read())
						{
							//object dd = reader["ResponsePending"];
							bool IsPending = (bool)reader["ResponsePending"];
							int UserId  = (int)reader["PrincipalId"];

							if(IsPending)
							{
								if(!users.Contains(UserId))
									users.Add(UserId);
							}
						}
					}
				}
				else
				{
					if(ResponsibleId>0)
						users.Add(ResponsibleId);
				}
			}

			//add manager
			if(!users.Contains(incDoc.GeneralBlock.Manager))
				users.Add(incDoc.GeneralBlock.Manager);

			//add controller
			if(IncidentState == ObjectStates.OnCheck)
			{
				if(!users.Contains(ControllerId))
					users.Add(ControllerId);
			}
			
			return users;
		}
		#endregion

		#region GetListIncidentsByIncidentBox
		public static IDataReader GetListIncidentsByIncidentBox(int IncidentBoxId)
		{
			return DBIncident.GetListIncidentsByIncidentBox(IncidentBoxId);
		}
		#endregion

		#region RaiseTurnOffControling
		//controlling turn off for box
		public static void RaiseTurnOffControling(int IncidentBoxId)
		{
			DbIssue2.UpdateOnCheckByBox(IncidentBoxId);
		}

		//change incident box for issue
		public static void RaiseTurnOffControling(int IncidentId, int NewIncidentBoxId)
		{
			DbIssue2.UpdateOnCheck(IncidentId);
		}

		#endregion

		#region RaiseChangeController
		public static void RaiseChangeController(int IncidentBoxId, int OldControlerId, int NewControlerId)
		{
			
		}

		public static void RaiseChangeController(int IncidentId, int NewIncidentBoxId, int OldControlerId, int NewControlerId)
		{
			
		}

		#endregion

		#region RaiseChangeManager
		public static void RaiseChangeManager(int IncidentBoxId, int OldControlerId, int NewControlerId)
		{
			// TODO: Not implemented yet
		}

		public static void RaiseChangeManager(int IncidentId, int NewIncidentBoxId, int OldControlerId, int NewControlerId)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(OldControlerId > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Controller_Resigned, IncidentId, OldControlerId);
				if(NewControlerId > 0)
					SystemEvents.AddSystemEvents(SystemEventTypes.Issue_Updated_Controller_Assigned, IncidentId, OldControlerId);

				tran.Commit();
			}
		}
		#endregion

		#region GetListIncidentStates
		/// <summary>
		/// Reader returns fields:
		///		StateId, StateName 
		/// </summary>
		public static IDataReader GetListIncidentStates()
		{
			return DBIncident.GetListIncidentStates(0, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetListIncidentStatesDataTable
		/// <summary>
		/// Reader returns fields:
		///		StateId, StateName 
		/// </summary>
		public static DataTable GetListIncidentStatesDataTable()
		{
			return DBIncident.GetListIncidentStatesDataTable(0, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetIncidentState
		/// <summary>
		/// Reader returns fields:
		///		StateId, StateName 
		/// </summary>
		public static IDataReader GetIncidentState(int StateId)
		{
			return DBIncident.GetListIncidentStates(StateId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetIncidentStateName
		/// <summary>
		/// Reader returns fields:
		///		StateName 
		/// </summary>
		public static string GetIncidentStateName(int StateId)
		{
			string retVal = String.Empty;
			using (IDataReader reader = DBIncident.GetListIncidentStates(StateId, Security.CurrentUser.LanguageId))
			{
				if(reader.Read())
					retVal = reader["StateName"].ToString();
			}
			return retVal;
		}
		#endregion

		#region GetIdentifier
        public static string GetIdentifier(int IncidentId)
        {
            string retVal = string.Empty;

            using (IDataReader reader = DBIncident.GetIncident(IncidentId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
            {
                if (reader.Read())
                    retVal = (reader["Identifier"] == DBNull.Value) ? string.Empty : (string)reader["Identifier"];
            }

            if (retVal == string.Empty)
            {
                IncidentBox box = IncidentBox.Load(Incident.GetIncidentBox(IncidentId));
                retVal = TicketUidUtil.Create(box.IdentifierMask, IncidentId);
            }

            return retVal;
        }
        #endregion

		#region GetListIncidentResponsibles
		/// <summary>
		///  UserId, UserName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListIncidentResponsibles()
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
				return DBIncident.GetListIncidentResponsibles();
			else
				return DBIncident.GetListIncidentResponsiblesForPartner(Security.CurrentUser.UserID);
		}

		/// <summary>
		///  UserId, UserName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListIncidentResponsiblesDT()
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
				return DBIncident.GetListIncidentResponsiblesDT();
			else
				return DBIncident.GetListIncidentResponsiblesForPartnerDT(Security.CurrentUser.UserID);
		}
		#endregion

		#region GetContactId
		public static void GetContactId(string str, out PrimaryKeyId orgUid, out PrimaryKeyId contactUid)
		{
			orgUid = PrimaryKeyId.Empty;
			contactUid = PrimaryKeyId.Empty;
			if (str.IndexOf("_") >= 0)
			{
				string stype = str.Substring(0, str.IndexOf("_"));
				if (stype.ToLower() == "org")
					orgUid = PrimaryKeyId.Parse(str.Substring(str.IndexOf("_") + 1));
				else if (stype.ToLower() == "contact")
					contactUid = PrimaryKeyId.Parse(str.Substring(str.IndexOf("_") + 1));
			}
		}
		#endregion
	}
}
