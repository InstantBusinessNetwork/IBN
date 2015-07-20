using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.UI.Web.Apps.IbnCommon.WidgetControls
{
	public partial class GoogleGadget : System.Web.UI.UserControl
	{

		#region prop: PageSource
		/// <summary>
		/// Gets or sets the page source.
		/// </summary>
		/// <value>The page source.</value>
		public string PageSource
		{
			get
			{
				if (ControlProperties.Provider.GetValue(this.ID, "PageSource") != null)
					return ControlProperties.Provider.GetValue(this.ID, "PageSource").ToString();

				return string.Empty;
			}
			set
			{
				ControlProperties.Provider.SaveValue(this.ID, "PageSource", value);
			}
		}
		#endregion

		#region Page_Load
		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(this.PageSource))
			{
				GoogleGadgetEntity gge = (GoogleGadgetEntity)BusinessManager.Load("GoogleGadget", PrimaryKeyId.Parse(this.PageSource));
				if (gge != null)
				{
					mainFrame.Attributes.Add("src", gge.Link.Replace("&", "&amp;"));
					ltEmpty.Visible = false;
					mainFrame.Visible = true;
				}
				else
				{
					//todo: show error message
					ltEmpty.Visible = true;
					mainFrame.Visible = false;
				}
			}
			else
			{
				ltEmpty.Visible = true;
				mainFrame.Visible = false;
			}
		} 
		#endregion
	}
}