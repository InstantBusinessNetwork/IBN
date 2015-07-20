using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Mediachase.IBN.Business
{
  public class UserDateTime
  {
    private const string sUserNow = "usernow";

    public static DateTime UserNow
    {
      get
      {
        return _UserNow();
      }

    }

    public static DateTime UserToday
    {
      get
      {
        return _UserToday();
      }
    }

    private static DateTime _UserNow()
    {
      DateTime usernow = DateTime.UtcNow;
      HttpContext context = HttpContext.Current;

      if (context.Items.Contains(sUserNow))
      {
        usernow = (DateTime)context.Items[sUserNow];
      }
      else
      {
        try
        {
          usernow = User.GetLocalDate(usernow);
          context.Items.Add(sUserNow, usernow);
        }
        catch
        {
        }
      }
      return usernow;
    }

    private static DateTime _UserToday()
    {
      DateTime today = _UserNow();
      return new DateTime(today.Year, today.Month, today.Day);
    }
  }
}