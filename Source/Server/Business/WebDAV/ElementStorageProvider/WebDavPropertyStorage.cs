using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Net.WebDavServer;
using Mediachase.Net.Wdom;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Database.WebDAV;
using Mediachase.Ibn.Data;
using System.Web;
using Mediachase.IBN.Business.WebDAV.Definition;
using System.Xml;

namespace Mediachase.IBN.Business.WebDAV.ElementStorageProvider
{
    public abstract class WebDavPropertyStorage : WebDavElementStorageProvider
    {
        #region Properies
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public override Mediachase.Net.Wdom.PropertyInfoCollection GetProperties(Mediachase.Net.Wdom.WebDavElementInfo element)
        {
            PropertyInfoCollection retVal = null;
            PropertyInfo prop = null;
            WebDavDocument tmpDoc = WebDavDocument.CreateDocument();

            if (element == null)
                return retVal;

            retVal = new PropertyInfoCollection();
            //FileInfo fileInfo = !!!!(FileInfo)element.Tag;!!!
            WebDavTicket ticket = WebDavTicket.Parse(element.AbsolutePath);
            
            WebDavStorageElementPropertiesRow row = GetWebDavStoragePropertyRow(ticket.AbsolutePath.StorageType, ticket.AbsolutePath.UniqueId);
            if (row != null)
            {
                retVal = Mediachase.Ibn.Data.McXmlSerializer.GetObject<PropertyInfoCollection>(row.Value);
            }

            #region CreateDefaultProperties
            foreach (PropertyInfo defaultProp in PropertyInfo.CreateDefaultProperties(element))
            {
                SetPropertyAndSave(retVal, defaultProp, false, false);
            }
            #endregion

            //Add <supportedlock> property
            if (((int)(WebDavApplication.Class & WebDavServerClass.Class2)) != 0)
            {
                prop = PropertyInfo.CreateSupportedLockProperty();
                prop.Calculated = false;
                SetPropertyAndSave(retVal, prop, false, false);
            }

            //Add <resourcetype> property
            prop = PropertyInfo.CreateResourceTypeProperty(element);
            SetPropertyAndSave(retVal, prop, false, false);

            //Assign context and set closure to class member
            retVal.ElementInfo = element;


            return retVal;
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="property">The property.</param>
        public override void SetProperty(Mediachase.Net.Wdom.PropertyInfoCollection collection, Mediachase.Net.Wdom.PropertyInfo property)
        {
            if (property != null)
            {
                SetPropertyAndSave(collection, property, true, true);
            }
        }

        /// <summary>
        /// Removes the property.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="propName">Name of the prop.</param>
        public override void RemoveProperty(Mediachase.Net.Wdom.PropertyInfoCollection collection, string propName)
        {
            if (collection != null)
            {
                PropertyInfo propToRemove = collection[propName];
                if (propToRemove != null)
                {
                    collection.Remove(propToRemove);
                    SavePropertyCollection(collection);
                }

            }
        }

        /// <summary>
        /// Sets the property and save.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="prop">The prop.</param>
        /// <param name="save">if set to <c>true</c> [save].</param>
        private void SetPropertyAndSave(PropertyInfoCollection collection, PropertyInfo property, bool save, bool overwrite)
        {
            if (collection != null)
            {
                PropertyInfo setProperty = collection[property.Name];

                if (setProperty == null)
                {
                    collection.Add(property);
                }
                else if (overwrite == true)
                {
                    setProperty.Namespace = property.Namespace;
                    //Calculated property not set
                    if (property.Calculated == false)
                        setProperty.Value = property.Value;
                }

                if (save == true)
                    SavePropertyCollection(collection);
            }
        }

        /// <summary>
        /// Saves the property collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        private void SavePropertyCollection(PropertyInfoCollection collection)
        {
            if (collection == null)
                return;

            bool haveStoredProp = false;
            WebDavStorageElementPropertiesRow row = null;

            foreach (PropertyInfo prop in collection)
            {
                if (prop.Calculated == false)
                {
                    haveStoredProp = true;
                    try
                    {
                        WebDavTicket ticket = WebDavTicket.Parse(collection.ElementInfo.AbsolutePath);
                        using (TransactionScope tran = DataContext.Current.BeginTransaction())
                        {
                            row = GetWebDavStoragePropertyRow(ticket.AbsolutePath.StorageType, ticket.AbsolutePath.UniqueId);
                            if (row == null)
                            {
                                row = new WebDavStorageElementPropertiesRow();
                                row.ObjectTypeId = (int)ticket.AbsolutePath.StorageType;
                                row.ObjectId = ticket.AbsolutePath.UniqueId;
                                row.Key = "propertyCollection";
                            }
                            string value = Mediachase.Ibn.Data.McXmlSerializer.GetString<PropertyInfoCollection>(collection);
                            row.Value = value;
                            row.Update();

                            tran.Commit();
                        }
                    }
                    catch (Exception)
                    {
                        throw new HttpException(500, "Internal Server Error");
                    }
                    break;
                }
            }

            //remove property file if empty
            if (haveStoredProp == false)
            {
                try
                {
                    DeleteWebDavStorageElementPropertiesRow(row);
                }
                catch (Exception)
                {
                    throw new HttpException(500, "Unable to delete property");
                }
            }
        }

        private void DeleteWebDavStorageElementPropertiesRow(WebDavStorageElementPropertiesRow row)
        {
            if (row != null)
            {
                using (TransactionScope tran = DataContext.Current.BeginTransaction())
                {
                    row.Delete();
                    tran.Commit();
                }

            }
        }

        private WebDavStorageElementPropertiesRow GetWebDavStoragePropertyRow(ObjectTypes storageType, int objectId)
        {
            WebDavStorageElementPropertiesRow retVal = null;

            FilterElement filterEl1 = new FilterElement("ObjectTypeId", FilterElementType.Equal, (int)storageType);
            FilterElement filterEl2 = new FilterElement("ObjectId", FilterElementType.Equal, objectId);

            WebDavStorageElementPropertiesRow[] propertyRow = WebDavStorageElementPropertiesRow.List(filterEl1, filterEl2);

            if (propertyRow.Length > 0)
            {
                retVal = propertyRow[0];
            }

            return retVal;
        }

        #endregion


        #region Lock
        /// <summary>
        /// Locks the specified depth.
        /// </summary>
        /// <param name="depth">The depth.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="lockInfoEl">The lock info el.</param>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public override OpaqueLockToken Lock(Mediachase.Net.Wdom.DepthValue depth, string timeout, Mediachase.Net.Wdom.LockInfoElement lockInfoEl, Mediachase.Net.Wdom.WebDavElementInfo element)
        {
			ReplaceLockOwner(lockInfoEl);
            PropertyInfoCollection collection = GetProperties(element);
            return WebDavLockManager.Lock(this, depth, timeout, lockInfoEl, collection);
        }

        /// <summary>
        /// Unlocks the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="lockToken">The lock token.</param>
        public override void Unlock(Mediachase.Net.Wdom.WebDavElementInfo element, OpaqueLockToken lockToken)
        {
            PropertyInfoCollection collection = GetProperties(element);
            WebDavLockManager.Unlock(this, collection, lockToken);
        }

        /// <summary>
        /// Gets the active locks.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
		public override IEnumerable<ActiveLockElement> GetActiveLocks(WebDavElementInfo element)
        {
			List<ActiveLockElement> retVal = new List<ActiveLockElement>();
            WebDavDocument document = WebDavDocument.CreateDocument();
            PropertyInfoCollection collection = GetProperties(element);
			//Remove obsolete lock
			WebDavLockManager.RemoveObsoleteLock(this, collection);
            PropertyInfo lockDiscoveryProp = collection[PropertyInfo.LockDiscoveryProperty];
            if (lockDiscoveryProp != null)
            {
                LockDiscoveryPropertyElement lockDiscoveryEl = (LockDiscoveryPropertyElement)
                                                     document.ImportNode((XmlElement)lockDiscoveryProp.Value, true);
                foreach (ActiveLockElement activeLockEl in lockDiscoveryEl.GetActiveLocks())
                {
					yield return activeLockEl;
                }

            }
        }


        /// <summary>
        /// Refreshes the lock.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="lockToken">The lock token.</param>
        /// <param name="timeout">The timeout.</param>
        public override void RefreshLock(Mediachase.Net.Wdom.WebDavElementInfo element, OpaqueLockToken lockToken, string timeout)
        {
            PropertyInfoCollection collection = GetProperties(element);
            WebDavLockManager.RefreshLock(this, collection, lockToken, timeout);
        }

        #endregion

        private static void ReplaceLockOwner(Mediachase.Net.Wdom.LockInfoElement lockInfoEl)
        {
			if (lockInfoEl != null)
			{
				string newValue = Security.CurrentUser.Login;
				if (!String.IsNullOrEmpty(newValue))
				{
					lockInfoEl.Owner.InnerText = newValue;
				}
			}
        }
    }
}
