using System.Drawing;

namespace Mediachase.Ibn.Web.Drawing.Charting
{
	/// <summary>
	/// Represents a data point in a chart.
	/// </summary>
	public class ChartItem
	{
		private string _label;
		private string _description;
		private float _value;
		private Color _color;
		private float _startPos;
		private float _sweepSize;

		private ChartItem() { }

		public ChartItem(string label, string description, float data, float start, float sweep, Color color)
		{
			_label = label;
			_description = description;
			_value = data;
			_startPos = start;
			_sweepSize = sweep;
			_color = color;
		}

		public string Label
		{
			get { return _label; }
			set { _label = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public float Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public Color ItemColor
		{
			get { return _color; }
			set { _color = value; }
		}

		public float StartPos
		{
			get { return _startPos; }
			set { _startPos = value; }
		}

		public float SweepSize
		{
			get { return _sweepSize; }
			set { _sweepSize = value; }
		}
	}
}
