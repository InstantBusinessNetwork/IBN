using System;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.ComponentModel;
using System.Data;

namespace Mediachase.Ibn.WebAsp
{
	/// <summary>
	/// Common Page for all other. Provides unified error handling
	/// </summary>
	public class PageBase : Page
	{
		/// <summary>
		/// Html Span for displaying error messages
		/// </summary>
		protected HtmlGenericControl ErrorSpan;

		/// <summary>
		/// Html Span for displaying information messages
		/// </summary>		
		protected HtmlGenericControl InfoSpan;


		///   <summary>
		///      Cause specified text to be displayed as an error
		///      message on the page
		///   </summary>
   		protected string ErrorMessage
		{
			get 
			{
				return ErrorSpan.InnerHtml;
			}
			set 
			{
				StringBuilder stringBuilder = new StringBuilder( value );
				stringBuilder.Replace( "\r\n", "<br>" );
				ErrorSpan.InnerHtml = stringBuilder.ToString();
			}
		}

		public PageBase()
		{
		}

		protected override void OnError(EventArgs e)
		{
			this.ErrorMessage = e.ToString();
		}
	}
}
