//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------
using System;
using System.Web.UI.WebControls;
using System.Drawing;

namespace Mediachase.Web.UI.WebControls
{
	/// <summary>
	/// Summary description for CssTableItemStyle.
	/// </summary>
	public class CssTableItemStyle : TableItemStyle
	{
		public CssTableItemStyle() : base()
		{
		}

		public override string ToString()
		{
			string strStyle="";
			if (this.BackColor!=Color.Empty)
				strStyle+="background-color:" + ColorTranslator.ToHtml(this.BackColor)+ ";";
			if (this.BorderColor!=Color.Empty)
				strStyle+="border-color:" + ColorTranslator.ToHtml(this.BorderColor)+ ";";
			if (this.BorderStyle!=BorderStyle.NotSet)
				strStyle+="border-style:" + this.BorderStyle.ToString() + ";";
			if (this.BorderWidth!=Unit.Empty)
				strStyle+="border-width:" + this.BorderWidth.ToString() + ";";
			if (this.Font.Name.Length!=0)
				strStyle+="font-family:" + this.Font.Name.ToString()+ ";";
			if (this.Font.Size!=FontUnit.Empty)
				strStyle+="font-size:" + this.Font.Size.ToString()+ ";";
			if (this.Font.Bold==true)
				strStyle+="font-weight:Bold;";
			if (this.Font.Italic==true)
				strStyle+="font-style:Italic;";
			if (this.Font.Underline==true)
				strStyle+="text-decoration:underline;";
			if (!this.Height.IsEmpty)
				strStyle+="height:" + this.Height.ToString()+ ";";
			if (!this.Width.IsEmpty)
				strStyle+="width:" + this.Width.ToString()+ ";";
			return strStyle;
		}

	}
}
