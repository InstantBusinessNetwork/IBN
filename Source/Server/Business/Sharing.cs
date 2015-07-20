using System;
using System.Data;
using System.Collections;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for Sharing.
	/// </summary>
	public class Sharing
	{
		#region GetSharingLevel
		public static int GetSharingLevel(ObjectTypes ObjectType, int ObjectId)
		{
			int UserId = Security.CurrentUser.UserID;

			int RetVal = -1;
			switch(ObjectType) 
			{
				case ObjectTypes.ToDo:
					RetVal = DBToDo.GetSharingLevel(UserId, ObjectId);
					break;
				case ObjectTypes.Task:
					RetVal = DBTask.GetSharingLevel(UserId, ObjectId);
					break;
				case ObjectTypes.CalendarEntry:
					RetVal = DBEvent.GetSharingLevel(UserId, ObjectId);
					break;
				case ObjectTypes.Issue:
					RetVal = DBIncident.GetSharingLevel(UserId, ObjectId);
					break;
				case ObjectTypes.Project:
					RetVal = DBProject.GetSharingLevel(UserId, ObjectId);
					break;
				case ObjectTypes.Document:
					RetVal = DBDocument.GetSharingLevel(UserId, ObjectId);
					break;
				default:
					RetVal = -1;
					break;
			}
			return RetVal;
		}
		#endregion
	}
}
