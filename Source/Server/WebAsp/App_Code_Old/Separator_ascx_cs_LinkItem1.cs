//====================================================================
// This file is generated as part of Web project conversion.
// The extra class 'LinkItem1' in the code behind file in 'Modules\Separator.ascx.cs' is moved to this file.
//====================================================================




namespace Mediachase.Ibn.WebAsp
 {

	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;

	public class LinkItem1:IGetHTML
	{
		public string _text;
		public string _url;

		public LinkItem1(string text,string url)
		{
			_text = text;
			_url = url;
		}

		#region Implementation of IGetHTML
		public string GetHTML()
		{
			return String.Format("<a href='{0}'>{1}</a>",_url,_text);
		}
		#endregion
	}

}