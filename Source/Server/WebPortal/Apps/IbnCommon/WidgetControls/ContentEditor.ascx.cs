using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.IBN.Business.WidgetEngine;
using System.Globalization;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.Apps.Shell.Modules
{
	public partial class ContentEditor : System.Web.UI.UserControl
	{

		private const string _iframeMode = "iframe";
		private const string _htmlMode = "html";

		#region prop: PageMode
		/// <summary>
		/// Gets or sets the page mode.
		/// </summary>
		/// <value>The page mode.</value>
		public string PageMode
		{
			get
			{
				if (ControlProperties.Provider.GetValue(this.ID, "PageMode") != null)
					return ControlProperties.Provider.GetValue(this.ID, "PageMode").ToString();

				return _htmlMode;
			}
			set
			{
				ControlProperties.Provider.SaveValue(this.ID, "PageMode", value);
			}
		} 
		#endregion

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

		#region prop: BlockHeight
		/// <summary>
		/// Gets the height of the block.
		/// </summary>
		/// <value>The height of the block.</value>
		public int BlockHeight
		{
			get
			{
				if (ControlProperties.Provider.GetValue(this.ID, "BlockHeight") != null)
				{
					return Convert.ToInt32(ControlProperties.Provider.GetValue(this.ID, "BlockHeight").ToString(), CultureInfo.InvariantCulture);
				}

				return -1;
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
			if (this.BlockHeight != -1)
			{
				mainFrame.Style.Add(HtmlTextWriterStyle.Height, string.Format("{0}px", this.BlockHeight));
			}
		} 
		#endregion

		#region Page_PreRender
		/// <summary>
		/// Handles the PreRender event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (this.PageMode == _iframeMode)
			{
				mainFrame.Attributes.Add("src", this.PageSource);
				mainFrame.Visible = true;
				divHtml.Visible = false;
			}
			else
			{
				if (ControlProperties.Provider.GetValue(this.ID, "HtmlValue") != null)
					divHtml.InnerHtml = ControlProperties.Provider.GetValue(this.ID, "HtmlValue").ToString();
				else
					divHtml.InnerText = CHelper.GetResFileString("{IbnFramework.Global:_ce_EmptyBody}");

				mainFrame.Visible = false;
				divHtml.Visible = true;
			}
		} 
		#endregion
	}
}