using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Net.Wdom;
using Mediachase.IBN.Database.WebDAV;
using System.Xml;
using Mediachase.IBN.Business.WebDAV.Definition;
using Mediachase.Net.WebDavServer;
using Mediachase.IBN.Business.WebDAV.ElementStorageProvider;
using Mediachase.IBN.Business.WebDAV.Common;
using System.Web;

namespace Mediachase.IBN.Business.WebDAV
{
	/// <summary>
	/// Represents current locked WebDav resources manager
	/// </summary>
	public static class WebDavSessionManager
	{

		/// <summary>
		/// Gets the active locked webdav resources.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<WebDavLockInfo> GetActiveLocksInfo()
		{
			WebDavAbstractFactory factory = new WebDavAbstractFactory();
			foreach (WebDavStorageElementPropertiesRow row in WebDavStorageElementPropertiesRow.List())
			{
				WebDavElementStorageProvider provider = factory.Create<WebDavElementStorageProvider>(row.ObjectTypeId);
				ResourceInfo resInfo = GetResourceInfoFromPropertyRow(row);
				if (resInfo != null)
				{
					foreach (ActiveLockElement activeLockEl in provider.GetActiveLocks(resInfo))
					{
						OpaqueLockToken lockToken = OpaqueLockToken.Parse(activeLockEl.LockToken.InnerText);
						McLockElement mcLockEl = GetMcLockElement(lockToken, provider.GetProperties(resInfo));

						WebDavLockInfo retval = new WebDavLockInfo();
						retval.WebDavElementPropertyId = row.PrimaryKeyId.Value;
						retval.FileName = resInfo.Name;
						retval.ContentTypeId = ContentTypeResolver.GetContentTypeId(resInfo.ContentType);
						retval.StartLocking = new DateTime(mcLockEl.CreationDate * TimeSpan.TicksPerSecond);
						retval.Duration = DateTime.UtcNow - retval.StartLocking;
						UserLight user = UserLight.Load(activeLockEl.Owner);
						if (user != null)
						{
							retval.LockedBy = user.DisplayName;
						}
						else
						{
							retval.LockedBy = "Unknow";
						}

						yield return retval;
					}
				}
			}
		}

		/// <summary>
		/// Unlocks the webdav resource.
		/// </summary>
		/// <param name="resource">The resource.</param>
		public static void UnlockResource(int webDavElementPropertyId)
		{
			WebDavAbstractFactory factory = new WebDavAbstractFactory();
			WebDavStorageElementPropertiesRow propertyRow = null;
			try
			{
				propertyRow = new WebDavStorageElementPropertiesRow(webDavElementPropertyId);
			}
			catch
			{
				return;
			}
			ResourceInfo resInfo = GetResourceInfoFromPropertyRow(propertyRow);
			if (resInfo != null)
			{
				WebDavTicket ticket = WebDavTicket.Parse(resInfo.AbsolutePath);
				WebDavElementStorageProvider storageProvider =
									factory.Create<WebDavElementStorageProvider>(ticket.AbsolutePath.StorageType);

				foreach (ActiveLockElement activeLockEl in storageProvider.GetActiveLocks(resInfo))
				{
					OpaqueLockToken lockToken = OpaqueLockToken.Parse(activeLockEl.LockToken.InnerText);
					storageProvider.Unlock(resInfo, lockToken);
				}
			}

		}

		/// <summary>
		/// Gets the file locked user id.
		/// </summary>
		/// <param name="fileUrl">The file URL.</param>
		/// <returns></returns>
		public static UserLight GetFileLockedUserId(string fileUrl)
		{
			UserLight retVal = null;
			try
			{
				WebDavTicket ticket = WebDavUrlBuilder.GetWebDavTicket(fileUrl);
				WebDavAbstractFactory factory = new WebDavAbstractFactory();
				WebDavPropertyStorage provider =
									   factory.Create<WebDavElementStorageProvider>(ticket.AbsolutePath.StorageType) as WebDavPropertyStorage;
				if (provider != null)
				{
					WebDavElementInfo elInfo = provider.GetElementInfo(ticket.ToString());
					foreach (ActiveLockElement activeLock in provider.GetActiveLocks(elInfo))
					{
						UserLight user = UserLight.Load(activeLock.Owner);
						if (user != null)
						{
							retVal = user;
							break;
						}
					}
				}

			}
			catch (System.Exception)
			{

			}

			return retVal;
		}


		private static ResourceInfo GetResourceInfoFromPropertyRow(WebDavStorageElementPropertiesRow propertyRow)
		{
			if (propertyRow == null)
			{
				throw new ArgumentNullException("propertyRow");
			}
			ResourceInfo retVal = null;
			WebDavAbstractFactory factory = new WebDavAbstractFactory();
			//For MetaData stored int hash for Guid value is not possible restore back from hash to Guid
			if ((ObjectTypes)propertyRow.ObjectTypeId != ObjectTypes.File_MetaData)
			{
				WebDavElementStorageProvider provider = factory.Create<WebDavElementStorageProvider>(propertyRow.ObjectTypeId);
				WebDavAbsolutePath absPath = WebDavAbsolutePath.CreateInstance((ObjectTypes)propertyRow.ObjectTypeId);
				absPath.UniqueId = propertyRow.ObjectId;
				absPath.FileName = "file";
				WebDavTicket ticket = WebDavTicket.CreateInstance(ePluginToken.webdav, Guid.Empty, absPath);
				try
				{
					retVal = provider.GetElementInfo(ticket.ToString()) as ResourceInfo;
				}
				catch (Exception)
				{
				}
			}
			return retVal;
		}

		private static McLockElement GetMcLockElement(OpaqueLockToken lockToken, PropertyInfoCollection propInfoColl)
		{
			if (propInfoColl == null)
			{
				throw new ArgumentNullException("propInfoColl");
			}

			McLockElement retVal = null;
			PropertyInfo mcLockDateProp = propInfoColl[PropertyInfo.McLockDateProperty];

			if (mcLockDateProp != null)
			{
				WebDavDocument document = WebDavDocument.CreateDocument();
				McLockDateElement mcLockDateEl = (McLockDateElement)
								document.ImportNode((XmlElement)mcLockDateProp.Value, true);

				//Try to find McLock element with same token as lockToken
				foreach (McLockElement lockEl in mcLockDateEl.GetLocks())
				{
					if (lockEl.Token == lockToken.ToString())
					{
						retVal = lockEl;
						break;
					}
				}
			}

			return retVal;
		}
		
	}
}
