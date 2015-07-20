using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Mediachase.Ibn.Web.Drawing.Gantt
{
	public class GanttStyle : Style
	{
		private static string[] _propertyNames = { "background-color", "shape", "color", "border-style", "border-width", "border-color", "fill-style", "hatch-style", "width", "height", "alpha" };
		private static string[] _defaultValues = { "Transparent", "Rectangle", "Black", "Solid", "1", "Black", "None", "ForwardDiagonal", "15", "15", "255" };
		private static string[] _notInheritableNames = { "shape" };

		private Color _backgroundColor;
		private Color _foregroundColor;
		private Color _borderColor;
		private ShapeStyle _shape;
		private FillStyle _fillStyle;
		private HatchStyle _hatchStyle;
		private BorderStyle _borderStyle;
		private float _borderWidth;
		private float _width;
		private float _height;
		private int _alpha;

		#region * Properties *
		public Color BackgroundColor
		{
			get { return GetColor(_backgroundColor); }
			set { _backgroundColor = value; }
		}

		public Color ForegroundColor
		{
			get { return GetColor(_foregroundColor); }
			set { _foregroundColor = value; }
		}

		public Color BorderColor
		{
			get { return GetColor(_borderColor); }
			set { _borderColor = value; }
		}

		public ShapeStyle ShapeStyle
		{
			get { return _shape; }
			set { _shape = value; }
		}

		public FillStyle FillStyle
		{
			get { return _fillStyle; }
			set { _fillStyle = value; }
		}

		public HatchStyle HatchStyle
		{
			get { return _hatchStyle; }
			set { _hatchStyle = value; }
		}

		public BorderStyle BorderStyle
		{
			get { return _borderStyle; }
			set { _borderStyle = value; }
		}

		public float BorderWidth
		{
			get { return _borderWidth; }
			set { _borderWidth = value; }
		}

		public float Width
		{
			get { return _width; }
			set { _width = value; }
		}

		public float Height
		{
			get { return _height; }
			set { _height = value; }
		}

		public int Alpha
		{
			get { return _alpha; }
			set { _alpha = value; }
		}
		#endregion

		public GanttStyle()
			: base(_propertyNames, _defaultValues, _notInheritableNames)
		{
		}

		public override void CalculateValues()
		{
			foreach (string name in Properties.Keys)
			{
				string value = Properties[name];
				switch (name)
				{
					case "background-color":
						_backgroundColor = ParseColor(value);
						break;
					case "shape":
						_shape = (ShapeStyle)Enum.Parse(typeof(ShapeStyle), value);
						break;
					case "color":
						_foregroundColor = ParseColor(value);
						break;
					case "border-style":
						_borderStyle = (BorderStyle)Enum.Parse(typeof(BorderStyle), value);
						break;
					case "border-width":
						_borderWidth = float.Parse(value, CultureInfo.InvariantCulture);
						break;
					case "border-color":
						_borderColor = ParseColor(value);
						break;
					case "fill-style":
						_fillStyle = (FillStyle)Enum.Parse(typeof(FillStyle), value);
						break;
					case "hatch-style":
						_hatchStyle = (HatchStyle)Enum.Parse(typeof(HatchStyle), value);
						break;
					case "width":
						_width = float.Parse(value, CultureInfo.InvariantCulture);
						break;
					case "height":
						_height = float.Parse(value, CultureInfo.InvariantCulture);
						break;
					case "alpha":
						_alpha = int.Parse(value, CultureInfo.InvariantCulture);
						break;
				}
			}
		}

		private Color GetColor(Color color)
		{
			if (color != Color.Transparent)
				color = Color.FromArgb(_alpha, color);
			return color;
		}

		private static Color ParseColor(string value)
		{
			Color result = Color.Black;

			if (!string.IsNullOrEmpty(value))
			{
				if (value[0] == '#')
				{
					int argb = int.Parse(value.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					result = Color.FromArgb(argb);
				}
				else
				{
					result = Color.FromName(value);
				}
			}

			return result;
		}
	}
}
