using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;


namespace Mediachase.Ibn.WebTrial
{
    public class AntiRobotImage
    {
        private string[] Fonts = { "Verdana", "Tahoma", "Arial", "Small", "Courier", "Fixedsys", "Garamound" };

        #region Text
        private string text;
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }
        #endregion

        #region Width
        private int width;
        /// <summary>
        /// Gets or sets the image width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }
        #endregion

        #region Height
        private int height;
        /// <summary>
        /// Gets or sets the image height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }
        #endregion

        #region Image
        private Bitmap image;
        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        public Bitmap Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }
        #endregion

        #region FontFamily
        private string fontFamily;
        public string FontFamily
        {
            get
            {
                return fontFamily;
            }
            set
            {
                fontFamily = value;
            }
        }
        #endregion

        #region Random
        // For generating random numbers.
        private Random random = new Random();
        #endregion

        #region GenerateImage()
        private void GenerateImage()
        {
            // Create a new 32-bit bitmap image.
            Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            // Create a graphics object for drawing.
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, Width, Height);

            // Fill in the background.
            HatchBrush hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.LightGray, Color.White);
            g.FillRectangle(hatchBrush, rect);

            // Set up the text font.
            SizeF size;
            float fontSize = height;
            Font font;
            // Adjust the font size until the text fits within the image.
            do
            {
                fontSize--;
                font = new Font(FontFamily, fontSize, FontStyle.Bold);
                size = g.MeasureString(this.text, font);
            } while (size.Width > rect.Width);

            // Set up the text format.
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            // Create a path using the text and warp it randomly.
            GraphicsPath path = new GraphicsPath();
            path.AddString(this.text, font.FontFamily, (int)font.Style, font.Size, rect, format);
            float v = 4F;
            PointF[] points =
			{
				new PointF(this.random.Next(rect.Width) / v, this.random.Next(rect.Height) / v),
				new PointF(rect.Width - this.random.Next(rect.Width) / v, this.random.Next(rect.Height) / v),
				new PointF(this.random.Next(rect.Width) / v, rect.Height - this.random.Next(rect.Height) / v),
				new PointF(rect.Width - this.random.Next(rect.Width) / v, rect.Height - this.random.Next(rect.Height) / v)
			};
            Matrix matrix = new Matrix();
            matrix.Translate(0F, 0F);
            path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);

            // Draw the text.
            hatchBrush = new HatchBrush(HatchStyle.LargeConfetti, Color.LightGray, Color.DarkGray);
            g.FillPath(hatchBrush, path);

            // Add some random noise.
            int m = Math.Max(rect.Width, rect.Height);
            for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
            {
                int x = this.random.Next(rect.Width);
                int y = this.random.Next(rect.Height);
                int w = this.random.Next(m / 50);
                int h = this.random.Next(m / 50);
                g.FillEllipse(hatchBrush, x, y, w, h);
            }

            // Clean up.
            font.Dispose();
            hatchBrush.Dispose();
            g.Dispose();

            // Set the image.
            this.image = bitmap;
        }
        #endregion

        #region Constructors()

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AntiRobotImage"/> class.
        /// </summary>
        /// <param name="Text">The text.</param>
        /// <param name="Width">The width.</param>
        /// <param name="Height">The height.</param>
        public AntiRobotImage(string Text, int Width, int Height)
        {
            this.Text = Text;
            this.Width = Width;
            this.Height = Height;
            this.FontFamily = Fonts[random.Next(0, Fonts.Length - 1)];
            GenerateImage();
        }
        #endregion
    }
}