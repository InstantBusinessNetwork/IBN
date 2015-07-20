using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web.Services.Protocols;

namespace Mediachase.Sync.Core.ErrorManagement
{
	/// <summary>
	/// Summary description for SoapErrorCreator
	/// </summary>
	public class SoapErrorCreator
	{
		public static SoapException RaiseException(string webServiceUri, SyncronizationServiceError error,
												  bool isServerError)
		{
			XmlDocument xml = null;
			XmlQualifiedName faultCodeLocation = SoapException.ServerFaultCode;
			XmlNode rootNode = null;
			XmlNode errorNode = null;
			XmlNode errorNumberNode = null;
			XmlNode errorMessageNode = null;
			XmlNode errorSourceNode = null;
			XmlNode errorStackNode = null;
			SoapException soapException = null;

			try
			{

				if (error == null)
				{
					return new SoapException("web service failure with no trapped errors",
											 SoapException.ServerFaultCode);
				}

				if (!isServerError)
				{
					faultCodeLocation = SoapException.ClientFaultCode;
				}

				xml = new XmlDocument();

				rootNode = xml.CreateNode(XmlNodeType.Element,
										  SoapException.DetailElementName.Name,
										  SoapException.DetailElementName.Namespace);


				errorNode = xml.CreateNode(XmlNodeType.Element, "Error", webServiceUri);

				errorNumberNode = xml.CreateNode(XmlNodeType.Element, "ErrorType", webServiceUri);
				errorNumberNode.InnerText = error.errorType.ToString();

				errorMessageNode = xml.CreateNode(XmlNodeType.Element, "ErrorMessage", webServiceUri);
				errorMessageNode.InnerText = error.message;

				errorSourceNode = xml.CreateNode(XmlNodeType.Element, "ErrorSource", webServiceUri);
				errorSourceNode.InnerText = error.name;

				errorStackNode = xml.CreateNode(XmlNodeType.Element, "ErrorStackTrace", webServiceUri);
				errorStackNode.InnerText = error.stackTrace;

				errorNode.AppendChild(errorNumberNode);
				errorNode.AppendChild(errorMessageNode);
				errorNode.AppendChild(errorSourceNode);
				errorNode.AppendChild(errorStackNode);

				rootNode.AppendChild(errorNode);


				soapException = new SoapException("Web Service Failure", faultCodeLocation,
												   webServiceUri, rootNode);
			}
			catch
			{
			}

			return soapException;
		}
	}
}
