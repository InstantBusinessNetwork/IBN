using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace Mediachase.Ibn.Web.UI
{
	/// <summary>
	/// Summary description for Images.
	/// </summary>
	public class Images
	{
		public Images()
		{
		}

		public static Image ProcessImage(Mediachase.FileUploader.Web.McHttpPostedFile file, out string extension)
		{
			return ProcessImage(file, 150, 150, out extension);
		}

		public static Image ProcessImage(Mediachase.FileUploader.Web.McHttpPostedFile file, int maxWidth, int maxHeight, out string extension)
		{
			extension = file.FileName.Substring(file.FileName.LastIndexOf("."));
			Image img = System.Drawing.Image.FromStream(file.InputStream);

			if (img.Height > maxHeight || img.Width > maxWidth)
			{
				System.IO.Stream mem = new System.IO.MemoryStream();

				ImageCodecInfo imageCodecInfo = GetEncoderInfo("image/jpeg");
				Encoder encoder = Encoder.Quality;
				EncoderParameters encoderParameters = new EncoderParameters(1);
				encoderParameters.Param[0] = new EncoderParameter(encoder, 100L);

				CorrectImageSize(maxWidth, maxHeight, img).Save(mem, imageCodecInfo, encoderParameters);

				img = System.Drawing.Image.FromStream(mem);
				extension = ".jpg";
			}
			return img;
		}

		private static ImageCodecInfo GetEncoderInfo(String mimeType)
		{
			int j;
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			for (j = 0; j < encoders.Length; ++j)
			{
				if (encoders[j].MimeType == mimeType)
					return encoders[j];
			}
			return null;
		}

		public static Image CorrectImageSize(int maxWidth, int maxHeight, Image img)
		{
			Decimal scalepercent = 1M;
			if (img.Height > maxHeight)
				scalepercent = (Decimal)img.Height / (Decimal)maxHeight;
			if (img.Width > maxWidth && img.Width / (Decimal)maxWidth > (Decimal)scalepercent)
				scalepercent = (Decimal)img.Width / (Decimal)maxWidth;

			Decimal newWidth = Decimal.Round((Decimal)img.Width / scalepercent, 0);
			Decimal newHeight = Decimal.Round((Decimal)img.Height / scalepercent, 0);

			return new Bitmap(img, (int)newWidth, (int)newHeight);
		}
	}
}
