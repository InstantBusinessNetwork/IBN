//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------

namespace Mediachase.Web.UI.WebControls.Design
{
    using System;
	using System.Drawing;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Windows.Forms.Design;
    using System.Web.UI.Design;
    using Mediachase.Web.UI.WebControls;
	using Mediachase.Web.UI.WebControls.Util;

    /// <summary>
    /// Provides an editor for visually picking an URL.
    /// </summary>
	public class CalendarColorsEditor : UITypeEditor 
	{

		/// <summary>
		/// Default Constructor
		/// </summary>
		public CalendarColorsEditor() : base() 
		{
		}

		/// <summary>
		/// Internam method
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) 
		{
			return true;
		}

		/*
		public override void PaintValue(PaintValueEventArgs e) 
		{
			CalendarColorPalette paletteEnum;
			System.Drawing.Color[] colorArray;
			int colorArrayLength;
			int times;
			RectangleF rectangleFloat;
			int index;
			Rectangle rectangle;

			paletteEnum = (CalendarColorPalette) e.Value;
			if (paletteEnum == 0) 
			{
				this.PaintValue(e);
				return;
			}
			colorArray = CalendarColors.GetPaletteColors(paletteEnum);
			colorArrayLength = (int) colorArray.Length;
			if (colorArrayLength > 6)
				colorArrayLength = 6;
			times = (int) colorArray.Length / colorArrayLength;
			rectangleFloat = (RectangleF)e.Bounds;
			rectangle = e.Bounds;
			rectangleFloat.Width = (float) rectangle.Width / (float) colorArrayLength;
			index = 0;
			while (index < colorArrayLength) 
			{
				if (index == colorArrayLength - 1) 
				{
					rectangle = e.Bounds;
					rectangleFloat.Width = (float) rectangle.Right - rectangleFloat.X;
				}
				e.Graphics.FillRectangle(new SolidBrush(colorArray[index * times]), rectangleFloat);
				rectangleFloat.X = rectangleFloat.Right;
				rectangle = e.Bounds;
				rectangleFloat.Width = (float) rectangle.Width / (float) colorArrayLength;
				index++;
			}
		}
		*/

		/// <summary>
		/// Paints structure
		/// </summary>
		/// <param name="e"></param>
		public override void PaintValue(PaintValueEventArgs e)
		{
			CalendarColorPalette palette1;
			Color[] array1;
			int num1;
			int num2;
			RectangleF ef1;
			int num3;
			Rectangle rectangle1;
			palette1 = (CalendarColorPalette)e.Value;
			if (palette1 == 0)
			{
				base.PaintValue(e);
				return; 
			}
			array1 = CalendarColors.GetPaletteColors(palette1);
			num1 = array1.Length;
			if (num1 > 6)
			{
				num1 = 6;
 
			}
			num2 = (array1.Length / num1);
			// Overwrite and always display first 6 colors
			num2 = 1;
			ef1 = (RectangleF)(e.Bounds);
			rectangle1 = e.Bounds;
			ef1.Width = (((float) rectangle1.Width) / ((float) num1));
			for (num3 = 0; (num3 < num1); num3++)
			{
				if (num3 == (num1 - 1))
				{
					rectangle1 = e.Bounds;
					ef1.Width = (((float) rectangle1.Right) - ef1.X);
 
				}
				e.Graphics.FillRectangle(new SolidBrush(array1[(num3 * num2)]), ef1);
				ef1.X = ef1.Right;
				rectangle1 = e.Bounds;
				ef1.Width = (((float) rectangle1.Width) / ((float) num1));
			}
		}
	}
}
