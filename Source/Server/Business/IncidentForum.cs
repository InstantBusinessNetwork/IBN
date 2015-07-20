using System;
using System.Collections;
using System.Data;
using Mediachase.IBN.Database;
using Mediachase.IBN.Business.ControlSystem; 
using Mediachase.IBN.Business.EMail;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for IncidentForum.
	/// </summary>
	public class IncidentForum
	{
		public enum IssueEvent
		{
			State = 0,
			Responsibility = 1,
			Declining = 2,
//			Card = 3
		}

//		public enum CardElements
//		{
//			ProjectId = 0,
//			TypeId = 1,
//			SeverityId = 2,
//			PriorityId = 3,
//			GeneralCategories = 4,
//			IssueCategories = 5
//		}

		#region AddForumMesageWithStateChange
		public static int AddForumMesageWithStateChange(int IncidentId, int ForumNodeId, int newStateId)
		{
			ForumThreadNodeInfo info = null;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				BaseIbnContainer destContainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", IncidentId));
				ForumStorage forumStorage = (ForumStorage)destContainer.LoadControl("ForumStorage");
				if(ForumNodeId>0)
					info = forumStorage.GetForumThreadNode(ForumNodeId);
				else
					info = forumStorage.CreateForumThreadNode("", Security.CurrentUser.UserID, (int)ForumStorage.NodeContentType.Text);
				
				ForumThreadNodeSettingCollection settings = new ForumThreadNodeSettingCollection(info.Id);
//				settings.Add("IssueEvent", IssueEvent.State.ToString());
				settings.Add(IssueEvent.State.ToString(), newStateId.ToString());

				tran.Commit();
			}
			return info.Id;
		}
		#endregion

		#region AddForumMesageWithResponsibleChange
		//newRespId: UserId OR "-2" - NotSet, "-1" - GroupResponsibility
		public static int AddForumMesageWithResponsibleChange(int IncidentId, int ForumNodeId, int newRespId)
		{
			ForumThreadNodeInfo info = null;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				BaseIbnContainer destContainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", IncidentId));
				ForumStorage forumStorage = (ForumStorage)destContainer.LoadControl("ForumStorage");
				if(ForumNodeId>0)
					info = forumStorage.GetForumThreadNode(ForumNodeId);
				else
					info = forumStorage.CreateForumThreadNode("", Security.CurrentUser.UserID, (int)ForumStorage.NodeContentType.Text);

				ForumThreadNodeSettingCollection settings = new ForumThreadNodeSettingCollection(info.Id);
//				settings.Add("IssueEvent", IssueEvent.Responsibility.ToString());
				settings.Add(IssueEvent.Responsibility.ToString(), newRespId.ToString());

				tran.Commit();
			}
			return info.Id;
		}
		#endregion

		#region AddForumMesageWithDeclining
		public static int AddForumMesageWithDeclining(int IncidentId, int ForumNodeId)
		{
			ForumThreadNodeInfo info = null;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				BaseIbnContainer destContainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", IncidentId));
				ForumStorage forumStorage = (ForumStorage)destContainer.LoadControl("ForumStorage");
				if(ForumNodeId>0)
					info = forumStorage.GetForumThreadNode(ForumNodeId);
				else
					info = forumStorage.CreateForumThreadNode("", Security.CurrentUser.UserID, (int)ForumStorage.NodeContentType.Text);

				ForumThreadNodeSettingCollection settings = new ForumThreadNodeSettingCollection(info.Id);
				settings.Add(IssueEvent.Declining.ToString(), Security.CurrentUser.UserID.ToString());

				tran.Commit();
			}
			return info.Id;
		}
		#endregion

		#region //AddForumMesageWithStateNResponsibleChange
//		public static void AddForumMesageWithStateNResponsibleChange(int IncidentId, ForumThreadNodeInfo node, int newStateId, int newRespId)
//		{
//			using(DbTransaction tran = DbTransaction.Begin())
//			{
//				ForumThreadNodeInfo info = null;
//				if(node==null)
//				{
//					BaseIbnContainer destContainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", IncidentId));
//					ForumStorage forumStorage = (ForumStorage)destContainer.LoadControl("ForumStorage");
//
//					info = forumStorage.CreateForumThreadNode("", Security.CurrentUser.UserID, (int)ForumStorage.NodeContentType.Text);
//				}
//				else
//					info = node;
//
//				ForumThreadNodeSettingCollection settings = new ForumThreadNodeSettingCollection(info.Id);
//				settings.Add("IssueEvent", IssueEvent.Responsibility.ToString());
//				settings.Add(IssueEvent.State.ToString(), newStateId.ToString());
//				settings.Add(IssueEvent.Responsibility.ToString(), newRespId.ToString());
//
//				tran.Commit();
//			}
//		}
		#endregion

		#region //AddForumMesageWithCardChange
