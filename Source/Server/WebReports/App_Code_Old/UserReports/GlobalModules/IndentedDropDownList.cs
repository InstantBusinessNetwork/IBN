using System;
using System.Drawing;

using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;

using System.Globalization;

namespace Mediachase.UI.Web.UserReports.GlobalModules
{
	/// <summary>
	/// Summary description for IndentedDropDownList.
	/// </summary>
	public class IndentedDropDownList:DropDownList
	{
		public IndentedDropDownList()
		{

		}

		protected override void Render(HtmlTextWriter writer)
		{
			RenderBeginTag(writer);
			foreach (ListItem li in this.Items)
			{
				writer.WriteBeginTag("option");
				writer.WriteAttribute("value", li.Value);
				if (li.Selected)
					writer.WriteAttribute("selected", "");
				writer.Write(HtmlTextWriter.TagRightChar);
				writer.Write(li.Text.Replace(" ", "&nbsp;"));
				writer.WriteEndTag("option");
			}
			RenderEndTag(writer);
		}
	}
}
