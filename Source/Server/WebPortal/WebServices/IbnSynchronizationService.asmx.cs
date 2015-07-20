using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using Mediachase.Sync.Core;
using Mediachase.Sync.EntityObjectProvider;
using Microsoft.Synchronization;
using Mediachase.Sync.Core.Common;
using Mediachase.Ibn.Events;
using Mediachase.IBN.Business;
using System.Net;
using System.Security.Principal;
using Mediachase.Ibn.Data;
using Mediachase.Sync.Core.ErrorManagement;

namespace Mediachase.UI.Web.WebServices
{
	/// <summary>
	/// Summary description for IbnSynchronizationService
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	public class IbnSynchronizationService : System.Web.Services.WebService
	{

		/// <summary>
		/// Gets or sets the session principal.
		/// </summary>
		/// <value>The session principal.</value>
		UserLight SessionPrincipal
		{
			get { return Session["SynchronizationAuthData"] as UserLight; }
			set { Session["SynchronizationAuthData"] = value; }
		}

		/// <summary>
		/// Gets or sets the type of the session provider.
		/// </summary>
		/// <value>The type of the session provider.</value>
		eSyncProviderType SessionProviderType
		{
			get
			{
				object retVal = Session["SynchronizationProviderType"];
				if (retVal == null)
				{
					return eSyncProviderType.Undef;
				}

				return (eSyncProviderType)retVal;
			}

			set { Session["SynchronizationProviderType"] = value; }

		}

		/// <summary>
		/// Authenticates the specified credentials.
		/// </summary>
		/// <param name="credentials">The credentials.</param>
		private void Authenticate(NetworkCredential credentials)
		{
			try
			{
				string sUserLight = "userlight";
				UserLight currentUser = SessionPrincipal;
				if (currentUser == null)
				{
					// check user's name and password here
					currentUser = Security.GetUser(credentials.UserName, credentials.Password);

					//store user to web service session
					SessionPrincipal = currentUser;
				}

				if (HttpContext.Current.Items.Contains(sUserLight))
					HttpContext.Current.Items.Remove(sUserLight);

				HttpContext.Current.Items.Add(sUserLight, currentUser);
				HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(currentUser.Login), null);

				DataContext.Current.CurrentUserId = Security.CurrentUser.UserID;
				DataContext.Current.CurrentUserTimeZone = Security.CurrentUser.CurrentTimeZone;
			}
			catch(Exception e)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
														  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.AuthFailed, e),
														  false);
			}
			
		}

		/// <summary>
		/// Gets the session provider.
		/// </summary>
		/// <returns></returns>
		private GenericRemoteSyncProvider<EntityObjectHierarchy> GetSessionProvider()
		{
			//if (SessionPrincipal == null || SessionPrincipal.UserID == -1)
			//{
			//    throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
			//                                          new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.NotAuthRequest, "not auth request"),
			//                                          false);
			//}

			Authenticate(null);

			if (SessionProviderType == eSyncProviderType.Undef)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
													  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.ProviderNotSpecified, "provider not specified"),
													  false);
			}

			GenericRemoteSyncProvider<EntityObjectHierarchy> retVal = null;
			retVal =  EntitySyncProviderManager.GetProvider(SessionPrincipal.UserID, SessionProviderType);

			if (retVal == null)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
													  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncProvider, "provider not found"),
													  false);
			}

			return retVal;
		}


		/// <summary>
		/// Sets the credentials.
		/// </summary>
		/// <param name="credentials">The credentials.</param>
		[WebMethod(EnableSession = true)]
		public void SetCredentials(NetworkCredential credentials)
		{
			Authenticate(credentials);
		}

		/// <summary>
		/// Sets the provider type for sync session.
		/// </summary>
		/// <param name="providerType">Type of the provider.</param>
		[WebMethod(EnableSession = true)]
		public void SetProviderTypeForSyncSession(eSyncProviderType providerType)
		{
			SessionProviderType = providerType;
		}

		/// <summary>
		/// Gets the id formats.
		/// </summary>
		/// <returns></returns>
		[WebMethod(EnableSession = true)]
		public byte[] GetIdFormats()
		{
			KnowledgeSyncProvider provider = GetSessionProvider();

			byte[] retVal = null;
			try
			{
				retVal = SerializerHelper.BinarySerialize(provider.IdFormats);
			}
			catch (SyncException e)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
													  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncFramework, e),
													  true);
			}
			catch (Exception e)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
													  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncProvider, e),
													  true);
			}

			return retVal;
		}


		/// <summary>
		/// Begins the session.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public void BeginSession()
		{
			GenericRemoteSyncProvider<EntityObjectHierarchy> provider = GetSessionProvider();
			try
			{
				provider.BeginSession(SyncProviderPosition.Unknown, null);
			}
			catch (SyncException e)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
													  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncFramework, e),
													  true);
			}
			catch (Exception e)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
												  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncProvider, e),
												  true);
			}

		}


		/// <summary>
		/// Ends the session.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public void EndSession()
		{

			try
			{
				KnowledgeSyncProvider provider = GetSessionProvider();
				provider.EndSession(null);
			}
			catch
			{
			}
			//catch(SyncException e)
			//{
			//    throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
			//                                      new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncFramework, e),
			//                                      true);
			//}
			//catch (Exception e)
			//{
			//    throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
			//                                      new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncProvider, e),
			//                                      true);
			//}
		}


		/// <summary>
		/// Gets the change batch.
		/// </summary>
		/// <param name="batchSize">Size of the batch.</param>
		/// <param name="rawDestinationKnowledge">The raw destination knowledge.</param>
		/// <param name="changeDataRetriever">The change data retriever.</param>
		/// <returns></returns>
		[WebMethod(EnableSession = true)]
		public byte[] GetChangeBatch(uint batchSize, byte[] rawDestinationKnowledge,
								   out byte[] changeDataRetriever)
		{
			GenericRemoteSyncProvider<EntityObjectHierarchy> provider = GetSessionProvider();
			byte[] retVal = null;
			try
			{
				SyncKnowledge destinationKnowledge = SyncKnowledge.Deserialize(provider.IdFormats, rawDestinationKnowledge);
				object dataRetriever;
				ChangeBatch changeBatch = provider.GetChangeBatch(batchSize, destinationKnowledge, out dataRetriever);
				CachedChangeDataRetriever cachedChangeDataRetriever =
												new CachedChangeDataRetriever(dataRetriever as IChangeDataRetriever, changeBatch);
				string debugCachedRetr = SerializerHelper.XmlSerialize(cachedChangeDataRetriever);
				changeDataRetriever = SerializerHelper.BinarySerialize(cachedChangeDataRetriever);
				retVal = changeBatch.Serialize();
			}
			catch (SyncException e)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
												  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncFramework, e),
												  true);
			}
			catch (Exception e)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
												  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncProvider, e),
												  true);
			}
			return retVal;
		}


		/// <summary>
		/// Gets the full enumeration change batch.
		/// </summary>
		/// <param name="batchSize">Size of the batch.</param>
		/// <param name="lowerEnumerationBoundRaw">The lower enumeration bound raw.</param>
		/// <param name="rawKnowledgeForDataRetrieval">The raw knowledge for data retrieval.</param>
		/// <param name="changeDataRetriever">The change data retriever.</param>
		/// <returns></returns>
		[WebMethod(EnableSession = true)]
		public byte[] GetFullEnumerationChangeBatch(uint batchSize, byte[] lowerEnumerationBoundRaw,
																 byte[] rawKnowledgeForDataRetrieval,
																 out byte[] changeDataRetriever)
		{
			GenericRemoteSyncProvider<EntityObjectHierarchy> provider = GetSessionProvider();
			byte[] retVal = null;
			try
			{
				SyncKnowledge knowledgeForDataRetrieval = SyncKnowledge.Deserialize(provider.IdFormats,
																					rawKnowledgeForDataRetrieval);
				SyncId lowerEnumerationBound = new SyncId(lowerEnumerationBoundRaw, false);
				object dataRetriever;
				FullEnumerationChangeBatch changeBatch = provider.GetFullEnumerationChangeBatch(batchSize,
																								lowerEnumerationBound,
																								knowledgeForDataRetrieval,
																								out dataRetriever);
				CachedChangeDataRetriever cachedChangeDataRetriever =
												new CachedChangeDataRetriever(dataRetriever as IChangeDataRetriever, changeBatch);
				changeDataRetriever = SerializerHelper.BinarySerialize(cachedChangeDataRetriever);
				retVal = changeBatch.Serialize();
			}
			catch (SyncException e)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
												  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncFramework, e),
												  true);
			}
			catch (Exception e)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
												  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncProvider, e),
												  true);
			}

			return retVal;
		}

		/// <summary>
		/// Gets the sync batch parameters.
		/// </summary>
		/// <param name="batchSize">Size of the batch.</param>
		/// <param name="rawKnowledge">The raw knowledge.</param>
		[WebMethod(EnableSession = true)]
		public void GetSyncBatchParameters(out uint batchSize, out byte[] rawKnowledge)
		{
			GenericRemoteSyncProvider<EntityObjectHierarchy> provider = GetSessionProvider();
			try
			{
				SyncKnowledge knowledge;
				provider.GetSyncBatchParameters(out batchSize, out knowledge);
				rawKnowledge = knowledge.Serialize();
			}
			catch (SyncException e)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
												  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncFramework, e),
												  true);
			}
			catch (Exception e)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
												  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncProvider, e),
												  true);
			}
		}

		/// <summary>
		/// Processes the change batch.
		/// </summary>
		/// <param name="resolutionPolicy">The resolution policy.</param>
		/// <param name="sourceChanges">The source changes.</param>
		/// <param name="rawChangeDataRetriever">The raw change data retriever.</param>
		/// <param name="changeApplierInfo">The change applier info.</param>
		/// <returns></returns>
		[WebMethod(EnableSession = true)]
		public byte[] ProcessChangeBatch(int resolutionPolicy, byte[] sourceChanges,
								  byte[] rawChangeDataRetriever, byte[] changeApplierInfo)
		{
			GenericRemoteSyncProvider<EntityObjectHierarchy> provider = GetSessionProvider();
			byte[] retVal = null;
			try
			{
				ChangeBatch sourceChangeBatch =
								ChangeBatch.Deserialize(provider.IdFormats, sourceChanges);
				CachedChangeDataRetriever chachedDataRetriever =
								SerializerHelper.BinaryDeserialize<CachedChangeDataRetriever>(rawChangeDataRetriever);

				retVal = provider.ProcessRemoteChangeBatch((ConflictResolutionPolicy)resolutionPolicy, sourceChangeBatch,
														chachedDataRetriever, changeApplierInfo);
			}
			catch (SyncException e)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
												  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncFramework, e),
												  true);
			}
			catch (Exception e)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
												  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncProvider, e),
												  true);
			}
			return retVal;
		}

		/// <summary>
		/// Processes the full enumeration change batch.
		/// </summary>
		/// <param name="resolutionPolicy">The resolution policy.</param>
		/// <param name="sourceChanges">The source changes.</param>
		/// <param name="rawChangeDataRetriever">The raw change data retriever.</param>
		/// <param name="changeApplierInfo">The change applier info.</param>
		/// <returns></returns>
		[WebMethod(EnableSession = true)]
		public byte[] ProcessFullEnumerationChangeBatch(int resolutionPolicy, byte[] sourceChanges,
												byte[] rawChangeDataRetriever, byte[] changeApplierInfo)
		{
			GenericRemoteSyncProvider<EntityObjectHierarchy> provider = GetSessionProvider();
			byte[] retVal = null;
			try
			{
				FullEnumerationChangeBatch sourceChangeBatch =
									FullEnumerationChangeBatch.Deserialize(provider.IdFormats, sourceChanges);
				CachedChangeDataRetriever chachedDataRetriever =
									SerializerHelper.BinaryDeserialize<CachedChangeDataRetriever>(rawChangeDataRetriever);
				retVal = provider.ProcessRemoteFullEnumerationChangeBatch((ConflictResolutionPolicy)resolutionPolicy, sourceChangeBatch,
																		chachedDataRetriever, changeApplierInfo);
			}
			catch (SyncException e)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
												  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncFramework, e),
												  true);
			}
			catch (Exception e)
			{
				throw SoapErrorCreator.RaiseException(HttpContext.Current.Request.Url.ToString(),
												  new SyncronizationServiceError(SyncronizationServiceError.eServiceErrorType.SyncProvider, e),
												  true);
			}

			return retVal;
		}

		/// <summary>
		/// Cleanups the tombstones.
		/// </summary>
		/// <param name="timespan">The timespan.</param>
		[WebMethod(EnableSession = true)]
		public void CleanupTombstones(TimeSpan timespan)
		{
			//TODO: Need impl
		}
	}
}
