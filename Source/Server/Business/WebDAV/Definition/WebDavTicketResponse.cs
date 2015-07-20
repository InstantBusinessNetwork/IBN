using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Net.WebDavServer;
using System.Web;

namespace Mediachase.IBN.Business.WebDAV.Definition
{
    internal class WebDavTicketResponse : HttpWebDavResponse
    {
        public WebDavTicketResponse(HttpResponse response)
            :base(response)
        {

        }
    }
}
