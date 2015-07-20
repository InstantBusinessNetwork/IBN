using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.XPath;

namespace Mediachase.Gantt
{
	public class RelationElement : GanttElement
	{
		public const string ElementName = "relation";
		public const string OriginIdName = "originId";
		public const string TargetIdName = "targetId";

		private string _originId;
		private string _targetId;
		private Element _origin;
		private Element _target;
		private float _arrowSize;

		public string OriginId
		{
			get { return _originId; }
			set { _originId = value; }
		}

		public string TargetId
		{
			get { return _targetId; }
			set { _targetId = value; }
		}

		public Element Origin
		{
			get { return _origin; }
			set { _origin = value; }
		}

		public Element Target
		{
			get { return _target; }
			set { _target = value; }
		}

		internal RelationElement(GanttView view, string id, string type, string tag, string originId, string targetId)
			: base(view, ElementName)
		{
			_originId = originId;
			_targetId = targetId;

			AddAttribute(IdName, id);
			AddAttribute(TypeName, type);
			AddAttribute(TagName, tag);
			AddAttribute(OriginIdName, originId);
			AddAttribute(TargetIdName, targetId);
		}

		internal RelationElement(GanttView view, XPathNavigator node)
			: base(view, node)
		{
			_originId = Attributes[OriginIdName].Value;
			_targetId = Attributes[TargetIdName].Value;
		}

		public override void RenderGanttElement(DrawingContext context, RectangleF rectangle, Brush backgroundBrush, Pen borderPen)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			// Calculate arrow size
			GanttStyle style = this.Style as GanttStyle;
			_arrowSize = Math.Min(style.Width, style.Height);

			GanttElement originElement = _origin as GanttElement;
			GanttElement targetElement = _target as GanttElement;

			RectangleF origin = originElement.Rectangle;
			RectangleF target = targetElement.Rectangle;

			PointF originEnd = new PointF(origin.Right, Convert.ToSingle(Math.Ceiling(origin.Top - 1 + origin.Height / 2)));
			PointF targetStart = new PointF(targetElement.StartX, Convert.ToSingle(Math.Ceiling(target.Top - 1 + target.Height / 2)));

			PointF[] linePoints = null;
			PointF[] arrowPoints = null;

			bool riseY = target.Y > origin.Y;
			float doubleArrowSize = _arrowSize * 2;

			if (targetStart.X - originEnd.X > doubleArrowSize)
			{
				PointF pointer = new PointF(targetStart.X, riseY ? target.Top - 1 : target.Bottom);

				// Direct arrow: 3 points
				linePoints = new PointF[3]
				{
					originEnd,
					new PointF(pointer.X, originEnd.Y),
					new PointF(pointer.X, riseY ? pointer.Y - _arrowSize - 1 : pointer.Y + _arrowSize + 1)
				};

				arrowPoints = CreateArrow(pointer, false, riseY);
			}
			else
			{
				PointF pointer = new PointF(target.Left - 1, targetStart.Y);

				// Back arrow: 6 points
				float intermediateY = riseY ? origin.Bottom + 1 : origin.Top - 2;

				linePoints = new PointF[6]
				{
					originEnd,
					new PointF(originEnd.X + doubleArrowSize, originEnd.Y),
					new PointF(originEnd.X + doubleArrowSize, intermediateY),
					new PointF(pointer.X - doubleArrowSize - 1, intermediateY),
					new PointF(pointer.X - doubleArrowSize - 1, pointer.Y),
					new PointF(pointer.X - _arrowSize - 1, pointer.Y)
				};

				arrowPoints = CreateArrow(pointer, true, true);
			}

			if (linePoints != null)
			{
				if (borderPen != null)
					context.Graphics.DrawLines(borderPen, linePoints);
			}

			if (arrowPoints != null)
			{
				if (backgroundBrush != null)
					context.Graphics.FillPolygon(backgroundBrush, arrowPoints);
				if (borderPen != null)
					context.Graphics.DrawPolygon(borderPen, arrowPoints);
			}
		}

		#region private CreateArrow(...)
		private PointF[] CreateArrow(PointF pointer, bool horizontal, bool rise)
		{
			float dx1;
			float dx2;
			float dy1;
			float dy2;

			if (horizontal)
			{
				if (rise)
				{
					dx1 = -1;
					dx2 = -1;
					dy1 = -1;
					dy2 = 1;
				}
				else
				{
					dx1 = 1;
					dx2 = 1;
					dy1 = -1;
					dy2 = 1;
				}
			}
			else
			{
				if (rise)
				{
					dx1 = -1;
					dx2 = 1;
					dy1 = -1;
					dy2 = -1;
				}
				else
				{
					dx1 = -1;
					dx2 = 1;
					dy1 = 1;
					dy2 = 1;
				}
			}

			return new PointF[3]
			{
				new PointF(pointer.X, pointer.Y),
				new PointF(pointer.X + _arrowSize * dx1, pointer.Y + _arrowSize * dy1),
				new PointF(pointer.X + _arrowSize * dx2, pointer.Y + _arrowSize * dy2),
			};
		}
		#endregion
	}
}
