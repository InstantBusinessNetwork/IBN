using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.MetaUI
{
    /// <summary>
    /// Summary description for IFactory
    /// </summary>
    public interface IFactory
    {
        DataControlField GetColumn(Page PageInstance, MetaField Field);
    }
}