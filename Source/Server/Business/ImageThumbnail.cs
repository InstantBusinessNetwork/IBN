using System;
using Mediachase.IBN.Business.ControlSystem;
using System.Drawing;
using System.Drawing.Imaging;

namespace Mediachase.IBN.Business
{
	[Flags]
	public enum ImageThumbnailMode
	{
		Default = 0,
		SaveProportion = 1,
		SkipSmallImage = 2,
	}
	/// <summary>
	/// Summary description for ImageThumbnail.
	/// </summary>
	public class ImageThumbnail
	{
		private ImageThumbnail()
		{
		}

		private static bool ThumbnailCallback()
		{
			return false;
		}

		public static bool Create(string ContainerKey, int FileId, Size size, ImageFormat imageFormat, System.IO.Stream output)
		{
			return Create(ImageThumbnailMode.Default, ContainerKey, FileId, size, imageFormat, output);
		}

		public static bool Create(ImageThumbnailMode mode, string ContainerKey, int FileId, Size size, ImageFormat imageFormat, System.IO.Stream output)
		{
			try
			{
				BaseIbnContainer bic = BaseIbnContainer.Create("FileLibrary", ContainerKey);
				FileStorage fs = (FileStorage)bic.LoadControl("FileStorage");

				using(System.IO.MemoryStream stream = new System.IO.MemoryStream())
				{
					fs.LoadFile(FileId,stream);

					Image image = Image.FromStream(stream);

					return Create(mode, image, size, imageFormat, output);
				}
			}
			catch(Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex);
				return false;
			}
		}

		public static bool Create(ImageThumbnailMode mode, Image image, Size size, ImageFormat imageFormat, System.IO.Stream output)
		{
			try
			{
				Size realSize = size;

				if((mode & ImageThumbnailMode.SaveProportion)==ImageThumbnailMode.SaveProportion)
				{
          // 2007-04-24 by Oleg Rylin: the old code worked only for 1:1 size proportions
          realSize.Width = (int)(1.0 * image.Width * size.Height / image.Height);
          if (realSize.Width > size.Width)
          {
            realSize.Width = size.Width;
            realSize.Height = (int)(1.0 * image.Height * size.Width / image.Width);
          }
/*					double prop = image.Width / (1.0 * image.Height);

					realSize.Width = prop>1?size.Width:(int)(size.Width*prop);
					realSize.Height = prop>1?(int)(size.Height/prop):size.Height;
 * */
				}

				if((mode & ImageThumbnailMode.SkipSmallImage)==ImageThumbnailMode.SkipSmallImage )
				{
					if(image.Width<size.Width && image.Height< size.Height)
					{
						realSize.Width = image.Width;
						realSize.Height = image.Height;
					}
				}

				Image thumbnailImage = image.GetThumbnailImage(realSize.Width, realSize.Height, new Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);

				using(System.IO.MemoryStream stream = new System.IO.MemoryStream())
				{
					thumbnailImage.Save(stream, imageFormat);
					stream.Position = 0;
					output.Write(stream.GetBuffer(), 0, (int)stream.Length);
				}
			}
			catch(Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex);
				return false;
			}

			return true;
		}

	}
}
