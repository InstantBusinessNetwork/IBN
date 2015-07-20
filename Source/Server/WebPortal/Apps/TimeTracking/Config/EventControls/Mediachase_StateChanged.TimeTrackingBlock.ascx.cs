using System;
using System.Data;
using System.Collections;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Services;

namespace Mediachase.Ibn.Web.UI.TimeTracking.EventControls
{
	public partial class Mediachase_StateChanged_TimeTrackingBlock : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{	
		}

		public override void DataBind()
		{
			object bindObject = DataBinder.GetDataItem(this.Parent);
			string retVal = "";
			if (bindObject != null && bindObject is MetaObject)
			{
				string argumentType = ((MetaObject)bindObject).Properties["ArgumentType"].Value.ToString();
				string argumentData = ((MetaObject)bindObject).Properties["ArgumentData"].Value.ToString();

				Type objType = Mediachase.Ibn.Data.AssemblyUtil.LoadType(argumentType);
				object obj = McXmlSerializer.GetObject(objType, argumentData);
				if (obj != null)
				{
					PropertyInfo pinfo = objType.GetProperty("CurrentState");
					if (pinfo != null)
					{
						object curState = pinfo.GetValue(obj, null);
						if (curState is State)
							retVal = String.Format("<b>{0}:</b>&nbsp;{1}", 
								CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_State}"), 
								CHelper.GetResFileString(StateMachineManager.GetState("TimeTrackingBlock", ((State)curState).Name).Properties["FriendlyName"].Value.ToString()));
					}
					
				}
				if (String.IsNullOrEmpty(retVal))
					retVal = CHelper.GetEventResourceString((MetaObject)bindObject);

				lblStateValue.Text = retVal;
			}
			base.DataBind();
		}
	}
}