//		public static void AddForumMesageWithCardChange(int IncidentId, ForumThreadNodeInfo node, 
//			int newProjectId, int newTypeId, int newSeverityId, int newPriorityId, 
//			ArrayList newGenCats, ArrayList newIssCats)
//		{
//			using(DbTransaction tran = DbTransaction.Begin())
//			{
//				ForumThreadNodeInfo info = null;
//				if(node==null)
//				{
//					BaseIbnContainer destContainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", IncidentId));
//					ForumStorage forumStorage = (ForumStorage)destContainer.LoadControl("ForumStorage");
//
//					info = forumStorage.CreateForumThreadNode("", Security.CurrentUser.UserID, (int)ForumStorage.NodeContentType.Text);
//				}
//				else
//					info = node;
//
//				ForumThreadNodeSettingCollection settings = new ForumThreadNodeSettingCollection(info.Id);
//				settings.Add("IssueEvent", IssueEvent.Card.ToString());
//				if(newProjectId>0)
//					settings.Add(CardElements.ProjectId.ToString(), newProjectId.ToString());
//				if(newTypeId>0)
//					settings.Add(CardElements.TypeId.ToString(), newTypeId.ToString());
//				if(newSeverityId>0)
//					settings.Add(CardElements.SeverityId.ToString(), newSeverityId.ToString());
//				if(newPriorityId>0)
//					settings.Add(CardElements.PriorityId.ToString(), newPriorityId.ToString());
//				if(newGenCats!=null)
//				{
//					string sNewGenCats = "";
//					foreach(int iCatId in newGenCats)
//						sNewGenCats += iCatId.ToString()+"_";
//					if(sNewGenCats.Length>0)
//						sNewGenCats = sNewGenCats.Substring(0, sNewGenCats.Length-1);
//					settings.Add(CardElements.GeneralCategories.ToString(), sNewGenCats);
//				}
//				if(newIssCats!=null)
//				{
//					string sNewIssCats = "";
//					foreach(int iCatId in newIssCats)
//						sNewIssCats += iCatId.ToString()+"_";
//					if(sNewIssCats.Length>0)
//						sNewIssCats = sNewIssCats.Substring(0, sNewIssCats.Length-1);
//					settings.Add(CardElements.IssueCategories.ToString(), sNewIssCats);
//				}
//
//				tran.Commit();
//			}
//		}
		#endregion

		#region //******Last Methods - private variants
//		#region AddForumMesageWithProjectChange
//		public static void AddForumMesageWithProjectChange(int IncidentId, ForumThreadNodeInfo node, int newProjectId)
//		{
//			AddForumMesageWithCardChange(IncidentId, node, newProjectId, -1, -1, -1, null, null);
//		}
//		#endregion
//
//		#region AddForumMesageWithTypeChange
//		public static void AddForumMesageWithTypeChange(int IncidentId, ForumThreadNodeInfo node, int newTypeId)
//		{
//			AddForumMesageWithCardChange(IncidentId, node, -1, newTypeId, -1, -1, null, null);
//		}
//		#endregion
//
//		#region AddForumMesageWithSeverityChange
//		public static void AddForumMesageWithSeverityChange(int IncidentId, ForumThreadNodeInfo node, int newSeverityId)
//		{
//			AddForumMesageWithCardChange(IncidentId, node, -1, -1, newSeverityId, -1, null, null);
//		}
//		#endregion
//
//		#region AddForumMesageWithPriorityChange
//		public static void AddForumMesageWithPriorityChange(int IncidentId, ForumThreadNodeInfo node, int newPriorityId)
//		{
//			AddForumMesageWithCardChange(IncidentId, node, -1, -1, -1, newPriorityId, null, null);
//		}
//		#endregion
//
//		#region AddForumMesageWithGeneralCategoriesChange
//		public static void AddForumMesageWithGeneralCategoriesChange(int IncidentId, ForumThreadNodeInfo node, ArrayList newGenCats)
//		{
//			AddForumMesageWithCardChange(IncidentId, node, -1, -1, -1, -1, newGenCats, null);
//		}
//		#endregion
//
//		#region AddForumMesageWithIssueCategoriesChange
//		public static void AddForumMesageWithIssueCategoriesChange(int IncidentId, ForumThreadNodeInfo node, ArrayList newIssCats)
//		{
//			AddForumMesageWithCardChange(IncidentId, node, -1, -1, -1, -1, null, newIssCats);
//		}
//		#endregion
		#endregion
	}
}
