using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutlookAddin.OutlookUI
{
    public interface ISettingView
    {
        void SetSetting(object setting);
        void HarvestSetting(ref object setting);
    }
}
