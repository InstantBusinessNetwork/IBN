using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;

namespace Mediachase.Ibn.Web.Drawing.Charting
{
	public class GraphicImage
	{
		private double[] _chartItemsL;
		private double[] _chartItemsR;
		private double[] _chartItems1;
		private double[] _chartItems2;
		private string _chartItemsLName;
		private string _chartItemsRName;
		private string _chartItems1Name;
		private string _chartItems2Name;
		private string[] _chartLabelsX;
		private int _width;
		private int _height;
		private int _widthChart;
		private Color _backgroundColor;
		private Color _borderColor;
		private int _legendFontWidth;
		private int _legendFontHeight;
		private string _legendFontStyle;
		private float _legendFontSize;
		private int _spacer;
		private int _x0;
		private int _y0;
		private int _graph_width;
		private int _graph_height;
		private int _legend_height;
		private int _x_range;
		private int _y_range;
		private int _range_width;
		private int _deltaHeight;

		#region Public Properties
		public int BorderWidth { get; set; }
		public string[] GetChartLabelsX()
		{
			return _chartLabelsX;
		}

		public int ImageHeight
		{
			get { return _height; }
			set { _height = value; }
		}

		public int ImageWidth
		{
			get { return _width; }
			set { _width = value; }
		}

		public int ChartItemWidth
		{
			get { return _widthChart; }
			set { _widthChart = value; }
		}

		public string ChartItemsLName
		{
			get { return _chartItemsLName; }
			set { _chartItemsLName = value; }
		}
		public string ChartItemsRName
		{
			get { return _chartItemsRName; }
			set { _chartItemsRName = value; }
		}
		public string ChartItems1Name
		{
			get { return _chartItems1Name; }
			set { _chartItems1Name = value; }
		}
		public string ChartItems2Name
		{
			get { return _chartItems2Name; }
			set { _chartItems2Name = value; }
		}
		#endregion

		#region .ctor
		public GraphicImage()
		{
			_width = 550;
			_height = 420;
			_backgroundColor = Color.White;
			_borderColor = Color.Black;
			_legendFontSize = 8;
			_legendFontStyle = "Tahoma";
			_x_range = 24;
			_y_range = 5;
			_range_width = 5;
			_spacer = 15;
			_widthChart = 7;
			_legend_height = 30;
		}
		#endregion

		#region public void CollectDataPoints(string[] keys, double[] valuesLeft, double[] valuesRight)
		public void CollectDataPoints(string[] keys, double[] valuesLeft, double[] valuesRight)
		{
			CollectDataPoints(keys, valuesLeft, valuesRight, new double[0], new double[0]);
			GetDeltaHeight();
		}
		#endregion
		#region public void CollectDataPoints(string[] keys, double[] valuesLeft, double[] valuesRight, double[] values1, double[] values2)
		public void CollectDataPoints(string[] keys, double[] valuesLeft, double[] valuesRight, double[] values1, double[] values2)
		{
			_x_range = keys.Length - 1;
			_chartLabelsX = keys;
			_chartItemsL = valuesLeft;
			_chartItemsR = valuesRight;
			_chartItems1 = values1;
			_chartItems2 = values2;
		}
		#endregion

		#region GetDeltaHeight()
		private void GetDeltaHeight()
		{
			Bitmap bmp = new Bitmap(this.ImageWidth, this.ImageHeight);

			Graphics grp = Graphics.FromImage(bmp);
			grp.SmoothingMode = SmoothingMode.AntiAlias;
			float titleWidth = 0;
			float titleHeight = 0;
			foreach (string s in _chartLabelsX)
			{
				SizeF LabelSize = grp.MeasureString(s, new Font(_legendFontStyle, _legendFontSize));
				titleHeight = (LabelSize.Height > titleHeight) ? LabelSize.Height : titleHeight;
				titleWidth = (LabelSize.Width > titleWidth) ? LabelSize.Width : titleWidth;
			}
			float deltaHeight = titleWidth - titleHeight + 5;

			int t_x0 = _spacer + _legendFontWidth;
			int t_graph_width = _width - t_x0 - _spacer;
			int t_widthChart = _widthChart;
			if (!(_widthChart > 0))
				t_widthChart = (int)(t_graph_width / _x_range * 0.4);
			t_x0 += t_widthChart;
			t_graph_width -= 2 * t_widthChart;
			int t_label_pos_width = (int)((t_graph_width - 30) / _x_range);

			if (deltaHeight > 0 && deltaHeight > t_label_pos_width)
			{
				_deltaHeight = (int)Math.Round(deltaHeight);
			}
		}
		#endregion

