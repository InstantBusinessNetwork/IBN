//====================================================================
// This file is generated as part of Web project conversion.
// The extra class 'TabItem' in the code behind file in 'Modules\TopTabs.ascx.cs' is moved to this file.
//====================================================================




namespace Mediachase.Ibn.WebAsp
 {

	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;

	public class TabItem {
		public string text;
		public string link;
		public bool selected;
		public string key;

		public TabItem(string _text, string _link, bool _selected)
		{
			text = _text;
			link = _link;
			selected = _selected;
			key = "";
		}

		public TabItem(string _text, string _link, bool _selected,string _key)
		{
			text = _text;
			link = _link;
			selected = _selected;
			key = _key;
		}
	}

}