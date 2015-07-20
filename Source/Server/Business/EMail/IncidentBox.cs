using System;
using System.Collections;
using System.Data;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.EMail;

namespace Mediachase.IBN.Business.EMail
{
	public class IncidentBoxDuplicateNameException: Exception
	{
		public IncidentBoxDuplicateNameException():base("Cannot set duplicate name of the incident box.")
		{
		}
	}

	public class IncidentBoxDuplicateIdentifierMaskException: Exception
	{
		public IncidentBoxDuplicateIdentifierMaskException():base("Cannot set duplicate identifier mask of the incident box.")
		{
		}
	}

	/// <summary>
	/// Summary description for IncidentBox.
	/// </summary>
	public class IncidentBox
	{
		private IncidentBoxRow _srcRow = null;

		private IncidentBoxDocument _document = null;

		private IncidentBox(IncidentBoxRow row)
		{
			_srcRow = row;
		}

        /// <summary>
        /// Determines whether this instance can delete.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can delete; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanDelete(int IncidentBoxId)
        {
            return IncidentBoxRow.CanDelete(IncidentBoxId);
        }

        /// <summary>
        /// Determines whether this instance can modify.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can modify; otherwise, <c>false</c>.
        /// </returns>
		public static bool CanModify()
		{
			return Security.IsUserInGroup(InternalSecureGroups.Administrator);
		}

		public static int Create(string Name, string IdentifierMask, bool IsDefault)
		{
			return Create(Name, IdentifierMask, IsDefault, -1);
		}

		public static int Create(string Name, string IdentifierMask, bool IsDefault, int Index)
		{
			if(!CanModify())
				throw new AccessDeniedException();


			try
			{
				IncidentBoxRow row = new IncidentBoxRow();

				row.Name = Name;
				row.IdentifierMask = IdentifierMask;
				row.IsDefault = IsDefault;
				row.Index = Index;

				row.Update();

				// Create Default IncidentBoxDocument
				IncidentBoxDocument doc = IncidentBoxDocument.Load(row.PrimaryKeyId);

				doc.GeneralBlock.Manager = Security.CurrentUser.UserID;
				doc.GeneralBlock.Responsible =  Security.CurrentUser.UserID;

				IncidentBoxDocument.Save(doc);

				return row.PrimaryKeyId;
			}
			catch(System.Data.SqlClient.SqlException ex)
			{
				if(ex.Message.IndexOf("'IX_IncidentBox_1'")!=-1)
					throw new IncidentBoxDuplicateIdentifierMaskException();
				else if(ex.Message.IndexOf("'IX_IncidentBox'")!=-1)
					throw new IncidentBoxDuplicateNameException();
				else
					throw;
			}
		}

		public static IncidentBox Load(int IncidentBoxId)
		{
			return new IncidentBox(IncidentBoxRow.Load(IncidentBoxId));
		}

		public static void Update(IncidentBox box, IncidentBoxDocument document)
		{
			if(!CanModify())
				throw new AccessDeniedException();

			using(Database.DbTransaction tran = Database.DbTransaction.Begin())
			{
				IncidentBox.Update(box);
				IncidentBoxDocument.Save(document);

				// O.R.[2008-12-16]: Recalculate Current Responsible
				DBIncident.RecalculateCurrentResponsibleByIncidentBox(box.IncidentBoxId);

				tran.Commit();
			}
		}

		public static void Update(IncidentBox box)
		{
			if(!CanModify())
				throw new AccessDeniedException();

			try
			{
				box._srcRow.Update();
			}
			catch(System.Data.SqlClient.SqlException ex)
			{
				if(ex.Message.IndexOf("'IX_IncidentBox_1'")!=-1)
					throw new IncidentBoxDuplicateIdentifierMaskException();
				else if(ex.Message.IndexOf("'IX_IncidentBox'")!=-1)
					throw new IncidentBoxDuplicateNameException();
				else
					throw;
			}
		}

		public static void Delete(int IncidentBoxId)
		{
			if(!CanModify())
				throw new AccessDeniedException();

			using(Database.DbTransaction tran = Database.DbTransaction.Begin())
			{
//				using(IDataReader reader = Incident.GetListIncidentsByIncidentBox(IncidentBoxId))
//				{
//					while(reader.Read())
//					{
//						int IncidentId = (int)reader["IncidentId"];
//						Incident.Delete(IncidentId, true, false);
//					}
//				}

				IncidentBoxRow.Delete(IncidentBoxId);

				tran.Commit();
			}
		}

		/// <summary>
		/// Lists this instance.
		/// </summary>
		/// <returns></returns>
		public static IncidentBox[] List()
		{
			ArrayList retVal = new ArrayList();

			foreach(IncidentBoxRow row in IncidentBoxRow.List())
			{
				retVal.Add(new IncidentBox(row));
			}

			return (IncidentBox[])retVal.ToArray(typeof(IncidentBox));
		}

		/// <summary>
		/// Lists the with rules.
		/// </summary>
		/// <returns></returns>
		public static IncidentBox[] ListWithRules()
		{
			ArrayList retVal = new ArrayList();

			foreach(IncidentBoxRow row in IncidentBoxRow.ListWithRules())
			{
				retVal.Add(new IncidentBox(row));
			}

			return (IncidentBox[])retVal.ToArray(typeof(IncidentBox));
		}

		/// <summary>
		/// Lists the with rules.
		/// </summary>
		/// <returns></returns>
		public static IncidentBox[] ListWithoutRules()
		{
			ArrayList retVal = new ArrayList();

			foreach(IncidentBoxRow row in IncidentBoxRow.ListWithoutRules())
			{
				retVal.Add(new IncidentBox(row));
			}

			return (IncidentBox[])retVal.ToArray(typeof(IncidentBox));
		}

