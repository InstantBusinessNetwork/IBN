using System;
using System.Collections;
using System.Xml;
using System.Configuration;

namespace Mediachase.IBN.Business
{
  public class RssConfigurationHandle : IConfigurationSectionHandler
  {
    #region IConfigurationSectionHandler Members

    public object Create(object parent, object configContext, XmlNode section)
    {
      Hashtable ht = new Hashtable();
      XmlNodeList apps = section.SelectNodes("Rss");
      foreach (XmlNode app in apps)
      {
        string sCulture = app.Attributes["culture"].Value.ToLower();
        string sPath = app.Attributes["urlPath"].Value;
        if (!ht.Contains(sCulture))
          ht.Add(sCulture, sPath);
      }
      return ht;
    }

    #endregion
  }
}
