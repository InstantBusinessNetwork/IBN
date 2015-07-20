//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------
using System;
using System.Drawing;
using Mediachase.Web.UI.WebControls;

namespace Mediachase.Web.UI.WebControls.Util
{
	/// <summary>
	/// Summary description for CalendarColors.
	/// </summary>
	public class CalendarColors
	{

		//<CalendarDefaultStyle BackColor="Black"></CalendarDefaultStyle>
		//<CalendarHeaderStyle ForeColor="White" BackColor="#31639C"></CalendarHeaderStyle>
		//<CalendarItemDefaultStyle ForeColor="Black" BackColor="White"></CalendarItemDefaultStyle>
		//<CalendarItemSelectedStyle BackColor="#FFFFCC"></CalendarItemSelectedStyle>
		//<CalendarItemInactiveStyle BackColor="#FFFFCC"></CalendarItemInactiveStyle>
		//<CalendarItemHoverStyle BackColor="Red"></CalendarItemHoverStyle>
		

		// Colors
		//
		private static Color[] colorsDefault = new Color[]
		{
			Color.Black, // Default global
			ColorTranslator.FromHtml("#31639C"), Color.White, // header
			ColorTranslator.FromHtml("#31639C"), Color.White, // subheader/allday
			Color.White, Color.Black, // item default
			ColorTranslator.FromHtml("#FFFFCC"), Color.Black, // item selected
			ColorTranslator.FromHtml("#EBF0F0"), Color.Black, // item inactive
			Color.White, Color.Black, // item hover
			ColorTranslator.FromHtml("#31639C"), Color.White, // EventBarFilledColor, EventBarEmptyColor
			Color.Black, ColorTranslator.FromHtml("#cccccc") // borders
		};

		private static Color[] colorsPrinter = new Color[]
		{
			Color.Black, 
			Color.White, Color.Black,
			Color.White, Color.Black,
			Color.White, Color.Black,
			Color.White, Color.Black,
			Color.White, Color.Black,
			Color.White, Color.Black,
			Color.Black, Color.White,
			Color.Black, ColorTranslator.FromHtml("#cccccc") // borders
		};

		private static Color[] colorsGrey = new Color[]
		{
			ColorTranslator.FromHtml("#7f7f7f"), // Default global
			ColorTranslator.FromHtml("#8f8f8f"), Color.White, // header
			ColorTranslator.FromHtml("#8f8f8f"), Color.White, // subheader/allday
			ColorTranslator.FromHtml("#dfdfdf"), Color.Black, // item default
			ColorTranslator.FromHtml("#efefef"), Color.Black, // item selected
			ColorTranslator.FromHtml("#cfcfcf"), Color.Black, // item inactive
			ColorTranslator.FromHtml("#f4f4f4"), Color.Black, // item hover
			ColorTranslator.FromHtml("#8f8f8f"), Color.White,
			Color.Black, ColorTranslator.FromHtml("#cccccc") // borders
		};

		private static Color[] colorsBrown = new Color[]
		{
			ColorTranslator.FromHtml("#812C00"), // Default global
			ColorTranslator.FromHtml("#8C3000"), Color.White, // header
			ColorTranslator.FromHtml("#8C3000"), Color.White, // subheader/allday
			ColorTranslator.FromHtml("#692400"), Color.White, // item default
			ColorTranslator.FromHtml("#993300"), Color.White, // item selected
			ColorTranslator.FromHtml("#5C2003"), Color.White, // item inactive
			ColorTranslator.FromHtml("#993300"), Color.White, // item hover
			Color.Blue, Color.White,
			Color.Black, ColorTranslator.FromHtml("#cccccc") // borders
		};

		private static Color[] colorsGreen = new Color[]
		{
			ColorTranslator.FromHtml("#008600"), // Default global
			ColorTranslator.FromHtml("#008600"), Color.White, // header
			ColorTranslator.FromHtml("#008600"), Color.White, // subheader/allday
			Color.White, Color.Black, // item default
			ColorTranslator.FromHtml("#DFF2DF"), Color.Black, // item selected
			ColorTranslator.FromHtml("#DCFADC"), Color.Black, // item inactive
			ColorTranslator.FromHtml("#E6F7E6"), Color.Black, // item hover
			ColorTranslator.FromHtml("#008600"), Color.White,
			Color.Black, ColorTranslator.FromHtml("#cccccc") // borders
		};

		private static Color[] colorsRed = new Color[]
		{
			ColorTranslator.FromHtml("#D20000"), // Default global
			ColorTranslator.FromHtml("#FE6363"), Color.White, // header
			ColorTranslator.FromHtml("#FE6363"), Color.White, // subheader/allday
			ColorTranslator.FromHtml("#FEA8A8"), Color.Black, // item default
			ColorTranslator.FromHtml("#FAB1B1"), Color.Black, // item selected
			ColorTranslator.FromHtml("#F79999"), Color.Black, // item inactive
			ColorTranslator.FromHtml("#F0AAAA"), Color.Black, // item hover
			ColorTranslator.FromHtml("#FE6363"), Color.White,
			Color.Black, ColorTranslator.FromHtml("#cccccc") // borders
		};

		private static Color[] colorsWindows = new Color[]
		{
			Color.Black, // Default global
			ColorTranslator.FromHtml("#B7B39F"), Color.White, // header
			ColorTranslator.FromHtml("#B7B39F"), Color.White, // subheader/allday
			Color.White, Color.Black, // item default
			ColorTranslator.FromHtml("#FAC56C"), Color.Black, // item selected
			ColorTranslator.FromHtml("#E7E6D8"), Color.Black, // item inactive
			ColorTranslator.FromHtml("#FAC56C"), Color.Black, // item hover
			Color.Blue, Color.White,
			Color.Black, ColorTranslator.FromHtml("#cccccc") // borders
		};

		private static Color[] colorsOffice = new Color[]
		{
			Color.Black, // Default global
			ColorTranslator.FromHtml("#ECE9D8"), Color.Black, // header
			ColorTranslator.FromHtml("#ACA899"), Color.White, // subheader/allday
			ColorTranslator.FromHtml("#FFFFD5"), Color.Black, // item default
			ColorTranslator.FromHtml("khaki"), Color.White, // item selected
			ColorTranslator.FromHtml("#FFF4BC"), Color.Black, // item inactive
			ColorTranslator.FromHtml("#FAC56C"), Color.Black, // item hover
			Color.Blue, Color.White,
			ColorTranslator.FromHtml("#EAD098"), ColorTranslator.FromHtml("#F3E4B1") // borders
		};

		/// <summary>
		/// 
		/// </summary>
		public CalendarColors()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="palette"></param>
		/// <returns></returns>
		public static System.Drawing.Color[] GetPaletteColors(CalendarColorPalette palette) 
		{
			CalendarColorPalette local0;

			local0 = palette;
			switch (local0) 
			{
				case CalendarColorPalette.Default:
					return CalendarColors.colorsDefault;
				case CalendarColorPalette.Windows:
					return CalendarColors.colorsWindows;
				case CalendarColorPalette.Office:
					return CalendarColors.colorsOffice;
				case CalendarColorPalette.Grey:
					return CalendarColors.colorsGrey;
				case CalendarColorPalette.Green:
					return CalendarColors.colorsGreen;
				case CalendarColorPalette.Red:
					return CalendarColors.colorsRed;
				case CalendarColorPalette.PrinterFriendly:
					return CalendarColors.colorsPrinter;
			}
			return CalendarColors.colorsPrinter;
		}
	}
}
