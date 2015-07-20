using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Mediachase.Ibn.Web.Drawing.Charting
{
	/// <summary>
	/// Pie charts and bar charts for dashboard reports
	/// </summary>
	abstract public class Chart
	{
		private const int _colorLimit = 16;

		private Color[] _color = { Color.SkyBlue, Color.LimeGreen, Color.MediumOrchid, Color.LightCoral, Color.SteelBlue, Color.YellowGreen, Color.Turquoise, Color.HotPink, Color.Khaki, Color.Tan, Color.DarkSeaGreen, Color.CornflowerBlue, Color.Plum, Color.CadetBlue, Color.PeachPuff, Color.LightSalmon };

		// Represent collection of all data points for the chart
		private ChartItemsCollection _dataPoints = new ChartItemsCollection();
		System.Random rnd = new System.Random();

		// The implementation of this method is provided by derived classes
		public abstract void Draw(Graphics graphics);
		public abstract void Draw(System.IO.Stream stream, ImageFormat format);

		public ChartItemsCollection DataPoints
		{
			get { return _dataPoints; }
		}

		public void SetColor(int index, Color newColor)
		{
			if (index >= _colorLimit)
				throw new ArgumentException("Color index must not be greater than " + _colorLimit, "index");

			_color[index] = newColor;
		}

		public Color GetColor(int index)
		{
			if (index < _colorLimit)
			{
				return _color[index];
			}
			else
			{
				return Color.FromArgb(rnd.Next(256),
					rnd.Next(256),
					rnd.Next(256));
			}
		}
	}
}
