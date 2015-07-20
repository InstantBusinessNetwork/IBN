using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;

namespace Mediachase.Ibn.Web.Drawing.Charting
{
	public class PieChart : Chart
	{
		private const int _bufferSpace = 125;
		private const int _seriesLength = 500;
		private Color _backgroundColor;
		private Color _borderColor;
		private float _total;
		private int _legendWidth;
		private int _legendHeight;
		private float _maxLegendWidth;
		private int _legendFontHeight;
		private string _legendFontStyle;
		private int _titleHeight;
		private int _spacer = 15;
		private float _legendFontSize;
		private float _const = 1.0f * 2 / 3;

		public ChartItemsCollection Items { get; private set; }
		public string Title { get; set; }
		public int BorderWidth { get; set; }
		public int Width { get; set; }

		public int Radius { get; set; }
		public bool DrawPercents { get; set; }
		public float MinPercentToDraw { get; set; }

		public PieChart()
		{
			Items = new ChartItemsCollection();
			Title = string.Empty;
			BorderWidth = 1;

			Radius = 160;
			Width = 200;

			_backgroundColor = Color.White;
			_borderColor = Color.Gainsboro;
			_legendFontSize = 8;
			_legendFontStyle = "Tahoma";
		}

		public void CollectDataPoints(string[] keys, double[] values)
		{
			List<ChartItem> items = new List<ChartItem>();
			for (int i = 0; i < keys.Length; i++)
			{
				float ftemp = Convert.ToSingle(values[i]);
				items.Add(new ChartItem(keys[i], keys.ToString(), ftemp, 0, 0, Color.AliceBlue));
				_total += ftemp;
			}
			items.Sort(new ChartItemsComparer());

			foreach (ChartItem item in items)
				Items.Add(item);

			float nextStartPos = 0.0f;
			int counter = 0;
			foreach (ChartItem item in Items)
			{
				item.StartPos = nextStartPos;
				item.SweepSize = item.Value / _total * 360;
				nextStartPos = item.StartPos + item.SweepSize;
				item.ItemColor = GetColor(counter++);
			}

			CalculateLegendWidthHeight();
			CalculateTitleHeight();
		}



		//*********************************************************************
		//
		// This method returns a bitmap to the calling function.  This is the method
		// that actually draws the pie chart and the legend with it.
		//
		//*********************************************************************

		public override void Draw(System.IO.Stream stream, ImageFormat format)
		{
			using (Bitmap bitmap = new Bitmap(Width + _legendWidth + _spacer,
					  _spacer + _titleHeight + _spacer + Radius + _spacer))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					Draw(graphics);
				}
				bitmap.Save(stream, format);
			}
		}

		public override void Draw(Graphics graphics)
		{
			int width = Width;
			int spacer = _spacer;

			Rectangle pieRect = new Rectangle(
				(width - Radius) / 2, spacer + _titleHeight,
				Radius, Radius);

			StringFormat sf = null;

			try
			{
				sf = new StringFormat();

				//Paint Back ground
				graphics.FillRectangle(new SolidBrush(_backgroundColor), 0, 0, width + _legendWidth + _spacer,
					spacer + _titleHeight + spacer + Radius + spacer);
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				//Align text to the right
				sf.Alignment = StringAlignment.Near;

				SizeF titlesize = graphics.MeasureString(Title, new Font(_legendFontStyle, _legendFontSize));

				graphics.DrawString(Title, new Font(_legendFontStyle, _legendFontSize),
					new SolidBrush(Color.Black), (width + _legendWidth + _spacer - titlesize.Width) / 2, spacer);

				//Draw all wedges and legends

				for (int i = 0; i < Items.Count; i++)
				{
					ChartItem item = (ChartItem)Items[i];
					SolidBrush brs = null;
					try
					{
						brs = new SolidBrush(item.ItemColor);
						graphics.FillPie(brs, pieRect, item.StartPos, item.SweepSize);
						float fPerc = (float)100 * item.Value / _total;
						if (DrawPercents && fPerc >= MinPercentToDraw)
						{
							string sPerc = (Math.Round(fPerc, 2)).ToString(CultureInfo.CurrentUICulture) + "%";
							SizeF PercSize = graphics.MeasureString(sPerc, new Font(_legendFontStyle, _legendFontSize));
							double _angle = Math.PI * (item.StartPos + item.SweepSize / 2) / 180;
							double fcos = _const * Radius * Math.Cos(_angle) / 2;
							double fsin = _const * Radius * Math.Sin(_angle) / 2;
							float fx = (float)(width / 2 - PercSize.Width / 2 + fcos);
							float fy = (float)(spacer + _titleHeight + Radius / 2 + fsin - PercSize.Height / 2);
							graphics.DrawString(sPerc, new Font(_legendFontStyle, _legendFontSize),
								new SolidBrush(Color.Black), fx, fy);
						}
						graphics.FillRectangle(brs, width + (_legendWidth - _maxLegendWidth) / 2,
							_titleHeight + i * _legendFontHeight + spacer * 2, 10, 10);
						graphics.DrawString(item.Label, new Font(_legendFontStyle, _legendFontSize), new SolidBrush(Color.Black),
							width + (_legendWidth - _maxLegendWidth) / 2 + 15, _titleHeight + i * _legendFontHeight + spacer * 2);

					}
					finally
					{
						if (brs != null)
							brs.Dispose();
					}
				}

				//draws the border around Pie
				graphics.DrawEllipse(new Pen(_borderColor, 1), pieRect);

				//draw border around Chart
				if (BorderWidth > 0)
					graphics.DrawRectangle(new Pen(_borderColor, BorderWidth), 0, 0, width + _legendWidth + _spacer - 1, Radius + _titleHeight + 3 * spacer - 1);

				//Draw Total under legend
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
			}
			finally
			{
				if (sf != null) sf.Dispose();
				if (graphics != null) graphics.Dispose();
			}
		}

		//*********************************************************************
		//
		//	This method calculates the space required to draw the chart legend.
		//
		//*********************************************************************

		private void CalculateLegendWidthHeight()
		{
			Font fontLegend = new Font(_legendFontStyle, _legendFontSize);
			_legendFontHeight = fontLegend.Height + 5;
			_legendHeight = _legendFontHeight * (Items.Count);
			if (_legendHeight > Radius)
				Radius = _legendHeight;
			Width = 2 * _spacer + Radius;

			_legendWidth = _bufferSpace;


			Bitmap bmp = new Bitmap(Width + _legendWidth + _spacer,
			   _spacer + _titleHeight + _spacer + Radius + _spacer);

			Graphics grp = Graphics.FromImage(bmp);
			grp.SmoothingMode = SmoothingMode.AntiAlias;

			for (int i = 0; i < Items.Count; i++)
			{
				ChartItem item = (ChartItem)Items[i];
				SizeF LabelSize = grp.MeasureString(item.Label, new Font(_legendFontStyle, _legendFontSize));
				if (LabelSize.Width > _maxLegendWidth)
					_maxLegendWidth = LabelSize.Width;
			}
			_maxLegendWidth += 15;
			if (_maxLegendWidth > _legendWidth)
			{
				_legendWidth = (int)Math.Round(_maxLegendWidth);
			}

		}

		private void CalculateTitleHeight()
		{
			Font fontLegend = new Font(_legendFontStyle, _legendFontSize);
			if (!string.IsNullOrEmpty(Title))
				_titleHeight = fontLegend.Height + _spacer;
			else _titleHeight = 0;
		}
	}
}
