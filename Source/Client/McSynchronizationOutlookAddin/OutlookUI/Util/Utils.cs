using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace OutlookAddin.OutlookUI
{
    static class Utils
    {
        public static void SetHandlers(Control control, MouseEventHandler mouseDownEventHandler, MouseEventHandler mouseUpEventHandler, MouseEventHandler mouseMoveEventHandler)
        {
            control.MouseDown -= mouseDownEventHandler;
            control.MouseUp -= mouseUpEventHandler;
            control.MouseMove -= mouseMoveEventHandler;

            control.MouseDown += mouseDownEventHandler;
            control.MouseUp += mouseUpEventHandler;
            control.MouseMove += mouseMoveEventHandler;

            foreach (Control childControl in control.Controls)
            {
                SetHandlers(childControl, mouseDownEventHandler, mouseUpEventHandler, mouseMoveEventHandler);
            }
        }

        public static void RemoveHandlers(Control control, MouseEventHandler mouseDownEventHandler, MouseEventHandler mouseUpEventHandler, MouseEventHandler mouseMoveEventHandler)
        {
            control.MouseDown -= mouseDownEventHandler;
            control.MouseUp -= mouseUpEventHandler;
            control.MouseMove -= mouseMoveEventHandler;

            foreach (Control childControl in control.Controls)
            {
                RemoveHandlers(childControl, mouseDownEventHandler, mouseUpEventHandler, mouseMoveEventHandler);
            }
        }

        public static Point GetAbsolute(Point point, Control sourceControl, Control rootControl)
        {
            Point tempPoint = new Point();
            for (Control iterator = sourceControl; iterator != rootControl; iterator = iterator.Parent)
            {
                tempPoint.Offset(iterator.Left, iterator.Top);
            }

            tempPoint.Offset(point.X, point.Y);
            return tempPoint;
        }
    }
}
