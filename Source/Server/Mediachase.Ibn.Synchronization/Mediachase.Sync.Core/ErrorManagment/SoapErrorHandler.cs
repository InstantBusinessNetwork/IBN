using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Web.Services.Protocols;


namespace Mediachase.Sync.Core.ErrorManagement
{
	public class SoapErrorHandler
	{
		#region Handle Error
		public static SyncronizationServiceError HandleError(SoapException soapException)
		{

			SyncronizationServiceError error = null;
			XmlDocument xml = new XmlDocument();
			XmlNodeList detailErrors = null;
			XmlNamespaceManager nameSpaceManager = null;

			try
			{
				if (soapException.Detail.OuterXml.IndexOf("detail") > 0)
				{
					nameSpaceManager = new XmlNamespaceManager(xml.NameTable);
					xml.LoadXml(soapException.Detail.OuterXml);
					nameSpaceManager = new XmlNamespaceManager(xml.NameTable);
					nameSpaceManager.AddNamespace("tns", soapException.Node);
					detailErrors = xml.SelectNodes("//tns:Error", nameSpaceManager);

					foreach (XmlNode detailError in detailErrors)
					{
						error = new SyncronizationServiceError();

						error.stackTrace = detailError.SelectSingleNode("./tns:ErrorStackTrace", nameSpaceManager).InnerText; 

						error.message = detailError.SelectSingleNode("./tns:ErrorMessage", nameSpaceManager).InnerText;

						error.name = detailError.SelectSingleNode("./tns:ErrorSource", nameSpaceManager).InnerText;

						string errorType = detailError.SelectSingleNode("./tns:ErrorType", nameSpaceManager).InnerText;
						error.errorType = SyncronizationServiceError.eServiceErrorType.Undef;
						if(!string.IsNullOrEmpty(errorType))
						{
							error.errorType = (SyncronizationServiceError.eServiceErrorType)
												Enum.Parse(typeof(SyncronizationServiceError.eServiceErrorType), errorType);
						}
						break;
					}
				}
			}
			catch (Exception) 
			{ 
			}
			return error;
		}
		#endregion

	}

}
