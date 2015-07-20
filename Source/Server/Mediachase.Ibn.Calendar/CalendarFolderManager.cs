using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Calendar
{
    /// <summary>
    /// Represents: Calendar folder management. 
    /// </summary>
    public static class CalendarFolderManager
    {
        #region Events
        public static event EventHandler CalendarCreating;
        public static event EventHandler CalendarCreated;

        public static event EventHandler CalendarLinkCreating;
        public static event EventHandler CalendarLinkCreated;

        public static event EventHandler CalendarLinkDeleting;
        public static event EventHandler CalendarLinkDeleted;

        public static event EventHandler CalendarDeleting;
        public static event EventHandler CalendarDeleted;

        public static event EventHandler CalendarMoving;
        public static event EventHandler CalendarMoved;
             
        #endregion

        #region Rais Event Methods
        /// <summary>
        /// Raises the event.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        internal static void RaiseEvent(EventHandler eventHandler, object sender, EventArgs args)
        {
            if (eventHandler != null)
                eventHandler(sender, args);
        }

       #endregion

        #region Get XXX Root
        /// <summary>
        /// Gets the public root.
        /// </summary>
        /// <returns></returns>
        public static CalendarFolder GetPublicRoot()
        {
            CalendarFolder[] nodes = CalendarFolder.List(FilterElement.IsNullElement(TreeService.ParentIdFieldName),
                                                         FilterElement.IsNullElement("ProjectId"));

            if (nodes.Length > 0)
                return nodes[0];

            // Create Public Root
            return CreateRootNode("Public", null, null);
        }

        /// <summary>
        /// Gets the project root.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <returns></returns>
        public static CalendarFolder GetProjectRoot(int projectId)
        {
            CalendarFolder[] nodes = CalendarFolder.List(FilterElement.IsNullElement(TreeService.ParentIdFieldName),
                FilterElement.IsNullElement("Owner"),
                FilterElement.EqualElement("ProjectId", projectId));

            if (nodes.Length > 0)
                return nodes[0];

            // Create Project Root
            return CreateRootNode(string.Format(CultureInfo.InvariantCulture, "Project_{0}", projectId), projectId, null);
        }

        /// <summary>
        /// Gets the private root.
        /// </summary>
        /// <param name="ownerId">The owner id.</param>
        /// <returns></returns>
        public static CalendarFolder GetPrivateRoot(int ownerId)
        {
            CalendarFolder[] nodes = CalendarFolder.List(FilterElement.IsNullElement(TreeService.ParentIdFieldName),
                FilterElement.EqualElement("Owner", ownerId),
                FilterElement.IsNullElement("ProjectId"));

            if (nodes.Length > 0)
                return nodes[0];

            // Create Private Root
            return CreateRootNode(string.Format(CultureInfo.InvariantCulture, "Private_{0}", ownerId), null, ownerId);
        }

        /// <summary>
        /// Creates the root.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="projectId">The project id.</param>
        /// <param name="ownerId">The owner id.</param>
        /// <returns></returns>
        private static CalendarFolder CreateRootNode(string title, int? projectId, int? ownerId)
        {
            if (title == null)
                throw new ArgumentNullException("title");

            using (TransactionScope tran = DataContext.Current.BeginTransaction())
            {
                // Step 1. Create List Folder
                CalendarFolder newRoot = new CalendarFolder();
                newRoot.Title = title;
                newRoot.ProjectId = projectId;
                newRoot.Owner = ownerId;
                newRoot.Save();

                // Step 2. Assign Root
                TreeNode retVal = TreeManager.AppendRootNode(Calendar.GetAssignedMetaClass(), newRoot);

                tran.Commit();

                return newRoot;
            }
        }
        #endregion

        #region CreateCalendar
        /// <summary>
        /// Creates the calendar.
        /// </summary>
        /// <param name="folderId">The folder id.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static CalendarInfo CreateCalendar(int folderId, string name)
        {
            CalendarFolder folder = new CalendarFolder(folderId);

            return CreateCalendar(folder, name);
        }

        /// <summary>
        /// Creates the calendar.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static CalendarInfo CreateCalendar(CalendarFolder folder, string name)
        {
            return CreateCalendar(folder, name, (int?)folder.Owner);
        }

        /// <summary>
        /// Creates the calendar.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="name">The name.</param>
        /// <param name="ownerId">The owner id.</param>
        /// <returns></returns>
        public static CalendarInfo CreateCalendar(CalendarFolder folder, string name, int? ownerId)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (folder == null)
                throw new ArgumentNullException("folder");
            if (!folder.GetTreeService().CurrentNode.IsAttached)
                throw new ArgumentException("The folder is dettached from tree.", "folder");

            Calendar newItem = null;

            using (TransactionScope tran = DataContext.Current.BeginTransaction())
            {
                // Step 1. Create New Calendar
                newItem = new Calendar();
                newItem.CalendarFolderId = folder.PrimaryKeyId.Value;
                newItem.Title = name;
                newItem.Owner = ownerId;

                // Raise List Creating Event
                RaiseEvent(CalendarCreating, newItem, EventArgs.Empty);

                // Add additional parameters
                newItem.Save();

                // Raise List Created Event
                RaiseEvent(CalendarCreated, newItem, EventArgs.Empty);

              
                // Step. Commit Transaction Return new list info
                tran.Commit();
             }

             return new CalendarInfo(newItem, null);

        }

        #endregion

        #region AddLink
        /// <summary>
        /// Adds the calendar link.
        /// </summary>
        /// <param name="folderId">The folder id.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns></returns>
        public static CalendarInfo AddCalendarLink(int folderId, Calendar calendar)
        {
            CalendarFolder folder = new CalendarFolder(folderId);

            return AddCalendarLink(folder, calendar);
        }

        /// <summary>
        /// Adds the calendar link.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns></returns>
        public static CalendarInfo AddCalendarLink(CalendarFolder folder, Calendar calendar)
        {
            if (folder == null)
                throw new ArgumentNullException("CalendarFolder");
            if (calendar == null)
                throw new ArgumentNullException("Calendar");

            if (!folder.GetTreeService().CurrentNode.IsAttached)
                throw new ArgumentException("The folder is detached from tree.", "folder");

            //Step 1. Determine for already exist link
            CalendarFolderLink[] links = CalendarFolderLink.List(new FilterElement("CalendarId",
                                                                      FilterElementType.Equal,
                                                                      calendar.PrimaryKeyId.Value),
                                                                  new FilterElement("FolderId",
                                                                      FilterElementType.Equal,
                                                                      folder.PrimaryKeyId.Value));
            if (links.Length != 0)
            {
                Calendar cal = new Calendar(links[0].CalendarId);
                return new CalendarInfo(cal, links[0]);
            }

            CalendarFolderLink link = null;
            using (TransactionScope tran = DataContext.Current.BeginTransaction())
            {
               
                // Step 2. Create New CalendarFolderLink
                link = new CalendarFolderLink();
                link.CalendarId = calendar.PrimaryKeyId.Value;
                link.FolderId = folder.PrimaryKeyId.Value;

                // Raise List Creating Event
                RaiseEvent(CalendarLinkCreating, link, EventArgs.Empty);

                // Add additional parameters
                link.Save();

                // Raise List Created Event
                RaiseEvent(CalendarLinkCreated, link, EventArgs.Empty);


                // Step. Commit Transaction Return new list info
                tran.Commit();
                   
            }

            return new CalendarInfo(calendar, link);
        }
        #endregion     

        #region Delete link

        /// <summary>
        /// Deletes the link.
        /// </summary>
        /// <param name="link">The link.</param>
        public static void DeleteLink(CalendarFolderLink link)
        {
          
            if (link == null)
                throw new ArgumentNullException("link");

            using (TransactionScope tran = DataContext.Current.BeginTransaction())
            {
                // Raise Calendar link Delete Event
                RaiseEvent(CalendarLinkDeleting, link, EventArgs.Empty);

                link.Delete();

                RaiseEvent(CalendarLinkDeleted, link, EventArgs.Empty);


                tran.Commit();
            }

        }
        #endregion

        #region DeleteCalendar
        /// <summary>
        /// Deletes the calendar.
        /// </summary>
        /// <param name="calendarId">The calendar id.</param>
        public static void DeleteCalendar(int calendarId)
        {
            DeleteCalendar(new Calendar(calendarId));
        }

        public static void DeleteCalendar(Calendar calendar)
        {
            if (calendar == null)
                throw new ArgumentNullException("calendar");

            using (TransactionScope tran = DataContext.Current.BeginTransaction())
            {
                // Raise calendar Creating Event
                RaiseEvent(CalendarDeleting, calendar, EventArgs.Empty);

                calendar.Delete();

                RaiseEvent(CalendarDeleted, calendar, EventArgs.Empty);


                tran.Commit();
            }
          
        }

        #endregion

        #region GetCalendars
        /// <summary>
        /// Lists the specified folder id.
        /// </summary>
        /// <param name="folderId">The folder id.</param>
        /// <returns></returns>
        public static CalendarInfo[] GetCalendars(int folderId)
        {
            List<CalendarInfo> retVal = new List<CalendarInfo>();

            // Load primary list
            Calendar [] calendars = Calendar.List(new FilterElement("CalendarFolderId",
                                             FilterElementType.Equal, folderId));

           List<CalendarFolderLink> links = new List<CalendarFolderLink>
                                                (CalendarFolderLink.List(new FilterElement("FolderId", 
                                                                         FilterElementType.Equal, folderId)));
            //Get primary calendar
            foreach(Calendar calendar in calendars)
            {
                CalendarInfo newItem = new CalendarInfo(calendar, null);
                              
                foreach(CalendarFolderLink link in new List<CalendarFolderLink>(links))
                {
                    //remove duplicate
                   if(link.CalendarId == calendar.PrimaryKeyId.Value)
                      links.Remove(link);
                }

                retVal.Add(newItem);
            }
            //Get links
            foreach(CalendarFolderLink link in links)
            {
                Calendar calendar = new Calendar(link.CalendarId);
                CalendarInfo newItem = new CalendarInfo(calendar, link);
                retVal.Add(newItem);

            }
           
            return retVal.ToArray();
        }

        /// <summary>
        /// Lists the specified folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns></returns>
        public static CalendarInfo[] GetCalendars(CalendarFolder folder)
        {
            CalendarInfo[] retVal = new CalendarInfo[] { };
    
           if(folder != null)
           {
               retVal = GetCalendars(folder.PrimaryKeyId.Value);
           }

           return retVal;
        }
        #endregion

        
        #region CreateFolder
        /// <summary>
        /// Creates the folder.
        /// </summary>
        /// <param name="parentId">The parent id.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static CalendarFolder CreateFolder(int parentId, string name)
        {
            return CreateFolder(new CalendarFolder(parentId), name);
        }

        /// <summary>
        /// Creates the folder.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static CalendarFolder CreateFolder(CalendarFolder parent, string name)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

          
            using (TransactionScope tran = DataContext.Current.BeginTransaction())
            {
                // Create Detached Folder
                CalendarFolder newRoot = new CalendarFolder();

                newRoot.Title = name;
                newRoot.ProjectId = parent.ProjectId;
                newRoot.Owner = parent.Owner;
                newRoot.Save();

                // Append to
                TreeNode node = parent.GetTreeService().AppendChild(newRoot);
                parent.Save();

                tran.Commit();

                return newRoot;
            }
        }
        #endregion

        #region DeleteFolder
        /// <summary>
        /// Deletes the folder.
        /// </summary>
        /// <param name="folderId">The folder id.</param>
        public static void DeleteFolder(int folderId)
        {
            DeleteFolder(new CalendarFolder(folderId));
        }


        /// <summary>
        /// Deletes the folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        public static void DeleteFolder(CalendarFolder folder)
        {
            if (folder == null)
                throw new ArgumentNullException("folder");

            using (TransactionScope tran = DataContext.Current.BeginTransaction())
            {
                TreeService treeService = folder.GetTreeService();

                //Erase all child folders
                foreach (TreeNode node in TreeManager.GetAllChildNodes(treeService.CurrentNode))
                {
                    CalendarFolder childFolder = (CalendarFolder)node.InnerObject;
                    EraseFolder(childFolder);
                    treeService.RemoveChild(node.ObjectId);
                    childFolder.Delete();
                }

                //Erase current folder
                EraseFolder(folder);
                folder.Delete();

                tran.Commit();
            }
        }

        /// <summary>
        /// Erases the folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        private static void EraseFolder(CalendarFolder folder)
        {
            CalendarInfo[] calendarsInfo = GetCalendars(folder);

            foreach (CalendarInfo calInfo in calendarsInfo)
            {
                if (calInfo.IsLink())
                    DeleteLink(calInfo.CalendarLink);
                else
                    DeleteCalendar(calInfo.Calendar);

            }
        }
        #endregion

        #region MoveCalendar
        /// <summary>
        /// Moves the calendar.
        /// </summary>
        /// <param name="calendar">The calendar.</param>
        /// <param name="newFolder">The new folder.</param>
        public static void MoveCalendar(Calendar calendar, CalendarFolder newFolder)
        {
            if (calendar == null)
                throw new ArgumentNullException("calendar");
            if (newFolder == null)
                throw new ArgumentNullException("newFolder");

            using (TransactionScope tran = DataContext.Current.BeginTransaction())
            {
                // Raise Calendar Moving
                RaiseEvent(CalendarMoving, calendar, EventArgs.Empty);

                calendar.CalendarFolderId = newFolder.PrimaryKeyId.Value;
                calendar.Owner = newFolder.Owner;

                calendar.Save();

                RaiseEvent(CalendarMoved, calendar, EventArgs.Empty);

                tran.Commit();
            }
        }
        #endregion


    }
}