		#region public void Render(Graphics graphics)
		public void Render(Graphics graphics)
		{
			if (graphics == null)
				throw new ArgumentNullException("graphics");

			StringFormat sf = null;

			sf = new StringFormat();

			//Paint Back ground
			graphics.FillRectangle(new SolidBrush(_backgroundColor), 0, 0, _width, _height + _deltaHeight);

			graphics.SmoothingMode = SmoothingMode.AntiAlias;


			//draw border around Chart
			if (BorderWidth > 0)
				graphics.DrawRectangle(new Pen(_borderColor, BorderWidth), 0, 0, _width - 1, _height - 1);

			Pen PenBorder = new Pen(_borderColor, BorderWidth);

			Pen PenDash = new Pen(Color.LightBlue, BorderWidth);
			PenDash.DashStyle = DashStyle.Custom;
			PenDash.DashPattern = new float[] { 4, 2 };
			PenDash.Width = 1;


			double MaxValue = 0;
			for (int i = 0; i < _chartLabelsX.Length; i++)
			{
				if (i < _chartItemsL.Length)
					if (_chartItemsL[i] > MaxValue)
						MaxValue = _chartItemsL[i];
				if (i < _chartItemsR.Length)
					if (_chartItemsR[i] > MaxValue)
						MaxValue = _chartItemsR[i];
				if (i < _chartItems1.Length)
					if (_chartItems1[i] > MaxValue)
						MaxValue = _chartItems1[i];
				if (i < _chartItems2.Length)
					if (_chartItems2[i] > MaxValue)
						MaxValue = _chartItems2[i];
			}

			int MaxRoundValue = 0;
			int NDigit = 1;
			while (MaxValue / Math.Pow(10, NDigit) > 1) NDigit++;
			int firstDigit = (int)(MaxValue / Math.Pow(10, NDigit - 1));
			if (firstDigit > 5)
			{
				MaxRoundValue = (firstDigit + 1) * (int)Math.Pow(10, NDigit - 1);
				_y_range = 10;
			}
			else
			{
				MaxRoundValue = (firstDigit + 1) * (int)Math.Pow(10, NDigit - 1);
				_y_range = (firstDigit + 1) * 2;
			}

			_legendFontWidth = (int)graphics.MeasureString(MaxRoundValue.ToString(CultureInfo.CurrentUICulture), new Font(_legendFontStyle, _legendFontSize)).Width;
			_legendFontHeight = (int)graphics.MeasureString("0", new Font(_legendFontStyle, _legendFontSize)).Height;

			_x0 = _spacer + _legendFontWidth;
			_y0 = _spacer;
			_graph_height = _height - _y0 - _spacer - _legendFontHeight - (_legend_height + _spacer);
			_graph_width = _width - _x0 - _spacer;
			if (!(_widthChart > 0))
				_widthChart = (int)(_graph_width / _x_range * 0.4);
			_x0 += _widthChart;
			_graph_width -= 2 * _widthChart;

			graphics.DrawLine(PenBorder, new Point(_x0, _y0), new Point(_x0, _y0 + _graph_height));
			graphics.DrawLine(PenBorder, new Point(_x0, _y0 + _graph_height), new Point(_x0 + _graph_width, _y0 + _graph_height));

			for (int i = 1; i <= _x_range; i++)
				graphics.DrawLine(PenDash, new Point(_x0 + (int)(i * _graph_width / _x_range), _y0), new Point(_x0 + (int)(i * _graph_width / _x_range), _y0 + _graph_height));
			for (int i = 0; i < _y_range; i++)
				graphics.DrawLine(PenDash, new Point(_x0, _y0 + (int)(i * _graph_height / _y_range)), new Point(_x0 + _graph_width, _y0 + (int)(i * _graph_height / _y_range)));

			for (int i = 0; i <= _x_range; i++)
			{
				SizeF sValueSize = graphics.MeasureString(_chartLabelsX[i], new Font(_legendFontStyle, _legendFontSize));
				if (_x_range != 0)
				{
					graphics.DrawLine(PenBorder, new Point(_x0 + (int)(i * _graph_width / _x_range), _y0 + _graph_height), new Point(_x0 + (int)(i * _graph_width / _x_range), _y0 + _graph_height + _range_width));
					System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
					float t_delta = sValueSize.Width / 2;
					if (_deltaHeight > 0)
					{
						drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
						t_delta = _widthChart;
					}
					graphics.DrawString(_chartLabelsX[i],
						new Font(_legendFontStyle, _legendFontSize),
						new SolidBrush(Color.Black),
						_x0 + (int)(i * _graph_width / _x_range - t_delta),
						_y0 + _graph_height + _range_width + (int)(sValueSize.Height / 4), drawFormat);
				}
				else
				{
					graphics.DrawLine(PenBorder, new Point(_x0, _y0 + _graph_height), new Point(_x0, _y0 + _graph_height + _range_width));
					graphics.DrawString(_chartLabelsX[0],
						new Font(_legendFontStyle, _legendFontSize),
						new SolidBrush(Color.Black),
						_x0 - (int)(sValueSize.Width / 2),
						_y0 + _graph_height + _range_width + (int)(sValueSize.Height / 4));
				}
			}
			for (int i = 0; i <= _y_range; i++)
			{
				double dValue = i * (double)MaxRoundValue / _y_range;
				string valueFormat = MaxRoundValue > 10 ? "0;0;0" : "0.0;0;0";
				string sValue = dValue.ToString(valueFormat, CultureInfo.CurrentUICulture);
				SizeF sValueSize = graphics.MeasureString(sValue, new Font(_legendFontStyle, _legendFontSize));
				graphics.DrawLine(PenBorder, new Point(_x0, _y0 + i * _graph_height / _y_range), new Point(_x0 - _range_width, _y0 + (int)(i * _graph_height / _y_range)));
				graphics.DrawString(sValue,
					new Font(_legendFontStyle, _legendFontSize),
					new SolidBrush(Color.Black),
					_x0 - sValueSize.Width - 7,
					_y0 + _graph_height - (int)(i * _graph_height / _y_range + _legendFontHeight / 2));
			}

			//legend
			int lx0 = _x0;
			int ly0 = _height - _spacer - _legend_height + _deltaHeight;
			//grp.DrawRectangle(new Pen(_borderColor, _borderWidth), lx0 , ly0, _graph_width , _legend_height);

			SolidBrush BrFillRectL = new SolidBrush(Color.PowderBlue);
			SolidBrush BrFillRectR = new SolidBrush(Color.DodgerBlue);
			Pen PenRed = new Pen(Color.ForestGreen, 3);
			Pen PenBlue = new Pen(Color.LimeGreen, 3);

			//Align text to the right
			sf.Alignment = StringAlignment.Near;

			SizeF titlesizeL = graphics.MeasureString(this.ChartItemsLName, new Font(_legendFontStyle, _legendFontSize));
			graphics.FillRectangle(BrFillRectL, lx0, ly0 + (int)(titlesizeL.Height / 4), _widthChart, 7);
			graphics.DrawRectangle(PenBorder, lx0, ly0 + (int)(titlesizeL.Height / 4), _widthChart, 7);

			graphics.DrawString(this.ChartItemsLName, new Font(_legendFontStyle, _legendFontSize), new SolidBrush(Color.Black),
				lx0 + _widthChart + (int)(_spacer / 2), ly0);

			SizeF titlesizeR = graphics.MeasureString(this.ChartItemsLName, new Font(_legendFontStyle, _legendFontSize));
			graphics.FillRectangle(BrFillRectR, lx0, ly0 + (int)(titlesizeR.Height / 4) + _spacer, _widthChart, 7);
			graphics.DrawRectangle(PenBorder, lx0, ly0 + (int)(titlesizeR.Height / 4) + _spacer, _widthChart, 7);

			graphics.DrawString(this.ChartItemsRName, new Font(_legendFontStyle, _legendFontSize), new SolidBrush(Color.Black),
				lx0 + _widthChart + (int)(_spacer / 2), ly0 + _spacer);

			int new_lx0 = (int)titlesizeR.Width;
			if (new_lx0 < titlesizeL.Width) new_lx0 = (int)titlesizeL.Width;
			new_lx0 += lx0 + _widthChart + 3 * _spacer;

			if (_chartItems1.Length > 0 && _chartItems2.Length > 0)
			{
				graphics.DrawLine(PenRed, new_lx0, ly0 + (int)(titlesizeR.Height / 2), new_lx0 + 2 * _spacer, ly0 + (int)(titlesizeR.Height / 2));
				graphics.DrawString(this.ChartItems2Name, new Font(_legendFontStyle, _legendFontSize), new SolidBrush(Color.Black),
					new_lx0 + _widthChart + 2 * _spacer, ly0);

				graphics.DrawLine(PenBlue, new_lx0, ly0 + (int)(titlesizeR.Height / 2) + _spacer, new_lx0 + 2 * _spacer, ly0 + (int)(titlesizeR.Height / 2) + _spacer);
				graphics.DrawString(this.ChartItems1Name, new Font(_legendFontStyle, _legendFontSize), new SolidBrush(Color.Black),
					new_lx0 + _widthChart + 2 * _spacer, ly0 + _spacer);
			}

			//gistorgam

			for (int i = 0; i < _chartLabelsX.Length; i++)
			{
				if (i < _chartItemsL.Length)
				{
					int GraphValue = (int)(_graph_height * _chartItemsL[i] / MaxRoundValue);
					if (_x_range > 0)
					{
						graphics.FillRectangle(BrFillRectL, _x0 - _widthChart + (int)(i * _graph_width / _x_range), _y0 + _graph_height - GraphValue, _widthChart, GraphValue);
						graphics.DrawRectangle(PenBorder, _x0 - _widthChart + (int)(i * _graph_width / _x_range), _y0 + _graph_height - GraphValue, _widthChart, GraphValue);
					}
					else
					{
						graphics.FillRectangle(BrFillRectL, _x0 - _widthChart, _y0 + _graph_height - GraphValue, _widthChart, GraphValue);
						graphics.DrawRectangle(PenBorder, _x0 - _widthChart, _y0 + _graph_height - GraphValue, _widthChart, GraphValue);
					}
				}
			}

			for (int i = 0; i < _chartLabelsX.Length; i++)
			{
				if (i < _chartItemsR.Length)
				{
					int GraphValue = (int)(_graph_height * _chartItemsR[i] / MaxRoundValue);
					if (_x_range > 0)
					{
						graphics.FillRectangle(BrFillRectR, _x0 + (int)(i * _graph_width / _x_range), _y0 + _graph_height - GraphValue, _widthChart, GraphValue);
						graphics.DrawRectangle(PenBorder, _x0 + (int)(i * _graph_width / _x_range), _y0 + _graph_height - GraphValue, _widthChart, GraphValue);
					}
					else
					{
						graphics.FillRectangle(BrFillRectR, _x0, _y0 + _graph_height - GraphValue, _widthChart, GraphValue);
						graphics.DrawRectangle(PenBorder, _x0, _y0 + _graph_height - GraphValue, _widthChart, GraphValue);
					}
				}
			}

			if (_chartItems1.Length > 0 && _chartItems2.Length > 0)
			{
				GraphicsPath graphPath1 = new GraphicsPath();
				int first_y = -1;
				int last_y = 0;
				int last_x = 0;

				for (int i = 1; i < _chartLabelsX.Length; i++)
				{
					int y1 = (int)(_graph_height * _chartItems1[i - 1] / MaxRoundValue);
					int y2 = (int)(_graph_height * _chartItems1[i] / MaxRoundValue);
					if (_x_range > 0)
					{
						if (i < _chartItems1.Length)
							graphPath1.AddLine(_x0 + (int)((i - 1) * _graph_width / _x_range), _y0 + _graph_height - y1,
								_x0 + (int)(i * _graph_width / _x_range), _y0 + _graph_height - y2);
						if (first_y == -1) first_y = _y0 + _graph_height - y1;
						last_x = _x0 + (int)(i * _graph_width / _x_range);
						last_y = _y0 + _graph_height - y2;
					}
					else
					{
						if (i < _chartItems1.Length)
							graphPath1.AddLine(_x0, _y0 + _graph_height - y1,
								_x0 + _spacer, _y0 + _graph_height - y2);
						if (first_y == -1) first_y = _y0 + _graph_height - y1;
						last_x = _x0 + _spacer;
						last_y = _y0 + _graph_height - y2;
					}
				}

				//grp.DrawPath(PenRed,graphPath);
				GraphicsPath graphPathReg1 = new GraphicsPath();
				graphPathReg1.AddPath(graphPath1, false);
				graphPathReg1.AddLine(last_x, last_y, last_x, _y0 + _graph_height);
				graphPathReg1.AddLine(_x0, _y0 + _graph_height, _x0, first_y);
				Region r1 = new Region(graphPathReg1);

				GraphicsPath graphPath2 = new GraphicsPath();
				first_y = -1;
				last_y = 0;
				last_x = 0;

				for (int i = 1; i < _chartLabelsX.Length; i++)
				{
					int y1 = (int)(_graph_height * _chartItems2[i - 1] / MaxRoundValue);
					int y2 = (int)(_graph_height * _chartItems2[i] / MaxRoundValue);
					if (_x_range > 0)
					{
						if (i < _chartItems2.Length)
							graphPath2.AddLine(_x0 + (int)((i - 1) * _graph_width / _x_range), _y0 + _graph_height - y1,
								_x0 + (int)(i * _graph_width / _x_range), _y0 + _graph_height - y2);
						if (first_y == -1) first_y = _y0 + _graph_height - y1;
						last_x = _x0 + (int)(i * _graph_width / _x_range);
						last_y = _y0 + _graph_height - y2;
					}
					else
					{
						if (i < _chartItems2.Length)
							graphPath2.AddLine(_x0, _y0 + _graph_height - y1,
								_x0 + _spacer, _y0 + _graph_height - y2);
						if (first_y == -1) first_y = _y0 + _graph_height - y1;
						last_x = _x0 + _spacer;
						last_y = _y0 + _graph_height - y2;
					}
				}

				GraphicsPath graphPathReg2 = new GraphicsPath();
				graphPathReg2.AddPath(graphPath2, false);
				graphPathReg2.AddLine(last_x, last_y, last_x, _y0 + _graph_height);
				graphPathReg2.AddLine(_x0, _y0 + _graph_height, _x0, first_y);
				r1.Exclude(graphPathReg2);
				graphics.FillRegion(new SolidBrush(Color.FromArgb(120, Color.LawnGreen)), r1);

				Region r2 = new Region(graphPathReg2);
				r2.Exclude(graphPathReg1);
				graphics.FillRegion(new SolidBrush(Color.FromArgb(120, Color.Red)), r2);

				graphics.DrawPath(PenBlue, graphPath2);
				graphics.DrawPath(PenRed, graphPath1);
			}

			//Draw Total under legend
			//grp.SmoothingMode = SmoothingMode.AntiAlias;
		}
		#endregion

		#region Render(stream)
		public void Render(System.IO.Stream stream, ImageFormat format)
		{
			// Step 1. Create a bitmap that measures W x H pixels 
			using (Bitmap bitmap = new Bitmap(this.ImageWidth, this.ImageHeight + _deltaHeight))
			{
				// Step 2. Create a Graphics object for drawing to the bitmap
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					// Step 3. Use Graphics methods to draw the image
					Render(graphics);
				}
				// Clean up before returning

				// Write the image to the stream
				bitmap.Save(stream, format);

			}// Clean up before returning
		}
		#endregion
	}
}
