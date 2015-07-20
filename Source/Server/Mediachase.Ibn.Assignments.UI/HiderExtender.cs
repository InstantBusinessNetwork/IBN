using System;
using System.Collections.Generic;

using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Threading;

namespace Mediachase.Ibn.Assignments.UI
{
	[TargetControlType(typeof(Control))]
	public class HiderExtender : ExtenderControl
	{
		#region prop: ExchangeTarget
		/// <summary>
		/// Gets or sets the exchange target.
		/// </summary>
		/// <value>The exchange target.</value>
		public string ExchangeTarget
		{
			get
			{
				if (ViewState["__exchangeTarget"] == null)
					return string.Empty;

				return ViewState["__exchangeTarget"].ToString();
			}
			set
			{
				ViewState["__exchangeTarget"] = value;
			}
		} 
		#endregion

		#region ExtenderControl
		/// <summary>
		/// Gets the script descriptors.
		/// </summary>
		/// <param name="targetControl">The target control.</param>
		/// <returns></returns>
		protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors(Control targetControl)
		{
			ScriptControlDescriptor descriptor = new ScriptControlDescriptor("Mediachase.Hider", targetControl.ClientID);
			descriptor.AddElementProperty("exchangeTarget", this.ExchangeTarget);

			return new ScriptDescriptor[] { descriptor };
		}

		/// <summary>
		/// Gets the script references.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<ScriptReference> GetScriptReferences()
		{
			ScriptReference reference = new ScriptReference();

			reference.Path = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "Mediachase.Ibn.Assignments.UI.Scripts.Hider.js");

			return new ScriptReference[] { reference };
		}
		#endregion
	}
}
