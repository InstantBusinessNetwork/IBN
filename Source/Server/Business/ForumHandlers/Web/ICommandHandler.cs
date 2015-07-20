using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.IBN.Business
{
    public interface ICommandHandler
    {
        void Init(Object Sender);
    }
}