		public virtual IncidentBoxDocument Document
		{
			get
			{
				if(_document==null)
					_document = IncidentBoxDocument.Load(this.IncidentBoxId);
				return _document;
			}
		}
		
		/// <summary>
		/// Determines whether this instance [can delete user] the specified user id.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can delete user] the specified user id; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanDeleteUser(int UserId)
		{
			foreach(IncidentBox box in IncidentBox.List())
			{
				if(box.Document.GeneralBlock.AllowControl &&
					box.Document.GeneralBlock.ControllerAssignType == ControllerAssignType.CustomUser &&
					box.Document.GeneralBlock.Controller==UserId)
					return false;

				if(box.Document.GeneralBlock.Manager==UserId)
					return false;

				if(box.Document.GeneralBlock.ResponsibleAssignType==ResponsibleAssignType.CustomUser &&
					box.Document.GeneralBlock.Responsible==UserId)
					return false;

			}
			return true;
		}

		/// <summary>
		/// Deletes the user.
		/// </summary>
		/// <param name="UserId">The user id.</param>
		public static void DeleteUser(int UserId)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				foreach(IncidentBox box in IncidentBox.List())
				{
					bool bWasModified = false;

					if(box.Document.GeneralBlock.ResponsiblePool.Contains(UserId))
					{
						box.Document.GeneralBlock.ResponsiblePool.Remove(UserId);
						bWasModified = true;
					}

					if(box.Document.EMailRouterBlock.InformationRecipientList.Contains(UserId))
					{
						box.Document.EMailRouterBlock.InformationRecipientList.Remove(UserId);
						bWasModified = true;
					}

					if(bWasModified)
						IncidentBoxDocument.Save(box.Document);
				}

				tran.Commit();
			}
		}

		/// <summary>
		/// Replases the user.
		/// </summary>
		/// <param name="OldUserId">The old user id.</param>
		/// <param name="NewUserId">The new user id.</param>
		public static void ReplaseUser(int OldUserId, int NewUserId)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				foreach(IncidentBox box in IncidentBox.List())
				{
					bool bWasModified = false;

					if(box.Document.GeneralBlock.AllowControl &&
						box.Document.GeneralBlock.ControllerAssignType == ControllerAssignType.CustomUser &&
						box.Document.GeneralBlock.Controller==OldUserId)
					{
						box.Document.GeneralBlock.Controller = NewUserId;

						bWasModified = true;
					}

					if(box.Document.GeneralBlock.Manager==OldUserId)
					{
						box.Document.GeneralBlock.Manager = NewUserId;

						bWasModified = true;
					}

					if(box.Document.GeneralBlock.ResponsibleAssignType==ResponsibleAssignType.CustomUser &&
						box.Document.GeneralBlock.Responsible==OldUserId)
					{
						box.Document.GeneralBlock.Responsible = NewUserId;
						bWasModified = true;
					}

					if(box.Document.GeneralBlock.ResponsiblePool.Contains(OldUserId))
					{
						box.Document.GeneralBlock.ResponsiblePool.Remove(OldUserId);
						bWasModified = true;
					}

					if(box.Document.EMailRouterBlock.InformationRecipientList.Contains(OldUserId))
					{
						box.Document.EMailRouterBlock.InformationRecipientList.Remove(OldUserId);
						bWasModified = true;
					}

					if(bWasModified)
						IncidentBoxDocument.Save(box.Document);
				}

				tran.Commit();
			}
		}

		/// <summary>
		/// Changes the index of the autodetection.
		/// </summary>
		/// <param name="IncidentBoxId">The incident box id.</param>
		/// <param name="Index">The index.</param>
		public static void ChangeAutodetectionIndex(int IncidentBoxId, int Index)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				IncidentBox[] iBoxList = IncidentBox.ListWithRules();

				// fix Index problem
				if(Index<0)
					Index = 0;
				if(Index>=iBoxList.Length)
					Index = iBoxList.Length-1;

				int curIncidentBoxIndex = -1;

				for(int index = 0;index<iBoxList.Length;index++)
				{
					// Set Index as real value
					iBoxList[index].Index = index;

					// find IncidentBoxId index
					if(iBoxList[index].IncidentBoxId==IncidentBoxId)
					{
						curIncidentBoxIndex = index;
					}
				}

				if(curIncidentBoxIndex!=-1)
				{

					iBoxList[curIncidentBoxIndex].Index = Index;

					// Change Index
					for(int index = 0;index<iBoxList.Length;index++)
					{
						if(index>=Index && index<curIncidentBoxIndex)
							iBoxList[index].Index = index+1;
						if(index>curIncidentBoxIndex && index<=Index)
							iBoxList[index].Index = index-1;
						IncidentBox.Update(iBoxList[index]);
					}
				}

				tran.Commit();
			}
		}

		#region Public Properties
		
        
		public virtual int IncidentBoxId
		{
			get
			{
				return _srcRow.IncidentBoxId;
			}
		}

		
		public virtual string Name
	    
		{
			get
			{
				return _srcRow.Name;
			}
			
			set
			{
				_srcRow.Name = value;
			}	
			
		}
		
		public virtual string IdentifierMask
	    
		{
			get
			{
				return _srcRow.IdentifierMask;
			}
			
			set
			{
				_srcRow.IdentifierMask = value;
			}	
			
		}
		
		public virtual bool IsDefault
	    
		{
			get
			{
				return _srcRow.IsDefault;
			}
			
			set
			{
				_srcRow.IsDefault = value;
			}	
			
		}

		protected virtual int Index
	    
		{
			get
			{
				return _srcRow.Index;
			}
			
			set
			{
				_srcRow.Index = value;
			}	
			
		}
		
		#endregion

	}
}
