using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.Apps.WidgetEngine;


namespace Mediachase.UI.Web.Apps.Shell.Modules
{
	public partial class ContentEditorPropertyPage : System.Web.UI.UserControl, IPropertyPageControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			fckEditor.Language = Security.CurrentUser.Culture;
			fckEditor.EnableSsl = Request.IsSecureConnection;
			fckEditor.SslUrl = ResolveUrl("~/Common/Empty.html");

			lblIframe.Click += new EventHandler(lblIframe_Click);
			lblHtml.Click += new EventHandler(lblHtml_Click);

			lblIframe.Text = CHelper.GetResFileString("{IbnFramework.Global:_ce_IFrameText}");
			lblHtml.Text = CHelper.GetResFileString("{IbnFramework.Global:_ce_HtmlText}");
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (ControlProperties.Provider.GetValue(this.ID, "PageMode") != null)
			{
				if (ControlProperties.Provider.GetValue(this.ID, "PageMode").ToString() == "iframe")
				{
					ShowIframeStep();
				}
				else
				{
					ShowHtmlStep();
				}
			}

			if (ControlProperties.Provider.GetValue(this.ID, "HtmlValue") != null)
			{
				fckEditor.Text = ControlProperties.Provider.GetValue(this.ID, "HtmlValue").ToString();
			}

			if (ControlProperties.Provider.GetValue(this.ID, ControlProperties._titleKey) != null)
			{
				tbTitle.Text = ControlProperties.Provider.GetValue(this.ID, ControlProperties._titleKey).ToString();
			}

			if (ControlProperties.Provider.GetValue(this.ID, "BlockHeight") != null)
			{
				tbHeight.Text = ControlProperties.Provider.GetValue(this.ID, "BlockHeight").ToString();
			}

			if (ControlProperties.Provider.GetValue(this.ID, "PageSource") != null)
			{
				string source = ControlProperties.Provider.GetValue(this.ID, "PageSource").ToString();
				if (source.ToLower().StartsWith("http://"))
					source = source.Substring(7);
				tbSource.Text = source;
			}
		}

		#region ShowIframeStep
		/// <summary>
		/// Shows the iframe step.
		/// </summary>
		private void ShowIframeStep()
		{
			divHtml.Visible = false;
			divIframe.Visible = true;
			lblHtml.Visible = true;
			lblIframe.Visible = false;
			ControlProperties.Provider.SaveValue(this.ID, "PageMode", "iframe");
		} 
		#endregion

		#region ShowHtmlStep
		/// <summary>
		/// Shows the HTML step.
		/// </summary>
		private void ShowHtmlStep()
		{
			divHtml.Visible = true;
			divIframe.Visible = false;
			lblHtml.Visible = false;
			lblIframe.Visible = true;
			ControlProperties.Provider.SaveValue(this.ID, "PageMode", "html");
		} 
		#endregion

		#region lblHtml_Click
		/// <summary>
		/// Handles the Click event of the lblHtml control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void lblHtml_Click(object sender, EventArgs e)
		{
			ShowHtmlStep();
		}
		#endregion

		#region lblIframe_Click
		/// <summary>
		/// Handles the Click event of the lblIframe control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void lblIframe_Click(object sender, EventArgs e)
		{
			ShowIframeStep();
		}
		#endregion

		#region IPropertyPageControl Members

		/// <summary>
		/// Saves this instance.
		/// </summary>
		public void Save()
		{
			ControlProperties.Provider.SaveValue(this.ID, "HtmlValue", fckEditor.Text);
			if (divHtml.Visible)
				ControlProperties.Provider.SaveValue(this.ID, "PageMode", "html");
			else
				ControlProperties.Provider.SaveValue(this.ID, "PageMode", "iframe");

			if (tbSource.Text.ToLower().StartsWith("http://"))
				ControlProperties.Provider.SaveValue(this.ID, "PageSource", tbSource.Text);
			else
				ControlProperties.Provider.SaveValue(this.ID, "PageSource", "http://" + tbSource.Text);
			ControlProperties.Provider.SaveValue(this.ID, ControlProperties._titleKey, tbTitle.Text);
			int _result = -1;
			if (int.TryParse(tbHeight.Text, out _result))
			{
				ControlProperties.Provider.SaveValue(this.ID, "BlockHeight", tbHeight.Text);
			}
			else
			{
				ControlProperties.Provider.SaveValue(this.ID, "BlockHeight", -1);
			}
		}

		#endregion
	}
}