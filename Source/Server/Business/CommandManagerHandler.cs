using System;
using System.Collections;
using System.Xml;
using System.Configuration;

namespace Mediachase.IBN.Business
{
    public class CommandManagerHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, XmlNode section)
        {
            ArrayList  aList = new ArrayList();
            XmlNodeList apps = section.SelectNodes("itemHandler");
            //string basePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
            //basePath += section.Attributes["modulesDir"].InnerText + System.IO.Path.DirectorySeparatorChar.ToString();

            foreach (XmlNode app in apps)
            {
                aList.Add(/*basePath + */app.Attributes["fileName"].InnerText);
            }
            return aList;
        }

        #endregion
    }
}
