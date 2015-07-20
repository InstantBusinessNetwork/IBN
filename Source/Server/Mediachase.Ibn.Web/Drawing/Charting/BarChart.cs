using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;

namespace Mediachase.Ibn.Web.Drawing.Charting
{
	public class BarChart : Chart
	{
		private const int _bufferSpace = 125;
		private const int _seriesLength = 500;
		private Color _backgroundColor;
		private Color _borderColor;
		private int _legendFontHeight;
		private string _legendFontStyle;
		private float _legendFontSize;
		private float _maxLegendWidth;
		private int _titleHeight;
		private int _spacer = 15;
		private int _gistwidth = 15;

		public ChartItemsCollection Items { get; private set; }
		public string Title { get; set; }
		public int BorderWidth { get; set; }
		public int Width { get; set; }

		public int Height { get; set; }
		public bool Sort { get; set; }

		public BarChart()
		{
			Items = new ChartItemsCollection();
			Title = string.Empty;
			BorderWidth = 1;
			Width = 400;

			Height = 200;
			Sort = true;

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
			}

			if (Sort)
				items.Sort(new ChartItemsComparer());

			foreach (ChartItem item in items)
				Items.Add(item);

			if (Items.Count > 0)
				_gistwidth = (int)Math.Round((double)((Width - 2 * _spacer) / Items.Count));

			float nextStartPos = 0;
			if (_gistwidth > 100)
			{
				_gistwidth = 100;
				nextStartPos = (Width - _gistwidth * Items.Count) / 2;
			}
			else
				nextStartPos = (float)_spacer;
			int counter = 0;
			foreach (ChartItem item in Items)
			{
				item.StartPos = nextStartPos;
				nextStartPos = item.StartPos + _gistwidth;
				item.ItemColor = GetColor(counter++);
			}

			Font fontLegend = new Font(_legendFontStyle, _legendFontSize);
			_legendFontHeight = fontLegend.Height + 5;
			CalculateTitleHeight();
			CalculateLegendWidthHeight();
		}

		public override void Draw(System.IO.Stream stream, ImageFormat format)
		{
			using (Bitmap bitmap = new Bitmap(Width,
					  _spacer + _titleHeight + _spacer + Height + 2 * _spacer))
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

			StringFormat sf = null;

			try
			{
				sf = new StringFormat();

				//Paint Back ground
				graphics.FillRectangle(new SolidBrush(_backgroundColor), 0, 0, width + 20,
					spacer + _titleHeight + spacer + Height + 2 * spacer);

				graphics.SmoothingMode = SmoothingMode.AntiAlias;

				//Align text to the right
				sf.Alignment = StringAlignment.Near;

				SizeF titlesize = graphics.MeasureString(Title, new Font(_legendFontStyle, _legendFontSize));

				graphics.DrawString(Title, new Font(_legendFontStyle, _legendFontSize),
					new SolidBrush(Color.Black), (width - titlesize.Width) / 2, spacer);
				float MaxValue = 1;

				MaxValue = 0;
				foreach (ChartItem ci in Items)
					if (ci.Value > MaxValue)
						MaxValue = ci.Value;

				for (int i = 0; i < Items.Count; i++)
				{
					ChartItem item = (ChartItem)Items[i];
					SolidBrush brs = null;
					try
					{
						brs = new SolidBrush(item.ItemColor);
						int _curHeight = (int)Math.Round((Height - _maxLegendWidth) * item.Value / MaxValue);
						if (_curHeight == 0)
							_curHeight = 1;
						string sValue = (Math.Round(item.Value, 2)).ToString(CultureInfo.CurrentUICulture);
						SizeF sValueSize = graphics.MeasureString(sValue, new Font(_legendFontStyle, _legendFontSize));
						SizeF sLabelSize = graphics.MeasureString(item.Label, new Font(_legendFontStyle, _legendFontSize));
						graphics.FillRectangle(brs, item.StartPos, spacer + _titleHeight + Height -
							_maxLegendWidth - _curHeight,
							_gistwidth, _curHeight);

						if (sValueSize.Width <= _gistwidth && sValueSize.Height <= _curHeight)
						{
							graphics.DrawString(sValue,
								new Font(_legendFontStyle, _legendFontSize),
								new SolidBrush(Color.Black),
								item.StartPos + _gistwidth / 2 - sValueSize.Width / 2,
								spacer + _titleHeight + Height - _maxLegendWidth - _curHeight / 2 - sValueSize.Height / 2);
						}
						if (_maxLegendWidth >= _gistwidth)
						{
							System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat(StringFormatFlags.DirectionVertical);
							graphics.DrawString(item.Label, new Font(_legendFontStyle, _legendFontSize), new SolidBrush(Color.Black),
								item.StartPos + _gistwidth / 2 - sLabelSize.Height / 2,
								spacer + _titleHeight + Height - _maxLegendWidth + spacer, drawFormat);
						}
						else
							graphics.DrawString(item.Label, new Font(_legendFontStyle, _legendFontSize), new SolidBrush(Color.Black),
								item.StartPos + _gistwidth / 2 - sLabelSize.Width / 2,
								spacer + _titleHeight + Height + spacer);
					}
					finally
					{
						if (brs != null)
							brs.Dispose();
					}
				}

				//draw border around Chart
				if (BorderWidth > 0)
					graphics.DrawRectangle(new Pen(_borderColor, BorderWidth), 0, 0, width - 1, Height + _titleHeight + 4 * spacer - 1);

				//Draw Total under legend
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
			}
			finally
			{
				if (sf != null) sf.Dispose();
				if (graphics != null) graphics.Dispose();
			}
		}

		private void CalculateTitleHeight()
		{
			Font fontLegend = new Font(_legendFontStyle, _legendFontSize);
			if (!string.IsNullOrEmpty(Title))
				_titleHeight = fontLegend.Height + _spacer;
			else _titleHeight = 0;
		}
		private void CalculateLegendWidthHeight()
		{
			Bitmap bmp = new Bitmap(Width,
				_spacer + _titleHeight + _spacer + Height + _spacer + _legendFontHeight + _spacer);

			Graphics grp = Graphics.FromImage(bmp);
			grp.SmoothingMode = SmoothingMode.AntiAlias;
			foreach (ChartItem item in Items)
			{
				SizeF LabelSize = grp.MeasureString(item.Label, new Font(_legendFontStyle, _legendFontSize));
				if (LabelSize.Width > _maxLegendWidth)
					_maxLegendWidth = (int)LabelSize.Width;
			}
			if (_maxLegendWidth > _gistwidth)
			{
				Height += (int)Math.Round(_maxLegendWidth) - _legendFontHeight;
			}
			else
			{
				_maxLegendWidth = 0;
				Height += _legendFontHeight;
			}
		}
	}
}
